using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Antlr.Runtime;
using Echelon.Core;

namespace Echelon.Web.Controllers
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
