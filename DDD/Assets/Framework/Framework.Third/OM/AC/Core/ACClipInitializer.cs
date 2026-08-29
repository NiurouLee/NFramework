using System;
using UnityEngine;

namespace OM.AC
{
    /// <summary>
    /// the initializer for the clip
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public class ACClipInitializer<T> where T : struct
    {
        [SerializeField] private T value;
        [SerializeField] private bool enabled = false;

        public T Value => value;
        public bool Enabled => enabled;

        /// <summary>
        /// Initialize the clip with the value if enabled
        /// </summary>
        /// <param name="callback"></param>
        public void Initialize(Action callback)
        {
            if(!enabled) return;
            callback?.Invoke();
        }
    }
}