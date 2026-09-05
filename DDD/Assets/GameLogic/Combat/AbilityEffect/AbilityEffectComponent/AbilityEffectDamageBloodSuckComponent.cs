using NFramework.Module.EntityModule;
using NFramework.Core.Live;

namespace NFramework.Module.Combat
{
    public class AbilityEffectDamageBloodSuckComponent : Entity, IAwakeSystem, IDestroySystem
    {
        public CombatEntity Owner => GetParent<AbilityEffect>().Owner;
        public void Awake()
        {
            Owner.DamageActionAbility.AddComponent<DamageBloodSuckComponent>();
        }

        public void Destroy()
        {
            var component = Owner.DamageActionAbility.GetComponent<DamageBloodSuckComponent>();
            if (component != null)
            {
                component.Dispose();
            }
        }
    }
}