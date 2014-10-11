using System.ServiceModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using WorkManager.DataContracts;

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

            Mock.Reset();
        }

        [TestMethod]
        public void StartWorkingShouldAddConnectedClientToPoolOfAvailableClients()
        {
            Manager.StartWorking();

            CollectionAssert.Contains(Manager.GetWorkers(), WorkerCallback);
        }

        [TestMethod]
        public void StartWorkingShouldBindInternalCallbacksToChannelClosedEvent()
        {
            Manager.StartWorking();

            Assert.IsTrue(CommunicationObject.IsEventHandlerClosedSet());
        }

        [TestMethod]
        public void StartWorkingShouldBindInternalCallbacksToChannelClosingEvent()
        {
            Manager.StartWorking();

            Assert.IsTrue(CommunicationObject.IsEventHandlerClosingSet());
        }

        [TestMethod]
        public void StopWorkingShouldRemoveCurrentWorkerFromAvailableWorkers()
        {
            Manager.StartWorking();
            Manager.StopWorking();

            CollectionAssert.DoesNotContain(Manager.GetWorkers(), WorkerCallback);
        }

    }
}
