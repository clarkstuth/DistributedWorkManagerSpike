using System.Runtime.Serialization;

namespace WorkManager.DataContracts
{
    [DataContract]
    public class WorkItem
    {
        [DataMember(IsRequired = true)]
        public readonly int WorkToDo;

        [DataMember(IsRequired = true)]
        public readonly string WorkGuid;

        public WorkItem(string workGuid, int workToDo)
        {
            WorkGuid = workGuid;
            WorkToDo = workToDo;
        }
    }
}
