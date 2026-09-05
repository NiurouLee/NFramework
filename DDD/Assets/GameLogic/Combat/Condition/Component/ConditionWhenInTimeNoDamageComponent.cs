using System;
using NFramework.Core.Live;
using NFramework.Module.EntityModule;
using NFramework.Module.TimerModule;

namespace NFramework.Module.Combat
{
    public class ConditionWhenInTimeNoDamageComponent : Entity, IAwakeSystem<long>, IDestroySystem
    {
        public long time;
        private long noDamageTime;
        public void Awake(long a)
        {
            time = a;
            parent.GetParent<CombatEntity>().ListenActionPoint(ActionPointType.PostReceiveDamage, WhenReceiveDamage);
        }

        public void Destroy()
        {
            NFROOT.Instance.GetModule<TimerM>().RemoveTimer(noDamageTime);
            parent.GetParent<CombatEntity>().UnListenActionPoint(ActionPointType.PostReceiveDamage, WhenReceiveDamage);
        }

        public void StartListen(Action whenNoDamageInTimeCallback)
        {
            noDamageTime = NFROOT.Instance.GetModule<TimerM>().NewOnceTimer(time, whenNoDamageInTimeCallback);
        }

        private void WhenReceiveDamage(Entity combatAction)
        {
            NFROOT.Instance.GetModule<TimerM>().RemoveTimer(noDamageTime);
        }


    }
}