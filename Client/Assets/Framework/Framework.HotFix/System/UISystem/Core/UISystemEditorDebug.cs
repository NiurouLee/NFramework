using System.Collections.Generic;

namespace NFramework.ModuleSystem
{
    public partial class UISystem
    {
#if UNITY_EDITOR
        /// <summary>
        /// [仅编辑器] 返回当前所有 WindowRequest 的快照，
        /// 供 [UIROOT] 上 UISystemWindowRequestDebugger 的自定义 Inspector 读取。
        /// </summary>
        public List<WindowRequest> DebugGetWindowRequests()
        {
            if (this.WindowRequestDictionary == null)
            {
                return new List<WindowRequest>();
            }

            return new List<WindowRequest>(this.WindowRequestDictionary.Values);
        }

        /// <summary>
        /// [仅编辑器] WindowRequest.IsCanceled 是 internal，
        /// 这里提供只读入口给 NFramework.Editor 的自定义 Inspector 使用。
        /// </summary>
        public bool DebugIsRequestCanceled(WindowRequest inRequest)
        {
            return inRequest != null && inRequest.IsCanceled;
        }

        /// <summary>
        /// [仅编辑器] 当前窗口池是否启用。
        /// </summary>
        public bool DebugWindowPoolEnabled => this.WindowPoolEnabled;

        /// <summary>
        /// [仅编辑器] 返回窗口池中全部缓存项的 key + Window 快照。
        /// </summary>
        public List<KeyValuePair<string, WindowPoolEntry>> DebugGetWindowPoolEntries()
        {
            if (!this.WindowPoolEnabled || this.m_Pool == null)
            {
                return new List<KeyValuePair<string, WindowPoolEntry>>();
            }

            return this.m_Pool.DebugGetEntries();
        }
#endif
    }
}
