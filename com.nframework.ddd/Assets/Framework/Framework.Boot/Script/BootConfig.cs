namespace NFramework.Boot
{
    public static class BootConfig
    {
        public static readonly string packageName = "DefaultPackage";

        public static readonly string packageUrl = "http://192.168.1.167:8084/NFramework/Res/";

        public static readonly string hotUpdateDllName = "Game.Logic.dll";

        public static readonly string hotUpdateEntryScript = "Game.Logic.Game";

        public static readonly string hotUpdateEntryMethod = "Start";
        
        public static readonly string FrameworkConfigAssetName = "FrameworkConfig";
    }

    /// <summary>
    /// 关闭游戏框架类型。
    /// </summary>
    public enum GameQuitType : byte
    {
        /// <summary>
        /// 仅关闭游戏框架。
        /// </summary>
        None = 0,

        /// <summary>
        /// 关闭游戏框架并重启游戏。
        /// </summary>
        Restart,

        /// <summary>
        /// 关闭游戏框架并退出游戏。
        /// </summary>
        Quit,
    }
}
