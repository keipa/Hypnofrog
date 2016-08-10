using Hypnofrog.DBModels;
using Hypnofrog.Models;
using Hypnofrog.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lucene.Net.Support;

namespace Hypnofrog.ViewModels
{
    public class MainPageViewModel
    {
        public List<LiteSiteViewModel> Sites { get; private set; }
        public string TopUserName { get; private set; }
        public string FirstAvatar { get; private set; }
        public string Tags { get; private set; }

        public MainPageViewModel()
        {
            //TopUserName = MainService.GetTopUser().UserName;
            TopUserName = "qwertyADMIN";
            FirstAvatar = MainService.GetTopUserAvatar();
            Sites = ConvertSitesToLite(MainService.GetTopThreeSites());
            Tags = MainService.GetMainTags();
        }

        private static List<LiteSiteViewModel> ConvertSitesToLite(IEnumerable<Site> sites)
        {
            List<LiteSiteViewModel> litelist = new EquatableList<LiteSiteViewModel>();
            litelist.AddRange(sites.Select(site => new LiteSiteViewModel(site)));
            return litelist;
        }


    }


}