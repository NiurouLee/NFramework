namespace Framework.ModuleSystem
{
    public interface IWindowOpenAnim
    {
        public float OpenAnimTime { get; }
        public void PlayOpen();
        public void PauseOpen();
    }

    public interface IwindowCloseAnim
    {
        public float CloseAnimTime { get; }
        public void PlayClose();
        public void PauseClose();
    }
}