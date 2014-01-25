using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imenik.Entities
{
    public class Phone
    {
        
        public int PhoneId { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneType { get; set; }
        public string PhoneDescription { get; set; }

        public int ContactId { get; set; }
        public Contact Contact { get; set; }
    }
}
