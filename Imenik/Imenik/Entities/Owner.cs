using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imenik.Entities
{
    public class Owner
    {
        public Owner() 
        {
            Contacts = new List<Contact>();
        }
        public int OwnerId { get; set; }
        public string Username { get; set; }
        public ICollection<Contact> Contacts { get; set; }

    }
}
