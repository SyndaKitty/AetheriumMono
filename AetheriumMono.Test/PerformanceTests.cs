using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Enumeration;
using AetheriumMono.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AetheriumMono.Test
{
    [TestClass]
    public class PerformanceTests
    {
        [TestMethod]
        public void PoolAddPerformance()
        {
            int number = 1_000_000;
            List<TestGameObject> gameObjects = new List<TestGameObject>(number);
            List<EntityRef<TestGameObject>> gameObjectRef = new List<EntityRef<TestGameObject>>(number);
            Pool<TestGameObject> pool = new Pool<TestGameObject>(number);

            for (int i = 0; i < number; i++)
            {
                gameObjects.Add(new TestGameObject{Datum = (int)Mathf.Cos(i)});
            }

            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < number; i++)
            {
                gameObjectRef.Add(pool.Create(gameObjects[i]));
            }
            sw.Stop();
            Assert.AreEqual(0, sw.ElapsedMilliseconds);
        }

        [TestMethod]
        public void PoolRemovePerformance()
        {
            int number = 1_000_000;
            List<TestGameObject> gameObjects = new List<TestGameObject>(number);
            List<EntityRef<TestGameObject>> gameObjectRef = new List<EntityRef<TestGameObject>>(number);
            Pool<TestGameObject> pool = new Pool<TestGameObject>(number);
            
            for (int i = 0; i < number; i++)
            {
                gameObjects.Add(new TestGameObject{Datum = (int)Mathf.Cos(i)});
            }

            for (int i = 0; i < number; i++)
            {
                gameObjectRef.Add(pool.Create(gameObjects[i]));
            }

            // Results in about 40% being removed
            List<int> removeIndices = new List<int>();
            for (int i = 0; i < number;)
            {
                i += (int)Mathf.Random(1, 5);
                if (i < number) removeIndices.Add(i);
            }

            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < removeIndices.Count; i++)
            {
                pool.Remove(gameObjectRef[removeIndices[i]]);
            }
            sw.Stop();
            //Assert.AreEqual(0, removeIndices.Count);
            Assert.AreEqual(0, sw.ElapsedMilliseconds);
        }

        [TestMethod]
        public void PoolAddRemoveAddPerformance()
        {
            int number = 1_000_000;
            List<TestGameObject> gameObjects = new List<TestGameObject>(number);
            List<EntityRef<TestGameObject>> gameObjectRef = new List<EntityRef<TestGameObject>>(number);
            Pool<TestGameObject> pool = new Pool<TestGameObject>(number);
            
            for (int i = 0; i < number; i++)
            {
                gameObjects.Add(new TestGameObject{Datum = (int)Mathf.Cos(i)});
            }

            for (int i = 0; i < number; i++)
            {
                gameObjectRef.Add(pool.Create(gameObjects[i]));
            }

            // Results in about 40% being removed
            List<int> removeIndices = new List<int>();
            for (int i = 0; i < number;)
            {
                i += (int)Mathf.Random(1, 5);
                if (i < number) removeIndices.Add(i);
            }

            for (int i = 0; i < removeIndices.Count; i++)
            {
                pool.Remove(gameObjectRef[removeIndices[i]]);
            }

            Stopwatch sw = Stopwatch.StartNew();
            for (int i = removeIndices.Count - 1; i >= 0; i--)
            {
                pool.Create(gameObjects[removeIndices[i]]);
            }
            sw.Stop();

            Assert.AreEqual(0, sw.ElapsedMilliseconds);
        }

        [TestMethod]
        public void SparseEnumeratePerformance()
        {
            
            int number = 1_000_000;
            List<TestGameObject> gameObjects = new List<TestGameObject>(number);
            List<EntityRef<TestGameObject>> gameObjectRef = new List<EntityRef<TestGameObject>>(number);
            Pool<TestGameObject> pool = new Pool<TestGameObject>(number);
        
            for (int i = 0; i < number; i++)
            {
                gameObjects.Add(new TestGameObject{Datum = (int)Mathf.Cos(i)});
            }

            for (int i = 0; i < number; i++)
            {
                gameObjectRef.Add(pool.Create(gameObjects[i]));
            }

            // Results in about 40% being removed
            List<int> removeIndices = new List<int>();
            for (int i = 0; i < number;)
            {
                i += (int)Mathf.Random(1, 5);
                if (i < number) removeIndices.Add(i);
            }

            List<int> data = new List<int>();
            for (int i = 0; i < number; i++)
            {
                data.Add((int) Mathf.Cos(2 * i));
            }

            Stopwatch sw = Stopwatch.StartNew();
            int j = 0;
            foreach (var go in pool)
            {
                go.Datum = data[j++];
            }
            sw.Stop();

            Assert.AreEqual(0, sw.ElapsedMilliseconds);
        }
    }

    public class TestGameObject
    {
        public int Datum { get; set; }

    }
}
