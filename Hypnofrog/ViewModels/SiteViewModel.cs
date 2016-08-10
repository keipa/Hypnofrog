using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hypnofrog.Models;
using Hypnofrog.DBModels;
using Hypnofrog.Services;

namespace Hypnofrog.ViewModels
{
    public class SiteViewModel
    {
        public Site Site { get; set; }
        public List<string> Titles { get; set; }
        public List<int> Ids { get; set; }
        public bool Preview { get; set; }
        public bool IsAdmin { get; set; }
        public string Layout { get; set; }
        public List<PageViewModel> Pages { get; set; }
        public Avatar UserAvatar { get; set; }

        public SiteViewModel() { }

        public SiteViewModel(string username, string siteurl, string currentuser, bool isadmin)
        {
            Site = MainService.SiteByUrlAndName(siteurl, username);
            Preview = true;
            GetSiteSettings(this, Site, isadmin, currentuser);
        }

        public SiteViewModel(int siteid, string currentuser, bool isadmin)
        {
            Site = MainService.SiteById(siteid);
            Preview = false;
            GetSiteSettings(this, Site, isadmin, currentuser);
        }

        private static void GetSiteSettings(SiteViewModel model, Site site, bool isadmin, string currentuser)
        {
            model.Titles = MainService.GetSiteTitles(site);
            model.Ids = MainService.GetSiteIds(site);
            model.IsAdmin = MainService.IsAdmin(isadmin, currentuser, site.UserId);
            model.Layout = MainService.GetSiteLayout(site);
            model.Pages = MainService.GenerateSitePages(site, isadmin, currentuser, site.UserId);
            foreach (var page in model.Pages)
            {
                page.SiteUrl = site.Url;
                page.UserName = site.UserId;
            }
            var user = MainService.GetUserByName(currentuser);
            model.UserAvatar = MainService.GetUserAvatar(user);
        }
    }
}