using System;
using System.ServiceModel;
using System.Threading;
using WorkManager.Client.IntegerWorkManager;

namespace WorkManager.Client
{
    class Program
    {
        internal static WorkManagerClient Client { get; set; }

        static void Main(string[] args)
        {
            var callbackInstance = new InstanceContext(new Callback());
            Client = new WorkManagerClient(callbackInstance);

            Client.Open();

            Client.StartWorking();

            Console.WriteLine("Waiting for work to do.");
            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();

            Client.StopWorking();

            Client.Close();
        }

        public class Callback : IWorkManagerCallback
        {
            Random RandomGenerator { get; set; }

            public Callback()
            {
                RandomGenerator = new Random();
            }

            public void DoWork(WorkItem workItem)
            {
                Console.WriteLine("--- Work Received --- Guid: {0}   Value: {1}", workItem.WorkGuid, workItem.WorkToDo);
                var randomSleepTime = RandomGenerator.Next() % 5;
                Thread.Sleep(TimeSpan.FromSeconds(randomSleepTime));
                Client.WorkComplete(workItem);
            }
        }

    }
}
