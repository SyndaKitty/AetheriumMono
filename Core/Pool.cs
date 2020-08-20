using System;
using System.Collections;
using System.Collections.Generic;

namespace AetheriumMono.Core
{
    public class Pool<T> : IEnumerable<T> where T : class
    {
        List<T> pooledObjects;
        Stack<int> removedPositions;
        Stack<int> positionsToRemove;

        Dictionary<EntityRef<T>, EntityRemovedEventCallback<T>> callbacks;

        public Pool() : this(256)
        {}

        public Pool(int capacity)
        {
            pooledObjects = new List<T>(capacity);
            removedPositions = new Stack<int>(64);
            positionsToRemove = new Stack<int>(64);
            callbacks = new Dictionary<EntityRef<T>, EntityRemovedEventCallback<T>>();
        }

        public bool Get(int index, out T outEntity)
        {
            outEntity = pooledObjects[index];
            return outEntity != null;
        }

        public EntityRef<T> Create(T t)
        {
            int index;
            if (removedPositions.Count > 0)
            {
                index = removedPositions.Pop();
                pooledObjects[index] = t;
            }
            else
            {
                index = pooledObjects.Count;
                pooledObjects.Add(t);
            }
            return new EntityRef<T>(index, this);
        }

        public bool Get(EntityRef<T> entityRef, out T outEntity) => Get(entityRef.Index, out outEntity);

        public void Remove(EntityRef<T> entityRef)
        {
            if (entityRef.Get(out var entity))
            {
                positionsToRemove.Push(entityRef.Index);
                if (callbacks.TryGetValue(entityRef, out var callback))
                {
                    callback.OnEntityRemoved(entity);
                }
                callbacks.Remove(entityRef);
                return;
            }
            throw new InvalidOperationException("Deleting an entity that was already deleted");
        }

        public void RegisterRemovedEvent(EntityRef<T> entityRef, params EntityRemoved<T>[] funcs)
        {
            if (!entityRef.Get(out _)) throw new InvalidOperationException("EntityRef does not exist");

            EntityRemovedEventCallback<T> callback;
            if (!callbacks.TryGetValue(entityRef, out callback))
            {
                callback = new EntityRemovedEventCallback<T>();
                callbacks.Add(entityRef, callback);
            }

            foreach (var func in funcs)
            {
                callback.EntityRemoved += func;
            }
        }

        public void EndOfFrame()
        {
            while (positionsToRemove.Count > 0)
            {
                int index = positionsToRemove.Pop();
                pooledObjects[index] = null;
                removedPositions.Push(index);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        class Enumerator<T> : IEnumerator<T> where T : class
        {
            int index = -1;
            List<T> pooledObjects;

            public Enumerator(Pool<T> pool)
            {
                pooledObjects = pool.pooledObjects;
            }

            public void Dispose()
            {
                // TODO
                //foreach ()
            }

            public bool MoveNext()
            {
                index++;
                if (index >= pooledObjects.Count)
                {
                    return false;
                }

                while (pooledObjects[index] == null)
                {
                    index++;
                    if (index >= pooledObjects.Count)
                    {
                        return false;
                    }
                }
                return true;
            }

            public void Reset()
            {
                index = 0;
            }

            public T Current => pooledObjects[index];

            object IEnumerator.Current => Current;
        }
    }

    public class EntityRemovedEventCallback<T> where T : class
    {
        public event EntityRemoved<T> EntityRemoved;

        public virtual void OnEntityRemoved(T entity)
        {
            EntityRemoved?.Invoke(entity);
        }
    }

    public delegate void EntityRemoved<T>(T entity) where T : class;

    public struct EntityRef<T> : IEquatable<EntityRef<T>> where T : class
    {
        public EntityRef(int index, Pool<T> parentPool)
        {
            this.ParentPool = parentPool;
            Index = index;
        }

        public readonly Pool<T> ParentPool;
        public int Index { get; }

        public bool Get(out T entity)
        {
            return ParentPool.Get(Index, out entity);
        }

        public void Remove()
        {
            ParentPool.Remove(this);
        }

        //public RegisterOnRemoveCallback(params EntityRemoved<T>[] funcs)
        //{
        //    parentPool.RegisterRemovedEvent(this, funcs);
        //}

        public bool Equals(EntityRef<T> other)
        {
            return Equals(ParentPool, other.ParentPool) && Index == other.Index;
        }

        public override bool Equals(object obj)
        {
            return obj is EntityRef<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((ParentPool != null ? ParentPool.GetHashCode() : 0) * 397) ^ Index;
            }
        }
    }
}
