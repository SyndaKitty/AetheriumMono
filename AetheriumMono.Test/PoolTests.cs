using System;
using System.Collections.Generic;
using AetheriumMono.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AetheriumMono.Test
{
    [TestClass]
    public class PoolTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            pool = new Pool<GameObject>();
        }

        Pool<GameObject> pool;

        [TestMethod]
        public void FirstEntity()
        {
            string testData = "TestData";
            EntityRef<GameObject> testRef = pool.Create(new GameObject {Data = testData});

            Assert.IsTrue(testRef.Get(out var entity));
            Assert.AreEqual(testData, entity.Data);
        }

        [TestMethod]
        public void ObjectRemoved_BeforeFrame()
        {
            string testData = "TestData";
            EntityRef<GameObject> testRef = pool.Create(new GameObject {Data = testData});

            pool.Remove(testRef);

            Assert.IsTrue(testRef.Get(out var entity));
            Assert.AreEqual(testData, entity.Data);
        }

        [TestMethod]
        public void ObjectRemoved_AfterFrame()
        {
            string testData = "TestData";
            EntityRef<GameObject> testRef = pool.Create(new GameObject {Data = testData});

            pool.Remove(testRef);
            pool.EndOfFrame();

            Assert.IsFalse(testRef.Get(out var entity));
            Assert.AreEqual(null, entity);
        }

        [TestMethod]
        public void ObjectRemoved_Reassign()
        {
            string testData = "TestData1";
            EntityRef<GameObject> testRef = pool.Create(new GameObject {Data = testData});

            pool.Remove(testRef);
            pool.EndOfFrame();

            Assert.IsFalse(testRef.Get(out var entity));
            Assert.AreEqual(null, entity);

            string testData2 = "testData2";
            EntityRef<GameObject> testRef2 = pool.Create(new GameObject {Data = testData2});

            Assert.IsTrue(testRef2.Get(out entity));
            Assert.AreEqual(testData2, entity.Data);
        }

        [TestMethod]
        public void IndexReused()
        {
            string testData = "TestData";
            EntityRef<GameObject> testRef = pool.Create(new GameObject {Data = testData});

            int firstIndex = testRef.Index;

            pool.Remove(testRef);
            pool.EndOfFrame();

            testRef = pool.Create(new GameObject {Data = testData});
            Assert.AreEqual(firstIndex, testRef.Index);
        }

        [TestMethod]
        public void Enumerate()
        {
            List<string> testData = new List<string> {"testData1", "testData2", "testData3", "testData4"};

            List<EntityRef<GameObject>> refs = new List<EntityRef<GameObject>>();

            for (int i = 0; i < testData.Count; i++)
            {
                string data = testData[i];
                refs.Add(pool.Create(new GameObject {Data = data}));
            }

            int removedIndex = 1;
            pool.Remove(refs[removedIndex]);
            pool.EndOfFrame();
            testData.RemoveAt(removedIndex);

            int index = 0;
            foreach (var gameObject in pool)
            {
                Assert.AreEqual(testData[index], gameObject.Data);
                index++;
            }
        }

        [TestMethod]
        public void RegisterRemoveEvent()
        {
            string testData1 = "TestData1";
            string testData2 = "TestData2";

            var go = new GameObject {Data = testData1};
            EntityRef<GameObject> testRef = pool.Create(go);

            string test = testData1;

            testRef.RegisterRemovedCallback((go) => test = testData2);

            pool.Remove(testRef);
            
            Assert.AreEqual(testData2, test);
        }
    }

    public class GameObject
    {
        public string Data { get; set; }
    }
}
