using System.ServiceModel;

namespace WorkManager.Factories
{
    public class WorkDistributerFactory
    {
        public WorkDistributer CreateWorkDistributer()
        {
            var serviceHost = CreateWcfServiceHost();
            var distributer = new WorkDistributer(serviceHost);
            return distributer;
        }

        private ServiceHost CreateWcfServiceHost()
        {
            return new ServiceHost(typeof(IntegerWorkManager));
        }
    }
}
