using System;
using System.Collections.Generic;
using NFramework.Core;

namespace NFramework.ModuleSystem
{
    public partial class Entity : NObject, IDisposable
    {
        [Flags]
        public enum EntityFlagsSlot : int
        {
            None = 0,
            IsFromPool = 1,
            IsRegister = 2,
            IsComponent = 3,
            IsRoot = 4,
            IsValid = 5,
            IsDisposed = 6,
        }

        private long id;

        public long Id
        {
            get => this.id;
            protected set
            {
                this.id = value;
                this.IsDisposed = false;
                this.IsValid = true;
            }
        }

        /// <summary>
        /// 状态标志位
        /// </summary>
        public BitField16 Flags = new BitField16(0);

        protected Entity()
        {
        }

        private bool IsFromPool
        {
            get => this.Flags.GetBit((int)EntityFlagsSlot.IsFromPool);
            set => this.Flags.SetBit((int)EntityFlagsSlot.IsFromPool, value);
        }

        private bool IsRoot
        {
            get => this.Flags.GetBit((int)EntityFlagsSlot.IsRoot);
            set => this.Flags.SetBit((int)EntityFlagsSlot.IsRoot, value);
        }

        protected bool IsRegister
        {
            get => this.Flags.GetBit((int)EntityFlagsSlot.IsRegister);
            set
            {
                this.Flags.SetBit((int)EntityFlagsSlot.IsRegister, value);
                if (value)
                {
                    GetSystem<EntitySystem>().RegisterSystem(this);
                }
            }
        }

        private bool IsComponent
        {
            get => this.Flags.GetBit((int)EntityFlagsSlot.IsComponent);
            set => this.Flags.SetBit((int)EntityFlagsSlot.IsComponent, value);
        }

        public bool IsValid
        {
            get => this.Flags.GetBit((int)EntityFlagsSlot.IsValid);
            set => this.Flags.SetBit((int)EntityFlagsSlot.IsValid, value);
        }

        public bool IsDisposed
        {
            get => this.Flags.GetBit((int)EntityFlagsSlot.IsDisposed);
            set => this.Flags.SetBit((int)EntityFlagsSlot.IsDisposed, value);
        }

        protected Entity parent;

        // 可以改变parent，但是不能设置为null
        public Entity Parent
        {
            get => this.parent;
            private set
            {
                if (this.parent != null) // 之前有parent
                {
                    // parent相同，不设置
                    if (this.parent == value)
                    {
                        GetSystem<LoggerSystem>()?.Error
                            ?.Print($"重复设置了Parent: {this.GetType().Name} parent: {this.parent.GetType().Name}");
                        return;
                    }

                    this.parent.RemoveFromChildren(this);
                }

                this.parent = value;
                this.IsComponent = false;
                if (parent != null)
                {
                    this.parent.AddToChildren(this);
                }

                this.IsRegister = true;
            }
        }

        // 该方法只能在AddComponent中调用，其他人不允许调用
        private Entity ComponentParent
        {
            set
            {
                if (value == null)
                {
                    throw new Exception($"cant set parent null: {this.GetType().Name}");
                }

                if (value == this)
                {
                    throw new Exception($"cant set parent self: {this.GetType().Name}");
                }

                if (this.parent != null) // 之前有parent
                {
                    // parent相同，不设置
                    if (this.parent == value)
                    {
                        GetSystem<LoggerSystem>().Error
                            ?.Print($"重复设置了Parent: {this.GetType().Name} parent: {this.parent.GetType().Name}");
                    }

                    this.Parent.RemoveFromComponents(this);
                }

                this.Parent = value;
                this.IsComponent = true;
                this.Parent.AddToComponents(this);
            }
        }

        public T GetParent<T>() where T : Entity
        {
            return this.Parent as T;
        }

        public T GetRoot<T>() where T : Entity
        {
            if (this.Parent == null)
            {
                return this as T;
            }

            return this.Parent.GetRoot<T>();
        }

        private Dictionary<long, Entity> children;

        public Dictionary<long, Entity> Children
        {
            get { return this.children ??= DictionaryPool.Alloc<long, Entity>(); }
        }

        private void AddToChildren(Entity entity)
        {
            this.Children.Add(entity.Id, entity);
        }

        private void RemoveFromChildren(Entity entity)
        {
            if (this.children == null)
            {
                return;
            }

            this.children.Remove(entity.Id);

            if (this.children.Count == 0)
            {
                DictionaryPool.Free(this.children);
                this.children = null;
            }
        }

        private Dictionary<Type, Entity> components;

        public Dictionary<Type, Entity> Components
        {
            get { return this.components ??= DictionaryPool.Alloc<Type, Entity>(); }
        }

        private bool _enable = true;

        public bool Enable
        {
            get => _enable;
            set
            {
                _enable = value;
                if (value)
                    OnEnable();
                else
                    OnDisable();
            }
        }

        public void SetEnable(bool enable)
        {
            Enable = enable;
        }

        protected virtual void OnEnable()
        {
        }


        protected virtual void OnDisable()
        {
        }

        public void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            this.IsRegister = false;
            this.Id = 0;

            // 清理Component
            if (this.components != null)
            {
                foreach (KeyValuePair<Type, Entity> kv in this.components)
                {
                    kv.Value.Dispose();
                }

                this.components.Clear();
                DictionaryPool.Free(this.components);
                this.components = null;
            }

            // 清理Children
            if (this.children != null)
            {
                foreach (Entity child in this.children.Values)
                {
                    child.Dispose();
                }

                this.children.Clear();
                DictionaryPool.Free(this.children);
                this.children = null;
            }

            // 触发Destroy事件
            if (this is IDestroySystem)
            {
                GetSystem<EntitySystem>().Destroy(this);
            }


            if (this.parent != null && !this.parent.IsDisposed)
            {
                if (this.IsComponent)
                {
                    this.parent.RemoveComponent(this);
                }
                else
                {
                    this.parent.RemoveFromChildren(this);
                }
            }

            this.parent = null;

            Dispose();

            if (this.IsFromPool)
            {
                // ObjectPool.Ins.Recycle(this);
            }

            this.Flags.Clear();
            this.IsDisposed = true;
        }

        private void AddToComponents(Entity component)
        {
            this.Components.Add(component.GetType(), component);
        }

        private void RemoveFromComponents(Entity component)
        {
            if (this.components == null)
            {
                return;
            }

            this.components.Remove(component.GetType());

            if (this.components.Count == 0)
            {
                // ObjectPool.Ins.Recycle(this.components);
                this.components = null;
            }
        }

        public K GetChild<K>(long id) where K : Entity
        {
            if (this.children == null)
            {
                return null;
            }

            this.children.TryGetValue(id, out Entity child);
            return child as K;
        }

        public void RemoveChild(long id)
        {
            if (this.children == null)
            {
                return;
            }

            if (!this.children.TryGetValue(id, out Entity child))
            {
                return;
            }

            this.children.Remove(id);
            child.Dispose();
        }

        public void RemoveComponent<K>() where K : Entity
        {
            if (this.IsDisposed)
            {
                return;
            }

            if (this.components == null)
            {
                return;
            }

            Type type = typeof(K);
            Entity c = this.GetComponent(type);
            if (c == null)
            {
                return;
            }

            this.RemoveFromComponents(c);
            c.Dispose();
        }

        public void RemoveComponent(Entity component)
        {
            if (this.IsDisposed)
            {
                return;
            }

            if (this.components == null)
            {
                return;
            }

            Entity c = this.GetComponent(component.GetType());
            if (c == null)
            {
                return;
            }

            if (c.Id != component.Id)
            {
                return;
            }

            this.RemoveFromComponents(c);
            c.Dispose();
        }

        public void RemoveComponent(Type type)
        {
            if (this.IsDisposed)
            {
                return;
            }

            Entity c = this.GetComponent(type);
            if (c == null)
            {
                return;
            }

            RemoveFromComponents(c);
            c.Dispose();
        }

        public K GetComponent<K>()
        {
            if (this.components == null)
            {
                return default;
            }

            var type = typeof(K);
            Entity component = null;
            foreach (var items in components)
            {
                if (type.IsAssignableFrom(items.Key))
                {
                    component = items.Value;
                    break;
                }
            }

            if (component == null) return default;
            return (K)(object)component;
        }

        public bool TryGetComponent<K>(out K outComponent)
        {
            var result = GetComponent<K>();
            outComponent = result;
            return result != null;
        }

        public List<K> GetComponents<K>()
        {
            if (this.components == null)
            {
                return null;
            }

            List<K> result = ListPool.Alloc<K>();
            foreach (var items in components)
            {
                if (typeof(K).IsAssignableFrom(items.Key))
                {
                    result.Add((K)(object)items.Value);
                }
            }

            return result;
        }

        public Entity GetComponent(Type type)
        {
            if (this.components == null)
            {
                return null;
            }

            Entity component;
            if (!this.components.TryGetValue(type, out component))
            {
                return null;
            }

            return component;
        }

        public static Entity Create(System.Type type, bool isFromPool = false)
        {
            Entity component;
            if (isFromPool)
            {
                component = NFROOT.I.GetSystem<EntityPoolSystem>().Fetch(type) as Entity;
            }
            else
            {
                component = CreateOnly(type);
            }

            component.IsFromPool = isFromPool;
            component.Id = NFROOT.I.GetSystem<IDGeneratorSystem>().GenerateInstanceId();
            return component;
        }

        private static T Create<T>(bool isFromPool = false) where T : Entity, new()
        {
            Entity component;
            if (isFromPool)
            {
                component = NFROOT.I.GetSystem<EntityPoolSystem>().Fetch<T>();
            }
            else
            {
                component = Entity.CreateOnly<T>();
            }

            component.IsFromPool = isFromPool;
            component.Id = NFROOT.I.GetSystem<IDGeneratorSystem>().GenerateInstanceId();
            NFROOT.I.GetSystem<EntitySystem>().Awake(component);
            NFROOT.I.GetSystem<EntitySystem>().Start(component);
            return component as T;
        }


        public static T CreateRoot<T>(long id) where T : Entity, new()
        {
            var component = CreateOnly<T>();
            component.IsFromPool = false;
            component.IsRoot = true;
            component.Id = long.MaxValue;
            return component as T;
        }

        /// <summary>
        /// 所有的创建都走这里，方便打点测试，这个函数不走Eneity的声明周期。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateOnly<T>() where T : Entity, new()
        {
            return new T();
        }

        public static Entity CreateOnly(Type type)
        {
            return Activator.CreateInstance(type) as Entity;
        }


        public Entity AddComponent(Entity component)
        {
            Type type = component.GetType();
            if (this.components != null && this.components.ContainsKey(type))
            {
                throw new Exception($"entity already has component: {type.FullName}");
            }

            component.ComponentParent = this;
            return component;
        }

        public Entity AddComponent(Type type, bool isFromPool = false)
        {
            if (this.components != null && this.components.ContainsKey(type))
            {
                throw new Exception($"entity already has component: {type.FullName}");
            }

            Entity component = Create(type, isFromPool);
            component.ComponentParent = this;
            GetSystem<EntitySystem>().Awake(component);
            GetSystem<EntitySystem>().Start(component);
            return component;
        }

        public K AddComponent<K>(bool isFromPool = false) where K : Entity, new()
        {
            Type type = typeof(K);
            if (this.components != null && this.components.ContainsKey(type))
            {
                throw new Exception($"entity already has component: {type.FullName}");
            }

            Entity component = Create(type, isFromPool);
            component.ComponentParent = this;
            GetSystem<EntitySystem>().Awake(component);
            GetSystem<EntitySystem>().Start(component);
            return component as K;
        }

        public K AddComponent<K, P1>(P1 p1, bool isFromPool = false) where K : Entity, IAwakeSystem<P1>, new()
        {
            Type type = typeof(K);
            if (this.components != null && this.components.ContainsKey(type))
            {
                throw new Exception($"entity already has component: {type.FullName}");
            }

            Entity component = Create(type, isFromPool);
            component.ComponentParent = this;
            GetSystem<EntitySystem>().Awake(component, p1);
            GetSystem<EntitySystem>().Start(component);
            return component as K;
        }

        public K AddComponent<K, P1, P2>(P1 p1, P2 p2, bool isFromPool = false)
            where K : Entity, IAwakeSystem<P1, P2>, new()
        {
            Type type = typeof(K);
            if (this.components != null && this.components.ContainsKey(type))
            {
                throw new Exception($"entity already has component: {type.FullName}");
            }

            Entity component = Create(type, isFromPool);
            component.ComponentParent = this;
            GetSystem<EntitySystem>().Awake(component, p1, p2);
            GetSystem<EntitySystem>().Start(component);
            return component as K;
        }

        public K AddComponent<K, P1, P2, P3>(P1 p1, P2 p2, P3 p3, bool isFromPool = false)
            where K : Entity, IAwakeSystem<P1, P2, P3>, new()
        {
            Type type = typeof(K);
            if (this.components != null && this.components.ContainsKey(type))
            {
                throw new Exception($"entity already has component: {type.FullName}");
            }

            Entity component = Create(type, isFromPool);
            component.ComponentParent = this;
            GetSystem<EntitySystem>().Awake(component, p1, p2, p3);
            GetSystem<EntitySystem>().Start(component);
            return component as K;
        }

        public K AddComponent<K, P1, P2, P3, P4>(P1 p1, P2 p2, P3 p3, P4 p4, bool isFromPool = false)
            where K : Entity, IAwakeSystem<P1, P2, P3, P4>, new()
        {
            Type type = typeof(K);
            if (this.components != null && this.components.ContainsKey(type))
            {
                throw new Exception($"entity already has component: {type.FullName}");
            }

            Entity component = Create(type, isFromPool);
            component.ComponentParent = this;
            GetSystem<EntitySystem>().Awake(component, p1, p2, p3, p4);
            GetSystem<EntitySystem>().Start(component);
            return component as K;
        }

        /// <summary>
        /// 这个只允许Root使用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        public T AddChild<T>(T component) where T : Entity, new()
        {
            component.Id = GetSystem<IDGeneratorSystem>().GenerateId();
            component.Parent = this;

            GetSystem<EntitySystem>().Awake(component);
            GetSystem<EntitySystem>().Start(component);
            return component;
        }

        public Entity AddChild(Type entityType, bool isFromPool = false)
        {
            Entity child = Create(entityType, isFromPool);
            child.Id = GetSystem<IDGeneratorSystem>().GenerateId();
            child.Parent = this;

            GetSystem<EntitySystem>().Awake(child);
            GetSystem<EntitySystem>().Start(child);
            return child;
        }

        public Entity AddChild<P>(Type entityType, P p, bool isFromPool = false)
        {
            Entity child = Create(entityType, isFromPool);
            child.Id = GetSystem<IDGeneratorSystem>().GenerateId();
            child.Parent = this;

            GetSystem<EntitySystem>().Awake(child, p);
            GetSystem<EntitySystem>().Start(child);
            return child;
        }

        public Entity AddChild<P1, P2>(Type entityType, P1 p1, P2 p2, bool isFromPool = false)
        {
            Entity child = Create(entityType, isFromPool);
            child.Id = GetSystem<IDGeneratorSystem>().GenerateId();
            child.Parent = this;

            GetSystem<EntitySystem>().Awake(child, p1, p2);
            GetSystem<EntitySystem>().Start(child);
            return child;
        }

        public T AddChild<T, A>(T inT, A inA) where T : Entity, IAwakeSystem<A>, new()
        {
            inT.Id = GetSystem<IDGeneratorSystem>().GenerateId();
            inT.Parent = this;
            GetSystem<EntitySystem>().Awake(inT, inA);
            GetSystem<EntitySystem>().Start(inT);
            return inT;
        }

        public T AddChild<T>(bool isFromPool = false) where T : Entity, new()
        {
            T component = (T)Entity.Create<T>(isFromPool);
            component.Id = GetSystem<IDGeneratorSystem>().GenerateId();
            component.Parent = this;
            GetSystem<EntitySystem>().Awake(component);
            GetSystem<EntitySystem>().Start(component);
            return component;
        }


        public T AddChild<T, A>(A a, bool isFromPool = false) where T : Entity, IAwakeSystem<A>
        {
            Type type = typeof(T);
            T component = (T)Entity.Create(type, isFromPool);
            component.Id = GetSystem<IDGeneratorSystem>().GenerateId();
            component.Parent = this;

            GetSystem<EntitySystem>().Awake(component, a);
            GetSystem<EntitySystem>().Start(component);
            return component;
        }

        public T AddChild<T, A, B>(A a, B b, bool isFromPool = false) where T : Entity, IAwakeSystem<A, B>
        {
            Type type = typeof(T);
            T component = (T)Entity.Create(type, isFromPool);
            component.Id = GetSystem<IDGeneratorSystem>().GenerateId();
            component.Parent = this;

            GetSystem<EntitySystem>().Awake(component, a, b);
            GetSystem<EntitySystem>().Start(component);
            return component;
        }

        public T AddChild<T, A, B, C>(A a, B b, C c, bool isFromPool = false) where T : Entity, IAwakeSystem<A, B, C>
        {
            Type type = typeof(T);
            T component = (T)Entity.Create(type, isFromPool);
            component.Id = GetSystem<IDGeneratorSystem>().GenerateId();
            component.Parent = this;

            GetSystem<EntitySystem>().Awake(component, a, b, c);
            GetSystem<EntitySystem>().Start(component);
            return component;
        }

        public T AddChild<T, A, B, C, D>(A a, B b, C c, D d, bool isFromPool = false)
            where T : Entity, IAwakeSystem<A, B, C, D>
        {
            Type type = typeof(T);
            T component = (T)Entity.Create(type, isFromPool);
            component.Id = GetSystem<IDGeneratorSystem>().GenerateId();
            component.Parent = this;

            GetSystem<EntitySystem>().Awake(component, a, b, c, d);
            GetSystem<EntitySystem>().Start(component);
            return component;
        }

        public T AddChildWithId<T>(long id, bool isFromPool = false) where T : Entity, new()
        {
            Type type = typeof(T);
            T component = Entity.Create(type, isFromPool) as T;
            component.Parent = this;
            GetSystem<EntitySystem>().Awake(component);
            GetSystem<EntitySystem>().Start(component);
            return component;
        }

        public T AddChildWithId<T, A>(long id, A a, bool isFromPool = false) where T : Entity, IAwakeSystem<A>
        {
            Type type = typeof(T);
            T component = (T)Entity.Create(type, isFromPool);
            component.Parent = this;

            GetSystem<EntitySystem>().Awake(component, a);
            GetSystem<EntitySystem>().Start(component);
            return component;
        }

        public T AddChildWithId<T, A, B>(long id, A a, B b, bool isFromPool = false)
            where T : Entity, IAwakeSystem<A, B>
        {
            Type type = typeof(T);
            T component = (T)Entity.Create(type, isFromPool);
            component.Parent = this;

            GetSystem<EntitySystem>().Awake(component, a, b);
            GetSystem<EntitySystem>().Start(component);
            return component;
        }

        public T AddChildWithId<T, A, B, C>(long id, A a, B b, C c, bool isFromPool = false)
            where T : Entity, IAwakeSystem<A, B, C>
        {
            Type type = typeof(T);
            T component = (T)Entity.Create(type, isFromPool);
            component.Parent = this;

            GetSystem<EntitySystem>().Awake(component, a, b, c);
            GetSystem<EntitySystem>().Start(component);
            return component;
        }
    }
}