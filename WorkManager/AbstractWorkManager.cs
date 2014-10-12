using System.ServiceModel;
using WorkManager.DataContracts;

namespace WorkManager
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public abstract class AbstractWorkManager : IWorkManager
    {
        public abstract void StartWorking();
        public abstract void StopWorking();
        public abstract void WorkComplete(WorkItem workItem);

        private OperationContext OperationContext { get; set; }

        public OperationContext GetOperationContext()
        {
            if (OperationContext == null)
            {
                OperationContext = OperationContext.Current;
            }
            return OperationContext;
        }

        public void SetOperationContext(OperationContext context)
        {
            OperationContext = context;
        }

        protected ICommunicationObject GetCallbackChannel()
        {
            var operationContext = GetOperationContext();
            var communicationObject = operationContext.GetCallbackChannel<ICommunicationObject>();
            return communicationObject;
        }

        protected IWorker GetCurrentWorkerCallback()
        {
            var operationContext = GetOperationContext();
            var workerCallback = operationContext.GetCallbackChannel<IWorker>();
            return workerCallback;
        }
    }
}