using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imenik.Entities
{
    public class Contact
    {
        public Contact()
        {
            ContactPhones = new List<Phone>();
        }
        
        public int ContactId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string City { get; set; }
        public string ImgUri { get; set; }
        public string Description { get; set; }

        public ICollection<Phone> ContactPhones { get; set; }
        public int OwnerId { get; set; }
        public Owner Owner { get; set; }
    
    }
}
