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
                    ViewBag.Email = GetTopUser(db, udb);
                    ViewBag.Avatarpath = GetTopUsersAvatar(db, ViewBag.Email);
                    return View(GetTop3Sites(db));
                }
            }
        }

        private string GetTopUsersAvatar(Context db, string email)
        {
            return db.Avatars.Where(x => x.UserId == email).FirstOrDefault().Path;
        }

        private string GetTopUser(Context db, ApplicationDbContext udb)
        {
            var userid = db.Sites.OrderByDescending(x => x.Rate).FirstOrDefault().UserId;
            return udb.Users.Where(x => x.Id == userid).FirstOrDefault().Email;
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Admin()
        {
            return RedirectToAction("AllUsers");
        }

        public ActionResult Users()
        {
            return RedirectToAction("AllUsers");
        }

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

        public PartialViewResult ChangeTemplate(SettingsModel model)
        {
            return PartialView("_ColorTemplate", SettingsModel.CreatePhoto(model.Color, model.Menu, model.Template));
        }

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
                var site = new Site { CreationTime = DateTime.Now, Title = param[0], Description = param[1], Url = param[2], Iscomplited = false, MenuType = param[5], UserId = User.Identity.GetUserId(), Rate = 0.0, Tags = param[7]};
                db.Sites.Add(site);
                dbsite = site;
                page = new Page { SiteId = site.SiteId, Color = param[4], HasComments = param[3] == "true" ? true : false, TemplateType = param[6], Title = "Page Title" };
                db.Pages.Add(page);
                AddUpdateNewTags(param[7], db);

                db.SaveChanges();
            }
            CreatePageContent(page.PageId, param[6]);
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
                model = db.Sites.Where(x => x.SiteId == siteid).Include(x => x.Pages).FirstOrDefault();
                var pages = db.Pages.Where(x => x.SiteId == siteid).Include(x => x.Contents).ToList();
                model.Pages = pages;
            }
            ViewBag.PageTitles = FromPageTitles(model);
            return View(model);
        }

        private List<string> FromPageTitles(Site model)
        {
            var obj = model.Pages.Select(x => x.Title);
            List<string> titles = new List<string>();
            foreach (var elem in obj)
            {
                titles.Add(Regex.Replace(elem, "<[^>]+>", string.Empty));
            }
            return titles;
        }

        private void CreatePageContent(int pageid, string type)
        {
            int count = type == "mixed" ? 3 : type == "solid" ? 1 : 2;
            using (var db = new Context())
            {
                for (int i = 0; i < count; i++)
                    db.Contents.Add(new Content() { HtmlContent = "", PageId = pageid });
                db.SaveChanges();
            }
        }

        [ValidateInput(false)]
        public PartialViewResult SavePage(Page model, List<string> HtmlContent)
        {
            SavePageToDB(model, (int)Session["PageId"]);
            SavePageContentToDB((int)Session["PageId"], HtmlContent);
            Page page = GetPageFromId((int)Session["PageId"]);
            return PartialView(String.Format("_RedactPage{0}", page.TemplateType), page);
        }

        private void SavePageContentToDB(int pageid, List<string> htmlContent)
        {
            using (var db = new Context())
            {
                var contents = db.Contents.Where(x => x.PageId == pageid).ToList();
                for (int i = 0; i < contents.Count; i++)
                    contents[i].HtmlContent = htmlContent[i];
                db.SaveChanges();
            }
        }

        private void SavePageToDB(Page model, int pageid)
        {
            using (var db = new Context())
            {
                Page page = db.Pages.Where(x => x.PageId == pageid).FirstOrDefault();
                page.Title = model.Title;
                db.SaveChanges();
            }
        }

        private Page GetPageFromId(int pageid)
        {
            Page page;
            using (var db = new Context())
                page = db.Pages.Where(x => x.PageId == pageid).Include(x => x.Contents).FirstOrDefault();
            return page;
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
            catch (Exception ex)
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

        public PartialViewResult DeleteSite(int siteid)
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
                    userid = (string)Session["useremail"];
                    db.Sites.Remove(site);
                    db.SaveChanges();
                    return PartialView("_SiteTable", GetProfilerSites(userid != "" ? userid : User.Identity.GetUserName(), udb, db));
                }
            }
        }

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
                    return View(GetProfilerSites(userid, applicationdb, db));
                }
            }

        }

        private string GetAllTags()
        {
            string tagstring = "";
            using (var db = new Context())
            {
                var tags = db.Tags.ToList();
                foreach (var item in tags)
                {
                    tagstring += item.Name + " " + item.Repeats.ToString() + ";";
                }
                return tagstring.Remove(tagstring.Length - 1); ;
            }

        }

        private string GetIdThoughtEmail(ApplicationDbContext applicationdb, string email)
        {
            return applicationdb.Users.Where(x => x.Email == email).FirstOrDefault().Id;
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
    }
}