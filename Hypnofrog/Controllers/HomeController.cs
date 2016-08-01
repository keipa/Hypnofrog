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
            //inputData - строка в которой идут свойства через ';' т.е. "dark;vertical;mixed;"
            //после их сплита можно в бд кидать
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
                        return RedirectToAction("UserProfile");
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

        public ActionResult UserProfile(string userid = "")
        {
            using (var db = new Context())
            {
                //db.Sites.Add(new Site() { CreationTime = DateTime.Now, Description = "kek", Iscomplited = false, MenuId = 1488, MenuType ="topkek", SiteId= 34567, Title= "pizda",UserId="pohui" });
                //db.Pages.Add(new Page() { UserId = "ejje", Title = "wej", Color = "dfklj", HasComments = false,PageId = "kek",TemplateType = "kek" });
                //db.SaveChanges();

                if (userid == "")
                {
                    ViewBag.IsMyProfile = true;
          
                    userid = User.Identity.Name;
                }
                else
                {
                    if (userid == User.Identity.Name)
                    {
                        ViewBag.IsMyProfile = true;

                    }
                    else
                    {
                        ViewBag.IsMyProfile = false;

                    }

                }
                var avatar = db.Avatars.Where(x => x.UserId == userid).FirstOrDefault();
                ViewBag.avatarpath = avatar != null? avatar.Path : "";
            }
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}