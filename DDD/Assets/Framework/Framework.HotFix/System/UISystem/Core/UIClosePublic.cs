
namespace NFramework.ModuleSystem
{
    public partial class UISystem
    {

        public void Close<T>() where T : View
        {
            this._Close(this.GetViewID<T>());
        }

        public void Close<T>(T inWindow) where T : Window
        {
            var type = inWindow.GetType();
            this._Close(this.GetViewID(type));
        }

        public void Close(string inWindowID)
        {
            this._Close(inWindowID);
        }
    }
}