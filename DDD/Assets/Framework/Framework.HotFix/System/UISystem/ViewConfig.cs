using System;
using  NFramework.Core;

namespace NFramework.ModuleSystem
{
    [Serializable]
    public class ViewConfig
    {
        public string ID;
        public string AssetID;
        private BitField32 Set;

        /// <summary>
        /// Layer
        /// </summary>
        /// <returns></returns>
        public ushort Layer => this.Set.Low;
        public bool IsWindow => this.Set.GetBit(31);

        public void SetLayer(ushort inLayer)
        {
            this.Set.Low = inLayer;
        }

        public void SetWindow(bool inWindow)
        {
            this.Set.SetBit(31, inWindow);
        }
    }
}
