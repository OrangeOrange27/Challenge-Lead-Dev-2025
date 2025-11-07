using Newtonsoft.Json;
using UnityEngine;

namespace Common.Server
{
    /// <summary>
    /// Class to match the server's response structure for deserialization.
    /// </summary>
    public class ResponseModel<T>
    {
        /// <summary>
        /// Indicates whether the API call was successful, based on the "status" field.
        /// </summary>
        [JsonProperty("success")] public bool IsSuccess { get; set; }

        /// <summary>
        /// The data returned from the API, deserialized from the "data" field.
        /// </summary>
        [JsonProperty("data")] public T Data { get; set; }

        /// <summary>
        /// The error message if the API call failed, based on the "error" field.
        /// </summary>
        [JsonProperty("error")] public string ErrorMessage { get; set; }

        /// <summary>
        /// Static method to create a success response model.
        /// </summary>
        /// <param name="data">The data to be returned in the success response.</param>
        /// <returns>A ResponseModel with IsSuccess set to true and the provided data.</returns>
        public static ResponseModel<T> Success(T data)
        {
            return new ResponseModel<T>
            {
                IsSuccess = true,
                Data = data
            };
        }

        /// <summary>
        /// Static method to create an error response model.
        /// </summary>
        /// <param name="errorMessage">The error message to be returned.</param>
        /// <returns>A ResponseModel with IsSuccess set to false and the provided error message.</returns>
        public static ResponseModel<T> Error(string errorMessage)
        {
            return new ResponseModel<T>
            {
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }

        /// <summary>
        /// Static method to create a response model based on the server response.
        /// </summary>
        /// <param name="serverResponse">The raw JSON response from the server.</param>
        /// <returns>A ResponseModel with IsSuccess, Data, and ErrorMessage set appropriately.</returns>
        public static ResponseModel<T> FromServerResponse(string serverResponse)
        {
            ResponseModel<T> responseModel;

            try
            {
                responseModel = JsonConvert.DeserializeObject<ResponseModel<T>>(serverResponse);
            }
            catch (JsonException ex)
            {
                Debug.LogError(ex);
                return Error("Failed to deserialize response.");
            }


            if (responseModel == null)
            {
                return Error("Can't deserialize response");
            }

            return responseModel.IsSuccess ? Success(responseModel.Data) : Error(responseModel.ErrorMessage);
        }
    }
}
