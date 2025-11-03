using Cysharp.Threading.Tasks;

namespace Infra.AssetManagement.DataProvider.Storage
{
    public interface IDataStorage
    {
        string Get(string key);
        void Set(string key, string value);
        UniTask SetAsync(string key, string value);

        void DeleteAll();
    }
}