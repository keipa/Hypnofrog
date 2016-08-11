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
using System.Web.Mvc;

namespace Hypnofrog.Services
{
    public class MainService
    {
        private static IRepository Repository;

        public MainService()
        {
            Repository = DependencyResolver.Current.GetService<IRepository>();
        }

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
            var site = Repository.SitesList.Where(x => x.Url == siteurl && x.UserId == username).Include(x => x.Comments).FirstOrDefault();
            var pages = Repository.PageList.Where(x => x.SiteId == site.SiteId).Include(x => x.Contents).ToList();
            site.Pages = pages;
            return site;
        }

        internal static Page GetPageById(int pageid)
        {
            return Repository.PageList.Where(x => x.PageId == pageid).Include(x => x.Contents).FirstOrDefault();
        }

        internal static Site GetSite(int? siteId)
        {
            return Repository.SitesList.Where(x => x.SiteId == siteId).FirstOrDefault();
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
            foreach (var page in site.Pages)
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

        internal static void SavePageTitleAndContent(int pageid, string title, List<string> htmlContent)
        {
            var page = GetPageById(pageid);
            var list_content = page.Contents.ToList();
            for (int i = 0; i < page.Contents.Count(); i++)
            {
                Repository.UpdateContent(list_content[i].ContentId, htmlContent[i]);
            }
        }

        public static int CreateSite(string inputstring, string username)
        {
            string[] param = CreateParam(inputstring);
            Site site = CreateSite(param, username);
            return site.SiteId;
        }

        private static Site CreateSite(string[] param, string username)
        {
            var site = new Site
            {
                CreationTime = DateTime.Now,
                HasComments = param[3] == "true",
                Title = param[0],
                Description = param[1],
                Iscomplited = false,
                MenuType = param[5],
                UserId = username,
                Rate = 0.0,
                Tags = param[7],
                Url = param[2]
            };
            Repository.CreateSite(site);
            UpdateSiteUrl(param[2], site);
            CreatePage(site.SiteId, param);
            return site;
        }

        private static void UpdateSiteUrl(string url, Site site)
        {
            if (url == "" || url != site.Url)
            {
                site.Url = url == "" ? site.SiteId.ToString() : url;
                Repository.UpdateSite(site);
            }
        }

        public static bool CreatePage(string inputData, int siteid)
        {
            string[] values = CreatePageParam(inputData);
            Page page = new Page() { Color = values[1], TemplateType = values[2], SiteId = siteid, Title = values[0] };
            if (!Repository.CreatePage(page))
                return false;
            int count = page.TemplateType == "mixed" ? 3 : page.TemplateType == "solid" ? 1 : 2;
            return CreateContent(count, page.PageId);
        }

        private static string[] CreatePageParam(string inputData)
        {
            string[] values = inputData.Split(';');
            values[0] = values[0] == "" ? "Page Title" : values[0];
            return values;
        }

        internal static bool SiteConfirm(int siteid, SettingsModel sitevm)
        {
            Site site = GetSite(siteid);
            site.Title = sitevm.Name;
            site.Description = sitevm.Description;
            site.HasComments = sitevm.CommentsAvailable;
            site.Url = sitevm.Url == "" ? siteid.ToString() : sitevm.Url;
            site.Tags = sitevm.CurrentTags;
            return Repository.UpdateSite(site);
        }

        internal static bool RemoveSite(int siteid)
        {
            Site site = GetSite(siteid);
            var pages = site.Pages.ToList();
            foreach (var page in pages)
            {
                var contents = page.Contents.ToList();
                foreach (var content in contents)
                {
                    if (!Repository.RemoveContent(content.ContentId))
                        return false;
                }
                if (!Repository.RemovePage(page.PageId))
                    return false;
            }
            var comments = site.Comments.ToList();
            foreach(var comment in comments)
            {
                if (!Repository.RemoveComment(comment.CommentId))
                    return false;
            }
            return Repository.RemoveSite(siteid);
        }

        internal static int DeletePageOrSite(int pageid)
        {
            var page = GetPageById(pageid);
            int count = Repository.PageList.Where(x => x.SiteId == page.SiteId).Count();
            if (count > 1)
            {
                int siteid = (int)page.SiteId;
                RemovePage(page);
                return siteid;
            }
            else
            {
                RemoveSite((int)page.SiteId);
                return 0;
            }
        }

        internal static bool RemoveUser(string id)
        {
            return Repository.RemoveUsers(id);
        }

        internal static void RemovePage(Page page)
        {
            var contents = page.Contents.ToList();
            foreach (var content in contents)
            {
                Repository.RemoveContent(content.ContentId);
            }
            Repository.RemovePage(page.PageId);
        }

        internal static bool DownInRole(string id)
        {
            return Repository.UserDownInRole(id);
        }

        internal static bool UpInRole(string id)
        {
            return Repository.UserUpInRole(id);
        }

        internal static List<UserView> GetAllUsers()
        {
            var list_of_users = Repository.UsersList;
            return UserView.GetUserViews(list_of_users).ToList();
        }

        private static void AddOrUpdateTags(string newtags)
        {
            foreach (var item in newtags.Split(','))
            {
                Repository.CreateTag(new Tag() { Name = item, Repeats = 1 });
            }
        }

        private static void CreatePage(int siteId, string[] param)
        {
            var page = new Page()
            {
                SiteId = siteId,
                Color = param[4],
                TemplateType = param[6],
                Title = "Page Title"
            };
            Repository.CreatePage(page);
            int count = page.TemplateType == "mixed" ? 3 : page.TemplateType == "solid" ? 1 : 2;
            CreateContent(count, page.PageId);
        }

        private static bool CreateContent(int count, int pageId)
        {
            for (int i = 0; i < count; i++)
            {
                var content = new Content() { HtmlContent = "", PageId = pageId };
                if (!Repository.CreateContent(content))
                    return false;
            }
            return true;
        }

        private static string[] CreateParam(string inputdata)
        {
            string[] param = inputdata.Split(';');
            param[0] = param[0] == "" ? "Мой сайт" : param[0];
            param[1] = param[1] == "" ? "Описание сайта" : param[1];
            return param;
        }

        internal static string GetSiteMenu(int siteid)
        {
            var site = Repository.SitesList.Where(x => x.SiteId == siteid).FirstOrDefault();
            if (site == null) throw new HttpException(404, "Some invalid information.");
            return site.MenuType;
        }

        public static List<Achievement> GetUserAchivments(ApplicationUser user)
        {
            return Repository.AchievementList.Where(x => x.User == user.Id).ToList();
        }

        public static List<Site> GetUserSites(ApplicationUser user)
        {
            return Repository.SitesList.Where(x => x.UserId == user.UserName).Include(x => x.Pages).ToList();
        }

        public static double GetRate(List<Site> sites)
        {
            double overall = 0, count = 0;
            foreach (var elem in sites)
            {
                if (elem.Rate != 0)
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
                return tagstring.Remove(tagstring.Length - 1);
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