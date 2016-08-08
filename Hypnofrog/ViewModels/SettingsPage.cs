using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hypnofrog.ViewModels
{
    public class SettingsPage
    {
        public int SiteId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string Template { get; set; }
    }
}