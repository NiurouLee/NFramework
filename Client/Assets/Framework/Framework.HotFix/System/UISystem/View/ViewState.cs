using  NFramework.Core;

namespace NFramework.ModuleSystem
{
    public partial class View
    {
        public BitField32 State = new BitField32(0u);

        public void Learn(ViewStateFlag inFlag)
        {
            this.State.Learn((ushort)inFlag);
        }
        public void Forget(ViewStateFlag inFlag)
        {
            this.State.Forget((ushort)inFlag);
        }
        public bool Has(ViewStateFlag inFlag)
        {
            return this.State.Has((ushort)inFlag);
        }
    }

    //view flag 标记 一共32位
    public enum ViewStateFlag : ushort
    {
        None = 0,
        BindFacade = 1,
        Awake = 2,
        Destroy = 3,
        Show = 4,
        Hide = 5,
        Visible = 6,
        NotVisible = 7,
        Focus = 8,
        NotFocus = 9,


        Components = 31,
    }

}