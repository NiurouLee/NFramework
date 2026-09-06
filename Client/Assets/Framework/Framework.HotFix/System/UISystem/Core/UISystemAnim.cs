namespace NFramework.ModuleSystem
{
    /// <summary>
    /// 处理打开的Anim
    /// </summary>
    public partial class UISystem
    {
        private void PlayOpenAnim(ViewConfig inViewConfig, WindowRequest inWindowRequest)
        {
            var facade = inWindowRequest.CacheFacadeObj;
            var openAnimCom = facade.GetOpenAnimComponent();
            if (openAnimCom != null && openAnimCom.OpenAnimTime != 0)
            {
                openAnimCom.PlayOpen();
            }

            this.Error?.Print($"OpenAnim id:{inViewConfig.ID}");
        }

        private void PlayCloseAnim(ViewConfig inViewConfig, WindowRequest inWindowRequest)
        {
            var facade = inWindowRequest.CacheFacadeObj;
            var closeAnimCom = facade.GetCloseAnimComponent();
            if (closeAnimCom != null && closeAnimCom.CloseAnimTime != 0)
            {
                closeAnimCom.PlayClose();
            }

            this.Error?.Print($"CloseAnim id:{inViewConfig.ID}");
        }

        private bool HaveCloseAnim(WindowRequest inWindowRequest, out float f)
        {
            f = 0f;
            if (inWindowRequest == null || inWindowRequest.CacheFacadeObj == null)
            {
                return false;
            }

            var closeAnimCom = inWindowRequest.CacheFacadeObj.GetCloseAnimComponent();
            if (closeAnimCom != null && closeAnimCom.CloseAnimTime != 0)
            {
                f = closeAnimCom.CloseAnimTime;
                return true;
            }

            return false;
        }

        private bool HaveOpenAnim(WindowRequest inWindowRequest, out float f)
        {
            f = 0f;
            if (inWindowRequest == null || inWindowRequest.CacheFacadeObj == null)
            {
                return false;
            }

            var openAnimCom = inWindowRequest.CacheFacadeObj.GetOpenAnimComponent();
            if (openAnimCom != null && openAnimCom.OpenAnimTime != 0)
            {
                f = openAnimCom.OpenAnimTime;
                return true;
            }

            return false;
        }
    }
}