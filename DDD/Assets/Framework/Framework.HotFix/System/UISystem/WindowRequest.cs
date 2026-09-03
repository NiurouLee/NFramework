using System;
using Cysharp.Threading.Tasks;

namespace NFramework.ModuleSystem
{
    /// <summary>
    ///  UIRequest阶段,当前到那个阶段了
    /// </summary>
    public enum WindowRequestStage : Byte
    {
        None = 0,
        Construct = 1,
        Cache = 2,
        FacadeLoading = 4,
        FacadeLoaded = 5,
        Layer = 6,
        WindowAwake = 8,
        WindowOpen = 9,
        WindowOpenAnim = 10,
        WindowClose = 11,
        WindowCloseAnim = 12,
        GameObjectUnloading = 13,
        Invalid = 14,
    }

    /// <summary>
    /// 把打开一个UI封装成Request
    /// </summary>
    public abstract class WindowRequest : NObject, IEquatable<WindowRequest>
    {
        public string Name { get; private set; }
        public ViewConfig Config { get; private set; }
        public WindowRequestStage Stage { get; private set; }

        /// <summary>
        /// 同一 ViewConfig 下区分多个窗口实例的 key；不传（null/空串）时为默认单例实例。
        /// </summary>
        public string keyObj { get; private set; }

        public Window CacheWindowObj { get; private set; }
        public UIFacade CacheFacadeObj { get; private set; }
        public System.Object CacheViewDataObj { get; private set; }
        public IUIFacadeProvider CacheProviderObj { get; private set; }

        public int CacheOrderObj { get; private set; }

        /// <summary>分隔请求键与业务 key，避免组合串歧义</summary>
        private const char KeySeparator = '\u0001';

        /// <summary>
        /// 拼接窗口唯一请求键：不传 key 时等于窗口 ID，保证旧接口仍是“每类型单例”；
        /// 传 key 时同类型不同 key 可并存。
        /// </summary>
        public static string MakeRequestKey(string inWindowID, string inKey)
        {
            return string.IsNullOrEmpty(inKey) ? inWindowID : string.Concat(inWindowID, KeySeparator, inKey);
        }

        /// <summary>当前请求在 WindowRequestDictionary 中使用的唯一键</summary>
        public string RequestKey => MakeRequestKey(this.Name, this.keyObj);


        public WindowRequest(ViewConfig inConfig)
        {
            if (inConfig == null)
            {
                this.GetSystem<LoggerSystem>()?.ErrStack(" WindowRequest inConfig is null");
            }

            this.Config = inConfig;
            this.Name = inConfig.ID;
        }

        public void SetStage(WindowRequestStage inStage)
        {
            if (inStage == this.Stage)
            {
                NFROOT.Instance.GetSystem<LoggerSystem>()
                    ?.ErrStack(
                        $"WindowRequest Err:Stage Repeat,WindowName：{this.Name},Stage：{this.Stage},NewStage：{inStage}");
            }

            if (inStage < this.Stage)
            {
                NFROOT.Instance.GetSystem<LoggerSystem>()
                    ?.ErrStack(
                        $"WindowRequest Err:Stage Inverse,WindowName：{this.Name},Stage：{this.Stage},NewStage：{inStage}");
            }

            this.GetSystem<LoggerSystem>().Log
                ?.Print($"WindowRequest SetStage,WindowName：{this.Name},Stage：{this.Stage},NewStage：{inStage}");
            this.Stage = inStage;
        }

        public virtual void CacheKey(string key)
        {
            this.keyObj = string.IsNullOrEmpty(key) ? string.Empty : key;
        }

        public virtual void CacheWindow(Window inWindow)
        {
            if (inWindow == null)
            {
                NFROOT.Instance.GetSystem<LoggerSystem>()
                    ?.ErrStack($"WindowRequest set Window Err,inWindow is null:WindowName{this.Name}");
            }

            if (this.CacheWindowObj != null)
            {
                NFROOT.Instance.GetSystem<LoggerSystem>()
                    ?.ErrStack($"WindowRequest set Window Err,Window dont is null:WindowName{this.Name}");
            }

            this.CacheWindowObj = inWindow;
        }

        public virtual void CacheOrder(int inOrder)
        {
            this.CacheOrderObj = inOrder;
        }

        public virtual void CacheWindowAndData(Window inWindow, System.Object obj)
        {
            this.CacheWindow(inWindow);
            this.CacheViewData(obj);
        }


        public virtual void CacheFacade(UIFacade inFacade)
        {
            if (inFacade == null)
            {
                NFROOT.Instance.GetSystem<LoggerSystem>()
                    ?.ErrStack($"WindowRequest set Facade Err,inFacade is null:WindowName{this.Name}");
            }

            if (this.CacheFacadeObj != null)
            {
                NFROOT.Instance.GetSystem<LoggerSystem>()
                    ?.ErrStack($"WindowRequest set Facade Err,Facade dont is null:WindowName{this.Name}");
            }

            this.CacheFacadeObj = inFacade;
        }


        public virtual void CacheViewData(System.Object inViewData)
        {
            if (inViewData == null)
            {
                NFROOT.Instance.GetSystem<LoggerSystem>()
                    ?.ErrStack($"WindowRequest set ViewData Err,inViewData is null:WindowName{this.Name}");
            }

            if (this.CacheViewDataObj != null)
            {
                NFROOT.Instance.GetSystem<LoggerSystem>()
                    ?.ErrStack($"WindowRequest set ViewData Err,ViewData dont is null:WindowName{this.Name}");
            }

            this.CacheViewDataObj = inViewData;
        }

        public virtual void CacheProvider(IUIFacadeProvider inProvider)
        {
            if (inProvider == null)
            {
                NFROOT.Instance.GetSystem<LoggerSystem>()
                    ?.ErrStack($"WindowRequest set Provider Err,inProvider is null:WindowName{this.Name}");
            }

            if (this.CacheProviderObj != null)
            {
                NFROOT.Instance.GetSystem<LoggerSystem>()
                    ?.ErrStack($"WindowRequest set Provider Err,Provider dont is null:WindowName{this.Name}");
            }

            this.CacheProviderObj = inProvider;
        }

        public bool Equals(WindowRequest other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name && this.keyObj == other.keyObj;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Name != null ? Name.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (this.keyObj != null ? this.keyObj.GetHashCode() : 0);
                return hashCode;
            }
        }

        public virtual void SetupViewData()
        {
        }


        /// <summary>
        /// 返回给业务的UniTask
        /// </summary>
        public UniTaskCompletionSource Deferred;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deferred"></param>
        public virtual void SetTaskCompletionSource(UniTaskCompletionSource deferred)
        {
            this.Deferred = deferred;
        }

        /// <summary>请求是否已被取消（用于异步加载完成后丢弃结果）</summary>
        internal bool IsCanceled { get; private set; }

        internal void MarkCanceled()
        {
            this.IsCanceled = true;
        }

        public abstract void Awake();

        public virtual void Show()
        {
            this.CacheWindowObj.Show();
        }

        /// <summary>
        /// 从窗口池复用时调用：窗口已经 Awake，不应重复 Awake，
        /// 带数据的请求在这里把新数据 SetData 给已存在的窗口。
        /// </summary>
        public virtual void PrepareReuse()
        {
        }
    }


    public class WindowRequestByWindow : WindowRequest
    {
        public WindowRequestByWindow(ViewConfig inConfig) : base(inConfig)
        {
        }

        public void Setup(Window inWindow)
        {
            base.CacheWindow(inWindow);
        }

        public override void Awake()
        {
            var inWindow = this.CacheWindowObj;
            inWindow.SetUIFacade(this.CacheFacadeObj, this.CacheProviderObj);
            inWindow.Awake();
        }

        public override void Show()
        {
            var inWindow = this.CacheWindowObj;
            inWindow.Show();
        }
    }

    public class WindowRequestByData<TD> : WindowRequest where TD : class
    {
        public TD ViewData => this.CacheViewDataObj as TD;

        public WindowRequestByData(ViewConfig inConfig) : base(inConfig)
        {
        }

        public override void SetupViewData()
        {
            if (this.CacheViewDataObj != null && this.CacheWindowObj != null &&
                this.CacheWindowObj is IViewSetData<TD> viewSetData)
            {
                viewSetData.InitData(this.CacheViewDataObj as TD);
            }
        }

        public override void Awake()
        {
            var inWindow = this.CacheWindowObj;
            inWindow.SetUIFacade(this.CacheFacadeObj, this.CacheProviderObj);
            this.SetupViewData();
            inWindow.Awake();
        }

        public override void PrepareReuse()
        {
            if (this.CacheWindowObj is IViewSetData<TD> viewSetData &&
                this.CacheViewDataObj is TD data)
            {
                viewSetData.SetData(data);
            }
        }
    }

    public class WindowRequest<TW, TD> : WindowRequest where TW : Window where TD : class
    {
        public TW Window => base.CacheWindowObj as TW;
        public TD ViewData => this.CacheViewDataObj as TD;

        public WindowRequest(ViewConfig inConfig) : base(inConfig)
        {
        }

        public override void SetupViewData()
        {
            if (this.CacheViewDataObj != null && this.CacheWindowObj != null &&
                this.CacheWindowObj is IViewSetData<TD> viewSetData)
            {
                viewSetData.InitData(this.CacheViewDataObj as TD);
            }
        }

        public override void Awake()
        {
            var inWindow = this.CacheWindowObj;
            inWindow.SetUIFacade(this.CacheFacadeObj, this.CacheProviderObj);
            this.SetupViewData();
            inWindow.Awake();
        }

        public override void PrepareReuse()
        {
            if (this.CacheWindowObj is IViewSetData<TD> viewSetData &&
                this.CacheViewDataObj is TD data)
            {
                viewSetData.SetData(data);
            }
        }
    }

    public class WindowRequestByWindow<TW> : WindowRequest where TW : Window
    {
        public TW Window => base.CacheWindowObj as TW;

        public WindowRequestByWindow(ViewConfig inConfig) : base(inConfig)
        {
        }

        public override void Awake()
        {
            var inWindow = this.CacheWindowObj;
            inWindow.SetUIFacade(this.CacheFacadeObj, this.CacheProviderObj);
            this.SetupViewData();
            inWindow.Awake();
        }
    }
}
