using Common.Server.DTOs;
using Cysharp.Threading.Tasks;

namespace Common.Server
{
    public static partial class ServerAPI
    {
        public class Minigames
        {
            /// <summary>
            /// Get all minigames
            /// GET /api/minigames
            /// </summary>
            public static async UniTask<GetGamesResponse> GetGamesAsync(string bearerToken)
            {
                var url = $"{BaseUrl}/minigames";
                var response = await ServerRequest.GetRequest<GetGamesResponse>(url, bearerToken);
                return response.Data;
            }

            /// <summary>
            /// Get all minigame modes
            /// GET /api/minigames/modes/all
            /// </summary>
            public static async UniTask<GetGameModesResponse> GetAllModesAsync(string bearerToken)
            {
                var url = $"{BaseUrl}/minigames/modes/all";
                var response = await ServerRequest.GetRequest<GetGameModesResponse>(url, bearerToken);
                return response.Data;
            }
        }
    }
}