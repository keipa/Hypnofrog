using Hypnofrog.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hypnofrog.Models;
using Hypnofrog.Services;

namespace Hypnofrog.ViewModels
{

    public class LiteSiteViewModel
    {
        public string Imagepath { get; set; }
        public string Name { get; set; }
        public string[] Tags { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Userid { get; set; }
        public int Siteid { get; set; }
        public double Rating { get; set; }

        public LiteSiteViewModel(Site site)
        {
            var restoredsite = MainService.GetSite(site.SiteId);
            var backgroundcolor = restoredsite.Pages.ElementAt(0).Color;
            var newbcolor = backgroundcolor == "orange" ? "FF7C00" : backgroundcolor == "dark" ? "0C0114" : "9AC6BC";
            var newfcolor = backgroundcolor == "orange" ? "A63400" : backgroundcolor == "dark" ? "BDAECD" : "37474f";
            Imagepath = "http://dummyimage.com/400x150/"+newbcolor+"/"+newfcolor+"&text=" + restoredsite.Title.Replace(' ', '+');
            Name = restoredsite.Title;
            Tags = restoredsite.Tags.Split(',');
            Siteid = restoredsite.SiteId;
            Description = restoredsite.Description;
            Url = restoredsite.Url;
            Userid = restoredsite.UserId;
            Rating = restoredsite.Rate;
        }


  
    }
}