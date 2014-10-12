using System;

namespace WorkManager.Exceptions
{
    public class WorkNotAssignedException : Exception
    {
        public WorkNotAssignedException(string message) : base(message)
        {
            
        }
    }
}
