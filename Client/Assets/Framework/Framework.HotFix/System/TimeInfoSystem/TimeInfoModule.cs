using System;

namespace NFramework.ModuleSystem
{
    public class TimeInfoSystem : FrameworkSystemModuleBase
    {
        private DateTime dt1970;
        private DateTime dt;

        public long ServerMinusClientTime { private get; set; }

        public TimeInfoSystem()
        {
            this.dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            this.dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }

        public void Update(float deltaTime)
        {
        }

        /// <summary> 
        /// 根据时间戳获取时间 
        /// </summary>  
        public DateTime ToDateTime(long timeStamp)
        {
            return dt.AddTicks(timeStamp * 10000);
        }

        // 线程安全
        /// <summary>
        /// 返回值是毫秒
        /// </summary>
        /// <returns></returns>
        public long ClientNow()
        {
            return (DateTime.UtcNow.Ticks - this.dt1970.Ticks) / 10000;
        }

        /// <summary>
        /// 返回值是毫秒
        /// </summary>
        /// <returns></returns>
        public long ServerNow()
        {
            return ClientNow() + ServerMinusClientTime;
        }

        public long Transition(DateTime d)
        {
            return (d.Ticks - dt.Ticks) / 10000;
        }
    }
}