using System.Collections.Generic;

namespace NFramework.ModuleSystem
{
    public interface IObjectPoolCollection
    {
        void ClearOne();
        void ClearAll();
    }

    public class ObjectPoolCollection<T> : IObjectPoolCollection where T : class, IFreeToPool, new()
    {
        private Stack<T> objects = new Stack<T>();
        private HashSet<T> objectSet = new HashSet<T>();
        public System.Type ObjectType { get; private set; }
        public uint UsingCount { get; private set; }
        public uint AcquireCount { get; private set; }
        public uint ReleaseCount { get; private set; }
        public uint AddCount { get; private set; }
        public uint RemoveCount { get; private set; }

        public ObjectPoolCollection()
        {
            ObjectType = typeof(T);
        }

        public T Alloc()
        {
            this.UsingCount++;
            this.AcquireCount++;
            lock (this.objects)
            {
                if (this.objects.TryPop(out var result))
                {
                    return result;
                }
            }

            this.AddCount++;
            return new T();
        }

        public void Free(T inT)
        {
            if (this.objectSet.Contains(inT))
            {
                throw new System.Exception("objece is freeded");
            }

            lock (this.objects)
            {
                this.objects.Push(inT);
                this.objectSet.Add(inT);
            }

            this.ReleaseCount++;
            this.UsingCount--;
        }

        public void ClearAll()
        {
            this.objects.Clear();
            this.objectSet.Clear();
            this.UsingCount = 0;
            this.AcquireCount = 0;
            this.AddCount = 0;
            this.RemoveCount = 0;
            this.ReleaseCount = 0;
        }

        public void ClearOne()
        {
            this.objects.TryPop(out var result);
        }
    }
}