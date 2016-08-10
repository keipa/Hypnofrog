using Hypnofrog.Models;
using Hypnofrog.DBModels;
using Hypnofrog.Repository;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Hypnofrog.ViewModels;
using System.Web.Security;

namespace Hypnofrog.Services
{
    public class MainService
    {
        [Inject]
        private static IRepository Repository = new MSSQLRepository();

        public static ApplicationUser GetTopUser()
        {
            return _GetTopUser();
        }


        public static string GetTopUserAvatar()
        {
            var topuser = _GetTopUser();
            var avatar = Repository.AvatarList.Where(x => x.UserId == topuser.UserName).FirstOrDefault();
            return avatar.Path;
        }

        public static string GetPageStyle(Page page)
        {
            return page.Color == "dark" ? "dark" : page.Color == "orange" ? "red" : "gray";
        }

        public static Site SiteByUrlAndName(string siteurl, string username)
        {
            var site = Repository.SitesList.Where(x => x.Url == siteurl && x.UserId == username).Include(x=>x.Comments).FirstOrDefault();
            var pages = Repository.PageList.Where(x => x.SiteId == site.SiteId).Include(x => x.Contents).ToList();
            site.Pages = pages;
            return site;
        }

        public static List<int> GetSiteIds(Site site)
        {
            return site.Pages.Select(x => x.PageId).ToList();
        }

        public static string GetSiteLayout(Site site)
        {
            return site.MenuType == "without" ? "~/Views/Shared/_LayoutWM.cshtml" :
        site.MenuType == "vertical" ? "~/Views/Shared/_LayoutVM.cshtml" : "~/Views/Shared/_Layout.cshtml";
        }

        internal static Site SiteById(int siteid)
        {
            return Repository.SitesList.Where(x => x.SiteId == siteid).FirstOrDefault();
        }

        public static List<PageViewModel> GenerateSitePages(Site site, bool isadmin, string currentuser, string user)
        {
            List<PageViewModel> pageviews = new List<PageViewModel>();
            foreach(var page in site.Pages)
            {
                pageviews.Add(new PageViewModel(page, IsAdmin(isadmin, currentuser, user)));
            }
            return pageviews;
        }

        public static bool IsAdmin(bool isadmin, string currentuser, string user)
        {
            return isadmin || currentuser == user;
        }

        public static List<string> GetSiteTitles(Site site)
        {
            return site.Pages.Select(x => x.Title).ToList();
        }

        public static ApplicationUser GetUserByName(string username)
        {
            return Repository.UsersList.Where(x => x.UserName == username).FirstOrDefault();
        }

        public static Avatar GetUserAvatar(ApplicationUser user)
        {
            if (user == null) return null;
            return Repository.AvatarList.Where(x => x.UserId == user.UserName).FirstOrDefault();
        }

        public static List<Achievement> GetUserAchivments(ApplicationUser user)
        {
            return Repository.AchievementList.Where(x => x.User == user.Id).ToList();
        }
        
        public static List<Site> GetUserSites(ApplicationUser user)
        {
            return Repository.SitesList.Where(x => x.UserId == user.UserName).Include(x=>x.Pages).ToList();
        }

        public static double GetRate(List<Site> sites)
        {
            double overall = 0, count = 0;
            foreach(var elem in sites)
            {
                if(elem.Rate != 0)
                {
                    overall += elem.Rate;
                    count++;
                }
            }
            count = count == 0 ? 1 : count;
            return overall / count;
        }

        public static List<Site> GetTopThreeSites()
        {
            var sites = Repository.SitesList.OrderByDescending(x => x.Rate).Take(3).Include(x => x.Pages).ToList();
            return sites;
        }

        public static string GetMainTags()
        {
            string tagstring = "";
            foreach (var item in Repository.TagList.OrderByDescending(x => x.Repeats).ToList())
                tagstring += item.Name + "," + item.Repeats.ToString() + ";";
            try
            {
                return tagstring.Remove(tagstring.Length - 1); ;
            }
            catch (Exception)
            {
                return tagstring;
            }
        }

        private static ApplicationUser _GetTopUser()
        {
            var userid = Repository.SitesList.OrderByDescending(x => x.Rate).FirstOrDefault().UserId;
            return Repository.UsersList.Where(x => x.UserName == userid).FirstOrDefault();
        }
    }
}