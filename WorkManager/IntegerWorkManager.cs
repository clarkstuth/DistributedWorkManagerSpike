﻿using System;
using System.Collections.Concurrent;
using WorkManager.ConcurrentContainers;
using WorkManager.DataContracts;

namespace WorkManager
{
    public class IntegerWorkManager : AbstractWorkManager
    {
        public readonly static ConcurrentBag<IWorker> AllCallbacks = new ConcurrentBag<IWorker>();
        public readonly static BlockingCollection<IWorker> AvailableCallbacks = new BlockingCollection<IWorker>();

        public WorkContainer WorkContainer { get; set; }

        public IntegerWorkManager(WorkContainer workContainer)
        {
            WorkContainer = workContainer;
        }

        /// <summary>
        /// Registers the current client as available to work.  Will add the client
        /// to the pool of available workers.
        /// </summary>
        public override void StartWorking()
        {

            Console.WriteLine("Client connected.");
            var callback = GetCurrentWorkerCallback();
            callback.Active = true;
            AvailableCallbacks.Add(callback);

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
            callback.Active = false;

            PutAssignedWorkBackIntoAvailableCollection(callback);
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

            if (WorkContainer.IsWorkAssigned(workerCallback))
            {
                WorkContainer.RemoveAssignedWork(workerCallback);
            }

            AvailableCallbacks.Add(workerCallback);
            workerCallback.IsWorking = false;
        }

        private void PutAssignedWorkBackIntoAvailableCollection(IWorker callback)
        {
            if (callback.IsWorking && WorkContainer.IsWorkAssigned(callback))
            {
                var assignedGuid = WorkContainer.RemoveAssignedWork(callback);
                WorkContainer.SetUnassignedWork(assignedGuid);
            }
            callback.IsWorking = false;
        }

    }
}
