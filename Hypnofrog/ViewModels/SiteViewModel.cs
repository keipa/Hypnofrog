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

        public SiteViewModel(string username, string siteurl, bool preview, string currentuser, bool isadmin)
        {
            Site = MainService.SiteByUrlAndName(siteurl, username);
            Titles = MainService.GetSiteTitles(Site);
            Ids = MainService.GetSiteIds(Site);
            Preview = preview;
            IsAdmin = MainService.IsAdmin(isadmin, currentuser, username);
            Layout = MainService.GetSiteLayout(Site);
            Pages = MainService.GenerateSitePages(Site, isadmin, currentuser, username);
            foreach(var page in Pages)
            {
                page.SiteUrl = siteurl;
                page.UserName = username;
            }
            var user = MainService.GetUserByName(currentuser);
            UserAvatar = MainService.GetUserAvatar(user);
        }

        public SiteViewModel(int siteid, string currentuser, bool isadmin)
        {
            Site = MainService.SiteById(siteid);
            Titles = MainService.GetSiteTitles(Site);
            Ids = MainService.GetSiteIds(Site);
            Preview = false;
            IsAdmin = MainService.IsAdmin(isadmin, currentuser, Site.UserId);
            Layout = MainService.GetSiteLayout(Site);
            Pages = MainService.GenerateSitePages(Site, isadmin, currentuser, Site.UserId);
            foreach (var page in Pages)
            {
                page.SiteUrl = Site.Url;
                page.UserName = Site.UserId;
            }
            var user = MainService.GetUserByName(currentuser);
            UserAvatar = MainService.GetUserAvatar(user);
        }
    }
}