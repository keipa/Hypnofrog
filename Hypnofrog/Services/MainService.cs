using Hypnofrog.Models;
using Hypnofrog.DBModels;
using Hypnofrog.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Hypnofrog.ViewModels;
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

        public MainService(IRepository repository)
        {
            Repository = repository;
        }

        internal static List<CommentViewModel> SearchComments(string searchString)
        {
            var siteSearcher = new SearchComments();
            siteSearcher.ClearLuceneIndex();
            siteSearcher.AddUpdateLuceneIndex(Repository.CommentList.ToList());
            return siteSearcher.Search(searchString).ToList();
        }

        internal static IEnumerable<LiteSiteViewModel> FromContentToLiteSites(IEnumerable<Content> contents, string currentuser, bool isadmin)
        {
            return contents.Select(elem => new LiteSiteViewModel(GetSiteByContent(elem.ContentId))).ToList();
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

        internal static IEnumerable<Content> SearchContent(string searchString)
        {
            var siteSearcher = new SearchContent();
            siteSearcher.ClearLuceneIndex();
            siteSearcher.AddUpdateLuceneIndex(Repository.ContentList.ToList());
            return siteSearcher.Search(searchString).ToList();
        }

        internal static List<UserView> SearchUsers(string searchString)
        {
            var siteSearcher = new SearchUsers();
            siteSearcher.ClearLuceneIndex();
            siteSearcher.AddUpdateLuceneIndex(Repository.UsersList.ToList());
            return siteSearcher.Search(searchString).ToList();
        }

        internal static List<LiteSiteViewModel> SearchLiteSites(string searchString, string currentuser, bool isadmin)
        {
            var siteSearcher = new SearchSites();
            siteSearcher.ClearLuceneIndex();
            siteSearcher.AddUpdateLuceneIndex(Repository.SitesList.ToList());
            return siteSearcher.Search(searchString, currentuser, isadmin).ToList();
        }

        internal static IEnumerable<SiteViewModel> FromSitesToVm(IEnumerable<Site> sites, string currentuser, bool isadmin)
        {
            return sites.Select(elem => new SiteViewModel(elem.SiteId, currentuser, isadmin)).ToList();
        }

        public static string GetTopUserAvatar()
        {
            var topuser = _GetTopUser();
            var avatar = Repository.AvatarList.FirstOrDefault(x => x.UserId == topuser.UserName);
            return avatar?.Path;
        }

        public static string GetPageStyle(Page page)
        {
            return page.Color == "dark" ? "dark" : page.Color == "orange" ? "red" : "gray";
        }

        public static Site SiteByUrlAndName(string siteurl, string username)
        {
            var site = Repository.SitesList.Where(x => x.Url == siteurl && x.UserId == username).Include(x => x.Comments).FirstOrDefault();
            var pages = Repository.PageList.Where(x => x.SiteId == site.SiteId).Include(x => x.Contents).ToList();
            if (site != null) site.Pages = pages;
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
            if (site != null) site.Pages = pages;
            return site;
        }

        public static List<int> GetSiteIds(Site site)
        {
            return site.Pages.Select(x => x.PageId).ToList();
        }

        public static string GetSiteLayout(Site site)
        {
            return site.MenuType == "without" ? Strings.Creditals.LayoutWM :
        site.MenuType == "vertical" ? Strings.Creditals.LayoutVM : Strings.Creditals.LayoutHM;
        }

        internal static Site SiteById(int siteid)
        {
            return Repository.SitesList.FirstOrDefault(x => x.SiteId == siteid);
        }

        public static List<PageViewModel> GenerateSitePages(Site site, bool isadmin, string currentuser, string user, bool preview)
        {
            return site.Pages.Select(page => new PageViewModel(page, IsAdmin(isadmin, currentuser, user), preview)).ToList();
        }

        public static bool IsAdmin(bool isadmin, string currentuser, string user)
        {
            return isadmin || currentuser == user;
        }

        public static List<string> GetSiteTitles(Site site)
        {
            var titles = site.Pages.Select(x => x.Title).ToList();
            return titles.Select(elem => Regex.Replace(elem ?? "Empty", "<[^>]+>", string.Empty)).ToList();
        }

        public static ApplicationUser GetUserByName(string username)
        {
            return Repository.UsersList.FirstOrDefault(x => x.UserName == username);
        }

        public static Avatar GetUserAvatar(ApplicationUser user)
        {
            return user == null ? null : Repository.AvatarList.FirstOrDefault(x => x.UserId == user.UserName);
        }

        private static Avatar GetUserAvatar(string username)
        {
            return Repository.AvatarList.FirstOrDefault(x => x.UserId == username);
        }

        internal static void SavePageTitleAndContent(int pageid, string title, List<string> htmlContent)
        {
            var page = GetPageById(pageid);
            page.Title = title;
            var listContent = page.Contents.ToList();
            for (var i = 0; i < page.Contents.Count; i++)
            {
                Repository.UpdateContent(listContent[i].ContentId, htmlContent[i]);
            }
        }

        public static int CreateSite(string inputstring, string username)
        {
            var param = CreateParam(inputstring);
            var site = CreateSite(param, username);
            return site.SiteId;
        }

        private static Site CreateSite(string[] param, string username)
        {
            var site = new Site
            {
                CreationTime = DateTime.Now,
                HasComments = param[3] == "true",
                Title = param[0] == "" ? "My site" : param[0],
                Description = param[1] == "" ? "Description" : param[1],
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
            AddOrUpdateTags(param[7]);
            return site;
        }

        private static void UpdateSiteUrl(string url, Site site)
        {
            if (url != "" && url == site.Url && url != null) return;
            site.Url = string.IsNullOrEmpty(url) ? site.SiteId.ToString() : url;
            Repository.UpdateSite(site);
        }

        public static bool CreatePage(string inputData, int siteid)
        {
            var values = CreatePageParam(inputData);
            var page = new Page() { Color = values[1], TemplateType = values[2], SiteId = siteid, Title = values[0] };
            if (!Repository.CreatePage(page))
                return false;
            var count = page.TemplateType == "mixed" ? 3 : page.TemplateType == "solid" ? 1 : 2;
            return CreateContent(count, page.PageId);
        }

        private static string[] CreatePageParam(string inputData)
        {
            var values = inputData.Split(';');
            values[0] = values[0] == "" ? "Page Title" : values[0];
            return values;
        }

        internal static bool SiteConfirm(int siteid, SettingsModel sitevm)
        {
            var site = GetSite(siteid);
            site.Title = sitevm.Name;
            site.Description = sitevm.Description;
            site.HasComments = sitevm.CommentsAvailable;
            site.Url = sitevm.Url == "" ? siteid.ToString() : sitevm.Url;
            site.Tags = sitevm.CurrentTags;
            return Repository.UpdateSite(site);
        }

        private static bool RemoveSite(Site site)
        {
            return _RemoveSite(site);
        }

        internal static bool RemoveSite(int siteid)
        {
            var site = GetSite(siteid);
            return _RemoveSite(site);
        }

        internal static Dictionary<string, bool> GetKeyValueAchievments(List<string> allachievments, List<string> alldesc, List<string> userachievments)
        {
            var achievments = new Dictionary<string, bool>();
            for (var i = 0; i < allachievments.Count - 1; i++)
                achievments.Add(allachievments[i] + ":" + alldesc[i], userachievments.Contains(allachievments[i]));
            return achievments;
        }

        internal static List<CommentViewModel> GetSiteComments(int siteid)
        {
            var site = Repository.SitesList.Where(x => x.SiteId == siteid).Include(x => x.Comments).FirstOrDefault();
            return site?.Comments.Select(elem => new CommentViewModel(elem)).OrderByDescending(x => x.CreationTime).ToList();
        }

        internal static bool DeleteComment(int comid)
        {
            return Repository.RemoveComment(comid);
        }

        internal static void SaveAvatar(out string fName, HttpPostedFileBase file, string username, DirectoryInfo info)
        {
            string path;
            ConfigureAvatarSaving(out fName, file, out path, info);
            var cloudinary = new Cloudinary(new Account(Strings.Creditals.CloudinaryAccount, Strings.Creditals.CloudinaryPassword, Strings.Creditals.CloudinaryHash));
            var param = new ImageUploadParams()
            {
                File = new FileDescription(path)
            };
            var result = cloudinary.Upload(param);
            SaveAvatarToDatabase(result, username);
        }

        internal static int CreateSiteWithTemplate(SettingsModel model, string username)
        {
            var site = new Site
            {
                CreationTime = DateTime.Now,
                HasComments = model.CommentsAvailable,
                Title = model.Name ?? "My site",
                Description = model.Description ?? "My site",
                Iscomplited = false,
                MenuType = model.Menu,
                UserId = username,
                Rate = 0.0,
                Tags = model.CurrentTags,
                Url = model.SiteUrl
            };
            Repository.CreateSite(site);
            var templ = new OwnTemplate() { CreationTime = DateTime.Now, HtmlRealize = model.OwnTemplate, UserName = username };
            Repository.CreateTemplate(templ);
            UpdateSiteUrl(model.SiteUrl, site);
            CreatePageWithTemlate(site.SiteId, model, templ);
            AddOrUpdateTags(model.CurrentTags);
            return site.SiteId;
        }

        internal static TemplateViewModel GetTemplate(int templid, List<Content> contents, bool preview)
        {
            var template = new TemplateViewModel(Repository.OwnTemplates.FirstOrDefault(x => x.OwnTemplateId == templid));
            template.HtmlTable = preview ? ReCreateTablePreview(template.HtmlTable, contents) : ReCreateTable(template.HtmlTable, contents);
            return template;
        }

        private static string ReCreateTablePreview(string ownTemplate, List<Content> contents)
        {
            ownTemplate = ownTemplate.Replace("<br>", "");
            var count = new Regex("{c{o{n{t}e}n}t}").Matches(ownTemplate).Count;
            for (var i = 0; i < count; i++)
            {
                ownTemplate = new Regex("{c{o{n{t}e}n}t}").Replace(ownTemplate, contents[i].HtmlContent, 1);
            }
            return ownTemplate;
        }

        private static string ReCreateTable(string ownTemplate, List<Content> contents)
        {
            ownTemplate = ownTemplate.Replace("<br>", "");
            var count = new Regex("{c{o{n{t}e}n}t}").Matches(ownTemplate).Count;
            for (var i = 0; i < count; i++)
            {
                ownTemplate = new Regex("{c{o{n{t}e}n}t}").Replace(ownTemplate, "<textarea class=\"test\" name=\"HtmlContent[" + i + "]\">"+contents[i].HtmlContent+"</textarea>", 1);
            }
            return ownTemplate;
        }

        private static bool CreatePageWithTemlate(int siteId, SettingsModel model, OwnTemplate template)
        {
            var page = new Page() { Color = model.Color, TemplateType = template.OwnTemplateId.ToString(), SiteId = siteId, Title = model.Name ?? "Page Title" };
            if (!Repository.CreatePage(page))
                return false;
            template.PageId = page.PageId;
            Repository.UpdateTemplate(template);
            var count = new Regex("</td>").Matches(template.HtmlRealize).Count;
            return CreateContent(count, page.PageId);
        }

        internal static bool SaveNewComment(string newComment, int siteid, string username)
        {
            var useravatar = GetUserAvatar(username).Path;
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
            return Repository.RateList.FirstOrDefault(x => x.User == userid && x.Site == siteid);
        }

        internal static string GetRateMessage(string userid, string siteid, string value)
        {
            var rate = GetRateForSite(userid, siteid);
            var result = GetResultForRate(userid, siteid, value, rate);
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
            if (rate.Value == Convert.ToInt32(value))
            {
                return "Updated.";
            }
            var res = rate.Value.ToString();
            rate.Value = Convert.ToInt32(value);
            Repository.UpdateRate(rate);
            return "Update. Last mark: " + res;
        }

        private static void SaveAndcountAverage(string siteid)
        {
            var average = CountingAverage(siteid);
            var siteidd = Convert.ToInt32(siteid);
            var site = Repository.SitesList.FirstOrDefault(x => x.SiteId == siteidd);
            if (site == null) return;
            site.Rate = average;
            Repository.UpdateSite(site);
        }

        private static double CountingAverage(string siteid)
        {
            var sites = Repository.RateList.Where(x => x.Site == siteid).ToList();
            var average = sites.Sum(item => (double) item.Value);
            if (!sites.Any())
            {
                return average;
            }
            average = average / sites.Count;
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
            path = $"{pathString}\\{file.FileName}";
            file.SaveAs(path);
        }

        private static void SaveAvatarToDatabase(UploadResult result, string username)
        {
            var useravatar = Repository.AvatarList.FirstOrDefault(x => x.UserId == username);
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
            isExists = Directory.Exists(pathString);
        }

        private static bool _RemoveSite(Site site)
        {
            var pages = site.Pages.ToList();
            foreach (var page in pages)
            {
                var contents = page.Contents.ToList();
                if (contents.Any(content => !Repository.RemoveContent(content.ContentId)))
                {
                    return false;
                }
                if (!Repository.RemovePage(page.PageId))
                    return false;
            }
            var comments = site.Comments.ToList();
            return comments.All(comment => Repository.RemoveComment(comment.CommentId)) && Repository.RemoveSite(site.SiteId);
        }

        internal static int DeletePageOrSite(int pageid)
        {
            var page = GetPageById(pageid);
            var count = Repository.PageList.Count(x => x.SiteId == page.SiteId);
            if (count > 1)
            {
                var siteid = (int)page.SiteId;
                RemovePage(page);
                return siteid;
            }
            RemoveSite((int)page.SiteId);
            return 0;
        }

        internal static bool RemoveUser(string id)
        {
            if (!ContainsUser(id))
                return false;
            return RemoveUserSites(id) && Repository.RemoveUsers(id);
        }

        private static bool ContainsUser(string id)
        {
            var user = Repository.UsersList.FirstOrDefault(x => x.Id == id);
            return user != null;
        }

        private static bool RemoveUserSites(string id)
        {
            var user = Repository.UsersList.FirstOrDefault(x => x.Id == id);
            var sites = GetUserSites(user);
            return sites.All(RemoveSite);
        }

        private static void RemovePage(Page page)
        {
            var contents = page.Contents.ToList();
            foreach (var content in contents)
            {
                Repository.RemoveContent(content.ContentId);
            }
            var model = Repository.OwnTemplates.FirstOrDefault(x => (int)x.PageId == page.PageId);
            if (model != null) Repository.RemoveTemplate(model);
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
            var listOfUsers = Repository.UsersList;
            return UserView.GetUserViews(listOfUsers).ToList();
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

        private static void CreatePage(int siteId, IReadOnlyList<string> param)
        {
            var page = new Page()
            {
                SiteId = siteId,
                Color = param[4],
                TemplateType = param[6],
                Title = "Page Title"
            };
            Repository.CreatePage(page);
            var count = page.TemplateType == "mixed" ? 3 : page.TemplateType == "solid" ? 1 : 2;
            CreateContent(count, page.PageId);
        }

        private static bool CreateContent(int count, int pageId)
        {
            for (var i = 0; i < count; i++)
            {
                var content = new Content() { HtmlContent = "", PageId = pageId };
                if (!Repository.CreateContent(content))
                    return false;
            }
            return true;
        }

        private static string[] CreateParam(string inputdata)
        {
            var param = inputdata.Split(';');
            param[0] = param[0] == "" ? "Мой сайт" : param[0];
            param[1] = param[1] == "" ? "Описание сайта" : param[1];
            return param;
        }

        internal static string GetSiteMenu(int siteid)
        {
            var site = Repository.SitesList.FirstOrDefault(x => x.SiteId == siteid);
            if (site == null) throw new HttpException(404, "Some invalid information.");
            return site.MenuType;
        }

        public static IEnumerable<Achievement> GetUserAchivments(ApplicationUser user)
        {
            return Repository.AchievementList.Where(x => x.User == user.Id).ToList();
        }

        public static IEnumerable<Achievement> GetUserAchivments(string userid)
        {
            return Repository.AchievementList.Where(x => x.User == userid).ToList();
        }

        public static List<Site> GetUserSites(ApplicationUser user)
        {
            var lsites = Repository.SitesList.Where(x => x.UserId == user.UserName).ToList();
            return lsites.Select(site => GetSite(site.SiteId)).ToList();
        }

        public static List<SiteViewModel> GetUserSites(int siteid)
        {
            var username = Repository.SitesList.FirstOrDefault(x => x.SiteId == siteid)?.UserId;
            var lsites = Repository.SitesList.Where(x => x.UserId == username).ToList();
            return FromSitesToVm(lsites.Select(site => GetSite(site.SiteId)).ToList(), username, true).ToList();
        }

        public static double GetRate(IEnumerable<Site> sites)
        {
            double overall = 0, count = 0;
            foreach (var elem in sites)
            {
                if (elem.Rate == 0) continue;
                overall += elem.Rate;
                count++;
            }
            count = count == 0 ? 1 : count;
            return overall / count;
        }

        public static IEnumerable<Site> GetTopThreeSites()
        {
            var sites = Repository.SitesList.OrderByDescending(x => x.Rate).Take(3).Include(x => x.Pages).ToList();
            return sites;
        }

        public static string GetMainTags()
        {
            var tagstring = Repository.TagList.OrderByDescending(x => x.Repeats).ToList().Aggregate("", (current, item) => current + (item.Name + "," + item.Repeats.ToString() + ";"));
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
            var userid = "";
            var sites = Repository.SitesList.OrderByDescending(x => x.Rate).FirstOrDefault();
            if (sites == null)
                return Repository.UsersList.FirstOrDefault(x => x.UserName == "qwertyADMIN");
            else
                userid = sites.UserId;
            var user = Repository.UsersList.FirstOrDefault(x => x.UserName == userid);
            return user ?? Repository.UsersList.FirstOrDefault(x => x.UserName == "qwertyADMIN");
        }

        public static string CheckAchievments(string id, string username)
        {
            var achievments = new AchievmentChecker(Repository.SitesList.Where(x => x.UserId == username).ToList(),
                                                                                                              Repository.RateList.Where(x => x.User == id).OrderByDescending(x => x.Value).ToList(),
                                                                                                              Repository.AchievementList.Where(x => x.User == id).ToList(),
                                                                                                              id);
            SaveAchievments(achievments.NewAchievments);
            return achievments.Result;
        }

        private static void SaveAchievments(List<Achievement> log)
        {
            if (!log.Any()) return;
            foreach (var item in log)
                Repository.CreateAchievement(item);
        }
    }
}