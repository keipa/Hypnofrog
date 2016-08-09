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
        public Page Page { get; set; }
        public string SiteUrl { get; set; }
        public string UserName { get; set; }

        public PageViewModel() { }

        public PageViewModel(Page page, bool isadmin)
        {
            this.Page = page;
            IsAdmin = isadmin;
            Style = MainService.GetPageStyle(Page);
        }
    }
}