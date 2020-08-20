using System.Collections.Generic;

namespace AetheriumMono.Core
{
    public class Pool<T> where T : class
    {
        List<T> pooledObjects;
        Stack<int> removedPositions;
        Stack<int> positionsToRemove;

        public Pool() : this(256)
        {}

        public Pool(int capacity)
        {
            pooledObjects = new List<T>(capacity);
            removedPositions = new Stack<int>(64);
            positionsToRemove = new Stack<int>(64);
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
            positionsToRemove.Push(entityRef.Index);
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
    }


    public struct EntityRef<T> where T : class
    {
        public EntityRef(int index, Pool<T> parentPool)
        {
            this.parentPool = parentPool;
            Index = index;
        }

        Pool<T> parentPool { get; }
        public int Index { get; }

        public bool Get(out T entity)
        {
            return parentPool.Get(Index, out entity);
        }
    }
}
