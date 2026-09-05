using System.Collections.Generic;
using NFramework.Core.Live;
using NFramework.Module.EntityModule;


namespace NFramework.Module.Combat
{
    //AbilityEffect是挂在Ability上的
    public class AbilityEffect : Entity, IAwakeSystem<Effect>
    {
        public Effect effect;
        public Entity OwnerAbility => (Entity)Parent;
        public CombatEntity Owner => ((IAbility)OwnerAbility).Owner;

        public void Awake(Effect a)
        {
            effect = a;
            if (effect is AddStatusEffect)
            {
                this.AddComponent<AbilityEffectAddStatusComponent>();
            }
            if (effect is ClearAllStatusEffect)
            {

            }
            if (effect is CureEffect)
            {
                AddComponent<AbilityEffectCureComponent>();
            }

            if (effect is DamageEffect)
            {
                AddComponent<AbilityEffectDamageComponent>();
            }
            if (effect is RemoveStatusEffect)
            {

            }
            AddComponent<AbilityEffectDecoratosComponent>();
        }
        public void EnableEffect()
        {
            if (effect is ActionControlEffect)
            {
                AddComponent<AbilityEffectActionControlComponent>();
            }
            if (effect is AttributeModifyEffect)
            {
                AddComponent<AbilityEffectAttributeModifyComponent>();
            }
            if (effect is CustomEffect)
            { }
            if (effect is DamageBloodSuckEffect)
            {
                AddComponent<AbilityEffectDamageBloodSuckComponent>();
            }
            if (effect is not ActionControlEffect && effect is not AttributeModifyEffect)
            {
                if (effect.EffectTriggerType == EffectTriggerType.Instant)
                {
                    TryAssignEffectToOwner();
                }
                if (effect.EffectTriggerType == EffectTriggerType.Action)
                {
                    AddComponent<AbilityEffectActionTriggerComponent>();
                }

                if (effect.EffectTriggerType == EffectTriggerType.Interval && !string.IsNullOrEmpty(effect.IntervalValueFormula))
                {
                    AddComponent<AbilityEffectIntervalTriggerComponent>();
                }

                if (effect.EffectTriggerType == EffectTriggerType.Condition && !string.IsNullOrEmpty(effect.ConditioNValueFormula))
                {
                    AddComponent<AbilityEffectConditionTriggerComponent>();
                }
            }
        }

        public Dictionary<string, string> GetParamsDict()
        {
            Dictionary<string, string> temp;
            if (OwnerAbility is StatusAbility status)
            {
                temp = status.paramsDict;
                return temp;
            }
            else
            {
                temp = new Dictionary<string, string>();
                temp.Add("自身生命值", Owner.GetComponent<AttributeComponent>().HealthPoint.Value.ToString());
                temp.Add("自身攻击力", Owner.GetComponent<AttributeComponent>().Attack.Value.ToString());
            }
            return temp;
        }

        public void TryAssignEffectToOwner()
        {
            TryAssignEffectToOwner();
        }

        public void TryAssignEffectToTarget(CombatEntity target)
        {
            if (Owner.EffectAssignActionAbility.TryMakeAction(out var action))
            {
                action.Target = target;
                action.SourceAbility = OwnerAbility;
                action.AbilityEffect = this;
                action.ApplyEffectAssign();
            }
        }

        public void TryAssignEffectToTarget(CombatEntity target, IActionExecution actionExecution)
        {
            if (Owner.EffectAssignActionAbility.TryMakeAction(out var action))
            {
                action.Target = target;
                action.SourceAbility = OwnerAbility;
                action.AbilityEffect = this;
                action.ActionExecution = actionExecution;
                action.ApplyEffectAssign();
            }
        }

        public void TryAssignEffectToTarget(CombatEntity target, IAbilityExecution abilityExecution)
        {
            if (Owner.EffectAssignActionAbility.TryMakeAction(out var action))
            {
                action.Target = target;
                action.SourceAbility = OwnerAbility;
                action.AbilityEffect = this;
                action.AbilityExecution = abilityExecution;
                action.ApplyEffectAssign();
            }
        }

        public void TryAssignEffectToTarget(CombatEntity target, AbilityItem abilityItem)
        {
            if (Owner.EffectAssignActionAbility.TryMakeAction(out var action))
            {
                action.Target = target;
                action.SourceAbility = OwnerAbility;
                action.AbilityEffect = this;
                action.AbilityItem = abilityItem;
                action.ApplyEffectAssign();
            }
        }
        /// <summary>
        ///  增加一个效果
        /// </summary>
        /// <param name="action"></param>
        public void StartAssignEffect(EffectAssignAction action)
        {
            if (effect is AddStatusEffect)
            {
                GetComponent<AbilityEffectAddStatusComponent>().OnAssignEffect(action);
            }
            if (effect is ClearAllStatusEffect)
            {
            }
            if (effect is CureEffect)
            {
                GetComponent<AbilityEffectCureComponent>().OnAssignEffect(action);
            }
            if (effect is DamageEffect)
            {
                GetComponent<AbilityEffectDamageComponent>().OnAssignEffect(action);
            }
            if (effect is RemoveStatusEffect)
            {

            }
        }
    }
}