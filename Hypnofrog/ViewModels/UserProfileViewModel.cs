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
        public string Email { get; set; }
        public double Rate { get; set; }
        public int AchivmentsCount { get; set; }
        public List<Site> Sites { get; set; }

        public UserProfileViewModel() { }

        public UserProfileViewModel(string username)
        {
            ApplicationUser user = MainService.GetUserByName(username);
            Avatar = MainService.GetUserAvatar(user);
            Email = user.Email;
            AchivmentsCount = MainService.GetUserAchivments(user).Count();
            Sites = MainService.GetUserSites(user);
            Rate = MainService.GetRate(Sites);
        }
    }
}