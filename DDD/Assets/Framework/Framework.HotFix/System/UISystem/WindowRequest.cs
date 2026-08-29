using System;

using Cysharp.Threading.Tasks;

namespace NFramework.ModuleSystem
{
    /// <summary>
    ///  UIRequest阶段,当前到那个阶段了
    /// </summary>
    [Flags]
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
        public Window CacheWindowObj { get; private set; }
        public UIFacade CacheFacadeObj { get; private set; }
        public System.Object CacheViewDataObj { get; private set; }
        public IUIFacadeProvider CacheProviderObj { get; private set; }

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

            this.GetSystem<LoggerSystem>().Log?.Print($"WindowRequest SetStage,WindowName：{this.Name},Stage：{this.Stage},NewStage：{inStage}");
            this.Stage = inStage;
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
            return Name == other.Name;
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
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

        internal void Cancel()
        {
        }

        public abstract void Awake();

        public virtual void Show()
        {
            this.CacheWindowObj.Show();
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
                viewSetData.SetData(this.CacheViewDataObj as TD);
            }
        }

        public override void Awake()
        {
            var inWindow = this.CacheWindowObj;
            inWindow.SetUIFacade(this.CacheFacadeObj, this.CacheProviderObj);
            this.SetupViewData();
            inWindow.Awake();
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
                viewSetData.SetData(this.CacheViewDataObj as TD);
            }
        }

        public override void Awake()
        {
            var inWindow = this.CacheWindowObj;
            inWindow.SetUIFacade(this.CacheFacadeObj, this.CacheProviderObj);
            this.SetupViewData();
            inWindow.Awake();
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
