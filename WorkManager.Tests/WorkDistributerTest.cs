using System.Collections.Generic;
using System.ServiceModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace WorkManager.Tests
{
    [TestClass]
    public class WorkDistributerTest : AbstractIntegerServiceAwareTestCase
    {
        ServiceHost Host { get; set; }
        WorkDistributer Distributer { get; set; }

        [TestInitialize]
        public void SetUp()
        {
            base.SetUp();
            Host = Mock.Create<ServiceHost>();
            
            Distributer = new WorkDistributer(Host);
        }

        [TestCleanup]
        public void TearDown()
        {
            Distributer = null;
            Host = null;
            base.TearDown();
        }

        [TestMethod]
        public void AddWorkWith1ShouldAdd1ToWorkPool()
        {
            var workToDo = new List<int> {1};

            Distributer.AddWork(workToDo);

            Assert.IsTrue(IntegerWorkManager.AllWork.ContainsValue(workToDo[0]));
        }

        [TestMethod]
        public void AddWorkWith1And2ShouldAdd1And2ToWorkPool()
        {
            var workToDo = new List<int> {1, 2};

            Distributer.AddWork(workToDo);

            Assert.IsTrue(IntegerWorkManager.AllWork.ContainsValue(workToDo[0]));
            Assert.IsTrue(IntegerWorkManager.AllWork.ContainsValue(workToDo[1]));
        }

        [TestMethod]
        public void AddWorkWith1And2ShouldAdd1And2ToUnassignedWorkPool()
        {
            var workToDo = new List<int> { 1, 2 };

            Distributer.AddWork(workToDo);

            Assert.IsTrue(IntegerWorkManager.UnassignedWork.ContainsValue(workToDo[0]));
            Assert.IsTrue(IntegerWorkManager.UnassignedWork.ContainsValue(workToDo[1]));
        }

        [TestMethod]
        public void StartDistributingShouldSetIsDistributingToTrue()
        {
            Distributer.StartDistrubutingWork();

            Assert.IsTrue(Distributer.IsDistributingWork);
        }

        [TestMethod]
        public void StopDistributingShouldSetIsDistributingToFalse()
        {
            Distributer.StartDistrubutingWork();
            Distributer.StopDistributingWork();

            Assert.IsFalse(Distributer.IsDistributingWork);
        }

    }
}
