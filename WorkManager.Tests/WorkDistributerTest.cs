using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkManager.ConcurrentContainers;
using WorkManager.DataContracts;
using WorkManager.DataContracts.Fakes;
using WorkManager.ServiceHosting;
using WorkManager.ServiceHosting.Fakes;

namespace WorkManager.Tests
{
    [TestClass]
    public class WorkDistributerTest : AbstractIntegerServiceAwareTestCase
    {
        IWorkDistributerServiceHost Host { get; set; }
        WorkContainer WorkContainer { get; set; }
        CallbackContainer CallbackContainer { get; set; }
        WorkDistributer Distributer { get; set; }

        [TestInitialize]
        public void SetUp()
        {
            base.SetUp();

            WorkContainer = new WorkContainer();
            CallbackContainer = new CallbackContainer();

            var uris = new[] {new Uri("http://localhost:443")};

            Host = new StubIWorkDistributerServiceHost()
            {
                
            };

            InitializeDistributer();
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

        private void InitializeDistributer()
        {
            Distributer = new WorkDistributer(Host, WorkContainer, CallbackContainer);
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

            Distributer.DistributionCancelled += (sender, e) =>
            {
                called = true;
            };

            Distributer.StartDistrubutingWork();
            Distributer.StopDistributingWork();

            Thread.Sleep(TimeSpan.FromSeconds(1));

            Assert.IsTrue(called);
        }

        [TestMethod]
        public void StartDistributingWorkWithOneWorkerAndOneItemShouldPassItemToWorker()
        {
            var items = new List<int> {1};
            var worker = new DataContracts.Fakes.StubIWorker();
            CallbackContainer.AddAvailableCallback(worker);

            var called = false;
            worker.DoWorkWorkItem = (item) =>
            {
                if (item.WorkToDo == 1)
                {
                    called = true;
                }
            };

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
            var worker = new DataContracts.Fakes.StubIWorker();
            CallbackContainer.AddAvailableCallback(worker);

            worker.DoWorkWorkItem = (item) => { };

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

            var worker1 = new DataContracts.Fakes.StubIWorker();
            var worker2 = new DataContracts.Fakes.StubIWorker();
            var worker3 = new DataContracts.Fakes.StubIWorker();

            bool oneSeen = false, twoSeen = false, threeSeen = false;
            var sequence = new List<int>();

            var action = new FakesDelegates.Action<WorkItem>((item) =>
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

            worker1.DoWorkWorkItem = action;
            worker2.DoWorkWorkItem = action;
            worker3.DoWorkWorkItem = action;

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
            var called = false;

            Host.Opened += (sender, args) =>
            {
                called = true;
            };

            Distributer.StartDistrubutingWork();

            Assert.IsTrue(called);
        }

        [TestMethod]
        public void StopDistributingShouldSetServiceHostToClosed()
        {
            var called = false;

            Host.Closed += (sender, args) =>
            {
                called = true;
            };
            
            Distributer.StartDistrubutingWork();
            Distributer.StopDistributingWork();

            Assert.IsTrue(called);
        }

        [TestMethod]
        public void IfCallbackDoWorkThrowsCommunicationObjectAbortedExceptionShouldTryAgain()
        {
            var items = new List<int> {7};
            var worker1 = new StubIWorker();
            var worker2 = new StubIWorker();

            Distributer.AddWork(items);
            CallbackContainer.AddAvailableCallback(worker1);
            CallbackContainer.AddAvailableCallback(worker2);

            var called = false;

            worker1.DoWorkWorkItem = (workItem) => { throw new CommunicationObjectAbortedException(); };
            worker2.DoWorkWorkItem = (workItem) => { called = true; };


            Distributer.StartDistrubutingWork();
            
            Thread.Sleep(TimeSpan.FromSeconds(2));

            Distributer.StopDistributingWork();

            Assert.IsTrue(called);
        }

    }
}
