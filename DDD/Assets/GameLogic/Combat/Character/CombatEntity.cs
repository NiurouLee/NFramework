using System;
using NFramework.Core.Live;
using NFramework.Module.EntityModule;
using NFramework.Module.EventModule;
using NFramework.Module.Math;
using UnityEngine;

namespace NFramework.Module.Combat
{
    /// <summary>
    /// 一个战斗单元，根据需求挂载不同的Ability
    /// </summary>
    public class CombatEntity : Entity, IAwakeSystem
    {
        public HealthPoint CurrentHealth;
        public ActionControlType ActionControlType;
        /// <summary>
        /// 效果分配能力
        /// </summary>
        public EffectAssignActionAbility EffectAssignActionAbility;
        /// <summary>
        /// 添加状态能力
        /// </summary>
        public AddStatusActionAbility AddStatusActionAbility;
        /// <summary>
        /// 释放技能能力
        /// </summary>
        public SpellSkillActionAbility SpellSkillActionAbility;
        /// <summary>
        /// 释放物品能力
        /// </summary>
        public SpellItemActionAbility SpellItemActionAbility;
        /// <summary>
        /// 伤害能力
        /// </summary>
        public DamageActionAbility DamageActionAbility;
        /// <summary>
        /// 治疗能力
        /// </summary>
        public CureActionAbility CureActionAbility;
        /// <summary>
        /// 技能执行体
        /// </summary>
        public SkillExecution SpellingSkillExecution;
        /// <summary>
        /// Transform
        /// </summary>
        public TransformComponent TransformComponent => GetComponent<TransformComponent>();
        /// <summary>
        ///RVO  
        /// </summary>
        public OrcaComponent OrcaComponent => GetComponent<OrcaComponent>();
        /// <summary>
        /// 动画组件
        /// </summary>
        public AnimationComponent AnimationComponent => GetComponent<AnimationComponent>();
        /// <summary>
        /// 属性组件 类似于AttributeSet
        /// </summary>
        public AttributeComponent AttributeComponent => GetComponent<AttributeComponent>();
        /// <summary>
        /// AABB组件
        /// </summary>
        public AABBComponent AABBComponent => GetComponent<AABBComponent>();
        /// <summary>
        /// 标签组件
        /// </summary>
        public TagComponent TagComponent => GetComponent<TagComponent>();

        /// <summary>
        /// 普攻组件
        /// </summary>
        public AttackSpellAbility AttackSpellAbility => GetComponent<AttackSpellAbility>();

        public void Awake()
        {
            var @event = new SyncCreateCombat(Id);
            NFROOT.Instance.GetModule<EventM>().D.Publish(ref @event);
            AddComponent<TransformComponent>();
            AddComponent<OrcaComponent>();
            AddComponent<AnimationComponent>();
            AABB aabb = new AABB(new Vector2(-1, -1), new Vector2(1, 1));
            AddComponent<AABBComponent, AABB>(aabb);
            AddComponent<AttributeComponent>();
            AddComponent<ActionPointComponent>();
            AddComponent<ConditionComponent>();

            AddComponent<MotionComponent>();

            AddComponent<StatusComponent>();
            AddComponent<SkillComponent>();
            AddComponent<ExecutionComponent>();
            AddComponent<ItemComponent>();

            AddComponent<SpellSkillComponent>();
            AddComponent<JoystickComponent>();

            CurrentHealth = AddChild<HealthPoint>();

            EffectAssignActionAbility = AttachAction<EffectAssignActionAbility>();

            AddStatusActionAbility = AttachAction<AddStatusActionAbility>();

            SpellSkillActionAbility = AttachAction<SpellSkillActionAbility>();

            SpellItemActionAbility = AttachAction<SpellItemActionAbility>();

            DamageActionAbility = AttachAction<DamageActionAbility>();

            CureActionAbility = AttachAction<CureActionAbility>();

            OrcaComponent.AddAgent2D(TransformComponent.Position);

            ListenActionPoint(ActionPointType.PostReceiveDamage, e =>
            {
                var damageAction = e as DamageAction;
                var syncDamage = new SyncDamage(this.Id, damageAction.DamageValue);
                NFROOT.Instance.GetModule<EventM>().D.Publish(ref syncDamage);
            });

            ListenActionPoint(ActionPointType.PostReceiveCure, e =>
            {
                var cureAction = e as CureAction;
                var syncCure = new SyncCure(this.Id, cureAction.CureValue);
                NFROOT.Instance.GetModule<EventM>().D.Publish(ref syncCure);
            });
        }


        public void Dead()
        {
            var syncDeleteCombat = new SyncDeleteCombat(this.Id);
            NFROOT.Instance.GetModule<EventM>().D.Publish(ref syncDeleteCombat);
            GetParent<CombatContext>().RemoveCombat(this.Id);
        }


        /// <summary>
        /// 接收伤害
        /// </summary>
        /// <param name="actionExecution"></param>
        public void ReceiveDamage(IActionExecution actionExecution)
        {
            var damageAction = actionExecution as DamageAction;
            CurrentHealth.Minus(damageAction.DamageValue);
        }
        /// <summary>
        /// 接受治疗
        /// </summary>
        /// <param name="actionExecution"></param>
        public void ReceiveCure(IActionExecution actionExecution)
        {
            var cureAction = actionExecution as CureAction;
            CurrentHealth.Add(cureAction.CureValue);
        }

        public bool CheckDead()
        {
            return CurrentHealth.Value <= 0;
        }

        //能力
        public T AttachAbility<T>(object configObject) where T : Entity, IAbility, IAwakeSystem<object>
        {
            var ability = AddChild<T, object>(configObject);
            ability.AddComponent<AbilityLevelComponent>();
            return ability;
        }


        //行动
        public T AttachAction<T>() where T : Entity, IActionAbility, new()
        {
            var action = AddChild<T>();
            action.AddComponent<ActionComponent, Type>(typeof(T));
            action.Enable = true;
            return action;
        }


        public StatusAbility AttachStatus(int statusId)
        {
            return GetComponent<StatusComponent>().AttachStatus(statusId);
        }

        public StatusAbility GetStatus(int statusId, int index = 0)
        {
            return GetComponent<StatusComponent>().GetStatus(statusId, index);
        }

        public void OnStatueRemove(StatusAbility statusAbility)
        {
            GetComponent<StatusComponent>().OnStatusRemove(statusAbility);
        }
        public bool HasStatus(int statusId)
        {
            return GetComponent<StatusComponent>().HasStatus(statusId);
        }

        public void OnStatuesChanged(StatusAbility statusAbility)
        {
            GetComponent<StatusComponent>().OnStatuesChanged(statusAbility);
        }


        public Ability AttachSkill(int skillId)
        {
            return GetComponent<SkillComponent>().AttachSkill(skillId);
        }

        public Ability GetSkill(int skillId)
        {
            return GetComponent<SkillComponent>().GetSkill(skillId);
        }


        public ExecutionConfigObject AttachExecution(int executionId)
        {
            return GetComponent<ExecutionComponent>().AttachExecution(executionId);
        }

        public ExecutionConfigObject GetExecution(int executionID)
        {
            return GetComponent<ExecutionComponent>().GetExecution(executionID);
        }

        public ItemAbility AttachItem(int itemID)
        {
            return GetComponent<ItemComponent>().AttachItem(itemID);
        }

        public ItemAbility GetItem(int itemID)
        {
            return GetComponent<ItemComponent>().GetItem(itemID);
        }


        #region ActionPoint
        public void ListenActionPoint(ActionPointType type, Action<Entity> action)
        {
            GetComponent<ActionPointComponent>().AddListener(type, action);
        }

        public void UnListenActionPoint(ActionPointType type, Action<Entity> action)
        {
            GetComponent<ActionPointComponent>().RemoveListener(type, action);
        }

        public void TriggerActionPoint(ActionPointType type, Entity action)
        {
            GetComponent<ActionPointComponent>().TriggerActionPoint(type, action);
        }
        #endregion


        public void ListenCondition(ConditionType type, Action action, object obj = null)
        {
            GetComponent<ConditionComponent>().AddListener(type, action, obj);
        }

        public void UnListenCondition(ConditionType type, Action action)
        {
            GetComponent<ConditionComponent>().RemoveListener(type, action);
        }
    }
}