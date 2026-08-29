using UnityEngine;
using System;

namespace NFramework.ModuleSystem
{
    public partial class View
    {
        public UIFacade Facade { get; private set; }

        //记录Facade的Provider,根据不同的Facade 进行不同的归还策略
        public IUIFacadeProvider Provider { get; private set; }

        ///
        public void SetUIFacade(UIFacade inUIFacade, IUIFacadeProvider inProvider)
        {
            if (inUIFacade == null)
            {
                throw new Exception("SetUIFacade: inUIFacade is null");
            }

            if (inProvider == null)
            {
                throw new Exception("SetUIFacade: inProvider is null");
            }

            if (this.Facade != null)
            {
                throw new Exception("SetUIFacade: Facade is null");
            }

            if (this.Provider != null)
            {
                throw new Exception("SetUIFacade: Provider is null");
            }
            this.Provider = inProvider;
            inUIFacade.UIComponentAwake();
            this.Facade = inUIFacade;
            this.RectTransform = inUIFacade.GetComponent<RectTransform>();
            this.OnBindFacade();
        }

        protected virtual void OnBindFacade()
        {
        }

        private void DestroyFacade()
        {
            this.Facade.UIComponentDestroy();
            this.Provider.Free(this.Facade);
            this.Facade = null;
            this.Provider = null;
            this.RectTransform = null;
        }
    }
}