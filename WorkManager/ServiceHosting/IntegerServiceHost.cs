using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using WorkManager.ConcurrentContainers;

namespace WorkManager.ServiceHosting
{
    public class IntegerServiceHost : ServiceHost, IWorkDistributerServiceHost
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

        public IntegerServiceHost(CallbackContainer callbackContainer, WorkContainer workContainer, Type serviceType,
            IDictionary<string, ContractDescription> implementedContracts, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
            CallbackContainer = callbackContainer;
            WorkContainer = workContainer;

            foreach (var cd in implementedContracts.Values)
            {
                cd.Behaviors.Add(new IntegerInstanceProvider(callbackContainer, workContainer));
            }
        }
    }
}
