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

        protected OperationContext _operationContext { get; set; }

        public OperationContext GetOperationContext()
        {
            if (_operationContext == null)
            {
                _operationContext = OperationContext.Current;
            }
            return _operationContext;
        }

        public void SetOperationContext(OperationContext context)
        {
            _operationContext = context;
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