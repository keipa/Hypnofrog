using Hypnofrog.DBModels;
using Hypnofrog.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
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
            return PartialView("_ViewConfig");
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
                if (userid == "")
                {
                    userid = User.Identity.Name;
                }
                var avatar = db.Avatars.Where(x => x.UserId == userid).FirstOrDefault();
                ViewBag.avatarpath = avatar.Path;
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