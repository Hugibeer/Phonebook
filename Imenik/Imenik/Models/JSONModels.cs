using Imenik.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Imenik.Models
{

    public static class JSONManager
    {
    
        public static JSONUser Jsonify(Owner owner)
        {
            return new JSONUser(owner, true);
        }
        
        public static List<JSONUser> Jsonify(List<Owner> owners, bool contacts)
        {
            List<JSONUser> ret = new List<JSONUser>();
            foreach (var owner in owners)
            {
                var newUser = new JSONUser(owner, contacts);
                ret.Add(newUser);
            }
            return ret;
        }

        public static IEnumerable<JSONContact> Jsonify(List<Contact> result)
        {
            var ret = new List<JSONContact>();
            foreach (var c in result)
            {
                var newContact = new JSONContact(c);
                ret.Add(newContact);
            }
            return ret;
        }
        public static JSONContact Jsonify(Contact contact)
        {
            var ret = new JSONContact(contact);
            return ret;
        }
        
        public static JSONPhone Jsonify(Phone phone)
        {
            JSONPhone ret = new JSONPhone(phone);
            return ret;
        }
    
    }

    public class JSONUser
    {
        public JSONUser() { }

        public JSONUser(Owner o, bool loadContacts)
        {
            id = o.OwnerId;
            userName = o.Username;
            if (!loadContacts)
                return;
            contacts = new List<JSONContact>();
            foreach (var contact in o.Contacts)
            {
                JSONContact c = new JSONContact(contact);
                contacts.Add(c);
            }
        }

        public int id;
        public string userName;
        public List<JSONContact> contacts;
    }

    public class JSONContact
    {
        public JSONContact() { }
        public JSONContact(Contact c)
        {
            contactId = c.ContactId;
            firstName = c.FirstName;
            lastName = c.LastName;
            city = c.City;
            description = c.Description;
            imgUrl = c.ImgUri;
            if (c.ContactPhones.Count == 0)
                return;
            phones = new List<JSONPhone>();
            foreach (var ph in c.ContactPhones)
            {
                var newph = new JSONPhone(ph);
                phones.Add(newph);
            }
        }

        public int contactId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string city { get; set; }
        public string description { get; set; }
        public string imgUrl { get; set; }
        public List<JSONPhone> phones { get; set; }
    }


    public class JSONPhone
    {
        public JSONPhone() { }
        public JSONPhone(Phone p)
        {
            phoneId = p.PhoneId;
            phoneNumber = p.PhoneNumber;
            description = p.PhoneDescription;
            type = p.PhoneType;
        }

        public int phoneId { get; set; }
        public string phoneNumber { get; set; }
        public string description { get; set; }
        public string type { get; set; }
    }
}