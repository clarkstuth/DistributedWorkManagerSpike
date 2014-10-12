using WorkManager.IntegrationTests.IntegerWorkManager;

namespace WorkManager.IntegrationTests
{
    public class WorkerCallback : IWorkManagerCallback
    {
        public void DoWork(int number)
        {
            throw new System.NotImplementedException();
        }
    }
}
