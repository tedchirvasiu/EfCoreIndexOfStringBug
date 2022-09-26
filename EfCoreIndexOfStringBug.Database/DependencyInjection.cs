using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfCoreIndexOfStringBug.Database {
    public static class DependencyInjection {
        public static IApplicationBuilder UseSqlServer(this IApplicationBuilder app, IConfiguration configuration) {

            var serviceProvider = app.ApplicationServices;
            var connectionString = serviceProvider.GetService<ConnectionString>();
            var dbUpgrader = new DbUpgradeDeployer(connectionString, configuration);
            var result = dbUpgrader.Upgrade();
            if (result?.Successful == false)
                throw result.Error;

            return app;
        }
    }
}
