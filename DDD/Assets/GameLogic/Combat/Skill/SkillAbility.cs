using System.Collections.Generic;
using NFramework.Core.Live;
using NFramework.Module.EntityModule;
using NFramework.Module.ResModule;

namespace NFramework.Module.Combat
{
    public class Ability : Entity, IAbility, IAwakeSystem<System.Object>
    {
        public bool Spelling { get; set; }
        public CombatEntity Owner => GetParent<CombatEntity>();
        public AbilityConfigObject SkillConfigObject;
        public ExecutionConfigObject ExecutionConfigObject;
        private List<StatusAbility> m_StatusList = new List<StatusAbility>();
        public void Awake(object a)
        {
            SkillConfigObject = a as AbilityConfigObject;
            AddComponent<AbilityEffectComponent, List<Effect>>(SkillConfigObject.EffectList);
            // ExecutionConfigObject = NFROOT.I.G<ResM>().Load<ExecutionConfigObject>(string.Empty);
        }

        public void ActivateAbility()
        {
            Enable = true;

            if (SkillConfigObject.EnableChildStatus)
            {
                foreach (var item in SkillConfigObject.StatusList)
                {
                    var status = Owner.AttachStatus(item.StatusConfigObject.Id);
                    status.Creator = Owner;
                    status.isChildStatus = true;
                    status.childStatusData = item;
                    status.SetParams(item.ParamsDict);
                    status.ActivateAbility();
                    m_StatusList.Add(status);
                }
            }
        }


        public void EndAbility()
        {

            Enable = false;
            if (SkillConfigObject.EnableChildStatus)
            {
                foreach (var item in m_StatusList)
                {
                    item.EndAbility();
                }
                m_StatusList.Clear();
            }
            Dispose();
        }

        /// <summary>
        /// 创建执行体
        /// </summary>
        /// <returns></returns>
        public Entity CreateExecution()
        {
            var execution = Owner.AddChild<SkillExecution, Ability>(this);
            execution.executionConfigObject = ExecutionConfigObject;
            execution.LoadExecutionEffect();
            return execution;
        }
    }


}