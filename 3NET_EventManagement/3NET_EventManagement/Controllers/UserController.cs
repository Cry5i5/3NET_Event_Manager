using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _3NET_EventManagement.Models;
using WebMatrix.WebData;
using Microsoft.Web.WebPages.OAuth;
using System.Web.Security;

namespace _3NET_EventManagement.Controllers
{
    public class UserController : Controller
    {
        private AppDbContext db = new AppDbContext();

        //
        // GET: /User/

        public ActionResult Index()
        {
            if (!Roles.GetRolesForUser(User.Identity.Name).Contains("admin"))
                return HttpNotFound();
            if (TempData["createdUser"] != null)
            {
                ViewData["createdUser"] = TempData["createdUser"].ToString();
            }
            List<User> listOfUsers = new List<User>();
            // On filtre pour afficher que les utilisateurs voulus
            foreach(var item in db.Users.ToList())
            {
                if ( item != db.Users.Find(WebSecurity.GetUserId(User.Identity.Name)))
                {
                    listOfUsers.Add(item);
                }
            }

            return View(listOfUsers);
        }

        //
        // GET: /User/Details/5

        public ActionResult Details(int id = 0)
        {
            if (!Roles.GetRolesForUser(User.Identity.Name).Contains("admin"))
                return HttpNotFound();
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // GET: /User/Create

        public ActionResult Create()
        {
            if (!Roles.GetRolesForUser(User.Identity.Name).Contains("admin"))
                return HttpNotFound();
            return View();
        }

        //
        // POST: /User/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                if (db.Users.Where(x => x.UserName == user.UserName).FirstOrDefault() == null)
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                    WebSecurity.CreateUserAndAccount(user.UserName, "123456");
                    // si la checkbox admin est cochée ou pas on le met en tant qu'admin ou pas.
                    if (user.isAdmin == false)
                    {
                        Roles.AddUserToRole(user.UserName, "user");
                        TempData["createdUser"] = "User created with default password '123456', please contact him to give the password";
                    }
                    else
                    {
                        Roles.AddUserToRole(user.UserName, "admin");
                        TempData["createdUser"] = "User admin created with default password '123456', please contact him to give the password";
                    }
                     
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewData["error"] = "Username already exists please choose another.";
                }

                
            }

            return View(user);
        }

        //
        // GET: /User/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (!Roles.GetRolesForUser(User.Identity.Name).Contains("admin"))
                return HttpNotFound();
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /User/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        //
        // GET: /User/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (!Roles.GetRolesForUser(User.Identity.Name).Contains("admin"))
                return HttpNotFound();
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /User/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);

            foreach(var item in user.Friends)
            {
                var userFriend = db.Users.Find(item.Id);
                userFriend.Friends.Remove(user);
            }
            user.Friends.Clear();
            
            var listOfInvitations = db.Invitations.Where(x => x.UserId == id).ToList();
            foreach (var item in listOfInvitations)
                db.Invitations.Remove(item);
            var listOfContributions = db.Contributions.Where(x => x.UserId == id).ToList();
            foreach (var item in listOfContributions)
                db.Contributions.Remove(item);

            db.Users.Remove(user);
            db.SaveChanges();
            
            ((SimpleMembershipProvider)Membership.Provider).DeleteAccount(user.UserName); // deletes record from webpages_Membership table
            Roles.RemoveUserFromRole(user.UserName, Roles.GetRolesForUser(user.Name).First());
            ((SimpleMembershipProvider)Membership.Provider).DeleteUser(user.UserName, true);
            
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}