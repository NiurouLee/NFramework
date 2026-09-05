using UnityEngine;
using    NFramework.ModuleSystem;

namespace Game.Logic
{
    public partial class EventcenterWindow : Window
    {
        #region UI Components (Auto Generated)
        public UnityEngine.RectTransform Top => Facade.Components[0] as UnityEngine.RectTransform;
        public UnityEngine.RectTransform LeftTop => Facade.Components[1] as UnityEngine.RectTransform;
        public UnityEngine.RectTransform RightTop => Facade.Components[2] as UnityEngine.RectTransform;
        public UnityEngine.RectTransform Bottom => Facade.Components[3] as UnityEngine.RectTransform;
        public UnityEngine.UI.LoopVerticalScrollRect LoopScrollView => Facade.Components[4] as UnityEngine.UI.LoopVerticalScrollRect;
        public NFramework.ModuleSystem.PrefabSourceMap LoopScrollViewPrefabMap => Facade.Components[5] as NFramework.ModuleSystem.PrefabSourceMap;
        public UnityEngine.UI.ScrollRect SimpleScrollView => Facade.Components[6] as UnityEngine.UI.ScrollRect;
        public NFramework.ModuleSystem.PrefabSourceMap SimpleScrollViewPrefabMap => Facade.Components[7] as NFramework.ModuleSystem.PrefabSourceMap;
        public NFramework.ModuleSystem.NSimpleButton Button => Facade.Components[8] as NFramework.ModuleSystem.NSimpleButton;
        public NFramework.ModuleSystem.NSimpleButton Button_close => Facade.Components[9] as NFramework.ModuleSystem.NSimpleButton;

        protected override void OnBindFacade()
        {
this.BindClick(Button, OnButtonClick);
this.BindClick(Button_close, OnButton_closeClick);

        }
        #endregion

        #region  Event Handlers
private void OnButtonClick(NFramework.ModuleSystem.NSimpleButton inButton)
        {
            
        }
private void OnButton_closeClick(NFramework.ModuleSystem.NSimpleButton inButton)
        {
            this.Close();
        }

        #endregion


        #region  Live

        protected override void OnShow()
        {
        }

        protected override void OnHide()
        {
        }
        #endregion


    }
}
