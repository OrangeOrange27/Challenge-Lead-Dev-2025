using Common.Authentication.Providers;
using Core.SplashScreen;
using Cysharp.Threading.Tasks;

namespace Core.EntryPoint
{
    public class GameInitManager
    {
        private readonly SplashSceneView _splashSceneView;
        private readonly IPlayerDataService _playerDataService;
        
        public GameInitManager(IPlayerDataService playerDataService, SplashSceneView splashSceneView)
        {
            _playerDataService = playerDataService;
            _splashSceneView = splashSceneView;
        }

        public async UniTask Login()
        {
            await _playerDataService.LoginWithProvider(AuthProvider.Guest);
            _splashSceneView.ShowLoadingCompleted();
        }
    }
}