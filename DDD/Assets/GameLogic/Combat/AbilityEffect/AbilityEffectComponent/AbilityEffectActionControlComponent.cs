using NFramework.Core.Live;
using NFramework.Module.EntityModule;

namespace NFramework.Module.Combat
{
    public class AbilityEffectActionControlComponent : Entity, IAwakeSystem, IDestroySystem
    {
        public ActionControlEffect ActionControlEffect => (ActionControlEffect)GetParent<AbilityEffect>().effect;
        public CombatEntity Owner => GetParent<AbilityEffect>().Owner;
        public StatusAbility OwnerAbility => (StatusAbility)GetParent<AbilityEffect>().OwnerAbility;

        public void Awake()
        {
            Owner.OnStatuesChanged(OwnerAbility);
        }

        public void Destroy()
        {
            Owner.OnStatuesChanged(OwnerAbility);
        }
    }
}