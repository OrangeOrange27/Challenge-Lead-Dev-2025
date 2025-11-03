using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Infra.ControllersTree;
using Infra.ControllersTree.Abstractions;
using Infra.StateMachine.Utils;
using UnityEngine;

namespace Infra.StateMachine
{
	public class StateMachineController : IControllerWithPayloadAndReturn<IStateMachineInstruction, IStateMachineInstruction>
	{
		protected const int NoMaxHistorySize = -1;
		private static readonly IStateMachineInstruction DefaultResultOfStateMachine = StateMachineInstruction.None;

		private readonly List<IStateMachineInstruction> _stackedInstructions = new();

		private CancellationTokenSource _executeCancellationTokenSource;
		private IStateMachineInstruction _resultStateMachineInstruction;

		protected int MaxHistorySize = NoMaxHistorySize;
		
		private bool IsHistoryExceedMaxSize => MaxHistorySize != NoMaxHistorySize && StackSize > MaxHistorySize;
		private int StackSize => _stackedInstructions.Count;

		public UniTask OnInitialize(IControllerResources resources, CancellationToken token)
		{
			return UniTask.CompletedTask;
		}

		public async UniTask OnStart(IStateMachineInstruction payload, IControllerResources resources,
			IControllerChildren controllerChildren, CancellationToken token)
		{
			if (payload == StateMachineInstruction.None)
				return;

			AddStateAsNext(payload);
		}

		public async UniTask<IStateMachineInstruction> Execute(IControllerResources resources,
			IControllerChildren controllerChildren, CancellationToken token)
		{
			try
			{
				while (_stackedInstructions.Count > 0 && !token.IsCancellationRequested)
					await RunState(controllerChildren, token);
			}
			catch (Exception e)
			{
				Debug.LogError(e);
				throw;
			}
			token.ThrowIfCancellationRequested();

			return _resultStateMachineInstruction ?? DefaultResultOfStateMachine;
		}

		public UniTask OnStop(CancellationToken token)
		{
			return UniTask.CompletedTask;
		}

		public UniTask OnDispose(CancellationToken token)
		{
			return UniTask.CompletedTask;
		}

		public void ChangeActiveState(IStateMachineInstruction stateMachineInstruction)
		{
			AddStateAsNext(stateMachineInstruction);
			KillCurrentState();
		}

		public void AddStateAsNext(IStateMachineInstruction stateMachineInstruction)
		{
			if (stateMachineInstruction.MultipleInstructions.Any())
			{
				_stackedInstructions.PushRange(stateMachineInstruction.MultipleInstructions);
				return;
			}

			_stackedInstructions.Push(stateMachineInstruction);
		}

		public List<IStateMachineInstruction> GetStateMachineInstructionsInStack()
		{
			return _stackedInstructions;
		}

		public IStateMachineInstruction GetCurrentInstruction()
		{
			return GetStateMachineInstructionsInStack().Peek();
		}

		private void KillCurrentState()
		{
			if (_executeCancellationTokenSource == null) throw new ControllerException("No active state");

			_executeCancellationTokenSource?.Cancel();
		}

		private async UniTask RunState(IControllerChildren controllerChildren, CancellationToken cancellationToken)
		{
			var instructionToTake = _stackedInstructions.Peek();
			if (instructionToTake == StateMachineInstruction.None)
			{
				_stackedInstructions.Pop();
				return;
			}

			if (instructionToTake.GoBackRequested)
			{
				_stackedInstructions.Pop(); //Pop GoBackInstruction
				while (_stackedInstructions.Any() && _stackedInstructions.Peek().IgnoreInHistoryRequested)
					_stackedInstructions.Pop();
				return;
			}

			if (instructionToTake.ExitRequested)
			{
				if (instructionToTake.NestedInstruction != null)
					_resultStateMachineInstruction = instructionToTake.NestedInstruction;

				_stackedInstructions.Clear();
				return;
			}

			if (IsHistoryExceedMaxSize) RemoveOldestInstructionFromStack();


			if (instructionToTake.NextStateFactory != null)
			{
				_executeCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

				var currentState = instructionToTake.NextStateFactory.Invoke(controllerChildren);

				try
				{
					await currentState.Initialize(_executeCancellationTokenSource.Token);
					_executeCancellationTokenSource.Token.ThrowIfCancellationRequested();

					await instructionToTake.StartDelegate.Invoke(currentState, _executeCancellationTokenSource.Token);
					_executeCancellationTokenSource.Token.ThrowIfCancellationRequested();

					if (currentState is IControllerRunnerReturn<IStateMachineInstruction> runnerWithReturnValue)
					{
						var nextInstruction =
							await runnerWithReturnValue.Execute(_executeCancellationTokenSource.Token);
						_executeCancellationTokenSource.Token.ThrowIfCancellationRequested();

						AddResultStateAsNext(nextInstruction);
					}
					else
					{
						throw new StateMachineException(
							$"State {currentState.GetType().FullName} has not correct controller type. It should have return value with IStateMachineInstruction. Try to implement IStateController");
					}
				}
				catch (Exception e)
				{
					if (!e.GetBaseException().IsOperationCanceledException())
					{
						throw;
					}

					//todo: probably a good idea would be to make error handling configurable
					AddResultStateAsNext(StateMachineInstruction.GoBack);
				}
				finally
				{
					_executeCancellationTokenSource.Dispose();
					_executeCancellationTokenSource = null;
				}

				if (!instructionToTake.DontAwaitStopAndDisposeRequested)
					await StopAndDispose(currentState);
				else
					StopAndDispose(currentState).Forget();

				return;
			}

			throw new StateMachineException("Returned instruction has no configuration to decide what to do next");

			async UniTask StopAndDispose(IControllerRunnerBase currentState)
			{
				try
				{
					await currentState.TryRunThroughLifecycleToStop(cancellationToken);
				}
				catch (Exception e)
				{
					throw; 				//todo: handle
				}

				try
				{
					await currentState.TryRunThroughLifecycleToDispose(cancellationToken);
				}
				catch (Exception e)
				{
					throw;				//todo: handle
				}
			}

			void AddResultStateAsNext(IStateMachineInstruction stateMachineInstructionToAdd)
			{
				if (stateMachineInstructionToAdd.GoBackRequested)
					//reverse for require to remove latest instruction in case of duplicated instructions
					for (var i = _stackedInstructions.Count - 1; i >= 0; i--)
						if (_stackedInstructions[i] == instructionToTake)
						{
							_stackedInstructions.RemoveAt(i);
							break;
						}

				AddStateAsNext(stateMachineInstructionToAdd);
			}
		}

		private void RemoveOldestInstructionFromStack()
		{
			_ = _stackedInstructions.PopBottom();
		}
	}
}