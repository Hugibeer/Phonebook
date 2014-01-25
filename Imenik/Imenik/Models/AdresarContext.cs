using Imenik.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imenik.Entities
{
    public class AdresarContext : DbContext
    {
        public AdresarContext() : base("HBServer")
        {
        }

        public DbSet<Owner> AllOwners { get; set; }
        public DbSet<Contact> AllContacts { get; set; }
        public DbSet<Phone> AllPhones { get; set; }
    }
}
