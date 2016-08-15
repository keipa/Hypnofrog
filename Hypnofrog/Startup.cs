using Hypnofrog.DBModels;
using Hypnofrog.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Hypnofrog.Startup))]
namespace Hypnofrog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRolesandUsers();
            app.MapSignalR();

        }

        private void CreateRolesandUsers()
        {
            var context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            if (!roleManager.RoleExists("Admin"))
            {
                var role = new IdentityRole {Name = "Admin"};
                roleManager.Create(role);
                var user = new ApplicationUser
                {
                    UserName = "qwertyADMIN",
                    Email = "qwerty@gmail.com",
                    EmailConfirmed = true
                };
                string userPWD = "102938kek";
                var chkUser = userManager.Create(user, userPWD);
                if (chkUser.Succeeded)
                {
                    userManager.AddToRole(user.Id, "Admin");
                    context.Avatars.Add(new Avatar() { Path = "https://pp.vk.me/c637127/v637127185/26b3/d6xhDAEYvW8.jpg", UserId = user.UserName });
                    context.SaveChanges();
                }
            }
            if (!roleManager.RoleExists("Anonymous"))
            {
                var role = new IdentityRole {Name = "Anonymous"};
                roleManager.Create(role);
            }
            if (!roleManager.RoleExists("User"))
            {
                var role = new IdentityRole {Name = "User"};
                roleManager.Create(role);
            }
        }
    }
}
