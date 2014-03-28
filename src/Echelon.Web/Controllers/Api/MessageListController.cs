using System;
using System.Collections.Generic;
using System.Data.Metadata.Edm;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Messaging;
using Echelon.Core;

namespace Echelon.Web.Controllers.Api
{
    public class MessageListController : ApiController
    {
        public IEnumerable<MessageInfo> GetMessages(string computer, string queueName)
        {
            var queueBrowser = new QueueBrowser(computer);
            return queueBrowser.PeekMessages(queueName);
        }
    }
}
