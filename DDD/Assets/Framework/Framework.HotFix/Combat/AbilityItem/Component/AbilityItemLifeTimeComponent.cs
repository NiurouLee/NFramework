
namespace NFramework.ModuleSystem.Combat
{
    public class AbilityItemLifeTimeComponent : Entity, IAwakeSystem<long>, IDestroySystem
    {
        public long lifeTimer;

        public void Awake(long p1)
        {
            lifeTimer = GetSystem<TimerSystem>().NewOnceTimer(p1, this.Dispose);
        }


        public void Destroy()
        {
            GetSystem<TimerSystem>().RemoveTimer(lifeTimer);
        }
    }
}