using System;
using System.Collections.Generic;
using  NFramework.Core;

namespace NFramework.ModuleSystem
{
    public class EntitySystem : FrameworkSystemModuleBase
    {
        private class OneTypeSystems
        {
            public readonly UnOrderMultiMapList<Type, object> Map = new();

            // 这里不用hash，数量比较少，直接for循环速度更快
            public readonly bool[] QueueFlag = new bool[(int)InstanceQueueIndex.Max];
        }

        private class TypeSystems
        {
            private readonly Dictionary<Type, OneTypeSystems> typeSystemsMap = new();

            public OneTypeSystems GetOrCreateOneTypeSystems(Type type)
            {
                OneTypeSystems systems = null;
                this.typeSystemsMap.TryGetValue(type, out systems);
                if (systems != null)
                {
                    return systems;
                }

                systems = new OneTypeSystems();
                this.typeSystemsMap.Add(type, systems);
                return systems;
            }

            public OneTypeSystems GetOneTypeSystems(Type type)
            {
                OneTypeSystems systems = null;
                this.typeSystemsMap.TryGetValue(type, out systems);
                return systems;
            }

            public List<object> GetSystems(Type type, Type systemType)
            {
                OneTypeSystems oneTypeSystems = null;
                if (!this.typeSystemsMap.TryGetValue(type, out oneTypeSystems))
                {
                    return null;
                }

                if (!oneTypeSystems.Map.TryGetValue(systemType, out List<object> systems))
                {
                    return null;
                }

                return systems;
            }
        }

        private Dictionary<long, Entity> entities = new();
        private readonly Queue<long>[] queues = new Queue<long>[(int)InstanceQueueIndex.Max];
        private readonly Queue<long> startQueue = new Queue<long>();


        public EntitySystem()
        {
            for (int i = 0; i < this.queues.Length; i++)
            {
                this.queues[i] = new Queue<long>();
            }

            NFROOT.Instance.AddFixedUpdateCallback(FixedUpdate);
            NFROOT.Instance.AddUpdateCallback(RendererUpdate);
            NFROOT.Instance.AddUpdateCallback(LogicUpdate);
            NFROOT.Instance.AddLateUpdateCallback(LateUpdate);
        }

        private const float logicUpdateTime = 1 / 15f;
        private float logicUpdateTimer = logicUpdateTime;

        private void LogicUpdate(float deltaTime)
        {
            logicUpdateTimer += deltaTime;
            if (logicUpdateTimer >= logicUpdateTime)
            {
                Update(logicUpdateTimer);
                logicUpdateTimer = 0;
            }
        }

        public Queue<long> GetQueueIndex(InstanceQueueIndex index)
        {
            return this.queues[(int)index];
        }


        public void RegisterSystem(Entity component)
        {
            Type type = component.GetType();

            foreach (KeyValuePair<Type, InstanceQueueIndex> instanceQueueIndex in InstanceQueueMap.InstanceQueueMapDic)
            {
                if (instanceQueueIndex.Key.IsAssignableFrom(type))
                {
                    this.queues[(int)instanceQueueIndex.Value].Enqueue(component.Id);
                }
            }
        }

        public void Start(Entity component)
        {
            LivingSystem.Start(component);
        }

        public void Awake(Entity component)
        {
            LivingSystem.Awake(component);
        }

        public void Awake<P1>(Entity component, P1 p1)
        {
            LivingSystem.Awake(component, p1);
        }

        public void Awake<P1, P2>(Entity component, P1 p1, P2 p2)
        {
            LivingSystem.Awake(component, p1, p2);
        }

        public void Awake<P1, P2, P3>(Entity component, P1 p1, P2 p2, P3 p3)
        {
            LivingSystem.Awake(component, p1, p2, p3);
        }

        public void Awake<P1, P2, P3, P4>(Entity component, P1 p1, P2 p2, P3 p3, P4 p4)
        {
            LivingSystem.Awake(component, p1, p2, p3, p4);
        }

        public void Destroy(Entity component)
        {
            LivingSystem.Destroy(component);
        }

        private void Update(float deltaTime)
        {
            while (startQueue.Count > 0)
            {
                var entity = this.entities[startQueue.Dequeue()];
                (entity as IStartSystem)?.Start();
            }

            Queue<long> queue = this.queues[(int)InstanceQueueIndex.Update];
            int count = queue.Count;
            while (count-- > 0)
            {
                long instanceId = queue.Dequeue();
                Entity component = this.entities[instanceId];
                if (component == null)
                {
                    continue;
                }

                if (component.IsDisposed)
                {
                    continue;
                }

                queue.Enqueue(instanceId);

                if (!component.Enable)
                {
                    continue;
                }

                if (component is IUpdateSystem iUpdateSystem)
                {
                    try
                    {
                        iUpdateSystem.Update(deltaTime);
                    }
                    catch (Exception e)
                    {
                        GetSystem<LoggerSystem>()?.Error?.Print($"报错信息：{component.GetType().FullName} \n {e}");
                    }
                }
            }
        }

        private void FixedUpdate(float deltaTime)
        {
            Queue<long> queue = this.queues[(int)InstanceQueueIndex.FixedUpdate];
            int count = queue.Count;
            while (count-- > 0)
            {
                long instanceId = queue.Dequeue();
                Entity component = this.entities[instanceId];
                if (component == null)
                {
                    continue;
                }

                if (component.IsDisposed)
                {
                    continue;
                }

                queue.Enqueue(instanceId);

                if (!component.Enable)
                {
                    continue;
                }

                if (component is IFixedUpdateSystem iUpdateSystem)
                {
                    iUpdateSystem.FixedUpdate(deltaTime);
                }
            }
        }

        private void RendererUpdate(float deltaTime)
        {
            while (startQueue.Count > 0)
            {
                var entity = this.entities[startQueue.Dequeue()];
                (entity as IStartSystem)?.Start();
            }

            Queue<long> queue = this.queues[(int)InstanceQueueIndex.RendererUpdate];
            int count = queue.Count;
            while (count-- > 0)
            {
                long instanceId = queue.Dequeue();
                Entity component = this.entities[instanceId];
                if (component == null)
                {
                    continue;
                }

                if (component.IsDisposed)
                {
                    continue;
                }

                queue.Enqueue(instanceId);

                if (!component.Enable)
                {
                    continue;
                }

                if (component is IRendererUpdateSystem iUpdateSystem)
                {
                    iUpdateSystem.RendererUpdate(deltaTime);
                }
            }
        }

        public void LateUpdate(float deltaTime)
        {
            Queue<long> queue = this.queues[(int)InstanceQueueIndex.LateUpdate];
            int count = queue.Count;
            while (count-- > 0)
            {
                long instanceId = queue.Dequeue();
                Entity component = this.entities[instanceId];
                if (component == null)
                {
                    continue;
                }

                if (component.IsDisposed || !component.Enable)
                {
                    continue;
                }

                queue.Enqueue(instanceId);

                if (component is ILateUpdateSystem iLateUpdateSystem)
                {
                    iLateUpdateSystem.LateUpdate(deltaTime);
                }
            }
        }
    }
}