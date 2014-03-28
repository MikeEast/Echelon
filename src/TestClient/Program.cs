using System;
using System.Linq;
using Echelon.Core;


namespace TestClient
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var networkBrowser = new NetworkBrowser();
            var computers = networkBrowser.GetNetworkComputers();

            Console.WriteLine("Found these computers: ");

            foreach (var computer in computers)
            {
                Console.WriteLine(computer.Name + ":");

                var qb = new QueueBrowser(computer.Name);
                var queues = qb.GetMessageQueues();
                foreach (var queue in queues)
                {
                    Console.WriteLine(queue.QueueName + " has " + queue.MessageCount);
                }
            }

            //var queues = MessageQueue.GetPrivateQueuesByMachine(computer);
            //Console.WriteLine(computer + " has " + queues.Count() + " queues");
            Console.ReadKey();
        }
    }
}