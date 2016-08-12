using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Hypnofrog.DBModels;
using Hypnofrog.Models;
using Hypnofrog.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Web.Script.Serialization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Lucene.Net.Analysis;
using Hypnofrog.Filters;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Hypnofrog.SearchLucene;
using System.Net.Mail;
using Hypnofrog.Services;

namespace Hypnofrog.Controllers
{
    [Authorize]
    [Culture]
    public class HomeController : Controller
    {
        private MainService main { get; set; }

        public HomeController()
        {
            main = DependencyResolver.Current.GetService<MainService>();
        }

        [AllowAnonymous]
        public ActionResult ChangeCulture(string lang)
        {
            string returnUrl = Request.UrlReferrer.AbsolutePath;
            List<string> cultures = new List<string>() { "ru", "en" };
            if (!cultures.Contains(lang))
            {
                lang = "ru";
            }
            HttpCookie cookie = Request.Cookies["lang"];
            if (cookie != null)
                cookie.Value = lang;
            else
            {
                cookie = new HttpCookie("lang");
                cookie.HttpOnly = false;
                cookie.Value = lang;
                cookie.Expires = DateTime.Now.AddYears(1);
            }
            Response.Cookies.Add(cookie);
            return Redirect(returnUrl);
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View(new MainPageViewModel(User.Identity.GetUserName(), User.IsInRole("Admin")));
        }

        [AllowAnonymous]
        [Route("User/{userid}")]
        public ActionResult UserProfile(string userid)
        {
            return View(new UserProfileViewModel(userid, User.IsInRole("Admin")));
        }

        [AllowAnonymous]
        [Route("User/{username}/{siteurl}")]
        public ActionResult PreviewSite(string username, string siteurl)
        {
            return View(new SiteViewModel(username, siteurl, User.Identity.GetUserName(), User.IsInRole("Admin")));
        }

        [Route("EditSite/{siteid}")]
        public ActionResult EditSite(int siteid = 0)
        {
            if (siteid == 0) return View("Error");
            return View(new SiteViewModel(siteid, User.Identity.GetUserName(), User.IsInRole("Admin")));
        }

        [ValidateInput(false)]
        public void SavePage(PageViewModel model, List<string> HtmlContent)
        {
            MainService.SavePageTitleAndContent((int)Session["PageId"], model.Title, HtmlContent);
        }

        [HttpPost]
        public ActionResult CreateSite(string inputData)
        {
            return RedirectToAction("EditSite", new { siteid = MainService.CreateSite(inputData, User.Identity.GetUserName()) });
        }

        public ActionResult Creating()
        {
            return PartialView("_ViewConfig", new SettingsModel() { UserId = User.Identity.GetUserId() });
        }

        public ActionResult CreatingPage(int siteid = 0)
        {
            string menutype = MainService.GetSiteMenu(siteid);
            Session["menu"] = menutype;
            Session["siteid"] = siteid;
            return PartialView("_ViewConfigPage", new SettingsModel(menutype));
        }

        public PartialViewResult ChangeTemplate(SettingsModel model)
        {
            ReChangeSession(model);
            string url = SettingsModel.CreatePhoto((string)Session["color"], (string)Session["menu"], (string)Session["template"]);
            return PartialView("_ColorTemplate", url);
        }

        private void ReChangeSession(SettingsModel model)
        {
            Session["color"] = model.Color ?? (string)Session["color"];
            Session["menu"] = model.Menu ?? (string)Session["menu"];
            Session["template"] = model.Template ?? (string)Session["template"];
        }

        [HttpPost]
        public ActionResult LoadAction()
        {
            if (Request != null && Request.IsAjaxRequest())
            {
                System.Threading.Thread.Sleep(3000);
                return Content(bool.TrueString);
            }
            return Content(bool.FalseString);
        }

        [HttpPost]
        public ActionResult AddPage(string inputData)
        {
            int siteid = (int)Session["siteid"];
            if (!MainService.CreatePage(inputData, siteid))
                throw new HttpException(500, "Sorry, but smth wrong with server.");
            return RedirectToAction("EditSite", new { siteid = siteid });
        }

        public PartialViewResult ShowPage(int pageid = 0)
        {
            return _Show(pageid, "_RedactPage");
        }

        [AllowAnonymous]
        public PartialViewResult PreviewShowPage(int pageid = 0)
        {
            return _Show(pageid, "_PreviewPage");
        }

        [AllowAnonymous]
        private PartialViewResult _Show(int pageid, string templ)
        {
            var page = new PageViewModel(pageid, templ == "_PreviewPage");
            page.IsAdmin = User.IsInRole("Admin") || User.Identity.GetUserName() == page.UserName;
            return PartialView(String.Format("{1}{0}", page.TemplateType, templ), page);
        }

        [HttpGet]
        public PartialViewResult Settings(int siteid)
        {
            Session["currentsite"] = siteid;
            return PartialView("_Settings", new SettingsModel(siteid));
        }

        public void SettingsConf(SettingsModel site)
        {
            if (!MainService.SiteConfirm((int)Session["currentsite"], site))
                throw new HttpException(500, "Sorry, but smth wrong with server.");
        }

        public void DeleteSite(int siteid)
        {
            if (!MainService.RemoveSite(siteid))
                throw new HttpException(404, "Some components of this site is removed recently.");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Admin()
        {
            return RedirectToAction("AllUsers");
        }

        [AllowAnonymous]
        public ActionResult Users()
        {
            return RedirectToAction("AllUsers");
        }

        [AllowAnonymous]
        [Route("AllUsers")]
        public ActionResult AllUsers()
        {
            return View(MainService.GetAllUsers());
        }

        [Authorize(Roles = "Admin")]
        public void UpInRole(string id)
        {
            if (!MainService.UpInRole(id))
                throw new HttpException(404, "This user is admin.");
        }

        [Authorize(Roles = "Admin")]
        public void DownInRole(string id)
        {
            if (!MainService.DownInRole(id))
                throw new HttpException(404, "This user already has role \"User\".");
        }

        public ActionResult DeletePage(int pageid = 0)
        {
            int siteid = MainService.DeletePageOrSite(pageid);
            if (siteid > 0)
            {
                return RedirectToAction("EditSite", new { siteid = siteid });
            }
            else
            {
                return RedirectToAction("UserProfile", new { userid = User.Identity.GetUserName() });
            }
        }

        [Authorize(Roles = "Admin")]
        public void Delete(string id = "")
        {
            if (!MainService.RemoveUser(id))
                throw new HttpException(404, "This user is removed resently.");
        }

        public ActionResult DeleteAccount()
        {
            if (!MainService.RemoveUser(User.Identity.GetUserId()))
                throw new HttpException(404, "This user is removed resently.");
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("Search")]
        public ActionResult DefSearch()
        {
            return View("Search",new SearchViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("SearchResults")]
        public ActionResult Search(string searchstring)
        {
            return View(new SearchViewModel(searchstring, User.Identity.GetUserName(), User.IsInRole("Admin")));
        }

        public ActionResult SaveUploadedFile()
        {
            string fName = "";
            try
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    if (file != null && file.ContentLength > 0)
                        return SavingAvatar(out fName, file);
                }
            }
            catch (Exception) { return Json(new { Message = "Error in saving file" }); }
            return Json(new { Message = fName });
        }

        private ActionResult SavingAvatar(out string fName, HttpPostedFileBase file)
        {
            MainService.SaveAvatar(out fName, file, User.Identity.GetUserName(), new DirectoryInfo(string.Format("{0}Images\\WallImages", Server.MapPath(@"\"))));
            return RedirectToAction("UserProfile", new { userid = User.Identity.Name });
        }

        [Route("Famehall")]
        public ActionResult Famehall()
        {
            AchievmentChecker achievments = MainService.GetAchivmentsChecker(User.Identity.GetUserId(), User.Identity.GetUserName());
            List<string> userachievments = MainService.GetUserAchivments(User.Identity.GetUserId()).Select(x => x.Name).ToList();
            return View(MainService.GetKeyValueAchievments(achievments.GetAllAchievments(), achievments.GetAllAchievmentsDescriptionsRU(), userachievments));
        }

        public PartialViewResult UpdateRating(string userid, string siteid, string value)
        {
            return PartialView("_UpdateRatingResult", MainService.GetRateMessage(userid, siteid, value));
        }

        [ValidateInput(false)]
        public PartialViewResult CommentSite(string NewComment, int siteid)
        {
            if(!MainService.SaveNewComment(NewComment, siteid, User.Identity.GetUserName()))
                throw new HttpException(500, "Sorry, but smth wrong with server.");
            return PartialView("_Comments", MainService.GetSiteComments(siteid));
        }

        public PartialViewResult DeleteComment(int comid, int siteid)
        {
            if(!MainService.DeleteComment(comid))
                throw new HttpException(404, "This comment is removed resently.");
            return PartialView("_Comments", MainService.GetSiteComments(siteid));
        }

        [HttpGet]
        public ActionResult CreateWithTemplate()
        {
            return View(new SettingsModel());
        }
        
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateWithTemplate(SettingsModel model)
        {
            return RedirectToAction("EditSite", new { siteid = MainService.CreateSiteWithTemplate(model, User.Identity.GetUserName()) });
        }



        //private void SearchComponents(string searchstring)
        //{
        //    ViewBag.comments = SearchCommentsLucene(searchstring);
        //    ViewBag.users = SearchUsersLucene(searchstring);
        //    ViewBag.SearchString = searchstring;
        //}

        //private List<Site> ConvertContentToSites(List<Content> list)
        //{
        //    List<Site> converted = new List<Site>();
        //    foreach (var content in list)
        //    {
        //        var siteid = GetSiteIdThoughtContent(content, db);
        //        converted.Add(db.Sites.Where(x => x.SiteId == siteid).FirstOrDefault());
        //    }
        //    return converted;
        //}

        //private IEnumerable<Content> SearchContentLucene(string searchstring)
        //{
        //    var site_searcher = new SearchContent();
        //    site_searcher.ClearLuceneIndex();
        //    site_searcher.AddUpdateLuceneIndex(db.Contents.ToList());
        //    return site_searcher.Search(searchstring);
        //}

        //private int? GetSiteIdThoughtContent(Content content)
        //{
        //    var pageid = content.PageId;
        //    return db.Pages.Where(x => x.PageId == pageid).FirstOrDefault().SiteId;
        //}

        //private IEnumerable<Comment> SearchCommentsLucene(Context db, string searchstring)
        //{
        //    var site_searcher = new SearchComments();
        //    site_searcher.ClearLuceneIndex();
        //    site_searcher.AddUpdateLuceneIndex(db.Comments.ToList());
        //    return site_searcher.Search(searchstring);
        //}

        //private IEnumerable<UserView> SearchUsersLucene(string searchstring)
        //{
        //    var site_searcher = new SearchUsers();
        //    site_searcher.ClearLuceneIndex();
        //    using (var db = new ApplicationDbContext())
        //        site_searcher.AddUpdateLuceneIndex(db.Users.ToList());
        //    return site_searcher.Search(searchstring);
        //}

        //private void SetIndexParameters(Context db, ApplicationDbContext udb)
        //{
        //    ViewBag.Tags = GetAllTags();
        //    ViewBag.UserName = GetTopUser(db, udb);
        //    ViewBag.Avatarpath = GetTopUsersAvatar(db, ViewBag.UserName);
        //    ViewBag.Achievment = CheckAchievments(db);
        //}

        //public ActionResult Search(string searchstring)
        //{
        //    using (var db = new Context())
        //    {
        //        var sites = SearchSitesLucene(db, searchstring).ToList();
        //        SearchComponents(searchstring, db);
        //        sites.AddRange(ConvertContentToSites(SearchContentLucene(db, searchstring).ToList(), db));
        //        return View(sites);
        //    }
        //}

        //private void SearchComponents(string searchstring, Context db)
        //{
        //    ViewBag.comments = SearchCommentsLucene(db, searchstring);
        //    ViewBag.users = SearchUsersLucene(searchstring);
        //    ViewBag.SearchString = searchstring;
        //}

        //private IEnumerable<UserView> SearchUsersLucene(string searchstring)
        //{
        //    var site_searcher = new SearchUsers();
        //    site_searcher.ClearLuceneIndex();
        //    using (var db = new ApplicationDbContext())
        //        site_searcher.AddUpdateLuceneIndex(db.Users.ToList());
        //    return site_searcher.Search(searchstring);
        //}

        //private List<Site> ConvertContentToSites(List<Content> list, Context db)
        //{
        //    List<Site> converted = new List<Site>();
        //    foreach (var content in list)
        //    {
        //        var siteid = GetSiteIdThoughtContent(content, db);
        //        converted.Add(db.Sites.Where(x => x.SiteId == siteid).FirstOrDefault());
        //    }
        //    return converted;
        //}

        //private int? GetSiteIdThoughtContent(Content content, Context db)
        //{
        //    var pageid = content.PageId;
        //    return db.Pages.Where(x => x.PageId == pageid).FirstOrDefault().SiteId;
        //}

        //private IEnumerable<Comment> SearchCommentsLucene(Context db, string searchstring)
        //{
        //    var site_searcher = new SearchComments();
        //    site_searcher.ClearLuceneIndex();
        //    site_searcher.AddUpdateLuceneIndex(db.Comments.ToList());
        //    return site_searcher.Search(searchstring);
        //}

        //private IEnumerable<Content> SearchContentLucene(Context db, string searchstring)
        //{
        //    var site_searcher = new SearchContent();
        //    site_searcher.ClearLuceneIndex();
        //    site_searcher.AddUpdateLuceneIndex(db.Contents.ToList());
        //    return site_searcher.Search(searchstring);
        //}

        //public IEnumerable<Site> SearchSitesLucene(Context db, string searchstring)
        //{
        //    var site_searcher = new SearchSites();
        //    site_searcher.ClearLuceneIndex();
        //    site_searcher.AddUpdateLuceneIndex(db.Sites.ToList());
        //    return site_searcher.Search(searchstring);
        //}

        //public string CheckAchievments(Context db)
        //{
        //    var id = User.Identity.GetUserId();
        //    AchievmentChecker achievments = new AchievmentChecker(db.Sites.Where(x => x.UserId == id).ToList(),
        //                                                                                                      db.RateLog.Where(x => x.User == id).OrderByDescending(x => x.Value).ToList(),
        //                                                                                                      db.Achievements.Where(x => x.User == id).ToList(),
        //                                                                                                      id);
        //    SaveAchievments(db, achievments.NewAchievments);
        //    return achievments.Result;
        //}

        //private void SaveAchievments(Context db, List<Achievement> log)
        //{
        //    if (log.Count() != 0)
        //        foreach (var item in log)
        //            db.Achievements.Add(item);
        //}

        //private string GetTopUsersAvatar(Context db, string username)
        //{
        //    try { return db.Avatars.Where(x => x.UserId == username).FirstOrDefault().Path; }
        //    catch (Exception) { return "http://cs.pikabu.ru/images/def_avatar/def_avatar_100.png"; }
        //}

        //private string GetTopUser(Context db, ApplicationDbContext udb)
        //{
        //    try
        //    {
        //        var userid = db.Sites.OrderByDescending(x => x.Rate).FirstOrDefault().UserId;
        //        return udb.Users.Where(x => x.UserName == userid).FirstOrDefault().UserName;
        //    }
        //    catch (Exception) { return "qwertyADMIN"; }
        //}

        //[Authorize(Roles = "Admin")]
        //public ActionResult Admin()
        //{
        //    return RedirectToAction("AllUsers");
        //}

        //[Route("Famehall")]
        //public ActionResult Famehall()
        //{
        //    using (var db = new Context())
        //    {
        //        var id = User.Identity.GetUserId();
        //        List<string> userachievments = new List<string>();
        //        AchievmentChecker achievments = new AchievmentChecker(db.Sites.Where(x => x.UserId == id).ToList(),
        //                                                                                                          db.RateLog.Where(x => x.User == id).OrderByDescending(x => x.Value).ToList(),
        //                                                                                                          db.Achievements.Where(x => x.User == id).ToList(),
        //                                                                                                          id);
        //        foreach (var item in db.Achievements.Where(x => x.User == id).ToList())
        //            userachievments.Add(item.Name);
        //        return View(GetKeyValueAchievments(achievments.GetAllAchievments(), achievments.GetAllAchievmentsDescriptionsRU(), userachievments));
        //    }
        //}

        //private Dictionary<string, bool> GetKeyValueAchievments(List<string> allachievments, List<string> alldesc, List<string> userachievments)
        //{
        //    Dictionary<string, bool> achievments = new Dictionary<string, bool>();
        //    for (int i = 0; i < allachievments.Count() - 1; i++)
        //        if (userachievments.Contains(allachievments[i]))
        //            achievments.Add(allachievments[i] + ":" + alldesc[i], true);
        //        else
        //            achievments.Add(allachievments[i] + ":" + alldesc[i], false);
        //    return achievments;
        //}

        //[AllowAnonymous]
        //public ActionResult Users()
        //{
        //    return RedirectToAction("AllUsers");
        //}

        //[AllowAnonymous]
        //[Route("AllUsers")]
        //public ActionResult AllUsers()
        //{
        //    ViewBag.IsAdmin = User.IsInRole("Admin");
        //    List<ApplicationUser> list_of_users;
        //    using (var db = new ApplicationDbContext())
        //        list_of_users = db.Users.ToList();
        //    return View(UserView.GetUserViews(list_of_users));
        //}

        //public ActionResult Delete(string id = "")
        //{
        //    if (id == "") return RedirectToAction("AllUsers");
        //    using (var db = new ApplicationDbContext())
        //    {
        //        var user = db.Users.Where(x => x.Id == id).FirstOrDefault();
        //        db.Users.Remove(user);
        //        db.SaveChanges();
        //    }
        //    return RedirectToAction("AllUsers");
        //}

        //public ActionResult Creating()
        //{
        //    ViewBag.Tags = GetAllTags();
        //    return PartialView("_ViewConfig", new SettingsModel()
        //    {
        //        UserId = User.Identity.GetUserId(),
        //        isActive = false,
        //        Color = "dark",
        //        Menu = "without",
        //        Template = "solid",
        //        CommentsAvailable = false,
        //        Url = SettingsModel.CreatePhoto("dark", "without", "solid")
        //    });
        //}

        //public ActionResult CreatingPage()
        //{
        //    return PartialView("_ViewConfigPage", new SettingsModel()
        //    {
        //        Color = "dark",
        //        Template = "solid",
        //        Url = SettingsModel.CreatePhoto("dark", (string)Session["menu"], "solid")
        //    });
        //}

        //public PartialViewResult ChangeTemplate(SettingsModel model)
        //{
        //    Session["color"] = model.Color ?? (string)Session["color"];
        //    Session["menu"] = model.Menu ?? (string)Session["menu"];
        //    Session["template"] = model.Template ?? (string)Session["template"];
        //    return PartialView("_ColorTemplate", SettingsModel.CreatePhoto((string)Session["color"], (string)Session["menu"], (string)Session["template"]));
        //}

        //public ActionResult UpInRole(string id)
        //{
        //    using(var db = new ApplicationDbContext())
        //    {
        //        var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        //        UserManager.AddToRole(id, "Admin");
        //    }
        //    return RedirectToAction("AllUsers");
        //}

        //public PartialViewResult UpdateRating(string userid, string siteid, string value)
        //{
        //    string firstRate = "Thank you!";
        //    string sameRate = "Updated.";
        //    using (var db = new Context())
        //    {
        //        var rate = db.RateLog.Where(x => x.User == userid).Where(x => x.Site == siteid).FirstOrDefault();
        //        if (rate == null)
        //        {
        //            rate = FirstRatedRate(userid, siteid, value, firstRate, db);
        //        }
        //        else
        //        {
        //            if (rate.Value == Convert.ToInt32(value))
        //            {
        //                return RepeatedValueRating(sameRate);
        //            }
        //            else
        //            {
        //                UpdatingRating(value, rate);
        //            }
        //        }
        //        SaveAndcountAverage(siteid, db);
        //        db.SaveChanges();
        //    }
        //    return PartialView("_UpdateRatingResult");
        //}

        //private static void SaveAndcountAverage(string siteid, Context db)
        //{
        //    double average = CountingAverage(siteid, db);
        //    int siteidd = Convert.ToInt32(siteid);
        //    var site = db.Sites.Where(x => x.SiteId == siteidd).FirstOrDefault();
        //    site.Rate = average;
        //    db.SaveChanges();
        //}

        //private static double CountingAverage(string siteid, Context db)
        //{
        //    var sites = db.RateLog.Where(x => x.Site == siteid).ToList();
        //    double average = 0.0;
        //    foreach (var item in sites)
        //        average += (double)item.Value;
        //    if (sites.Count() == 0)
        //    {
        //        return average;
        //    }
        //    else
        //    {
        //        average = average / (double)sites.Count();

        //    }
        //    return average;
        //}

        //private void UpdatingRating(string value, Rate rate)
        //{
        //    string updateRate = "Update. Last mark: " + rate.Value;
        //    rate.Value = Convert.ToInt32(value);
        //    ViewBag.Answer = updateRate;
        //}

        //private PartialViewResult RepeatedValueRating(string sameRate)
        //{
        //    ViewBag.Answer = sameRate;
        //    return PartialView("_UpdateRatingResult");
        //}

        //private Rate FirstRatedRate(string userid, string siteid, string value, string firstRate, Context db)
        //{
        //    Rate rate = new Rate() { Value = Convert.ToInt32(value), Site = siteid, User = userid };
        //    ViewBag.Answer = firstRate;
        //    db.RateLog.Add(rate);
        //    return rate;
        //}

        //private void AddUpdateNewTags(string newtags, Context db)
        //{
        //    var alltags = db.Tags.ToList();
        //    foreach (var item in newtags.Split(','))
        //    {
        //        var checktag = alltags.Where(x => x.Name == item).FirstOrDefault();
        //        if (checktag == null)
        //        {
        //            Tag mynewtag = new Tag() { Name = item, Repeats = 1 };
        //            db.Tags.Add(mynewtag);
        //        }
        //        else checktag.Repeats += 1;
        //    }
        //}



        //private string GetNameById(string userId)
        //{
        //    using (var db = new ApplicationDbContext())
        //        return db.Users.Where(x => x.Id == userId).FirstOrDefault().UserName;
        //}

        //private Site GetFullSite(int siteid, Context udb)
        //{
        //    Site model = udb.Sites.Where(x => x.SiteId == siteid).Include(x => x.Pages).FirstOrDefault();
        //    var pages = udb.Pages.Where(x => x.SiteId == siteid).Include(x => x.Contents).ToList();
        //    model.Pages = pages;
        //    return model;
        //}

        //private List<int> FromPageIds(Site model)
        //{
        //    return model.Pages.Select(x => x.PageId).ToList();
        //}

        //private List<string> FromPageTitles(Site model)
        //{
        //    var obj = model.Pages.Select(x => x.Title);
        //    List<string> titles = new List<string>();
        //    foreach (var elem in obj)
        //        titles.Add(Regex.Replace(elem ?? "Empty", "<[^>]+>", string.Empty));
        //    return titles;
        //}

        //private void CreatePageContent(int pageid, string type, Context db)
        //{
        //    int count = type == "mixed" ? 3 : type == "solid" ? 1 : 2;
        //    for (int i = 0; i < count; i++)
        //        db.Contents.Add(new Content() { HtmlContent = "", PageId = pageid });
        //}

        //[ValidateInput(false)]
        //public PartialViewResult SavePage(Page model, List<string> HtmlContent)
        //{
        //    using (var db = new Context())
        //    {
        //        SavePageToDB(model, (int)Session["PageId"], db);
        //        SavePageContentToDB((int)Session["PageId"], HtmlContent, db);
        //        Page page = GetPageFromId((int)Session["PageId"], db);
        //        return PartialView(String.Format("_RedactPage{0}", page.TemplateType), page);
        //    }
        //}

        //private void SavePageContentToDB(int pageid, List<string> htmlContent, Context db)
        //{
        //    var contents = db.Contents.Where(x => x.PageId == pageid).ToList();
        //    for (int i = 0; i < contents.Count; i++)
        //        contents[i].HtmlContent = htmlContent[i];
        //    db.SaveChanges();
        //}

        //private void SavePageToDB(Page model, int pageid, Context db)
        //{
        //    Page page = db.Pages.Where(x => x.PageId == pageid).FirstOrDefault();
        //    page.Title = model.Title;
        //    db.SaveChanges();
        //}

        //private Page GetPageFromId(int pageid, Context db)
        //{
        //    return db.Pages.Where(x => x.PageId == pageid).Include(x => x.Contents).FirstOrDefault();
        //}

        //public ActionResult Wysiwyg()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Wysiwyg(HTMLContextClass obj)
        //{
        //    return View();
        //}



        //public ActionResult About()
        //{
        //    ViewBag.Message = "Your application description page.";
        //    return View();
        //}

        //public PartialViewResult DeleteTopSite(int siteid)
        //{
        //    using (var db = new Context())
        //    using (var udb = new ApplicationDbContext())
        //    {
        //        var pages = db.Pages.Where(l => l.SiteId == siteid).ToList();
        //        foreach (var item in pages)
        //            DeleteTopContentAndPages(db, item);
        //        var site = db.Sites.Where(x => x.SiteId == siteid).FirstOrDefault();
        //        ViewBag.Email = (string)Session["username"];
        //        if (site != null)
        //            db.Sites.Remove(site);
        //        db.SaveChanges();
        //        return PartialView("_HomePageTopSiteTable", GetTop3Sites(db));
        //    }
        //}

        //private static void DeleteTopContentAndPages(Context db, Page item)
        //{
        //    var content = db.Contents.Where(x => x.PageId == item.PageId).ToList();
        //    foreach (var elem in content)
        //        db.Contents.Remove(elem);
        //    db.Pages.Remove(item);
        //}

        //public PartialViewResult DeleteSite(int siteid)
        //{
        //    string userid = "";
        //    using (var db = new Context())
        //    using (var udb = new ApplicationDbContext())
        //    {
        //        DeleteSiteFromId(siteid, db);
        //        ViewBag.Email = userid = (string)Session["username"];
        //        return PartialView("_SiteTable", GetProfilerSites(userid != "" ? userid : User.Identity.GetUserName(), udb, db));
        //    }
        //}

        //private void DeleteSiteFromId(int siteid, Context db)
        //{
        //    foreach (var item in db.Pages.Where(l => l.SiteId == siteid).ToList())
        //    {
        //        foreach (var elem in db.Contents.Where(x => x.PageId == item.PageId).ToList())
        //            db.Contents.Remove(elem);
        //        db.Pages.Remove(item);
        //    }
        //    var site = db.Sites.Where(x => x.SiteId == siteid).FirstOrDefault();
        //    if (site != null)
        //        db.Sites.Remove(site);
        //    db.SaveChanges();
        //}

        //private Site ConfigureModelToPreview(string siteurl, Context db)
        //{
        //    Site model = db.Sites.Where(x => x.Url == siteurl).Include(x => x.Pages).Include(x => x.Comments).FirstOrDefault();
        //    var pages = db.Pages.Where(x => x.SiteId == model.SiteId).Include(x => x.Contents).ToList();
        //    model.Pages = pages;
        //    model.Comments = model.Comments.OrderByDescending(x => x.CreationTime).ToList();
        //    ViewBag.UserAvatar = GetCurrentUserAvatar(db);
        //    return model;
        //}

        //private string GetCurrentUserAvatar(Context db)
        //{
        //    string email = User.Identity.GetUserName();
        //    var avatar = db.Avatars.Where(x => x.UserId == email).FirstOrDefault();
        //    return avatar == null ? null : avatar.Path;
        //}

        //private List<Comment> GetSiteComments(int siteid, Context db)
        //{
        //    return db.Comments.Where(x => x.SiteId == siteid).OrderByDescending(x => x.CreationTime).ToList();
        //}

        //

        //private void ConfigureUserBanner(string userid, Context db, ApplicationDbContext applicationdb, string id, Avatar avatar)
        //{
        //    ViewBag.Email = userid;
        //    ViewBag.avatarpath = avatar != null ? avatar.Path : "";
        //    ViewBag.Rate = GetProfilerRate(db, applicationdb);
        //    ViewBag.howmuchachievments = db.Achievements.Where(x => x.User == id).Count().ToString() + " / 8";
        //}

        //private string GetAllTags()
        //{
        //    string tagstring = "";
        //    using (var db = new Context())
        //    {
        //        foreach (var item in db.Tags.OrderByDescending(x => x.Repeats).ToList())
        //            tagstring += item.Name + "," + item.Repeats.ToString() + ";";
        //        try
        //        {
        //            return tagstring.Remove(tagstring.Length - 1); ;
        //        }
        //        catch (Exception)
        //        {
        //            return tagstring;
        //        }
        //    }

        //}

        //private string GetIdThoughtEmail(ApplicationDbContext applicationdb, string username)
        //{
        //    try
        //    {
        //        return applicationdb.Users.Where(x => x.UserName == username).FirstOrDefault().Id;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new HttpException(404, "Item Not Found");
        //    }
        //}

        //private double GetProfilerRate(Context db, ApplicationDbContext applicationdb)
        //{
        //    List<Site> sites = new List<Site>();
        //    double average = 0.0;
        //    string name = (string)Session["username"];
        //    sites = db.Sites.Where(x => x.UserId == name).ToList();
        //    foreach (var item in sites)
        //        average += item.Rate;
        //    if (sites.Count() == 0)
        //        return 0.0;
        //    average = average / sites.Count();
        //    return average;
        //}

        //private List<Site> GetTop3Sites(Context db)
        //{
        //    var sites = db.Sites.OrderByDescending(x => x.Rate).Take(3).Include(x=>x.Pages).ToList();
        //    return sites;
        //}

        //private List<Site> GetProfilerSites(string userid, ApplicationDbContext udb, Context db)
        //{
        //    List<Site> sites = new List<Site>();
        //    if (userid == null) { throw new HttpException(404, "Item Not Found"); }
        //    sites = db.Sites.Where(x => x.UserId == userid).Include(x=>x.Pages).ToList();
        //    return sites;
        //}

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult LoadAction()
        //{
        //    if (Request != null && Request.IsAjaxRequest())
        //    {
        //        System.Threading.Thread.Sleep(3000);
        //        return Content(bool.TrueString);
        //    }
        //    return Content(bool.FalseString);
        //}

        //[HttpPost]
        //public ActionResult AddPage(string inputData)
        //{
        //    using (var db = new Context())
        //    {
        //        string[] values = inputData.Split(';');
        //        Page page = new Page { SiteId = (int)Session["siteid"], Color = values[1], TemplateType = values[2], Title = values[0] == "" ? "Page Title" : values[0] };
        //        db.Pages.Add(page);
        //        CreatePageContent(page.PageId, page.TemplateType, db);
        //        db.SaveChanges();
        //        return RedirectToAction("EditSite", new { siteid = (int)Session["siteid"] });
        //    }
        //}

        //public PartialViewResult ShowPage(int pageid = 0)
        //{
        //    return _Show(pageid, "_RedactPage");
        //}

        //public PartialViewResult PreviewShowPage(int pageid = 0)
        //{
        //    return _Show(pageid, "_PreviewPage");
        //}

        //private PartialViewResult _Show(int pageid, string templ)
        //{
        //    using (var db = new Context())
        //    {
        //        Page page = db.Pages.Where(x => x.PageId == pageid).Include(x => x.Contents).FirstOrDefault();
        //        return PartialView(String.Format("{1}{0}", page.TemplateType, templ), page);
        //    }
        //}

        //public ActionResult DeletePage(int pageid = 0)
        //{
        //    using (var db = new Context())
        //    {
        //        var page = db.Pages.Where(x => x.PageId == pageid).FirstOrDefault();
        //        int? siteid = page.SiteId;
        //        int count_pages = db.Pages.Where(x => x.SiteId == siteid).ToList().Count();
        //        if (count_pages > 1)
        //        {
        //            DeleteOnePage(page, db);
        //            return RedirectToAction("EditSite", new { siteid = siteid });
        //        }
        //        else
        //        {
        //            DeleteSiteFromId((int)siteid, db);
        //            return RedirectToAction("UserProfile", new { userid = User.Identity.GetUserName() });
        //        }
        //    }
        //}

        //private void DeleteOnePage(Page page, Context db)
        //{
        //    DeleteContentFromPage(page.PageId, db);
        //    db.Pages.Remove(page);
        //    db.SaveChanges();
        //}

        //private void DeleteContentFromPage(int pageId, Context db)
        //{
        //    var contents = db.Contents.Where(x => x.PageId == pageId);
        //    foreach (var elem in contents)
        //        db.Contents.Remove(elem);
        //}

        //public PartialViewResult Settings(int siteid)
        //{
        //    using(var db = new Context())
        //    {
        //        ViewBag.Tags = GetAllTags();
        //        var site = db.Sites.Where(x => x.SiteId == siteid).FirstOrDefault();
        //        if (site.UserId != User.Identity.GetUserName() && !User.IsInRole("Admin")) throw new HttpException(404, "Not your site!");
        //        Session["sitesettings"] = site.SiteId;
        //        ViewBag.CurrentTags = site.Tags;
        //        return PartialView("_Settings", site);
        //    }
        //}

        //[HttpPost]
        //public ActionResult Settings(Site site, string url)
        //{
        //    using (var db = new Context()) {
        //        int siteid = (int)Session["sitesettings"];
        //        ConfirmChanges(site, url, siteid, db);
        //        string userid = db.Sites.Where(x => x.SiteId == siteid).FirstOrDefault().UserId;
        //        return RedirectToAction("UserProfile","Home", new { userid = userid });
        //    }
        //}

        //private void ConfirmChanges(Site site, string url, int siteid, Context db)
        //{
        //    Site oldsite = db.Sites.Where(x => x.SiteId == siteid).FirstOrDefault();
        //    oldsite.Url = url == "" ? oldsite.Url : url;
        //    AddUpdateNewTags(site.Tags, db);
        //    oldsite.Tags = site.Tags;
        //    oldsite.Description = site.Description == "" ? oldsite.Description : site.Description;
        //    oldsite.HasComments = site.HasComments;
        //    oldsite.Title = site.Title == "" ? oldsite.Title : site.Title;
        //    db.SaveChanges();
        //}
    }
}