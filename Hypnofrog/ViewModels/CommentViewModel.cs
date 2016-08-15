using Hypnofrog.DBModels;
using System;

namespace Hypnofrog.ViewModels
{
    public class CommentViewModel
    {
        public string UserId { get; set; }
        public string UserAvatar { get; set; }
        public int CommentId { get; set; }
        public DateTime CreationTime { get; set; }
        public string Text { get; set; }
        public int SiteId { get; set; }

        public CommentViewModel(Comment comment)
        {
            UserId = comment.UserId;
            UserAvatar = comment.UserAvatar;
            CommentId = comment.CommentId;
            CreationTime = comment.CreationTime;
            Text = comment.Text;
            if (comment.SiteId != null) SiteId = (int)comment.SiteId;
        }
    }
}