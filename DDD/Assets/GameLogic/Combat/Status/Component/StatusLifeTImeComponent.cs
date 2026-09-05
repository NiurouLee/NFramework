using NFramework.Module.EntityModule;
using NFramework.Module.TimerModule;
using NFramework.Core.Live;

namespace NFramework.Module.Combat
{
    public class StatusLifeTimeComponent : Entity, IAwakeSystem, IDestroySystem
    {
        public long LifeTimer;
        public void Awake()
        {
            long lifeTime = GetParent<StatusAbility>().duration;
            LifeTimer = NFROOT.Instance.GetModule<TimerM>().NewOnceTimer(lifeTime, GetParent<StatusAbility>().EndAbility);
        }

        public void Destroy()
        {
            NFROOT.Instance.GetModule<TimerM>().RemoveTimer(LifeTimer);
        }

    }
}