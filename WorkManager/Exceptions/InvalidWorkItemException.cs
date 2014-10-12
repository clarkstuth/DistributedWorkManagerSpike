using System;

namespace WorkManager.Exceptions
{
    public class InvalidWorkItemException : Exception
    {
        public InvalidWorkItemException(string message) : base(message)
        {
            
        }
    }
}
