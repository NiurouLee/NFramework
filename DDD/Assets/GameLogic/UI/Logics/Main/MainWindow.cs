using UnityEngine;
using    NFramework.ModuleSystem;

namespace Game.Logic
{
    public partial class MainWindow : Window
    {
        #region UI Components (Auto Generated)
        public   NFramework.ModuleSystem.NSimpleButton Button1 => Facade.Components[0] as   NFramework.ModuleSystem.NSimpleButton;
        public   NFramework.ModuleSystem.NSimpleButton Button2 => Facade.Components[1] as   NFramework.ModuleSystem.NSimpleButton;
        public   NFramework.ModuleSystem.NImage Image => Facade.Components[2] as   NFramework.ModuleSystem.NImage;
        public   NFramework.ModuleSystem.NImage Image2 => Facade.Components[3] as   NFramework.ModuleSystem.NImage;
        protected override void OnBindFacade()
        {
            this.BindClick(Button1, OnButton1Click);
            this.BindClick(Button2, OnButton2Click);
        }
        #endregion

        #region  Event Handlers
        private void OnButton1Click(  NFramework.ModuleSystem.NSimpleButton inButton)
        {
            this.Close();
        }
        private void OnButton2Click(  NFramework.ModuleSystem.NSimpleButton inButton)
        {
            Application.Quit();
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
