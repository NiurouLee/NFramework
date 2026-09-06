using NFramework.ModuleSystem;

namespace NFramework.ModuleSystem.Combat
{
    public interface IAbility
    {
        public bool Enable { get; set; }
        public CombatEntity Owner { get; }
        public void ActivateAbility();
        public void EndAbility();
        public Entity CreateExecution();
    }
}