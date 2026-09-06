using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using   NFramework.ModuleSystem;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// 记录View上未完成的异步任务,View销毁时统一取消
    /// </summary>
    public class PromiseRecords : IFreeToPool
    {
        private readonly List<ICancelPromise> m_promises = new List<ICancelPromise>();

        public void FreeToPool()
        {
            m_promises.Clear();
        }

        public void Awake()
        {
            m_promises.Clear();
        }

        public void TryAdd(ICancelPromise inPromise)
        {
            if (inPromise != null)
            {
                m_promises.Add(inPromise);
            }
        }

        public void CancelAll()
        {
            foreach (var promise in m_promises)
            {
                promise.TrySetCanceled();
            }
            m_promises.Clear();
        }
    }

    public class ViewPromiseComponent : ViewComponent
    {
        private PromiseRecords m_promiseRecords;

        public PromiseRecords PromiseRecords
        {
            get
            {
                if (m_promiseRecords == null)
                {
                    m_promiseRecords = GetSystem<ObjectPoolSystem>().Alloc<PromiseRecords>();
                    m_promiseRecords.Awake();
                }

                return m_promiseRecords;
            }
        }

        public override void OnViewComponentDestroy()
        {
            if (m_promiseRecords != null)
            {
                m_promiseRecords.CancelAll();
                GetSystem<ObjectPoolSystem>().Free(m_promiseRecords);
                m_promiseRecords = null;
            }
        }
    }

    public static class ViewPromiseComponentExtensions
    {
        public static void AddPromise(this View inView, ICancelPromise inPromise)
        {
            var component = ViewUtils.CheckAndAdd<ViewPromiseComponent>(inView);
            component.PromiseRecords.TryAdd(inPromise);
        }
    }
}
