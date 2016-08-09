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
        public List<Site> Sites { get; set; }
        public ApplicationUser FirstUser { get; set; }
        public string FirstAvatar { get; set; }
        public string Tags { get; set; }

        public MainPageViewModel()
        {
            FirstUser = MainService.GetTopUser();
            FirstAvatar = MainService.GetTopUserAvatar();
            Sites = MainService.GetTopThreeSites();
            Tags = MainService.GetMainTags();
        }
    }
}