using _3NET_EventManagement.Filters;
using _3NET_EventManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace _3NET_EventManagement.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class FriendController : Controller
    {
        private readonly AppDbContext db = new AppDbContext();
        //
        // GET: /Friend/

        public ActionResult Index()
        {
            User user = db.Users.Find(WebSecurity.GetUserId(User.Identity.Name));

            return View(user.Friends.ToList());
        }

        //
        // GET: /Friend/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }


        //
        // GET: /Friend/Delete/5
        public ActionResult Delete(int id)
        {

            db.Users.Find(WebSecurity.GetUserId(User.Identity.Name)).Friends.RemoveAt(id);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

       

        //
        // GET: /Friend/Search

        public ActionResult Search()
        {
           
            return View();
        }

        //
        // GET: /Friend/SearchResults

        public ActionResult SearchResults()
        {

            return View();
        }

        //
        // GET: /Friend/Add/5

       /* public ActionResult Add(int id = 0)
        {
            User friend = db.Users.Find(id);
            if (friend == null)
            {
                return HttpNotFound();
            }
            return View(friend);
        }*/
        //
        // POST: /Friend/Add

       //[HttpPost, ActionName("Add")]
       //[ValidateAntiForgeryToken]
        public ActionResult Add(int id)
        {
            try
            {
               // if (!ModelState.IsValid) return View();

              /*  User friend  = (from u in db.Users
                               where u.UserName.Equals(addFriendViewModel.FriendName) || u.Name.Equals(addFriendViewModel.FriendName)
                               select u).FirstOrDefault();

                User currentUser = db.Users.Find(WebSecurity.GetUserId(User.Identity.Name));
                
                // Si l'utilisateur n 'existe pas 
                if (friend == null)
                {
                    ViewData["error"] = " User not found, please try other username.";
                    return View();
                }
                // Si l'utilisateur est l'utilisateur qui a fait la demande
                if (friend.Id == currentUser.Id)
                {
                    ViewData["error"] = " You can't yourself as a friend.";
                    return View();               
                }
                // Si l'utilisateur est déjà dans la liste d'amis
                if (currentUser.Friends.ToList().Any(u => u.Id.Equals(friend.Id)))
                {
                    ViewData["error"] = "User already exists in your friends list.";
                    return View();
                }
                // Sinon on ajoute l'ami dans la liste d'amis de l'utilisateur courant*/
                User currentUser = db.Users.Find(WebSecurity.GetUserId(User.Identity.Name));
                User friend = db.Users.Find(id);
                currentUser.Friends.Add(friend);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View("Index");
            }
        }

        //
        // POST: /Friend/Search

        [HttpPost]
        public ActionResult Search(AddFriendViewModel addFriendViewModel)
        {
            
                if (!ModelState.IsValid) return View();

                List<User> friends =  (from u in db.Users
                                            where u.UserName.Contains(addFriendViewModel.FriendName) || u.Name.Contains(addFriendViewModel.FriendName)
                                            select u).ToList();

                User currentUser = db.Users.Find(WebSecurity.GetUserId(User.Identity.Name));

                if (friends.Count() == 0)
                {
                    // Si l'utilisateur n 'existe pas 
                    ViewData["error"] = " User not found, please try other username.";
                    return View();
                }
                List<User> filteredFriends = new List<User>();
                List<User> listOfFriends = currentUser.Friends.ToList();
                foreach(User item in friends)
                {
                   
                    if (item.Id != currentUser.Id  )
                    {
                     filteredFriends.Add(item);  
                    }

                    foreach (var friend in listOfFriends)
                    {
                        if (friend.Id == item.Id)
                        {
                            filteredFriends.Remove(item);
                        }
                    }
                    if (filteredFriends.Count == 0)
                    {
                        ViewData["error"] = " User not found, please try other username.";
                        return View();
                    }
                       
                }

                //ViewData["friendsFoundList"] = filteredFriends;
                return View("SearchResults", filteredFriends.ToList());
            
        }

        //
        // GET: /Friend/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Friend/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Friend/Delete/5

       /* public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Friend/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }*/
    }
}
