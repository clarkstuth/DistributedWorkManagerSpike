using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using WorkManager.DataContracts;

namespace WorkManager
{
    public delegate void DistributionCancelledHandler(object sender, EventArgs e);

    public class WorkDistributer : IDisposable
    {
        private ServiceHost Host { get; set; }

        private CancellationTokenSource Cancellation { get; set; }
        public bool IsDistributingWork { get; set; }

        public event DistributionCancelledHandler DistributionCancelled;

        protected virtual void OnDistributionCancelled()
        {
            var handler = DistributionCancelled;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public WorkDistributer(ServiceHost host)
        {
            Host = host;
        }

        public void AddWork(List<int> workToDo)
        {
            workToDo.ForEach((i) =>
            {
                var guid = Guid.NewGuid();
                IntegerWorkManager.AllWork.TryAdd(guid, i);
                IntegerWorkManager.UnassignedWork.TryAdd(guid, i);
            });
        }

        public void StartDistrubutingWork()
        {
            if (!IsDistributingWork)
            {
                IsDistributingWork = true;
                Cancellation = new CancellationTokenSource();

                new Thread(() =>
                {
                    try
                    {
                        DistributeWorkThread(Cancellation.Token);
                    }
                    catch (OperationCanceledException e)
                    {
                        OnDistributionCancelled();
                        Console.WriteLine("Work distribution interrupted.");
                    }
                }).Start();
            }
        }

        public void StopDistributingWork()
        {
            if (Cancellation != null)
            {
                Cancellation.Cancel();
            }
            IsDistributingWork = false;
        }

        public void DistributeWorkThread(CancellationToken cancelToken)
        {
            while (true)
            {
                cancelToken.ThrowIfCancellationRequested();

                if (IntegerWorkManager.UnassignedWork.Count == 0 || IntegerWorkManager.AvailableCallbacks.Count == 0)
                {
                    //no point in doing anything if no workers are available, or if there is no work to be done
                    continue;
                }

                var workerSelectionTimeout = TimeSpan.FromSeconds(1);
                var worker = GetAvailableCallback(workerSelectionTimeout);

                if (worker != null && worker.Active && !worker.IsWorking)
                {
                    var work = GetHighestPriorityWork();
                    worker.DoWork(work);
                }
            }
        }

        private IWorker GetAvailableCallback(TimeSpan timeout)
        {
            IWorker worker;
            IntegerWorkManager.AvailableCallbacks.TryTake(out worker, timeout);
            return worker;
        }

        private WorkItem GetHighestPriorityWork()
        {
            var workCollection = IntegerWorkManager.UnassignedWork;
            var largestItemGuid = GetLargestIntegerGuid();
            int outValue;
            workCollection.TryRemove(largestItemGuid, out outValue);
            var workItem = new WorkItem(largestItemGuid, outValue);
            return workItem;
        }

        private Guid GetLargestIntegerGuid()
        {
            var guid = default(Guid);
            var largest = int.MinValue;
            var workCollection = IntegerWorkManager.UnassignedWork;
            foreach (var key in workCollection.Keys)
            {
                if (workCollection[key] > largest)
                {
                    largest = workCollection[key];
                    guid = key;
                }
            }
            return guid;
        }

        public void Dispose()
        {
            StopDistributingWork();
        }
    }
}
