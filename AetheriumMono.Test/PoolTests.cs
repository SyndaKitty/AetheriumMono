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
        
    }

    public class GameObject
    {
        public string Data { get; set; }
    }
}
