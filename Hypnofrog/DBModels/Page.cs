using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hypnofrog.DBModels
{
    public class Page
    {
        public int PageId { get; set; }
        public string Title { get; set; }
        public string Color { get; set; }
        public string TemplateType { get; set; }
        public virtual ICollection<Content> Contents { get; set; }

        public int? SiteId { get; set; }
        public virtual Site Site { get; set; }

        public Page()
        {
            Contents = new List<Content>();
        }
    }
}