using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;

using XHFramework;
namespace NFramework.ModuleSystem
{
    /// <summary>
    /// ViewTypeRegistry代码生成器，生成Type与ViewConfigID的映射字典
    /// </summary>
    public static class ViewTypeRegistryGenerator
    {
        /// <summary>
        /// 生成ViewTypeRegistryAuto类，包含所有View类型与ViewConfigID的映射
        /// </summary>
        public static bool GenerateRegistryClass(out string errorMessage)
        {
            errorMessage = "";

            // 读取所有ViewConfig
            ViewConfigManager.ViewConfigsContainer container = ViewConfigManager.LoadAllViewConfigs();
            if (container == null || container.Configs == null || container.Configs.Count == 0)
            {
                errorMessage = "ViewConfigs.json为空，无法生成注册表";
                return false;
            }

            // 检查是否有重复的ID
            if (ViewConfigManager.HasDuplicateIDs(container.Configs, out List<string> duplicateIDs))
            {
                errorMessage = $"ViewConfigs.json中存在重复的配置ID，无法生成注册表！\n\n重复的ID:\n{string.Join("\n", duplicateIDs)}";
                return false;
            }

            // 过滤有效的配置
            List<ViewConfigManager.ViewConfigData> validConfigs = new List<ViewConfigManager.ViewConfigData>();
            foreach (var configData in container.Configs)
            {
                if (!string.IsNullOrEmpty(configData.ID))
                {
                    validConfigs.Add(configData);
                }
            }

            if (validConfigs.Count == 0)
            {
                errorMessage = "没有有效的配置数据，无法生成注册表";
                return false;
            }

            // 生成代码
            string generatedFolderPath = FrameworkConfigAssetEditor.GetConfig().UISystemConfig.ViewTypeGeneratedPath;
            if (!Directory.Exists(generatedFolderPath)) Directory.CreateDirectory(generatedFolderPath);

            string generatedFilePath = FrameworkConfigAssetEditor.GetConfig().UISystemConfig.ViewTypeRegistryGeneratedPath;

            // 清理其他位置的历史生成文件，确保项目里只有一个注册类
            CleanupStaleRegistryFiles(generatedFilePath);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("");

            // 收集所有唯一的namespace
            HashSet<string> namespaces = new HashSet<string>();
            foreach (var configData in validConfigs)
            {
                if (!string.IsNullOrEmpty(configData.Namespace))
                {
                    namespaces.Add(configData.Namespace);
                }
            }

            // 添加using语句
            foreach (var ns in namespaces)
            {
                sb.AppendLine($"using {ns};");
            }

            sb.AppendLine("");
            sb.AppendLine("// 自动生成代码，请勿手动修改");
            sb.AppendLine("namespace Game.Logic");
            sb.AppendLine("{");
            sb.AppendLine("    /// <summary>");
            sb.AppendLine("    /// 自动生成的View类型映射表");
            sb.AppendLine("    /// </summary>");
            sb.AppendLine("    public static partial class ViewTypeRegistryAuto");
            sb.AppendLine("    {");
            sb.AppendLine("        /// <summary>");
            sb.AppendLine("        /// Type到ViewConfigID的映射字典（由代码生成器填充）");
            sb.AppendLine("        /// </summary>");
            sb.AppendLine("        public static Dictionary<Type, string> TypeToConfigIDMap = new Dictionary<Type, string>");
            sb.AppendLine("        {");

            // 生成字典条目
            foreach (var configData in validConfigs)
            {
                sb.AppendLine($"            {{ typeof({configData.ID}), \"{configData.ID}\" }},");
            }

            sb.AppendLine("        };");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            try
            {
                File.WriteAllText(generatedFilePath, sb.ToString());
                AssetDatabase.Refresh();
                return true;
            }
            catch (System.Exception ex)
            {
                errorMessage = $"生成注册表代码失败: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// 删除其他目录下残留的 ViewTypeRegistryAuto.Generated.cs，
        /// 避免配置路径变更后出现重复的注册类
        /// </summary>
        private static void CleanupStaleRegistryFiles(string targetFilePath)
        {
            string[] guids = AssetDatabase.FindAssets("ViewTypeRegistryAuto");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (string.IsNullOrEmpty(path))
                {
                    continue;
                }

                if (!string.Equals(Path.GetFileName(path), "ViewTypeRegistryAuto.Generated.cs",
                        System.StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (string.Equals(path, targetFilePath, System.StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                AssetDatabase.DeleteAsset(path);
            }
        }
    }
}
