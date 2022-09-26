using EfCoreIndexOfStringBug.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace EfCoreIndexOfStringBug.Net5 {
    public class Startup {

        IConfiguration Configuration { get; set; }

        public Startup(
            IConfiguration configuration
        ) {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services) {

            var connectionString = new ConnectionString(Configuration.GetConnectionString("EfCoreIndexOfStringBugConnection"));

            services.AddSingleton(connectionString);

            services.AddDbContext<EfCoreIndexOfStringBugDbContext>((serviceProvider, ctxBuilder) => {

                var connection = new SqlConnection(serviceProvider.GetRequiredService<ConnectionString>().Value);

                ctxBuilder
                    .UseSqlServer(connection);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseSqlServer(Configuration);

            app.UseEndpoints(endpoints => {
                endpoints.MapGet("/", async context => {

                    var dbContext = context.RequestServices.GetService<EfCoreIndexOfStringBugDbContext>();
                    var str = "stan";

                    //Works
                    var workaroundQuery = await dbContext.People
                        .Where(person => person.FullName.Contains(str))
                        .OrderBy(person => DbStringFunctions.IndexOf(DbStringFunctions.NormalizeString(person.FullName), str))
                        .ToListAsync();

                    //Throws: 'Object reference not set to an instance of an object.'
                    var normalQuery = await dbContext.People
                        .Where(person => person.FullName.Contains(str))
                        .OrderBy(person => DbStringFunctions.NormalizeString(person.FullName).IndexOf(str))
                        .ToListAsync();

                    await context.Response.WriteAsync("All works!");
                });
            });
        }
    }
}
