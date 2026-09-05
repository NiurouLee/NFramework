using System.Collections.Generic;
using NFramework.Core.Live;
using NFramework.Module.LogModule;

namespace NFramework.Module.Combat
{
  public class CombatM : FrameworkModule
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
        GetM<LoggerM>().Err("combatContext have ");
        return null;
      }
      var combatContext = this.AddChild<CombatContext>();
      this.CombatDic.Add(inID, combatContext);
      this.CurrentContext = combatContext;
      return combatContext;
    }

  }
}
