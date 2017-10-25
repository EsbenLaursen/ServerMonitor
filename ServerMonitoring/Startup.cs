using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using ServerMonitoring.Models;
using System;
using System.Web.Http;

[assembly: OwinStartupAttribute(typeof(ServerMonitoring.Startup))]
namespace ServerMonitoring
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);


            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

 
            if (!roleManager.RoleExists("Admin"))
            {
 
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);
             
                var user = new ApplicationUser();
                user.UserName = "info@vetech.dk";
                user.Email = "info@vetech.dk";

                string userPWD = "Weekend1519";

                var chkUser = UserManager.Create(user, userPWD);
            
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Admin");
                }
            }


          
            
        }
    }
}
