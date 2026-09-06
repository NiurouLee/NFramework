using NFramework.ModuleSystem;
using TMPro;

namespace Game.Logic
{
    /// <summary>
    /// 右侧功能列表 item 数据
    /// </summary>
    public class FunctionItemData
    {
        public string Name;
    }

    /// <summary>
    /// 右侧功能列表 item View（对应 FunctionButton_template prefab）
    /// </summary>
    public class FunctionItemView : View, IListView
    {
        public int Index { get; set; }
        public object Data { get; set; }

        private TextMeshProUGUI m_Label;

        protected override void OnBindFacade()
        {
            m_Label = Facade != null ? Facade.Cast<TextMeshProUGUI>(0) : null;
        }

        public void Refresh(int index)
        {
            Index = index;
        }

        public void Refresh(int index, object data)
        {
            Index = index;
            Data = data;
            if (m_Label != null)
            {
                m_Label.text = data is FunctionItemData itemData && !string.IsNullOrEmpty(itemData.Name)
                    ? itemData.Name
                    : "";
            }
        }

        public void OnRecycle()
        {
            Data = null;
        }
    }
}