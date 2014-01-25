using Imenik.Entities;
using Imenik.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Imenik.Controllers
{
    public class UserController : BaseController
    {
        public IHttpActionResult Get(bool contacts = false)
        {
            var ret = Repository.GetOwners().ToList();
            if (ret == null)
                return NotFound();
            return Ok<IEnumerable<JSONUser>>(JSONManager.Jsonify(ret, contacts));
        }

        public IHttpActionResult Get(string username)
        {
            try
            {
                var ret = Repository.GetOwnerByUsername(username);
                return Ok<JSONUser>(JSONManager.Jsonify(ret));
            }
            catch 
            {
                return NotFound();
            }
        }


        public IHttpActionResult Post([FromBody]JSONUser user)
        {
            if (user.userName.CompareTo("") == 0)
                return BadRequest("Username can't be empty string");

            var owner = new Owner { Username = user.userName };
            if (!Repository.Create(owner))
                return BadRequest("Couldn't create user with username=" + user.userName);
            Repository.Save();
            owner.OwnerId = Repository.GetOwnerId(owner);
            return Ok<JSONUser>(JSONManager.Jsonify(owner));          

        }
  
        public IHttpActionResult Delete()
        {
            try
            {
                var username = Request.GetRouteData().Values["username"].ToString();
                var owner = Repository.GetOwnerByUsername(username);
                if (owner == null)
                    return BadRequest("No owner with username=" + username);
                Repository.DeleteOwner(owner.OwnerId);
                Repository.Save();
                return Ok();
            } 
            catch
            {
                return BadRequest();
            }
        }

    }
}
