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

        private LoopListViewComponent m_LoopScrollViewList;

        // private SimpleListViewComponent m_SimpleScrollViewList;
        protected override void OnBindFacade()
        {
            this.BindClick(Button, OnButtonClick);
            this.BindClick(Button_close, OnButton_closeClick);
        }

        private void OnButtonClick(NSimpleButton obj)
        {
            if (m_DataList.Count == 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    m_DataList.Add(new FunctionItemData { Name = i.ToString() });
                }
            }

            m_LoopScrollViewList.SetList(m_DataList);
            m_LoopScrollViewList.Refill(0);
        }

        void OnButton_closeClick(NSimpleButton obj)
        {
            m_DataList.Clear();
            m_LoopScrollViewList.Refill();
        }

        #endregion

        #region Event Handlers

        #endregion


        private List<FunctionItemData> m_DataList = new List<FunctionItemData>();

        #region Live

        protected override void OnAwake()
        {
            base.OnAwake();
            InitLoopListView();
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

        private int OnBindLoopItem(int arg1, object arg2)
        {
            return 0;
        }

        private IListView OnCreateLoopItem(UIFacade arg1, IUIFacadeProvider arg2, int arg3, object arg4)
        {
            return this.AddSubViewByFacade<FunctionItemView>(arg1, arg2);
        }

        #endregion
    }
}