using System;
using System.Collections;
using System.Collections.Generic;
using XHFramework;

namespace NFramework.ModuleSystem
{
    public class NFROOT : NObject
    {
        private static NFROOT m_Instance;
        public static NFROOT I => Instance;

        public FrameworkConfig Config { get; set; }

        private List<Action<float>> OnLateUpdateList = new();
        private List<Action<float>> OnUpdateList = new();
        private List<Action<float>> OnFixedUpdateList = new();
        private List<Action<float>> OnEndOfFrameUpdateList = new();
        private List<Action> OnApplicationQuitEventList = new();
        private List<Action<bool>> OnApplicationFocusEventList = new();
        private List<Action<bool>> OnApplicationPauseEventList = new();

        public void AddLateUpdateCallback(Action<float> callback)
        {
            OnLateUpdateList.Add(callback);
        }

        public void AddUpdateCallback(Action<float> callback)
        {
            OnUpdateList.Add(callback);
        }

        public void AddFixedUpdateCallback(Action<float> callback)
        {
            OnFixedUpdateList.Add(callback);
        }

        public void AddEndOfFrameCallback(Action<float> callback)
        {
            OnEndOfFrameUpdateList.Add(callback);
        }

        public void AddApplicationQuitCallback(Action<bool> callback)
        {
        }

        public UnityEngine.Coroutine StartCoroutineUnity(IEnumerator enumerator)
        {
            return EngineLoop.Instance.StartCoroutine(enumerator);
        }

        public void StopCoroutineUnity(UnityEngine.Coroutine coroutine)
        {
            EngineLoop.Instance.StopCoroutine(coroutine);
        }

        public static void AwakeRoot()
        {
            NFROOT.Instance.RegisterEngineLoop();
            NFROOT.Instance.Log?.Print("AwakeRoot");
        }

        /// <summary>
        /// 按类型存储
        /// </summary>
        public Dictionary<Type, FrameworkSystemModuleBase> m_SystemDict;

        public HashSet<FrameworkSystemModuleBase> m_Systems;

        public void Awake()
        {
            m_SystemDict = new Dictionary<Type, FrameworkSystemModuleBase>();
            m_Systems = new HashSet<FrameworkSystemModuleBase>();
        }

        public static T AddSystem<T>(T module) where T : FrameworkSystemModuleBase, new()
        {
            Instance.m_Systems.Add(module);
            m_Instance.m_SystemDict.Add(module.GetType(), module);
            return module;
        }

        public static NFROOT Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new NFROOT();
                    m_Instance.Awake();
                }

                return m_Instance;
            }
        }


        public T GetSystem<T>() where T : FrameworkSystemModuleBase, new()
        {
            return GetSystemInternal<T>();
        }

        private T GetSystemInternal<T>() where T : FrameworkSystemModuleBase, new()
        {
            var type = typeof(T);
            if (m_SystemDict.TryGetValue(type, out var module))
            {
                return (T)module;
            }
            else
            {
                module = new T();
                m_SystemDict.Add(type, module);
                module.Awake();
            }

            return (T)module;
        }

        private void RegisterEngineLoop()
        {
            EngineLoop.Instance.OnEndOfFrameUpdateList = this.OnEndOfFrameUpdateList;
            EngineLoop.Instance.OnLateUpdateList = this.OnLateUpdateList;
            EngineLoop.Instance.OnUpdateList = this.OnUpdateList;
            EngineLoop.Instance.OnFixedUpdateList = this.OnFixedUpdateList;
            EngineLoop.Instance.OnApplicationQuitEventList = this.OnApplicationQuitEventList;
            EngineLoop.Instance.OnApplicationFocusEventList = this.OnApplicationFocusEventList;
            EngineLoop.Instance.OnApplicationPauseEventList = this.OnApplicationPauseEventList;
        }
    }
}