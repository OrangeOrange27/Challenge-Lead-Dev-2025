using Cysharp.Threading.Tasks;

namespace Core.EntryPoint
{
    public interface IAfterAuthInitialize
    {
        UniTask InitializeAfterAuth();
    }
}