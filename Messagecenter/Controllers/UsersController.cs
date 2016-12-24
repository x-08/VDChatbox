using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Messagecenter.Models;
using System.Web.Security;
using System.Web.Configuration;
using System.Text;

namespace Messagecenter.Controllers
{
    public class UsersController : BaseController
    {
        private messagecenterEntities db = new messagecenterEntities();


        public ActionResult LoginVerify(string name, String password)
        {
            if (db.Users.Any(x => x.Name == name))
            {
                var admin = from user in db.Users.Where(x => x.Name == name)
                            select user;

                var selectedUser = admin.FirstOrDefault();
                if (selectedUser.Password == password)
                {
                    Session["IsAdmin"] = true;
                    Session["User"] = selectedUser.Name;
                    if (Request.Cookies["ZikrCookies"] != null)
                    {
                        return RedirectToAction("Index", "Messages");
                        
                    }
                    createCookies(selectedUser.Name, selectedUser.Password);
                    return RedirectToAction("Index", "Messages");

                }
               }
            return RedirectToAction("Login", "Users");



        }


        public ActionResult Login()
        {   if(Request.Cookies["ZikrCookies"] != null)
            {
                return decrpytcookie();
            }
            else if (Session["IsAdmin"] != null && (bool)Session["IsAdmin"] == true)
            {
                return RedirectToAction("Logout");
            }
            else
            {
                Session["IsAdmin"] = null;
                return View();

            }

        }


        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            deletecookie();
            return RedirectToAction("Login", "Users");

        }
        private void createCookies(string a, string b)
        {
            string val = null;
            HttpCookie Zikr = new HttpCookie("ZikrCookies");
            val = a + "|" + b;
            if (val != null)
            {
                val = Convert.ToBase64String(MachineKey.Protect(System.Text.Encoding.UTF8.GetBytes(val)));

            }
            Zikr.Value = val;
            Zikr.Expires = DateTime.Now.AddDays(2);
            if(WebConfigurationManager.AppSettings["ZikrCookieDomain"] != null)
            {
                Zikr.Domain = WebConfigurationManager.AppSettings["ZikrCookieDomain"];
            }
            
            Response.Cookies.Add(Zikr);
        }
        private void deletecookie()
        {
            if(Request.Cookies["ZikrCookies"] != null)
            {
                HttpCookie Zikr = new HttpCookie("ZikrCookies");
                Zikr.Expires=DateTime.Now.AddDays(-1);
                if (WebConfigurationManager.AppSettings["ZikrCookieDomain"] != null)
                {
                    Zikr.Domain = WebConfigurationManager.AppSettings["ZikrCookieDomain"];
                }

                Response.Cookies.Add(Zikr);
            }
            

        }
        private ActionResult decrpytcookie()
        {
            var val=Request.Cookies["ZikrCookies"].Value;
            var decryptedVal = Encoding.UTF8.GetString(MachineKey.Unprotect(Convert.FromBase64String(val)));
                char delimeter = '|';
                string[] parts = decryptedVal.Split((delimeter));
                return LoginVerify(parts[0], parts[1]);
            
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
