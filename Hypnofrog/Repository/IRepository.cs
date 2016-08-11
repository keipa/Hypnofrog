using Hypnofrog.DBModels;
using Hypnofrog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hypnofrog.Repository
{
    public interface IRepository
    {
        IQueryable<ApplicationUser> UsersList { get; }
        bool RemoveUsers(string userId);
        bool UserUpInRole(string id);
        bool UserDownInRole(string id);

        IQueryable<Site> SitesList { get; }
        bool CreateSite(Site site);
        bool UpdateSite(Site site);
        bool RemoveSite(int siteId);

        IQueryable<Page> PageList { get; }
        bool CreatePage(Page page);
        bool RemovePage(int pageId);

        IQueryable<Avatar> AvatarList { get; }
        bool CreateAvatar(Avatar avatar);
        bool UpdateAvatar(Avatar avatar);

        IQueryable<Content> ContentList { get; }
        bool CreateContent(Content content);
        bool UpdateContent(int contentid, string newcontent);
        bool RemoveContent(int contentId);

        IQueryable<Comment> CommentList { get; }
        bool CreateComment(Comment comment);
        bool RemoveComment(int commentId);

        IQueryable<Rate> RateList { get; }
        bool CreateRate(Rate rate);
        bool UpdateRate(Rate rate);

        IQueryable<Tag> TagList { get; }
        bool CreateTag(Tag tag);
        bool RemoveTag(int tagId);

        IQueryable<Achievement> AchievementList { get; }
        bool CreateAchievement(Achievement achievement);
    }
}
