using System;

namespace OM.AC
{
    /// <summary>
    /// The Attribute for the clip to create the clip in the Search Window of the ACAnimator
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ACClipCreateAttribute : Attribute
    {
        /// <summary>
        /// The Menu Name of the Clip
        /// </summary>
        public string ClipMenuName { get; }
        
        /// <summary>
        /// The Clip Name of the Clip
        /// </summary>
        public string ClipName { get; }
        
        public ACClipCreateAttribute(string clipMenuName = null, string clipName = null)
        {
            ClipMenuName = clipMenuName;
            ClipName = clipName;
        }
        
        
    }
}