using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using NFramework.ModuleSystem;

namespace NFramework.ModuleSystem
{
    public enum UILayer
    {
        HUD = 2000,
        Basic = 3000,
        Popup = 4000,
        PopupHigh = 5000,
        Tips = 6000,
        SystemTips = 7000,
    }

    public abstract class UILayerServices
    {
        public static short OneUiSortOder = 50;
        private GameObject go;
        public GameObject Go => go;
        protected List<Window> stack;
        protected Dictionary<string, Window> windowMap;
        protected UISystem UISystem;

        public UILayerServices(UISystem inUISystem, GameObject inGo)
        {
            this.UISystem = inUISystem;
            this.go = inGo;
            this.stack = new List<Window>();
            this.windowMap = new Dictionary<string, Window>();
        }

        public bool TryGetWindow(string inWindowID, out Window outWindow)
        {
            if (this.windowMap.TryGetValue(inWindowID, out outWindow))
            {
                return true;
            }

            outWindow = null;
            return false;
        }

        public abstract void PushWindow(Window inWindow, ViewConfig inViewConfig, UIFacade inFacade);
        public abstract void PopWindow(ViewConfig inViewConfig);
    }

    public class UIStackLayerServices : UILayerServices
    {
        private List<int> orders;

        private int StartLayer = 1000;

        public UIStackLayerServices(UISystem inUISystem, GameObject inGo) : base(inUISystem, inGo)
        {
            orders = new List<int>(1000);
        }

        public override void PushWindow(Window inWindow, ViewConfig inViewConfig, UIFacade inFacade)
        {
            this.stack.Add(inWindow);
            this.windowMap.Add(inViewConfig.ID, inWindow);
            var canvas = inFacade.GetOrAddComponent<Canvas>();
            inFacade.transform.SetParent(this.Go.transform);
            this.orders.Sort();
            var currentMax = this.orders.Count > 0 ? this.orders[^1] : StartLayer;
            var newOrder = currentMax + OneUiSortOder;
            this.orders.Add(newOrder);
            canvas.sortingOrder = newOrder;
        }

        public override void PopWindow(ViewConfig inViewConfig)
        {
            var id = inViewConfig.ID;
            if (this.windowMap.TryGetValue(id, out var window))
            {
                var order = window.Order;
                this.stack.Remove(window);
                this.windowMap.Remove(inViewConfig.ID);
                this.orders.Remove(order);
            }
        }
    }

    public class UIFixedLayerServices : UILayerServices
    {
        public UIFixedLayerServices(UISystem inUISystem, GameObject inGo) : base(inUISystem, inGo)
        {
        }

        public override void PushWindow(Window inWindow, ViewConfig inViewConfig, UIFacade inFacade)
        {
            stack.Add(inWindow);
            this.windowMap.Add(inViewConfig.ID, inWindow);
            inFacade.transform.SetParent(this.Go.transform);
            var canvas = inFacade.GetOrAddComponent<Canvas>();
            canvas.sortingOrder = inViewConfig.Layer;
        }

        public override void PopWindow(ViewConfig inViewConfig)
        {
            if (this.windowMap.TryGetValue(inViewConfig.ID, out var window))
            {
                this.UISystem.Close(window);
                this.stack.Remove(window);
                this.windowMap.Remove(inViewConfig.ID);
            }
        }
    }
}