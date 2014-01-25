using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Imenik.Controllers
{
    public class EchoController : ApiController
    {
        public IHttpActionResult Get()
        {
            return Ok<string>("Stigo do servisa");
        }
    }
}
