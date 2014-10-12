using System;
using System.Collections.Generic;
using System.ServiceModel;
using WorkManager;

namespace WorkManagerApplication
{
    public class Program
    {
        public static ServiceHost IntegerWebServiceHost { get; set; }

        static void Main(string[] args)
        {
            try
            {
                CreateWebServiceHost();
                StartWebServiceHost();

                DisplayStartupMessage();
                PassUserInputToWorkManager();
            }
            finally
            {
                CloseWebServiceHost();
            }
        }

        static void CreateWebServiceHost()
        {
            IntegerWebServiceHost = new ServiceHost(typeof(IntegerWorkManager));
        }

        static void StartWebServiceHost()
        {
            Console.WriteLine("Starting web service host...");
            IntegerWebServiceHost.Open();
        }

        static void CloseWebServiceHost()
        {
            if (IntegerWebServiceHost.State == CommunicationState.Opened ||
                IntegerWebServiceHost.State == CommunicationState.Opening)
            {
                Console.WriteLine("Closing web service host...");
                IntegerWebServiceHost.Close();
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

                intList.ForEach(IntegerWorkManager.AddWorkItem);
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
