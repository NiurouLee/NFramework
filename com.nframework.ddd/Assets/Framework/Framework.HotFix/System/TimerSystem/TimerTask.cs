using System;
using    NFramework.ModuleSystem;

namespace NFramework.ModuleSystem
{
    public class TimerTask : IFreeToPool
    {
        public long ID { get; set; } = 0;
        /// <summary>
        /// 总计时时间
        /// </summary>
        public ulong IntervalInMS { get; set; } = 0;
        /// <summary>
        /// 剩余时间
        /// </summary>
        /// <value></value>
        public ulong RemainingWheelInMS { get; set; } = 0;
        /// <summary>
        /// 总时长 0 => Loop
        /// </summary>
        private ulong totalInMS = 0;
        /// <summary>
        /// 剩余次数
        /// </summary>
        private ulong leftInMS = 0;
        private Action onStartEvent = null;
        private Action onIntervalEvent = null;
        private Action onEndEvent = null;

        public TimerTask()
        {

        }

        public void Init(ulong inIntervalInMS, ulong inTotalMS, Action inStartCallback, Action inIntervalCallback, Action inEndCallback)
        {
            this.IntervalInMS = inIntervalInMS;
            if (inTotalMS <= 0)
            {
                this.totalInMS = 0;
            }
            else
            {
                this.totalInMS = inTotalMS;
            }

            this.onStartEvent = inStartCallback;
            this.onIntervalEvent = inIntervalCallback;
            this.onEndEvent = inEndCallback;

            this.RemainingWheelInMS = this.IntervalInMS;
            this.leftInMS = this.totalInMS;
        }

        public void OnTaskStart()
        {
            this.onStartEvent?.Invoke();
        }

        public void OnTrigger()
        {
            this.onIntervalEvent?.Invoke();
            //loop
            if (totalInMS == 0)
            {
                this.RemainingWheelInMS = IntervalInMS;
            }
            else if (totalInMS > 0)
            {
                leftInMS -= IntervalInMS;
                if (leftInMS > 0)
                {
                    if (leftInMS >= IntervalInMS)
                    {
                        RemainingWheelInMS = IntervalInMS;
                    }
                    else
                    {
                        RemainingWheelInMS = leftInMS;
                    }
                }
                else
                {
                    onEndEvent?.Invoke();
                }
            }
        }


        public bool IsValid()
        {
            if (IntervalInMS <= 0)
            {
                return false;
            }
            return totalInMS >= 0;
        }

        public void Clear()
        {
            this.ID = 0;
            this.IntervalInMS = 0;
            this.RemainingWheelInMS = 0;
            this.totalInMS = 0;
            this.leftInMS = 0;
            this.onStartEvent = null;
            this.onIntervalEvent = null;
            this.onEndEvent = null;
        }

        public void FreeToPool()
        {
            this.Clear();
        }
    }

}