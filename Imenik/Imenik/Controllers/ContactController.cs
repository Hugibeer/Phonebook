using Imenik.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Imenik.Models;
using System.Web.Http.Routing;
using System.Web;
using System.Threading.Tasks;
using System.IO;

namespace Imenik.Controllers
{
    public class ContactController : BaseController
    {
        private string DefaultImageUrl = "https://dl.dropboxusercontent.com/u/16697048/Lock.jpg";
        public object Get(string username, int currentPage = 0, int pageSize=7)
        {
            if (username == null || username == "")
                return null;
                //                return BadRequest("You know, maybe you should ask me for someone, not just mess with those empty strings");
            if (currentPage < 0)
                return null;
                //    return BadRequest("Parameter you sent me was out of range.");
            
            var result = Repository.GetContacts(username).ToList();
            if (result != null)
            {
                var cnt = result.Count;
                var maxPages = (int)Math.Ceiling((double)cnt/pageSize);
                if (currentPage > maxPages)
                    return null;
                    //    return BadRequest("Parameter you sent me was out of range");
                urlHelper = new UrlHelper(Request);
                var prevLink = currentPage > 0 ? urlHelper.Link("Username", new { username = username, currentPage = currentPage - 1, pageSize = pageSize }) : "";
                var nextLink = currentPage < maxPages - 1 ? urlHelper.Link("Username", new { username = username, currentPage = currentPage + 1, pageSize = pageSize }) : "";

                result = result.Skip(currentPage * pageSize)
                    .Take(pageSize)
                    .ToList();
                var ret = JSONManager.Jsonify(result);

                return new XMLRetClass
                {
                    prevLink = prevLink, nextLink = nextLink,
                    max = maxPages, contacts = ret.ToList(), curr = currentPage
                };
                

            }
            return null;
        }
        [HttpGet]
        public JSONContact Get(int id)
        {
            var contact = Repository.GetContact(id);
            if (null == contact)
                return null;
            return JSONManager.Jsonify(contact);
        }

        public async Task<IHttpActionResult> Post()
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            string imagebase = HttpContext.Current.Server.MapPath("~/App_Data/Images/");
            string imagedir = HttpContext.Current.Server.MapPath("~/App_Data/Images/Temp");
            var provider = new MultipartFormDataStreamProvider(imagedir);
            
            try
            {
                // Pročitat sadržaj forme
                await Request.Content.ReadAsMultipartAsync(provider);

                Random rand = new Random((int)DateTime.Now.Ticks);
                var imgname = rand.Next(1000000, 9999999);
                while (File.Exists(Path.Combine(imagedir, imgname.ToString())))
                {
                    imgname = rand.Next(1000000, 9999999);
                }

                var form = provider.FormData;

                string imgUrl = DefaultImageUrl;
                try
                {

                    List<MultipartFileData> files = new List<MultipartFileData>();
                    foreach (MultipartFileData file in provider.FileData)
                    {
                        files.Add(file);
                    }

                    string imgSrc = null;
                    try
                    {
                        imgSrc = form.GetValues("image")[0];
                    }
                    catch
                    {

                    }
                    if (files != null && imgSrc != null)
                    {
                        imgUrl = imagebase + "Contacts/" + imgname.ToString() + Path.GetExtension(imgSrc);
                        File.Move(files[0].LocalFileName, imgUrl);
                        File.Delete(files[0].LocalFileName);
                        imgUrl = "/App_Data/Images/Contacts/" + imgname.ToString() + ".jpg";
                    }
                }
                catch
                {
                    return InternalServerError();                    
                }

                Contact contact = new Contact();
                string redirectUrl = "https://www.youtube.com/watch?v=RP4abiHdQpc";
                string username = "";
                foreach (var key in form.AllKeys)
                {
                    switch (key)
                    {
                        case "redirect":
                            redirectUrl = form.GetValues(key)[0];
                            break;
                        case "firstName":
                            contact.FirstName = form.GetValues(key)[0];
                            break;
                        case "lastName":
                            contact.LastName = form.GetValues(key)[0];
                            break;
                        case "city":
                            contact.City = form.GetValues(key)[0];
                            break;
                        case "description":
                            contact.Description = form.GetValues(key)[0];
                            break;
                        case "username":
                            username = form.GetValues(key)[0];
                            break;
                    }
                }

                contact.ImgUri = imgUrl;
                Repository.Create(username, contact);
                Repository.Save();
                return Redirect(redirectUrl);
            }
            catch (Exception e)
            {
                // Gotta catch'em all gotta catch'em all :)
                return InternalServerError(e);
            }
        }

        public IHttpActionResult Put(int id, [FromBody] JSONContact contact)
        {
            Contact newcontact = new Contact { 
                ContactId = id,
                FirstName = contact.firstName,
                LastName = contact.lastName,
                Description = contact.description,
                City = contact.city,
                ImgUri = contact.imgUrl
            };
            Repository.Update(newcontact);
            Repository.Save();
            return Ok();
        }

        public IHttpActionResult Delete(int id)
        {
            if (Repository.DeleteContact(id))
            {
                Repository.Save();
                return Ok();
            }
            else
                return NotFound();
        }

    }

    public class XMLRetClass
    {
        public XMLRetClass() { }
        public string prevLink { get; set; }
        public string nextLink { get; set; }
        public int max { get; set; }
        public int curr { get; set; }
        public List<JSONContact> contacts { get; set; }

    }

}
