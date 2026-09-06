using System;
using Cysharp.Threading.Tasks;

namespace NFramework.Boot
{
    public interface IJITServices
    {
        // 事件
        public event Action<string> OnStepChange;
        public event Action<string> OnError;

        public UniTask<bool> StartJITUpdate();

        public UniTask<bool> LoadAOT();

        public UniTask<bool> LoadJIT();

        public UniTask<bool> EnterMainEntry();
        
        public void Release();
    }
}