using System.Collections.Generic;
using System.Messaging;

namespace TestClient
{
    public class Queue
    {
        public string Name { get; set; }
        public bool CanRead { get; set; }
        public IEnumerable<Message> Messages { get; set; }
    }
}