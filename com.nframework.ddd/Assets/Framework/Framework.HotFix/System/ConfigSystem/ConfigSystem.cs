using System;
using System.Collections;
using System.Collections.Generic;
using Luban;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// 非泛型标记接口，供缓存字典统一存储。
    /// LoadData 放在这一层，使缓存只持有 IConfig 引用时也能驱动加载。
    /// </summary>
    public interface IConfig
    {
        void LoadData(ByteBuf _buf);
    }

    /// <summary>
    /// 泛型配置接口，TbXxx reader 实现此接口。
    /// T 为数据类型（TableXxx）。
    /// </summary>
    public interface IConfig<T> : IConfig
    {
        T Get(int id);
        IReadOnlyDictionary<int, T> GetAll();
        IDictionary GetDic();
        IReadOnlyList<T> GetList();
        T GetBean();
    }


    public class ConfigSystem : FrameworkSystemModuleBase
    {
        private readonly Dictionary<Type, IConfig> _cache = new Dictionary<Type, IConfig>();
        private ResLoadRecords _resLoader;
        public Func<Type, string> assetInfoRegister;
        public Func<Type, IConfig> readerRegister;

        public override void Awake()
        {
            _resLoader = new ResLoadRecords();
            _resLoader.Awake();
        }

        /// <summary>
        /// 按需懒加载：首次访问时从 TablesReader.Registry 取文件名和工厂，
        /// 加载二进制资源并调用 LoadData 填充数据，之后走缓存。
        /// </summary>
        private void CheckCache<T>() where T : Luban.BeanBase
        {
            if (_cache.ContainsKey(typeof(T)))
                return;
            string assetName = assetInfoRegister?.Invoke(typeof(T));
            if (string.IsNullOrEmpty(assetName))
            {
                throw new KeyNotFoundException($"[ConfigM] No registry entry for type: {typeof(T).FullName}");
            }

            var textAsset = _resLoader.LoadRawAsset(assetName);
            if (textAsset == null)
            {
                throw new Exception(
                    $"[ConfigM] Failed to load asset for type: {typeof(T).FullName}, asset name: {assetName}");
            }

            var bytes = textAsset as byte[];
            var bytebuffer = new ByteBuf(bytes);
            var reader = readerRegister?.Invoke(typeof(T));
            if (reader == null)
            {
                throw new KeyNotFoundException($"[ConfigM] No reader registered for type: {typeof(T).FullName}");
            }

            reader.LoadData(bytebuffer); // 反序列化填充
            _cache[typeof(T)] = reader;
        }

        /// <summary>根据 id 获取单条配置（map 表）。</summary>
        public T Get<T>(int id) where T : Luban.BeanBase
        {
            CheckCache<T>();
            return ((IConfig<T>)_cache[typeof(T)]).Get(id);
        }

        /// <summary>获取全部配置字典（map 表）。</summary>
        public IReadOnlyDictionary<int, T> GetAll<T>() where T : Luban.BeanBase
        {
            CheckCache<T>();
            return ((IConfig<T>)_cache[typeof(T)]).GetAll();
        }

        /// <summary>获取全部配置列表（list / map 表）。</summary>
        public IReadOnlyList<T> GetList<T>() where T : Luban.BeanBase
        {
            CheckCache<T>();
            return ((IConfig<T>)_cache[typeof(T)]).GetList();
        }

        /// <summary>获取单例配置（one 表）。</summary>
        public T GetBean<T>() where T : Luban.BeanBase
        {
            CheckCache<T>();
            return ((IConfig<T>)_cache[typeof(T)]).GetBean();
        }

        /// <summary>
        /// 获取原始字典（key 非 int 的 map 表）。
        /// 调用方自行强转，例如：
        ///   (IReadOnlyDictionary&lt;string, TableRole&gt;)configM.GetDic&lt;TableRole&gt;()
        /// </summary>
        public IDictionary<K, T> GetDic<K, T>() where T : Luban.BeanBase
        {
            CheckCache<T>();
            var raw = ((IConfig<T>)_cache[typeof(T)]).GetDic();
            if (raw is not IDictionary<K, T> typed)
                throw new InvalidCastException(
                    $"[ConfigM] GetDic<{typeof(K).Name}, {typeof(T).Name}>: " +
                    $"actual dictionary type is {raw.GetType().Name}, cannot cast.");
            return typed;
        }
    }
}