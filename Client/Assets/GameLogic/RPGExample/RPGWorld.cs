using NFramework.ModuleSystem;
using UnityEngine;

namespace Game.Logic
{
    /// <summary>
    /// RPG Demo 用的战斗 World：作为纯逻辑的 Entity 根节点，
    /// 只保存 Hero/Monster 两个战斗单元，后续接真实 Combat 时把单元替换成 CombatEntity 即可。
    /// </summary>
    public class RPGWorld : World
    {
        public RPGCombatUnit Hero { get; private set; }
        public RPGCombatUnit Monster { get; private set; }

        public RPGCombatUnit CreateHero(string inName, int inMaxHp, int inAttack)
        {
            this.Hero = this.CreateUnit(inName, true, inMaxHp, inAttack);
            return this.Hero;
        }

        public RPGCombatUnit CreateMonster(string inName, int inMaxHp, int inAttack)
        {
            this.Monster = this.CreateUnit(inName, false, inMaxHp, inAttack);
            return this.Monster;
        }

        private RPGCombatUnit CreateUnit(string inName, bool inIsHero, int inMaxHp, int inAttack)
        {
            var unit = this.AddChild<RPGCombatUnit>();
            unit.Init(inName, inIsHero, inMaxHp, inAttack);
            return unit;
        }

        public void ClearBattle()
        {
            if (this.Hero != null)
            {
                this.RemoveChild(this.Hero.Id);
                this.Hero = null;
            }

            if (this.Monster != null)
            {
                this.RemoveChild(this.Monster.Id);
                this.Monster = null;
            }
        }

        /// <summary>
        /// 日志模拟表现层的普攻：先播“表现”，再结算伤害。
        /// </summary>
        public void Attack(RPGCombatUnit inAttacker, RPGCombatUnit inTarget)
        {
            if (inAttacker == null || inTarget == null || inAttacker.IsDead || inTarget.IsDead)
            {
                return;
            }

            Log($"[表现] {inAttacker.DisplayName} 播放攻击动作，朝 {inTarget.DisplayName} 发起普攻");

            int damage = Random.Range(inAttacker.Attack / 2, inAttacker.Attack + 1);
            inTarget.TakeDamage(damage);

            Log($"[结算] {inAttacker.DisplayName} 对 {inTarget.DisplayName} 造成 {damage} 伤害，" +
                $"{inTarget.DisplayName} HP {inTarget.Hp}/{inTarget.MaxHp}");

            if (inTarget.IsDead)
            {
                Log($"[结算] {inTarget.DisplayName} 生命值归零，死亡");
            }
        }

        private void Log(string inMessage)
        {
            this.GetSystem<LoggerSystem>()?.Log?.Print($"[RPG Demo] {inMessage}");
        }
    }
}
