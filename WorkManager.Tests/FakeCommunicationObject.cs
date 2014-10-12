using System;
using System.ServiceModel;
using WorkManager.DataContracts;

namespace WorkManager.Tests
{
    public class FakeCommunicationObject : ICommunicationObject
    {
        public void Abort()
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginClose(AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginOpen(AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public void Close(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public event EventHandler Closed;

        public event EventHandler Closing;

        public void EndClose(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public void EndOpen(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public event EventHandler Faulted;

        public void Open(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public void Open()
        {
            throw new NotImplementedException();
        }

        public event EventHandler Opened;

        public event EventHandler Opening;

        public CommunicationState State
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsEventHandlerClosedSet()
        {
            if (Closed != null)
            {
                return true;
            }
            return false;
        }

        public bool IsEventHandlerClosingSet()
        {
            if (Closing != null)
            {
                return true;
            }
            return false;
        }

        public void TriggerClosedEvent(IWorker sender)
        {
            if (Closed != null)
            {
                EventArgs args = null;
                Closed(sender, args);
            }
        }

        public void TriggerClosingEvent(WorkItem sender)
        {
            if (Closing != null)
            {
                EventArgs args = null;
                Closing(sender, args);
            }
        }

    }
}
