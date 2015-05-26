using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _3NET_EventManagement.Models;
using WebMatrix.WebData;

namespace _3NET_EventManagement.Controllers
{
    public class ContributionController : Controller
    {
        private AppDbContext db = new AppDbContext();

        //
        // GET: /Contribution/EventId

        public ActionResult Index(int id = 0)
        {

            var contributions = db.Contributions.Include(c => c.Event).Include(c => c.Type).Where(c => c.EventId == id);
            return View(contributions.ToList());
        }

        //
        // GET: /Contribution/Details/5

        public ActionResult Details(int id = 0)
        {
            Contribution contribution = db.Contributions.Find(id);
            if (contribution == null)
            {
                return HttpNotFound();
            }
            return View(contribution);
        }

        //
        // GET: /Contribution/Create

        public ActionResult Create(int id = 0)
        {
            Event concernedEvent = db.Events.Find(id);
            Contribution contrib = new Contribution();
            contrib.Event = concernedEvent;
            contrib.EventId = concernedEvent.Id;
            contrib.User = db.Users.Find(WebSecurity.GetUserId(User.Identity.Name));
            contrib.UserId = WebSecurity.GetUserId(User.Identity.Name);
            ViewBag.EventId = new SelectList(db.Events, "Id", "EventName");
            ViewBag.TypeId = new SelectList(db.ContributionTypes, "Id", "ContributionTypeName");
            return View(contrib);
        }

        //
        // POST: /Contribution/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Contribution contribution)
        {
            contribution.User = db.Users.Find(WebSecurity.GetUserId(User.Identity.Name));
            contribution.Event = db.Events.Find(contribution.EventId);
            contribution.Type = db.ContributionTypes.Find(contribution.TypeId);
            if (contribution.Type != null && contribution.User != null && contribution.Event != null)
            {
                db.Contributions.Add(contribution);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = contribution.EventId });
            }

            ViewBag.EventId = new SelectList(db.Events, "Id", "EventName", contribution.EventId);
            ViewBag.TypeId = new SelectList(db.ContributionTypes, "Id", "ContributionTypeName", contribution.TypeId);
            return View(contribution);
        }

        //
        // GET: /Contribution/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Contribution contribution = db.Contributions.Find(id);
            if (contribution == null)
            {
                return HttpNotFound();
            }
            ViewBag.EventId = new SelectList(db.Events, "Id", "EventName", contribution.EventId);
            ViewBag.TypeId = new SelectList(db.ContributionTypes, "Id", "ContributionTypeName", contribution.TypeId);
            return View(contribution);
        }

        //
        // POST: /Contribution/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Contribution contribution)
        {
            contribution.User = db.Users.Find(contribution.UserId);
            contribution.Event = db.Events.Find(contribution.EventId);
            if (contribution.User != null && contribution.Event != null)
            {
                db.Entry(contribution).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = contribution.EventId });
            }
            ViewBag.EventId = new SelectList(db.Events, "Id", "EventName", contribution.EventId);
            ViewBag.TypeId = new SelectList(db.ContributionTypes, "Id", "ContributionTypeName", contribution.TypeId);
            return View(contribution);
        }

        //
        // GET: /Contribution/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Contribution contribution = db.Contributions.Find(id);
            if (contribution == null)
            {
                return HttpNotFound();
            }
            return View(contribution);
        }

        //
        // POST: /Contribution/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Contribution contribution = db.Contributions.Find(id);
            db.Contributions.Remove(contribution);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = contribution.EventId });
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}