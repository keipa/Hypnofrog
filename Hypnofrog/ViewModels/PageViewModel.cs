using Hypnofrog.DBModels;
using Hypnofrog.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

        public PageViewModel() { }

        public PageViewModel(Page page, bool isadmin)
        {
            IsAdmin = isadmin;
            GetPageSettings(this, page);
        }

        public PageViewModel(int pageid)
        {
            IsAdmin = true;
            var page = MainService.GetPageById(pageid);
            GetPageSettings(this, page);
        }

        private static void GetPageSettings(PageViewModel vmodel, Page page)
        {
            vmodel.PageId = page.PageId;
            vmodel.Title = page.Title;
            vmodel.SiteId = page.SiteId;
            vmodel.TemplateType = page.TemplateType;
            vmodel.Contents = page.Contents.ToList();
            vmodel.Style = MainService.GetPageStyle(page);
            Site site = MainService.GetSite(page.SiteId);
            vmodel.UserName = site.UserId;
            vmodel.SiteUrl = site.Url;
        }
    }
}