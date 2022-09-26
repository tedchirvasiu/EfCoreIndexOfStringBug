using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfCoreIndexOfStringBug.Net5 {
    public static class DbStringFunctions {

        [DbFunction("NormalizeString", "dbo")]
        public static string NormalizeString(string str)
            => throw new NotImplementedException();

        [DbFunction("IndexOf", "dbo")]
        public static int IndexOf(string input, string stringToMatch)
            => throw new NotImplementedException();

    }
}
