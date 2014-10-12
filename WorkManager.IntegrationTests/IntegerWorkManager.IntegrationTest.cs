using System.ServiceModel;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkManager.IntegrationTests.IntegerWorkManager;

namespace WorkManager.IntegrationTests
{
    [TestClass]
    public class IntegerWorkManagerTest
    {
        [TestMethod]
        public void IntegerWorkManagerShouldRemoveWorkerOnSuddenDisconnect()
        {
            var worker = new WorkerCallback();
            var instanceContext = new InstanceContext(worker);
            var client = new WorkManagerClient(instanceContext);

            //connect client, ensure this was recorded.
            client.StartWorking();
            Thread.Sleep(1);
            Assert.AreEqual(1, WorkManager.IntegerWorkManager.Workers.Count);

            //disconnect client without stopping working, ensure this was also recorded.
            client.Close();
            Thread.Sleep(1);
            Assert.AreEqual(0, WorkManager.IntegerWorkManager.Workers.Count);
        }
    }
}
