﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hypnofrog.DBModels
{
    public class Rate
    {
        public int RateId { get; set; }
        public string Site { get; set; }
        public string User { get; set; }
        public int Value { get; set; }
    }
}