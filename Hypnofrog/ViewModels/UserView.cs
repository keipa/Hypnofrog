using Hypnofrog.DBModels;
using Hypnofrog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hypnofrog.ViewModels
{
    public class UserView
    {
        public string UserName { get; set; }
        public string UserAvatar { get; set; }
        public string UserId { get; set; }
        public double Rating { get; set; }

        public UserView() { }

        public UserView(Models.ApplicationUser user, string avatarpath, double rating)
        {
            this.UserName = user.UserName;
            this.UserId = user.Id;
            this.UserAvatar = avatarpath;
            this.Rating = rating;
        }

        public static IEnumerable<UserView> GetUserViews(IEnumerable<Models.ApplicationUser> users)
        {
            List<UserView> result = new List<UserView>();
            using (var db = new Context())
            {

                foreach (var user in users)
                    result.Add(new UserView(user, db.Avatars.Where(x => x.UserId == user.UserName).FirstOrDefault().Path, GetProfilerRate(db, user.UserName)));
            }
            return result;
        }

        private static double GetProfilerRate(Context db, string username)
        {
            List<Site> sites = new List<Site>();
            double average = 0.0;
            sites = db.Sites.Where(x => x.UserId == username).ToList();
            foreach (var item in sites) average += item.Rate;
            if (sites.Count() == 0) return 0.0;
            average = average / sites.Count();
            return average;
        }
    }
}