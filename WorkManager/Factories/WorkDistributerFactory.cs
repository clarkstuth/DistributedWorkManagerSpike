using System.ServiceModel;
using WorkManager.ConcurrentContainers;

namespace WorkManager.Factories
{
    public class WorkDistributerFactory
    {
        public WorkDistributer CreateWorkDistributer()
        {
            var workContainer = new WorkContainer();
            var callbackContainer = new CallbackContainer();
            var serviceHost = CreateWcfServiceHost();
            var distributer = new WorkDistributer(serviceHost, workContainer, callbackContainer);
            return distributer;
        }

        private ServiceHost CreateWcfServiceHost()
        {
            return new ServiceHost(typeof(IntegerWorkManager));
        }
    }
}
