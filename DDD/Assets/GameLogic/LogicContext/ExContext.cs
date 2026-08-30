using System.Collections;
using NFramework.ModuleSystem;

namespace Game.Logic
{
    public class ExContext : Context
    {
        public World ExWorld { get; private set; }


        public void Enter()
        {
            GetSystem<UISystem>().OpenAsync<ExampleExwindow>();
            this.ExWorld = this.GetSystem<WorldSystem>().CreateWorld<ExWorld>();
        }
    }
}