using UnityEngine;
using    NFramework.ModuleSystem;

namespace Game.Logic
{
    public partial class ExampleExwindow : Window
    {
        #region UI Components (Auto Generated)
        public UnityEngine.RectTransform Button1_Copy => Facade.Components[0] as UnityEngine.RectTransform;
        public UnityEngine.UI.Image Button1_Copy_Copy => Facade.Components[1] as UnityEngine.UI.Image;
        public UnityEngine.CanvasRenderer Button2 => Facade.Components[2] as UnityEngine.CanvasRenderer;
        public TMPro.TextMeshProUGUI Button2_tmp => Facade.Components[3] as TMPro.TextMeshProUGUI;
        protected override void OnBindFacade()
        {
        }
        #endregion

        #region  Event Handlers
        private void OnButton1Click(NSimpleButton inButton)
        {
            GetSystem<UISystem>().OpenAsync<MainWindow>();
        }
        private void OnButton2Click(NSimpleButton inButton)
        {
            GetSystem<UISystem>().OpenAsync<MainWindow>();
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
