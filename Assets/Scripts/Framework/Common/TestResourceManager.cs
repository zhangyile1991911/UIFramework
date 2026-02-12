using Common;
using Cysharp.Threading.Tasks;
using UnityEngine;


namespace Common
{
    public class TestResourceManager : IResourceManager
    {
        public async UniTask<T> LoadAssetAsync<T>(string path) where T : UnityEngine.Object
        {
            var real = path.Replace(".prefab", "").Replace("Assets/Resources/","");
            var result = await Resources.LoadAsync<T>(real).ToUniTask();
            return result as T;
        }
        
        public T LoadAsset<T>(string path) where T : UnityEngine.Object
        {
            var real = path.Replace(".prefab", "").Replace("Assets/Resources/","");
            var pb = Resources.Load(real);
            return pb as T;
        }
        
    }
}
