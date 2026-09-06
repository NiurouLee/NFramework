using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// 带池的 provider：按 ViewConfig ID 维护 UIFacade 实例池。
    /// InitPool 注册 prefab（可多次调用，每次注册一个 prefab），
    /// Alloc 优先从池中取，池空时从注册的 prefab 实例化；Free 回收到池根节点下，等待复用。
    /// </summary>
    public class UIFacadeProviderPool : IUIFacadeProvider
    {
        /// <summary>ID -> 注册的 prefab（UIFacade 组件）</summary>
        private readonly Dictionary<string, UIFacade> m_PrefabByID = new Dictionary<string, UIFacade>();

        /// <summary>ID -> 空闲实例栈</summary>
        private readonly Dictionary<string, Stack<UIFacade>> m_PoolByID = new Dictionary<string, Stack<UIFacade>>();

        private RectTransform m_PoolRoot;

        /// <summary>
        /// 注册 prefab 到池。可被多次调用（一次一个 prefab），相同 ID 会覆盖注册。
        /// </summary>
        public void InitPool(RectTransform poolRoot, UIFacade inUIFacade)
        {
            if (inUIFacade == null)
            {
                Debug.LogError("[UIFacadeProviderPool] InitPool: inUIFacade 为空");
                return;
            }

            if (poolRoot != null)
            {
                m_PoolRoot = poolRoot;
            }

            string id = inUIFacade.ID;
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogError($"[UIFacadeProviderPool] InitPool: prefab 缺少 ID，无法入池，prefab: {inUIFacade.name}");
                return;
            }

            m_PrefabByID[id] = inUIFacade;
            if (!m_PoolByID.ContainsKey(id))
            {
                m_PoolByID[id] = new Stack<UIFacade>();
            }
        }

        public UIFacade Alloc<T>() where T : View
        {
            var viewConfig = NFROOT.I.GetSystem<UISystem>().GetViewConfig<T>();
            if (viewConfig == null)
            {
                Debug.LogError($"[UIFacadeProviderPool] Alloc<{typeof(T).Name}>: 未找到 ViewConfig");
                return null;
            }

            return Alloc(viewConfig.ID);
        }

        public UIFacade Alloc(string inViewID)
        {
            if (string.IsNullOrEmpty(inViewID))
            {
                Debug.LogError("[UIFacadeProviderPool] Alloc: inViewID 为空");
                return null;
            }

            // 优先从池中取
            if (m_PoolByID.TryGetValue(inViewID, out var stack) && stack.Count > 0)
            {
                var pooled = stack.Pop();
                if (pooled != null)
                {
                    pooled.gameObject.SetActive(true);
                    return pooled;
                }
            }

            // 池空则从注册的 prefab 实例化
            if (!m_PrefabByID.TryGetValue(inViewID, out var prefab) || prefab == null)
            {
                Debug.LogError($"[UIFacadeProviderPool] Alloc: 未注册 prefab，ID: {inViewID}，请先调用 InitPool");
                return null;
            }

            var go = UnityEngine.Object.Instantiate(prefab.gameObject);
            var facade = go.GetComponent<UIFacade>();
            if (facade == null)
            {
                Debug.LogError($"[UIFacadeProviderPool] Alloc: 实例上缺少 UIFacade，ID: {inViewID}");
                UnityEngine.Object.Destroy(go);
                return null;
            }

            return facade;
        }

        public UniTaskCompletionSource<UIFacade> AllocAsync<T>(CancellationToken inCancellationToken = default)
            where T : View
        {
            var deferred = new UniTaskCompletionSource<UIFacade>();
            if (inCancellationToken.IsCancellationRequested)
            {
                deferred.TrySetCanceled();
                return deferred;
            }

            var facade = Alloc<T>();
            if (facade != null)
            {
                deferred.TrySetResult(facade);
            }
            else
            {
                deferred.TrySetCanceled();
            }

            return deferred;
        }

        public UniTaskCompletionSource<UIFacade> AllocAsync(string inViewID,
            CancellationToken inCancellationToken = default)
        {
            var deferred = new UniTaskCompletionSource<UIFacade>();
            if (inCancellationToken.IsCancellationRequested)
            {
                deferred.TrySetCanceled();
                return deferred;
            }

            var facade = Alloc(inViewID);
            if (facade != null)
            {
                deferred.TrySetResult(facade);
            }
            else
            {
                deferred.TrySetCanceled();
            }

            return deferred;
        }

        public void Free(UIFacade inUIFacade)
        {
            if (inUIFacade == null)
            {
                return;
            }

            string id = inUIFacade.ID;
            if (string.IsNullOrEmpty(id))
            {
                // 没有 ID 的实例无法回池，直接销毁
                UnityEngine.Object.Destroy(inUIFacade.gameObject);
                return;
            }

            if (!m_PoolByID.TryGetValue(id, out var stack))
            {
                stack = new Stack<UIFacade>();
                m_PoolByID[id] = stack;
            }

            inUIFacade.gameObject.SetActive(false);
            if (m_PoolRoot != null)
            {
                inUIFacade.transform.SetParent(m_PoolRoot, false);
            }
            else
            {
                inUIFacade.transform.SetParent(null);
            }

            stack.Push(inUIFacade);
        }

        public void Destroy()
        {
            foreach (var stack in m_PoolByID.Values)
            {
                while (stack.Count > 0)
                {
                    var facade = stack.Pop();
                    if (facade != null && facade.gameObject != null)
                    {
                        UnityEngine.Object.Destroy(facade.gameObject);
                    }
                }
            }

            m_PoolByID.Clear();
            m_PrefabByID.Clear();
            m_PoolRoot = null;
        }
    }
}
