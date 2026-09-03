using System;
using System.Collections.Generic;
using UnityEngine;
using NFramework.ModuleSystem;
using Unity.VisualScripting;


namespace Game.Logic
{
    public partial class MainWindow : Window
    {
        #region UI Components (Auto Generated)

        public UnityEngine.RectTransform Top => Facade.Components[0] as UnityEngine.RectTransform;
        public UnityEngine.RectTransform LeftTop => Facade.Components[1] as UnityEngine.RectTransform;
        public UnityEngine.RectTransform RightTop => Facade.Components[2] as UnityEngine.RectTransform;
        public UnityEngine.RectTransform Bottom => Facade.Components[3] as UnityEngine.RectTransform;

        public UnityEngine.UI.LoopVerticalScrollRect LoopScrollView =>
            Facade.Components[4] as UnityEngine.UI.LoopVerticalScrollRect;

        public NFramework.ModuleSystem.PrefabSourceMap LoopScrollViewPrefabMap =>
            Facade.Components[5] as NFramework.ModuleSystem.PrefabSourceMap;

        public UnityEngine.UI.ScrollRect SimpleScrollView => Facade.Components[6] as UnityEngine.UI.ScrollRect;

        public NFramework.ModuleSystem.PrefabSourceMap SimpleScrollViewPrefabMap =>
            Facade.Components[7] as NFramework.ModuleSystem.PrefabSourceMap;

        public NFramework.ModuleSystem.NSimpleButton Button =>
            Facade.Components[8] as NFramework.ModuleSystem.NSimpleButton;

        public NFramework.ModuleSystem.NSimpleButton Button_close =>
            Facade.Components[9] as NFramework.ModuleSystem.NSimpleButton;

        public NFramework.ModuleSystem.NSimpleButton Button_openEventCenter =>
            Facade.Components[10] as NFramework.ModuleSystem.NSimpleButton;

        private LoopListViewComponent m_LoopScrollViewList;

        private SimpleListViewComponent m_SimpleScrollViewList;
        protected override void OnBindFacade()
        {
            this.BindClick(Button, OnButtonClick);
            this.BindClick(Button_close, OnButton_closeClick);
            this.BindClick(Button_openEventCenter, OnButton_openEventCenterClick);
        }

        #endregion

        #region Event Handlers

        #endregion


        private List<FunctionItemData> m_DataList = new List<FunctionItemData>();


        private void OnButtonClick(NSimpleButton obj)
        {
            this.m_LoopScrollViewList.SetList(this.m_DataList);
            this.m_SimpleScrollViewList?.SetList(this.m_DataList);
            for (int i = 0; i < 10; i++)
            {
                m_DataList.Add(new FunctionItemData() { Name = i.ToString() });
            }

            this.m_LoopScrollViewList.Refill();
            this.m_SimpleScrollViewList?.Refill();
        }

        private void OnButton_openEventCenterClick(NSimpleButton obj)
        {
            GetSystem<UISystem>().OpenAsync<EventcenterWindow>();
        }

        private void OnButton_closeClick(NSimpleButton obj)
        {
            this.m_DataList.Clear();
            this.m_LoopScrollViewList.Refill();
            this.m_SimpleScrollViewList?.Refill();
        }

        #region Live

        protected override void OnAwake()
        {
            base.OnAwake();
            InitLoopListView();
            InitSimpleListView();
        }

        protected override void OnShow()
        {
        }

        protected override void OnHide()
        {
        }


        private void InitLoopListView()
        {
            m_LoopScrollViewList = new LoopListViewComponent();
            m_LoopScrollViewList
                .BindScrollRect(LoopScrollView)
                .Init("LoopListView1", LoopScrollViewPrefabMap, OnCreateLoopItem, OnBindLoopItem);
        }

        private void InitSimpleListView()
        {
            PrepareSimpleScrollContent();

            m_SimpleScrollViewList = new SimpleListViewComponent();
            m_SimpleScrollViewList
                .BindScrollRect(SimpleScrollView)
                .Init("SimpleListView1", SimpleScrollViewPrefabMap, OnCreateSimpleItem, OnBindSimpleItem);
        }

        /// <summary>
        /// 测试用：为 SimpleScrollView 的 Content 补上纵向布局，
        /// 否则 SimpleListView 生成的多个子项会叠在一起。
        /// </summary>
        private void PrepareSimpleScrollContent()
        {
            if (SimpleScrollView == null)
            {
                return;
            }

            // 普通列表按纵向测试
            SimpleScrollView.vertical = true;
            SimpleScrollView.horizontal = false;

            var content = SimpleScrollView.content;
            if (content == null)
            {
                return;
            }

            var verticalLayout = content.GetComponent<UnityEngine.UI.VerticalLayoutGroup>();
            if (verticalLayout == null)
            {
                verticalLayout = content.gameObject.AddComponent<UnityEngine.UI.VerticalLayoutGroup>();
            }

            // item 模板自带高度，布局组只负责纵向排列，不强制拉伸高度
            verticalLayout.childControlHeight = false;
            verticalLayout.childForceExpandHeight = false;

            if (content.GetComponent<UnityEngine.UI.ContentSizeFitter>() == null)
            {
                var fitter = content.gameObject.AddComponent<UnityEngine.UI.ContentSizeFitter>();
                fitter.horizontalFit = UnityEngine.UI.ContentSizeFitter.FitMode.Unconstrained;
                fitter.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;
            }
        }

        private int OnBindLoopItem(int arg1, object arg2)
        {
            return 0;
        }

        private IListView OnCreateLoopItem(UIFacade arg1, IUIFacadeProvider arg2, int arg3, object arg4)
        {
            return this.AddSubViewByFacade<FunctionItemView>(arg1, arg2);
        }

        private int OnBindSimpleItem(int arg1, object arg2)
        {
            return 0;
        }

        private IListView OnCreateSimpleItem(UIFacade arg1, IUIFacadeProvider arg2, int arg3, object arg4)
        {
            return this.AddSubViewByFacade<FunctionItemView>(arg1, arg2);
        }

        #endregion
    }
}
