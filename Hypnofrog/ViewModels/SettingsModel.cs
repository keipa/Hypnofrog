using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hypnofrog.ViewModels
{
    public class SettingsModel
    {
        public string UserId { get; set; }
        public bool isActive { get; set; }
        public string Color { get; set; }
        public string Menu { get; set; }
        public string Template { get; set; }
        public string Url { get; set; }
        public bool CommentsAvailable { get; set; }
        public static string CreatePhoto(string Color, string Menu, string Template)
        {
            return String.Format("http://res.cloudinary.com/dldmfb5fo/image/upload/v1469794513/{0}_{1}_{2}.png", Color, Menu, Template);
        }
    }
}