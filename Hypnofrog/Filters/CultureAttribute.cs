using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Web.Mvc;

namespace Hypnofrog.Filters
{
    public class CultureAttribute : FilterAttribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var cultureCookie = filterContext.HttpContext.Request.Cookies["lang"];
            var cultureName = cultureCookie != null ? cultureCookie.Value : "ru";
            
            var cultures = new List<string>() { "ru", "en"};
            if (!cultures.Contains(cultureName))
            {
                cultureName = "ru";
            }
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureName);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(cultureName);
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
        }
    }
}