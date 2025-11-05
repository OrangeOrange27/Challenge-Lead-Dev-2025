using System;
using System.Threading;
using Common.Minigames;
using Common.Minigames.Models;
using Common.Utils;
using Cysharp.Threading.Tasks;
using Infra.AssetManagement.ViewLoader;
using Infra.ControllersTree.Abstractions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Minigames.Match
{
    // Ideally this logic should also be split into states, but for demo purpose it's ok to keep it simple
    public class MatchMinigameFlow : IMinigameFlow
    {
        private const int MaxPointsForReaction = 10_000;
        private const int MaxPointsForPrecision = 10_000;

        private const float DelayAfterTouch = 2f;
        private static readonly Vector2 WaitingTimeRange = new(1f, 3f);

        private readonly IViewLoader<IMatchMinigameView> _viewLoader;

        private IMatchMinigameView _view;
        
        public MatchMinigameFlow(IViewLoader<IMatchMinigameView> viewLoader)
        {
            _viewLoader = viewLoader;
        }

        public UniTask OnInitialize(IControllerResources resources, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public async UniTask OnStart(MinigameModel payload, IControllerResources resources,
            IControllerChildren controllerChildren,
            CancellationToken token)
        {
            _view = await _viewLoader.Load(resources, token, null);
        }

        public async UniTask<MinigameResult> Execute(IControllerResources resources,
            IControllerChildren controllerChildren, CancellationToken token)
        {
            var waitingTime = Random.Range(WaitingTimeRange.x, WaitingTimeRange.y);
            await UniTask.Delay(TimeSpan.FromSeconds(waitingTime), cancellationToken: token);

            var startTime = DateTime.UtcNow;
            _view.Target.anchoredPosition = UiUtils.GetRandomLocalPosition(_view.PlayingField, _view.Target);
            _view.Target.gameObject.SetActive(true);
            _view.Text.text = "GO!!!";

            var inputPos = await InputUtils.WaitForPlayerInputAsync(token);
            var reactionTime = (DateTime.UtcNow - startTime).TotalSeconds;
            _view.Text.text = $"Reaction Time: {reactionTime:F3} s";

            await UniTask.WaitForSeconds(DelayAfterTouch, cancellationToken: token);

            return CreateResult(inputPos, reactionTime);
        }

        public UniTask OnStop(CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnDispose(CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        private MinigameResult CreateResult(Vector2 inputPosition, double reactionTime)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _view.PlayingField,
                inputPosition,
                null,
                out var localInputPoint
            );

            float precisionPoints = CalculatePrecisionPoints(_view.Target.anchoredPosition, localInputPoint);
            float reactionPoints = CalculateReactionPoints((float)reactionTime);

            return new MinigameResult
            {
                PointsForPrecision = Mathf.RoundToInt(precisionPoints),
                PointsForReaction = Mathf.RoundToInt(reactionPoints)
            };
        }

        private static int CalculatePrecisionPoints(Vector2 pointA, Vector2 pointB, float falloff = 400f)
        {
            var distance = Vector2.Distance(pointA, pointB);
            var score = MaxPointsForPrecision * Mathf.Exp(-distance / falloff);
            return Mathf.RoundToInt(score);
        }

        private static int CalculateReactionPoints(float reactionTime, float falloff = 1f)
        {
            var score = MaxPointsForReaction * Mathf.Exp(-reactionTime / falloff);
            return Mathf.RoundToInt(score);
        }
    }
}