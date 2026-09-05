using System;
using System.Collections.Generic;

namespace NFramework.ModuleSystem
{
    public class TimerWheel
    {
        public uint Index { get; private set; } = 0;

        /// <summary>
        /// 槽位 
        /// </summary>
        /// <value></value>
        public uint SlotSize { get; private set; }

        /// <summary>
        /// 步进刻度
        /// </summary>
        /// <value></value>
        public ulong TickInMS { get; private set; }

        /// <summary>
        /// 表盘最大时间
        /// </summary>
        /// <value></value>
        public ulong MaxTickInMS { get; private set; }

        private uint currentSlotIndex = 0;
        private List<TimerTask>[] slotArr = null;
        private List<TimerTask> willTriggerTaskList = new List<TimerTask>();
        public Action<uint, List<TimerTask>> WheelTriggerEvent = null;
        public Action<uint> WheelOutEvent = null;

        /// <summary>
        /// 当前时间
        /// </summary>
        public ulong RunTime => this.currentSlotIndex * TickInMS;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inIndex">索引</param>
        /// <param name="inTickInMS">每格走多少ms</param>
        /// <param name="inSlotSize">有几格子</param>
        public TimerWheel(uint inIndex, ulong inTickInMS, uint inSlotSize)
        {
            this.Index = inIndex;
            this.TickInMS = inTickInMS;
            this.SlotSize = inSlotSize;
            this.slotArr = new List<TimerTask>[SlotSize];
            this.MaxTickInMS = this.SlotSize * this.TickInMS;
        }

        public bool AddTimerTask(TimerTask inTask, ref int inSlotIndex)
        {
            if (inTask.RemainingWheelInMS >= this.MaxTickInMS)
            {
                inSlotIndex = -1;
                return false;
            }

            int _targetSlot = (int)(inTask.RemainingWheelInMS / TickInMS);
            if (_targetSlot == 0)
            {
                _targetSlot = 1;
                inTask.RemainingWheelInMS = 0;
            }
            else
            {
                inTask.RemainingWheelInMS = inTask.RemainingWheelInMS % TickInMS;
            }

            inSlotIndex = (int)(currentSlotIndex + _targetSlot);
            inSlotIndex = inSlotIndex % (int)SlotSize;
            if (slotArr[inSlotIndex] == null)
            {
                slotArr[inSlotIndex] = new List<TimerTask>();
            }

            slotArr[inSlotIndex].Add(inTask);
            return true;
        }

        public bool RemoveTimerTask(int inSlotIndex, long inTimerTaskID)
        {
            List<TimerTask> _taskList = slotArr[inSlotIndex];
            if (_taskList == null)
            {
                return false;
            }

            for (int i = 0; i < _taskList.Count; i++)
            {
                var _task = _taskList[i];
                if (_task.ID == inTimerTaskID)
                {
                    NFROOT.I.GetSystem<ObjectPoolSystem>().Free(_task);
                    _taskList.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public void DoTimerTurn(int inTurnNum)
        {
            for (int i = 0; i < inTurnNum; i++)
            {
                this.currentSlotIndex++;
                if (currentSlotIndex == SlotSize)
                {
                    this.currentSlotIndex = 0;
                    this.WheelOutEvent?.Invoke(this.Index);
                }

                if (slotArr[this.currentSlotIndex] != null)
                {
                    willTriggerTaskList.AddRange(slotArr[this.currentSlotIndex]);
                    slotArr[currentSlotIndex].Clear();
                }
            }

            if (this.willTriggerTaskList.Count > 0)
            {
                this.WheelTriggerEvent?.Invoke(this.Index, willTriggerTaskList);
                this.willTriggerTaskList.Clear();
            }
        }
    }
}