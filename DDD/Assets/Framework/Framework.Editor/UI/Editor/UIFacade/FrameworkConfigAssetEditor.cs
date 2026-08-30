using System.IO;
using NFramework;
using NFramework.ModuleSystem;
using UnityEditor;
using UnityEngine;

namespace NFramework
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
            // 不要在静态构造器中同步创建/删除资产：
            // 首次导入 FrameworkConfig.asset 时，AssetDatabase 可能正处于导入中途，
            // LoadAssetAtPath 会返回 null（即使文件已存在），随后 CreateAsset 因文件已存在而抛异常，
            // 且异常发生在类型初始化器里，会变成 TypeInitializationException 污染整个类。
            // 改为延迟到编辑器刷新结束后再执行。
            EditorApplication.delayCall += OnEditorReady;
        }

        private static void OnEditorReady()
        {
            // 等待编译/资源导入彻底结束，避免再次触发导入竞态。
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += OnEditorReady;
                return;
            }

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

        [MenuItem("FrameWorkTool/打开 FrameworkConfig", priority = 300)]
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
            if (config != null)
            {
                return config;
            }

            // 文件已存在但加载失败：可能是导入尚未完成，或旧文件损坏/脚本丢失。
            // 先让 Unity 完成一次刷新；仍加载不到就删除旧文件后重建。
            if (File.Exists(ConfigAssetPath))
            {
                AssetDatabase.Refresh();
                config = AssetDatabase.LoadAssetAtPath<FrameworkConfig>(ConfigAssetPath);
                if (config != null)
                {
                    return config;
                }

                Debug.LogWarning($"[FrameworkConfig] 检测到无法加载的旧资产，删除后重建: {ConfigAssetPath}");
                if (!AssetDatabase.DeleteAsset(ConfigAssetPath) && File.Exists(ConfigAssetPath))
                {
                    File.Delete(ConfigAssetPath);
                }
                AssetDatabase.Refresh();
            }

            config = ScriptableObject.CreateInstance<FrameworkConfig>();
            try
            {
                AssetDatabase.CreateAsset(config, ConfigAssetPath);
            }
            catch (UnityException)
            {
                // 创建失败（例如文件刚被写入、尚未完成导入），清理残留后重试一次。
                Debug.LogWarning($"[FrameworkConfig] 首次创建失败，清理后重试: {ConfigAssetPath}");
                if (!AssetDatabase.DeleteAsset(ConfigAssetPath) && File.Exists(ConfigAssetPath))
                {
                    File.Delete(ConfigAssetPath);
                }
                AssetDatabase.Refresh();
                AssetDatabase.CreateAsset(config, ConfigAssetPath);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[FrameworkConfig] 已创建配置资产: {ConfigAssetPath}");

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
