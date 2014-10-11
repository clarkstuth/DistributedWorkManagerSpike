using System;
using System.Collections.Generic;
using WorkManager.DataContracts;

namespace WorkManager
{
    public class IntegerWorkManager : AbstractWorkManager
    {
        public static List<IWorker> Workers { get; private set; }

        public IntegerWorkManager()
        {
            if (Workers == null)
            {
                Workers = new List<IWorker>();
            }
        }

        public List<IWorker> GetWorkers()
        {
            return Workers;
        } 

        public override void StartWorking()
        {
            
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
