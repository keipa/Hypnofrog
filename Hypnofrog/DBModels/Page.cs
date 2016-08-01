using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hypnofrog.DBModels
{
    public class Page
    {
        public int PageId { get; set; }
        public int Site { get; set; }
        public string Title { get; set; }
        public string Color { get; set; }
        public string TemplateType { get; set; }
        public bool HasComments { get; set; }
    }
}