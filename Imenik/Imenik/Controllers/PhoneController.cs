using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Imenik.Models;
using Imenik.Entities;
using System.Web.Http.Routing;

namespace Imenik.Controllers
{

    public class PhoneController : BaseController
    {
        public IHttpActionResult Get(string phonenumber)
        {
            var ret = Repository.GetPhone(phonenumber);
            if (ret == null)
                return NotFound();
            var jsonret = JSONManager.Jsonify(ret);
            return Ok<object>(new
            {
                phone = jsonret,
                contact = ret.ContactId
            });
        }

        public JSONPhone Get(int id)
        {
            var phone = Repository.GetPhone(id);
            return JSONManager.Jsonify(phone);
        }

        public IHttpActionResult Post(int id, [FromBody] JSONPhone phone)
        {
            if (Repository.CheckForPhone(phone.phoneNumber, id))
                return BadRequest("You have already registered this phone number - " + phone.phoneNumber + " or you sent me wrong id value");
            Contact contact = Repository.GetContact(id);
            if (contact == null)
                return BadRequest("There is no contact with the specified id value of " + id.ToString());
            Phone newPhone = new Phone { 
                PhoneDescription = phone.description,
                PhoneType = phone.type,
                PhoneNumber = phone.phoneNumber,
                ContactId = contact.ContactId
            };

            Repository.Create(id,newPhone);
            Repository.Save();

            phone.phoneId = Repository.GetPhone(newPhone.PhoneNumber).PhoneId;
            urlHelper = new UrlHelper(Request);
            var route = urlHelper.Link("Phone", new { phonenumber = phone.phoneNumber });
            return Created<JSONPhone>(route, phone);
        }


        public IHttpActionResult Put(string phonenumber, [FromBody] JSONPhone phone)
        {

            if (ModelState.IsValid)
            {
                Phone newPhone = Repository.GetPhone(phonenumber);
                if (newPhone == null)
                    return BadRequest("Phonenumber " + phonenumber + " was not found in the database. I told you that alcohol isn't good for you.");

                newPhone.PhoneDescription = phone.description;
                newPhone.PhoneNumber = phone.phoneNumber;
                newPhone.PhoneType = phone.type;

                if (Repository.Update(newPhone))
                {
                    Repository.Save();
                    return Ok<int>(newPhone.PhoneId);
                }
                else 
                    return InternalServerError();
            }
            return BadRequest("Something went wrong. ");
        }

        public IHttpActionResult Delete(int id)
        {
            try
            {
                Repository.DeletePhone(id);
                Repository.Save();
                return Ok();
            }
            catch
            {
                return BadRequest("Something went wrong while deleting the contact phone with the id of " + id);
            }

        }
        public IHttpActionResult Delete(string phonenumber)
        {
            try
            {
                var id = (Repository.GetPhone(phonenumber)).PhoneId;
                Repository.DeletePhone(id);
                Repository.Save();
                return Ok();
            }
            catch
            {
                return BadRequest("Something went wrong while deleting the contact phone with the number of " + phonenumber);
            }
        }


    }
}
