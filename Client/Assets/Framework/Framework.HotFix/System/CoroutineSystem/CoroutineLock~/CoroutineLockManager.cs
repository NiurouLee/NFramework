using ET;
using System;
using System.Collections.Generic;

namespace NFramework.Module.Coroutine
{
    public class CoroutineLockManager : AObjectBase, IUpdate
    {
        public static CoroutineLockManager Instance { get; set; }

        private readonly List<CoroutineLockQueueType> list = new List<CoroutineLockQueueType>(CoroutineLockType.Max);
        private readonly Queue<(int, long, int)> nextFrameRun = new Queue<(int, long, int)>();


        public override void Awake()
        {
            base.Awake();

            for (int i = 0; i < CoroutineLockType.Max; ++i)
            {
                CoroutineLockQueueType coroutineLockQueueType = new CoroutineLockQueueType(i);
                list.Add(coroutineLockQueueType);
            }
            Instance = this;
        }
        public override void OnDestroy()
        {
            base.OnDestroy();

            list.Clear();
            nextFrameRun.Clear();

            Instance = null;
        }

        public async ETTask<CoroutineLock> Wait(int coroutineLockType, long key, int time = 60000)
        {
            CoroutineLockQueueType coroutineLockQueueType = list[coroutineLockType];
            return await coroutineLockQueueType.Wait(key, time);
        }
        private void Notify(int coroutineLockType, long key, int level)
        {
            CoroutineLockQueueType coroutineLockQueueType = list[coroutineLockType];
            coroutineLockQueueType.Notify(key, level);
        }
        public void RunNextCoroutine(int coroutineLockType, long key, int level)
        {
            //һ��Э�̶���һ֡������100��,˵���Ƚ϶���,���warning,���һ���Ƿ�����
            if (level == 100)
            {
                LogHelper.Warning($"too much coroutine level: {coroutineLockType} {key}");
            }

            this.nextFrameRun.Enqueue((coroutineLockType, key, level));
        }
        public void Update()
        {

            //ѭ�������л��ж�������������
            while (this.nextFrameRun.Count > 0)
            {
                (int coroutineLockType, long key, int count) = this.nextFrameRun.Dequeue();
                this.Notify(coroutineLockType, key, count);
            }

        }

    }
}