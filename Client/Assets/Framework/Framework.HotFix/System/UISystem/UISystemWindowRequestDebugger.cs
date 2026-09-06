#if UNITY_EDITOR
using UnityEngine;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// [仅编辑器] UISystem WindowRequest 状态监视组件。
    /// 由 <see cref="UISystem.AwakeRoot"/> 在编辑器 Play Mode 下自动附加到 [UIROOT]，
    /// 仅用于把当前 UISystem 引用暴露给自定义 Inspector，不参与正式构建。
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class UISystemWindowRequestDebugger : MonoBehaviour
    {
        // 运行时由 UISystem.AwakeRoot 注入，不需要序列化到预制体
        private UISystem m_UISystem;

        /// <summary>当前绑定的 UISystem，供自定义 Inspector 读取</summary>
        public UISystem UISystem => m_UISystem;

        internal void Bind(UISystem inUISystem)
        {
            this.m_UISystem = inUISystem;
        }
    }
}
#endif
