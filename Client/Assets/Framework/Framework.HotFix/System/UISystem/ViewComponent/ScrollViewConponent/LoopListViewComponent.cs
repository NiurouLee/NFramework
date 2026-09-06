using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// LoopListView 初始化状态。
    /// </summary>
    public enum LoopListViewState
    {
        None,
        Inited,
    }

    /// <summary>
    /// 基于 LoopScrollRect 的循环列表（非泛型）。
    /// Item 通过 UIFacadeProvider.Alloc 创建、View.Destroy 回收；
    /// 使用前需 BindScrollRect 并保证 LoopScrollRect 的 dataSource/prefabSource 由本组件接管。
    /// 已 Init 后再次调用 Init 会被忽略（防止重复初始化），Clear 后可重新 Init。
    /// </summary>
    public class LoopListViewComponent : INListViewComponent
    {
        public LoopScrollRect ScrollRect { get; set; }

        /// <summary>MoveTo 滚动速度（>0）</summary>
        public float ScrollSpeed = 0.5f;

        /// <summary>MoveToWithAnim 动画时长（秒，>0）</summary>
        public float ScrollAnimTime = 0.3f;

        /// <summary>当前初始化状态</summary>
        public LoopListViewState State { get; private set; } = LoopListViewState.None;

        private readonly Dictionary<Transform, IListView> m_ViewByTransform = new Dictionary<Transform, IListView>();
        private readonly Dictionary<int, string> m_PrefabIndex2ID = new Dictionary<int, string>();
        private bool m_SourcesReady;

        public LoopListViewComponent BindScrollRect(LoopScrollRect scrollRect)
        {
            ScrollRect = scrollRect;
            return this;
        }

        private void EnsureSources()
        {
            if (m_SourcesReady || ScrollRect == null)
            {
                return;
            }

            ScrollRect.prefabSource = new ProviderPrefabSource(this);
            ScrollRect.dataSource = new ProviderDataSource(this);
            m_SourcesReady = true;
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
            if (State == LoopListViewState.Inited)
            {
                Debug.LogWarning("[LoopListView] 已初始化，忽略重复 Init");
                return;
            }

            if (ScrollRect == null)
            {
                Debug.LogError("[LoopListView] 尚未 BindScrollRect，无法 Init");
                return;
            }

            if (OnCreateItemView == null)
            {
                Debug.LogError("1");
            }

            if (OnGetUIFacadeIndex == null)
            {
                OnGetUIFacadeIndex = this.DefaultOnGetUIFacadeIndex;
            }

            base.Init(name, OnCreateItemView, OnGetUIFacadeIndex);
            base.InitProvider(provider);
            EnsureSources();
            State = LoopListViewState.Inited;
        }

        /// <summary>
        /// 初始化方式二：根据 PrefabSourceMap 建立 item 池后初始化。
        /// </summary>
        public void Init(string name, PrefabSourceMap prefabSource,
            Func<UIFacade, IUIFacadeProvider, int, object, IListView> OnCreateItemView = null,
            Func<int, object, int> OnGetUIFacadeIndex = null)
        {
            if (State == LoopListViewState.Inited)
            {
                Debug.LogWarning("[LoopListView] 已初始化，忽略重复 Init");
                return;
            }

            if (ScrollRect == null)
            {
                Debug.LogError("[LoopListView] 尚未 BindScrollRect，无法 Init");
                return;
            }

            if (prefabSource == null || prefabSource.prefabGameObjectList == null)
            {
                Debug.LogError("[LoopListView] PrefabSourceMap 为空，无法 Init");
                return;
            }

            var provider = new UIFacadeProviderPool();
            var root = this.ScrollRect.GetComponent<RectTransform>();
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

                var id = facade.ID;
                var index = i;
                this.m_PrefabIndex2ID.TryAdd(index, id);
                provider.InitPool(root, facade);
            }

            base.Init(name, OnCreateItemView, OnGetUIFacadeIndex);
            base.InitProvider(provider);
            EnsureSources();
            State = LoopListViewState.Inited;
        }

        public override Vector2 GetVisibleRange()
        {
            if (ScrollRect == null)
            {
                return Vector2.zero;
            }

            int first = ScrollRect.GetFirstItem(out _);
            int last = ScrollRect.GetLastItem(out _);
            return new Vector2(first, last);
        }

        public override void MoveTo(int index, int posType)
        {
            if (ScrollRect == null || List == null)
            {
                return;
            }

            ScrollRect.ScrollToCell(index, ScrollSpeed, 0f, ToScrollMode(posType));
        }

        public override void MoveToWithAnim(int index, int posType)
        {
            if (ScrollRect == null || List == null)
            {
                return;
            }

            ScrollRect.ScrollToCellWithinTime(index, ScrollAnimTime, 0f, ToScrollMode(posType));
        }

        public override void RefreshSingleItem(int index)
        {
        }

        public override View GetViewByIndex(int index)
        {
            return null;
        }


        public override void Refill(int startIndex = 0, int endIndex = -1)
        {
            EnsureSources();
            if (ScrollRect == null || List == null)
            {
                return;
            }

            ScrollRect.ClearCells();
            ScrollRect.totalCount = List.Count;
            m_ViewByTransform.Clear();
            ScrollRect.RefillCells(startIndex);
        }

        public override void RefillNoMove()
        {
            if (ScrollRect == null)
            {
                return;
            }

            ScrollRect.RefreshCells();
        }

        public override void RefillFromEnd(int endIndex = 0, float contentOffset = 0)
        {
            EnsureSources();
            if (ScrollRect == null || List == null)
            {
                return;
            }

            ScrollRect.ClearCells();
            ScrollRect.totalCount = List.Count;
            m_ViewByTransform.Clear();
            ScrollRect.RefillCellsFromEnd(endIndex, contentOffset);
        }

        /// <summary>回收所有 item 并解除与 ScrollRect 的绑定</summary>
        public void Clear()
        {
            RecycleAll();
            if (ScrollRect != null)
            {
                ScrollRect.prefabSource = null;
                ScrollRect.dataSource = null;
                m_SourcesReady = false;
            }

            State = LoopListViewState.None;
        }

        public override void OnViewComponentDestroy()
        {
            Clear();
            base.OnViewComponentDestroy();
        }

        private static LoopScrollRectBase.ScrollMode ToScrollMode(int posType)
        {
            switch (posType)
            {
                case 1: return LoopScrollRectBase.ScrollMode.ToCenter;
                case 2: return LoopScrollRectBase.ScrollMode.JustAppear;
                default: return LoopScrollRectBase.ScrollMode.ToStart;
            }
        }

        private GameObject CreateItemObject(int index)
        {
            if (Provider == null)
            {
                Debug.LogError("[LoopListView] Provider 为空，无法创建 item");
                return null;
            }

            var prefabIndex = this.OnGetUIFacadeIndex(index, this.List[index]);
            if (this.m_PrefabIndex2ID.TryGetValue(prefabIndex, out var ItemViewID))
            {
                var facade = Provider.Alloc(ItemViewID);
                return facade.gameObject;
            }

            return null;
        }

        private void BindItemByTransform(Transform transform, int index)
        {
            var data = this.List[index];
            if (!this.m_ViewByTransform.TryGetValue(transform, out var view))
            {
                var uiFacade = transform.GetComponent<UIFacade>();
                view = (IListView)this.OnCreateItemView(uiFacade, this.Provider, index, data);
                this.m_ViewByTransform.Add(transform, view);
                if (view != null && view is View _view)
                {
                    view.Refresh(index, data);
                    _view.Show();
                }
            }
            else
            {
                if (view != null && view is View _view)
                {
                    _view.Show();
                    view.Refresh(index, data);
                }
            }
        }

        private void RecycleItem(Transform transform)
        {
            if (!m_ViewByTransform.TryGetValue(transform, out var view))
            {
                return;
            }

            m_ViewByTransform.Remove(transform);
            var facade = transform.GetComponent<UIFacade>();
            this.Provider.Free(facade);
        }

        private void RecycleAll()
        {
        }

        private void RecycleView(View view)
        {
            if (view == null)
            {
                return;
            }

            var facade = view.Facade;
            var provider = view.Provider;
            view.Destroy(); // 内部会调用 Provider.Free(facade)

            // UIFacadeProviderPool 已接管回收（实例入池复用），不要销毁；
            // 其他 provider（如动态加载）Free 未实现时兜底销毁，避免泄漏
            if (!(provider is UIFacadeProviderPool) && facade != null && facade.gameObject != null)
            {
                UnityEngine.Object.Destroy(facade.gameObject);
            }
        }

        private class ProviderPrefabSource : LoopScrollPrefabSource
        {
            private readonly LoopListViewComponent m_Owner;

            public ProviderPrefabSource(LoopListViewComponent owner)
            {
                m_Owner = owner;
            }

            public GameObject GetObject(int index)
            {
                return m_Owner.CreateItemObject(index);
            }

            public void ReturnObject(Transform trans)
            {
                m_Owner.RecycleItem(trans);
            }
        }

        private class ProviderDataSource : LoopScrollDataSource
        {
            private readonly LoopListViewComponent m_Owner;

            public ProviderDataSource(LoopListViewComponent owner)
            {
                m_Owner = owner;
            }

            public void ProvideData(Transform transform, int idx)
            {
                m_Owner.BindItemByTransform(transform, idx);
            }
        }
    }
}