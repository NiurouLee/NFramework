namespace NFramework.ModuleSystem
{
    public partial class View
    {
        public View Parent { get; private set; }

        internal void SetParent(View inParent)
        {
            this.Parent = inParent;
        }

        private void DestroyParent()
        {
            this.Parent = null;
        }
    }
}
