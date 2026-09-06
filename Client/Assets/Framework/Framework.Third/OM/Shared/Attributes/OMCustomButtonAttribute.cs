using System;

namespace OM.Shared
{
    [AttributeUsage(AttributeTargets.Method)]
    public class OMCustomButtonAttribute : Attribute
    {
        public string ButtonName { get; }

        public OMCustomButtonAttribute(string buttonName)
        {
            this.ButtonName = buttonName;
        }
    }
}