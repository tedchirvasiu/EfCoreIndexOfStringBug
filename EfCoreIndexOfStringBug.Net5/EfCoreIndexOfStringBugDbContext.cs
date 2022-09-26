using EfCoreIndexOfStringBug.Net5.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EfCoreIndexOfStringBug.Net5 {
    public class EfCoreIndexOfStringBugDbContext: DbContext {

        public virtual DbSet<Person> People { get; set; }

        public EfCoreIndexOfStringBugDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            RegisterDbFunctions(modelBuilder);
        }

        protected virtual void RegisterDbFunctions(ModelBuilder modelBuilder) {

            var methods = Assembly.GetExecutingAssembly().GetTypes()
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes(typeof(DbFunctionAttribute), true).Length > 0)
                .ToArray();

            foreach (var method in methods) {
                modelBuilder.HasDbFunction(method);
            }
        }

    }
}
