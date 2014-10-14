using System.Collections.Concurrent;
using WorkManager.DataContracts;

namespace WorkManager.ConcurrentContainers
{
    public class CallbackContainer
    {
        private ConcurrentBag<IWorker> AllCallbacks { get; set; }
        private BlockingCollection<IWorker> AvailableCallbacks { get; set; }



    }
}
