using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Common.Server
{
    public static partial class ServerAPI
    {
        public struct LoginData
        {
            [JsonProperty("deviceId")] public string DeviceId { get; set; }
        }

        public struct LoginResponse
        {
            [JsonProperty("token")] public string Token { get; set; }
            [JsonProperty("playerId")] public string PlayerId { get; set; }
        }
        
        public class Login
        {
            /// <summary>
            /// Authenticates the user using the specified login data.
            /// POST: /auth/login
            /// </summary>
            /// <param name="loginData">The login data struct to be sent in the request body.</param>
            /// <returns>Returns the server response as a string.</returns>
            public static async UniTask<LoginResponse> LoginAsync(LoginData loginData)
            {
                var url = $"{BaseUrl}/auth/login";
                var jsonBody = JsonConvert.SerializeObject(loginData);

                var response = await ServerRequest.PostRequest<LoginResponse>(url, jsonBody);

                if (!response.IsSuccess)
                {
                    Debug.LogError(response.ErrorMessage);
                    return default;
                }

                return response.Data;
            }
        }
    }
}