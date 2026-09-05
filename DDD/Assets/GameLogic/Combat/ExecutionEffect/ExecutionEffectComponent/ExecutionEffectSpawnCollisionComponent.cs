using NFramework.Module.EntityModule;

namespace NFramework.Module.Combat
{
    public class ExecutionEffectSpawnCollisionComponent : Entity
    {
        public CombatEntity Owner => GetParent<SkillExecution>().Owner;

        public void OnTriggerExecutionEffect(ExecutionEffect executionEffect)
        {
            this.Parent.GetParent<SkillExecution>().SpawnCollisionItem(executionEffect.executeClipData);
        }
    }
}