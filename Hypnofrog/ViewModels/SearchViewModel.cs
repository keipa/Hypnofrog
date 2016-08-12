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
        public List<LiteSiteViewModel> Sites { get; set; }
        public List<CommentViewModel> Comments { get; set; }
        public List<UserView> Users { get; set; }
        //private List<Content> Contents { get; set; }
        public string searchString { get; set; }

        public SearchViewModel()
        {
            Sites = new List<LiteSiteViewModel>();
            Comments = new List<CommentViewModel>();
            Users = new List<UserView>();
        }

        public SearchViewModel(string SearchString, string currentuser, bool isadmin)
        {
            searchString = SearchString;
            Sites = MainService.SearchLiteSites(SearchString, currentuser, isadmin);
            Comments = MainService.SearchComments(SearchString);
            Users = MainService.SearchUsers(SearchString);
            var Contents = MainService.SearchContent(SearchString);
            List<LiteSiteViewModel> newsites = MainService.FromContentToLiteSites(Contents, currentuser, isadmin);
            foreach(var elem in newsites)
            {
                if (!Sites.Select(x=>x.Siteid).Contains(elem.Siteid))
                    Sites.Add(elem);
            }
        }
    }
}