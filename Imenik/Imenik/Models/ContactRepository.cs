using Imenik.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imenik.Entities
{
    public class ContactRepository : IContactRepository
    {
        private AdresarContext db;
        public ContactRepository()
        {
            db = new AdresarContext();
        }
        
        public IQueryable<Owner> GetOwners()
        {
            return db.AllOwners.Include("Contacts.ContactPhones")
                .AsQueryable();
        }
    
        public IQueryable<Contact> GetContacts(string userName)
        {
            var owner = db.AllOwners
                .Include("Contacts")
                .Include("Contacts.ContactPhones")
                .Where(o => userName == o.Username).FirstOrDefault();
            if (owner == null)
                return null;
            else
            {
                if (owner.Username.CompareTo(userName) != 0)
                    return null;
                return owner.Contacts.AsQueryable();
            }
        }

        public IQueryable<Contact> GetContacts(int userId)
        {
            var contacts = db.AllContacts
                .Include("ContactPhones")
                .Include("Owner")
                .Where(c => c.OwnerId == userId);
            if (contacts == null)
                return null;
            return contacts.AsQueryable();
        }

        public Contact GetContact(int contactId)
        {
            var contact = db.AllContacts
                .Include("ContactPhones")
                .Where(c => c.ContactId == contactId).FirstOrDefault();
            return contact;
        }

        public Phone GetPhone(string phonenumber)
        {
            var result = db.AllPhones
                .Where(p => p.PhoneNumber.CompareTo(phonenumber) == 0)
                .FirstOrDefault();
            return result;
        }

        public Phone GetPhone(int phoneid)
        {
            var result = db.AllPhones
                .Where(p => p.PhoneId == phoneid)
                .FirstOrDefault();
            return result;
        }


        public bool Create(Owner owner)
        {
            try
            {
                db.AllOwners.Add(owner);
                // Popunit tablicu s početnim kontaktima
                GenerateDefaultContacts(db, owner);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Create(string username, Contact contact)
        {
            Owner owner = GetOwnerByUsername(username);
            if (owner != null)
            {
                try 
                {
                    contact.Owner = owner;
                    contact.OwnerId = owner.OwnerId;
                    db.AllContacts.Add(contact);
                    return true;
                }
                catch 
                {
                    return false;
                }
            }
            return false;
        }

        public bool Create(Phone phone)
        {
            try
            {
                db.AllPhones.Add(phone);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Create(int contactId, Phone phone)
        {
            Contact ctc = GetContact(contactId);
            try 
            {
                phone.Contact = ctc;
                db.AllPhones.Add(phone);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Update(Contact newContact)
        {
            try
            {
                Contact contact = db.AllContacts.Where(c => c.ContactId == newContact.ContactId).FirstOrDefault();
                contact.City = newContact.City;
                contact.Description = newContact.Description;
                contact.FirstName = newContact.FirstName;
                contact.LastName = newContact.LastName;
                contact.ImgUri = newContact.ImgUri == null ? contact.ImgUri : newContact.ImgUri;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Update(Phone newPhone)
        {
            try
            {
                Phone phone = db.AllPhones.Where(p => p.PhoneId == newPhone.PhoneId).FirstOrDefault();
                phone.PhoneDescription = newPhone.PhoneDescription;
                phone.PhoneNumber = newPhone.PhoneNumber;
                phone.PhoneType = newPhone.PhoneType;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteOwner(int ownerId)
        {
            try
            {
                Owner own = GetOwners().Where(o => o.OwnerId == ownerId).FirstOrDefault();
                if (own == null)
                    return false;
                foreach (var contact in own.Contacts)
                {
                    foreach (var ph in contact.ContactPhones)
                    {
                        db.AllPhones.Remove(ph);
                    }
                    db.AllContacts.Remove(contact);
                }
                db.AllOwners.Remove(own);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteContact(int id)
        {
            var ctc = db.AllContacts
                .Include("ContactPhones")
                .Where(c => c.ContactId == id).FirstOrDefault();
            if (ctc == null)
                return false;
            db.AllContacts.Remove(ctc);
            return true;
        }

        public bool DeletePhone(int id)
        {
            var ph = db.AllPhones.Where(p => p.PhoneId == id).FirstOrDefault();
            if (ph != null)
            {
                db.AllPhones.Remove(ph);
                return true;
            }
            return false;
        }

        public bool CheckForPhone(string phonenumber, int id)
        {
            var contact = db.AllContacts.Where(c => c.ContactId == id).FirstOrDefault();
            if (contact == null)
                return true;
            var phone = db.AllPhones.Where(p => p.PhoneNumber.Equals(phonenumber)).FirstOrDefault();
            if (phone == null)
                return false;
            if (contact.OwnerId == phone.Contact.OwnerId)
                return true;
            return false;
        }


        public bool Save()
        {
            try
            {
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }


        public int GetOwnerId(Owner owner)
        {
            return db.Entry(owner).Entity.OwnerId;
        }


        public Owner GetOwnerByUsername(string username)
        {
            var owner = GetOwners().Where(o => o.Username.Equals(username)).First();
            return owner;
        }
 
        private void GenerateDefaultContacts(AdresarContext context, Owner newOwner) 
        {
            var owner = context.AllOwners
                .Where(o => o.Username.Equals(newOwner.Username)).FirstOrDefault();
            for (int i = 0; i < 20; ++i)
            {
                Contact ctc = new Contact
                {
                    FirstName = String.Format("Marko{0}", i),
                    LastName = String.Format("Marković{0}", i),
                    Owner = owner,
                    City = "Zagreb",
                    Description = "Poznanik iz srednje škole",
                    ImgUri = "https://dl.dropboxusercontent.com/u/16697048/Lock.jpg"
                };
                context.AllContacts.Add(ctc);
                Phone ph = new Phone
                {
                    Contact = ctc,
                    PhoneDescription = "Uvijek dostupan",
                    PhoneNumber = String.Format("097000000{0}", i),
                    PhoneType = "Oosobni"
                };
                context.AllPhones.Add(ph);
            }
            for (int i = 0; i < 20; ++i)
            {
                Contact ctc = new Contact
                {
                    FirstName = String.Format("Petra{0}", i),
                    LastName = String.Format("Petroović{0}", i),
                    Owner = owner,
                    City = "Zagreb",
                    Description = "Poznanica iz srednje škole",
                    ImgUri = "https://dl.dropboxusercontent.com/u/16697048/Lock.jpg"
                };
                context.AllContacts.Add(ctc);
                Phone ph = new Phone
                {
                    Contact = ctc,
                    PhoneDescription = "Uvijek dostupna",
                    PhoneNumber = String.Format("091000000{0}", i),
                    PhoneType = "Oosobni"
                };
                context.AllPhones.Add(ph);
            }

            for (int i = 0; i < 20; ++i)
            {
                Contact ctc = new Contact
                {
                    FirstName = String.Format("Ivan{0}", i),
                    LastName = String.Format("Ivković{0}", i),
                    Owner = owner,
                    City = "Zagreb",
                    Description = "Poznanik iz srednje škole",
                    ImgUri = "https://dl.dropboxusercontent.com/u/16697048/Lock.jpg"
                };
                context.AllContacts.Add(ctc);
                Phone ph = new Phone
                {
                    Contact = ctc,
                    PhoneDescription = "Uvijek dostupan",
                    PhoneNumber = String.Format("098000000{0}", i),
                    PhoneType = "Oosobni"
                };
                context.AllPhones.Add(ph);
            }


            var Milos = new Contact
            {
                FirstName = "Miloš",
                LastName = "Trifunović",
                City = "Slatina",
                Description = "Odličan kandidat za ovaj posao :)",
                Owner = owner,
                ImgUri = "https://dl.dropboxusercontent.com/u/16697048/Lock.jpg"
            };
            var p = new Phone
            {
                PhoneDescription = "Uvijek dostupan, osim kada nije",
                PhoneNumber = "0976456556",
                PhoneType = "Osobni",
                Contact =  Milos 
            };

            context.AllPhones.Add(p);
            context.AllContacts.Add(Milos);
            context.SaveChanges();

        }


    }
}
