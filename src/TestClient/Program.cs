﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
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
                    Console.WriteLine(queue.Name + " has " + queue.Messages.Count());
                }
            }

            //var queues = MessageQueue.GetPrivateQueuesByMachine(computer);
            //Console.WriteLine(computer + " has " + queues.Count() + " queues");
            Console.ReadKey();
        }
    }

    public class Queue
    {
        public string Name { get; set; }
        public bool CanRead { get; set; }
        public IEnumerable<Message> Messages { get; set; }
    }

    class QueueBrowser
    {
        private readonly string _computerName;
        private bool _isMsmqAvailable;

        public QueueBrowser(string computerName)
        {
            _computerName = computerName;
        }

        public IEnumerable<Queue> GetMessageQueues()
        {
            var messageQueues = Enumerable.Empty<MessageQueue>();
            try
            {
                messageQueues = MessageQueue.GetPrivateQueuesByMachine(_computerName).ToList();
            }
            catch (MessageQueueException mqe)
            {
                Console.WriteLine("No queues here");
                yield break;
            }

            foreach (var messageQueue in messageQueues)
            {
                var messages = Enumerable.Empty<Message>();
                var canRead = messageQueue.CanRead;
                if (canRead)
                    messages = PeekMessagesOnQueue(messageQueue);

                yield return new Queue
                {
                    Name = messageQueue.QueueName,
                    CanRead = canRead,
                    Messages = messages
                };
            }
        }

        IEnumerable<Message> PeekMessagesOnQueue(MessageQueue messageQueue)
        {
            var enumerator = messageQueue.GetMessageEnumerator2();
            while(enumerator.MoveNext()) {
                if (enumerator.Current == null)
                    continue;
                yield return messageQueue.PeekById(enumerator.Current.Id, TimeSpan.FromSeconds(1));
            }
        }
    }
}