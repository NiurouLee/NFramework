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

        public int State { get; private set; }

        /// <summary>数据源（object 级）</summary>
        public IList List { get; protected set; }

        /// <summary>Item Facade 提供者（接入框架 UIFacadeProvider）</summary>
        public IUIFacadeProvider Provider { get; protected set; }

        /// <summary>
        /// 创建View
        /// </summary>
        public Func<UIFacade, IUIFacadeProvider, int, object, IListView> OnCreateItemView { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public Func<int, object, int> OnGetUIFacadeIndex { get; protected set; }

        protected virtual void Init(string name,
            Func<UIFacade, IUIFacadeProvider, int, object, IListView> OnCreateItemView = null,
            Func<int, object, int> OnGetUIFacadeIndex = null)
        {
            Name = name;
            this.OnCreateItemView = OnCreateItemView;
            this.OnGetUIFacadeIndex = OnGetUIFacadeIndex;
        }

        public virtual void InitProvider(IUIFacadeProvider provider)
        {
            this.Provider = provider;
        }

        public INListViewComponent SetList(IList list)
        {
            List = list;
            return this;
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
}