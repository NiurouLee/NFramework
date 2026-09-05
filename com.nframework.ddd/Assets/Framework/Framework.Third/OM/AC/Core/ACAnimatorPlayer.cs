using System;
using System.Collections;
using OM.Shared;
using UnityEngine;
using UnityEngine.Events;

namespace OM.AC
{
    /// <summary>
    /// The Animator Play State
    /// </summary>
    public enum ACAnimatorPlayState
    {
        None,
        Playing,
        Paused,
        Finished
    }
    
    /// <summary>
    /// The Animator Events
    /// </summary>
    [System.Serializable]
    public class ACAnimatorEvents
    {
        public UnityEvent onPlay;
        public UnityEvent onPause;
        public UnityEvent onResume;
        public UnityEvent onStop;
        public UnityEvent onComplete;
        public UnityEvent onOneLoopCompleted;
    }
    
    /// <summary>
    /// The Main Animator Player Class
    /// Inherits from ACAnimator
    /// Here you can Play and Control the Animator like Play, Pause, Resume, Stop, Restart
    /// </summary>
    [OMTitle("AC Animator Player")]
    public class ACAnimatorPlayer : ACAnimator
    {
        [SerializeField] private bool loop = false;
        [SerializeField] private ACAnimatorEvents events;
        
        public ACAnimatorPlayState PlayState { get; protected set; } = ACAnimatorPlayState.None;

        private Action _completeCallback;

        public bool Loop { get => loop; set => loop = value; }

        private void OnEnable()
        {
            if (PlayOnEnable)
            {
                Play();
            }
        }

        private void Update()
        {
            if(PlayState != ACAnimatorPlayState.Playing) return;
            Simulate(false);

            if (TimelineTime >= FullDuration)
            {
                if (loop)
                {
                    events.onOneLoopCompleted?.Invoke();
                    
                    Restart();
                    return;
                }
                OnComplete();
            }
        }

        /// <summary>
        /// Restart the Animation from the beginning
        /// </summary>
        public void Restart()
        {
            foreach (var clip in GetClips())
            {
                clip.OnTimelineCompleted();
            }
            TimelineTime = 0f;
            PlayState = ACAnimatorPlayState.Playing;
            foreach (var clip in GetClips())
            {
                clip.OnTimelineStarted();
                clip.Reset();
            }
        }

        /// <summary>
        /// Play the Animation from the beginning
        /// </summary>
        public void Play()
        {
            Play(null);
        }

        /// <summary>
        /// Play the Animation from the beginning with a callback when the animation is finished
        /// </summary>
        /// <param name="completeCallback">finish callback</param>
        public void Play(Action completeCallback)
        {
            TimelineTime = 0f;
            _completeCallback = completeCallback;
            PlayState = ACAnimatorPlayState.Playing;
            foreach (var clip in GetClips())
            {
                clip.OnTimelineStarted();
                clip.OnAnimatorStartPlaying();
            }
            Evaluate(0,false);
            events.onPlay?.Invoke();
        }
        
        /// <summary>
        /// Stop The Animation
        /// </summary>
        public void Stop()
        {
            PlayState = ACAnimatorPlayState.Finished;
            TimelineTime = 0;
            events.onStop?.Invoke();
        }
        
        /// <summary>
        /// Pause the Animation
        /// </summary>
        public void Pause()
        {
            PlayState = ACAnimatorPlayState.Paused;
            events.onPause?.Invoke();
        }
        
        /// <summary>
        /// Resume the Animation if it is paused
        /// </summary>
        public void Resume()
        {
            PlayState = ACAnimatorPlayState.Playing;
            events.onResume?.Invoke();
        }
        
        /// <summary>
        /// this method is called when the animation is finished
        /// </summary>
        private void OnComplete()
        {
            PlayState = ACAnimatorPlayState.Finished;
            _completeCallback?.Invoke();
            _completeCallback = null;
            events.onComplete?.Invoke();
            
            //Complete all clips
            foreach (var clip in GetClips())
            {
                clip.OnTimelineCompleted();
                clip.OnAnimatorCompletePlaying();
            }
        }
        
        public IEnumerator WaitForComplete()
        {
            while (PlayState != ACAnimatorPlayState.Finished)
            {
                yield return null;
            }
        }
    }
}