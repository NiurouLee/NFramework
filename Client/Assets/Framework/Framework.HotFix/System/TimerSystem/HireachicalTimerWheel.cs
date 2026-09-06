using System.Collections.Generic;
using   NFramework.ModuleSystem;
using   NFramework.ModuleSystem;

namespace NFramework.ModuleSystem
{
    public class HireachicalTimerWheel
    {
        private long taskID = 0;
        private float lapseTime = 0f;
        private TimerWheel[] wheelArr = new TimerWheel[5];
        private int wheelArrLength = 5;
        public ulong MaxInterval = 36000000 * 24;
        public Dictionary<long, TaskTimerInfo> AllTaskInfoDic;



        public HireachicalTimerWheel()
        {
            AllTaskInfoDic = new Dictionary<long, TaskTimerInfo>();
            wheelArr[0] = new TimerWheel(0, 50, 20); //转一圈是1s 走一格是50ms
            wheelArr[1] = new TimerWheel(1, 1000, 60); //转一圈是一分钟， 走一格是1s
            wheelArr[2] = new TimerWheel(2, 60000, 60); //转一圈是1小时，走一格是1分钟
            wheelArr[3] = new TimerWheel(3, 3600000, 24); //转一圈是1天，走一格是1小时
            wheelArr[4] = new TimerWheel(4, MaxInterval, 30); //转一圈是1月，走一格是1天
            for (int i = 0; i < wheelArr.Length; i++)
            {
                wheelArr[i].WheelTriggerEvent = this.OnTimerWheelTrigger;
                wheelArr[i].WheelOutEvent = this.OnTimerWheelOut;
            }
        }

        public void OnUpdate(float inDeltaTime)
        {
            lapseTime += (inDeltaTime * 1000);
            var _timeWheel = wheelArr[0];
            uint _turnNum = (uint)(lapseTime / _timeWheel.TickInMS);
            _turnNum = _turnNum > _timeWheel.SlotSize ? _timeWheel.SlotSize : _turnNum;
            lapseTime -= _turnNum * _timeWheel.TickInMS;
        }

        public long AddTimerTask(TimerTask inTask)
        {
            var _taskTimerInfo = NFROOT.I.GetSystem<ObjectPoolSystem>().Alloc<TaskTimerInfo>();
            if (AddTimerTaskInteral(inTask, _taskTimerInfo, true))
            {
                inTask.OnTaskStart();
                return _taskTimerInfo.TaskID;
            }
            else
            {
                throw new System.Exception("AddTimerTask Failed");
            }
        }

        private bool AddTimerTaskInteral(TimerTask inTask, TaskTimerInfo inTaskInfo, bool IsInited)
        {
            if (!inTask.IsValid())
            {
                return false;
            }

            for (uint i = 0; i < wheelArrLength; i++)
            {
                if (IsInited && i > 0)
                {
                    var addTime = wheelArr[i - 1].RunTime;
                    inTask.RemainingWheelInMS += addTime;
                }
                if (wheelArr[i].AddTimerTask(inTask, ref inTaskInfo.WheelSlotIndex))
                {
                    inTaskInfo.WheelIndex = i;
                    break;
                }
            }
            if (inTaskInfo.WheelIndex < 0 || inTaskInfo.WheelSlotIndex < 0)
            {
                return false;
            }
            var _id = ++taskID;
            inTask.ID = _id;
            inTaskInfo.TaskID = _id;
            AllTaskInfoDic.Add(_id, inTaskInfo);
            return true;
        }


        public void RemoveTimerTask(long inID)
        {
            if (this.AllTaskInfoDic.TryGetValue(inID, out var taskTimerInfo))
            {
                this.AllTaskInfoDic.Remove(inID);
                var _wheelIndex = taskTimerInfo.WheelIndex;
                var _wheelSlotIndex = taskTimerInfo.WheelSlotIndex;
                var _timerTaskID = taskTimerInfo.TaskID;
                NFROOT.I.GetSystem<ObjectPoolSystem>().Free(taskTimerInfo);
                if (_wheelIndex >= 0 && _wheelIndex < wheelArrLength)
                {
                    wheelArr[_wheelIndex].RemoveTimerTask(_wheelSlotIndex, _timerTaskID);
                }
            }
        }


        private void OnTimerWheelOut(uint inIndex)
        {
            if (inIndex >= 0 && inIndex < wheelArrLength - 1)
            {
                wheelArr[inIndex + 1].DoTimerTurn(1);
            }
        }

        private void OnTimerWheelTrigger(uint inIndex, List<TimerTask> inTaskList)
        {
            for (int i = 0; i < inTaskList.Count; i++)
            {
                var _task = inTaskList[i];
                if (_task == null)
                {
                    continue;
                }
                if (AllTaskInfoDic.TryGetValue(_task.ID, out var _taskTimerInfo))
                {
                    if (_task.RemainingWheelInMS == 0)
                    {
                        _task.OnTrigger();
                    }

                    AllTaskInfoDic.Remove(_task.ID);
                    if (_task.IsValid())
                    {
                        AddTimerTaskInteral(_task, _taskTimerInfo, false);
                    }
                    else
                    {
                        NFROOT.I.GetSystem<ObjectPoolSystem>().Free(_task);
                    }
                }
            }
        }


        public void Clear()
        {
            if (AllTaskInfoDic.Count > 0)
            {
                foreach (var _timerInfo in AllTaskInfoDic.Values)
                {
                    this.RemoveTimerTask(_timerInfo.TaskID);
                }
            }
        }
    }
}