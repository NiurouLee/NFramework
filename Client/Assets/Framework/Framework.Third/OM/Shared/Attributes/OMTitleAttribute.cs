using System;
using UnityEngine;

namespace OM
{
    [AttributeUsage(AttributeTargets.Class)]
    public class OMTitleAttribute : Attribute
    {
        public string Title { get; }
        public OMTitleAttribute(string title)
        {
            Title = title;
        }
    }
}