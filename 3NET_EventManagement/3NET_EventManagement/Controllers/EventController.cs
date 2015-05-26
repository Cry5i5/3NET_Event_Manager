using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _3NET_EventManagement.Models;
using WebMatrix.WebData;
using System.Web.Security;
using System.Net;

namespace _3NET_EventManagement.Controllers
{
    public class EventController : Controller
    {
        private AppDbContext db = new AppDbContext();



        //
        // GET: /Event/
        public ActionResult Index()
        {
             User user = db.Users.Find(WebSecurity.GetUserId(User.Identity.Name));
             var events = db.Events.Include(e => e.Status).Include(e => e.Type).ToList();
            if (Roles.GetRolesForUser(User.Identity.Name).Contains("admin"))
            {
                events = db.Events.Include(e => e.Status).Include(e => e.Type).ToList();
            }
            else 
            {
               events = db.Events.Include(e => e.Status).Include(e => e.Type).Where( e => e.CreatorId == user.Id).ToList();
                if (events.ToList().Count == 0)
                {
                   events = db.Events.Include(e => e.Status).Include(e => e.Type).ToList();
                   var events2 = new List<Event>();
                    int count = 0;
                    foreach(var item in events)
                    {
                        foreach(var it2 in item.InvitedUsers)
                        {
                            if ( it2.Id == user.Id)
                            {
                                count++;
                            }
                        }
                        if(count == 1)
                        {
                            events2.Add(item);
                            count = 0;
                        }
                    }
                    if (events2.Count() == 0)
                    {
                        events = new List<Event>();
                       
                    }
                    else
                    {
                        events = events2;
                    }
                 

                }
                else
                {
                    var events3 = db.Events.Include(e => e.Status).Include(e => e.Type).ToList();
                   
                    var events2 = new List<Event>();
                    int count = 0;
                    foreach (var item in events3)
                    {
                        foreach (var it2 in item.InvitedUsers)
                        {
                            if (it2.Id == user.Id)
                            {
                                count++;
                            }
                        }
                        if (count == 1)
                        {
                            events2.Add(item);
                            count = 0;
                        }
                    }
                    if (events2.Count() != 0)
                    {
                      foreach(var item in events2)
                      {
                          if (events != null)
                          {
                              events.Add(item);
                          }
                          else
                          {
                              events = new List<Event>();
                              events.Add(item);
                          }
                           

                      }

                    }
                }
         
            }
           
            //int count = 0;
            //foreach (var item in events)
            //{
            //    if (item.CreatorId != user.Id )
            //    {
            //        foreach( var invitedUser in item.InvitedUsers.ToList())
            //        {
            //           if (invitedUser.Id == user.Id)
            //           {
            //               count++;
            //           }
            //           if (count == 0)
            //           {
            //              events.ToList().Remove(item);
            //           }
            //        }
                    
            //    }
                
            //}
            foreach(var item in events)
            {
                if (item.CreatorId == user.Id || Roles.GetRolesForUser(User.Identity.Name).Contains("admin"))
                {
                    ViewBag.isOwner = true;
                }
                else
                {
                    ViewBag.isOwner = false;
                }
            }
            if (events.ToList().Count == 0)
            {
                ViewData["message"] = "No Event found for the moment.";
            }
            return View(events.ToList());
        }

        //
        // GET: /Event/Details/5

        public ActionResult Details(int id = 0)
        {
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        //
        // GET: /Event/Create

        public ActionResult Create()
        {
            ViewBag.StatusId = new SelectList(db.Statuses, "Id", "StatusName");
            ViewBag.TypeId = new SelectList(db.EventTypes, "Id", "TypeName");
            return View();
        }

        //
        // POST: /Event/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Event @event)
        {
            User @user = db.Users.Find(WebSecurity.GetUserId(User.Identity.Name));
        
            
            if (ModelState.IsValid)
            {
                if (Roles.GetRolesForUser(User.Identity.Name).Contains("user"))
                {

                    @event.StatusId = 2;
                    @event.Creator = @user;
                   // @event.CreatorId = @user.Id;
                    db.Events.Add(@event);
                    db.SaveChanges();
                }
                else
                {
                    @event.CreatorId = @user.Id;
                    db.Events.Add(@event);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
           
                ViewBag.StatusId = new SelectList(db.Statuses, "Id", "StatusName", @event.StatusId);
            
            
            
           
            ViewBag.TypeId = new SelectList(db.EventTypes, "Id", "TypeName", @event.TypeId);
            return View(@event);
        }

        //
        // GET: /Event/Edit/5

        public ActionResult Edit(int id = 0)
        {
            var editedEvent = db.Events.Find(id);
           

            if (editedEvent == null)
            {
                return HttpNotFound();
            }
            ViewData["Event"] = editedEvent;
            ViewBag.StatusId = new SelectList(db.Statuses, "Id", "StatusName", editedEvent.StatusId);
            ViewBag.TypeId = new SelectList(db.EventTypes, "Id", "TypeName", editedEvent.TypeId);
            return View(editedEvent);
        }

        //
        // POST: /Event/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Event editedEvent, FormCollection formAttr)
        {
            if (ModelState.IsValid)
            {
                //Event eventSave = (Event)ViewBag.Event;
               /* if (Event.Creator == null)
                {
                    Event.Creator = eventSave.Creator;
                    Event.Status = eventSave.Status;
                }*/
                var test = ViewBag.eventModel;
                editedEvent.Creator = db.Users.Find(editedEvent.CreatorId);
                editedEvent.Status = db.Statuses.Find(editedEvent.StatusId);
                
                editedEvent.Type = db.EventTypes.Find(editedEvent.TypeId);

                db.Entry(editedEvent).State = EntityState.Modified;
             //  db.Entry(@event).Property("CreatorId").IsModified = false;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StatusId = new SelectList(db.Statuses, "Id", "StatusName", editedEvent.StatusId);
            ViewBag.TypeId = new SelectList(db.EventTypes, "Id", "TypeName", editedEvent.TypeId);
            return View(editedEvent);
        }

        //
        // GET: /Event/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        //
        // POST: /Event/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Event @event = db.Events.Find(id);
            db.Events.Remove(@event);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        //
        // POST: /Event/LockOrUnLock/5

        [HttpPost, ActionName("LockOrUnLock")]
        [ValidateAntiForgeryToken]
        public ActionResult LockOrUnLock(int id)
        {
            Event @event = db.Events.Find(id);
            if (@event.Status.StatusName.Equals("Pending"))
            {
                @event.Status = db.Statuses.Find(1);
                @event.StatusId = 1;
            }
            else if (@event.Status.StatusName.Equals("Open"))
            {
                @event.Status = db.Statuses.Find(2);
                @event.StatusId = 2;
            }
            db.Entry(@event).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //
        // GET: /Event/InviteFriend
        public ActionResult InviteFriend(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            Event newEvent = db.Events.Find(id);
            if (newEvent == null)
            {
                return HttpNotFound();
            }
            ViewBag.FriendUserId = new SelectList(db.Users.Find(WebSecurity.GetUserId(User.Identity.Name)).Friends, "Id", "UserName");
            return View();

        }
        //
        // POST: /Event/InviteFriend
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InviteFriend(int? id, [Bind(Include = "FriendUserId")] InviteFriendModel friend)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            Event newEvent = db.Events.Find(id);
            if (newEvent == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid) {

                int friendId = int.Parse(friend.FriendUserId);
                User friendInvited = db.Users.Where(x => x.Id == friendId ).FirstOrDefault();

                friendInvited.EventsInvitedTo.Add(newEvent);
                newEvent.InvitedUsers.Add(friendInvited);
                db.SaveChanges();

                return RedirectToAction("Index");
            
            }
            ViewBag.FriendUserId = new SelectList(db.Users.Find(WebSecurity.GetUserId(User.Identity.Name)).Friends, "Id", "UserName");
            return View(friend);

        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}