﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfCoreIndexOfStringBug.NetCore3_1.Models {
    public class Person {
        public Guid Id { get; set; }
        public string FullName { get; set; } = "";
    }
}
