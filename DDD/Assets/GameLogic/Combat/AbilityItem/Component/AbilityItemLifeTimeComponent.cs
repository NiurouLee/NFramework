using NFramework.Core.Live;
using NFramework.Module.EntityModule;
using NFramework.Module.TimerModule;

namespace NFramework.Module.Combat
{
    public class AbilityItemLifeTimeComponent : Entity, IAwakeSystem<long>, IDestroySystem
    {
        public long lifeTimer;
        public void Awake(long p1)
        {
            lifeTimer = NFROOT.Instance.GetModule<TimerM>().NewOnceTimer(p1, this.Dispose);
        }


        public void Destroy()
        {
            NFROOT.Instance.GetModule<TimerM>().RemoveTimer(lifeTimer);
        }
    }
}