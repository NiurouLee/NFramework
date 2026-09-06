using System.Collections.Generic;

namespace NFramework.ModuleSystem.Combat
{
    public partial class CombatSystem : FrameworkSystemModuleBase
    {
        public Dictionary<int, CombatContext> CombatDic { get; private set; }

        public CombatContext CurrentContext { get; private set; }

        public override void Awake()
        {
            CombatDic = new Dictionary<int, CombatContext>();
            base.Awake();
        }

        public CombatContext CreateCombatContext(int inID)
        {
            if (this.CombatDic.ContainsKey(inID))
            {
                GetSystem<LoggerSystem>().Error?.Print("combatContext have ");
                return null;
            }

            var combatContext = GetSystem<WorldSystem>().CreateWorld<CombatContext>();
            this.CombatDic.Add(inID, combatContext);
            this.CurrentContext = combatContext;
            return combatContext;
        }
    }
}