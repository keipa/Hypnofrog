using System.Web;
using System.Web.Optimization;

namespace Hypnofrog
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/popup").Include(
                        "~/Scripts/PopUp.js",
                        "~/Scripts/customcontrols.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-ui").Include(
                        "~/Scripts/jquery-ui-1.12.0.js",
                        "~/Scripts/jquery-ui-1.12.0.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/Site.css",
                      "~/Content/popupwindows.css"));


            bundles.Add(new ScriptBundle("~/bundles/dropzonescripts").Include(
                     "~/Scripts/dropzone/dropzone.js"));

            bundles.Add(new StyleBundle("~/bundles/dropzonescss").Include(
                     "~/Scripts/dropzone/basic.css",
                     "~/Scripts/dropzone/dropzone.css"));
        }
    }
}
