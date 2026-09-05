using NFramework.Module.EntityModule;

namespace NFramework.Module.Combat
{
    public class ExecutionEffectAssignToTargetComponent : Entity
    {
        public CombatEntity Owner => GetParent<SkillExecution>().Owner;

        public void OnTriggerExecutionEffect(ExecutionEffect executionEffect)
        {
            SkillExecution parentExecution = Parent.GetParent<SkillExecution>();

            if (parentExecution.TargetList.Count > 0)
            {
                if (executionEffect.executeClipData.actionEventData.EffectApply == EffectApplyType.AllEffects)
                {
                    foreach (var item in parentExecution.TargetList)
                    {
                        parentExecution.Ability.GetComponent<AbilityEffectComponent>().TryAssignAllEffectToTarget(item, parentExecution);
                    }
                }
                else
                {
                    foreach (var item in parentExecution.TargetList)
                    {
                        parentExecution.Ability.GetComponent<AbilityEffectComponent>()
                        .TryAssignEffectToTargetByIndex(item, (int)executionEffect.executeClipData.actionEventData.EffectApply - 1);
                    }
                }
            }
        }
    }
}