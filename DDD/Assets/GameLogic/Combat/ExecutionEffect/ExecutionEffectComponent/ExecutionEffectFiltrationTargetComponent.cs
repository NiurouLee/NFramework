
using System.Collections.Generic;
using NFramework.Core.Collections;
using NFramework.Module.EntityModule;

namespace NFramework.Module.Combat
{
    public class ExecutionEffectFiltrationTargetComponent : Entity
    {
        public CombatEntity Owner => GetParent<SkillExecution>().Owner;

        public void OnTriggerExecutionEffect(ExecutionEffect executionEffect)
        {
            SkillExecution parentExecution = parent.GetParent<SkillExecution>();

            parentExecution.TargetList.Clear();
            var list = this.SelectCombat(executionEffect.executeClipData);
            parentExecution.TargetList.AddRange(list);
            list.Dispose();
        }
        public List<CombatEntity> SelectCombat(ExecuteClipData executeClipData)
        {
            return FiltrationTarget.GetTargetList(Owner.TransformComponent, executeClipData.actionEventData.Distance, executeClipData.actionEventData.TagType);
        }
    }

}