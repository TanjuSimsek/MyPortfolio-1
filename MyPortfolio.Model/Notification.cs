﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPortfolio.Model
{
    public class Notification:BaseEntity
    {
       public string Message { get; set; }
        public string UserName { get; set; }

    }
}