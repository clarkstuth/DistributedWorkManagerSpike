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
            var guid = Guid.NewGuid().ToString();
            var work = 1;
            var workItem = new WorkItem(guid, work);
            IntegerWorkManager.ActiveWork.TryAdd(guid, work);

            Manager.WorkComplete(workItem);

            Assert.IsFalse(IntegerWorkManager.ActiveWork.ContainsKey(guid));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidWorkItemException))]
        public void WorkCompleteShouldRaiseAnExceptionIfTheWorkGuidDoesNotExist()
        {
            var guid = Guid.NewGuid().ToString();
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
            var guid = Guid.NewGuid().ToString();
            var value = 1;
            var workItem = new WorkItem(guid, value);
            IntegerWorkManager.ActiveWork.TryAdd(guid, value);

            Manager.StartWorking();
            Manager.WorkComplete(workItem);

            Assert.IsFalse(WorkerCallback.IsWorking);
        }

    }
}
