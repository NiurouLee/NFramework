using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// 基于原生 ScrollRect 的窗口化列表（非泛型）。
    /// 只创建“可见数量 + 缓冲”个 item，滚动时复用并重新绑定数据；
    /// item 通过 UIFacadeProvider.Alloc 创建、View.Destroy 回收。
    /// </summary>
    public class SimpleListViewComponent : INListViewComponent
    {
        public ScrollRect ScrollRect { get; set; }

        /// <summary>item 主轴尺寸；0 时从第一个 item 自动测量</summary>
        public float ItemSize = 0f;

        /// <summary>可见数量之外的缓冲 item 数（上下各）</summary>
        public int BufferCount = 2;

        private readonly List<View> m_ItemViews = new List<View>();
        private float m_MeasuredItemSize;
        private int m_StartIndex;
        private bool m_Vertical;
        private bool m_Subscribed;

        private RectTransform Content => ScrollRect != null ? ScrollRect.content : null;

        public SimpleListViewComponent BindScrollRect(ScrollRect scrollRect)
        {
            ScrollRect = scrollRect;
            return this;
        }

        public override void Init(string name, IUIFacadeProvider provider, string itemViewID,
            Func<UIFacade, int, View> onCreateItem = null,
            Action<View, object, int> onBindItem = null)
        {
            base.Init(name, provider, itemViewID, onCreateItem, onBindItem);
        }

        public override Vector2 GetVisibleRange()
        {
            return new Vector2(m_StartIndex, m_StartIndex + Mathf.Max(0, m_ItemViews.Count - 1));
        }

        public override void MoveTo(int index, int posType)
        {
            if (List == null || List.Count == 0 || m_MeasuredItemSize <= 0f || Content == null)
            {
                return;
            }

            index = Mathf.Clamp(index, 0, List.Count - 1);
            float offset = index * m_MeasuredItemSize;
            var pos = Content.anchoredPosition;
            Content.anchoredPosition = m_Vertical
                ? new Vector2(pos.x, offset)
                : new Vector2(-offset, pos.y);
            m_StartIndex = index;
            BindAll();
        }

        public override void MoveToWithAnim(int index, int posType)
        {
            // 简化：直接跳转。需要补间可在这里扩展
            MoveTo(index, posType);
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
            int local = index - m_StartIndex;
            if (local < 0 || local >= m_ItemViews.Count)
            {
                return null;
            }
            return m_ItemViews[local];
        }

        public override void Refill(int startIndex = 0, int endIndex = -1)
        {
            EnsureSubscribed();
            if (ScrollRect == null || List == null)
            {
                return;
            }

            RecycleAll();
            if (List.Count == 0)
            {
                UpdateContentSize(0f);
                return;
            }

            startIndex = Mathf.Clamp(startIndex, 0, List.Count - 1);
            m_StartIndex = startIndex;

            // 先创建一个用于测量尺寸
            var first = CreateItemView();
            if (first == null || first.RectTransform == null)
            {
                return;
            }
            first.RectTransform.SetParent(Content, false);
            m_MeasuredItemSize = ItemSize > 0f ? ItemSize : MeasureItemSize(first);
            if (m_MeasuredItemSize <= 0f)
            {
                RecycleAll();
                return;
            }

            var viewport = ScrollRect.viewport != null ? ScrollRect.viewport : (RectTransform)ScrollRect.transform;
            float viewportSize = m_Vertical
                ? viewport.rect.height
                : viewport.rect.width;
            if (viewportSize <= 0f)
            {
                viewportSize = 200f;
            }

            int visibleCount = Mathf.CeilToInt(viewportSize / m_MeasuredItemSize) + BufferCount * 2;
            visibleCount = Mathf.Clamp(visibleCount, 1, List.Count - m_StartIndex);

            UpdateContentSize(List.Count * m_MeasuredItemSize);

            m_ItemViews.Add(first);
            LayoutItem(first, 0);
            BindItem(first, m_StartIndex);

            for (int i = 1; i < visibleCount; i++)
            {
                var view = CreateItemView();
                if (view == null || view.RectTransform == null)
                {
                    break;
                }
                view.RectTransform.SetParent(Content, false);
                m_ItemViews.Add(view);
                LayoutItem(view, i);
                BindItem(view, m_StartIndex + i);
            }
        }

        public override void RefillNoMove()
        {
            BindAll();
        }

        public override void RefillFromEnd(int endIndex = 0, float contentOffset = 0)
        {
            Refill(0);
            MoveTo(Mathf.Max(0, List.Count - 1), 0);
        }

        /// <summary>回收所有 item 并解除 ScrollRect 监听</summary>
        public void Clear()
        {
            if (ScrollRect != null && m_Subscribed)
            {
                ScrollRect.onValueChanged.RemoveListener(OnScroll);
                m_Subscribed = false;
            }
            RecycleAll();
        }

        public override void OnViewComponentDestroy()
        {
            Clear();
            base.OnViewComponentDestroy();
        }

        private void EnsureSubscribed()
        {
            if (m_Subscribed || ScrollRect == null)
            {
                return;
            }

            ScrollRect.onValueChanged.AddListener(OnScroll);
            m_Vertical = ScrollRect.vertical;
            m_Subscribed = true;
        }

        private void OnScroll(Vector2 normalizedPosition)
        {
            if (List == null || List.Count == 0 || m_MeasuredItemSize <= 0f || Content == null)
            {
                return;
            }

            float offset = m_Vertical ? Content.anchoredPosition.y : -Content.anchoredPosition.x;
            int maxStart = Mathf.Max(0, List.Count - m_ItemViews.Count);
            int newStart = Mathf.Clamp(Mathf.FloorToInt(offset / m_MeasuredItemSize), 0, maxStart);
            if (newStart == m_StartIndex)
            {
                return;
            }

            m_StartIndex = newStart;
            BindAll();
        }

        private View CreateItemView()
        {
            if (Provider == null)
            {
                Debug.LogError("[SimpleListViewComponent] Provider 为空，无法创建 item");
                return null;
            }

            var facade = Provider.Alloc(ItemViewID);
            return OnCreateItem(facade, m_ItemViews.Count);
        }

        private float MeasureItemSize(View view)
        {
            var rect = view.RectTransform.rect;
            return m_Vertical ? rect.height : rect.width;
        }

        private void LayoutItem(View view, int slot)
        {
            var rt = view.RectTransform;
            rt.SetParent(Content, false);
            rt.pivot = new Vector2(0f, 1f);
            rt.anchorMin = m_Vertical ? new Vector2(0f, 1f) : new Vector2(0f, 0f);
            rt.anchorMax = m_Vertical ? new Vector2(1f, 1f) : new Vector2(0f, 1f);
            rt.sizeDelta = m_Vertical
                ? new Vector2(0f, m_MeasuredItemSize)
                : new Vector2(m_MeasuredItemSize, 0f);
            rt.anchoredPosition = m_Vertical
                ? new Vector2(0f, -slot * m_MeasuredItemSize)
                : new Vector2(slot * m_MeasuredItemSize, 0f);
        }

        private void BindAll()
        {
            for (int i = 0; i < m_ItemViews.Count; i++)
            {
                LayoutItem(m_ItemViews[i], i);
                BindItem(m_ItemViews[i], m_StartIndex + i);
            }
        }

        private void UpdateContentSize(float totalSize)
        {
            var content = Content;
            if (content == null)
            {
                return;
            }

            var size = content.sizeDelta;
            if (m_Vertical)
            {
                size.y = totalSize;
            }
            else
            {
                size.x = totalSize;
            }
            content.sizeDelta = size;
        }

        private void RecycleAll()
        {
            for (int i = m_ItemViews.Count - 1; i >= 0; i--)
            {
                RecycleView(m_ItemViews[i]);
            }
            m_ItemViews.Clear();
            m_StartIndex = 0;
            m_MeasuredItemSize = 0f;
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
    }

    /// <summary>
    /// 泛型外壳：类型化 Init / SetList / 数据读取。
    /// </summary>
    public class SimpleListViewComponent<T> : SimpleListViewComponent
    {
        public IList<T> DataList => (IList<T>)List;

        public SimpleListViewComponent<T> Init(string name, IUIFacadeProvider provider, string itemViewID,
            Func<UIFacade, int, View> onCreateItem = null,
            Action<View, T, int> onBindItem = null)
        {
            base.Init(name, provider, itemViewID, onCreateItem,
                onBindItem == null ? null : (Action<View, object, int>)((view, data, index) => onBindItem(view, (T)data, index)));
            return this;
        }

        public SimpleListViewComponent<T> SetList(IList<T> list)
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
