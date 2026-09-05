using UnityEngine;

namespace OM.AC
{
    public static class ACExtension 
    {
        /// <summary>
        /// Add clip to animator
        /// and invoke OnClipAddedToACAnimator
        /// and record undo
        /// </summary>
        /// <param name="animator">the target Animator</param>
        /// <param name="clip">the new Clip</param>
        public static void AddClip(this ACAnimator animator, ACClip clip)
        {
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(animator, "Add Clip");
#endif
            animator.GetClips().Add(clip);
            clip.OnClipAddedToACAnimator(animator);
        }
        
        /// <summary>
        /// Remove the clip from the animator
        /// + record undo
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="clip"></param>
        public static void RemoveClip(this ACAnimator animator, ACClip clip)
        {
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(animator, "Add Clip");
#endif
            animator.GetClips().Remove(clip);
        }
        
    }
}