using System.Collections.Generic;
using    NFramework.ModuleSystem;
using   NFramework.ModuleSystem;
using Unity.VisualScripting;

namespace NFramework.ModuleSystem
{
    public partial class UISystem
    {

        public Dictionary<string, UIPoolEntity> PoolDictionary = new Dictionary<string, UIPoolEntity>();
        private void _Close(string inWindowName)
        {
            if (string.IsNullOrEmpty(inWindowName))
            {
                this.GetSystem<LoggerSystem>().ErrStack($"UIM::Close inWindowName is null");

            }
            var vc = GetViewConfig(inWindowName);
            if (this.CheckWindowReq(vc, out var outWindowRequest))
            {
                if (outWindowRequest.Stage == WindowRequestStage.WindowOpen)
                {
                    this.__Close(outWindowRequest.CacheWindowObj, vc);
                }
                else if (outWindowRequest.Stage == WindowRequestStage.FacadeLoading)
                {
                    outWindowRequest.Cancel();
                }
            }
        }

        /// <summary>
        ///  关键所在，要考虑清除到底缓存什么，
        /// </summary>
        /// <param name="inWindow"></param>
        /// <param name="inViewConfig"></param>
        private void __Close(Window inWindow, ViewConfig inViewConfig)
        {
            inWindow.Hide();
            if (inViewConfig.IsFixedLayer)
            {
                this.m_FixedLayer.PopWindow(inViewConfig);
            }
            else
            {
                this.m_StackLayer.PopWindow(inViewConfig);
            }
            var provider = inWindow.Provider as UIFacadeProviderDynamic;
            var resLoader = inWindow.GetComponent<ViewResLoadComponent>();
            var facade = inWindow.Facade;
            this.RemoveWindowRequest(inViewConfig.ID);
            inWindow.Destroy();

            // //入池
            // var poolEntity = new UIPoolEntity()
            // {
            //     ID = inViewConfig.ID,
            //     Entity = provider,
            //     ResLoader = resLoader,
            //     Facade = facade,
            //     Window = inWindow
            // };
            // this.PoolDictionary.Add(inViewConfig.ID, poolEntity);
        }

        public bool TryGetByPool(string inID, out UIFacadeProviderDynamic providerDynamic, out ViewResLoadComponent resLoader, out UIFacade facade, out Window window)
        {
            if (this.PoolDictionary.TryGetValue(inID, out var poolEntity))
            {
                providerDynamic = poolEntity.Entity;
                resLoader = poolEntity.ResLoader;
                facade = poolEntity.Facade;
                window = poolEntity.Window;
                this.PoolDictionary.Remove(inID);
                return true;
            }
            else
            {
                providerDynamic = null;
                resLoader = null;
                facade = null;
                window = null;
                return false;
            }
        }


        public struct UIPoolEntity
        {
            public string ID;
            public UIFacadeProviderDynamic Entity;
            public ViewResLoadComponent ResLoader;
            public UIFacade Facade;
            public Window Window;
        }
    }


}