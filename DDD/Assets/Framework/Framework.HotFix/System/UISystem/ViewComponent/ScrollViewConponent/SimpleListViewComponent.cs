using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// SimpleListView 初始化状态。
    /// </summary>
    public enum SimpleListViewState
    {
        None,
        Inited,
    }

    /// <summary>
    /// 基于 UGUI ScrollRect 的普通列表（无回收滚动，非虚拟化）。
    /// 数据源数量变化时调用 Refill，Item 通过 UIFacadeProviderPool 管理：
    /// 已有子项尽量原位刷新复用，数量增加时从池中 Alloc，减少时 Free 回池，
    /// 避免频繁 Instantiate/Destroy，也无需像 LoopScrollRect 那样由列表接管 dataSource。
    /// 使用前需 BindScrollRect，Item 需挂在 ScrollRect.content 下（配合 Vertical/HorizontalLayoutGroup 或自行排版）。
    /// 已 Init 后再次调用 Init 会被忽略（防止重复初始化），Clear 后可重新 Init。
    /// </summary>
    public class SimpleListViewComponent : INListViewComponent
    {
        public ScrollRect ScrollRect { get; set; }

        /// <summary>MoveTo 滚动速度（>0）</summary>
        public float ScrollSpeed = 0.5f;

        /// <summary>MoveToWithAnim 动画时长（秒，>0）</summary>
        public float ScrollAnimTime = 0.3f;

        /// <summary>当前初始化状态</summary>
        public SimpleListViewState State { get; private set; } = SimpleListViewState.None;

        private readonly Dictionary<int, IListView> m_ViewByIndex = new Dictionary<int, IListView>();
        private readonly Dictionary<int, string> m_PrefabIndex2ID = new Dictionary<int, string>();

        /// <summary>该组件在 Init(PrefabSourceMap) 内部创建的 provider，负责在 Clear/Destroy 时一并清理</summary>
        private bool m_ProviderOwned;

        private CancellationTokenSource m_ScrollCts;

        public SimpleListViewComponent BindScrollRect(ScrollRect scrollRect)
        {
            ScrollRect = scrollRect;
            return this;
        }

        private int DefaultOnGetUIFacadeIndex(int index, object data)
        {
            return 0;
        }

        /// <summary>
        /// 初始化方式一：外部 provider 按 ItemViewID 创建 item。
        /// </summary>
        public void Init(string name, IUIFacadeProvider provider,
            Func<UIFacade, IUIFacadeProvider, int, object, IListView> OnCreateItemView = null,
            Func<int, object, int> OnGetUIFacadeIndex = null)
        {
            if (State == SimpleListViewState.Inited)
            {
                Debug.LogWarning("[SimpleListView] 已初始化，忽略重复 Init");
                return;
            }

            if (ScrollRect == null)
            {
                Debug.LogError("[SimpleListView] 尚未 BindScrollRect，无法 Init");
                return;
            }

            if (OnCreateItemView == null)
            {
                Debug.LogError("[SimpleListView] OnCreateItemView 为空，无法 Init");
                return;
            }

            if (OnGetUIFacadeIndex == null)
            {
                OnGetUIFacadeIndex = DefaultOnGetUIFacadeIndex;
            }

            base.Init(name, OnCreateItemView, OnGetUIFacadeIndex);
            base.InitProvider(provider);
            m_ProviderOwned = false;
            State = SimpleListViewState.Inited;
        }

        /// <summary>
        /// 初始化方式二：根据 PrefabSourceMap 建立 item 池后初始化。
        /// 池根节点使用 ScrollRect 自身，回收的 item 会离开 content，不会干扰 ScrollRect 的布局与滚动范围。
        /// </summary>
        public void Init(string name, PrefabSourceMap prefabSource,
            Func<UIFacade, IUIFacadeProvider, int, object, IListView> OnCreateItemView = null,
            Func<int, object, int> OnGetUIFacadeIndex = null)
        {
            if (State == SimpleListViewState.Inited)
            {
                Debug.LogWarning("[SimpleListView] 已初始化，忽略重复 Init");
                return;
            }

            if (ScrollRect == null)
            {
                Debug.LogError("[SimpleListView] 尚未 BindScrollRect，无法 Init");
                return;
            }

            if (OnCreateItemView == null)
            {
                Debug.LogError("[SimpleListView] OnCreateItemView 为空，无法 Init");
                return;
            }

            if (OnGetUIFacadeIndex == null)
            {
                OnGetUIFacadeIndex = DefaultOnGetUIFacadeIndex;
            }

            if (prefabSource == null ||
                prefabSource.prefabGameObjectList == null ||
                prefabSource.prefabGameObjectList.Count == 0)
            {
                Debug.LogError("[SimpleListView] PrefabSourceMap 为空，无法 Init");
                return;
            }

            var provider = new UIFacadeProviderPool();
            var root = ScrollRect.GetComponent<RectTransform>();
            if (root == null)
            {
                Debug.LogError("[SimpleListView] ScrollRect 上没有 RectTransform，无法 Init");
                return;
            }

            var list = prefabSource.prefabGameObjectList;
            for (int i = 0; i < list.Count; i++)
            {
                var go = list[i];
                if (go == null)
                {
                    throw new Exception("go is null");
                }

                var facade = go.GetComponent<UIFacade>();
                if (facade == null)
                {
                    throw new Exception("go dont have uifacade Component");
                }

                if (string.IsNullOrEmpty(facade.ID))
                {
                    throw new Exception("go dont have uifacade ID");
                }

                m_PrefabIndex2ID[i] = facade.ID;
                provider.InitPool(root, facade);
            }

            base.Init(name, OnCreateItemView, OnGetUIFacadeIndex);
            base.InitProvider(provider);
            m_ProviderOwned = true;
            State = SimpleListViewState.Inited;
        }

        public override Vector2 GetVisibleRange()
        {
            // 普通列表所有 item 都真实存在，不存在“只实例化可视区”的概念
            if (List == null || List.Count == 0)
            {
                return Vector2.zero;
            }

            return new Vector2(0, List.Count - 1);
        }

        public override void MoveTo(int index, int posType)
        {
            if (!TryGetScrollTarget(index, posType, out var target))
            {
                return;
            }

            StopScrollAnimation();
            ScrollRect.normalizedPosition = target;
        }

        public override void MoveToWithAnim(int index, int posType)
        {
            if (!TryGetScrollTarget(index, posType, out var target))
            {
                return;
            }

            if (ScrollAnimTime <= 0f)
            {
                MoveTo(index, posType);
                return;
            }

            StopScrollAnimation();
            m_ScrollCts = new CancellationTokenSource();
            var token = m_ScrollCts.Token;
            PlayScrollAnimationAsync(ScrollRect.normalizedPosition, target, ScrollAnimTime, token).Forget();
        }

        public override void RefreshSingleItem(int index)
        {
            if (List == null || index < 0 || index >= List.Count)
            {
                Debug.LogError($"[SimpleListView] RefreshSingleItem 索引越界: {index}");
                return;
            }

            if (m_ViewByIndex.ContainsKey(index))
            {
                RefreshItemAt(index);
            }
            else
            {
                CreateItemAt(index);
            }
        }

        public override View GetViewByIndex(int index)
        {
            if (m_ViewByIndex.TryGetValue(index, out var view) && view is View v)
            {
                return v;
            }

            return null;
        }

        public override void Refill(int startIndex = 0, int endIndex = -1)
        {
            if (!CanFill())
            {
                return;
            }

            ReconcileItems();
            ScrollToStart();
        }

        public override void RefillNoMove()
        {
            if (!CanFill())
            {
                return;
            }

            ReconcileItems();
        }

        public override void RefillFromEnd(int endIndex = 0, float contentOffset = 0)
        {
            if (!CanFill())
            {
                return;
            }

            ReconcileItems();
            ScrollToEnd(contentOffset);
        }

        /// <summary>回收所有 item；若 provider 由本组件创建则一并销毁池实例，并解除初始化状态</summary>
        public void Clear()
        {
            StopScrollAnimation();
            RecycleAll();

            m_PrefabIndex2ID.Clear();

            if (m_ProviderOwned)
            {
                (Provider as UIFacadeProviderPool)?.Destroy();
                Provider = null;
                m_ProviderOwned = false;
            }

            State = SimpleListViewState.None;
        }

        public override void OnViewComponentDestroy()
        {
            Clear();
            base.OnViewComponentDestroy();
        }

        #region Item 管理

        private bool CanFill()
        {
            if (ScrollRect == null)
            {
                Debug.LogError("[SimpleListView] 尚未 BindScrollRect，无法刷新");
                return false;
            }

            if (ScrollRect.content == null)
            {
                Debug.LogError("[SimpleListView] ScrollRect.content 为空，请在 ScrollRect 上配置 Content");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 对齐数据源与 content 下的子项：
        /// 已存在的 item 直接原位刷新；数量不足时 Alloc 补齐；数量超出时回收多余 item；
        /// 同一下标所需 prefab 类型变化时回收重建。
        /// </summary>
        private void ReconcileItems()
        {
            int count = List == null ? 0 : List.Count;

            // 多余的 item 从后往前回收，避免后续按下标重建时顺序被打乱
            var extraIndices = new List<int>();
            foreach (var index in m_ViewByIndex.Keys)
            {
                if (index >= count)
                {
                    extraIndices.Add(index);
                }
            }

            extraIndices.Sort((a, b) => b.CompareTo(a));
            foreach (int index in extraIndices)
            {
                RecycleItemAt(index);
            }

            for (int i = 0; i < count; i++)
            {
                if (m_ViewByIndex.ContainsKey(i))
                {
                    if (NeedRebind(i))
                    {
                        RecycleItemAt(i);
                        CreateItemAt(i);
                    }
                    else
                    {
                        RefreshItemAt(i);
                    }
                }
                else
                {
                    CreateItemAt(i);
                }
            }
        }

        /// <summary>同一下标的数据类型变化后，原 item 无法继续复用，需要回收重建</summary>
        private bool NeedRebind(int index)
        {
            if (!m_ViewByIndex.TryGetValue(index, out var view) || !(view is View v))
            {
                return false;
            }

            int prefabIndex = OnGetUIFacadeIndex?.Invoke(index, List[index]) ?? 0;
            return !m_PrefabIndex2ID.TryGetValue(prefabIndex, out var viewID) ||
                   v.Facade == null ||
                   v.Facade.ID != viewID;
        }

        private bool CreateItemAt(int index)
        {
            if (Provider == null)
            {
                Debug.LogError("[SimpleListView] Provider 为空，无法创建 item");
                return false;
            }

            object data = List[index];
            int prefabIndex = OnGetUIFacadeIndex?.Invoke(index, data) ?? 0;
            if (!m_PrefabIndex2ID.TryGetValue(prefabIndex, out var viewID))
            {
                Debug.LogError($"[SimpleListView] 未找到 prefabIndex {prefabIndex} 对应的 ViewID，请检查 PrefabSourceMap/OnGetUIFacadeIndex");
                return false;
            }

            var facade = Provider.Alloc(viewID);
            if (facade == null)
            {
                return false;
            }

            var view = OnCreateItemView(facade, Provider, index, data);
            if (view == null)
            {
                Debug.LogError($"[SimpleListView] OnCreateItemView 返回空，回收该 facade, index: {index}");
                FreeFacade(facade, Provider);
                return false;
            }

            var trans = facade.transform;
            trans.SetParent(ScrollRect.content, false);
            trans.SetSiblingIndex(Mathf.Clamp(index, 0, ScrollRect.content.childCount - 1));

            m_ViewByIndex[index] = view;
            BindItemView(view, index, data);
            return true;
        }

        private void RefreshItemAt(int index)
        {
            if (!m_ViewByIndex.TryGetValue(index, out var view) || view == null)
            {
                return;
            }

            BindItemView(view, index, List[index]);
        }

        private static void BindItemView(IListView view, int index, object data)
        {
            view.Index = index;
            view.Data = data;
            view.Refresh(index, data);

            if (view is View v)
            {
                v.Show();
            }
        }

        private void RecycleItemAt(int index)
        {
            if (!m_ViewByIndex.Remove(index, out var view))
            {
                return;
            }

            if (view is View v)
            {
                RecycleView(v);
            }
            else
            {
                view?.OnRecycle();
            }
        }

        private void RecycleAll()
        {
            foreach (int index in new List<int>(m_ViewByIndex.Keys))
            {
                RecycleItemAt(index);
            }
        }

        private void RecycleView(View view)
        {
            if (view == null)
            {
                return;
            }

            if (view is IListView itemView)
            {
                itemView.OnRecycle();
            }

            var facade = view.Facade;
            var provider = view.Provider;
            if (facade == null)
            {
                return;
            }

            if (provider == null)
            {
                if (facade.gameObject != null)
                {
                    UnityEngine.Object.Destroy(facade.gameObject);
                }

                return;
            }

            view.Destroy(); // 内部会调用 provider.Free(facade)

            // UIFacadeProviderPool 已接管回收（实例入池复用），不要销毁；
            // 其他 provider（如动态加载）Free 未实现时兜底销毁，避免泄漏
            if (!(provider is UIFacadeProviderPool) && facade != null && facade.gameObject != null)
            {
                UnityEngine.Object.Destroy(facade.gameObject);
            }
        }

        private static void FreeFacade(UIFacade facade, IUIFacadeProvider provider)
        {
            if (facade == null)
            {
                return;
            }

            if (provider is UIFacadeProviderPool)
            {
                provider.Free(facade);
                return;
            }

            provider?.Free(facade);
            if (facade.gameObject != null)
            {
                UnityEngine.Object.Destroy(facade.gameObject);
            }
        }

        #endregion

        #region 滚动定位

        /// <summary>
        /// 普通列表没有 LoopScrollRect 的 Cell 定位能力，这里按 index/(Count-1) 估算归一化位置，
        /// 适用于等尺寸 item 的纵向/横向列表。
        /// </summary>
        private bool TryGetScrollTarget(int index, int posType, out Vector2 target)
        {
            target = Vector2.zero;
            if (ScrollRect == null || List == null || List.Count == 0)
            {
                return false;
            }

            int lastIndex = List.Count - 1;
            index = Mathf.Clamp(index, 0, lastIndex);
            float progress = lastIndex == 0 ? 0f : (float)index / lastIndex;

            target = ScrollRect.normalizedPosition;
            if (ScrollRect.vertical)
            {
                target.y = 1f - progress;
            }

            if (ScrollRect.horizontal)
            {
                target.x = progress;
            }

            return true;
        }

        private void ScrollToStart()
        {
            if (ScrollRect == null)
            {
                return;
            }

            var target = ScrollRect.normalizedPosition;
            if (ScrollRect.vertical)
            {
                target.y = 1f;
            }

            if (ScrollRect.horizontal)
            {
                target.x = 0f;
            }

            ScrollRect.normalizedPosition = target;
        }

        private void ScrollToEnd(float contentOffset = 0f)
        {
            if (ScrollRect == null)
            {
                return;
            }

            var target = ScrollRect.normalizedPosition;
            if (ScrollRect.vertical)
            {
                target.y = 0f;
            }

            if (ScrollRect.horizontal)
            {
                target.x = 1f;
            }

            ScrollRect.normalizedPosition = target;
        }

        private void StopScrollAnimation()
        {
            if (m_ScrollCts == null)
            {
                return;
            }

            m_ScrollCts.Cancel();
            m_ScrollCts.Dispose();
            m_ScrollCts = null;
        }

        private async UniTaskVoid PlayScrollAnimationAsync(Vector2 from, Vector2 to, float duration,
            CancellationToken token)
        {
            try
            {
                float elapsed = 0f;
                while (elapsed < duration)
                {
                    if (ScrollRect == null || token.IsCancellationRequested)
                    {
                        return;
                    }

                    elapsed += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsed / duration);
                    ScrollRect.normalizedPosition = Vector2.LerpUnclamped(from, to, t);
                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: token);
                }

                if (!token.IsCancellationRequested && ScrollRect != null)
                {
                    ScrollRect.normalizedPosition = to;
                }
            }
            catch (OperationCanceledException)
            {
                // 新的滚动请求/清理会取消上一次动画，属于正常流程
            }
        }

        #endregion
    }
}
