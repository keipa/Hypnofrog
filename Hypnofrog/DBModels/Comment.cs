using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hypnofrog.DBModels
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string Text { get; set; }
        public DateTime CreationTime { get; set; }
        public string UserId { get; set; }
        public string UserAvatar { get; set; }


        public int? SiteId { get; set; }
        public virtual Site Site { get; set; }
    }
}