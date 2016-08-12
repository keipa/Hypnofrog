using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hypnofrog.DBModels;
using Hypnofrog.Models;
using System.Data.Entity;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Hypnofrog.Repository
{
    public class MSSQLRepository : IRepository
    {
        private ApplicationDbContext dbc;

        public MSSQLRepository()
        {
            dbc = DependencyResolver.Current.GetService<ApplicationDbContext>();
        }

        public IQueryable<Achievement> AchievementList
        {
            get
            {
                return dbc.Achievements;
            }
        }
        #region LISTS
        public IQueryable<Avatar> AvatarList
        {
            get
            {
                return dbc.Avatars;
            }
        }

        public IQueryable<Comment> CommentList
        {
            get
            {
                return dbc.Comments;
            }
        }

        public IQueryable<Content> ContentList
        {
            get
            {
                return dbc.Contents;
            }
        }

        public IQueryable<Page> PageList
        {
            get
            {
                return dbc.Pages;
            }
        }

        public IQueryable<Rate> RateList
        {
            get
            {
                return dbc.RateLog;
            }
        }

        public IQueryable<Site> SitesList
        {
            get
            {
                return dbc.Sites;
            }
        }

        public IQueryable<Tag> TagList
        {
            get
            {
                return dbc.Tags;
            }
        }

        public IQueryable<ApplicationUser> UsersList
        {
            get
            {
                return dbc.Users;
            }
        }

        #endregion

        public bool CreateAchievement(Achievement achievement)
        {
            if (achievement == null) return false;
            dbc.Achievements.Add(achievement);
            dbc.SaveChanges();
            return false;
        }

        public bool CreateAvatar(Avatar avatar)
        {
            if (avatar == null) return false;
            dbc.Avatars.Add(avatar);
            dbc.SaveChanges();
            return true;
        }

        public bool CreateComment(Comment comment)
        {
            if (comment == null) return false;
            dbc.Comments.Add(comment);
            dbc.SaveChanges();
            return true;
        }

        public bool CreateContent(Content content)
        {
            if (content == null) return false;
            dbc.Contents.Add(content);
            dbc.SaveChanges();
            return true;
        }

        public bool CreatePage(Page page)
        {
            if (page == null) return false;
            dbc.Pages.Add(page);
            dbc.SaveChanges();
            return true;
        }

        public bool CreateRate(Rate rate)
        {
            if (rate == null) return false;
            dbc.RateLog.Add(rate);
            dbc.SaveChanges();
            return true;
        }

        public bool CreateSite(Site site)
        {
            if (site == null) return false;
            dbc.Sites.Add(site);
            dbc.SaveChanges();
            return true;
        }

        public bool CreateTag(Tag tag)
        {
            if (tag == null) return false;
            var oldtag = dbc.Tags.FirstOrDefault(x => x.Name == tag.Name);
            if (oldtag == null)
            {
                dbc.Tags.Add(tag);
            }
            else
            {
                oldtag.Repeats += 1;
            }
            dbc.SaveChanges();
            return true;
        }

        public bool RemoveComment(int commentId)
        {
            var comment = dbc.Comments.FirstOrDefault(x => x.CommentId == commentId);
            if (comment != null)
            {
                dbc.Comments.Remove(comment);
                dbc.SaveChanges();
                return true;
            }
            return false;
        }

        public bool RemoveContent(int contentId)
        {
            var content = dbc.Contents.FirstOrDefault(x => x.ContentId == contentId);
            if (content != null)
            {
                dbc.Contents.Remove(content);
                dbc.SaveChanges();
                return true;
            }
            return false;
        }

        public bool RemovePage(int pageId)
        {
            var page = dbc.Pages.FirstOrDefault(x => x.PageId == pageId);
            if (page != null)
            {
                dbc.Pages.Remove(page);
                dbc.SaveChanges();
                return true;
            }
            return false;
        }

        public bool RemoveSite(int siteId)
        {
            var site = dbc.Sites.FirstOrDefault(x => x.SiteId == siteId);
            if (site != null)
            {
                dbc.Sites.Remove(site);
                dbc.SaveChanges();
                return true;
            }
            return false;
        }

        public bool RemoveTag(int tagId)
        {
            var tag = dbc.Tags.FirstOrDefault(x => x.TagId == tagId);
            if (tag != null)
            {
                dbc.Tags.Remove(tag);
                dbc.SaveChanges();
                return true;
            }
            return false;
        }

        public bool RemoveUsers(string userId)
        {
            var user = dbc.Users.Where(x => x.Id == userId).Include(x => x.Roles).FirstOrDefault();
            if (user != null)
            {
                dbc.Users.Remove(user);
                dbc.SaveChanges();
                return true;
            }
            return false;
        }

        public bool UpdateAvatar(Avatar avatar)
        {
            var oldavatar = dbc.Avatars.FirstOrDefault(x => x.AvatarId == avatar.AvatarId);
            if (oldavatar != null)
            {
                oldavatar = avatar;
                dbc.SaveChanges();
                return true;
            }
            return false;
        }

        public bool UpdateContent(int contentid, string newcontent)
        {
            var oldcontent = dbc.Contents.FirstOrDefault(x => x.ContentId == contentid);
            if (oldcontent != null)
            {
                oldcontent.HtmlContent = newcontent;
                dbc.SaveChanges();
                return true;
            }
            return false;
        }

        public bool UpdateRate(Rate rate)
        {
            var oldrate = dbc.RateLog.FirstOrDefault(x => x.RateId == rate.RateId);
            if (oldrate != null)
            {
                oldrate = rate;
                dbc.SaveChanges();
                return true;
            }
            return false;
        }

        public bool UpdateSite(Site site)
        {
            var oldsite = dbc.Sites.FirstOrDefault(x => x.SiteId == site.SiteId);
            if (oldsite != null)
            {
                oldsite = site;
                dbc.SaveChanges();
                return true;
            }
            return false;
        }

        public bool UserUpInRole(string id)
        {
            var user = dbc.Users.Where(x => x.Id == id);
            if (user == null) return false;
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(dbc));
            UserManager.AddToRole(id, "Admin");
            return true;
        }

        public bool UserDownInRole(string id)
        {
            var user = dbc.Users.Where(x => x.Id == id);
            if (user == null) return false;
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(dbc));
            UserManager.RemoveFromRole(id, "Admin");
            UserManager.AddToRole(id, "User");
            return true;
        }
    }
}