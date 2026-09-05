using NFramework.ModuleSystem;

namespace NFramework.ModuleSystem.Combat
{
    public class AbilityEffectIntervalTriggerComponent : Entity, IAwakeSystem
    {
        public Effect Effect => GetParent<AbilityEffect>().effect;
        public string InterValueFormula => Effect.IntervalValueFormula;
        public long IntervalTimer;
        public void Awake()
        {
        }
    }
}