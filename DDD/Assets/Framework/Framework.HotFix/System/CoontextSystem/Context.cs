using System;
using System.Collections.Generic;
using NFramework.Core;

namespace NFramework.ModuleSystem
{
    public class Context : NObject, IAwakeSystem, IDestroySystem
    {
        private Dictionary<string, object> _attributes;

        public virtual void Awake()
        {
            _attributes = DictionaryPool.Alloc<string, object>();
        }

        public virtual bool Contains(string name)
        {
            if (_attributes == null)
            {
                return false;
            }

            if (_attributes.ContainsKey(name))
            {
                return true;
            }

            return false;
        }

        public virtual bool Contains<T>()
        {
            return Contains(typeof(T).Name);
        }

        public virtual object Get(string name)
        {
            return Get<object>(name);
        }

        public virtual T Get<T>()
        {
            return Get<T>(typeof(T).Name);
        }

        public virtual T Get<T>(string name)
        {
            if (_attributes == null)
            {
                return default;
            }

            object v;
            if (_attributes.TryGetValue(name, out v))
            {
                return (T)v;
            }

            return default;
        }

        public virtual void Set(string name, object value)
        {
            if (_attributes == null)
            {
                _attributes = DictionaryPool.Alloc<string, object>();
            }

            _attributes[name] = value;
        }

        public virtual void Set<T>(T value)
        {
            Set(typeof(T).Name, value);
        }

        public virtual object Remove(string name)
        {
            return Remove<object>(name);
        }

        public virtual T Remove<T>()
        {
            return Remove<T>(typeof(T).Name);
        }

        public virtual T Remove<T>(string name)
        {
            if (_attributes == null)
            {
                return default;
            }

            if (!_attributes.ContainsKey(name))
            {
                return default;
            }

            object v = _attributes[name];
            _attributes.Remove(name);
            return (T)v;
        }

        public virtual void Destroy()
        {
            OnDestroy();
        }

        public virtual void OnDestroy()
        {
            if (_attributes != null)
            {
                _attributes.Clear();
                DictionaryPool.Free(_attributes);
                _attributes = null;
            }
        }
    }
}