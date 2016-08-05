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
            createRolesandUsers();
        }

        private void createRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            if (!roleManager.RoleExists("Admin"))
            { 
                var role = new IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);
                var user = new ApplicationUser();
                user.UserName = "qwerty@gmail.com";
                user.Email = "qwerty@gmail.com";
                user.EmailConfirmed = true;
                string userPWD = "102938kek";
                var chkUser = UserManager.Create(user, userPWD);
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Admin");
                    using(var db = new Context())
                    {
                        db.Avatars.Add(new Avatar() { Path = "https://pp.vk.me/c637127/v637127185/26b3/d6xhDAEYvW8.jpg", UserId = user.Email});
                        db.SaveChanges();
                    }
                }
            }
            if (!roleManager.RoleExists("Anonymous"))
            {
                var role = new IdentityRole();
                role.Name = "Anonymous";
                roleManager.Create(role);
            }
            if (!roleManager.RoleExists("User"))
            {
                var role = new IdentityRole();
                role.Name = "User";
                roleManager.Create(role);
            }
        }
    }
}
