using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// 基于 LoopScrollRect 的循环列表（非泛型）。
    /// Item 通过 UIFacadeProvider.Alloc 创建、View.Destroy 回收；
    /// 使用前需 BindScrollRect 并保证 LoopScrollRect 的 dataSource/prefabSource 由本组件接管。
    /// </summary>
    public class LoopListView : INListViewComponent
    {
        public LoopScrollRect ScrollRect { get; set; }

        /// <summary>MoveTo 滚动速度（>0）</summary>
        public float ScrollSpeed = 0.5f;

        /// <summary>MoveToWithAnim 动画时长（秒，>0）</summary>
        public float ScrollAnimTime = 0.3f;

        private readonly List<View> m_ActiveViews = new List<View>();
        private readonly Dictionary<Transform, View> m_ViewByTransform = new Dictionary<Transform, View>();
        private readonly Dictionary<View, int> m_IndexByView = new Dictionary<View, int>();
        private bool m_SourcesReady;

        public LoopListView BindScrollRect(LoopScrollRect scrollRect)
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

        public override void Init(string name, IUIFacadeProvider provider, string itemViewID,
            Func<UIFacade, int, View> onCreateItem = null,
            Action<View, object, int> onBindItem = null)
        {
            base.Init(name, provider, itemViewID, onCreateItem, onBindItem);
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
            var view = GetViewByIndex(index);
            if (view != null)
            {
                BindItem(view, index);
            }
        }

        public override View GetViewByIndex(int index)
        {
            foreach (var kv in m_IndexByView)
            {
                if (kv.Value == index)
                {
                    return kv.Key;
                }
            }
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
            m_ActiveViews.Clear();
            m_ViewByTransform.Clear();
            m_IndexByView.Clear();
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
            m_ActiveViews.Clear();
            m_ViewByTransform.Clear();
            m_IndexByView.Clear();
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

            var facade = Provider.Alloc(ItemViewID);
            var view = OnCreateItem(facade, index);
            if (view == null || view.RectTransform == null)
            {
                return null;
            }

            m_ActiveViews.Add(view);
            m_ViewByTransform[view.RectTransform] = view;
            return view.RectTransform.gameObject;
        }

        private void BindItemByTransform(Transform transform, int index)
        {
            if (!m_ViewByTransform.TryGetValue(transform, out var view))
            {
                return;
            }

            m_IndexByView[view] = index;
            BindItem(view, index);
        }

        private void RecycleItem(Transform transform)
        {
            if (!m_ViewByTransform.TryGetValue(transform, out var view))
            {
                return;
            }

            m_ViewByTransform.Remove(transform);
            m_IndexByView.Remove(view);
            m_ActiveViews.Remove(view);
            RecycleView(view);
        }

        private void RecycleAll()
        {
            for (int i = m_ActiveViews.Count - 1; i >= 0; i--)
            {
                RecycleView(m_ActiveViews[i]);
            }
            m_ActiveViews.Clear();
            m_ViewByTransform.Clear();
            m_IndexByView.Clear();
        }

        private void RecycleView(View view)
        {
            var facade = view.Facade;
            view.Destroy(); // 内部会调用 Provider.Free(facade)
            if (facade != null && facade.gameObject != null)
            {
                // Provider.Free 目前是空实现，这里兜底销毁 GameObject；等 Provider 支持池化后可去掉
                UnityEngine.Object.Destroy(facade.gameObject);
            }
        }

        private class ProviderPrefabSource : LoopScrollPrefabSource
        {
            private readonly LoopListView m_Owner;

            public ProviderPrefabSource(LoopListView owner)
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
            private readonly LoopListView m_Owner;

            public ProviderDataSource(LoopListView owner)
            {
                m_Owner = owner;
            }

            public void ProvideData(Transform transform, int idx)
            {
                m_Owner.BindItemByTransform(transform, idx);
            }
        }
    }

    /// <summary>
    /// 泛型外壳：类型化 Init / SetList / 数据读取。
    /// </summary>
    public class LoopListView<T> : LoopListView
    {
        public IList<T> DataList => (IList<T>)List;

        public LoopListView<T> Init(string name, IUIFacadeProvider provider, string itemViewID,
            Func<UIFacade, int, View> onCreateItem = null,
            Action<View, T, int> onBindItem = null)
        {
            base.Init(name, provider, itemViewID, onCreateItem,
                onBindItem == null ? null : (Action<View, object, int>)((view, data, index) => onBindItem(view, (T)data, index)));
            return this;
        }

        public LoopListView<T> SetList(IList<T> list)
        {
            base.SetList((IList)list);
            return this;
        }

        public T GetItemData(int index)
        {
            if (List == null || index < 0 || index >= List.Count)
            {
                return default;
            }
            return (T)List[index];
        }
    }
}
