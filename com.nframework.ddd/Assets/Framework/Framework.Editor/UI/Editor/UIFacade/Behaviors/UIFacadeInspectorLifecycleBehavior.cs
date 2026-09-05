using UnityEditor;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// 行为：Inspector 启用/禁用、Target 切换与完整初始化流程。
    /// </summary>
    public static class UIFacadeInspectorLifecycleBehavior
    {
        public static void OnEnable(UIFacadeInspector inspector)
        {
            var state = inspector.State;
            state.Facade = (UIFacade)inspector.target;
            Initialize(state);
            if (state.Facade != null)
                state.LastTargetInstanceID = state.Facade.GetInstanceID();
        }

        public static void OnDisable(UIFacadeInspector inspector)
        {
            var state = inspector.State;
            if (state.Facade == null) return;

            UIFacadeInspectorFoldoutPersistenceBehavior.Save(state);
            EditorUtility.SetDirty(state.Facade);
        }

        public static void EnsureTargetCurrent(UIFacadeInspector inspector)
        {
            if (inspector.target == null) return;

            var state = inspector.State;
            int currentInstanceID = inspector.target.GetInstanceID();
            if (currentInstanceID == state.LastTargetInstanceID) return;

            state.LastTargetInstanceID = currentInstanceID;
            state.Facade = (UIFacade)inspector.target;
            Initialize(state);
        }

        public static void Initialize(UIFacadeInspectorState state)
        {
            UIFacadeInspectorFoldoutPersistenceBehavior.Load(state);
            UIFacadeInspectorDataSyncBehavior.GenerateScriptNameAndID(state);

            state.ViewConfig = UIConfigUtilsEditor.GetViewConfig(state.Facade);
            UIFacadeInspectorDataSyncBehavior.InitializeViewConfig(state);
            UIFacadeInspectorDataSyncBehavior.LoadViewConfigFromJson(state);
        }
    }
}
