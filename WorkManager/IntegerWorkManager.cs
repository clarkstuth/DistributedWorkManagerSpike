using System;
using WorkManager.ConcurrentContainers;
using WorkManager.DataContracts;

namespace WorkManager
{
    public class IntegerWorkManager : AbstractWorkManager
    {
/*
        public readonly static ConcurrentBag<IWorker> AllCallbacks = new ConcurrentBag<IWorker>();
        public readonly static BlockingCollection<IWorker> AvailableCallbacks = new BlockingCollection<IWorker>();
*/

        private CallbackContainer CallbackContainer { get; set; }
        private WorkContainer WorkContainer { get; set; }

        public IntegerWorkManager(WorkContainer workContainer, CallbackContainer callbackContainer)
        {
            WorkContainer = workContainer;
            CallbackContainer = callbackContainer;
        }

        /// <summary>
        /// Registers the current client as available to work.  Will add the client
        /// to the pool of available workers.
        /// </summary>
        public override void StartWorking()
        {

            Console.WriteLine("Client connected.");
            var callback = GetCurrentWorkerCallback();
            CallbackContainer.AddAvailableCallback(callback);

            BindToCommunicationObjectCallbacks();
        }

        private void BindToCommunicationObjectCallbacks()
        {
            var communicationObject = GetCallbackChannel();
            communicationObject.Closed += HandleDisconnectEvent;
            communicationObject.Closing += HandleDisconnectEvent;
        }

        private void HandleDisconnectEvent(object sender, EventArgs e)
        {
            var callback = (IWorker)sender;

            PutAssignedWorkBackIntoAvailableCollection(callback);
        }

        /// <summary>
        /// Removes the current client from the list available to work on things.
        /// If work is currently assigned to the client it will e
        /// </summary>
        public override void StopWorking()
        {
            var callback = GetCurrentWorkerCallback();

            PutAssignedWorkBackIntoAvailableCollection(callback);
        }

        /// <summary>
        /// Will remove the current work item from being available.
        /// </summary>
        /// <param name="workItem"></param>
        public override void WorkComplete(WorkItem workItem)
        {
            var workerCallback = GetCurrentWorkerCallback();

            if (WorkContainer.IsWorkAssigned(workerCallback))
            {
                WorkContainer.RemoveAssignedWork(workerCallback);
            }

            CallbackContainer.AddAvailableCallback(workerCallback);
            WorkContainer.RemoveWork(workItem.WorkGuid);
        }

        private void PutAssignedWorkBackIntoAvailableCollection(IWorker callback)
        {
            if (WorkContainer.IsWorkAssigned(callback))
            {
                var assignedGuid = WorkContainer.RemoveAssignedWork(callback);
                WorkContainer.SetUnassignedWork(assignedGuid);
            }
        }

    }
}
