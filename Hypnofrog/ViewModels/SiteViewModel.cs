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
        public int SiteId { get; set; }
        public string Title { get; set; }
        public List<string> Titles { get; set; }
        public List<int> Ids { get; set; }
        public bool Preview { get; set; }
        public bool IsAdmin { get; set; }
        public string Layout { get; set; }
        public List<PageViewModel> Pages { get; set; }
        public Avatar UserAvatar { get; set; }
        public bool HasComments { get; set; }
        public List<CommentViewModel> Comments { get; set; }

        public SiteViewModel() { }

        public SiteViewModel(string username, string siteurl, string currentuser, bool isadmin)
        {
            var site = MainService.SiteByUrlAndName(siteurl, username);
            Preview = true;
            HasComments = site.HasComments;
            if (HasComments)
            {
                foreach(var elem in site.Comments)
                {
                    Comments.Add(new CommentViewModel(elem));
                }
            }
            GetSiteSettings(this, site, isadmin, currentuser);
        }

        public SiteViewModel(int siteid, string currentuser, bool isadmin)
        {
            var site = MainService.SiteById(siteid);
            HasComments = Preview = false;
            GetSiteSettings(this, site, isadmin, currentuser);
        }

        private static void GetSiteSettings(SiteViewModel model, Site site, bool isadmin, string currentuser)
        {
            model.SiteId = site.SiteId;
            model.Title = site.Title;
            model.Titles = MainService.GetSiteTitles(site);
            model.Ids = MainService.GetSiteIds(site);
            model.IsAdmin = MainService.IsAdmin(isadmin, currentuser, site.UserId);
            model.Layout = MainService.GetSiteLayout(site);
            model.Pages = MainService.GenerateSitePages(site, isadmin, currentuser, site.UserId);
            var user = MainService.GetUserByName(currentuser);
            model.UserAvatar = MainService.GetUserAvatar(user);
        }
    }
}