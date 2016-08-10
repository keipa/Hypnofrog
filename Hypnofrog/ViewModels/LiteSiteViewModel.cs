using Hypnofrog.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hypnofrog.ViewModels
{
    public class LiteSiteViewModel
    {
        public string Imagepath { get; set; }
        public string Name { get; set; }
        public string Tags { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Userid { get; set; }
        public double Rating { get; set; }

        public LiteSiteViewModel(Site site)
        {
            Imagepath = "http://dummyimage.com/400x150/@newbcolor/@newfcolor&text="+site.Title.Replace(' ', '+');
            Name = site.Title;
            Tags = site.Tags;
            Description = site.Description;
            Url = site.Url;
            Userid = site.UserId;
            Rating = site.Rate;
        }
    }
}