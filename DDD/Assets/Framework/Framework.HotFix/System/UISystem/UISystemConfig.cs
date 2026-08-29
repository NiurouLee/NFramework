using System.IO;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// UI 系统配置：编辑器路径与生成物位置。
    /// 作为 <see cref="XHFramework.FrameworkConfig"/> 的子配置统一管理，配置数据全部是实例字段。
    /// </summary>
    public class UISystemConfig : ScriptableObject
    {
        /// <summary>
        /// Window LRU Count
        /// </summary>
        public int LRUCount;

        /// <summary>
        /// 逻辑脚本生成根目录（保留字段，兼容旧配置）
        /// </summary>
#if UNITY_EDITOR
        [FolderPath(RequireExistingPath = true)]
#endif
        public string LogicScriptGenerateRootPath;

        /// <summary>
        /// ViewConfig 代码生成路径
        /// </summary>
#if UNITY_EDITOR
        [FolderPath(RequireExistingPath = true)]
#endif
        public string ViewConfigGeneratePath;

        /// <summary>
        /// ViewConfig.json 所在目录
        /// </summary>
#if UNITY_EDITOR
        [FormerlySerializedAs("ConfigDataRoot")] [FolderPath(RequireExistingPath = true)]
#endif
        public string ViewConfigDataRoot;

        /// <summary>
        /// 代码模板根目录（含 UIFacade、WindowGeneratorTemplate.txt、ViewGeneratorTemplate.txt 等）
        /// </summary>
#if UNITY_EDITOR
        [FolderPath(RequireExistingPath = true)]
#endif
        public string TemplatesRoot;

        /// <summary>ViewConfigs.json 完整路径</summary>
        public string ViewConfigsJsonPath => Path.Combine(ViewConfigDataRoot, "ViewConfigs.json");

        /// <summary>ViewType 生成代码所在目录</summary>
        public string ViewTypeGeneratedPath => ViewConfigGeneratePath;

        /// <summary>ViewTypeRegistry 自动生成文件路径</summary>
        public string ViewTypeRegistryGeneratedPath => Path.Combine(ViewConfigGeneratePath, "ViewTypeRegistryAuto.Generated.cs");

        /// <summary>Window 逻辑脚本生成模板路径</summary>
        public string WindowGeneratorTemplatePath =>
            Path.Combine(TemplatesRoot, "WindowGeneratorTemplate.txt");

        /// <summary>View 逻辑脚本生成模板路径</summary>
        public string ViewGeneratorTemplatePath =>
            Path.Combine(TemplatesRoot, "ViewGeneratorTemplate.txt");

        /// <summary>UIFacade 代码生成：类壳 + 小模板根目录（含 Default、Types、InteractTemplates）</summary>
        public string UIFacadeCodeTemplatesRoot =>
            Path.Combine(TemplatesRoot, "UIFacade");

        /// <summary>UIFacade 生成用类壳模板（新建完整脚本时）</summary>
        public string UIFacadeClassShellTemplatePath =>
            Path.Combine(UIFacadeCodeTemplatesRoot, "UIFacadeClassShell.txt");
    }
}
