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

        public override void StartWorking()
        {
            var callbackObject = GetWorkerCallback();
            Workers.Add(callbackObject);
        }

        public override void StopWorking()
        {
            throw new NotImplementedException();
        }

        public override void WorkComplete(string workItemGuid)
        {
            throw new NotImplementedException();
        }
    }
}
