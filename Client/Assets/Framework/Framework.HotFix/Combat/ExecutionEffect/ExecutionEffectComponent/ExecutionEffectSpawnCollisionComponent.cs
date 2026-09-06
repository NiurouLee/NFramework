using NFramework.ModuleSystem;

namespace NFramework.ModuleSystem.Combat
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