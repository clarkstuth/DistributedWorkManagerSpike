using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using WorkManager.DataContracts;

namespace WorkManager.ConcurrentContainers
{
    public class WorkContainer
    {
        private ConcurrentDictionary<Guid, int> AllWork { get; set; }
        private ConcurrentDictionary<Guid, int> UnassignedWork { get; set; }
        private ConcurrentDictionary<IWorker, Guid> AssignedWork { get; set; }

        public WorkContainer()
        {
            var allWork = new ConcurrentDictionary<Guid, int>();
            var unassignedWork = new ConcurrentDictionary<Guid, int>();
            var assignedWork = new ConcurrentDictionary<IWorker, Guid>();
            Initialize(allWork, unassignedWork, assignedWork);
        }

        public WorkContainer(ConcurrentDictionary<Guid, int> allWork, ConcurrentDictionary<Guid, int> unassignedWork, ConcurrentDictionary<IWorker, Guid> assignedWork)
        {
            Initialize(allWork, unassignedWork, assignedWork);
        }

        private void Initialize(ConcurrentDictionary<Guid, int> allWork, ConcurrentDictionary<Guid, int> unassignedWork,
            ConcurrentDictionary<IWorker, Guid> assignedWork)
        {
            AllWork = allWork;
            UnassignedWork = unassignedWork;
            AssignedWork = assignedWork;
        }

        public bool IsWorkAssigned(IWorker workerCallback)
        {
            return AssignedWork.ContainsKey(workerCallback);
        }

        public Guid AddNewWork(int work)
        {
            var newWorkGuid = Guid.NewGuid();
            AllWork.TryAdd(newWorkGuid, work);
            UnassignedWork.TryAdd(newWorkGuid, work);
            return newWorkGuid;
        }

        public Guid RemoveAssignedWork(IWorker workerCallback)
        {
            Guid assignedGuid;
            AssignedWork.TryRemove(workerCallback, out assignedGuid);
            return assignedGuid;
        }

        public void SetAssignedWork(IWorker workerCallback,Guid workGuid)
        {
            AssignedWork.TryAdd(workerCallback, workGuid);
        }

        public void SetUnassignedWork(Guid workGuid)
        {
            var unassignedWorkValue = AllWork[workGuid];
            UnassignedWork.TryAdd(workGuid, unassignedWorkValue);
        }

        public int RemoveUnassignedWork(Guid workGuid)
        {
            int outValue;
            UnassignedWork.TryRemove(workGuid, out outValue);
            return outValue;
        }

        public bool IsAnyWorkUnassigned()
        {
            return UnassignedWork.Count != 0;
        }

        public KeyValuePair<Guid, int>[] GetUnassignedWorkIterable()
        {
            return UnassignedWork.ToArray();
        }

        public bool WorkValueExists(int workValue)
        {
            return AllWork.Values.Any(v => v == workValue);
        }

        public bool UnassignedWorkValueExists(int workValue)
        {
            return UnassignedWork.Values.Any(v => v == workValue);
        }

        public int GetUnassignedWorkValue(Guid workGuid)
        {
            return UnassignedWork[workGuid];
        }

        public void RemoveWork(Guid workGuid)
        {
            int theValue;
            AllWork.TryRemove(workGuid, out theValue);
        }
    }
}