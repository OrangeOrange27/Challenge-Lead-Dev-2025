using System;

namespace Infra.Disposables
{
    public class Disposable : IDisposable
    {
        private Action _actionToCallAtDispose;

        public Disposable(Action actionToCallAtDispose)
        {
            _actionToCallAtDispose = actionToCallAtDispose;
        }

        public void Dispose()
        {
            _actionToCallAtDispose?.Invoke();
            _actionToCallAtDispose = null;
        }
    }
}