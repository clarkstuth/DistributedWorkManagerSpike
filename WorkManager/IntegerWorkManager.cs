using System;
using System.Collections.Generic;
using WorkManager.DataContracts;

namespace WorkManager
{
    public class IntegerWorkManager : AbstractWorkManager
    {
        public readonly static List<IWorker> Workers = new List<IWorker>();
        
        public List<IWorker> GetWorkers()
        {
            return Workers;
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
            communicationObject.Closing += EventServiceClosed;
        }
        
        public override void StopWorking()
        {
            RemoveCurrentWorkerFromCollection();
        }

        private void AddCurrentWorkerToCollection()
        {
            var callbackObject = GetWorkerCallback();
            Workers.Add(callbackObject);
        }


        public void RemoveCurrentWorkerFromCollection()
        {
            var callbackObject = GetWorkerCallback();
            Workers.Remove(callbackObject);
        }

        public override void WorkComplete(string workItemGuid)
        {
            throw new NotImplementedException();
        }

        void EventServiceClosing(object sender, EventArgs e)
        {
        }

        void EventServiceClosed(object sender, EventArgs e)
        {
        }
    }
}
