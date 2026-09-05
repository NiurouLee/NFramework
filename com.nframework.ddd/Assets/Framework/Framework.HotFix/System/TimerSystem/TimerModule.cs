using System;
using   NFramework;
using   NFramework.ModuleSystem;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// https://www.zhihu.com/question/52968810/answer/1929456142423163724
    /// </summary>
    public class TimerSystem : FrameworkSystemModuleBase, IRendererUpdateSystem
    {
        private HireachicalTimerWheel hireachicalTimerWheel;

        public override void Awake()
        {
            base.Awake();
            hireachicalTimerWheel = new();
        }

        public void RendererUpdate(float deltaTime)
        {
            this.hireachicalTimerWheel?.OnUpdate(deltaTime);
        }


        public long NewOnceTimer(float inTime, Action inAction)
        {
            return AddTimer(inTime, 1, inAction);
        }

        public long AddTimer(float inIntervalInSec, uint inRepateCount, Action inIntervalCallback,
            Action inStartCallback = null, Action inEndCallback = null)
        {
            if (inIntervalInSec <= 0 && inRepateCount == 1)
            {
                inStartCallback?.Invoke();
                inIntervalCallback?.Invoke();
                inEndCallback?.Invoke();
                return 0;
            }

            ulong _intervalInMs = (ulong)(inIntervalInSec * 1000);
            _intervalInMs = _intervalInMs > this.hireachicalTimerWheel.MaxInterval
                ? this.hireachicalTimerWheel.MaxInterval
                : _intervalInMs;
            ulong _totalInMS = _intervalInMs * inRepateCount;
            var _task = NFROOT.I.GetSystem<ObjectPoolSystem>().Alloc<TimerTask>();
            _task.Init(_intervalInMs, _totalInMS, inStartCallback, inIntervalCallback, inEndCallback);
            return this.hireachicalTimerWheel.AddTimerTask(_task);
        }

        public void RemoveTimer(long inTimerTaskID)
        {
            this.hireachicalTimerWheel?.RemoveTimerTask(inTimerTaskID);
        }

        internal void RestTimer(long statusLifeTimer)
        {
        }

        public override void Destroy()
        {
            base.Destroy();
            this.hireachicalTimerWheel?.Clear();
        }
    }
}