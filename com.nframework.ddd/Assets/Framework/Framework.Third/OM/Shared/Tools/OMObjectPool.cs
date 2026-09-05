using System;
using System.Collections.Generic;

namespace OM
{
    public interface IOMObjectPool<T>
    {
        void OnSpawn(OMObjectPool<T> pool);
        void OnDespawn();
    }
    
    public class OMObjectPool<T>
    {
        private readonly Func<T> _createObjFunc;
        private readonly Action<T> _onDespawnObj;
        private readonly Action<T> _onSpawn;
        private readonly bool _useIPool;
        private readonly int _maxCapacity;
        private readonly List<T> _list;
        private readonly List<T> _spawned;
        
        public int GetSpawnedCount => _spawned.Count;
        
        
        public OMObjectPool(int maxCapacity,int preload,bool useIPool,Func<T> createObjFunc, Action<T> onSpawn = null,Action<T> onDespawn = null)
        {
            _createObjFunc = createObjFunc ?? throw new ArgumentNullException("Create Function is null" + nameof(createObjFunc));
            _useIPool = useIPool;
            _onDespawnObj = onDespawn;
            _onSpawn = onSpawn;
            _maxCapacity = maxCapacity;
            _list = new List<T>();
            _spawned = new List<T>();

            for (var i = 0; i < preload; i++)
            {
                var obj = _createObjFunc();
                _list.Add(obj);
            }
        }

        public T Spawn()
        {
            T obj;
            if (_list.Count <= 0)
            {
                if (_spawned.Count >= _maxCapacity)
                {
                    obj = _spawned[0];
                    _spawned.RemoveAt(0);
                    if (_useIPool)
                    {
                        var pool = obj as IOMObjectPool<T>;
                        pool?.OnDespawn();
                    }
                }
                else
                {
                    obj = _createObjFunc();
                }
            }
            else
            {
                obj = _list[0];
                _list.RemoveAt(0);
            }

            _onSpawn?.Invoke(obj);
            if (_useIPool)
            {
                var pool = obj as IOMObjectPool<T>;
                pool?.OnSpawn(this);
            }
            _spawned.Add(obj);
            return obj;
        }

        public void Despawn(T obj)
        {
            _onDespawnObj?.Invoke(obj);
            if (_useIPool)
            {
                var pool = obj as IOMObjectPool<T>;
                pool?.OnDespawn();
            }
            _list.Add(obj);
            _spawned.Remove(obj);
        }
    }
}