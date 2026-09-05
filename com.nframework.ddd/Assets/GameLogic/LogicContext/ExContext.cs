using NFramework.Boot;
using NFramework.ModuleSystem;

namespace Game.Logic
{
    public class ExContext : Context
    {
        public ExWorld ExWorld { get; private set; }


        public async void Enter()
        {
            this.ExWorld = this.GetSystem<WorldSystem>().CreateWorld<ExWorld>();
            await this.ExWorld.LoadScene();
            await GetSystem<UISystem>().OpenAsync<MainWindow>();
            AOTLoading.Instace.gameObject.SetActive(false);
            this.InitView();
        }

        private void InitView()
        {
        }
    }
}