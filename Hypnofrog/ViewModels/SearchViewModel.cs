using Hypnofrog.DBModels;
using Hypnofrog.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hypnofrog.ViewModels
{
    public class SearchViewModel
    {
        public List<SiteViewModel> Sites { get; set; }
        public List<CommentViewModel> Comments { get; set; }
        public List<UserView> Users { get; set; }
        private List<Content> Contents { get; set; }

        public SearchViewModel()
        {
            Sites = new List<SiteViewModel>();
            Comments = new List<CommentViewModel>();
            Users = new List<UserView>();
        }

        public SearchViewModel(string SearchString, string currentuser, bool isadmin)
        {
            Sites = MainService.SearchSites(SearchString, currentuser, isadmin);
            Comments = MainService.SearchComments(SearchString);
            Users = MainService.SearchUsers(SearchString);
            Contents = MainService.SearchContent(SearchString);
            List<SiteViewModel> newsites = MainService.FromContentToSites(Contents, currentuser, isadmin);
            foreach(var elem in newsites)
            {
                if (!Sites.Select(x=>x.SiteId).Contains(elem.SiteId))
                    Sites.Add(elem);
            }
        }
    }
}