using System;
using System.Runtime.Serialization;

namespace WorkManager.DataContracts
{
    [DataContract]
    public class WorkItem
    {
        [DataMember(IsRequired = true)]
        public readonly int WorkToDo;

        [DataMember(IsRequired = true)]
        public readonly Guid WorkGuid;

        public WorkItem(Guid workGuid, int workToDo)
        {
            WorkGuid = workGuid;
            WorkToDo = workToDo;
        }
    }
}
