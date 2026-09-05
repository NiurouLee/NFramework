using NFramework.ModuleSystem;

namespace Game.Logic
{
    public partial class Game
    {
        public static void StartGameLogic()
        {
            var excontent = GetSystem<ContextSystem>().CreateContext<ExContext>();
            excontent.Enter();
        }
    }
}