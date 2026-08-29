using System.IO;
using NFramework;
using NFramework.ModuleSystem;
using UnityEditor;
using UnityEngine;

namespace XHFramework
{
    /// <summary>
    /// FrameworkConfig 配置资产的编辑器入口：
    /// 提供菜单快速定位、缺失时自动创建，并清理旧版自动生成的重复资产。
    /// </summary>
    [InitializeOnLoad]
    public static class FrameworkConfigAssetEditor
    {
        public const string ConfigAssetPath = "Assets/Framework/Framework.Config/FrameworkConfig.asset";

        static FrameworkConfigAssetEditor()
        {
            EnsureAssetExists();
            CleanupStaleAssets();
        }

        /// <summary>
        /// 编辑器内获取 FrameworkConfig（不存在则创建）。
        /// </summary>
        public static FrameworkConfig GetConfig()
        {
            return EnsureAssetExists();
        }

        [MenuItem("XHFramework/FrameworkConfig", priority = 1)]
        public static void Open()
        {
            var config = EnsureAssetExists();
            if (config != null)
            {
                Selection.activeObject = config;
                EditorGUIUtility.PingObject(config);
            }
        }

        private static FrameworkConfig EnsureAssetExists()
        {
            var directory = Path.GetDirectoryName(ConfigAssetPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                AssetDatabase.Refresh();
            }

            var config = AssetDatabase.LoadAssetAtPath<FrameworkConfig>(ConfigAssetPath);
            if (config == null)
            {
                config = ScriptableObject.CreateInstance<FrameworkConfig>();
                AssetDatabase.CreateAsset(config, ConfigAssetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log($"[FrameworkConfig] 已创建配置资产: {ConfigAssetPath}");
            }

            return config;
        }

        /// <summary>
        /// 删除旧版自动生成的重复资产：
        /// 它们与收集器中的 FrameworkConfig.asset / UISystemConfig.asset 地址冲突或已废弃。
        /// </summary>
        private static void CleanupStaleAssets()
        {
            const string staleFrameworkConfigPath = "Assets/Framework/Framework.Config/Resources/FrameworkConfig.asset";
            if (AssetDatabase.LoadAssetAtPath<FrameworkConfig>(staleFrameworkConfigPath) != null)
            {
                AssetDatabase.DeleteAsset(staleFrameworkConfigPath);
                Debug.Log($"[FrameworkConfig] 已删除旧版重复配置资产: {staleFrameworkConfigPath}");
            }

            const string staleUiSystemConfigPath = "Assets/Framework/Framework.HotFix/System/UISystem/Resources/UISystemConfig.asset";
            if (AssetDatabase.LoadAssetAtPath<UISystemConfig>(staleUiSystemConfigPath) != null)
            {
                AssetDatabase.DeleteAsset(staleUiSystemConfigPath);
                Debug.Log($"[FrameworkConfig] 已删除旧版独立 UISystemConfig 资产: {staleUiSystemConfigPath}");
            }
        }
    }
}
