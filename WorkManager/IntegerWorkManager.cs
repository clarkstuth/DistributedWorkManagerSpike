using System;
using System.Collections.Generic;
using WorkManager.Callbacks;
using WorkManager.DataContracts;

namespace WorkManager
{
    public class IntegerWorkManager : AbstractWorkManager
    {
        public readonly static CallbackContainer<IWorker> Callbacks = new CallbackContainer<IWorker>();

        public readonly static List<int> WorkItems = new List<int>(); 

        public CallbackContainer<IWorker> GetWorkers()
        {
            return Callbacks;
        }

        public List<int> GetWorkItems(int i)
        {
            return WorkItems;
        }

        /// <summary>
        /// Registers the current client as available to work.  Will add the client
        /// to the pool of available workers.
        /// </summary>
        public override void StartWorking()
        {
            AddCurrentWorkerToCollection();
            BindToCommunicationObjectCallbacks();
        }

        private void BindToCommunicationObjectCallbacks()
        {
            var communicationObject = GetCallbackChannel();
            communicationObject.Closed += EventServiceClosed;
            communicationObject.Closing += EventServiceClosing;
        }
        
        public override void StopWorking()
        {
            RemoveCurrentWorkerFromCollection();
        }

        private void AddCurrentWorkerToCollection()
        {
            var callbackObject = GetWorkerCallback();
            Callbacks.Add(callbackObject);
        }


        public void RemoveCurrentWorkerFromCollection()
        {
            var callbackObject = GetWorkerCallback();
            Callbacks.Remove(callbackObject);
        }

        public override void WorkComplete(int workItemGuid)
        {
            throw new NotImplementedException();
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
            if (callback != null && Callbacks.Contains(callback))
            {
                Callbacks.Remove(callback);
            }
        }

        public static void AddWorkItem(int i)
        {
            WorkItems.Add(i);
        }

    }
}
