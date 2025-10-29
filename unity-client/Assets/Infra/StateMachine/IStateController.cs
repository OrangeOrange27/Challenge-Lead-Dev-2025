using Infra.ControllersTree.Abstractions;

namespace Infra.StateMachine
{
	public interface IStateController<in TPayload> : IControllerWithPayloadAndReturn<TPayload, IStateMachineInstruction>
	{
	}
	
	public interface IStateController : IStateController<EmptyPayloadType>
	{
	}
}