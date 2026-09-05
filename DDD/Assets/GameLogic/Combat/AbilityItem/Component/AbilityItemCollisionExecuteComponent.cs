using NFramework.Core.Live;
using NFramework.Module.EntityModule;
namespace NFramework.Module.Combat
{
    public class AbilityItemCollisionExecuteComponent : Entity, IAwakeSystem<ExecuteClipData>
    {
        public ExecuteClipData ExecuteClipData;
        public CollisionExecuteData CollisionExecuteData => ExecuteClipData.CollisionExecuteData;

        public void Awake(ExecuteClipData executeClipData)
        {
            if (CollisionExecuteData.ActionData.ActionEventType == FireEventType.AssignEffect)
            {
                GetParent<AbilityItem>().effectApplyType = CollisionExecuteData.ActionData.EffectApply;
            }
            if (CollisionExecuteData.ActionData.ActionEventType == FireEventType.TriggerNewExecution)
            {

            }
        }
    }
}