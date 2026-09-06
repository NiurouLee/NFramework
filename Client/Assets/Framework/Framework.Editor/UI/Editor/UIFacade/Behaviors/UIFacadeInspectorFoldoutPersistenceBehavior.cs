using UnityEditor;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// 行为：折叠面板展开状态的 EditorPrefs 持久化。
    /// </summary>
    public static class UIFacadeInspectorFoldoutPersistenceBehavior
    {
        public static void Load(UIFacadeInspectorState state)
        {
            if (state.Facade == null) return;

            string foldKey = GetFoldKey(state.Facade);
            state.FoldBasicInfo = EditorPrefs.GetBool(foldKey + "_Basic", true);
            state.FoldUIElementsList = EditorPrefs.GetBool(foldKey + "_UIElements", true);
            state.FoldTools = EditorPrefs.GetBool(foldKey + "_Tools", true);
            state.FoldViewConfig = EditorPrefs.GetBool(foldKey + "_View", true);
        }

        public static void Save(UIFacadeInspectorState state)
        {
            if (state.Facade == null) return;

            string foldKey = GetFoldKey(state.Facade);
            EditorPrefs.SetBool(foldKey + "_Basic", state.FoldBasicInfo);
            EditorPrefs.SetBool(foldKey + "_UIElements", state.FoldUIElementsList);
            EditorPrefs.SetBool(foldKey + "_Tools", state.FoldTools);
            EditorPrefs.SetBool(foldKey + "_View", state.FoldViewConfig);
        }

        private static string GetFoldKey(UIFacade facade)
        {
            return $"UIFacadeFolds_{facade.GetInstanceID()}";
        }
    }
}
