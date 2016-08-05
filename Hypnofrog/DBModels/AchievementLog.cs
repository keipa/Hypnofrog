using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hypnofrog.DBModels
{
    public class Achievement
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string User { get; set; }
        public DateTime Time { get; set; }
    }
}