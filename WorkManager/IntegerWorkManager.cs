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

        public readonly static ConcurrentDictionary<Guid, int> AvailableWork = new ConcurrentDictionary<Guid, int>();

        public readonly static ConcurrentDictionary<Guid, IWorker> ActiveWork = new ConcurrentDictionary<Guid, IWorker>();

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
        }

        /// <summary>
        /// Will remove the current work item from being available.
        /// </summary>
        /// <param name="workItem"></param>
        public override void WorkComplete(WorkItem workItem)
        {
            GenerateExceptionIfGuidDoesNotExist(workItem.WorkGuid);

            IWorker assignedClient;
            ActiveWork.TryRemove(workItem.WorkGuid, out assignedClient);

            var workerCallback = GetCurrentWorkerCallback();
            workerCallback.IsWorking = false;

            if (workerCallback != assignedClient)
            {
                var message = String.Format("Work Guid: '{0}' not assigned to reporting callback.", workItem.WorkGuid);
                throw new WorkNotAssignedException(message);
            }
        }

        private static void GenerateExceptionIfGuidDoesNotExist(Guid guid)
        {
            if (!ActiveWork.ContainsKey(guid))
            {
                var message = String.Format("Provided Work GUID does not exist: {0}", guid);
                throw new InvalidWorkItemException(message);
            }
        }

        void EventServiceClosing(object sender, EventArgs e)
        {
            HandleDisconnectEvent(sender, e);
        }

        void EventServiceClosed(object sender, EventArgs e)
        {
            HandleDisconnectEvent(sender, e);
        }

        private static void HandleDisconnectEvent(object sender, EventArgs e)
        {
            var callback = (IWorker) sender;
            callback.Active = false;
        }

    }
}
