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

        // Action link avec l'action Add
        // GET: /Friend/Add/5
        public ActionResult Add(int id)
        {
            try
            {
               
                User currentUser = db.Users.Find(WebSecurity.GetUserId(User.Identity.Name));
                User friend = db.Users.Find(id);
                // Lorsqu'on ajoute un ami, on est automatiquement ajouté en tant qu'ami chez la personne en face
                currentUser.Friends.Add(friend);
                if(friend.Friends.Where( x => x.Id == currentUser.Id).FirstOrDefault() == null)
                {
                    friend.Friends.Add(currentUser);
                }
                
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
                // On recherche les amis selon l username , le nom et le prenom
                List<User> friends =  (from u in db.Users
                                            where u.UserName.Contains(addFriendViewModel.FriendName) || u.Name.Contains(addFriendViewModel.FriendName) || u.LastName.Contains(addFriendViewModel.FriendName)
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
                   // Si ce n'est pas l'utilisateur connecté
                    if (item.Id != currentUser.Id)
                    {
                     filteredFriends.Add(item);  
                    }
                    // On récupère et filtre par rapport aux amis déjà ajoutés
                    foreach (var friend in listOfFriends)
                    {
                        if (friend.Id == item.Id)
                        {
                            filteredFriends.Remove(item);
                        }
                    }
                   
                       
                }
                if (filteredFriends.Count == 0)
                {
                    ViewData["error"] = " User not found, please try other username.";
                    return View();
                }

                
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
    }
}
