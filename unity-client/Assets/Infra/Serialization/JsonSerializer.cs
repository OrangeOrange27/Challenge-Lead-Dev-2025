using System;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Infra.Serialization
{
    public class JsonSerializer : ISerializer
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public JsonSerializer(JsonSerializerSettings jsonSerializerSettings)
        {
            _jsonSerializerSettings = jsonSerializerSettings;
        }

        public T Deserialize<T>(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return default;
            }

            if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(value, _jsonSerializerSettings);
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("{0} | data deserialization error - type: {1}, error: {2}", GetType(),
                    typeof(T).Name, e);
            }

            return default;
        }

        public string Serialize<T>(T value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            if (value is string strValue)
            {
                return strValue;
            }
            else
            {
                return JsonConvert.SerializeObject(value, _jsonSerializerSettings);
            }
        }

        public async UniTask<T> DeserializeAsync<T>(string value)
        {
            return await UniTask.RunOnThreadPool(() => Deserialize<T>(value));
        }

        public async UniTask<string> SerializeAsync<T>(T value)
        {
            return await UniTask.RunOnThreadPool(() => Serialize<T>(value));
        }
    }
}