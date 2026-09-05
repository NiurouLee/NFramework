using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OM
{
    public interface IOMUpdater
    {
        bool IsDontDestroyOnLoad() => false;
        bool IsUpdaterCompleted();
        void OnUpdate();
        void Stop();
    }
    
    public class OMUpdaterRuntime : MonoBehaviour
    {
        public const string GameObjectName = "HCCUpdaterRuntime";
        public static OMUpdaterRuntime Instance { get; private set; }

        private readonly List<IOMUpdater> _updaters = new List<IOMUpdater>();
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            foreach (var updater in _updaters.Where(x => !x.IsDontDestroyOnLoad()))
            {
                updater.Stop();
            }
        }
        
        private void Update()
        {
            for (var i = _updaters.Count - 1; i >= 0; i--)
            {
                var updater = _updaters[i];
                updater.OnUpdate();
                if (updater.IsUpdaterCompleted())
                {
                    _updaters.RemoveAt(i);
                }
            }
        }
        
        public static void AddUpdater(IOMUpdater updater)
        {
            if (Instance == null)
            {
                Instance = new GameObject(GameObjectName).AddComponent<OMUpdaterRuntime>();
            }
            Instance._updaters.Add(updater);
        }

        public static void RemoveUpdater(IOMUpdater updater)
        {
            if(Instance == null) return;
            updater.Stop();
        }
    }
}