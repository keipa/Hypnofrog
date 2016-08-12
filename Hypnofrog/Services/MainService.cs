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
using System.Text.RegularExpressions;
using Hypnofrog.SearchLucene;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.IO;

namespace Hypnofrog.Services
{
    public class MainService
    {
        private static IRepository Repository;

        public MainService()
        {
            Repository = DependencyResolver.Current.GetService<IRepository>();
        }

        internal static List<CommentViewModel> SearchComments(string searchString)
        {
            var site_searcher = new SearchComments();
            site_searcher.ClearLuceneIndex();
            site_searcher.AddUpdateLuceneIndex(Repository.CommentList.ToList());
            return site_searcher.Search(searchString).ToList();
        }

        internal static List<SiteViewModel> FromContentToSites(List<Content> contents, string currentuser, bool isadmin)
        {
            List<SiteViewModel> list = new List<SiteViewModel>();
            foreach (var elem in contents)
            {
                list.Add(new SiteViewModel(GetSiteIdByContent(elem.ContentId), currentuser, isadmin));
            }
            return list;
        }

        internal static List<LiteSiteViewModel> FromContentToLiteSites(List<Content> contents, string currentuser, bool isadmin)
        {
            List<LiteSiteViewModel> list = new List<LiteSiteViewModel>();
            foreach (var elem in contents)
            {
                list.Add(new LiteSiteViewModel(GetSiteByContent(elem.ContentId)));
            }
            return list;
        }

        private static Site GetSiteByContent(int contentId)
        {
            var pageid = Repository.ContentList.FirstOrDefault(x => x.ContentId == contentId).PageId;
            return Repository.PageList.FirstOrDefault(x => x.PageId == pageid).Site;
        }

        private static int GetSiteIdByContent(int contentId)
        {
            var pageid = Repository.ContentList.FirstOrDefault(x => x.ContentId == contentId).PageId;
            return (int)Repository.PageList.FirstOrDefault(x => x.PageId == pageid).SiteId;
        }

        internal static List<Content> SearchContent(string searchString)
        {
            var site_searcher = new SearchContent();
            site_searcher.ClearLuceneIndex();
            site_searcher.AddUpdateLuceneIndex(Repository.ContentList.ToList());
            return site_searcher.Search(searchString).ToList();
        }

        internal static List<UserView> SearchUsers(string searchString)
        {
            var site_searcher = new SearchUsers();
            site_searcher.ClearLuceneIndex();
            site_searcher.AddUpdateLuceneIndex(Repository.UsersList.ToList());
            return site_searcher.Search(searchString).ToList();
        }

        //internal static List<SiteViewModel> SearchSites(string searchString, string currentuser, bool isadmin)
        //{
        //    var site_searcher = new SearchSites();
        //    site_searcher.ClearLuceneIndex();
        //    site_searcher.AddUpdateLuceneIndex(Repository.SitesList.ToList());
        //    return site_searcher.Search(searchString, currentuser, isadmin).ToList();
        //}

        internal static List<LiteSiteViewModel> SearchLiteSites(string searchString, string currentuser, bool isadmin)
        {
            var site_searcher = new SearchSites();
            site_searcher.ClearLuceneIndex();
            site_searcher.AddUpdateLuceneIndex(Repository.SitesList.ToList());
            return site_searcher.Search(searchString, currentuser, isadmin).ToList();
        }

        internal static IEnumerable<SiteViewModel> FromSitesToVM(List<Site> sites, string currentuser, bool isadmin)
        {
            List<SiteViewModel> viewmodels = new List<SiteViewModel>();
            foreach (var elem in sites)
                viewmodels.Add(new SiteViewModel(elem.SiteId, currentuser, isadmin));
            return viewmodels;
        }

        public static ApplicationUser GetTopUser()
        {
            return _GetTopUser();
        }


        public static string GetTopUserAvatar()
        {
            var topuser = _GetTopUser();
            var avatar = Repository.AvatarList.FirstOrDefault(x => x.UserId == topuser.UserName);
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
            var site = Repository.SitesList.Where(x => x.SiteId == siteId).Include(x => x.Comments).FirstOrDefault();
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
            var titles = site.Pages.Select(x => x.Title).ToList();
            List<string> valid_titles = new List<string>();
            foreach (var elem in titles)
                valid_titles.Add(Regex.Replace(elem ?? "Empty", "<[^>]+>", string.Empty));
            return valid_titles;
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

        public static Avatar GetUserAvatar(string username)
        {
            return Repository.AvatarList.Where(x => x.UserId == username).FirstOrDefault();
        }

        internal static void SavePageTitleAndContent(int pageid, string title, List<string> htmlContent)
        {
            var page = GetPageById(pageid);
            page.Title = title;
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

        internal static bool RemoveSite(Site site)
        {
            return _RemoveSite(site);
        }

        internal static bool RemoveSite(int siteid)
        {
            Site site = GetSite(siteid);
            return _RemoveSite(site);
        }

        internal static Dictionary<string, bool> GetKeyValueAchievments(List<string> allachievments, List<string> alldesc, List<string> userachievments)
        {
            Dictionary<string, bool> achievments = new Dictionary<string, bool>();
            for (int i = 0; i < allachievments.Count() - 1; i++)
                if (userachievments.Contains(allachievments[i]))
                    achievments.Add(allachievments[i] + ":" + alldesc[i], true);
                else
                    achievments.Add(allachievments[i] + ":" + alldesc[i], false);
            return achievments;
        }

        internal static List<CommentViewModel> GetSiteComments(int siteid)
        {
            List<CommentViewModel> model = new List<CommentViewModel>();
            var site = Repository.SitesList.Where(x => x.SiteId == siteid).Include(x => x.Comments).FirstOrDefault();
            foreach(var elem in site.Comments)
            {
                model.Add(new CommentViewModel(elem));
            }
            return model;
        }

        internal static bool DeleteComment(int comid)
        {
            return Repository.RemoveComment(comid);
        }

        internal static void SaveAvatar(out string fName, HttpPostedFileBase file, string username, DirectoryInfo info)
        {
            string path;
            ConfigureAvatarSaving(out fName, file, out path, info);
            Cloudinary cloudinary = new Cloudinary(new Account("dldmfb5fo", "568721824454478", "ZO4nwcMQwcT88lUNUK5KHJmy_fU"));
            var param = new ImageUploadParams()
            {
                File = new FileDescription(path)
            };
            var result = cloudinary.Upload(param);
            SaveAvatarToDatabase(result, username);
        }

        internal static bool SaveNewComment(string newComment, int siteid, string username)
        {
            string useravatar = GetUserAvatar(username).Path;
            return Repository.CreateComment(new Comment()
            {
                CreationTime = DateTime.Now,
                SiteId = siteid,
                Text = newComment,
                UserAvatar = useravatar,
                UserId = username
            });
        }

        private static Rate GetRateForSite(string userid, string siteid)
        {
            return Repository.RateList.Where(x => x.User == userid && x.Site == siteid).FirstOrDefault();
        }

        internal static string GetRateMessage(string userid, string siteid, string value)
        {
            Rate rate = GetRateForSite(userid, siteid);
            string result = GetResultForRate(userid, siteid, value, rate);
            SaveAndcountAverage(siteid);
            return result;
        }

        private static string GetResultForRate(string userid, string siteid, string value, Rate rate)
        {
            if (rate == null)
            {
                Repository.CreateRate(new Rate() { Value = Convert.ToInt32(value), Site = siteid, User = userid });
                return "Thank you";
            }
            else if (rate.Value == Convert.ToInt32(value))
            {
                return "Updated.";
            }
            else
            {
                string res = rate.Value.ToString();
                rate.Value = Convert.ToInt32(value);
                Repository.UpdateRate(rate);
                return "Update. Last mark: " + res;
            }
        }

        private static void SaveAndcountAverage(string siteid)
        {
            
            double average = CountingAverage(siteid);
            int siteidd = Convert.ToInt32(siteid);
            var site = Repository.SitesList.Where(x => x.SiteId == siteidd).FirstOrDefault();
            site.Rate = average;
            Repository.UpdateSite(site);
        }

        private static double CountingAverage(string siteid)
        {
            var sites = Repository.RateList.Where(x => x.Site == siteid).ToList();
            double average = 0.0;
            foreach (var item in sites)
                average += (double)item.Value;
            if (sites.Count() == 0)
            {
                return average;
            }
            else
            {
                average = average / (double)sites.Count();

            }
            return average;
        }

        internal static AchievmentChecker GetAchivmentsChecker(string userid, string username)
        {
            return new AchievmentChecker(Repository.SitesList.Where(x => x.UserId == username).ToList(),
                                         Repository.RateList.Where(x => x.User == userid).OrderByDescending(x => x.Value).ToList(),
                                         Repository.AchievementList.Where(x => x.User == userid).ToList(),
                                         userid);
        }

        private static void ConfigureAvatarSaving(out string fName, HttpPostedFileBase file, out string path, DirectoryInfo info)
        {
            string pathString;
            bool isExists;
            SaveAvatarToStorage(out fName, file, out pathString, out isExists, info);
            if (!isExists) Directory.CreateDirectory(pathString);
            path = string.Format("{0}\\{1}", pathString, file.FileName);
            file.SaveAs(path);
        }

        private static void SaveAvatarToDatabase(ImageUploadResult result, string username)
        {
            var useravatar = Repository.AvatarList.Where(x => x.UserId == username).FirstOrDefault();
            if (useravatar == null)
            {
                Repository.CreateAvatar(new Avatar() { UserId = username, Path = result.Uri.AbsoluteUri });
            }
            else
            {
                useravatar.Path = result.Uri.AbsoluteUri;
                Repository.UpdateAvatar(useravatar);
            }
        }

        private static void SaveAvatarToStorage(out string fName, HttpPostedFileBase file, out string pathString, out bool isExists, DirectoryInfo info)
        {
            fName = file.FileName;
            var originalDirectory = info;
            pathString = Path.Combine(originalDirectory.ToString(), "imagepath");
            var fileName1 = Path.GetFileName(file.FileName);
            isExists = Directory.Exists(pathString);
        }

        private static bool _RemoveSite(Site site)
        {
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
            foreach (var comment in comments)
            {
                if (!Repository.RemoveComment(comment.CommentId))
                    return false;
            }
            return Repository.RemoveSite(site.SiteId);
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
            if (!ContainsUser(id))
                return false;
            if (!RemoveUserSites(id))
                return false;
            return Repository.RemoveUsers(id);
        }

        private static bool ContainsUser(string id)
        {
            var user = Repository.UsersList.Where(x => x.Id == id).FirstOrDefault();
            return user == null ? false : true;
        }

        private static bool RemoveUserSites(string id)
        {
            var user = Repository.UsersList.Where(x => x.Id == id).FirstOrDefault();
            var sites = GetUserSites(user);
            foreach (var site in sites)
            {
                if (!RemoveSite(site))
                    return false;
            }
            return true;
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

        internal static UserView GetCurrentUser(string username, bool isadmin, string userid)
        {
            return new UserView() {IsAdmin =  isadmin, UserName = username, UserId =  userid};
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

        public static List<Achievement> GetUserAchivments(string userid)
        {
            return Repository.AchievementList.Where(x => x.User == userid).ToList();
        }

        public static List<Site> GetUserSites(ApplicationUser user)
        {
            List<Site> sites = new List<Site>();
            var lsites = Repository.SitesList.Where(x => x.UserId == user.UserName).ToList();
            foreach (var site in lsites)
                sites.Add(GetSite(site.SiteId));
            return sites;
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
            string userid = "";
            var sites = Repository.SitesList.OrderByDescending(x => x.Rate).FirstOrDefault();
            if (sites == null)
                return Repository.UsersList.Where(x => x.UserName == "qwertyADMIN").FirstOrDefault();
            else
                userid = sites.UserId;
            var user = Repository.UsersList.Where(x => x.UserName == userid).FirstOrDefault();
            return user ?? Repository.UsersList.Where(x => x.UserName == "qwertyADMIN").FirstOrDefault();
        }
    }
}