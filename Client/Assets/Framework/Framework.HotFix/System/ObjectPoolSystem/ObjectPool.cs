using System;
using System.Collections.Generic;

namespace NFramework.ModuleSystem
{
    public class ObjectPool
    {
        private Dictionary<Type, IObjectPoolCollection> objectCollections = new Dictionary<Type, IObjectPoolCollection>();
        public T Alloc<T>() where T : class, IFreeToPool, new()
        {
            return this.GetCollection<T>().Alloc();
        }

        public void Free<T>(T inT) where T : class, IFreeToPool, new()
        {
            if (inT == null)
            {
                return;
            }
            this.GetCollection<T>().Free(inT);
        }

        public void ClearOne<T>()
        {
            var _type = typeof(T);
            if (this.objectCollections.TryGetValue(_type, out var collection))
            {
                collection.ClearOne();
            }
        }

        private ObjectPoolCollection<T> GetCollection<T>() where T : class, IFreeToPool, new()
        {
            var _type = typeof(T);
            if (this.objectCollections.TryGetValue(_type, out var collections))
            {
                return (ObjectPoolCollection<T>)collections;
            }
            var result = new ObjectPoolCollection<T>();
            this.objectCollections.Add(_type, result);
            return result;
        }

        public void ClearAll()
        {
            foreach (var collection in this.objectCollections.Values)
            {
                collection.ClearAll();
            }
            this.objectCollections.Clear();
        }
    }
}