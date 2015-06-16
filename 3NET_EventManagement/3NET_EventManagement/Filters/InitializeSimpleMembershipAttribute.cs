using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Web.Mvc;
using WebMatrix.WebData;
using _3NET_EventManagement.Models;

namespace _3NET_EventManagement.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class InitializeSimpleMembershipAttribute : ActionFilterAttribute
    {
        private static SimpleMembershipInitializer _initializer;
        private static object _initializerLock = new object();
        private static bool _isInitialized;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // S'assurer qu'ASP.NET Simple Membership est initialisé une seule fois par démarrage d'application
            LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
        }

        private class SimpleMembershipInitializer
        {
            public SimpleMembershipInitializer()
            {
                Database.SetInitializer<AppDbContext>(new System.Data.Entity.CreateDatabaseIfNotExists<AppDbContext>());
               //Database.SetInitializer<AppDbContext>(new ApplicationDbContextInitializer());

                try
                {
                    using (var context = new AppDbContext())
                    {
                        if (!context.Database.Exists())
                        {
                            // Créer la base de données SimpleMembership sans schéma de migration Entity Framework
                            ((IObjectContextAdapter)context).ObjectContext.CreateDatabase();
                           
                        }
                    }

                    WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Impossible d'initialiser la base de données ASP.NET Simple Membership. Pour plus d'informations, consultez la page http://go.microsoft.com/fwlink/?LinkId=256588", ex);
                }
            }
        }
    }
}
