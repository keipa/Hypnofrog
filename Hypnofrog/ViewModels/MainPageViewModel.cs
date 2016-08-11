using Hypnofrog.DBModels;
using Hypnofrog.Models;
using Hypnofrog.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hypnofrog.ViewModels
{
    public class MainPageViewModel
    {
        public List<SiteViewModel> Sites { get; set; }
        public ApplicationUser FirstUser { get; set; }
        public string FirstAvatar { get; set; }
        public string Tags { get; set; }

        public MainPageViewModel() { }

        public MainPageViewModel(string currentuser, bool isadmin)
        {
            FirstUser = MainService.GetTopUser();
            FirstAvatar = MainService.GetTopUserAvatar();
            Sites = MainService.FromSitesToVM(MainService.GetTopThreeSites(), currentuser, isadmin).ToList();
            Tags = MainService.GetMainTags();
        }
    }
}