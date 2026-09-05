
using NFramework.Core.Live;
using NFramework.Module.EntityModule;
namespace NFramework.Module.Combat
{
    public class AbilityEffectDecoratosComponent : Entity, IAwakeSystem
    {
        public Effect Effect => GetParent<AbilityEffect>().effect;
        public void Awake()
        {
            if (Effect.DecoratorList != null)
            {
                foreach (var item in Effect.DecoratorList)
                {
                    if (item is DamageReduceWithTargetCountDecorator)
                    {
                        Parent.AddComponent<AbilityEffectDamageReduceWithTargetCountComponent>();
                    }
                }
            }
        }
    }
}