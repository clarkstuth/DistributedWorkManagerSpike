using System;
using System.Collections.Concurrent;
using WorkManager.DataContracts;
using WorkManager.Exceptions;

namespace WorkManager
{
    public class IntegerWorkManager : AbstractWorkManager
    {
        public readonly static BlockingCollection<IWorker> AvailableCallbacks = new BlockingCollection<IWorker>();
        
        public readonly static ConcurrentDictionary<Guid, int> AllWork = new ConcurrentDictionary<Guid, int>(); 
        public readonly static ConcurrentDictionary<Guid, int> UnassignedWork = new ConcurrentDictionary<Guid, int>();
        public readonly static ConcurrentDictionary<IWorker, Guid> AssignedWork = new ConcurrentDictionary<IWorker, Guid>();

        /// <summary>
        /// Registers the current client as available to work.  Will add the client
        /// to the pool of available workers.
        /// </summary>
        public override void StartWorking()
        {
            var callback = GetCurrentWorkerCallback();
            callback.Active = true;
            AvailableCallbacks.Add(callback);

            BindToCommunicationObjectCallbacks();
        }

        private void BindToCommunicationObjectCallbacks()
        {
            var communicationObject = GetCallbackChannel();
            communicationObject.Closed += EventServiceClosed;
            communicationObject.Closing += EventServiceClosing;
        }

        /// <summary>
        /// Removes the current client from the list available to work on things.
        /// If work is currently assigned to the client it will e
        /// </summary>
        public override void StopWorking()
        {
            var callback = GetCurrentWorkerCallback();
            callback.Active = false;

            PutAssignedWorkBackIntoAvailableCollection(callback);
        }

        /// <summary>
        /// Will remove the current work item from being available.
        /// </summary>
        /// <param name="workItem"></param>
        public override void WorkComplete(WorkItem workItem)
        {
            var workerCallback = GetCurrentWorkerCallback();

            if (AssignedWork.ContainsKey(workerCallback))
            {
                Guid assignedGuid;
                AssignedWork.TryRemove(workerCallback, out assignedGuid);
            }

            workerCallback.IsWorking = false;
        }

        private static void EventServiceClosing(object sender, EventArgs e)
        {
            HandleDisconnectEvent(sender, e);
        }

        private static void EventServiceClosed(object sender, EventArgs e)
        {
            HandleDisconnectEvent(sender, e);
        }

        private static void HandleDisconnectEvent(object sender, EventArgs e)
        {
            var callback = (IWorker) sender;
            callback.Active = false;

            PutAssignedWorkBackIntoAvailableCollection(callback);
        }

        private static void PutAssignedWorkBackIntoAvailableCollection(IWorker callback)
        {
            if (callback.IsWorking && AssignedWork.ContainsKey(callback))
            {
                Guid assignedGuid;
                AssignedWork.TryRemove(callback, out assignedGuid);

                UnassignedWork.GetOrAdd(assignedGuid, AllWork[assignedGuid]);
                
            }
            callback.IsWorking = false;
        }

    }
}
