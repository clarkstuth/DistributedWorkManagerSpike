using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WorkManager.Tests
{
    [TestClass]
    public class IntegerWorkManagerTest
    {
        IntegerWorkManager Manager { get; set; }

        [TestInitialize]
        public void SetUp()
        {
            Manager = new IntegerWorkManager();
        }

        [TestCleanup]
        public void TearDown()
        {
            Manager = null;
        }

        [TestMethod]
        public void StartWorkingShouldAddConnectedClientToPoolOfAvailableClients()
        {
            var expectedCount = 1;

            Manager.StartWorking();

            Assert.AreEqual(expectedCount, Manager.GetWorkers());
        }



    }
}
