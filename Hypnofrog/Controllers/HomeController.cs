using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Hypnofrog.Filters;
using Hypnofrog.Services;
using Hypnofrog.ViewModels;
using Microsoft.AspNet.Identity;

namespace Hypnofrog.Controllers
{
    [Authorize]
    [Culture]
    public class HomeController : Controller
    {
        public HomeController()
        {
            DependencyResolver.Current.GetService<MainService>();
        }

        [AllowAnonymous]
        public ActionResult ChangeCulture(string lang)
        {
            Debug.Assert(Request.UrlReferrer != null, "Request.UrlReferrer != null");
            var returnUrl = Request.UrlReferrer.AbsolutePath;
            var cultures = new List<string> { "ru", "en" };
            if (!cultures.Contains(lang))
            {
                lang = "ru";
            }
            var cookie = Request.Cookies["lang"];
            if (cookie != null)
                cookie.Value = lang;
            else
            {
                cookie = new HttpCookie("lang")
                {
                    HttpOnly = false,
                    Value = lang,
                    Expires = DateTime.Now.AddYears(1)
                };
            }
            Response.Cookies.Add(cookie);
            return Redirect(returnUrl);
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult IndexVm()
        {
            var model = new MainPageViewModel(User.Identity.GetUserName(), User.Identity.IsAuthenticated,
                User.Identity.GetUserId());
            var json = new JavaScriptSerializer().Serialize(model);
            return Content(json, "application/json");
        }

        [AllowAnonymous]
        [Route("User/{userid}")]
        public ActionResult UserProfile(string userid)
        {
            ViewBag.Achievment = MainService.CheckAchievments(User.Identity.GetUserId(), User.Identity.GetUserName());
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
            return siteid == 0
                ? View("Error")
                : View(new SiteViewModel(siteid, User.Identity.GetUserName(), User.IsInRole("Admin")));
        }

        [ValidateInput(false)]
        public void SavePage(PageViewModel model, List<string> htmlContent)
        {
            MainService.SavePageTitleAndContent((int)Session["PageId"], model.Title, htmlContent);
        }

        [HttpPost]
        public ActionResult CreateSite(string inputData)
        {
            return RedirectToAction("EditSite",
                new { siteid = MainService.CreateSite(inputData, User.Identity.GetUserName()) });
        }

        public ActionResult Creating()
        {
            return PartialView("_ViewConfig", new SettingsModel { UserId = User.Identity.GetUserId() });
        }

        public ActionResult CreatingPage(int siteid = 0)
        {
            var menutype = MainService.GetSiteMenu(siteid);
            Session["menu"] = menutype;
            Session["siteid"] = siteid;
            return PartialView("_ViewConfigPage", new SettingsModel(menutype));
        }

        public PartialViewResult ChangeTemplate(SettingsModel model)
        {
            ReChangeSession(model);
            var url = SettingsModel.CreatePhoto((string)Session["color"], (string)Session["menu"],
                (string)Session["template"]);
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
            if (Request == null || !Request.IsAjaxRequest()) return Content(bool.FalseString);
            Thread.Sleep(1000);
            return Content(bool.TrueString);
        }

        [HttpPost]
        public ActionResult AddPage(string inputData)
        {
            var siteid = (int)Session["siteid"];
            if (!MainService.CreatePage(inputData, siteid))
                throw new HttpException(500, "Sorry, but smth wrong with server.");
            return RedirectToAction("EditSite", new { siteid });
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

        public PartialViewResult SettingsConf(SettingsModel site)
        {
            if (!MainService.SiteConfirm((int)Session["currentsite"], site))
                throw new HttpException(500, "Sorry, but smth wrong with server.");
            return PartialView("_SiteTable", MainService.GetUserSites((int)Session["currentsite"]));
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


        [AllowAnonymous]
        [Route("AllUsersVM")]
        public ActionResult AllUsersVm()
        {
            var model = MainService.GetAllUsers();
            var json = new JavaScriptSerializer().Serialize(model);
            return Content(json, "application/json");
        }


        [AllowAnonymous]
        [Route("WhoamiVM")]
        public ActionResult Whoami()
        {
            var model = MainService.GetCurrentUser(User.Identity.Name, User.IsInRole("Admin"), User.Identity.GetUserId());
            var json = new JavaScriptSerializer().Serialize(model);
            return Content(json, "application/json");
        }


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
            var siteid = MainService.DeletePageOrSite(pageid);
            if (siteid > 0)
            {
                return RedirectToAction("EditSite", new { siteid });
            }
            return RedirectToAction("UserProfile", new { userid = User.Identity.GetUserName() });
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
            return View("Search", "");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateInput(false)]
        [Route("SearchResults")]
        public ActionResult Search(string searchstring = "")
        {
            ViewBag.searchstring = searchstring;
            return View();
        }

        [AllowAnonymous]
        [Route("SearchVm/{searchstring}")]
        public ActionResult SearchVm(string searchstring = "")
        {
            var model = new SearchViewModel(searchstring, User.Identity.GetUserName(), User.IsInRole("Admin"));
            var json = new JavaScriptSerializer().Serialize(model);
            return Content(json, "application/json");
        }

        public ActionResult SaveUploadedFile()
        {
            var fName = "";
            try
            {
                foreach (string fileName in Request.Files)
                {
                    var file = Request.Files[fileName];
                    if (file != null && file.ContentLength > 0)
                        return SavingAvatar(out fName, file);
                }
            }
            catch (Exception)
            {
                return Json(new { Message = "Error in saving file" });
            }
            return Json(new { Message = fName });
        }

        private ActionResult SavingAvatar(out string fName, HttpPostedFileBase file)
        {
            MainService.SaveAvatar(out fName, file, User.Identity.GetUserName(),
                new DirectoryInfo($"{Server.MapPath(@"\")}Images\\WallImages"));
            return RedirectToAction("UserProfile", new { userid = User.Identity.Name });
        }

        [Route("Famehall")]
        public ActionResult Famehall()
        {
            var achievments = MainService.GetAchivmentsChecker(User.Identity.GetUserId(), User.Identity.GetUserName());
            var userachievments = MainService.GetUserAchivments(User.Identity.GetUserId()).Select(x => x.Name).ToList();
            return
                View(MainService.GetKeyValueAchievments(achievments.GetAllAchievments(),
                    achievments.GetAllAchievmentsDescriptionsRu(), userachievments));
        }

        public PartialViewResult UpdateRating(string userid, string siteid, string value)
        {
            return PartialView("_UpdateRatingResult", MainService.GetRateMessage(userid, siteid, value));
        }

        [ValidateInput(false)]
        public PartialViewResult CommentSite(string newComment, int siteid)
        {
            if (!MainService.SaveNewComment(newComment, siteid, User.Identity.GetUserName()))
                throw new HttpException(500, "Sorry, but smth wrong with server.");
            return PartialView("_Comments", MainService.GetSiteComments(siteid));
        }

        public PartialViewResult DeleteCommentPV(int comid, int siteid)
        {
            if (!MainService.DeleteComment(comid))
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


        [Route("DeleteComment/{comid}")]
        public void DeleteComment(string comid = "")
        {
            if (!MainService.DeleteComment(Convert.ToInt32(comid)))
                throw new HttpException(404, "This comment is removed resently.");
        }
    }
}