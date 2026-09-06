
namespace NFramework.ModuleSystem.Combat
{
    public class ExecutionEffectParticleEffectComponent : Entity
    {
        public CombatEntity Owner => GetParent<SkillExecution>().Owner;

        public void OnTriggerExecutionEffect(ExecutionEffect executionEffect)
        {
            var @event = new SyncParticleEffect(Owner.Id,
                executionEffect.executeClipData.ParticleEffectData.ParticleEffectName,
                Owner.TransformComponent.Position, Owner.TransformComponent.Rotation);
            GetSystem<EventSystem>().Fire(ref @event);
        }

        public void OnTriggerExecutionEffectEnd(ExecutionEffect executionEffect)
        {
            var @event = new SyncDeleteParticleEffect(Owner.Id,
                executionEffect.executeClipData.ParticleEffectData.ParticleEffectName);
            GetSystem<EventSystem>().Fire(ref @event);
        }
    }
}