using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using WorkManager.ConcurrentContainers;
using WorkManager.DataContracts;

namespace WorkManager
{
    public delegate void DistributionCancelledHandler(object sender, EventArgs e);

    public class WorkDistributer : IDisposable
    {
        public event DistributionCancelledHandler DistributionCancelled;

        private ServiceHost Host { get; set; }

        private CancellationTokenSource Cancellation { get; set; }
        public bool IsDistributingWork { get; set; }

        private WorkContainer WorkContainer { get; set; }

        protected virtual void OnDistributionCancelled()
        {
            var handler = DistributionCancelled;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public WorkDistributer(ServiceHost host, WorkContainer workContainer)
        {
            Host = host;
            WorkContainer = workContainer;
        }

        public void AddWork(List<int> workToDo)
        {
            workToDo.ForEach(work => WorkContainer.AddNewWork(work));
        }

        public void StartDistrubutingWork()
        {
            
            if (!IsDistributingWork)
            {
                Host.Open();

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

            Host.Close();
        }

        public void DistributeWorkThread(CancellationToken cancelToken)
        {
            while (true)
            {
                cancelToken.ThrowIfCancellationRequested();

                if (!WorkContainer.IsAnyWorkUnassigned() || IntegerWorkManager.AvailableCallbacks.Count == 0)
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
            var largestItemGuid = GetLargestIntegerGuid();
            var workValue = WorkContainer.RemoveUnassignedWork(largestItemGuid);

            var workItem = new WorkItem(largestItemGuid, workValue);
            return workItem;
        }

        private Guid GetLargestIntegerGuid()
        {
            var guid = default(Guid);
            var largest = int.MinValue;
            var workArray = WorkContainer.GetUnassignedWorkIterable();
            foreach (var keyPair in workArray)
            {
                if (keyPair.Value > largest)
                {
                    largest = keyPair.Value;
                    guid = keyPair.Key;
                }
            }
            return guid;
        }

        public void Dispose()
        {
            StopDistributingWork();
            Host.Close();
        }
    }
}
