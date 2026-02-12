using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Common
{
    public interface IResourceManager
    {
        UniTask<T> LoadAssetAsync<T>(string path) where T : UnityEngine.Object;
        T LoadAsset<T>(string path) where T : UnityEngine.Object;
    }
}