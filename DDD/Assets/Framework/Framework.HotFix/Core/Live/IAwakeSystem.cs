namespace NFramework.ModuleSystem
{
    public interface IAwakeSystem : ISystemType
    {
        void Awake();
    }

    public interface IAwakeSystem<A> : ISystemType
    {
        void Awake(A a);
    }

    public interface IAwakeSystem<A, B> : ISystemType
    {
        void Awake(A a, B b);
    }

    public interface IAwakeSystem<A, B, C> : ISystemType
    {
        void Awake(A a, B b, C c);
    }

    public interface IAwakeSystem<A, B, C, D> : ISystemType
    {
        void Awake(A a, B b, C c, D d);
    }

    public interface IAwakeSystem<A, B, C, D, E> : ISystemType
    {
        void Awake(A a, B b, C c, D d, E e);
    }

    public interface IAwakeSystem<A, B, C, D, E, F> : ISystemType
    {
        void Awake(A a, B b, C c, D d, E e, F f);
    }

    public interface IAwakeSystem<A, B, C, D, E, F, G> : ISystemType
    {
        void Awake(A a, B b, C c, D d, E e, F f, G g);
    }

    public interface IAwakeSystem<A, B, C, D, E, F, G, H> : ISystemType
    {
        void Awake(A a, B b, C c, D d, E e, F f, G g, H h);
    }

    public interface IAwakeSystem<A, B, C, D, E, F, G, H, I> : ISystemType
    {
        void Awake(A a, B b, C c, D d, E e, F f, G g, H h, I i);
    }

    public interface IAwakeSystem<A, B, C, D, E, F, G, H, I, J> : ISystemType
    {
        void Awake(A a, B b, C c, D d, E e, F f, G g, H h, I i, J j);
    }

    public interface IAwakeSystem<A, B, C, D, E, F, G, H, I, J, K> : ISystemType
    {
        void Awake(A a, B b, C c, D d, E e, F f, G g, H h, I i, J j, K k);
    }

    public interface IAwakeSystem<A, B, C, D, E, F, G, H, I, J, K, L> : ISystemType
    {
        void Awake(A a, B b, C c, D d, E e, F f, G g, H h, I i, J j, K k, L l);
    }
}