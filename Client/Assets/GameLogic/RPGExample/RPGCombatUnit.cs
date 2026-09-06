using NFramework.ModuleSystem;
using UnityEngine;

namespace Game.Logic
{
    /// <summary>
    /// RPG Demo 第一阶段用的纯逻辑战斗单元。
    /// 以后接入正式 CombatEntity 后，这个类可以退化为对 CombatEntity 的包装/占位。
    /// </summary>
    public class RPGCombatUnit : Entity
    {
        public string DisplayName { get; private set; }
        public bool IsHero { get; private set; }
        public int MaxHp { get; private set; }
        public int Hp { get; private set; }
        public int Attack { get; private set; }
        public bool IsDead => this.Hp <= 0;

        public void Init(string inName, bool inIsHero, int inMaxHp, int inAttack)
        {
            this.DisplayName = inName;
            this.IsHero = inIsHero;
            this.MaxHp = Mathf.Max(1, inMaxHp);
            this.Hp = this.MaxHp;
            this.Attack = Mathf.Max(1, inAttack);
        }

        public void TakeDamage(int inDamage)
        {
            if (this.IsDead)
            {
                return;
            }

            this.Hp = Mathf.Max(0, this.Hp - Mathf.Max(0, inDamage));
        }

        public void Heal(int inCure)
        {
            if (this.IsDead)
            {
                return;
            }

            this.Hp = Mathf.Min(this.MaxHp, this.Hp + Mathf.Max(0, inCure));
        }
    }
}
