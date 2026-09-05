using NFramework.Module.EntityModule;

namespace NFramework.Module.Combat
{
    public class ExecutionEffectAnimationComponent : Entity
    {
        public CombatEntity Owner => GetParent<SkillExecution>().Owner;

        public void OnTriggerExecutionEffect(ExecutionEffect executionEffect)
        {
            executionEffect.Execution.Owner.AnimationComponent.PlayAnimation(executionEffect.executeClipData.AnimationData.AnimationType);
        }
    }
}