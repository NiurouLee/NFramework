using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// 列表组件非泛型基类：机制层。
    /// 数据源、UIFacadeProvider、回调都在这一层，内部数据统一用 object，
    /// 具体组件（LoopListView / SimpleListViewComponent）实现抽象行为。
    /// </summary>
    public abstract class INListViewComponent : ViewComponent
    {
        public string Name { get; private set; }

        /// <summary>数据源（object 级）</summary>
        public IList List { get; protected set; }

        /// <summary>Item Facade 提供者（接入框架 UIFacadeProvider）</summary>
        public IUIFacadeProvider Provider { get; protected set; }

        /// <summary>Item 对应的 ViewConfig ID，用于 Provider.Alloc</summary>
        public string ItemViewID { get; protected set; }

        /// <summary>创建 item View，默认 new View + SetUIFacade + Awake</summary>
        public Func<UIFacade, int, View> OnCreateItem { get; protected set; }

        /// <summary>绑定 item 数据：(itemView, data, index)</summary>
        public Action<View, object, int> OnBindItem { get; protected set; }

        public virtual void Init(string name, IUIFacadeProvider provider, string itemViewID,
            Func<UIFacade, int, View> onCreateItem = null,
            Action<View, object, int> onBindItem = null)
        {
            if (provider == null)
            {
                Debug.LogError("[INListViewComponent] provider 不能为空");
            }

            Name = name;
            Provider = provider;
            ItemViewID = itemViewID;
            OnCreateItem = onCreateItem ?? DefaultCreateItem;
            OnBindItem = onBindItem;
        }

        public INListViewComponent SetList(IList list)
        {
            List = list;
            return this;
        }

        protected View DefaultCreateItem(UIFacade facade, int index)
        {
            var view = new View();
            view.SetUIFacade(facade, Provider);
            view.Awake();
            return view;
        }

        protected void BindItem(View view, int index)
        {
            if (view == null)
            {
                return;
            }

            var data = (List != null && index >= 0 && index < List.Count) ? List[index] : null;
            OnBindItem?.Invoke(view, data, index);
        }

        public object GetDataByIndex(int index)
        {
            if (List == null || index < 0 || index >= List.Count)
            {
                return null;
            }
            return List[index];
        }

        public abstract Vector2 GetVisibleRange();
        public abstract void MoveTo(int index, int posType);
        public abstract void MoveToWithAnim(int index, int posType);
        public abstract void RefreshSingleItem(int index);
        public abstract View GetViewByIndex(int index);
        public abstract void Refill(int startIndex = 0, int endIndex = -1);
        public abstract void RefillNoMove();
        public abstract void RefillFromEnd(int endIndex = 0, float contentOffset = 0);
    }

    /// <summary>
    /// 列表组件泛型外壳：只加类型化 API，不重复抽象方法。
    /// </summary>
    public abstract class INListViewComponent<T> : INListViewComponent
    {
        /// <summary>类型化数据源（与基类 List 同一份存储）</summary>
        public IList<T> DataList => (IList<T>)List;

        public INListViewComponent<T> Init(string name, IUIFacadeProvider provider, string itemViewID,
            Func<UIFacade, int, View> onCreateItem = null,
            Action<View, T, int> onBindItem = null)
        {
            base.Init(name, provider, itemViewID, onCreateItem,
                onBindItem == null ? null : (Action<View, object, int>)((view, data, index) => onBindItem(view, (T)data, index)));
            return this;
        }

        public INListViewComponent<T> SetList(IList<T> list)
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
