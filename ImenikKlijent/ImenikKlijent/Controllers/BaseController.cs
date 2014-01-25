using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace ImenikKlijent.Controllers
{
    public class BaseController : Controller
    {
        private string adresa;
        protected string BaseAddress
        {
            get
            {
                if (adresa == null)
                    // TODO: Postaviti adresu tako da odgovara vašoj prilikom pokretanja projekta Imenik
                    adresa = "http://localhost:57947";
                return adresa;
            }
        }
        
        private HttpClient klijent;
        protected HttpClient Client
        {
            get
            {
                if (klijent == null)
                    klijent = new HttpClient();
                return klijent;
            }
        }
    }
}
