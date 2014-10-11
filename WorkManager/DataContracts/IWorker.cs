using System.ServiceModel;

namespace WorkManager.DataContracts
{
    
    public interface IWorker
    {
        [OperationContract(IsOneWay = true)]
        void DoWork();
    }
}