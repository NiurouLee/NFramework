using UnityEngine;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// UIFacade Inspector 会话状态（由 Inspector 持有，行为类读写）。
    /// </summary>
    public sealed class UIFacadeInspectorState
    {
        public UIFacade Facade;
        public ViewConfig ViewConfig;
        public bool FoldBasicInfo = true;
        public bool FoldUIElementsList = true;
        public bool FoldTools = true;
        public bool FoldViewConfig = true;
        public int LastTargetInstanceID = -1;
    }
}
