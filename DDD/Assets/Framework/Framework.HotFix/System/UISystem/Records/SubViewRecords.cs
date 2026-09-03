using NFramework.Core;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// 用于记录子view,这一层直接受 有view,有provider,有facade
    /// </summary>
    public class SubViewRecords : BaseRecordSet<View>, IFreeToPool
    {
        private View m_orderView;

        public void FreeToPool()
        {
        }

        public void SetView(View inOrder)
        {
            m_orderView = inOrder;
        }

        protected override void OnDestroy()
        {
            foreach (var view in this.Records)
            {
                view.Destroy();
            }
        }

        public T AddSubViewByFacadeAndSetParent<T>(T inView, UIFacade inFacade, IUIFacadeProvider inProvider)
            where T : View
        {
            inView.SetParent(this.m_orderView);
            this._AddChild(inView);
            inView.SetUIFacade(inFacade, inProvider);
            inView.Awake();
            return inView;
        }

        public T AddSubViewByFacade<T>(T inView, UIFacade inFacade, IUIFacadeProvider inProvider) where T : View
        {
            inView.SetParent(this.m_orderView);
            this._AddChild(inView);
            inView.SetUIFacade(inFacade, inProvider);
            inView.Awake();
            return inView;
        }

        public T AddSubViewByFacade<T, D>(T inView, UIFacade inFacade, IUIFacadeProvider inProvider, D inData)
            where T : View, IViewSetData<D>
        {
            inView.SetParent(this.m_orderView);
            this._AddChild(inView);
            inView.SetUIFacade(inFacade, inProvider);
            if (inView is IViewSetData<D> viewSetData)
            {
                viewSetData.InitData(inData);
            }

            inView.Awake();
            return inView;
        }

        private bool _AddChild(View inView)
        {
            if (inView == null)
            {
                return false;
            }

            return this.TryAdd(inView);
        }

        public T RemoveSubView<T>(T inView) where T : View
        {
            if (this.TryRemove(inView))
            {
                inView.Destroy();
            }

            return inView;
        }
    }
}
