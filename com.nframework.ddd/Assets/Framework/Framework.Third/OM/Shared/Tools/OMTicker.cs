using System;
using UnityEngine;

namespace OM
{
    public class OMTicker : IOMUpdater
    {
        public static OMTicker Create(
            float interval,
            Action<int> onTick,
            bool timeIndependent = false,
            bool persist = false)
        {
            var ticker = new OMTicker(interval,onTick,timeIndependent,persist);
            return ticker;
        }

        private readonly Action<int> _onTick;
        private readonly float _interval;
        private readonly bool _persist;
        private readonly bool _timeIndependent;

        private int _tickCount;
        private float _timer;
        public bool IsRunning { get; private set; } = true;

        private OMTicker(float interval, Action<int> onTick,bool timeIndependent,bool persist = false)
        {
            _interval = interval;
            _timeIndependent = timeIndependent;
            _onTick = onTick;
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
            return !IsRunning;
        }

        public void OnUpdate()
        {
            _timer += GetDeltaTime();
            
            if(_timer >= _interval)
            {
                _timer -= _interval;
                _tickCount++;
                _onTick?.Invoke(_tickCount);
            }
        }

        public void Stop()
        {
            IsRunning = false;
        }
    }
}