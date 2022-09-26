using DbUp;
using DbUp.Engine;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EfCoreIndexOfStringBug.Database {
    public class DbUpgradeDeployer {

        protected ConnectionString ConnectionString { get; set; }
        protected UpgradeEngine Upgrader { get; set; }

        public DbUpgradeDeployer(
            ConnectionString connectionString,
            IConfiguration configuration
        ) {
            ConnectionString = connectionString;
            Upgrader = DeployChanges.To
                        .SqlDatabase(ConnectionString.Value)
                        .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                        .WithTransactionPerScript()
                        .LogToConsole()
                        .Build();


            if (configuration.GetValue<bool>("CreateDatabase")) {
                EnsureDatabase.For.SqlDatabase(ConnectionString.Value);
            }
        }

        public bool IsUpgradeRequired()
            => Upgrader.IsUpgradeRequired();

        public DatabaseUpgradeResult Upgrade() {
            if (IsUpgradeRequired()) {
                return Upgrader.PerformUpgrade();
            } else {
                return null;
            }
        }
    }
}
