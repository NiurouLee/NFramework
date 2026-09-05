using NFramework.ModuleSystem;

using UnityEngine;

namespace NFramework.ModuleSystem.Combat
{
    /// <summary>
    /// 施法组件
    /// </summary>
    public class SpellSkillActionAbility : Entity, IActionAbility
    {
        public CombatEntity Owner => GetParent<CombatEntity>();

        /// <summary>
        /// 构造一个预览Action
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public bool TryMakeAction(out SpellSkillAction action)
        {
            if (!Enable)
            {
                action = null;
            }
            else
            {
                action = Owner.AddChild<SpellSkillAction>();
                action.ActionAbility = this;
                action.Creator = Owner;
            }
            return Enable;
        }
    }

    /// <summary>
    /// 施法行为
    /// </summary>
    public class SpellSkillAction : Entity, IActionExecution, IUpdateSystem
    {
        public Ability SkillAbility { get; set; }
        public SkillExecution SkillExecution { get; set; }
        public CombatEntity InputTarget { get; set; }
        public Vector3 InputPoint;
        public float InputDirection;
        public Entity ActionAbility { get; set; }
        public EffectAssignAction SourceAssignAction { get; set; }
        public CombatEntity Creator { get; set; }
        public CombatEntity Target { get; set; }

        public void FinishAction()
        {
            Dispose();
        }

        /// <summary>
        /// 预览前处理
        /// </summary>
        private void PreProcess()
        {
            Creator.TriggerActionPoint(ActionPointType.PreSpell, this);
        }
        /// <summary>
        /// 开始施法
        /// </summary>
        /// <param name="actionOccupy"></param>
        public void SpellSkill(bool actionOccupy = true)
        {
            PreProcess();

            //预览如果成功选择了目标，那就创建ability的Execution
            if (InputTarget != null)
            {
                SkillExecution.TargetList.Add(InputTarget);
                SkillExecution = (SkillExecution)SkillAbility.CreateExecution();
                SkillExecution.ActionOccupy = actionOccupy;
            }
            SkillExecution.InputPoint = this.InputPoint;
            SkillExecution.InputDirection = InputDirection;
            SkillExecution.BeginExecute();
        }

        public void Update()
        {
            if (SkillExecution != null)
            {
                if (SkillExecution.IsDisposed)
                {
                    PostProcess();
                    FinishAction();
                }
            }
        }

        private void PostProcess()
        {
            Creator.TriggerActionPoint(ActionPointType.PostSpell, this);
        }

        public void Update(float deltaTime)
        {
        }
    }
}