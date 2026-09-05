
using NFramework.Module.EntityModule;

namespace NFramework.Module.Combat
{
    public interface IActionExecution
    {
        public Entity ActionAbility { get; set; }
        public EffectAssignAction SourceAssignAction { get; set; }
        public CombatEntity Creator { get; set; }
        public CombatEntity Target { get; set; }
        public void FinishAction();

    }
}