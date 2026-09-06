using Cysharp.Threading.Tasks;
using NFramework.ModuleSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Logic
{
    /// <summary>
    /// RPG Demo 的 Context：负责创建/取得 RPGWorld，并启动一个纯日志的自动对战流程。
    /// </summary>
    public class RPGCombatContext : Context
    {
        public RPGWorld World { get; private set; }
        public RPGCombatUnit Hero => this.World?.Hero;
        public RPGCombatUnit Monster => this.World?.Monster;
        public bool IsRunning { get; private set; }

        /// <summary>
        /// 打开 RpgExample 场景时自动启动（在已有工程里可直接调用 StartDemo() 手动启动）。
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void TryAutoStart()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (SceneManager.GetActiveScene().name != "RPGExample")
            {
                return;
            }

            NFROOT.I.GetSystem<ContextSystem>().GetContext<RPGCombatContext>().StartDemo();
        }

        public void StartDemo()
        {
            if (this.IsRunning)
            {
                return;
            }

            this.IsRunning = true;
            RunDemoAsync().Forget();
        }

        /// <summary>Hero 手动普攻入口（可绑定到按键/UI）</summary>
        public void ManualHeroAttack()
        {
            if (this.World == null || this.World.Hero == null || this.World.Monster == null)
            {
                Log("战斗尚未初始化");
                return;
            }

            this.World.Attack(this.World.Hero, this.World.Monster);
        }

        private async UniTaskVoid RunDemoAsync()
        {
            Log("RPG 战斗 Demo 启动");

            if (this.World == null)
            {
                this.World = this.GetSystem<WorldSystem>().GetWorld<RPGWorld>();
            }

            this.World.ClearBattle();
            this.World.CreateHero("主角", 500, 80);
            this.World.CreateMonster("骷髅兵", 420, 70);

            Log($"对战开始：{this.World.Hero.DisplayName} HP {this.World.Hero.Hp} VS " +
                $"{this.World.Monster.DisplayName} HP {this.World.Monster.Hp}");

            bool heroTurn = true;
            while (this.World != null && !this.World.Hero.IsDead && !this.World.Monster.IsDead)
            {
                await UniTask.WaitForSeconds(1.2f);
                if (this.World == null || this.World.Hero == null || this.World.Monster == null)
                {
                    break;
                }

                if (this.World.Hero.IsDead || this.World.Monster.IsDead)
                {
                    break;
                }

                if (heroTurn)
                {
                    this.World.Attack(this.World.Hero, this.World.Monster);
                }
                else
                {
                    this.World.Attack(this.World.Monster, this.World.Hero);
                }

                heroTurn = !heroTurn;
            }

            if (this.World != null)
            {
                if (this.World.Hero.IsDead)
                {
                    Log("战斗结束：主角阵亡");
                }
                else if (this.World.Monster.IsDead)
                {
                    Log("战斗结束：怪物被击败");
                }
                else
                {
                    Log("战斗结束");
                }
            }

            this.IsRunning = false;
        }

        private void Log(string inMessage)
        {
            this.GetSystem<LoggerSystem>()?.Log?.Print($"[RPG Demo] {inMessage}");
        }
    }
}
