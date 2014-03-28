using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Echelon.Core;

namespace Echelon.Web.Controllers.Api
{
    public class QueueListController : ApiController
    {
        public IEnumerable<Queue> GetQueues([FromUri]string computer)
        {
            var queueBrowser = new QueueBrowser(computer);
            var messageQueues = queueBrowser.GetMessageQueues();

            return messageQueues;
        }
    }
}
