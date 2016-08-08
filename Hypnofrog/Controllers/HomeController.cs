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
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Hypnofrog.SearchLucene;

namespace Hypnofrog.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (var db = new Context())
            {
                using (var udb = new ApplicationDbContext())
                {
                    ViewBag.Tags = GetAllTags();
                    ViewBag.UserName = GetTopUser(db, udb);
                    ViewBag.Avatarpath = GetTopUsersAvatar(db, ViewBag.UserName);
                    ViewBag.Achievment = CheckAchievments(db);
                    db.SaveChanges();
                    return View(GetTop3Sites(db));
                }
            }
        }

        public ActionResult Search(string searchstring)
        {
            if (searchstring==null) return RedirectToAction("DefaultSearchPage");
            using (var db = new Context())
            {
                var sites = SearchSitesLucene(db, searchstring).ToList();
                ViewBag.comments = SearchCommentsLucene(db, searchstring);
                ViewBag.SearchString = searchstring;
                sites.AddRange(ConvertContentToSites(SearchContentLucene(db, searchstring).ToList(), db));
                return View(sites);

            }
        }

        public ActionResult DefaultSearchPage()
        {
            return View();
        }


        private List<Site> ConvertContentToSites(List<Content> list, Context db)
        {
            List<Site> converted = new List<Site>();
            foreach (var content in list)
            {
                var siteid = GetSiteIdThoughtContent(content,db);
                converted.Add(db.Sites.Where(x => x.SiteId == siteid).FirstOrDefault());
            }
            return converted;
        }

        private int? GetSiteIdThoughtContent(Content content, Context db)
        {
            var pageid = content.PageId;
            return  db.Pages.Where(x => x.PageId == pageid).FirstOrDefault().SiteId;
        }

        private IEnumerable<Comment> SearchCommentsLucene(Context db, string searchstring)
        {
            var site_searcher = new SearchComments();
            site_searcher.ClearLuceneIndex();
            site_searcher.AddUpdateLuceneIndex(db.Comments.ToList());
            return site_searcher.Search(searchstring);
        }

        private IEnumerable<Content> SearchContentLucene(Context db, string searchstring)
        {
            var site_searcher = new SearchContent();
            site_searcher.ClearLuceneIndex();
            site_searcher.AddUpdateLuceneIndex(db.Contents.ToList());
            return site_searcher.Search(searchstring);
        }

        public IEnumerable<Site> SearchSitesLucene(Context db, string searchstring )
        {
            var site_searcher = new SearchSites();
            site_searcher.ClearLuceneIndex();
            site_searcher.AddUpdateLuceneIndex(db.Sites.ToList());
            return site_searcher.Search(searchstring);
        }






        public string CheckAchievments(Context db)
        {
            var id = User.Identity.GetUserId();
            var rates = db.RateLog.Where(x => x.User == id).OrderByDescending(x => x.Value).ToList();
            var sites = db.Sites.Where(x => x.UserId == id).ToList();
            var log = db.Achievements.Where(x => x.User == id).ToList();
            AchievmentChecker achievments = new AchievmentChecker(sites, rates, log, id);
            SaveAchievments(db, achievments.NewAchievments);
            return achievments.Result;
        }

        private void SaveAchievments(Context db, List<Achievement> log)
        {
            if (log.Count() != 0)
            {
                foreach (var item in log)
                {
                    db.Achievements.Add(item);
                }
            }
        }

        private string GetTopUsersAvatar(Context db, string username)
        {
            return db.Avatars.Where(x => x.UserId == username).FirstOrDefault().Path;
        }

        private string GetTopUser(Context db, ApplicationDbContext udb)
        {
            try
            {
                var userid = db.Sites.OrderByDescending(x => x.Rate).FirstOrDefault().UserId;
                return udb.Users.Where(x => x.Id == userid).FirstOrDefault().UserName;
            }
            catch (Exception)
            {
                return "qwertyADMIN";
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Admin()
        {
            return RedirectToAction("AllUsers");
        }

        public ActionResult Famehall()
        {
            using (var db = new Context())
            {
                var id = User.Identity.GetUserId();
                List<string> userachievments = new List<string>();
                var rates = db.RateLog.Where(x => x.User == id).OrderByDescending(x => x.Value).ToList();
                var sites = db.Sites.Where(x => x.UserId == id).ToList();
                var log = db.Achievements.Where(x => x.User == id).ToList();
                AchievmentChecker achievments = new AchievmentChecker(sites, rates, log, id);
                foreach (var item in log)
                    userachievments.Add(item.Name);
                return View(GetKeyValueAchievments(achievments.GetAllAchievments(), achievments.GetAllAchievmentsDescriptionsRU(), userachievments));
            }
        }

        private Dictionary<string, bool> GetKeyValueAchievments(List<string> allachievments, List<string> alldesc, List<string> userachievments)
        {
            Dictionary<string, bool> achievments = new Dictionary<string, bool>();
            for (int i = 0; i < allachievments.Count() - 1; i++)
                if (userachievments.Contains(allachievments[i]))
                    achievments.Add(allachievments[i] + ":" + alldesc[i], true);
                else
                    achievments.Add(allachievments[i] + ":" + alldesc[i], false);
            return achievments;
        }

        public ActionResult Users()
        {
            return RedirectToAction("AllUsers");
        }

        [Route("AllUsers")]
        public ActionResult AllUsers()
        {
            ViewBag.IsAdmin = User.IsInRole("Admin");
            List<ApplicationUser> list_of_users;
            using (var db = new ApplicationDbContext())
            {
                list_of_users = db.Users.ToList();
            }
            return View(list_of_users);
        }

        public ActionResult Delete(string id = "")
        {
            if (id == "") return RedirectToAction("AllUsers");
            using (var db = new ApplicationDbContext())
            {
                var user = db.Users.Where(x => x.Id == id).FirstOrDefault();
                db.Users.Remove(user);
                db.SaveChanges();
            }
            return RedirectToAction("AllUsers");
        }

        public ActionResult Creating()
        {
            ViewBag.Tags = GetAllTags();
            return PartialView("_ViewConfig", new SettingsModel()
            {
                UserId = User.Identity.GetUserId(),
                isActive = false,
                Color = "dark",
                Menu = "without",
                Template = "solid",
                CommentsAvailable = false,
                Url = SettingsModel.CreatePhoto("dark", "without", "solid")
            });
        }

        public ActionResult CreatingPage()
        {
            return PartialView("_ViewConfigPage", new SettingsModel()
            {
                Color = "dark",
                Template = "solid",
                Url = SettingsModel.CreatePhoto("dark", (string)Session["menu"], "solid")
            });
        }

        public PartialViewResult ChangeTemplate(SettingsModel model)
        {
            Session["color"] = model.Color ?? (string)Session["color"];
            Session["menu"] = model.Menu ?? (string)Session["menu"];
            Session["template"] = model.Template ?? (string)Session["template"];
            return PartialView("_ColorTemplate", SettingsModel.CreatePhoto((string)Session["color"], (string)Session["menu"], (string)Session["template"]));
        }

        //public PartialViewResult ChangeTemplatePage(SettingsModel model)
        //{
        //    Session["color"] = model.Color ?? (string)Session["color"];
        //    Session["template"] = model.Template ?? (string)Session["template"];
        //    return PartialView("_ColorTemplate", SettingsModel.CreatePhoto((string)Session["color"], (string)Session["menu"], (string)Session["template"]));
        //}

        public PartialViewResult UpdateRating(string userid, string siteid, string value)
        {
            string firstRate = "Спасибо за вашу оценку";
            string sameRate = "Обновлено";
            using (var db = new Context())
            {
                var rate = db.RateLog.Where(x => x.User == userid).Where(x => x.Site == siteid).FirstOrDefault();
                if (rate == null)
                {
                    rate = new Rate() { Value = Convert.ToInt32(value), Site = siteid, User = userid };
                    ViewBag.Answer = firstRate;
                    db.RateLog.Add(rate);
                }
                else
                {
                    if (rate.Value == Convert.ToInt32(value))
                    {
                        ViewBag.Answer = sameRate;
                        return PartialView("_UpdateRatingResult");
                    }
                    else
                    {
                        string updateRate = "Обновлено. Предыдущая оценка: " + rate.Value;

                        rate.Value = Convert.ToInt32(value);
                        ViewBag.Answer = updateRate;
                    }
                }
                db.SaveChanges();
                var sites = db.RateLog.Where(x => x.Site == siteid).ToList();
                double average = 0.0;
                foreach (var item in sites)
                {
                    average += (double)item.Value;
                }
                average = average / (double)sites.Count();
                int siteidd = Convert.ToInt32(siteid);
                var site = db.Sites.Where(x => x.SiteId == siteidd).FirstOrDefault();
                site.Rate = average;
                db.SaveChanges();
            }
            return PartialView("_UpdateRatingResult");
        }

        [HttpPost]
        public ActionResult CreateSite(string inputData)
        {
            string[] param = inputData.Split(';');
            param[0] = param[0] == "" ? "Мой сайт" : param[0];
            param[1] = param[1] == "" ? "Описание сайта" : param[1];
            var dbsite = new Site { };
            var page = new Page();
            using (var db = new Context())
            {
                var site = new Site { CreationTime = DateTime.Now, HasComments = param[3] == "true", Title = param[0], Description = param[1], Url = param[2], Iscomplited = false, MenuType = param[5], UserId = User.Identity.GetUserId(), Rate = 0.0, Tags = param[7] };
                db.Sites.Add(site);
                dbsite = site;
                page = new Page { SiteId = site.SiteId, Color = param[4], TemplateType = param[6], Title = "Page Title" };
                db.Pages.Add(page);
                AddUpdateNewTags(param[7], db);
                CreatePageContent(page.PageId, param[6], db);
                db.SaveChanges();
            }
            return RedirectToAction("EditSite", new { siteid = dbsite.SiteId });
        }

        private void AddUpdateNewTags(string newtags, Context db)
        {
            var alltags = db.Tags.ToList();
            foreach (var item in newtags.Split(','))
            {
                var checktag = alltags.Where(x => x.Name == item).FirstOrDefault();
                if (checktag == null)
                {
                    Tag mynewtag = new Tag() { Name = item, Repeats = 1 };
                    db.Tags.Add(mynewtag);
                }
                else checktag.Repeats += 1;
            }
        }

        public ActionResult EditSite(int siteid = 0)
        {
            if (siteid == 0) return View("Error");
            Site model = null;
            using (var db = new Context())
            {
                model = GetFullSite(siteid, db);
            }
            ViewBag.PageTitles = FromPageTitles(model);
            ViewBag.PageIds = FromPageIds(model);
            Session["siteid"] = siteid;
            Session["menu"] = model.MenuType;
            return View(model);
        }

        private Site GetFullSite(int siteid, Context udb)
        {
            Site model = udb.Sites.Where(x => x.SiteId == siteid).Include(x => x.Pages).FirstOrDefault();
            var pages = udb.Pages.Where(x => x.SiteId == siteid).Include(x => x.Contents).ToList();
            model.Pages = pages;
            return model;
        }

        private List<int> FromPageIds(Site model)
        {
            return model.Pages.Select(x => x.PageId).ToList();
        }

        private List<string> FromPageTitles(Site model)
        {
            var obj = model.Pages.Select(x => x.Title);
            List<string> titles = new List<string>();
            foreach (var elem in obj)
            {
                titles.Add(Regex.Replace(elem ?? "Empty", "<[^>]+>", string.Empty));
            }
            return titles;
        }

        private void CreatePageContent(int pageid, string type, Context db)
        {
            int count = type == "mixed" ? 3 : type == "solid" ? 1 : 2;
            for (int i = 0; i < count; i++)
                db.Contents.Add(new Content() { HtmlContent = "", PageId = pageid });
        }

        [ValidateInput(false)]
        public PartialViewResult SavePage(Page model, List<string> HtmlContent)
        {
            using (var db = new Context())
            {
                SavePageToDB(model, (int)Session["PageId"], db);
                SavePageContentToDB((int)Session["PageId"], HtmlContent, db);
                Page page = GetPageFromId((int)Session["PageId"], db);
                return PartialView(String.Format("_RedactPage{0}", page.TemplateType), page);
            }
        }

        private void SavePageContentToDB(int pageid, List<string> htmlContent, Context db)
        {
            var contents = db.Contents.Where(x => x.PageId == pageid).ToList();
            for (int i = 0; i < contents.Count; i++)
                contents[i].HtmlContent = htmlContent[i];
            db.SaveChanges();
        }

        private void SavePageToDB(Page model, int pageid, Context db)
        {
            Page page = db.Pages.Where(x => x.PageId == pageid).FirstOrDefault();
            page.Title = model.Title;
            db.SaveChanges();
        }

        private Page GetPageFromId(int pageid, Context db)
        {
            return db.Pages.Where(x => x.PageId == pageid).Include(x => x.Contents).FirstOrDefault();
        }

        public ActionResult Wysiwyg()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Wysiwyg(HTMLContextClass obj)
        {
            return View();
        }

        public ActionResult SaveUploadedFile()
        {
            bool isSavedSuccessfully = true;
            string fName = "";
            try
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    if (file != null && file.ContentLength > 0)
                    {
                        fName = file.FileName;
                        var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\WallImages", Server.MapPath(@"\")));
                        string pathString = Path.Combine(originalDirectory.ToString(), "imagepath");
                        var fileName1 = Path.GetFileName(file.FileName);
                        bool isExists = Directory.Exists(pathString);
                        if (!isExists) Directory.CreateDirectory(pathString);
                        var path = string.Format("{0}\\{1}", pathString, file.FileName);
                        file.SaveAs(path);
                        Cloudinary cloudinary = new Cloudinary(new Account("dldmfb5fo", "568721824454478", "ZO4nwcMQwcT88lUNUK5KHJmy_fU"));
                        var param = new ImageUploadParams()
                        {
                            File = new FileDescription(path)
                        };
                        var result = cloudinary.Upload(param);
                        using (var db = new Context())
                        {
                            var useravatar = db.Avatars.Where(x => x.UserId == User.Identity.Name).FirstOrDefault();
                            if (useravatar == null)
                            {
                                useravatar = new Avatar() { UserId = User.Identity.GetUserName() };
                                db.Avatars.Add(useravatar);
                            }
                            useravatar.Path = result.Uri.AbsoluteUri;
                            db.SaveChanges();
                        }
                        return RedirectToAction("UserProfile", new { userid = User.Identity.Name });
                    }

                }

            }
            catch (Exception)
            {
                isSavedSuccessfully = false;
            }
            if (isSavedSuccessfully) return Json(new { Message = fName });
            else return Json(new { Message = "Error in saving file" });

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public PartialViewResult DeleteTopSite(int siteid)
        {
            string userid = "";
            using (var db = new Context())
            {
                using (var udb = new ApplicationDbContext())
                {
                    var pages = db.Pages.Where(l => l.SiteId == siteid).ToList();
                    foreach (var item in pages)
                    {
                        var content = db.Contents.Where(x => x.PageId == item.PageId).ToList();
                        foreach (var elem in content)
                        {
                            db.Contents.Remove(elem);
                        }
                        db.Pages.Remove(item);
                    }
                    var site = db.Sites.Where(x => x.SiteId == siteid).FirstOrDefault();
                    ViewBag.Email = userid = (string)Session["useremail"];
                    db.Sites.Remove(site);
                    db.SaveChanges();
                    return PartialView("_HomePageTopSiteTable", GetTop3Sites(db));
                }
            }
        }

        public PartialViewResult DeleteSite(int siteid)
        {
            string userid = "";
            using (var db = new Context())
            {
                using (var udb = new ApplicationDbContext())
                {
                    DeleteSiteFromId(siteid, db);
                    ViewBag.Email = userid = (string)Session["useremail"];
                    return PartialView("_SiteTable", GetProfilerSites(userid != "" ? userid : User.Identity.GetUserName(), udb, db));
                }
            }
        }

        private void DeleteSiteFromId(int siteid, Context db)
        {
            var pages = db.Pages.Where(l => l.SiteId == siteid).ToList();
            foreach (var item in pages)
            {
                var content = db.Contents.Where(x => x.PageId == item.PageId).ToList();
                foreach (var elem in content)
                {
                    db.Contents.Remove(elem);
                }
                db.Pages.Remove(item);
            }
            var site = db.Sites.Where(x => x.SiteId == siteid).FirstOrDefault();
            db.Sites.Remove(site);
            db.SaveChanges();
        }

        public ActionResult PreviewSite(int siteid)
        {
            Site model = null;
            using (var db = new Context())
            {
                model = db.Sites.Where(x => x.SiteId == siteid).Include(x => x.Pages).Include(x => x.Comments).FirstOrDefault();
                var pages = db.Pages.Where(x => x.SiteId == siteid).Include(x => x.Contents).ToList();
                model.Pages = pages;
                model.Comments = model.Comments.OrderByDescending(x => x.CreationTime).ToList();
                ViewBag.UserAvatar = GetCurrentUserAvatar(db);
            }
            ViewBag.PageTitles = FromPageTitles(model);
            ViewBag.PageIds = FromPageIds(model);
            Session["siteid"] = siteid;
            return View("PreviewSite", model);
        }

        private string GetCurrentUserAvatar(Context db)
        {
            string email = User.Identity.GetUserName();
            var avatar = db.Avatars.Where(x => x.UserId == email).FirstOrDefault();
            return avatar == null ? null : avatar.Path;
        }


        [ValidateInput(false)]
        public PartialViewResult CommentSite(string NewComment)
        {
            using (var db = new Context())
            {
                string useravatar = GetCurrentUserAvatar(db);
                db.Comments.Add(new Comment()
                {
                    CreationTime = DateTime.Now,
                    SiteId = (int)Session["siteid"],
                    Text = NewComment,
                    UserAvatar = useravatar,
                    UserId = User.Identity.GetUserName()
                });
                db.SaveChanges();
                return PartialView("_Comments", GetSiteComments((int)Session["siteid"], db));
            }
        }

        public PartialViewResult DeleteComment(int comid = 0)
        {
            using (var db = new Context())
            {
                var comment = db.Comments.Where(x => x.CommentId == comid).FirstOrDefault();
                db.Comments.Remove(comment);
                db.SaveChanges();
                return PartialView("_Comments", GetSiteComments((int)Session["siteid"], db));
            }
        }

        private List<Comment> GetSiteComments(int siteid, Context db)
        {
            return db.Comments.Where(x => x.SiteId == siteid).OrderByDescending(x => x.CreationTime).ToList();
        }
        

        [Route("User/{userid}")]
        public ActionResult UserProfile(string userid)
        {
            Session["useremail"] = userid;
            using (var db = new Context())
            {
                using (var applicationdb = new ApplicationDbContext())
                {
                    var avatar = db.Avatars.Where(x => x.UserId == userid).FirstOrDefault();
                    ViewBag.Email = userid;
                    ViewBag.avatarpath = avatar != null ? avatar.Path : "";
                    ViewBag.Rate = GetProfilerRate(db, applicationdb);
                    var id = GetIdThoughtEmail(applicationdb, userid);
                    ViewBag.howmuchachievments = db.Achievements.Where(x => x.User == id).Count().ToString()+" из 8";
                    if (userid == User.Identity.Name)
                    {
                        ViewBag.Achievment = CheckAchievments(db);
                    }
                    db.SaveChanges();
                    return View(GetProfilerSites(userid, applicationdb, db));
                }
            }

        }

        private string GetAllTags()
        {
            string tagstring = "";
            using (var db = new Context())
            {
                var tags = db.Tags.OrderByDescending(x => x.Repeats).ToList();
                foreach (var item in tags)
                {
                    tagstring += item.Name + "," + item.Repeats.ToString() + ";";
                }
                return tagstring.Remove(tagstring.Length - 1); ;
            }

        }

        private string GetIdThoughtEmail(ApplicationDbContext applicationdb, string username)
        {
            try
            {
                return applicationdb.Users.Where(x => x.UserName == username).FirstOrDefault().Id;
            }
            catch (Exception ex)
            {
                throw new HttpException(404, "Item Not Found");
            }


        }

        private double GetProfilerRate(Context db, ApplicationDbContext applicationdb)
        {
            List<Site> sites = new List<Site>();
            double average = 0.0;
            string id = GetIdThoughtEmail(applicationdb, (string)Session["useremail"]);
            sites = db.Sites.Where(x => x.UserId == id).ToList();
            foreach (var item in sites) average += item.Rate;
            if (sites.Count() == 0) return 0.0;
            average = average / sites.Count();
            return average;
        }

        private List<Site> GetTop3Sites(Context db)
        {
            var sites = db.Sites.OrderByDescending(x => x.Rate).Take(3).ToList();
            return sites;
        }

        private List<Site> GetProfilerSites(string userid, ApplicationDbContext udb, Context db)
        {
            List<Site> sites = new List<Site>();
            if (userid == null) { throw new HttpException(404, "Item Not Found"); }
            var id = GetIdThoughtEmail(udb, (string)Session["useremail"]);
            sites = db.Sites.Where(x => x.UserId == id).ToList();
            return sites;
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
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
            using (var db = new Context())
            {
                string[] values = inputData.Split(';');
                Page page = new Page { SiteId = (int)Session["siteid"], Color = values[1], TemplateType = values[2], Title = values[0] ==""? "Page Title": values[0] };
                db.Pages.Add(page);
                CreatePageContent(page.PageId, page.TemplateType, db);
                db.SaveChanges();
                return RedirectToAction("EditSite", new { siteid = (int)Session["siteid"] });
            }
        }

        public PartialViewResult ShowPage(int pageid = 0)
        {
            using (var db = new Context())
            {
                Page page = db.Pages.Where(x => x.PageId == pageid).Include(x => x.Contents).FirstOrDefault();
                return PartialView(String.Format("_RedactPage{0}", page.TemplateType), page);
            }
        }

        public PartialViewResult PreviewShowPage(int pageid = 0)
        {
            using (var db = new Context())
            {
                Page page = db.Pages.Where(x => x.PageId == pageid).Include(x => x.Contents).FirstOrDefault();
                return PartialView(String.Format("_PreviewPage{0}", page.TemplateType), page);
            }
        }

        public ActionResult DeletePage(int pageid = 0)
        {
            using (var db = new Context())
            {
                var page = db.Pages.Where(x => x.PageId == pageid).FirstOrDefault();
                int? siteid = page.SiteId;
                int count_pages = db.Pages.Where(x => x.SiteId == siteid).ToList().Count();
                if (count_pages > 1)
                {
                    DeleteOnePage(page, db);
                    return RedirectToAction("EditSite", new { siteid = siteid });
                }
                else
                {
                    DeleteSiteFromId((int)siteid, db);
                    return RedirectToAction("UserProfile", new { userid = User.Identity.GetUserName() });
                }
            }
        }

        private void DeleteOnePage(Page page, Context db)
        {
            DeleteContentFromPage(page.PageId, db);
            db.Pages.Remove(page);
            db.SaveChanges();
        }

        private void DeleteContentFromPage(int pageId, Context db)
        {
            var contents = db.Contents.Where(x => x.PageId == pageId);
            foreach (var elem in contents)
                db.Contents.Remove(elem);
        }
    }
}