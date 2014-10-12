using System.ServiceModel;

namespace WorkManager.DataContracts
{
    public interface IWorker
    {
        [OperationContract(IsOneWay = true)]
        void DoWork(WorkItem workItem);

        bool Active { get; set; }
        bool IsWorking { get; set; }
    }
}