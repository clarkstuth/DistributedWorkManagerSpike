using System;
using System.Linq;
using System.ServiceModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using WorkManager.ConcurrentContainers;
using WorkManager.DataContracts;

namespace WorkManager.Tests
{
    [TestClass]
    public class IntegerWorkManagerTest : AbstractIntegerServiceAwareTestCase
    {
        IntegerWorkManager Manager { get; set; }
        OperationContext Context { get; set; }
        FakeCommunicationObject CommunicationObject { get; set; }
        IWorker WorkerCallback { get; set; }
        WorkContainer WorkContainer { get; set; }

        [TestInitialize]
        public void SetUp()
        {
            base.SetUp();
            Context = Mock.Create<OperationContext>();
            CommunicationObject = new FakeCommunicationObject();
            WorkContainer = new WorkContainer();
            WorkerCallback = Mock.Create<IWorker>();

            Mock.Arrange(() => Context.GetCallbackChannel<ICommunicationObject>()).Returns(CommunicationObject);
            Mock.Arrange(() => Context.GetCallbackChannel<IWorker>()).Returns(WorkerCallback);

            Manager = new IntegerWorkManager(WorkContainer);
            Manager.SetOperationContext(Context);
        }

        [TestCleanup]
        public void TearDown()
        {
            Manager = null;
            WorkerCallback = null;
            WorkContainer = null;
            CommunicationObject = null;
            Context = null;
            base.TearDown();
        }

        [TestMethod]
        public void StartWorkingShouldSetCallbackActiveToTrue()
        {
            Manager.StartWorking();

            Assert.IsTrue(WorkerCallback.Active);
        }

        [TestMethod]
        public void StartWorkingShouldAddWorkerToAvailableCallbacksIfNotAlreadyWorking()
        {
            WorkerCallback.IsWorking = false;
            var expectedCallbackCount = 1;

            Manager.StartWorking();

            Assert.AreEqual(expectedCallbackCount, IntegerWorkManager.AvailableCallbacks.Count);
        }

        [TestMethod]
        public void StartWorkingShouldBindToCommunicationObjectClosedEvent()
        {
            Manager.StartWorking();

            Assert.IsTrue(CommunicationObject.IsEventHandlerClosedSet());
        }

        [TestMethod]
        public void StartWorkingShouldBindToCommunicationObjectClosingEvent()
        {
            Manager.StartWorking();

            Assert.IsTrue(CommunicationObject.IsEventHandlerClosingSet());
        }

        [TestMethod]
        public void StopWorkingShouldSetCurrentWorkerToInactive()
        {
            Manager.StartWorking();
            Manager.StopWorking();

            Assert.IsFalse(WorkerCallback.Active);
        }

        [TestMethod]
        public void WorkCompleteShouldRemoveWorkFromActiveWorkCollection()
        {
            var guid = Guid.NewGuid();
            var work = 1;
            var workItem = new WorkItem(guid, work);
            WorkContainer.SetAssignedWork(WorkerCallback, guid);

            Manager.WorkComplete(workItem);

            Assert.IsFalse(WorkContainer.IsWorkAssigned(WorkerCallback));
        }

        [TestMethod]
        public void WorkCompleteShouldSetCallbackToNotWorking()
        {
            WorkerCallback.IsWorking = true;
            var guid = Guid.NewGuid();
            var value = 1;
            var workItem = new WorkItem(guid, value);
            WorkContainer.SetAssignedWork(WorkerCallback, guid);

            Manager.StartWorking();
            Manager.WorkComplete(workItem);

            Assert.IsFalse(WorkerCallback.IsWorking);
        }

        [TestMethod]
        public void CallbackClosedEventShouldSetCallbackActiveToFalse()
        {
            Manager.StartWorking();

            CommunicationObject.TriggerClosedEvent(WorkerCallback);

            Assert.IsFalse(WorkerCallback.Active);
        }

        [TestMethod]
        public void CallbackClosingEventShouldSetCallbackActiveToFalse()
        {
            Manager.StartWorking();

            CommunicationObject.TriggerClosingEvent(WorkerCallback);

            Assert.IsFalse(WorkerCallback.Active);
        }

        [TestMethod]
        public void CallbackClosedEventShouldPutWorkBackIntoAvailableWorkIfAssigned()
        {
            WorkerCallback.IsWorking = true;
            var work = 2;
            var guid = WorkContainer.AddNewWork(work);

            Manager.StartWorking();
            CommunicationObject.TriggerClosedEvent(WorkerCallback);

            Assert.AreEqual(work, WorkContainer.GetUnassignedWorkValue(guid));
        }

        [TestMethod]
        public void CallbackClosingEventShouldPutWorkBackIntoAvailableWorkIfAssigned()
        {
            WorkerCallback.IsWorking = true;

            var work = 2;
            var guid = WorkContainer.AddNewWork(work);

            Manager.StartWorking();
            CommunicationObject.TriggerClosingEvent(WorkerCallback);

            Assert.AreEqual(work, WorkContainer.GetUnassignedWorkValue(guid));
        }

        [TestMethod]
        public void StopWorkingShouldPutAssignedWorkBackIntoAvailableWorkCollection()
        {
            WorkerCallback.IsWorking = true;

            var work = 2;
            var guid = WorkContainer.AddNewWork(work);

            Manager.StartWorking();
            Manager.StopWorking();

            Assert.AreEqual(work, WorkContainer.GetUnassignedWorkValue(guid));
        }

        [TestMethod]
        public void WorkCompleteShouldAddTheCurrentCallbackBackIntoTheCollectionOfAvailableCallbacks()
        {
            WorkerCallback.IsWorking = true;

            var work = 2;
            var guid = WorkContainer.AddNewWork(work);

            var workItem = new WorkItem(guid, work);

            Manager.WorkComplete(workItem);

            Assert.IsTrue(IntegerWorkManager.AvailableCallbacks.Any((callback) => callback == WorkerCallback));
        }

    }
}
