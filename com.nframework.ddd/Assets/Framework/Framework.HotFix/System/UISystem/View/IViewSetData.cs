namespace NFramework.ModuleSystem
{
    public interface IViewSetData<T>
    {
        void InitData(T inData);
        
        void SetData(T inData);
    }
}