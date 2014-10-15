using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using WorkManager.ConcurrentContainers;

namespace WorkManager.ServiceHosting
{
    public class IntegerServiceHostFactory : ServiceHostFactory
    {
        private readonly CallbackContainer CallbackContainer;
        private readonly WorkContainer WorkContainer;


        public IntegerServiceHostFactory(CallbackContainer callbackContainer, WorkContainer workContainer)
        {
            CallbackContainer = callbackContainer;
            WorkContainer = workContainer;
        }

        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            return new IntegerServiceHost(CallbackContainer, WorkContainer, serviceType, baseAddresses);
        }
    }
}
