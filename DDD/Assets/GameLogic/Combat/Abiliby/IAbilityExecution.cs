
using NFramework.Module.EntityModule;

namespace NFramework.Module.Combat
{
    /// <summary>
    /// 能力执行接口，具体的技能执行体，行为执行体等都需要实现这个接口，执行体是实际创建能力表现、执行能力表现、触发能力效果应用的地方。
    /// 执行体里可以存一些表现执行相关的临时的状态数据。
    /// </summary>
    public interface IAbilityExecution
    {
        public Entity Ability { get; set; }
        public CombatEntity Owner { get; }
        /// <summary>
        /// 开始执行
        /// </summary>
        public void BeginExecute();
        /// <summary>
        /// 结束执行
        /// </summary>
        public void EndExecute();
    }
}