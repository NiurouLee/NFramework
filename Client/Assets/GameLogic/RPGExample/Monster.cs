using NFramework.ModuleSystem;
using UnityEngine;

namespace Game.Logic
{
    /// <summary>
    /// RPG Demo 的 Monster 视图（第一阶段只做日志表现）。
    /// </summary>
    public class Monster : MonoBehaviour
    {
        public static Monster Instance { get; private set; }

        public RPGCombatUnit Unit { get; private set; }

        private void Start()
        {
            Instance = this;
            var context = NFROOT.Instance.GetSystem<ContextSystem>().GetContext<RPGCombatContext>();
            context.StartDemo();
            this.Unit = context.Monster;

            if (this.Unit != null)
            {
                Debug.Log($"[RPG Demo] Monster 视图已绑定战斗单元：{this.Unit.DisplayName}");
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}
