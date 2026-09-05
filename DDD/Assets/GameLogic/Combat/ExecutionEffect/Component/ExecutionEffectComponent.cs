using System.Collections.Generic;
using NFramework.Core.Live;
using NFramework.Module.EntityModule;

namespace NFramework.Module.Combat
{
    public class ExecutionEffectComponent : Entity, IAwakeSystem
    {
        public List<ExecutionEffect> executionEffectList = new List<ExecutionEffect>();

        public void Awake()
        {
            if (GetParent<SkillExecution>().executionConfigObject == null)
            {
                return;
            }
            foreach (var effect in GetParent<SkillExecution>().executionConfigObject.ExecuteClipDataList)
            {
                ExecutionEffect executionEffect = Parent.AddChild<ExecutionEffect, ExecuteClipData>(effect);
                AddEffect(executionEffect);
            }
        }

        public void AddEffect(ExecutionEffect executionEffect)
        {
            executionEffectList.Add(executionEffect);
        }

        public void BeginExecute()
        {
            foreach (var item in executionEffectList)
            {
                item.BeginExecute();
            }
        }
    }
}