using Imenik.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;

namespace Imenik.Controllers
{
    public class BaseController : ApiController
    {
        private IContactRepository repo = null;
        protected UrlHelper urlHelper;
        protected IContactRepository Repository
        {
            get
            {
                if (repo == null)
                {
                    repo = new ContactRepository();
                }
                return repo;
            }
        }


    }
}
