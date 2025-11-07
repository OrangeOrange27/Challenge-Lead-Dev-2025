using Common.Models.Economy;
using Common.Server.DTOs;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Common.Server
{
    public static partial class ServerAPI
    {
        private const string BaseUrl = "http://localhost:8080/api";

        public class Player
        {
            /// <summary>
            /// Retrieves the player data using the Bearer token.
            /// GET: /player/profile
            /// </summary>
            /// <param name="bearerToken">The Bearer token for authorization (access token).</param>
            /// <returns>Returns the player data as a string.</returns>
            public static async UniTask<T> GetPlayerDataAsync<T>(string bearerToken)
            {
                var url = $"{BaseUrl}/player/profile";

                var response = await ServerRequest.GetRequest<T>(url, bearerToken);

                if (!response.IsSuccess)
                {
                    Debug.LogError(response.ErrorMessage);
                    return default;
                }

                return response.Data;
            }

            /// <summary>
            /// Updates only the player's currency balance.
            /// PATCH: /player/balance
            /// </summary>
            /// <param name="amount">Amount to add (can be negative to subtract).</param>
            /// <param name="currency">Currency type (Cash or Gems).</param>
            /// <param name="bearerToken">Bearer token for authorization.</param>
            public static async UniTask<bool> UpdatePlayerBalanceAsync(int amount, CurrencyType currency,
                string bearerToken)
            {
                var url = $"{BaseUrl}/player/balance";

                var payload = new
                {
                    amount,
                    currencyType = CurrencyAdapter.ToBackend(currency) // Maps client enum to backend string
                };

                var jsonBody = JsonConvert.SerializeObject(payload);
                var response = await ServerRequest.PatchRequest<object>(url, jsonBody, bearerToken);

                if (!response.IsSuccess)
                {
                    Debug.LogError($"Failed to update balance: {response.ErrorMessage}");
                    return false;
                }

                return true;
            }

            /// <summary>
            /// Fetches the player's history from the backend.
            /// GET: /player/history?limit={limit}
            /// </summary>
            /// <param name="bearerToken">Bearer token for authorization.</param>
            /// <param name="limit">Optional limit of history items to fetch (default 50).</param>
            /// <returns>Returns the grouped player history response.</returns>
            public static async UniTask<PlayerHistoryResponse> GetPlayerHistoryAsync(string bearerToken, int limit = 50)
            {
                var url = $"{BaseUrl}/player/history?limit={limit}";

                var response = await ServerRequest.GetRequest<PlayerHistoryResponse>(url, bearerToken);

                if (!response.IsSuccess)
                {
                    Debug.LogError($"Failed to fetch player history: {response.ErrorMessage}");
                    return default;
                }

                return response.Data;
            }
        }
    }
}