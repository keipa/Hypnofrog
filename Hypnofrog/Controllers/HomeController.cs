using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Hypnofrog.DBModels;
using Hypnofrog.Models;
using Hypnofrog.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hypnofrog.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
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
            using(var db = new ApplicationDbContext())
            {
                 list_of_users = db.Users.ToList();
            }
            return View(list_of_users);
        }

        public ActionResult Delete(string id = "")
        {
            if (id == "") return RedirectToAction("AllUsers");
            using(var db = new ApplicationDbContext())
            {
                var user = db.Users.Where(x => x.Id == id).FirstOrDefault();
                db.Users.Remove(user);
                db.SaveChanges();
            }
            return RedirectToAction("AllUsers");
        }

        public ActionResult Creating()
        {

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

        [HttpPost]
        public ActionResult CreateSite(string inputData)

        {
            //CREATING SITE
            //inputData - строка в которой идут свойства через ';' т.е. ""Name;Desk;url;comments;white;horizontal;mixed;""
            //после их сплита можно в бд кидать
            string[] param = inputData.Split(';');

            var dbsite = new Site {};

            using (var db = new Context())
            {
                var site = new Site { CreationTime = DateTime.Now, Title = param[0], Description = param[1], Url = param[2], Iscomplited = false, MenuId = 0, MenuType = param[5], UserId = User.Identity.GetUserId(), SiteId = db.Sites.Count() + 1 };
                dbsite = site;
                db.Sites.Add(site);
                var page = new Page { PageId = db.Pages.Count() + 1, Site = site.SiteId, Color = param[4], HasComments = param[3] == "true" ? true : false, TemplateType = param[6], Title = "Page Title" };
                db.Pages.Add(page);
                db.SaveChanges();
            }
                return RedirectToAction("UserProfile", new {userid = User.Identity.Name });
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
                        if (!isExists)
                            Directory.CreateDirectory(pathString);
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
            if (isSavedSuccessfully)
            {
                return Json(new { Message = fName });
            }
            else
            {
                return Json(new { Message = "Error in saving file" });
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult DeleteSite(int siteid)
        {
            using (var db = new Context())
            {
                var pages = db.Pages.Where(l => l.Site == siteid);
                foreach (var item in pages)
                {
                    db.Pages.Remove(item);
                }
                var site = db.Sites.Where(x => x.SiteId == siteid).FirstOrDefault();
                db.Sites.Remove(site);
                db.SaveChanges(); 

               
            }
            return RedirectToAction("UserProfile", new { userid = User.Identity.Name });
        }

        public ActionResult UserProfile(string userid)
        {  
            using (var db = new Context())
            {               
                var avatar = db.Avatars.Where(x => x.UserId == userid).FirstOrDefault();
                ViewBag.Email = userid;
                ViewBag.avatarpath = avatar != null? avatar.Path : "";
            }
            return View(GetProfilerSites(userid));
        }



        private List<Site> GetProfilerSites (string userid)
        {
            List<Site> sites = new List<Site>();

            using (var db = new Context())
            {
                using (var udb = new ApplicationDbContext())
                {
                    var id = udb.Users.Where(y => y.Email == userid).FirstOrDefault().Id;
                    sites = db.Sites.Where(x => x.UserId == id).ToList();
            }
            }
            return sites;
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}