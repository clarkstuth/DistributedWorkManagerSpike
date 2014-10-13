using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using WorkManager.DataContracts;

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

        [TestMethod]
        public void StopDistributingShouldInvokeOnDistributionCancelledEvent()
        {
            var called = false;

            var handler = Mock.Create<DistributionCancelledHandler>();
            Mock.Arrange(() => handler.Invoke(Arg.IsAny<object>(), Arg.IsAny<EventArgs>())).DoInstead((object obj, EventArgs eventArgs) =>
            {
                called = true;
            });
            Distributer.DistributionCancelled += handler;

            Distributer.StartDistrubutingWork();
            Distributer.StopDistributingWork();

            Thread.Sleep(TimeSpan.FromSeconds(1));

            Assert.IsTrue(called);
        }

        [TestMethod]
        public void StartDistributingWorkWithOneWorkerAndOneItemShouldPassItemToWorker()
        {
            var items = new List<int> {1};
            var worker = Mock.Create<IWorker>();
            worker.Active = true;
            worker.IsWorking = false;
            IntegerWorkManager.AvailableCallbacks.Add(worker);

            var called = false;
            Mock.Arrange(() => worker.DoWork(Arg.IsAny<WorkItem>())).DoInstead((WorkItem item) =>
            {
                if (item.WorkToDo == 1)
                {
                    called = true;
                }
            });

            Distributer.AddWork(items);
            Distributer.StartDistrubutingWork();

            Thread.Sleep(TimeSpan.FromSeconds(1));

            Distributer.StopDistributingWork();

            Assert.IsTrue(called);
        }

        [TestMethod]
        public void StartDistributingWorkWithValidWorkToBeDoneShouldRemoveWorkerFromListOfAvailableWorkers()
        {
            var items = new List<int> {1};
            var worker = Mock.Create<IWorker>();
            worker.Active = true;
            worker.IsWorking = false;
            IntegerWorkManager.AvailableCallbacks.Add(worker);

            Mock.Arrange(() => worker.DoWork(Arg.IsAny<WorkItem>())).DoNothing();

            Distributer.AddWork(items);
            Distributer.StartDistrubutingWork();

            Thread.Sleep(TimeSpan.FromSeconds(1));

            Distributer.StartDistrubutingWork();

            Assert.IsFalse(IntegerWorkManager.AvailableCallbacks.Any(callback => callback == worker));
        }

    }
}
