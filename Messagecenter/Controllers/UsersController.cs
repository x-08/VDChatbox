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
                    return RedirectToAction("Index", "Messages");
                }
               




            }
            return RedirectToAction("Login", "Users");



        }


        public ActionResult Login()
        {
            if (Session["IsAdmin"] != null && (bool)Session["IsAdmin"] == true)
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
            return RedirectToAction("Login", "Users");

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
