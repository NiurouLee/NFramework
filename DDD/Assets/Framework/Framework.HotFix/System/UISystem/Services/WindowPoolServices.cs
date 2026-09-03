using NFramework.Core;

namespace NFramework.ModuleSystem
{
    public class WindowPoolEntry
    {
        public Window Window;
    }

    /// <summary>
    /// UI 框架 window 的 LRU 缓存。
    /// key 使用 WindowRequest.RequestKey（窗口ID + Key），保证同类型不同 Key 的实例能分别缓存；
    /// 缓存满时淘汰最久未使用的一项，并交给 UISystem 真正销毁。
    /// </summary>
    public class WindowPoolServices
    {
        private readonly UISystem m_Owner;
        private readonly LRU<string, WindowPoolEntry> m_Cache;

        public WindowPoolServices(UISystem inOwner, int inCapacity = 8)
        {
            this.m_Owner = inOwner;
            this.m_Cache = new LRU<string, WindowPoolEntry>(inCapacity);
            this.m_Cache.onRemoveEntry = OnRemoveEntry;
        }

        public bool TryGetWindow<T>(string inRequestKey, out T outWindow) where T : Window
        {
            if (m_Cache.TryGetValue(inRequestKey, out var poolEntry))
            {
                outWindow = poolEntry.Window as T;
                m_Cache.Remove(inRequestKey);
                return outWindow != null;
            }

            outWindow = null;
            return false;
        }

        public bool TryGetWindow(string inRequestKey, out Window outWindow)
        {
            if (m_Cache.TryGetValue(inRequestKey, out var poolEntry))
            {
                outWindow = poolEntry.Window;
                m_Cache.Remove(inRequestKey);
                return outWindow != null;
            }

            outWindow = null;
            return false;
        }

        public void Cache(string inRequestKey, Window inWindow)
        {
            if (string.IsNullOrEmpty(inRequestKey) || inWindow == null)
            {
                return;
            }

            m_Cache.Set(inRequestKey, new WindowPoolEntry { Window = inWindow });
        }

        public void Remove(string inRequestKey)
        {
            m_Cache.Remove(inRequestKey);
        }

        /// <summary>清空池并销毁所有缓存的窗口</summary>
        public void ClearAll()
        {
            var values = m_Cache.GetValues();
            foreach (var entry in values)
            {
                if (entry?.Window != null)
                {
                    this.m_Owner?.DestroyWindowObject(entry.Window);
                }
            }

            m_Cache.Clear();
        }

        private bool OnRemoveEntry(string inRequestKey, WindowPoolEntry inPoolEntry)
        {
            if (inPoolEntry?.Window != null)
            {
                this.m_Owner?.DestroyWindowObject(inPoolEntry.Window);
            }

            return true;
        }
    }
}
