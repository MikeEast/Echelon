using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Echelon.Core;

namespace Echelon.Web.Controllers.Api
{
    public class ComputerController : ApiController
    {
        public IEnumerable<string> GetComputers()
        {
            var networkBrowser = new NetworkBrowser();
            var computers = networkBrowser.GetNetworkComputers();

            return computers.Select(c => c.Name);
        }
    }
}
