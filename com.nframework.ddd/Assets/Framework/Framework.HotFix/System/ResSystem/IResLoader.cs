using System;
using Cysharp.Threading.Tasks;

namespace NFramework.ModuleSystem
{
    public interface IResLoader
    {
        public T Load<T>(string inAssetID) where T : UnityEngine.Object;
        public UniTask<T> LoadAsync<T>(string inAssetID) where T : UnityEngine.Object;
        public void LoadAsync<T>(string inAssetID, Action<T> callback) where T : UnityEngine.Object;
        public void Free<T>(T inObj) where T : UnityEngine.Object;
        public void Free(string inAssetID);
    }
}