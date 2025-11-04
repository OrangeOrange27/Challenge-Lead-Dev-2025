using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Common.Utils
{
    // simple input handling for demo purposes
    // in real project we should have a proper input system
    // and support for mobile/touch inputs
    public static class InputUtils
    {
        public static async UniTask<Vector2> WaitForPlayerInputAsync(CancellationToken cancellationToken)
        {
            await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), cancellationToken: cancellationToken);

            return Input.mousePosition;
        }
    }
}