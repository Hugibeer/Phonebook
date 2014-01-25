using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImenikKlijent.Models
{   
    public class user
    {
        public int id { get; set; }
        public string  userName { get; set; }
        public List<contact> contacts { get; set; }
    }
    public class contact
    {
        public int contactId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string city { get; set; }
        public string description { get; set; }
        public string imgUrl { get; set; }
        public List<phone> phones { get; set; }
    }
    public class phone
    {
        public int phoneId { get; set; }
        public string phoneNumber { get; set; }
        public string description { get; set; }
        public string type { get; set; }
    }
}