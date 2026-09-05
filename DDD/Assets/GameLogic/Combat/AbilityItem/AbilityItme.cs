using NFramework.Core.Live;
using NFramework.Module.EntityModule;
using NFramework.Module.EventModule;
using NFramework.Module.Math;
using UnityEngine;

namespace NFramework.Module.Combat
{
    public class AbilityItem : Entity, IAwakeSystem<SkillExecution, ExecuteClipData>
    {
        public IAbilityExecution AbilityExecution;
        public Entity ability;
        public EffectApplyType effectApplyType;
        public TransformComponent TransformComponent => GetComponent<TransformComponent>();
        public AABBComponent AABBComponent => GetComponent<AABBComponent>();

        public void Awake(SkillExecution a, ExecuteClipData b)
        {
            AbilityExecution = a;
            var @event = new SyncCreateAbilityItem(this.Id);
            NFROOT.Instance.GetModule<EventM>().D.Fire(ref @event);
            AddComponent<TransformComponent>();
            AddComponent<AbilityItemCollisionExecuteComponent, ExecuteClipData>(b);
            AABB aabb = new AABB(new Vector2(-1, -1), new Vector2(1, 1));
            AddComponent<AABBComponent, AABB>(aabb);

            this.ability = this.AbilityExecution.Ability;

            if (ability == null)
            {
                return;
            }

            var abilityEffects = ability.GetComponent<AbilityEffectComponent>().AbilityEffectList;
            foreach (var abilityEffect in abilityEffects)
            {
                if (abilityEffect.effect.DecoratorList != null)
                {
                    foreach (var EffectDecorator in abilityEffect.effect.DecoratorList)
                    {
                        if (EffectDecorator is DamageReduceWithTargetCountDecorator reduceWithTargetCountDecorator)
                        {
                            AddComponent<AbilityItemTargetCounterComponent>();
                        }
                    }
                }
            }
            
        }
    }
}