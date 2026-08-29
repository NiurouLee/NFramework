using NFramework.ModuleSystem;
using UnityEngine;

namespace NFramework
{
    /// <summary>
    /// 框架总配置，统一管理各系统配置（UI、Res 等）。
    /// 该资产由资源管线（YooAsset）加载，资源名见 BootConfig.FrameworkConfigAssetName，
    /// 加载后赋值给 NFROOT.Config。
    /// </summary>
    [CreateAssetMenu(fileName = "FrameworkConfig", menuName = "XHFramework/FrameworkConfig")]
    public class FrameworkConfig : ScriptableObject
    {
        public UISystemConfig UISystemConfig;

        public ResSystemConfig ResSystemConfig;
    }
}
