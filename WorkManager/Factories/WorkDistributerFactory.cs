using System.ServiceModel;
using WorkManager.ConcurrentContainers;
using WorkManager.ServiceHosting;

namespace WorkManager.Factories
{
    public class WorkDistributerFactory
    {
        public WorkDistributer CreateWorkDistributer()
        {
            var workContainer = new WorkContainer();
            var callbackContainer = new CallbackContainer();

            var serviceHost = new IntegerServiceHost(callbackContainer, workContainer, typeof (IntegerWorkManager));

            var distributer = new WorkDistributer(serviceHost, workContainer, callbackContainer);
            return distributer;
        }
    }
}
