using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Messagecenter.Controllers
{
    public class BaseController : Controller
    {
        public bool IsAdmin
        {
            get
            {
                if (Session != null && Session["IsAdmin"] != null && (bool)Session["IsAdmin"])
                    return true;
                return false;
            }
        }

        public string isUser
        {
            get
            {
                if (Session != null && Session["IsAdmin"] != null && Session["User"]!=null)
                {
                    return (string)Session["User"];
                    
                }
                else
                {
                    return null;
                }



            }
        }
        public BaseController()
        {
            // This code will run for all your controllers
            ViewBag.IsAdmin = IsAdmin;
            ViewBag.User = isUser;

        }

    }
}