using System;
using System.Collections.Generic;
using WorkManager;
using WorkManager.Factories;

namespace WorkManagerApplication
{
    public class Program
    {
        static WorkDistributer Distributer { get; set; }

        static void Main(string[] args)
        {
            try
            {
                CreateWorkDistributer();
                Distributer.StartDistrubutingWork();

                DisplayStartupMessage();
                PassUserInputToWorkManager();
            }
            finally
            {
                StopDistributingWork();
            }
        }

        static void CreateWorkDistributer()
        {
            var workDistributerFactory = new WorkDistributerFactory();
            Distributer = workDistributerFactory.CreateWorkDistributer();
        }

        static void StopDistributingWork()
        {
            if (Distributer.IsDistributingWork)
            {
                Console.WriteLine("Stopping work distribution...");
                Distributer.StopDistributingWork();
            }
        }

        static void DisplayStartupMessage()
        {
            Console.WriteLine("---------");
            Console.WriteLine("Integer Work Manager web service has started.");
            Console.WriteLine("To submit work to the service it, type a comma delimited line of integers.");
            Console.WriteLine("Work will be distributed from highest integer to lowest integer");
            Console.WriteLine("Type 'quit' to exit.");
            Console.WriteLine("---------");
        }

        static void PassUserInputToWorkManager()
        {
            while (true)
            {
                var input = Console.ReadLine();
                if (input == "quit")
                {
                    return;
                }

                var intList = new List<int>();
                try
                {
                    intList = ConvertInputToIntArray(input);
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (Exception)
                {
                    Console.WriteLine("Unexpected error parsing input.");
                }

                Distributer.AddWork(intList);
            }
        }

        private static List<int> ConvertInputToIntArray(string input)
        {
            var parts = input.Split(',');
            var intList = new List<int>();

            foreach (var part in parts)
            {
                int parsedInt;
                var result = int.TryParse(part, out parsedInt);
                if (!result)
                {
                    throw new ArgumentException("Invalid argument provided.  Input string must be a comma-delimeted list of integers.", part);
                }

                intList.Add(parsedInt);
            }

            return intList;
        }



    }
}
