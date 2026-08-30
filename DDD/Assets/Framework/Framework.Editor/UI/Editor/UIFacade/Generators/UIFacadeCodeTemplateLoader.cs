using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

using NFramework;
namespace NFramework.ModuleSystem
{
    /// <summary>
    /// 加载 UIFacade 代码片段：属性模板仍用 <c>Types/&lt;类型&gt;/ComponentProperty.txt</c> 与 <c>Default/</c>；
    /// 需要绑定 + 事件桩的组件由 <see cref="InteractTemplateIdByComponentTypeName"/> 映射到
    /// <c>InteractTemplates/{模板Id}.txt</c>（单文件内 <<<Bind>>> / <<<Event>>> 两段）。
    /// </summary>
    internal static class UIFacadeCodeTemplateLoader
    {
        public const string ComponentPropertyFile = "ComponentProperty.txt";

        private const string InteractTemplatesFolder = "InteractTemplates";
        private const string MarkBind = "<<<Bind>>>";
        private const string MarkEvent = "<<<Event>>>";

        /// <summary>
        /// 组件 <see cref="System.Type.Name"/> → 交互模板集 Id（对应磁盘文件 <c>InteractTemplates/{值}.txt</c>）。
        /// 扩展新的可点击/可绑定 UI 组件时，在此增加一项即可。
        /// </summary>
        public static readonly Dictionary<string, string> InteractTemplateIdByComponentTypeName =
            new Dictionary<string, string>
            {
                ["NSimpleButton"] = "ButtonInteraction",
                ["Button"] = "ButtonInteraction",
            };

        private static string DiskRoot()
        {
            string asset = FrameworkConfigAssetEditor.GetConfig().UISystemConfig.UIFacadeCodeTemplatesRoot.Replace('\\', '/');
            const string prefix = "Assets/";
            if (!asset.StartsWith(prefix, System.StringComparison.OrdinalIgnoreCase))
                return Path.GetFullPath(asset);
            string tail = asset.Substring(prefix.Length).Replace('/', Path.DirectorySeparatorChar);
            return Path.Combine(Application.dataPath, tail);
        }

        private static string AssetPathToAbsolute(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
                return assetPath;
            assetPath = assetPath.Replace('\\', '/');
            const string assetsPrefix = "Assets/";
            if (assetPath.StartsWith(assetsPrefix, System.StringComparison.OrdinalIgnoreCase))
            {
                string underAssets = assetPath.Substring(assetsPrefix.Length).Replace('/', Path.DirectorySeparatorChar);
                return Path.Combine(Application.dataPath, underAssets);
            }

            return Path.GetFullPath(assetPath);
        }

        /// <summary>完整类壳：新路径优先，回退旧版 Template/UIFacadeTemplate.txt。</summary>
        public static string LoadClassShellTemplate()
        {
            string primary = AssetPathToAbsolute(FrameworkConfigAssetEditor.GetConfig().UISystemConfig.UIFacadeClassShellTemplatePath.Replace('\\', '/'));
            if (File.Exists(primary))
                return File.ReadAllText(primary, Encoding.UTF8);

            string legacy = Path.Combine(Application.dataPath, "NFramework.Game/HotUpdateScripts/UI/Template/UIFacadeTemplate.txt");
            if (File.Exists(legacy))
                return File.ReadAllText(legacy, Encoding.UTF8);

            return null;
        }

        /// <param name="typeShortName">组件 Type.Name，用于查找 Types 子目录；传空则只用 Default。</param>
        public static string LoadSnippet(string typeShortName, string fileName)
        {
            typeShortName ??= "";

            if (!string.IsNullOrEmpty(typeShortName))
            {
                string typedPath = Path.Combine(DiskRoot(), "Types", typeShortName, fileName);
                if (File.Exists(typedPath))
                    return File.ReadAllText(typedPath, Encoding.UTF8);
            }

            string defPath = Path.Combine(DiskRoot(), "Default", fileName);
            if (File.Exists(defPath))
                return File.ReadAllText(defPath, Encoding.UTF8);

            return null;
        }

        /// <summary>
        /// 读取并拆分交互模板（Bind + Event 在同一份 txt 内）。
        /// </summary>
        public static bool TryLoadInteractTemplate(string templateId, out string bindTemplate, out string eventTemplate, out string error)
        {
            bindTemplate = null;
            eventTemplate = null;
            error = null;

            if (string.IsNullOrEmpty(templateId))
            {
                error = "交互模板 Id 为空。";
                return false;
            }

            string path = Path.Combine(DiskRoot(), InteractTemplatesFolder, templateId + ".txt");
            if (!File.Exists(path))
            {
                error = $"找不到交互模板文件：{InteractTemplatesFolder}/{templateId}.txt\n完整路径：{path}";
                return false;
            }

            string raw = File.ReadAllText(path, Encoding.UTF8);
            return TrySplitInteractTemplate(raw, out bindTemplate, out eventTemplate, out error);
        }

        public static bool TrySplitInteractTemplate(string fileContent, out string bindTemplate, out string eventTemplate, out string error)
        {
            bindTemplate = null;
            eventTemplate = null;
            error = null;

            if (string.IsNullOrEmpty(fileContent))
            {
                error = "交互模板内容为空。";
                return false;
            }

            int iBind = fileContent.IndexOf(MarkBind, System.StringComparison.Ordinal);
            int iEvent = fileContent.IndexOf(MarkEvent, System.StringComparison.Ordinal);
            if (iBind < 0 || iEvent < 0 || iEvent <= iBind)
            {
                error = $"交互模板须包含分段标记（顺序：{MarkBind} 在前，{MarkEvent} 在后）。";
                return false;
            }

            bindTemplate = fileContent.Substring(iBind + MarkBind.Length, iEvent - (iBind + MarkBind.Length)).Trim();
            eventTemplate = fileContent.Substring(iEvent + MarkEvent.Length).Trim();
            return true;
        }

        /// <summary>若未注册且全名像按钮，打编辑器提示。</summary>
        public static bool TryGetInteractTemplateId(string componentTypeShortName, string componentTypeFullName, out string templateId)
        {
            if (!string.IsNullOrEmpty(componentTypeShortName) &&
                InteractTemplateIdByComponentTypeName.TryGetValue(componentTypeShortName, out templateId))
                return true;

            templateId = null;
            if (!string.IsNullOrEmpty(componentTypeFullName) &&
                componentTypeFullName.IndexOf("Button", System.StringComparison.Ordinal) >= 0)
            {
                Debug.LogWarning(
                    "[UIFacade 生成] 组件类型 \"" + (componentTypeShortName ?? "") +
                    "\" 未在 " + nameof(InteractTemplateIdByComponentTypeName) +
                    " 中注册，已跳过绑定与事件桩。请在本类字典中添加，例如：[\"" +
                    (componentTypeShortName ?? "YourComponent") + "\"] = \"ButtonInteraction\",",
                    null);
            }

            return false;
        }

        public static string ApplyVariables(string template, IReadOnlyDictionary<string, string> variables)
        {
            if (template == null || variables == null)
                return template;

            string result = template;
            foreach (KeyValuePair<string, string> kv in variables)
                result = result.Replace("{{" + kv.Key + "}}", kv.Value ?? "");
            return result;
        }

        public static string ApplyVariables(string template, params (string key, string value)[] variables)
        {
            if (template == null || variables == null)
                return template;

            string result = template;
            foreach ((string key, string value) in variables)
                result = result.Replace("{{" + key + "}}", value ?? "");
            return result;
        }
    }
}