using NFramework.Module.EntityModule;

namespace NFramework.Module.Combat
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