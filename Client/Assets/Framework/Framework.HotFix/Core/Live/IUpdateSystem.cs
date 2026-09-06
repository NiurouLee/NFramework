namespace NFramework
{
    public interface IUpdateSystem : ISystemType
    {
        void Update(float deltaTime);
    }

    public interface IFixedUpdateSystem : ISystemType
    {
        void FixedUpdate(float deltaTime);
    }

    public interface IRendererUpdateSystem : ISystemType
    {
        void RendererUpdate(float deltaTime);
    }

    public interface ILateUpdateSystem : ISystemType
    {
        void LateUpdate(float deltaTime);
    }
}