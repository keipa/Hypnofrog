﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Hypnofrog.DBModels
{
    public class Context: DbContext
    {
        public DbSet<Avatar> Avatars { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<Rate> RateLog { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public Context() : base("DefaultConnection") { }
    }
}