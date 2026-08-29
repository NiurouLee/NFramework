using   NFramework;

namespace NFramework.ModuleSystem
{
    public class FrameworkSystemModuleBase : NObject, IRendererUpdateSystem, IUpdateSystem, ILateUpdateSystem
    {
        public virtual void Awake()
        {
        }

        public virtual void Destroy()
        {
        }

        public virtual void RendererUpdate(float deltaTime)
        {
        }

        public virtual void Update(float deltaTime)
        {
        }

        public virtual void LateUpdate(float deltaTime)
        {
        }
    }
}