using System;

namespace NFramework.ModuleSystem.Combat
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
            GetSystem<TimerSystem>().RemoveTimer(noDamageTime);
            parent.GetParent<CombatEntity>().UnListenActionPoint(ActionPointType.PostReceiveDamage, WhenReceiveDamage);
        }

        public void StartListen(Action whenNoDamageInTimeCallback)
        {
            noDamageTime = GetSystem<TimerSystem>().NewOnceTimer(time, whenNoDamageInTimeCallback);
        }

        private void WhenReceiveDamage(Entity combatAction)
        {
            GetSystem<TimerSystem>().RemoveTimer(noDamageTime);
        }
    }
}