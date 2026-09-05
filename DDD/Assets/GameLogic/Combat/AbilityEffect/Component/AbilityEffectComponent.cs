using System.Collections.Generic;
using NFramework.Core.Live;
using NFramework.Module.EntityModule;

namespace NFramework.Module.Combat
{
    public class AbilityEffectComponent : Entity, IAwakeSystem<List<Effect>>
    {
        public List<AbilityEffect> AbilityEffectList = new List<AbilityEffect>();
        public AbilityEffect DamageAbilityEffect;
        public AbilityEffect CureAbilityEffect;

        public void Awake(List<Effect> a)
        {
            if (a == null)
                return;
            foreach (var item in a)
            {
                AbilityEffect abilityEffect = parent.AddChild<AbilityEffect, Effect>(item);
                this.AddEffect(abilityEffect);

                if (abilityEffect.effect is DamageEffect)
                {
                    DamageAbilityEffect = abilityEffect;
                }

                if (abilityEffect.effect is CureEffect)
                {
                    CureAbilityEffect = abilityEffect;
                }
            }
        }

        public void EnableEffect()
        {
            foreach (var item in AbilityEffectList)
            {
                item.EnableEffect();

            }
        }

        public void AddEffect(AbilityEffect abilityEffect)
        {
            AbilityEffectList.Add(abilityEffect);
        }

        public AbilityEffect GetEffect(int index = 0)
        {
            return AbilityEffectList[index];
        }


        public void TryAssignAllEffectToTarget(CombatEntity target)
        {
            if (AbilityEffectList.Count > 0)
            {
                foreach (var item in AbilityEffectList)
                {
                    item.TryAssignEffectToTarget(target);
                }
            }
        }

        public void TryAssignAllEffectToTarget(CombatEntity target, IActionExecution actionExecution)
        {
            if (AbilityEffectList.Count > 0)
            {
                foreach (var item in AbilityEffectList)
                {
                    item.TryAssignEffectToTarget(target, actionExecution);
                }
            }
        }

        public void TryAssignAllEffectToTarget(CombatEntity target, IAbilityExecution abilityExecution)
        {
            if (AbilityEffectList.Count > 0)
            {
                foreach (var item in AbilityEffectList)
                {
                    item.TryAssignEffectToTarget(target, abilityExecution);
                }
            }
        }

        public void TryAssignAllEffectToTarget(CombatEntity target, AbilityItem abilityItem)
        {
            if (AbilityEffectList.Count > 0)
            {
                foreach (var item in AbilityEffectList)
                {
                    item.TryAssignEffectToTarget(target, abilityItem);
                }
            }
        }

        public void TryAssignEffectToTargetByIndex(CombatEntity target, int index)
        {
            AbilityEffectList[index].TryAssignEffectToTarget(target);
        }
    }

}
