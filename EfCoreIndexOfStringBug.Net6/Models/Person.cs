﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfCoreIndexOfStringBug.Net6.Models {
    public class Person {
        public Guid Id { get; set; }
        public string FullName { get; set; } = "";
    }
}
