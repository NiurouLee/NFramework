namespace NFramework.ModuleSystem
{
    public interface IGameLogicModule
    {
        public void Awake();
        public void Open();
        public void Update(float elapseSeconds, float realElapseSeconds);
        public void Close();
        public void Destroy();
    }
}