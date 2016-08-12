using Hypnofrog.DBModels;
using Hypnofrog.Models;
using Hypnofrog.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hypnofrog.ViewModels
{
    public class UserProfileViewModel
    {
        public Avatar Avatar { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public double Rate { get; set; }
        public int AchivmentsCount { get; set; }
        public IEnumerable<SiteViewModel> Sites { get; set; }

        public UserProfileViewModel() { }

        public UserProfileViewModel(string username, bool isadmin)
        {
            ApplicationUser user = MainService.GetUserByName(username);
            Avatar = MainService.GetUserAvatar(user);
            Email = user.Email;
            Name = user.UserName;
            AchivmentsCount = MainService.GetUserAchivments(user).Count();
            var sites = MainService.GetUserSites(user);
            Sites = MainService.FromSitesToVM(sites, Name, isadmin);
            Rate = MainService.GetRate(sites);
        }
    }
}