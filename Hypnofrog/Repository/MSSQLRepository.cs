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

        public IQueryable<OwnTemplate> OwnTemplates
        {
            get
            {
                return dbc.OwnTemplates;
            }
        }

        #endregion

        public bool CreateAchievement(Achievement achievement)
        {
            if (achievement != null)
            {
                dbc.Achievements.Add(achievement);
                dbc.SaveChanges();
            }
            return false;
        }

        public bool CreateAvatar(Avatar avatar)
        {
            if (avatar != null)
            {
                dbc.Avatars.Add(avatar);
                dbc.SaveChanges();
                return true;
            }
            return false;
        }

        public bool CreateComment(Comment comment)
        {
            if (comment != null)
            {
                dbc.Comments.Add(comment);
                dbc.SaveChanges();
                return true;
            }
            return false;
        }

        public bool CreateContent(Content content)
        {
            if (content != null)
            {
                dbc.Contents.Add(content);
                dbc.SaveChanges();
                return true;
            }
            return false;
        }

        public bool CreatePage(Page page)
        {
            if (page != null)
            {
                dbc.Pages.Add(page);
                dbc.SaveChanges();
                return true;
            }
            return false;
        }

        public bool CreateRate(Rate rate)
        {
            if (rate != null)
            {
                dbc.RateLog.Add(rate);
                dbc.SaveChanges();
                return true;
            }
            return false;
        }

        public bool CreateSite(Site site)
        {
            if (site != null)
            {
                dbc.Sites.Add(site);
                dbc.SaveChanges();
                return true;
            }
            return false;
        }

        public bool CreateTag(Tag tag)
        {
            if (tag != null)
            {
                Tag oldtag = dbc.Tags.Where(x => x.Name == tag.Name).FirstOrDefault();
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
            return false;
        }

        public bool RemoveComment(int commentId)
        {
            var comment = dbc.Comments.Where(x => x.CommentId == commentId).FirstOrDefault();
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
            var content = dbc.Contents.Where(x => x.ContentId == contentId).FirstOrDefault();
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
            var page = dbc.Pages.Where(x => x.PageId == pageId).FirstOrDefault();
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
            var site = dbc.Sites.Where(x => x.SiteId == siteId).FirstOrDefault();
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
            var tag = dbc.Tags.Where(x => x.TagId == tagId).FirstOrDefault();
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
            var oldavatar = dbc.Avatars.Where(x => x.AvatarId == avatar.AvatarId).FirstOrDefault();
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
            var oldcontent = dbc.Contents.Where(x => x.ContentId == contentid).FirstOrDefault();
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
            var oldrate = dbc.RateLog.Where(x => x.RateId == rate.RateId).FirstOrDefault();
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
            var oldsite = dbc.Sites.Where(x => x.SiteId == site.SiteId).FirstOrDefault();
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
            if (user != null)
            {
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(dbc));
                UserManager.AddToRole(id, "Admin");
                return true;
            }
            return false;
        }

        public bool UserDownInRole(string id)
        {
            var user = dbc.Users.Where(x => x.Id == id);
            if (user != null)
            {
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(dbc));
                UserManager.RemoveFromRole(id, "Admin");
                UserManager.AddToRole(id, "User");
                return true;
            }
            return false;
        }

        public bool CreateTemplate(OwnTemplate template)
        {
            if (template != null)
            {
                template.HtmlRealize = template.HtmlRealize.Replace("</td>", "{c{o{n{t}e}n}t}</td>");
                template.HtmlRealize = template.HtmlRealize.Replace("<td style=\"", "<td style=\"border:0;");
                dbc.OwnTemplates.Add(template);
                dbc.SaveChanges();
                return true;
            }
            return false;
        }

        public bool UpdateTemplate(OwnTemplate template)
        {
            var oldtemplate = dbc.OwnTemplates.Where(x => x.OwnTemplateId == template.OwnTemplateId).FirstOrDefault();
            if (oldtemplate != null)
            {
                oldtemplate = template;
                dbc.SaveChanges();
                return true;
            }
            return false;
        }

        public bool RemoveTemplate(OwnTemplate template)
        {
            var oldtemplate = dbc.OwnTemplates.Where(x => x.OwnTemplateId == template.OwnTemplateId).FirstOrDefault();
            if (oldtemplate != null)
            {
                dbc.OwnTemplates.Remove(oldtemplate);
                dbc.SaveChanges();
                return true;
            }
            return false;
        }
    }
}