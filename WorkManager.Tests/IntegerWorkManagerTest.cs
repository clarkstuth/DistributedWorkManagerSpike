using System.ServiceModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WorkManager.Tests
{
    [TestClass]
    public class IntegerWorkManagerTest
    {
        IntegerWorkManager Manager { get; set; }
        OperationContext Context { get; set; }


        [TestInitialize]
        public void SetUp()
        {
            Context = Mock.
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
