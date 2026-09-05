using UnityEngine;
using NFramework.Module.EntityModule;
using NFramework.Utils;

namespace NFramework.Module.Combat
{

    public enum DamageSource
    {
        Attack,
        Skill,
        Buff,
    }
    /// <summary>
    /// 受伤能力
    /// </summary>
    public class DamageActionAbility : Entity, IActionAbility
    {
        public bool Enable { get; set; }
        public CombatEntity Owner => GetParent<CombatEntity>();

        public bool TryMakeAction(out DamageAction action)
        {
            if (!Enable)
            {
                action = null;
            }
            else
            {
                action = Owner.AddChild<DamageAction>();
                action.ActionAbility = this;
                action.Creator = Owner;
            }
            return Enable;
        }
    }


    /// <summary>
    /// 这是一次具体的受伤行为 
    /// </summary>
    public class DamageAction : Entity, IActionExecution
    {
        public DamageSource DamageSource;
        public int DamageValue;
        public Entity ActionAbility { get; set; }
        public EffectAssignAction SourceAssignAction { get; set; }
        public CombatEntity Creator { get; set; }
        public CombatEntity Target { get; set; }

        public void FinishAction()
        {
            Dispose();
        }

        /// <summary>
        /// 预处理,根据攻击类型计算具体伤害值
        /// </summary>
        private void PreProcess()
        {
            DamageEffect damageEffect = (DamageEffect)SourceAssignAction.AbilityEffect.effect;
            //是否暴击
            bool isCritical = false;

            //如果是普攻
            if (this.DamageSource == DamageSource.Attack)
            {
                //暴击概率
                isCritical = (RandomUtil.RandomRate() / 100f) < Creator.GetComponent<AttributeComponent>().CriticalProbability.Value;
                //攻击方攻击力
                DamageValue = (int)Creator.GetComponent<AttributeComponent>().Attack.Value;
                //受击方防御力
                DamageValue = Mathf.CeilToInt(Mathf.Max(1, DamageValue - Target.GetComponent<AttributeComponent>().Defense.Value));
                if (isCritical)
                {
                    DamageValue = Mathf.CeilToInt(DamageValue * 1.5f);
                }
            }
            if (this.DamageSource == DamageSource.Skill)
            {
                if (damageEffect.CanCrit)
                {
                    isCritical = (RandomUtil.RandomRate() / 100f) < Creator.GetComponent<AttributeComponent>().CriticalProbability.Value;
                }
                DamageValue = SourceAssignAction.AbilityEffect.GetComponent<AbilityEffectDamageComponent>().GetDamageValue();
                DamageValue = Mathf.CeilToInt(Mathf.Max(1, DamageValue - Target.GetComponent<AttributeComponent>().Defense.Value));
                if (isCritical)
                {
                    DamageValue = Mathf.CeilToInt(DamageValue * 1.5f);
                }
            }

            if (DamageSource == DamageSource.Buff)
            {
                if (damageEffect.CanCrit)
                {
                    isCritical = (RandomUtil.RandomRate() / 100f) < Creator.GetComponent<AttributeComponent>().CriticalProbability.Value;
                }
                DamageValue = SourceAssignAction.AbilityEffect.GetComponent<AbilityEffectDamageComponent>().GetDamageValue();
                DamageValue = Mathf.CeilToInt(Mathf.Max(1, DamageValue - Target.GetComponent<AttributeComponent>().Defense.Value));
                if (isCritical)
                {
                    DamageValue = Mathf.CeilToInt(DamageValue * 1.5f);
                }
            }

            AbilityEffectDamageReduceWithTargetCountComponent component = SourceAssignAction.AbilityEffect.GetComponent<AbilityEffectDamageReduceWithTargetCountComponent>();
            if (component != null)
            {
                var targetCounterComponent = SourceAssignAction.AbilityItem.GetComponent<AbilityItemTargetCounterComponent>();
                if (targetCounterComponent != null)
                {
                    var damagePercent = component.GetDamagePercent(targetCounterComponent.targetCounter);
                    DamageValue = Mathf.CeilToInt(DamageValue * damagePercent);
                }
            }

            Creator.TriggerActionPoint(ActionPointType.PreCauseDamage, this);
            Target.TriggerActionPoint(ActionPointType.PreReceiveDamage, this);
        }

        public void ApplyDamage()
        {
            PreProcess();
            Target.ReceiveDamage(this);
            PostProcess();

            if (Target.CheckDead())
            {
                Target.Dead();
            }
            FinishAction();
        }
        private void PostProcess()
        {
            Creator.TriggerActionPoint(ActionPointType.PostCauseDamage, this);
            Target.TriggerActionPoint(ActionPointType.PostReceiveDamage, this);
        }
    }
}