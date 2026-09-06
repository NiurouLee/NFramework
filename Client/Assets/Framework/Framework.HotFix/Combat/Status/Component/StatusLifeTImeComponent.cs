
namespace NFramework.ModuleSystem.Combat
{
    public class StatusLifeTimeComponent : Entity, IAwakeSystem, IDestroySystem
    {
        public long LifeTimer;
        public void Awake()
        {
            long lifeTime = GetParent<StatusAbility>().duration;
            GetSystem<TimerSystem>().NewOnceTimer(lifeTime, GetParent<StatusAbility>().EndAbility);
        }

        public void Destroy()
        {
            GetSystem<TimerSystem>().RemoveTimer(LifeTimer);
        }

    }
}