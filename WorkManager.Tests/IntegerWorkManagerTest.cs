using System;
using System.ServiceModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using WorkManager.DataContracts;
using WorkManager.Exceptions;

namespace WorkManager.Tests
{
    [TestClass]
    public class IntegerWorkManagerTest
    {
        IntegerWorkManager Manager { get; set; }
        OperationContext Context { get; set; }
        FakeCommunicationObject CommunicationObject { get; set; }
        IWorker WorkerCallback { get; set; }

        [TestInitialize]
        public void SetUp()
        {
            Context = Mock.Create<OperationContext>();
            CommunicationObject = new FakeCommunicationObject();
            WorkerCallback = Mock.Create<IWorker>();

            Mock.Arrange(() => Context.GetCallbackChannel<ICommunicationObject>()).Returns(CommunicationObject);
            Mock.Arrange(() => Context.GetCallbackChannel<IWorker>()).Returns(WorkerCallback);

            Manager = new IntegerWorkManager();
            Manager.SetOperationContext(Context);
        }

        [TestCleanup]
        public void TearDown()
        {
            Manager = null;
            WorkerCallback = null;
            CommunicationObject = null;
            Context = null;

            EmptyAvailableCallbacksObject();
            IntegerWorkManager.AvailableWork.Clear();
            IntegerWorkManager.ActiveWork.Clear();
            Mock.Reset();
        }

        private void EmptyAvailableCallbacksObject()
        {
            while (IntegerWorkManager.AvailableCallbacks.Count > 0)
            {
                IntegerWorkManager.AvailableCallbacks.Take();
            }
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
            IntegerWorkManager.ActiveWork.TryAdd(guid, WorkerCallback);

            Manager.WorkComplete(workItem);

            Assert.IsFalse(IntegerWorkManager.ActiveWork.ContainsKey(guid));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidWorkItemException))]
        public void WorkCompleteShouldRaiseAnExceptionIfTheWorkGuidDoesNotExist()
        {
            var guid = Guid.NewGuid();
            var expectedError = String.Format("Provided Work GUID does not exist: {0}", guid);
            var workItem = new WorkItem(guid, 1);

            try
            {
                Manager.WorkComplete(workItem);
            }
            catch (InvalidOperationException e)
            {
                Assert.AreEqual(expectedError, e.Message);
                throw;
            }
        }

        [TestMethod]
        public void WorkCompleteShouldSetCallbackToNotWorking()
        {
            WorkerCallback.IsWorking = true;
            var guid = Guid.NewGuid();
            var value = 1;
            var workItem = new WorkItem(guid, value);
            IntegerWorkManager.ActiveWork.TryAdd(guid, WorkerCallback);

            Manager.StartWorking();
            Manager.WorkComplete(workItem);

            Assert.IsFalse(WorkerCallback.IsWorking);
        }

        [TestMethod]
        [ExpectedException(typeof(WorkNotAssignedException))]
        public void WorkCompleteShouldGenerateAnExceptionIfWorkGuidIsNotAssignedToExistingWorker()
        {
            var guid = Guid.NewGuid();
            var value = 1;
            var workItem = new WorkItem(guid, value);
            var differentAssignedCallback = Mock.Create<IWorker>();
            IntegerWorkManager.ActiveWork.TryAdd(guid, differentAssignedCallback);
            var expectedError = String.Format("Work Guid: '{0}' not assigned to reporting callback.", guid);

            try
            {
                Manager.WorkComplete(workItem);
            }
            catch (WorkNotAssignedException e)
            {
                Assert.AreEqual(expectedError, e.Message);
                throw;
            }
        }

        [TestMethod]
        public void CallbackClosedEventShouldSetCallbackActiveToFalse()
        {
            Manager.StartWorking();

            CommunicationObject.TriggerClosedEvent(WorkerCallback);

            Assert.IsFalse(WorkerCallback.Active);
        }

        [TestMethod]
        public void CallbackClosedEventShouldPutWorkBackIntoAvailableWorkIfWorkWasActive()
        {
            Manager.StartWorking();
            var guid = Guid.NewGuid();
            var work = 2;
            var workItem = new WorkItem(guid, work);
            IntegerWorkManager.ActiveWork.TryAdd(guid, WorkerCallback);

            CommunicationObject.TriggerClosedEvent(WorkerCallback);

        }

    }
}
