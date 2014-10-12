﻿using System;
using System.Collections.Concurrent;
using WorkManager.DataContracts;
using WorkManager.Exceptions;

namespace WorkManager
{
    public class IntegerWorkManager : AbstractWorkManager
    {
        public readonly static BlockingCollection<IWorker> AvailableCallbacks = new BlockingCollection<IWorker>();
        
        public readonly static ConcurrentDictionary<string, int> AvailableWork = new ConcurrentDictionary<string, int>();
        public readonly static ConcurrentDictionary<string, int> ActiveWork = new ConcurrentDictionary<string, int>();

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
            if (!ActiveWork.ContainsKey(workItem.WorkGuid))
            {
                var message = String.Format("Provided Work GUID does not exist: {0}", workItem.WorkGuid);
                throw new InvalidWorkItemException(message);
            }

            int outValue;
            ActiveWork.TryRemove(workItem.WorkGuid, out outValue);

            var workerCallback = GetCurrentWorkerCallback();
            workerCallback.IsWorking = false;
        }

        void EventServiceClosing(object sender, EventArgs e)
        {
            HandleDisconnectEvent(sender, e);
        }

        void EventServiceClosed(object sender, EventArgs e)
        {
            HandleDisconnectEvent(sender, e);
        }

        private void HandleDisconnectEvent(object sender, EventArgs e)
        {
            var callback = (IWorker) sender;
            
        }

    }
}
