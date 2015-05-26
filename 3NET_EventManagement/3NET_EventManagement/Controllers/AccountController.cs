using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using _3NET_EventManagement.Filters;
using _3NET_EventManagement.Models;


namespace _3NET_EventManagement.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
         
                return RedirectToLocal(returnUrl);
            }

            // Si nous sommes arrivés là, quelque chose a échoué, réafficher le formulaire
            ModelState.AddModelError("", "Incorrect Password or Username");
            return View(model);
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }
        // GET: /Account/AdminRegister

        [AllowAnonymous]
        public ActionResult AdminRegister()
        {
            return View("AdminRegister");
        }
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AdminRegister(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Tentative d'inscription de l'utilisateur
                try
                {
                    User u = new User();
                    u.UserName = model.UserName;
                    u.Name = model.Name;
                    using (AppDbContext db = new AppDbContext())
                    {
                        db.Users.Add(u);
                        db.SaveChanges();
                    }


                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                    if (!Roles.GetAllRoles().Contains("admin"))
                    {
                        Roles.CreateRole("admin");
                        Roles.CreateRole("user");
                    }

                    

                    Roles.AddUserToRole(model.UserName, "admin");

                    WebSecurity.Login(model.UserName, model.Password);
                    return RedirectToAction("Index", "Home");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // Si nous sommes arrivés là, quelque chose a échoué, réafficher le formulaire
            return View("AdminRegister", model);
        }
        // GET: /Account/Edit
        public ActionResult Edit()
        {
            
            var db = new AppDbContext();

            User @user = db.Users.Find(WebSecurity.GetUserId(User.Identity.Name));
            //UserManager.Update(@user);
            if (@user == null)
            {
                return HttpNotFound();
            }
            return View("Edit", new EditViewModel { UserName = @user.UserName, Name = @user.Name , LastName = @user.LastName, Age = @user.Age, Email= @user.Email });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: /Account/Edit
        public ActionResult Edit(EditViewModel editViewModel)
        {
             var db = new AppDbContext();

            // User @user = db.Users.Find(WebSecurity.GetUserId(User.Identity.Name));
          
             
             User user = db.Users.Find(WebSecurity.GetUserId(User.Identity.Name));

             if (ModelState.IsValid)
             {
                 //user.UserName = editViewModel.UserName;
                 user.Name = editViewModel.Name;
                 user.Email = editViewModel.Email;
                 user.Age = editViewModel.Age;
                 user.LastName = editViewModel.LastName;
                 db.Entry(user).State = System.Data.EntityState.Modified;
                 int rowAffected = db.SaveChanges();
                 if (rowAffected > 0)
                 {
                     ViewData["success"] = "Your account has been updated";
                     return View("Edit", new EditViewModel { UserName = @user.UserName, Name = @user.Name });
                 }
             }
              
         
             
           //  return RedirectToAction("Edit", new { Message = "Erreur lors de la mise à jour." });
             return View("Edit", new EditViewModel { UserName = @user.UserName, Name = @user.Name });
           
        }
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Tentative d'inscription de l'utilisateur
                try
                {
                    bool exist = false;
                    User u = new User();
                    u.UserName = model.UserName;
                    u.Name = model.Name;
                    using ( AppDbContext db = new AppDbContext())
                    {
                        if (db.Users.Where(x => x.UserName == u.UserName).FirstOrDefault() != null)
                            exist = true;
                        exist = false;
                        if (exist == false)
                        {
                            db.Users.Add(u);
                            db.SaveChanges();
                        }
                    }
                    
                    if (exist == false)
                    {
                        WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                        if (!Roles.GetAllRoles().Contains("admin"))
                        {
                            Roles.CreateRole("admin");
                            Roles.CreateRole("user");
                        }
                        Roles.AddUserToRole(model.UserName, "user");

                        WebSecurity.Login(model.UserName, model.Password);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewData["error"] = "Username already exist, please choose another username.";
                        return View();
                    }
                   
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // Si nous sommes arrivés là, quelque chose a échoué, réafficher le formulaire
            return View(model);
        }

        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {   
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Dissocier uniquement le compte si l'utilisateur actuellement connecté est le propriétaire
            if (ownerAccount == User.Identity.Name)
            {
                // Utiliser une transaction pour empêcher l'utilisateur de supprimer ses dernières informations d'identification de connexion
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Password has been successfully modified."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been setted."
                : message == ManageMessageId.RemoveLoginSuccess ? "Extern connection has been deleted."
                
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword va lever une exception plutôt que de renvoyer la valeur False dans certains scénarios de défaillance.
                    bool changePasswordSucceeded;
                    try
                    {
                      
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Incorrect current password or new password.");
                    }
                }
            }
            else
            {
                // L'utilisateur n'a pas de mot de passe local. Veuillez donc supprimer les erreurs de validation provoquées par un
                // champ OldPassword manquant
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", String.Format("This account cannot be created. Account with name \"{0}\" already exist.", User.Identity.Name));
                    }
                }
            }

            // Si nous sommes arrivés là, quelque chose a échoué, réafficher le formulaire
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // Si l'utilisateur actuel est connecté, ajoutez le nouveau compte
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
              
            }
            else
            {
                // L'utilisateur est nouveau. Demander le nom d'appartenance souhaité
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider = null;
            string providerUserId = null;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Insérer un nouvel utilisateur dans la base de données
                using (AppDbContext db = new AppDbContext())
                {
                    User user = db.Users.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
                    // Vérifier si l'utilisateur n'existe pas déjà
                    if (user == null)
                    {
                        // Insérer le nom dans la table des profils
                        db.Users.Add(new User { UserName = model.UserName });
                        db.SaveChanges();

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
                        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "Username already exists. Please choose another username.");
                    }
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/ExternalLoginFailure

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            List<ExternalLogin> externalLogins = new List<ExternalLogin>();
            foreach (OAuthAccount account in accounts)
            {
                AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                });
            }

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }

        #region Applications auxiliaires
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            UpdateProfile,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // Consultez http://go.microsoft.com/fwlink/?LinkID=177550 pour
            // obtenir la liste complète des codes d'état.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Username already exists. Please choose other";

                case MembershipCreateStatus.DuplicateEmail:
                    return "Username already exists for this email. Try with another email.";

                case MembershipCreateStatus.InvalidPassword:
                    return "Password not correct please retry with appropriate password.";

                case MembershipCreateStatus.InvalidEmail:
                    return "Email is not valid, please verify or try with another password.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "Response for password's confirmation is not correct.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The question is invalid. Please try another question.";

                case MembershipCreateStatus.InvalidUserName:
                    return "Username is not valid please try with another username.";

                case MembershipCreateStatus.ProviderError:
                    return "Authentication Server not respond. Please try again later.";

                case MembershipCreateStatus.UserRejected:
                    return "Creation of User canceled, please try again later or contact your administrator.";

                default:
                    return "An error occured please try again or contact your administrator.";
            }
        }
        #endregion

       
    }
}
