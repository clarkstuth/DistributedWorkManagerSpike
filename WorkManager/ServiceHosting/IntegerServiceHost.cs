using System;
using System.ServiceModel;
using WorkManager.ConcurrentContainers;

namespace WorkManager.ServiceHosting
{
    public class IntegerServiceHost : ServiceHost
    {
        private readonly CallbackContainer CallbackContainer;
        private readonly WorkContainer WorkContainer;

        public IntegerServiceHost(CallbackContainer callbackContainer, WorkContainer workContainer, Type serviceType, params Uri[] baseAddresses) : base(serviceType, baseAddresses)
        {
            CallbackContainer = callbackContainer;
            WorkContainer = workContainer;

            foreach (var cd in ImplementedContracts.Values)
            {
                cd.Behaviors.Add(new IntegerInstanceProvider(callbackContainer, workContainer));
            }
        }
    }
}
