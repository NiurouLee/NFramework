using  NFramework.Core;
using    NFramework.ModuleSystem;

namespace NFramework.ModuleSystem
{
    public class UIInputComponentRecords : BaseRecordSet<IUIInputComponent>, IFreeToPool
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
            foreach (var component in this.Records)
            {
                component.UIComponentDestroy();
            }
        }
    }

}