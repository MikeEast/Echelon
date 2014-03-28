using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Messaging;

namespace Echelon.Core
{
    public class QueueBrowser
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
                messageQueues = GetPrivateQueues(_computerName);
            }
            catch (MessageQueueException mqe)
            {
                Console.WriteLine("No queues here");
                yield break;
            }

            foreach (var messageQueue in messageQueues)
            {
                var journalQueue = GetJournalQueue(messageQueue);
                yield return new Queue
                {
                    QueueName = messageQueue.QueueName,
                    CanRead = HasAccess(messageQueue),
                    MessageCount = GetMessageCount(messageQueue),
                    JournalQueueMessageCount = GetMessageCount(journalQueue)
                };
            }
        }

        public IEnumerable<MessageInfo> PeekMessages(string queueName)
        {
            var messageQueue = GetMessageQueue(queueName);
            return PeekMessages(messageQueue);
        }

        public Message PeekMessage(string queueName, string messageId)
        {
            var messageQueue = GetMessageQueue(queueName);
            messageQueue.MessageReadPropertyFilter.SetAll();
            messageQueue.MessageReadPropertyFilter.SourceMachine = true;
            return messageQueue.PeekById(messageId);
        }

        MessageQueue GetMessageQueue(string queueName)
        {
            if(string.IsNullOrEmpty(queueName))
                throw new ArgumentException("Queue name is mandatory", "queueName");
            queueName = @"private$\" + queueName.Replace(@"private$\", string.Empty);
            var messageQueue = GetPrivateQueues(_computerName).FirstOrDefault(q => q.QueueName == queueName);
            if (messageQueue == null)
                throw new Exception("Queue not found: " + queueName);
            return messageQueue;
        }

        IEnumerable<MessageQueue> GetPrivateQueues(string computerName)
        {
            return MessageQueue.GetPrivateQueuesByMachine(computerName);
        }

        bool HasAccess(MessageQueue messageQueue)
        {
            return messageQueue.CanRead;
        }

        int GetMessageCount(MessageQueue messageQueue)
        {
            messageQueue.MessageReadPropertyFilter.ClearAll();
            var enumerator = messageQueue.GetMessageEnumerator2();
            var messageCount = 0;
            if (!HasAccess(messageQueue))
            {
                return messageCount;
            }
            while (enumerator.MoveNext())
            {
                messageCount++;
            }
            return messageCount;
        }

        MessageQueue GetJournalQueue(MessageQueue messageQueue)
        {
            var journalPath = messageQueue.Path + ";JOURNAL";
            return new MessageQueue(journalPath);
        }


        IEnumerable<MessageInfo> PeekMessages(MessageQueue queue, string labelFilter = null)
        {
            queue.MessageReadPropertyFilter.ClearAll();
            queue.MessageReadPropertyFilter.Id = true;
            queue.MessageReadPropertyFilter.Label = true;
            queue.MessageReadPropertyFilter.SentTime = true;

            var result = new List<MessageInfo>();
            if (!HasAccess(queue))
            {
                return result;
            }

            var messageEnumerator = queue.GetMessageEnumerator2();
            try
            {
                while (messageEnumerator.MoveNext())
                {
                    Message currentMessage = null;
                    try
                    {
                        currentMessage = messageEnumerator.Current;
                    }
                    catch (MessageQueueException e)
                    {
                        //TODO: Use ILog
                        Console.WriteLine(e);
                    }
                    if (currentMessage == null)
                        continue;

                    if (labelFilter == null || currentMessage.Label == labelFilter)
                        result.Add(currentMessage.ToMessageInfo());
                }
            }
            finally
            {
                messageEnumerator.Close();
            }
            queue.Close();
            return result;
        }
    }
}