namespace NFramework.ModuleSystem
{
    /// <summary>
    /// 选中功能接口：需要支持选中/取消选中的 item View 实现。
    /// 列表组件会维护当前选中索引，并在绑定/复用 item 时同步 IsSelected、调用 OnChangeSelect。
    /// 职责参考 SwiftLoopScrollRect.IListItem 的选中部分。
    /// </summary>
    public interface ISelectView
    {
        /// <summary>当前是否选中</summary>
        bool IsSelected { get; set; }

        /// <summary>是否允许被选中</summary>
        bool CanSelect { get; }

        /// <summary>选中状态变化时调用，用于更新高亮等表现</summary>
        void OnChangeSelect();

        /// <summary>点击了不可选中的 item 时调用，用于禁用反馈</summary>
        void OnClickDisable();
    }
}
