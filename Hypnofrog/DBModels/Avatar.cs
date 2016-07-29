using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Hypnofrog.DBModels
{
    public class Avatar
    {
        public int AvatarId { get; set; }

        [DefaultValue("http://cs.pikabu.ru/images/def_avatar/def_avatar_100.png")]
        public string Path { get; set; }

        public string UserId { get; set; }
    }
}