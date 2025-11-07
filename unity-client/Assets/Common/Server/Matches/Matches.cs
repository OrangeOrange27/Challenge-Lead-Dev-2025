using Common.Server.DTOs;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

namespace Common.Server
{
    public static partial class ServerAPI
    {
        public class Matches
        {
            /// <summary>
            /// Enter a match
            /// POST /api/matches/enter
            /// </summary>
            public static async UniTask<EnterMatchResponse> EnterMatchAsync(string gameId, string mode,
                string bearerToken)
            {
                var url = $"{BaseUrl}/matches/enter";
                var requestData = new EnterMatchRequest
                {
                    gameId = gameId,
                    playerId = null, // backend sets from token
                    mode = mode
                };
                var jsonBody = JsonConvert.SerializeObject(requestData);

                var response = await ServerRequest.PostRequest<EnterMatchResponse>(url, jsonBody, bearerToken);

                return response.Data;
            }

            /// <summary>
            /// Submit a match score
            /// POST /api/matches/:id/submit
            /// </summary>
            public static async UniTask<SubmitScoreResponse> SubmitScoreAsync(string matchId, int score,
                string bearerToken)
            {
                var url = $"{BaseUrl}/matches/{matchId}/submit";
                var requestData = new SubmitScoreRequest
                {
                    playerId = null, // backend sets from token
                    score = score
                };
                var jsonBody = JsonConvert.SerializeObject(requestData);

                var response = await ServerRequest.PostRequest<SubmitScoreResponse>(url, jsonBody, bearerToken);

                return response.Data;
            }
            
            /// <summary>
            /// Get leaderboard for a match
            /// GET /api/matches/:id/leaderboard
            /// </summary>
            public static async UniTask<GetLeaderboardResponse> GetLeaderboardAsync(string matchId, string bearerToken)
            {
                var url = $"{BaseUrl}/matches/{matchId}/leaderboard";

                var response = await ServerRequest.GetRequest<GetLeaderboardResponse>(url, bearerToken);
                return response.Data;
            }

            /// <summary>
            /// Claim reward for a match
            /// POST /api/matches/:id/claim
            /// </summary>
            public static async UniTask<ClaimRewardResponse> ClaimRewardAsync(string matchId, string bearerToken)
            {
                var url = $"{BaseUrl}/matches/{matchId}/claim";

                // POST body can be empty since backend takes playerId from token
                var response = await ServerRequest.PostRequest<ClaimRewardResponse>(url, "{}", bearerToken);
                return response.Data;
            }
        }
    }
}