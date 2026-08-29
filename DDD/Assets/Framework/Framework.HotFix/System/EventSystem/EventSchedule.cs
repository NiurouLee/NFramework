using System.Collections.Generic;
using  NFramework.Core;

namespace NFramework.ModuleSystem.Module.EventModule
{
    public partial class EventSchedule : NObject, IEventScheduler
    {
        private readonly UnOrderMultiMapLink<System.Type, BaseRegister> m_EventHandler;

        private readonly Dictionary<System.Type, LinkedListNode<BaseRegister>> m_CachedNodes;

        private readonly Dictionary<System.Type, LinkedListNode<BaseRegister>> m_TempNodes;

        public EventSchedule()
        {
            m_EventHandler = new UnOrderMultiMapLink<System.Type, BaseRegister>();
            m_CachedNodes = new Dictionary<System.Type, LinkedListNode<BaseRegister>>();
            m_TempNodes = new Dictionary<System.Type, LinkedListNode<BaseRegister>>();
        }

        public int EventHandlerCount
        {
            get { return m_EventHandler.Count; }
        }


        public void Shutdown()
        {
            Clear();
            m_EventHandler.Clear();
            m_CachedNodes.Clear();
            m_TempNodes.Clear();
        }

        public void Clear()
        {
        }

        public int Count(System.Type type)
        {
            LinkedListRange<BaseRegister> range = default(LinkedListRange<BaseRegister>);
            if (m_EventHandler.TryGetValue(type, out range))
            {
                return range.Count;
            }

            return 0;
        }

        private bool Check(BaseRegister inHandler)
        {
            if (inHandler == null)
            {
                throw new System.Exception("Event handler is invalid.");
            }

            var type = inHandler.EventType;
            var result = m_EventHandler.Contains(type, inHandler);
            inHandler.FreeToPool();
            return result;
        }

        private BaseRegister _Subscribe(BaseRegister handler)
        {
            if (handler == null)
            {
                throw new System.Exception("Event handler is invalid.");
            }

            m_EventHandler.Add(handler.EventType, handler);
            return handler;
        }

        private void _Unsubscribe(BaseRegister inHandler)
        {
            if (inHandler == null)
            {
                throw new System.Exception("Event handler is invalid.");
            }

            //检查是不是有在执行中的下一个
            if (m_CachedNodes.Count > 0)
            {
                foreach (var cachedNode in m_CachedNodes)
                {
                    //如果有在执行中的下一个要被移除，那就换成下一个的下一个
                    if (cachedNode.Value != null && cachedNode.Value.Value == inHandler)
                    {
                        var h1 = cachedNode.Value.Value;
                        m_TempNodes[cachedNode.Key] = cachedNode.Value.Next;
                        h1.FreeToPool();
                    }
                }
            }

            //修改Cache的指针
            if (m_TempNodes.Count > 0)
            {
                foreach (var cachedNode in m_TempNodes)
                {
                    m_CachedNodes[cachedNode.Key] = cachedNode.Value;
                }

                m_TempNodes.Clear();
            }

            var type = inHandler.EventType;
            if (m_EventHandler.TryRemove(type, inHandler, out var outHandler))
            {
                outHandler.FreeToPool();
            }

            inHandler.FreeToPool();
        }

        public void Fire<T>(ref T e) where T : IEvent
        {
            if (e == null)
            {
                throw new System.Exception("Event is invalid.");
            }

            HandleEvent(ref e);
        }


        /// <summary>
        /// 处理事件结点。
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="e">事件参数。</param>
        private void HandleEvent<T>(ref T e) where T : IEvent
        {
            var type = typeof(T);
            LinkedListRange<BaseRegister> range = default(LinkedListRange<BaseRegister>);
            if (m_EventHandler.TryGetValue(e.GetType(), out range))
            {
                var current = range.First;
                while (current != null && current != range.Terminal)
                {
                    //先记录下一个节点
                    m_CachedNodes[type] = current.Next != range.Terminal ? current.Next : null;
                    if (current.Value is ConditionRegister conditionRegister)
                    {
                        if (conditionRegister.Condition != null && conditionRegister.Condition is RefFunc<T> cond &&
                            cond(ref e))
                        {
                            current.Value.Invoke(e);
                        }
                    }
                    else if (e is IChannelEvent channelEvent && current.Value is ChannelRegister channelRegister)
                    {
                        var channel = channelRegister.Channel;
                        if (string.IsNullOrEmpty(channel) || channel == channelEvent.Channel)
                        {
                            current.Value.Invoke(e);
                        }
                        else
                        {
                            UnityEngine.Debug.LogError($"Event {type} not match channel");
                            current.Value.Invoke(e);
                        }
                    }
                    else
                    {
                        current.Value.Invoke(e);
                    }

                    //再等于之前记录的(防止执行过程中移除了下一个)
                    current = m_CachedNodes[type];
                }

                //移除缓存
                m_CachedNodes.Remove(type);
            }
        }
    }
}