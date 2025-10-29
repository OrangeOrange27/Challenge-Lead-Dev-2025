namespace Infra.ControllersTree.Implementations
{
    public enum ControllerStatus : byte
    {
        Created,
        Initializing,
        FailedAtInitialize,
        Initialized,
        Starting,
        FailedAtStart,
        Started,
        Executing,
        FailedAtExecution,
        Executed,
        Stopping,
        FailedAtStop,
        Stopped,
        Disposing,
        Disposed
    }
}