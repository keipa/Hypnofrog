using Hypnofrog.DBModels;
using Hypnofrog.Services;
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
        public string Name { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public string Menu { get; set; }
        public string Template { get; set; }
        public string Url { get; set; }
        public bool CommentsAvailable { get; set; }
        public string Tags { get; set; }
        public string CurrentTags { get; set; }
        public string OwnTemplate { get; set; }
        public string SiteUrl { get; set; }

        public SettingsModel()
        {
            isActive = false;
            Color = "dark";
            Menu = "without";
            Template = "solid";
            CommentsAvailable = false;
            Url = SettingsModel.CreatePhoto("dark", "without", "solid");
            Tags = MainService.GetMainTags();
        }

        public SettingsModel(string menutype)
        {
            Color = "dark";
            Template = "solid";
            Url = SettingsModel.CreatePhoto("dark", menutype, "solid");
        }

        public SettingsModel(int siteid)
        {
            Site site = MainService.GetSite(siteid);
            Name = site.Title;
            Description = site.Description;
            CurrentTags = site.Tags;
            Tags = MainService.GetMainTags();
            CommentsAvailable = site.HasComments;
            Url = site.Url;
        }

        public static string CreatePhoto(string Color, string Menu, string Template)
        {
            return String.Format("http://res.cloudinary.com/dldmfb5fo/image/upload/v1469794513/{0}_{1}_{2}.png", Color, Menu, Template);
        }
    }
}