namespace NFramework.ModuleSystem
{
    public static class LivingSystem
    {
        public static void Awake(object inObject)
        {
            if (inObject is IAwakeSystem awakeSystem)
            {
                awakeSystem.Awake();
            }
        }


        public static void Awake<A>(object inObject, A a)
        {
            if (inObject is IAwakeSystem<A> awakeSystemA)
            {
                awakeSystemA.Awake(a);
            }
        }

        public static void Awake<A, B>(object inObject, A a, B b)
        {
            if (inObject is IAwakeSystem<A, B> awakeSystemAB)
            {
                awakeSystemAB.Awake(a, b);
            }
        }


        public static void Awake<A, B, C>(object inObject, A a, B b, C c)
        {
            if (inObject is IAwakeSystem<A, B, C> awakeSystemABC)
            {
                awakeSystemABC.Awake(a, b, c);
            }
        }

        public static void Awake<A, B, C, D>(object inObject, A a, B b, C c, D d)
        {
            if (inObject is IAwakeSystem<A, B, C, D> awakeSystemABCD)
            {
                awakeSystemABCD.Awake(a, b, c, d);
            }
        }

        public static void Awake<A, B, C, D, E>(object inObject, A a, B b, C c, D d, E e)
        {
            if (inObject is IAwakeSystem<A, B, C, D, E> awakeSystemABCDE)
            {
                awakeSystemABCDE.Awake(a, b, c, d, e);
            }
        }

        public static void Awake<A, B, C, D, E, F>(object inObject, A a, B b, C c, D d, E e, F f)
        {
            if (inObject is IAwakeSystem<A, B, C, D, E, F> awakeSystemABCDEF)
            {
                awakeSystemABCDEF.Awake(a, b, c, d, e, f);
            }
        }

        public static void Awake<A, B, C, D, E, F, G>(object inObject, A a, B b, C c, D d, E e, F f, G g)
        {
            if (inObject is IAwakeSystem<A, B, C, D, E, F, G> awakeSystemABCDEFG)
            {
                awakeSystemABCDEFG.Awake(a, b, c, d, e, f, g);
            }
        }

        public static void Start(object inObject)
        {
            if (inObject is IStartSystem startSystem)
            {
                startSystem.Start();
            }
        }

        public static void Destroy(object inObject)
        {
            if (inObject is IDestroySystem destroySystem)
            {
                destroySystem.Destroy();
            }
        }
    }
}
