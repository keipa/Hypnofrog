using Hypnofrog.Models;
using Hypnofrog.Repository;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace Hypnofrog.Controllers
{
    public class BaseController : Controller
    {
        [Inject]
        public IRepository Repository { get; set; }

        public string CurrentUserId
        {
            get
            {
                return User.Identity.GetUserId();
            }
        }        
    }
}