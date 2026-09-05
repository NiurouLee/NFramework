using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NFramework.Module.EntityModule;

namespace NFramework.Module.Combat
{
    public class AttackSpellAbility : Entity, IActionAbility
    {
        public bool Enable { get; set; }
        public CombatEntity Owner => GetParent<CombatEntity>();

        public bool TryMakeAction(out AttackAction action)
        {
            if (!Enable)
            {
                action = null;
            }
            else
            {
                action = Owner.AddChild<AttackAction>();
                action.ActionAbility = this;
                action.Creator = Owner;
            }
            return Enable;
        }
    }

    /// <summary>
    /// 普攻
    /// </summary>
    public class AttackAction : Entity, IActionExecution
    {
        /// <summary>
        /// 普攻能力
        /// </summary>
        public Entity ActionAbility { get; set; }
        /// <summary>
        /// 效果赋予行动源
        /// </summary>
        public EffectAssignAction SourceAssignAction { get; set; }

        /// <summary>
        /// 行动实体
        /// </summary>
        public CombatEntity Creator { get; set; }

        /// <summary>
        /// 目标对象
        /// </summary>
        public CombatEntity Target { get; set; }

        public void FinishAction()
        {
            this.Dispose();
        }

        /// <summary>
        /// 前置处理 
        /// </summary>
        private void PreProcess()
        {
            Creator.TriggerActionPoint(ActionPointType.PreGiveAttack, this);
            Target.GetComponent<ActionPointComponent>().TriggerActionPoint(ActionPointType.PreReceiveAttack, this);
        }

        private async Task ApplyAttackAwait()
        {
            this.PreProcess();
            await Task.Delay(1000);
            ApplyAttack();
            await Task.Delay(1000);
            PostProcess();
            FinishAction();
        }

        public void ApplyAttack()
        {

        }

        public void PostProcess()
        {
            Creator.TriggerActionPoint(ActionPointType.PostGiveAttack, this);
            Target.GetComponent<ActionPointComponent>().TriggerActionPoint(ActionPointType.PostReceiveAttack, this);
        }



    }
}