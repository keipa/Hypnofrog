using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hypnofrog.DBModels
{
    public class Tag
    {
        public int TagId { get; set; }
        public string Name { get; set; }
        public int Repeats { get; set; }
    }
}