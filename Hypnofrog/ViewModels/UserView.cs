using Hypnofrog.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Linq;

namespace Hypnofrog.ViewModels
{
    public class UserView
    {
        public string UserName { get; set; }
        public string UserAvatar { get; set; }
        public string UserId { get; set; }
        public double Rating { get; set; }
        public bool IsAdmin { get; set; }

        public UserView() { }

        public UserView(ApplicationUser user, string avatarpath, double rating, bool isadmin)
        {
            UserName = user.UserName;
            UserId = user.Id;
            UserAvatar = avatarpath;
            Rating = rating;
            IsAdmin = isadmin;
        }

        public static IEnumerable<UserView> GetUserViews(IEnumerable<ApplicationUser> users)
        {
            var result = new List<UserView>();
                using (var udb = new ApplicationDbContext())
                {
                    var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(udb));
                    foreach (var user in users)
                        try
                        {
                            user.Id = udb.Users.FirstOrDefault(x => x.UserName == user.UserName)?.Id;
                            result.Add(new UserView(user, udb.Avatars.FirstOrDefault(x => x.UserId == user.UserName)?.Path, GetProfilerRate(udb, user.UserName), userManager.IsInRole(user.Id, "Admin")));
                        }
                        catch
                        {
                            result.Add(new UserView(user, udb.Avatars.FirstOrDefault(x => x.UserId == user.UserName)?.Path, GetProfilerRate(udb, user.UserName), false));
                        }
                }
            return result;
        }

        private static double GetProfilerRate(ApplicationDbContext db, string username)
        {
            var sites = db.Sites.Where(x => x.UserId == username).ToList();
            var average = sites.Sum(item => item.Rate);
            if (!sites.Any()) return 0.0;
            average = average / sites.Count;
            return average;
        }
    }
}