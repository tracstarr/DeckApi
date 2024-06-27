using DeckApi.ServiceInterface.Data;
using Microsoft.EntityFrameworkCore;

[assembly: HostingStartup(typeof(ConfigureDb))]

namespace DeckApi;

public class ConfigureDb : IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices((context, services) => {
            var connectionString = context.Configuration.GetConnectionString("DefaultConnection")
                                   ?? "DataSource=App_Data/app.db;Cache=Shared";
            
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(connectionString, b => b.MigrationsAssembly(nameof(DeckApi))));
            
        });
}