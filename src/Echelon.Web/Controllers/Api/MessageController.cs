using System.Messaging;
using System.Web.Http;
using Echelon.Core;

namespace Echelon.Web.Controllers.Api
{
    public class MessageController : ApiController
    {
        public Message GetMessage(string computer, string queueName, string messageId)
        {
            var queueBrowser = new QueueBrowser(computer);
            return queueBrowser.PeekMessage(queueName, messageId);
        }
    }
}
