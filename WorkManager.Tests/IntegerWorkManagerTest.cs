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
        CallbackContainer CallbackContainer { get; set; }

        [TestInitialize]
        public void SetUp()
        {
            base.SetUp();
            Context = Mock.Create<OperationContext>();
            CommunicationObject = new FakeCommunicationObject();

            WorkerCallback = Mock.Create<IWorker>();
            Mock.Arrange(() => Context.GetCallbackChannel<ICommunicationObject>()).Returns(CommunicationObject);
            Mock.Arrange(() => Context.GetCallbackChannel<IWorker>()).Returns(WorkerCallback);

            WorkContainer = new WorkContainer();
            CallbackContainer = new CallbackContainer();

            Manager = new IntegerWorkManager(WorkContainer, CallbackContainer);
            Manager.SetOperationContext(Context);
        }

        [TestCleanup]
        public void TearDown()
        {
            Manager = null;
            WorkerCallback = null;
            WorkContainer = null;
            CallbackContainer = null;
            CommunicationObject = null;
            Context = null;
            base.TearDown();
        }

        [TestMethod]
        public void StartWorkingShouldAddWorkerToAvailableCallbacksIfNotAlreadyWorking()
        {
            var expectedCallbackCount = 1;

            Manager.StartWorking();

            Assert.AreEqual(expectedCallbackCount, CallbackContainer.GetNumberAvailableCallbacks());
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
        public void CallbackClosedEventShouldPutWorkBackIntoAvailableWorkIfAssigned()
        {
            var work = 2;
            var guid = WorkContainer.AddNewWork(work);

            Manager.StartWorking();
            CommunicationObject.TriggerClosedEvent(WorkerCallback);

            Assert.AreEqual(work, WorkContainer.GetUnassignedWorkValue(guid));
        }

        [TestMethod]
        public void CallbackClosingEventShouldPutWorkBackIntoAvailableWorkIfAssigned()
        {
            var work = 2;
            var guid = WorkContainer.AddNewWork(work);

            Manager.StartWorking();
            CommunicationObject.TriggerClosingEvent(WorkerCallback);

            Assert.AreEqual(work, WorkContainer.GetUnassignedWorkValue(guid));
        }

        [TestMethod]
        public void StopWorkingShouldPutAssignedWorkBackIntoAvailableWorkCollection()
        {
            var work = 2;
            var guid = WorkContainer.AddNewWork(work);

            Manager.StartWorking();
            Manager.StopWorking();

            Assert.AreEqual(work, WorkContainer.GetUnassignedWorkValue(guid));
        }

        [TestMethod]
        public void WorkCompleteShouldAddTheCurrentCallbackBackIntoTheCollectionOfAvailableCallbacks()
        {
            var work = 2;
            var guid = WorkContainer.AddNewWork(work);

            var workItem = new WorkItem(guid, work);

            Manager.WorkComplete(workItem);

            Assert.IsTrue(CallbackContainer.IsCallbackAvailable(WorkerCallback));
        }

    }
}
