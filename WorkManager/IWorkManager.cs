using System.ServiceModel;
using WorkManager.DataContracts;

namespace WorkManager
{
    [ServiceContract(CallbackContract = typeof(IWorker))]
    interface IWorkManager
    {
        [OperationContract(IsOneWay = true)]
        void StartWorking();

        [OperationContract(IsOneWay = true)]
        void StopWorking();

        [OperationContract(IsOneWay = true)]
        void WorkComplete(WorkItem workItem);
    }
}
