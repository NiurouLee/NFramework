namespace NFramework.ModuleSystem
{
    public abstract class NObject
    {
        public T GetSystem<T>() where T : FrameworkSystemModuleBase, new()
        {
            return NFROOT.I.GetSystem<T>();
        }

        public Log? Log
        {
            get { return GetSystem<LoggerSystem>()?.Log; }
        }

        public Error? Error
        {
            get { return GetSystem<LoggerSystem>()?.Error; }
        }

        public Warning? Warning
        {
            get { return GetSystem<LoggerSystem>()?.Warning; }
        }
    }
}