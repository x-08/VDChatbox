using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Messagecenter.Models;
using System.Threading.Tasks;


namespace Messagecenter.Controllers
{
    public class MessagesController : BaseController
    {
        private messagecenterEntities db = new messagecenterEntities();
        private const int postPerPage = 10;
        // GET: Messages
        public ActionResult Index(int? id)
        {
            if (!IsAdmin)
            {
                return RedirectToAction("Login", "Users");
            }
            int p = id ?? 0;
            IEnumerable<Message> messages = (from post in db.Messages
                                             where post.Time < DateTime.Now
                                             orderby post.Time descending
                                             select post).Skip(p * postPerPage).Take(postPerPage);
            ViewBag.isPreviousLinkAvailable = p > 0;
            ViewBag.isNextLinkAvailable = messages.Count() > postPerPage - 1;
            ViewBag.pageNumber = p;
            ViewBag.isAdmin = IsAdmin;
            ViewBag.isUser = isUser;
            return View(messages.Reverse());
        }

        [OutputCache(NoStore = true, Location = System.Web.UI.OutputCacheLocation.Client, Duration = 10)]
        public ActionResult refreshview(int ? id)
        {
            if (!IsAdmin)
            {
                return RedirectToAction("Login", "Users");
            }
            int p = id ?? 0;
            IEnumerable<Message> messages = (from post in db.Messages
                                             where post.Time < DateTime.Now
                                             orderby post.Time descending
                                             select post).Skip(p * postPerPage).Take(postPerPage);
            ViewBag.isPreviousLinkAvailable = p > 0;
            ViewBag.isNextLinkAvailable = messages.Count() > postPerPage - 1;
            ViewBag.pageNumber = p;
            ViewBag.isAdmin = IsAdmin;
            ViewBag.isUser = isUser;
            return PartialView("_indexPartial", messages.Reverse());
        }
        
 

        
        [HttpPost]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Body,Time")] Message message)
        {
            if (!IsAdmin && isUser != null)
            {
                return RedirectToAction("Login", "Users");
            }
            string Username = isUser;

            message.Name = Username;
            message.Time = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
            if (ModelState.IsValid)
                if (ModelState.IsValid)
                {
                    db.Messages.Add(message);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }

            return View(message);
            

        }

    

        // GET: Messages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = db.Messages.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Message message = db.Messages.Find(id);
            db.Messages.Remove(message);
            db.SaveChanges();
            return RedirectToAction("Index");
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
