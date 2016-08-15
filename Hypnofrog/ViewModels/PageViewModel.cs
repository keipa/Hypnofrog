using Hypnofrog.DBModels;
using Hypnofrog.Services;
using System.Collections.Generic;
using System.Linq;

namespace Hypnofrog.ViewModels
{
    public class PageViewModel
    {
        public string Style { get; set; }
        public bool IsAdmin { get; set; }
        public string Title { get; set; }
        public int? SiteId { get; set; }
        public int PageId { get; set; }
        public string TemplateType { get; set; }
        public List<Content> Contents { get; set; }
        public string SiteUrl { get; set; }
        public string UserName { get; set; }
        public TemplateViewModel Template { get; set; }

        public PageViewModel()
        {
        }

        public PageViewModel(Page page, bool isadmin, bool preview)
        {
            IsAdmin = isadmin;
            GetPageSettings(this, page, preview);
        }

        public PageViewModel(int pageid, bool preview)
        {
            IsAdmin = true;
            var page = MainService.GetPageById(pageid);
            GetPageSettings(this, page, preview);
        }

        private static void GetPageSettings(PageViewModel vmodel, Page page, bool preview)
        {
            vmodel.PageId = page.PageId;
            vmodel.Title = page.Title;
            vmodel.SiteId = page.SiteId;
            vmodel.Contents = page.Contents.ToList();
            int templid;
            if (int.TryParse(page.TemplateType, out templid))
            {
                vmodel.TemplateType = "own";
                vmodel.Template = MainService.GetTemplate(templid, vmodel.Contents, preview);
            }
            else vmodel.TemplateType = page.TemplateType;
            vmodel.Style = MainService.GetPageStyle(page);
            var site = MainService.GetSite(page.SiteId);
            vmodel.UserName = site.UserId;
            vmodel.SiteUrl = site.Url;
        }
    }
}