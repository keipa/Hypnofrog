using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hypnofrog.DBModels
{
    public class Avatar
    {
        public int AvatarId { get; set; }
        public string Path { get; set; }
        public string UserId { get; set; }
    }
}