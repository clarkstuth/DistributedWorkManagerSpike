using System;
using System.Collections.Concurrent;
using System.Linq;
using WorkManager.DataContracts;

namespace WorkManager.ConcurrentContainers
{
    public class CallbackContainer
    {
        private BlockingCollection<IWorker> AvailableCallbacks { get; set; }

        public CallbackContainer()
        {
            AvailableCallbacks = new BlockingCollection<IWorker>();
        }

        public void AddAvailableCallback(IWorker callback)
        {
            AvailableCallbacks.Add(callback);
        }

        public bool AnyAvailableCallbacks()
        {
            return AvailableCallbacks.Count != 0;
        }

        public IWorker GetAvailableCallbackWithinTimeout(TimeSpan callbackSelectionTimeout)
        {
            IWorker worker;
            AvailableCallbacks.TryTake(out worker, callbackSelectionTimeout);
            return worker;
        }

        public int GetNumberAvailableCallbacks()
        {
            return AvailableCallbacks.Count;
        }

        public bool IsCallbackAvailable(IWorker workerCallback)
        {
            return AvailableCallbacks.Any(callback => callback == workerCallback);
        }
    }
}
