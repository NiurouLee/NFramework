using  NFramework.Core;

namespace NFramework.ModuleSystem
{
    public class WindowPoolEntry
    {
        public Window Window;
    }

    /// <summary>
    /// UI框架window缓存逻辑
    /// </summary>
    public class WindowPoolServices
    {
        private LRU<string, WindowPoolEntry> m_Cache = new LRU<string, WindowPoolEntry>(8);

        public bool TryGetWindow<T>(string inName, out T outWindow) where T : Window
        {
            if (m_Cache.TryGetValue(inName, out var poolEntry))
            {
                outWindow = poolEntry.Window as T;
                return true;
            }

            outWindow = null;
            return false;
        }

        public bool TryGetWindow(string inName, out Window outWindow)
        {
            if (m_Cache.TryGetValue(inName, out var poolEntry))
            {
                outWindow = poolEntry.Window;
                return true;
            }

            outWindow = null;
            return false;
        }

        public void Cache(string inName, Window inWindow)
        {
            var poolEntry = new WindowPoolEntry
            {
                Window = inWindow
            };
            this.m_Cache.Set(inName, poolEntry);
        }
    }
}