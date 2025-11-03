using Cysharp.Threading.Tasks;

namespace Core.EntryPoint
{
    public interface IBeforeAuthInitialize
    {
        UniTask InitializeBeforeAuth();
    }
}