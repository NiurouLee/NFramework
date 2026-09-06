namespace NFramework.ModuleSystem
{
    /// <summary>
    /// 列表 item 接口：由列表中的 item View 实现。
    /// 列表组件（LoopListView / SimpleListViewComponent）绑定数据时会赋值 Index/Data 并调用 Refresh，
    /// item 被回收时调用 OnRecycle。
    /// 职责参考 SwiftLoopScrollRect.IListItem。
    /// </summary>
    public interface IListView
    {
        /// <summary>当前所在的数据索引，由列表组件赋值</summary>
        int Index { get; set; }

        /// <summary>当前绑定的数据，由列表组件赋值</summary>
        object Data { get; set; }

        /// <summary>刷新：无数据（空数据/占位）</summary>
        void Refresh(int index);

        /// <summary>刷新：带数据</summary>
        void Refresh(int index, object data);

        /// <summary>item 被回收前调用，用于清理临时状态</summary>
        void OnRecycle();
    }
}
