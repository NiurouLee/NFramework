using System;
using System.Collections.Generic;
using UnityEngine;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// UI 层级。每个枚举成员对应一个独立的 Stack 层级，
    /// 枚举值同时作为该 Stack 的初始 sortingOrder（窗口会在层内继续 +OneUiSortOrder 叠加）。
    /// ViewConfig.SetLayer((ushort)UILayer.Xxx) 决定窗口属于哪个层级。
    /// </summary>
    public enum UILayer
    {
        HUD = 1000,
        Basic = 3000,
        Popup = 4000,
        PopupMid = 5000,
        PopupHigh = 6000,
        Tips = 7000,
        Guide = 8000,
        GuidTips = 9000,
        Loading = 10000,
        SystemTips = 11000,
    }

    /// <summary>
    /// 层级服务基类：每个 UILayer 对应一个实例。
    /// 层级内只按窗口实例管理，不按窗口类型 ID 管理，
    /// 这样同一个 ViewConfig 配不同 key 打开多个实例时不会互相覆盖/冲突。
    /// </summary>
    public abstract class UILayerServices
    {
        /// <summary>同一层级内相邻窗口的 sortingOrder 间隔</summary>
        public static short OneUiSortOder = 50;

        private GameObject go;

        public GameObject Go => go;

        protected List<Window> stack;
        protected HashSet<Window> windowSet;
        protected UISystem UISystem;

        /// <summary>当前层内打开的窗口数量</summary>
        public int Count => this.stack.Count;

        /// <summary>当前层是否为空</summary>
        public bool IsEmpty => this.stack.Count == 0;

        public UILayerServices(UISystem inUISystem, GameObject inGo)
        {
            this.UISystem = inUISystem;
            this.go = inGo;
            this.stack = new List<Window>();
            this.windowSet = new HashSet<Window>();
        }

        public bool Contains(Window inWindow)
        {
            return inWindow != null && this.windowSet.Contains(inWindow);
        }

        public abstract void PushWindow(Window inWindow, ViewConfig inViewConfig, UIFacade inFacade, int inOrder);

        public abstract void PopWindow(Window inWindow,int inOrder);

        public abstract int GetOrder();

        public abstract int ReturnOrder(int inOrder);

        public abstract bool CheckOrderSafe(int inOrder);
    }

    /// <summary>
    /// 单个 UILayer 对应的 Stack 层级。
    /// sortingOrder 从该层枚举值开始，每次 Push 在当前层最大值上 +OneUiSortOder。
    /// </summary>
    public class UIStackLayerServices : UILayerServices
    {
        private readonly List<int> orders;

        /// <summary>该 Stack 对应的 UILayer 枚举值（也是基础 sortingOrder）</summary>
        public ushort Layer { get; }

        public UIStackLayerServices(UISystem inUISystem, GameObject inGo, ushort inLayer) : base(inUISystem, inGo)
        {
            this.Layer = inLayer;
            this.orders = new List<int>(16);
        }

        public override void PushWindow(Window inWindow, ViewConfig inViewConfig, UIFacade inFacade, int inOrder)
        {
            if (inWindow == null || !this.windowSet.Add(inWindow))
            {
                return;
            }

            if (this.CheckOrderSafe(inOrder))
            {
                this.stack.Add(inWindow);
                var canvas = inFacade.GetOrAddComponent<Canvas>();
                inFacade.transform.SetParent(this.Go.transform);
                this.orders.Sort();
                canvas.sortingOrder = inOrder;
            }
            else
            {
                throw new Exception("Order error");
            }
        }


        public override void PopWindow(Window inWindow,int inOrder)
        {
            if (inWindow == null)
            {
                this.ReturnOrder(inOrder);
                return;
            }

            this.windowSet.Remove(inWindow);
            this.stack.Remove(inWindow);
            this.ReturnOrder(inOrder);
        }

        public override int GetOrder()
        {
            if (this.orders.Count == 0)
            {
                this.orders.Add(this.Layer);
                return this.Layer;
            }

            var newOrder = this.orders[^1] + 50;
            this.orders.Add(newOrder);

            return this.orders.Count > 0 ? this.orders[^1] : this.Layer;
        }

        public override int ReturnOrder(int inOrder)
        {
            this.orders.Remove(inOrder);
            return inOrder;
        }

        public override bool CheckOrderSafe(int inLayer)
        {
            return inLayer >= this.Layer && this.orders.Contains(inLayer);
        }
    }
}
