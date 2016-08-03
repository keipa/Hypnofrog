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

            bundles.Add(new StyleBundle("~/Content/userprofile").Include(
                 "~/Content/userprofilestyle.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/Site.css",
                      "~/Content/popupwindows.css"));

            bundles.Add(new ScriptBundle("~/bundles/vertical").Include(
                     "~/Scripts/verticalmenuj.js"));

            bundles.Add(new ScriptBundle("~/bundles/froalajs").Include(
                     "~/Scripts/froala/js/froala_editor.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/wordcloud").Include(
         "~/Scripts/wordcloud2.js"));


            bundles.Add(new StyleBundle("~/bundles/verticalstyle").Include(
                     "~/Content/verticalmenu.css"));

            bundles.Add(new ScriptBundle("~/bundles/dropzonescripts").Include(
                     "~/Scripts/dropzone/dropzone.js"));

            bundles.Add(new StyleBundle("~/bundles/dropzonescss").Include(
                     "~/Scripts/dropzone/basic.css",
                     "~/Scripts/dropzone/dropzone.css"));

            bundles.Add(new StyleBundle("~/bundles/froalacss").Include(
                     "~/Scripts/froala/css/froala_editor.css",
                     "~/Scripts/froala/css/froala_style.css"));

            bundles.Add(new StyleBundle("~/bundles/froala-plugins-css").Include(
                    "~/Scripts/froala/css/plugins/code_view.min.css",
                    "~/Scripts/froala/css/plugins/colors.css",
                    "~/Scripts/froala/css/plugins/emoticons.min.css",
                    "~/Scripts/froala/css/plugins/file.min.css",
                    "~/Scripts/froala/css/plugins/fullscreen.min.css",
                    "~/Scripts/froala/css/plugins/image.min.css",
                    "~/Scripts/froala/css/plugins/image_manager.min.css",
                    "~/Scripts/froala/css/plugins/line_breaker.min.css",
                    "~/Scripts/froala/css/plugins/quick_insert.min.css",
                    "~/Scripts/froala/css/plugins/table.min.css",
                    "~/Scripts/froala/css/plugins/video.min.css",
                    "~/Scripts/froala/css/plugins/char_counter.min.css",
                    "~/Scripts/froala/css/themes/dark.min.css",
                    "~/Scripts/froala/css/themes/gray.min.css",
                    "~/Scripts/froala/css/themes/red.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/froala-plugins-js").Include(
                    "~/Scripts/froala/js/froala_editor.min.js",
                    "~/Scripts/froala/js/plugins/align.min.js",
                    "~/Scripts/froala/js/plugins/char_counter.min.js",
                    "~/Scripts/froala/js/plugins/code_beautifier.min.js",
                    "~/Scripts/froala/js/plugins/code_view.min.js",
                    "~/Scripts/froala/js/plugins/colors.min.js",
                    "~/Scripts/froala/js/plugins/draggable.min.js",
                    "~/Scripts/froala/js/plugins/emoticons.min.js",
                    "~/Scripts/froala/js/plugins/entities.min.js",
                    "~/Scripts/froala/js/plugins/file.min.js",
                    "~/Scripts/froala/js/plugins/font_family.min.js",
                    "~/Scripts/froala/js/plugins/font_size.min.js",
                    "~/Scripts/froala/js/plugins/forms.min.js",
                    "~/Scripts/froala/js/plugins/fullscreen.min.js",
                    "~/Scripts/froala/js/plugins/image.min.js",
                    "~/Scripts/froala/js/plugins/image_manager.min.js",
                    "~/Scripts/froala/js/plugins/inline_style.min.js",
                    "~/Scripts/froala/js/plugins/line_breaker.min.js",
                    "~/Scripts/froala/js/plugins/link.min.js",
                    "~/Scripts/froala/js/plugins/lists.min.js",
                    "~/Scripts/froala/js/plugins/paragraph_format.min.js",
                    "~/Scripts/froala/js/plugins/paragraph_style.min.js",
                    "~/Scripts/froala/js/plugins/quick_insert.min.js",
                    "~/Scripts/froala/js/plugins/quote.min.js",
                    "~/Scripts/froala/js/plugins/save.min.js",
                    "~/Scripts/froala/js/plugins/table.min.js",
                    "~/Scripts/froala/js/plugins/url.min.js",
                    "~/Scripts/froala/js/plugins/video.min.js",
                    "~/Scripts/froala/js/languages/ru.js"));
        }
    }
}
