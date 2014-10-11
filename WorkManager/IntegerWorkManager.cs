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
            AddWorkerToCollection();
            BindToCommunicationObjectCallbacks();
        }

        private void AddWorkerToCollection()
        {
            var callbackObject = GetWorkerCallback();
            Workers.Add(callbackObject);
        }

        private void BindToCommunicationObjectCallbacks()
        {
            var communicationObject = GetCallbackChannel();
            communicationObject.Closed += EventServiceClosed;
            communicationObject.Closing += EventServiceClosed;
        }
        
        public override void StopWorking()
        {
            throw new NotImplementedException();
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
