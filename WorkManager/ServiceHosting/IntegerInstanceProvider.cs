using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using WorkManager.ConcurrentContainers;

namespace WorkManager.ServiceHosting
{
    public class IntegerInstanceProvider : IInstanceProvider, IContractBehavior
    {
        private readonly CallbackContainer CallbackContainer;
        private readonly WorkContainer WorkContainer;


        public IntegerInstanceProvider(CallbackContainer callbackContainer, WorkContainer workContainer)
        {
            CallbackContainer = callbackContainer;
            WorkContainer = workContainer;
        }

        public object GetInstance(InstanceContext instanceContext)
        {
            return new IntegerWorkManager(WorkContainer, CallbackContainer);
        }

        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            return GetInstance(instanceContext);
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            var disposable = instance as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
        {
        }

        public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint,
            DispatchRuntime dispatchRuntime)
        {
            dispatchRuntime.InstanceProvider = this;
        }

        public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }

        public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint,
            BindingParameterCollection bindingParameters)
        {
        }
    }
}
