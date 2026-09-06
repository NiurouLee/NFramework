using System.Collections.Generic;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// 游戏业务模块中心
    /// </summary>
    public class GameLogicSystem : FrameworkSystemModuleBase
    {
        public Dictionary<System.Type, IGameLogicModule> m_Modules = new Dictionary<System.Type, IGameLogicModule>();

        public T GetLogicModule<T>() where T : class, IGameLogicModule
        {
            return this.m_Modules[typeof(T)] as T;
        }
    }
}