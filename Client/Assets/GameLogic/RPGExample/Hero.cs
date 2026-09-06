using NFramework.ModuleSystem;
using UnityEngine;

namespace Game.Logic
{
    /// <summary>
    /// RPG Demo 的 Hero 视图（第一阶段只做日志表现）。
    /// 挂到场景的 Hero 物体上；战斗逻辑由 RPGCombatContext 驱动。
    /// </summary>
    public class Hero : MonoBehaviour
    {
        public static Hero Instance { get; private set; }

        public RPGCombatUnit Unit { get; private set; }
        public KeyCode AttackKey = KeyCode.Space;

        private RPGCombatContext m_Context;

        private void Start()
        {
            Instance = this;
            this.m_Context = NFROOT.Instance.GetSystem<ContextSystem>().GetContext<RPGCombatContext>();
            this.m_Context.StartDemo();
            this.Unit = this.m_Context.Hero;

            if (this.Unit != null)
            {
                Log($"Hero 视图已绑定战斗单元：{this.Unit.DisplayName}");
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(this.AttackKey))
            {
                this.m_Context?.ManualHeroAttack();
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Log(string inMessage)
        {
            Debug.Log($"[RPG Demo] {inMessage}");
        }
    }
}
