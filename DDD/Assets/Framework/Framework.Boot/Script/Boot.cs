using UnityEngine;

namespace NFramework.Boot
{
  public class Boot : MonoBehaviour
  {
    public AOTLoading AOTLoading;
    private IJITServices m_jitServices = null;
    void Awake()
    {
      Debug.unityLogger.logEnabled = true;
      this.AOTLoading.gameObject.SetActive(true);
      StartInit();
    }

    async void StartInit()
    {
      try
      {
        AOTLog.Info("开始启动游戏");
        AOTLog.Info("框架初始化");
        // FW.Init(gameObject);
        AOTLog.Info("热更初始化");
        YooAssetService yooAssetService = new YooAssetService();
        AOTLog.Info("加载更新界面");
        // 连接UI事件，用于显示进度和错误信息
        // yooAssetService.OnStepChange += patchWindow.OnStepChange;
        // yooAssetService.OnFoundUpdateFiles += patchWindow.OnFoundUpdateFiles;
        // yooAssetService.OnDownloadProgress += patchWindow.OnDownloadProgress;
        // yooAssetService.OnError += patchWindow.OnError;
        IJITServices _jitServices = null;
#if UNITY_WEBGL
        _jitServices = new WxServices();
#else
        _jitServices = new HybridClrService();
#endif
        this.m_jitServices = _jitServices;
        // _jitServices.OnStepChange += patchWindow.OnStepChange;
        // _jitServices.OnError += patchWindow.OnError;
        AOTLog.Info("开始资源更新流程");
        bool initSuccess = await yooAssetService.InitializeAndUpdate();
        
        if (!initSuccess)
        {
            // 资源初始化/更新失败时不能继续，否则热更入口会在没有激活清单的情况下
            // 调用 YooAssets.LoadAssetSync 而报 “Can not found active package manifest !”
            AOTLog.Error("YooAsset 初始化/更新失败，终止启动流程");
            return;
        }

        AOTLog.Info("开始代码更新流程");
        await _jitServices.StartJITUpdate();
        AOTLog.Info("进入主入口");
        await _jitServices.EnterMainEntry();
        // Destroy(patchWindowGameObject);
        this.m_jitServices.Release();
        AOTLog.Info("游戏启动流程完成");
      }
      catch (System.Exception e)
      {
        AOTLog.Error($"启动流程异常: {e.Message}");
      }
    }

    public void Update()
    {
      // FW.Update(Time.deltaTime, Time.unscaledDeltaTime);
    }

    private void OnDestroy()
    {
      // FW.Shutdown();
    }

    public static void GameQuit(GameQuitType gameQuitType)
    {
      switch (gameQuitType)
      {
        case GameQuitType.Restart:
          UnityEngine.SceneManagement.SceneManager.LoadScene("Boot");
          break;
        case GameQuitType.Quit:
          Application.Quit();
          break;
      }
    }
  }
}
