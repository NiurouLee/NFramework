using UnityEngine;
using UnityEngine.Events;

namespace OM.AC
{
    /// <summary>
    /// The Clip Events
    /// </summary>
    [System.Serializable]
    public class ACClipEvents
    {
        public UnityEvent onEnter;
        public UnityEvent onExit;
    }
    
    /// <summary>
    /// The Base class for all clips
    /// </summary>
    [System.Serializable]
    public abstract class ACClip
    {
        [SerializeField] private string name = "Clip";
        [SerializeField] private Color color = Color.cyan;
        [SerializeField,Min(0)] private float duration = .5f;
        [SerializeField,Min(0)] private float startAt = 0;
        [SerializeField] private bool enabled = true;
        [SerializeField] private ACClipEvents events;
        
        public float CurrentTime { get; private set; }
        public ACEvaluateState CurrentState { get; private set; } = ACEvaluateState.None;
        
        public string Name
        {
            get => name;
            set => name = value;
        }

        public Color Color
        {
            get => color;
            set => color = value;
        }

        public float Duration
        {
            get => duration;
            set => duration = Mathf.Max(value,0);
        }
        
        public float StartAt
        {
            get => startAt;
            set => startAt = Mathf.Max(value,0);
        }

        public bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        public ACClipEvents Events => events;


        /// <summary>
        /// Evaluate the Clip
        /// </summary>
        /// <param name="time">the timeline Time</param>
        /// <param name="previewMode">whether or not the it is in preview mode</param>
        /// <returns></returns>
        public ACEvaluateState Evaluate(float time,bool previewMode)
        {
            if (!Enabled) return ACEvaluateState.Failed;
            CurrentTime = time;
            var currentClipTime = Mathf.Min(CurrentTime - startAt,Duration);
            
            //Update the state based on the current time
            switch (CurrentState)
            {
                case ACEvaluateState.Running:
                    if (CurrentTime >= GetEndTime())
                    {
                        OnUpdate(CurrentState,CurrentTime,currentClipTime,Mathf.Clamp01(currentClipTime / Duration),previewMode);
                        SetState(ACEvaluateState.Finished);
                        OnExit();
                        Events.onExit?.Invoke();
                    }
                    else if(CurrentTime < startAt)
                    {
                        SetState(ACEvaluateState.None);
                    }
                    break;
                case ACEvaluateState.None:
                    if (CurrentTime >= startAt && CurrentTime < GetEndTime())
                    {
                        SetState(ACEvaluateState.Running);
                        OnEnter();
                        Events.onEnter?.Invoke();
                    }
                    else if(CurrentTime >= GetEndTime())
                    {
                        OnUpdate(CurrentState,CurrentTime,currentClipTime,Mathf.Clamp01(currentClipTime / Duration),previewMode);
                        SetState(ACEvaluateState.Finished);
                        OnExit();
                        Events.onExit?.Invoke();
                    }
                    break;
                case ACEvaluateState.Finished:
                    if(CurrentTime < startAt)
                    {
                        SetState(ACEvaluateState.None);
                    }
                    else if (CurrentTime < GetEndTime())
                    {
                        SetState(ACEvaluateState.Running);
                        //OnEnter();
                        Events.onEnter?.Invoke();
                    }
                    break;
            }
            
            OnUpdate(CurrentState,CurrentTime,currentClipTime,Mathf.Clamp01(currentClipTime / Duration),previewMode);
            
            return CurrentState;
        }
        
        /// <summary>
        /// Get the End time of the clip (StartAt + Duration)
        /// </summary>
        /// <returns>End time</returns>
        public float GetEndTime()
        {
            return Duration + StartAt;
        }
        
        /// <summary>
        /// Set the state of the clip
        /// </summary>
        /// <param name="state"></param>
        public void SetState(ACEvaluateState state)
        {
            var lastState = CurrentState;
            CurrentState = state;
            OnStateChanged(CurrentState,lastState);
            //Debug.Log("Change state to " + state + " from " + lastState + " Time: " + CurrentTime);
        }

        /// <summary>
        /// Reset the clip
        /// </summary>
        public void Reset()
        {
            CurrentState = ACEvaluateState.None;
            CurrentTime = 0;
        }

        /// <summary>
        /// On State Changed
        /// </summary>
        /// <param name="currentState">current State</param>
        /// <param name="lastState">Last State</param>
        protected virtual void OnStateChanged(ACEvaluateState currentState,ACEvaluateState lastState) { }
        
        /// <summary>
        /// On Enter the clip (when the clip is started) when the timeline time is more than the start time
        /// </summary>
        protected virtual void OnEnter() { }
        /// <summary>
        /// On Clip Update
        /// </summary>
        /// <param name="state">the Current Clip State</param>
        /// <param name="timelineTime">The Current Timeline Time</param>
        /// <param name="clipTime">The Clip time (Timeline time - Start At)</param>
        /// <param name="normalizedClipTime">The Clip time Normalized</param>
        /// <param name="previewMode">whether or not it is in preview mode</param>
        protected abstract void OnUpdate(ACEvaluateState state, float timelineTime, float clipTime,
            float normalizedClipTime, bool previewMode);
        /// <summary>
        /// On Exit the clip (when the clip is finished) when the timeline time is more than the end time
        /// </summary>
        protected virtual void OnExit() { }
        /// <summary>
        /// On Timeline started (time = 0)
        /// </summary>
        public virtual void OnTimelineStarted() { }
        /// <summary>
        /// On Timeline reached the end (time = full duration)
        /// </summary>
        public virtual void OnTimelineCompleted() { }
        /// <summary>
        /// On Preview Mode Changed via the Animator Preview Button in the Editor (Editor Only)
        /// </summary>
        /// <param name="previewMode">preview Value</param>
        public abstract void OnPreviewModeChanged(bool previewMode);
        /// <summary>
        /// On the Clip Added to the Animator (Editor Only)
        /// </summary>
        /// <param name="animator"></param>
        public virtual void OnClipAddedToACAnimator(ACAnimator animator) { }
        /// <summary>
        /// On the Animator Start Playing
        /// </summary>
        public virtual void OnAnimatorStartPlaying() { }
        /// <summary>
        /// On the Animator Complete Playing
        /// </summary>
        public virtual void OnAnimatorCompletePlaying() { }
        /// <summary>
        /// If the clip can be played in preview mode (Editor Only)
        /// </summary>
        /// <returns></returns>
        public abstract bool CanBePlayedInPreviewMode();
        /// <summary>
        /// If the clip is valid or not (Check the Target != null)
        /// </summary>
        /// <returns></returns>
        public abstract bool IsValid();
        /// <summary>
        /// Get the Target of the clip 
        /// </summary>
        /// <returns></returns>
        public abstract Component GetTarget();
        /// <summary>
        /// Set the target of the clip (Editor Only)
        /// </summary>
        /// <param name="newTarget"></param>
        public abstract void SetTarget(GameObject newTarget);
        
        /// <summary>
        /// Clone the Clip
        /// </summary>
        /// <typeparam name="T">the type of the clip</typeparam>
        /// <returns></returns>
        public T Clone<T>() where T : ACClip
        {
            var json = JsonUtility.ToJson(this);
            var newClip = (ACClip)JsonUtility.FromJson(json,GetType());
            return newClip as T;
        }

    }
}
