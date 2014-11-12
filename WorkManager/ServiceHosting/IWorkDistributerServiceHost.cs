using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkManager.ServiceHosting
{
    public interface IWorkDistributerServiceHost
    {
        void Open();
        void Close();

        event EventHandler Opened;
        event EventHandler Closed;
    }
}
