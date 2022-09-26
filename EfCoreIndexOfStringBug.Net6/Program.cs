using EfCoreIndexOfStringBug.Database;
using EfCoreIndexOfStringBug.Net6;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

var connectionString = new ConnectionString(builder.Configuration.GetConnectionString("EfCoreIndexOfStringBugConnection"));

builder.Services.AddSingleton(connectionString);

builder.Services.AddDbContext<EfCoreIndexOfStringBugDbContext>((serviceProvider, ctxBuilder) => {

    var connection = new SqlConnection(serviceProvider.GetRequiredService<ConnectionString>().Value);

    ctxBuilder
        .UseSqlServer(connection);
});

var app = builder.Build();

app.UseSqlServer(app.Configuration);

app.MapGet("/", async (EfCoreIndexOfStringBugDbContext dbContext) => {

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

});

app.Run();
