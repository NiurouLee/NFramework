using NFramework.ModuleSystem;

namespace Game.Logic
{
    public partial class Game
    {
        public static void StartGameLogic()
        {
            GetSystem<ContextSystem>().CreateContext<ExContext>();
        }
    }
}