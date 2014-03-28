using System.Collections.Generic;
using System.Messaging;

namespace Echelon.Core
{
    public class Queue
    {
        public string QueueName { get; set; }

        public string DisplayName
        {
            get { return (QueueName ?? "").Replace("private$\\", string.Empty); }
        }

        public bool CanRead { get; set; }
        public int MessageCount { get; set; }
        public int JournalQueueMessageCount { get; set; }
    }
}