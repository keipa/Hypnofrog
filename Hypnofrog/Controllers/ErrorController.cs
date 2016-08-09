using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hypnofrog.Controllers
{
    public class ErrorController : BaseController
    {
        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View();
        }


    }
}