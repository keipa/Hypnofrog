using Hypnofrog.DBModels;
using Hypnofrog.Models;
using Hypnofrog.Services;
using System.Collections.Generic;
using System.Linq;

namespace Hypnofrog.ViewModels
{
    public class UserProfileViewModel
    {
        public Avatar Avatar { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public double Rate { get; private set; }
        public int AchivmentsCount { get; private set; }
        public IEnumerable<SiteViewModel> Sites { get; private set; }

        public UserProfileViewModel() { }

        public UserProfileViewModel(string username, bool isadmin)
        {
            ApplicationUser user = MainService.GetUserByName(username);
            Avatar = MainService.GetUserAvatar(user);
            Email = user.Email;
            Name = user.UserName;
            AchivmentsCount = MainService.GetUserAchivments(user).Count();
            var sites = MainService.GetUserSites(user);
            Sites = MainService.FromSitesToVm(sites, Name, isadmin);
            Rate = MainService.GetRate(sites);
        }
    }
}