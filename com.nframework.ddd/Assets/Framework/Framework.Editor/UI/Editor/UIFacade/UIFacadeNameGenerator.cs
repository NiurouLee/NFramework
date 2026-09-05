using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// UIFacade名称生成器，负责生成脚本名称和ID
    /// </summary>
    public static class UIFacadeNameGenerator
    {
        /// <summary>
        /// 生成脚本名称和ID
        /// </summary>
        public static void GenerateScriptNameAndID(UIFacade facade, out string scriptName, out string id)
        {
            scriptName = "";
            id = "";

            if (facade == null) return;

            string moduleName = CleanName(facade.m_ModuleName ?? "");
            string subModuleName = facade.m_EnableSubModule ? CleanName(facade.m_SubModuleName ?? "") : "";
            string uiName = CleanName(facade.m_UIName ?? "");

            if (string.IsNullOrEmpty(uiName)) return;

            // 生成脚本名称 (PascalCase)
            scriptName = GenerateScriptName(moduleName, subModuleName, uiName);

            // 生成ID (snake_case)
            id = GenerateID(moduleName, subModuleName, uiName);
        }

        /// <summary>
        /// 检查脚本名称是否重复
        /// </summary>
        /// <param name="scriptName">要检查的脚本名称</param>
        /// <param name="duplicateInfo">重复信息</param>
        /// <param name="excludeConfigID">排除的配置ID（通常是当前正在编辑的配置，避免误报）</param>
        public static bool CheckScriptNameDuplicate(string scriptName, out string duplicateInfo, string excludeConfigID = null)
        {
            duplicateInfo = "";
            if (string.IsNullOrEmpty(scriptName)) return false;

            return ViewConfigManager.CheckConfigIDDuplicate(scriptName, out duplicateInfo, excludeConfigID);
        }

        private static string GenerateScriptName(string moduleName, string subModuleName, string uiName)
        {
            string scriptName = "";

            if (!string.IsNullOrEmpty(moduleName))
            {
                scriptName += ToPascalCase(moduleName);
            }

            if (!string.IsNullOrEmpty(subModuleName))
            {
                scriptName += ToPascalCase(subModuleName);
            }

            scriptName += ToPascalCase(uiName);

            return scriptName;
        }

        private static string GenerateID(string moduleName, string subModuleName, string uiName)
        {
            List<string> idParts = new List<string>();

            if (!string.IsNullOrEmpty(moduleName))
            {
                idParts.Add(ToSnakeCase(moduleName));
            }

            if (!string.IsNullOrEmpty(subModuleName))
            {
                idParts.Add(ToSnakeCase(subModuleName));
            }

            idParts.Add(ToSnakeCase(uiName));

            return string.Join(".", idParts);
        }

        private static string CleanName(string name)
        {
            if (string.IsNullOrEmpty(name)) return "";
            return Regex.Replace(name.Trim(), @"[^a-zA-Z0-9\s]", "");
        }

        private static string ToPascalCase(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";

            string[] words = Regex.Split(input, @"[\s_-]+");
            string result = "";

            foreach (string word in words)
            {
                if (!string.IsNullOrEmpty(word))
                {
                    result += char.ToUpper(word[0]) + word.Substring(1).ToLower();
                }
            }

            return result;
        }

        private static string ToSnakeCase(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";

            string[] words = Regex.Split(input, @"[\s_-]+|(?=[A-Z])");
            List<string> cleanWords = new List<string>();

            foreach (string word in words)
            {
                if (!string.IsNullOrEmpty(word))
                {
                    cleanWords.Add(word.ToLower());
                }
            }

            return string.Join("_", cleanWords);
        }
    }
}
