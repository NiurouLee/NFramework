using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NFramework.ModuleSystem
{
    public class EngineLoop : MonoBehaviour
    {
        public void Awake()
        {
            if (Instance != null)
            {
                throw new Exception("EngineLoop already created");
            }

            Instance = this;
            this.StartCoroutine(endOfFrame());
        }

        public static EngineLoop Instance { get; private set; }
        private WaitForEndOfFrame w = new WaitForEndOfFrame();
        public List<Action<float>> OnLateUpdateList;
        public List<Action<float>> OnUpdateList;
        public List<Action<float>> OnFixedUpdateList;
        public List<Action<float>> OnEndOfFrameUpdateList;
        public List<Action> OnApplicationQuitEventList;
        public List<Action<bool>> OnApplicationFocusEventList;
        public List<Action<bool>> OnApplicationPauseEventList;

        private IEnumerator endOfFrame()
        {
            while (true)
            {
                yield return w;
                if (OnEndOfFrameUpdateList != null)
                {
                    for (int i = 0; i < OnEndOfFrameUpdateList.Count; i++)
                    {
                        OnEndOfFrameUpdateList[i]?.Invoke(Time.deltaTime);
                    }
                }
            }
        }


        private void Update()
        {
            if (OnUpdateList != null)
            {
                for (int i = 0; i < OnUpdateList.Count; i++)
                {
                    OnUpdateList[i]?.Invoke(Time.deltaTime);
                }
            }
        }

        private void FixedUpdate()
        {
            if (OnFixedUpdateList != null)
            {
                for (int i = 0; i < OnFixedUpdateList.Count; i++)
                {
                    OnFixedUpdateList[i]?.Invoke(Time.fixedDeltaTime);
                }
            }
        }

        private void LateUpdate()
        {
            if (OnLateUpdateList != null)
            {
                for (int i = 0; i < OnLateUpdateList.Count; i++)
                {
                    OnLateUpdateList[i]?.Invoke(Time.deltaTime);
                }
            }
        }

        private void OnApplicationQuit()
        {
            if (OnApplicationQuitEventList != null)
            {
                for (int i = 0; i < OnApplicationQuitEventList.Count; i++)
                {
                    OnApplicationQuitEventList[i]?.Invoke();
                }
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (OnApplicationFocusEventList != null)
            {
                for (int i = 0; i < OnApplicationFocusEventList.Count; i++)
                {
                    OnApplicationFocusEventList[i]?.Invoke(hasFocus);
                }
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (OnApplicationPauseEventList != null)
            {
                for (int i = 0; i < OnApplicationPauseEventList.Count; i++)
                {
                    OnApplicationPauseEventList[i]?.Invoke(pauseStatus);
                }
            }
        }
    }
}