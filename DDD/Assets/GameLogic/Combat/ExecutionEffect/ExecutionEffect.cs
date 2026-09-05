using NFramework.Core.Live;
using NFramework.Module.EntityModule;

namespace NFramework.Module.Combat
{
    public partial class ExecutionEffect : Entity, IAwakeSystem<ExecuteClipData>
    {
        public ExecuteClipData executeClipData;
        public SkillExecution Execution => GetParent<SkillExecution>();

        public void Awake(ExecuteClipData a)
        {
            this.executeClipData = a;
            var clipType = a.ExecuteClipType;
            if (clipType == ExecuteClipType.ActionEvent)
            {
                if (this.executeClipData.actionEventData.ActionEventType == FireEventType.FiltrationTarget)
                {
                    AddComponent<ExecutionEffectFiltrationTargetComponent>();
                }
                if (this.executeClipData.actionEventData.ActionEventType == FireEventType.AssignEffect)
                {
                    AddComponent<ExecutionEffectAssignToTargetComponent>();
                }
                if (executeClipData.actionEventData.ActionEventType == FireEventType.TriggerNewExecution)
                {
                    AddComponent<ExecutionEffectTriggerNewExecutionComponent>();
                }
            }
            if (clipType == ExecuteClipType.CollisionExecute)
            {
                AddComponent<ExecutionEffectSpawnCollisionComponent>();
            }
            if (clipType == ExecuteClipType.Animation)
            {
                AddComponent<ExecutionEffectAnimationComponent>();
            }
            if (clipType == ExecuteClipType.ParticleEffect)
            {
                AddComponent<ExecutionEffectParticleEffectComponent>();
            }
        }

        public void BeginExecute()
        {
            var clipType = executeClipData.ExecuteClipType;
            if (clipType == ExecuteClipType.ActionEvent)
            {
                AddComponent<ExecutionEffectTimerTriggerComponent>().startTime = (long)(executeClipData.StartTime * 1000);
            }
            else if (executeClipData.Duration > 0)
            {
                var com = AddComponent<ExecutionEffectTimerTriggerComponent>();
                com.startTime = (long)(executeClipData.StartTime * 1000);
                com.endTime = (long)(executeClipData.EndTime * 1000f);
            }
            if (GetComponent<ExecutionEffectTimerTriggerComponent>() == null)
            {
                StartTriggerEffect();
            }
        }

        public void StartTriggerEffect()
        {
            var clipType = executeClipData.ExecuteClipType;
            if (clipType == ExecuteClipType.ActionEvent)
            {
                if (executeClipData.actionEventData.ActionEventType == FireEventType.FiltrationTarget)
                {
                    AddComponent<ExecutionEffectFiltrationTargetComponent>();
                }
                if (executeClipData.actionEventData.ActionEventType == FireEventType.AssignEffect)
                {
                    AddComponent<ExecutionEffectAssignToTargetComponent>();
                }
                if (executeClipData.actionEventData.ActionEventType == FireEventType.TriggerNewExecution)
                {
                    AddComponent<ExecutionEffectTriggerNewExecutionComponent>();
                }
            }
            if (clipType == ExecuteClipType.CollisionExecute)
            {
                AddComponent<ExecutionEffectSpawnCollisionComponent>();
            }
            if (clipType == ExecuteClipType.Animation)
            {
                AddComponent<ExecutionEffectAnimationComponent>();
            }
            if (clipType == ExecuteClipType.ParticleEffect)
            {
                GetComponent<ExecutionEffectParticleEffectComponent>().OnTriggerExecutionEffect(this);
            }
        }

        public void EndEffect()
        {
            var clipType = executeClipData.ExecuteClipType;
            if (clipType == ExecuteClipType.ActionEvent)
            {
                if (executeClipData.actionEventData.ActionEventType == FireEventType.FiltrationTarget)
                {
                }
                if (executeClipData.actionEventData.ActionEventType == FireEventType.AssignEffect)
                {

                }
                if (executeClipData.actionEventData.ActionEventType == FireEventType.TriggerNewExecution)
                {
                }
            }

            if (clipType == ExecuteClipType.CollisionExecute)
            {
            }
            if (clipType == ExecuteClipType.Animation)
            {

            }
            if (clipType == ExecuteClipType.ParticleEffect)
            {
                GetComponent<ExecutionEffectParticleEffectComponent>().OnTriggerExecutionEffectEnd(this);
            }
        }
    }
}