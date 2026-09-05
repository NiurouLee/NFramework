using System;
using UnityEngine;

namespace OM
{
    public class OMTimer : IOMUpdater
    {
        public static OMTimer Create(float duration,Action onComplete,bool timeIndependent = false,bool persist = false)
        {
            return new OMTimer(duration,onComplete,timeIndependent,persist);
        }

        private float _duration;
        private readonly bool _persist;
        private readonly Action _onComplete;
        private readonly bool _timeIndependent = false;

        private OMTimer(float duration,
            Action onComplete,
            bool timeIndependent,
            bool persist)
        {
            _duration = duration;
            _timeIndependent = timeIndependent;
            _onComplete = onComplete;
            _persist = persist;
            OMUpdaterRuntime.AddUpdater(this);
        }

        private float GetDeltaTime()
        {
            return _timeIndependent ? Time.unscaledDeltaTime : Time.deltaTime;
        }

        public bool IsDontDestroyOnLoad()
        {
            return _persist;
        }

        public bool IsUpdaterCompleted()
        {
            return _duration <= 0;
        }

        public void OnUpdate()
        {
            if(IsUpdaterCompleted()) return;
            
            _duration -= GetDeltaTime();
            if (_duration <= 0)
            {
                _onComplete?.Invoke();
                OMUpdaterRuntime.RemoveUpdater(this);
            }
        }

        public void Stop()
        {
            _duration = 0;
        }
    }
}