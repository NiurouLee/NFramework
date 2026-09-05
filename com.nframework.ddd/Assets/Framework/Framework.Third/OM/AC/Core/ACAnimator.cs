using System.Collections.Generic;
using System.Linq;
using OM.Shared;
using UnityEngine;

namespace OM.AC
{
    /// <summary>
    /// The base class for all AC Animators
    /// Here you can add your own clips and evaluate them
    /// </summary>
    [OMTitle("AC Animator")]
    public abstract class ACAnimator : MonoBehaviour
    {
        [SerializeField,Min(.1f)] private float fullDuration = 1;
        [SerializeField,Min(0)] private float speed = 1;
        [SerializeField] private bool timeIndependent = false;
        [SerializeField] private bool playOnEnable = false;
        [SerializeReference] private List<ACClip> clips;
        
        public float TimelineTime { get; set; }
        public float FullDuration { get => fullDuration; set => fullDuration = Mathf.Max(value,.1f); }
        public float Speed { get => speed; set => speed = value; }
        public bool TimeIndependent { get => timeIndependent; set => timeIndependent = value; }
        public bool PlayOnEnable => playOnEnable;

        /// <summary>
        /// Simulate the Animator by delta time and speed
        /// </summary>
        /// <param name="previewMode"></param>
        public void Simulate(bool previewMode)
        {
            TimelineTime = Mathf.Clamp(TimelineTime + (TimeIndependent? Time.unscaledDeltaTime : Time.deltaTime) * Speed,0,fullDuration);
            Evaluate(TimelineTime,previewMode);
        }

        /// <summary>
        /// Evaluate the Animator 
        /// </summary>
        /// <param name="time">the actual time</param>
        /// <param name="previewMode">whether or not the it is in preview mode</param>
        public void Evaluate(float time,bool previewMode)
        {
            TimelineTime = Mathf.Clamp(time,0, fullDuration);
            foreach (var clip in GetClips())
            {
                if (!clip.IsValid()) continue;
                clip.Evaluate(TimelineTime,previewMode);
            }
        }
        
        /// <summary>
        /// Get All Clips in the Animator
        /// </summary>
        /// <returns></returns>
        public List<ACClip> GetClips()
        {
            if(clips == null) clips = new List<ACClip>();
            return clips;
        }

        /// <summary>
        /// Get the Last Clip in the Timeline based on the End Time
        /// </summary>
        /// <returns></returns>
        public ACClip GetTimelineLastClip()
        {
            if (GetClips().Count <= 0) return null;
            return GetClips().OrderBy(x => x.GetEndTime()).Last();
        }
    }
}