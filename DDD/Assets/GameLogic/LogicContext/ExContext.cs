using System.Collections;
using NFramework.ModuleSystem;

namespace Game.Logic
{
    public class ExContext : Context
    {
        public World ExWorld { get; private set; }


        public IEnumerator Enter()
        {
            this.ExWorld = this.GetSystem<WorldSystem>().CreateWorld<ExWorld>();
            yield return null;
        }
    }
}