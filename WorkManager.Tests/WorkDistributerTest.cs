using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using WorkManager.ConcurrentContainers;
using WorkManager.DataContracts;

namespace WorkManager.Tests
{
    [TestClass]
    public class WorkDistributerTest : AbstractIntegerServiceAwareTestCase
    {
        ServiceHost Host { get; set; }
        WorkContainer WorkContainer { get; set; }
        CallbackContainer CallbackContainer { get; set; }
        WorkDistributer Distributer { get; set; }

        [TestInitialize]
        public void SetUp()
        {
            base.SetUp();
            Host = Mock.Create<ServiceHost>();
            WorkContainer = new WorkContainer();
            CallbackContainer = new CallbackContainer();
            Distributer = new WorkDistributer(Host, WorkContainer, CallbackContainer);
        }

        [TestCleanup]
        public void TearDown()
        {
            Distributer = null;
            CallbackContainer = null;
            WorkContainer = null;
            Host = null;
            base.TearDown();
        }

        [TestMethod]
        public void AddWorkWith1ShouldAdd1ToWorkPool()
        {
            var workToDo = new List<int> {1};

            Distributer.AddWork(workToDo);

            Assert.IsTrue(WorkContainer.WorkValueExists(workToDo[0]));
        }

        [TestMethod]
        public void AddWorkWith1And2ShouldAdd1And2ToWorkPool()
        {
            var workToDo = new List<int> {1, 2};

            Distributer.AddWork(workToDo);

            Assert.IsTrue(WorkContainer.WorkValueExists(workToDo[0]));
            Assert.IsTrue(WorkContainer.WorkValueExists(workToDo[1]));
        }

        [TestMethod]
        public void AddWorkWith1And2ShouldAdd1And2ToUnassignedWorkPool()
        {
            var workToDo = new List<int> { 1, 2 };

            Distributer.AddWork(workToDo);

            Assert.IsTrue(WorkContainer.UnassignedWorkValueExists(workToDo[0]));
            Assert.IsTrue(WorkContainer.UnassignedWorkValueExists(workToDo[1]));
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
            CallbackContainer.AddAvailableCallback(worker);

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
            CallbackContainer.AddAvailableCallback(worker);

            Mock.Arrange(() => worker.DoWork(Arg.IsAny<WorkItem>())).DoNothing();

            Distributer.AddWork(items);
            Distributer.StartDistrubutingWork();

            Thread.Sleep(TimeSpan.FromSeconds(1));

            Distributer.StartDistrubutingWork();

            Assert.IsFalse(CallbackContainer.IsCallbackAvailable(worker));
        }

        //TODO: MAKE ME NOT HATE MYSELF SO MUCH FOR THESE FEW TESTS
        [TestMethod]
        public void StartDistributingWorkWithMultipleItemsShouldDistributeToAllAvailableWorkersInCorrectOrder()
        {
            //Arrange
            var items = new List<int> {1, 2, 3};

            var worker1 = Mock.Create<IWorker>();
            var worker2 = Mock.Create<IWorker>();
            var worker3 = Mock.Create<IWorker>();

            bool oneSeen = false, twoSeen = false, threeSeen = false;
            var sequence = new List<int>();

            var action = new Action<WorkItem>((item) =>
            {
                switch (item.WorkToDo)
                {
                    case 1:
                        oneSeen = true;
                        break;
                    case 2:
                        twoSeen = true;
                        break;
                    case 3:
                        threeSeen = true;
                        break;
                }
                sequence.Add(item.WorkToDo);
            });

            CallbackContainer.AddAvailableCallback(worker1);
            CallbackContainer.AddAvailableCallback(worker2);
            CallbackContainer.AddAvailableCallback(worker3);

            Mock.Arrange(() => worker1.DoWork(Arg.IsAny<WorkItem>())).DoInstead(action);
            Mock.Arrange(() => worker2.DoWork(Arg.IsAny<WorkItem>())).DoInstead(action);
            Mock.Arrange(() => worker3.DoWork(Arg.IsAny<WorkItem>())).DoInstead(action);

            //Act
            Distributer.AddWork(items);
            Distributer.StartDistrubutingWork();

            Thread.Sleep(TimeSpan.FromSeconds(3));

            Distributer.StopDistributingWork();

            //Assert
            Assert.AreEqual(3, sequence[0]);
            Assert.AreEqual(2, sequence[1]);
            Assert.AreEqual(1, sequence[2]);
            Assert.IsTrue(oneSeen);
            Assert.IsTrue(twoSeen);
            Assert.IsTrue(threeSeen);
        }

        [TestMethod]
        public void StartDistributingShouldSetServiceHostToOpen()
        {
            Mock.Arrange(() => Host.Open()).DoNothing().MustBeCalled();

            Distributer.StartDistrubutingWork();

            Mock.Assert(Host);
        }

        [TestMethod]
        public void StopDistributingShouldSetServiceHostToClosed()
        {
            Mock.Arrange(() => Host.Close()).DoNothing().MustBeCalled();

            Distributer.StartDistrubutingWork();
            Distributer.StopDistributingWork();

            Mock.Assert(Host);
        }

        [TestMethod]
        public void IfCallbackDoWorkThrowsCommunicationObjectAbortedExceptionShouldTryAgain()
        {
            var items = new List<int> {7};
            var worker1 = Mock.Create<IWorker>();
            var worker2 = Mock.Create<IWorker>();

            items.ForEach(i => WorkContainer.AddNewWork(i));
            CallbackContainer.AddAvailableCallback(worker1);
            CallbackContainer.AddAvailableCallback(worker2);

            var called = false;

            Mock.Arrange(() => worker1.DoWork(Arg.IsAny<WorkItem>())).Throws(new CommunicationObjectAbortedException()).MustBeCalled();
            Mock.Arrange(() => worker2.DoWork(Arg.IsAny<WorkItem>())).DoInstead((WorkItem item) => called = true);

            Distributer.StartDistrubutingWork();
            
            Thread.Sleep(TimeSpan.FromSeconds(2));

            Distributer.StopDistributingWork();

            Mock.Assert(worker1);
            Assert.IsTrue(called);
        }

    }
}
