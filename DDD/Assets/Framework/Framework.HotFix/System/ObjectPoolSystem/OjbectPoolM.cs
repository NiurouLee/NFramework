using System.Collections.Generic;

namespace NFramework.ModuleSystem
{
    public class ObjectPoolSystem : FrameworkSystemModuleBase
    {
        private List<IFreeToPool> _tempList = new List<IFreeToPool>();
        private ObjectPool pool;

        public T Alloc<T>() where T : class, IFreeToPool, new()
        {
            return pool.Alloc<T>();
        }

        public void Free<T>(T inT) where T : class, IFreeToPool, new()
        {
            pool.Free(inT);
        }

        public void Free(IFreeToPool freeToPool)
        {
            // pool.Free(freeToPool);
        }

        public void Add<T>(uint inCount) where T : class, IFreeToPool, new()
        {
            for (int i = 0; i <= inCount; i++)
            {
                this._tempList.Add(pool.Alloc<T>());
            }
            for (int i = 0; i < _tempList.Count; i++)
            {
                var _obj = (T)_tempList[i];
                pool.Free(_obj);
            }
        }
        public void ClearOne<T>() where T : class, IFreeToPool, new()
        {
            pool.ClearOne<T>();
        }
        public void ClearAll()
        {
            if (pool == null)
            {
                return;
            }
            pool.ClearAll();
        }
        public override void Awake()
        {
            pool = new ObjectPool();
        }

        public override void Destroy()
        {
            ClearAll();
        }
    }
}