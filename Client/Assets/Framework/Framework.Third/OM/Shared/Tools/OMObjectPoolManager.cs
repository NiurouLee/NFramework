using System.Collections.Generic;
using OM;
using UnityEngine;

namespace OM
{
    public class OMObjectPoolManager : MonoBehaviour
    {
        [System.Serializable]
        public class OMObjectPoolItem
        {
            public string key;
            public bool rename = true;
            public GameObject prefab;
            public bool useIPool = false;
            public int preload = 0;
            public int maxCapacity = 10;

            public OMObjectPool<GameObject> Pool { get; private set; }

            public void Setup(Transform parent)
            {
                Pool = new OMObjectPool<GameObject>(maxCapacity,preload,useIPool,() =>
                {
                    var obj = Instantiate(prefab, parent);
                    obj.gameObject.SetActive(false);
                    if (rename)
                    {
                        obj.name += $" _{Pool.GetSpawnedCount}";
                    }
                    return obj;
                },obj =>
                {
                    obj.SetActive(true);
                },obj =>
                {
                    obj.SetActive(false);
                    obj.transform.SetParent(parent,true);
                });
            }
        }

        public static OMObjectPoolManager Instance { get; private set; }
        
        [SerializeField] private OMObjectPoolItem[] items;

        private readonly Dictionary<string, OMObjectPool<GameObject>> _dictionary = new Dictionary<string, OMObjectPool<GameObject>>();

        private void Awake()
        {
            Instance = this;
            foreach (var item in items)
            {
                var parent = new GameObject($"Pool ({item.key})");
                parent.transform.SetParent(transform,true);
                item.Setup(parent.transform);
                _dictionary.TryAdd(item.key, item.Pool);
            }
        }

        public OMObjectPool<GameObject> GetPoolByKey(string key)
        {
            if (_dictionary.TryGetValue(key,out var pool))
            {
                return pool;
            }
            UnityEngine.Debug.LogError("No Pool Found");
            return null;
        }
        
        public GameObject Spawn(string key)
        {
            var objectPool = GetPoolByKey(key);
            var o = objectPool.Spawn();
            return o;
        }
        
        public GameObject Spawn(string key,Vector3 pos)
        {
            var objectPool = GetPoolByKey(key);
            var o = objectPool.Spawn();
            o.transform.position = pos;
            return o;
        }
        
        public GameObject Spawn(string key,Vector3 pos,Quaternion rot)
        {
            var objectPool = GetPoolByKey(key);
            var o = objectPool.Spawn();
            o.transform.SetPositionAndRotation(pos,rot);
            return o;
        }
        
        public GameObject Spawn(string key,Vector3 pos,Quaternion rot,Transform parent)
        {
            var objectPool = GetPoolByKey(key);
            var o = objectPool.Spawn();
            o.transform.SetPositionAndRotation(pos,rot);
            o.transform.SetParent(parent,true);
            return o;
        }
        
        public T Spawn<T>(string key)
        {
            var objectPool = GetPoolByKey(key);
            var o = objectPool.Spawn();
            return o.GetComponent<T>();
        }
        
        public T Spawn<T>(string key,Vector3 pos)
        {
            var objectPool = GetPoolByKey(key);
            var o = objectPool.Spawn();
            o.transform.position = pos;
            return o.GetComponent<T>();
        }
        
        public T Spawn<T>(string key,Vector3 pos,Quaternion rot)
        {
            var objectPool = GetPoolByKey(key);
            var o = objectPool.Spawn();
            o.transform.SetPositionAndRotation(pos,rot);
            return o.GetComponent<T>();
        }
        
        public T Spawn<T>(string key,Vector3 pos,Quaternion rot,Transform parent)
        {
            var objectPool = GetPoolByKey(key);
            var o = objectPool.Spawn();
            o.transform.SetPositionAndRotation(pos,rot);
            o.transform.SetParent(parent,true);
            return o.GetComponent<T>();
        }
        
        public void Despawn(string key,GameObject obj)
        {
            var objectPool = GetPoolByKey(key);
            objectPool.Despawn(obj);
        }
        
        public void Despawn<T>(string key,T obj) where T : MonoBehaviour
        {
            var objectPool = GetPoolByKey(key);
            objectPool.Despawn(obj.gameObject);
        }
        
    }
}