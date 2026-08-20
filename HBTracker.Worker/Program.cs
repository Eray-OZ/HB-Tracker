using HBTracker.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;




HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new ArgumentException("Connection String Cannot Be Null");
}

builder.Services.AddDbContext<HBTrackerDbContext>(options => 
options.UseNpgsql(connectionString));


using IHost app = builder.Build();


using IServiceScope scope = app.Services.CreateScope();

var dbContext =
    scope.ServiceProvider.GetRequiredService<HBTrackerDbContext>();

try
{
    bool canConnect = await dbContext.Database.CanConnectAsync();

    Console.WriteLine(
        canConnect
            ? "Database connection successful."
            : "Database connection failed.");
}
catch (Exception exception)
{
    Console.WriteLine("Database connection failed.");
    Console.WriteLine(exception.Message);
}