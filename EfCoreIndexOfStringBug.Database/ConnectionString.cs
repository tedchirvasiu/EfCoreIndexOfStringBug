using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfCoreIndexOfStringBug.Database {
    public class ConnectionString {
        public string Value { get; }

        public ConnectionString(string connectionString) {
            Value = connectionString;
        }
    }
}
