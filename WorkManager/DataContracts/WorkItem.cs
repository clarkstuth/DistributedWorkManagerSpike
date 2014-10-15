using System;
using System.Runtime.Serialization;

namespace WorkManager.DataContracts
{
    [DataContract]
    public class WorkItem
    {
        [DataMember(IsRequired = true)]
        public int WorkToDo { get; set; }

        [DataMember(IsRequired = true)]
        public Guid WorkGuid { get; set; }
    }
}
