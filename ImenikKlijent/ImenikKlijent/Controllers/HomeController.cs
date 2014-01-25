using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Web.Http;
using ImenikKlijent.Models;
using System.Threading.Tasks;
using System.Net.Http.Formatting;

namespace ImenikKlijent.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            ViewBag.BaseAddress = BaseAddress;
            return View();
        }
        public JsonResult GetContacts(string what = "lName", bool up = false)
        {
            user model = null;
            var task = Client.GetAsync(BaseAddress + "/api/users/" + User.Identity.Name)
                .ContinueWith(twr =>
                {
                    var response = twr.Result;
                    var result = response.Content.ReadAsAsync<user>();
                    result.Wait();
                    model = result.Result;
                });
            task.Wait();

            // Sad treba popravit slike
            for (int i = 0; i < model.contacts.Count; ++i)
            {
                if (model.contacts[i].imgUrl.Contains("App_Data/Images/Contacts"))
                    model.contacts[i].imgUrl = BaseAddress + model.contacts[i].imgUrl;
            }

            switch (what)
            {
                case "fName":
                    model.contacts.Sort((l, r) => l.firstName.CompareTo(r.firstName));
                    break;
                case "lName":
                    model.contacts.Sort((l, r) => l.lastName.CompareTo(r.lastName));
                    break;
                case "city":
                    model.contacts.Sort((l, r) => l.city.CompareTo(r.city));
                    break;
            }

            if (up)
                model.contacts.Reverse();
            return Json(model);
        }


        public async Task<ActionResult> PutContact(int id, [FromBody] contact contact)
        {
            string path = Server.MapPath("~/App_Data/Updates/Contacts/");

            var image = Request.Files.Get("image");
            if (image.ContentLength == 0)
            {
                var formatter = new JsonMediaTypeFormatter();
                var response = await Client.PutAsync(BaseAddress + "/api/contact/" + id.ToString(),contact, formatter);
                if (response.IsSuccessStatusCode)
                    return RedirectToAction("Index");
                else
                    return Redirect("https://www.youtube.com/watch?v=RP4abiHdQpc");
            }
            else
            {
                string ext = System.IO.Path.GetExtension(image.FileName);
                string imgurl = path + id.ToString() + ext;
                image.SaveAs(imgurl);
                contact.imgUrl = "/App_Data/Updates/Contacts/" + id.ToString() + ext;

                var formatter = new JsonMediaTypeFormatter();
                var response = await Client.PutAsync(BaseAddress + "/api/contact/" + id.ToString(), contact, formatter);
                if (response.IsSuccessStatusCode)
                    return RedirectToAction("Index");
                else
                    return Redirect("https://www.youtube.com/watch?v=RP4abiHdQpc");
            }
        }
        
        public JsonResult PostPhone(int id, [FromBody] phone p)
        {
            phone phone = null;
            var task = Client.PostAsJsonAsync<phone>(BaseAddress + "/api/phone/" + id.ToString(), p)
                .ContinueWith(twr =>
                {
                    var response = twr.Result;
                    var result = response.Content.ReadAsAsync<phone>();
                    result.Wait();
                    phone = result.Result;
                });
            task.Wait();
            return Json(phone);
        }

        public async Task<HttpResponseMessage> DeletePhone(int id)
        {
            var response = await Client.DeleteAsync(BaseAddress + "/api/phone/" + id.ToString());
            return response;
        }

        public async Task<HttpResponseMessage> DeleteContact(int id)
        {
            var response = await Client.DeleteAsync(BaseAddress + "/api/contact/" + id.ToString());
            return response;
        }

        public JsonResult EditPhone([FromUri] string old, [FromBody] phone phone)
        {
            var task = Client.PutAsJsonAsync<phone>(BaseAddress + "/api/phones/" + old, phone)
                .ContinueWith(twr => {
                    var response = twr.Result;
                    response.EnsureSuccessStatusCode();
                });
            task.Wait();
                       
            return Json(phone);
        } 
    

        // CV
        public ViewResult CVPass()
        {
            return View();
        }


    }

}
