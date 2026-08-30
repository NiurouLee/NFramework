namespace NFramework.ModuleSystem.Combat
{
    public class ExecutionEffectTimerTriggerComponent : Entity, IAwakeSystem, IDestroySystem
    {
        public long startTime;
        public long endTime;
        public long startTimer;
        public long endTimer;

        public void Awake()
        {
            if (this.startTime > 0)
            {
                this.startTimer = GetSystem<TimerSystem>()
                    .NewOnceTimer(startTime, GetParent<ExecutionEffect>().StartTriggerEffect);
            }
            else
            {
                GetParent<ExecutionEffect>().StartTriggerEffect();
            }

            if (endTime > 0)
            {
                endTimer = GetSystem<TimerSystem>().NewOnceTimer(endTime, GetParent<ExecutionEffect>().EndEffect);
            }
        }

        public void Destroy()
        {
            GetSystem<TimerSystem>().RemoveTimer(startTimer);
            GetSystem<TimerSystem>().RemoveTimer(endTimer);
        }
    }
}