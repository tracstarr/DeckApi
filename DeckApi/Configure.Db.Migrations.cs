using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DeckApi.ServiceInterface.Data;
using DeckApi.ServiceModel;
using DeckApi.ServiceModel.Types;
using DeckApi.ServiceModel.Types.Entity;

[assembly: HostingStartup(typeof(DeckApi.ConfigureDbMigrations))]

namespace DeckApi;

public class ConfigureDbMigrations : IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureAppHost(appHost => {

            AppTasks.Register("migrate", _ =>
            {
                var log = appHost.GetApplicationServices().GetRequiredService<ILogger<ConfigureDbMigrations>>();

                log.LogInformation("Running EF Migrations...");
                var scopeFactory = appHost.GetApplicationServices().GetRequiredService<IServiceScopeFactory>();
                
                using var scope = scopeFactory.CreateScope();
                using var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.Migrate();

                // Only seed users if DB was just created
                if (!db.Users.Any())
                {
                    log.LogInformation("Adding Seed Users...");
                    // not best practice to block async code, but this is a one-time operation
                    AddSeedUsers(scope.ServiceProvider).Wait();
                }

                if (db.Products.Any()) return;
                 
                log.LogInformation("Adding Seed Products...");
                db.Products.AddRange(new List<ProductEntity> {
                    new() { Name = "Product 1", Price = 10.99m, CreatedDate = DateTime.UtcNow},
                    new() { Name = "Product 2", Price = 20.99m, CreatedDate = DateTime.UtcNow },
                    new() { Name = "Product 3", Price = 30.99m, CreatedDate = DateTime.UtcNow },
                });
                db.SaveChanges();

                log.LogInformation("Adding Seed Carts...");
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                // again shouldn't be using a .Result here
                var existingUser = userManager.FindByEmailAsync("shopper@email.com").Result;
                    
                db.Carts.AddRange(new List<CartEntity> {
                    new() { UserId = existingUser!.Id, CreatedDate = DateTime.UtcNow, IsActive = true}
                });
                db.SaveChanges();
                        
                foreach (var product in db.Products)
                {
                    db.Items.Add(new CartItemEntity() { ProductId = product.Id, Quantity = 1, CreatedDate = DateTime.UtcNow, CartId = 1, Name = product.Name, Price = product.Price});
                }

                db.SaveChanges();
            });

            AppTasks.Run();
        });

    private async Task AddSeedUsers(IServiceProvider services)
    {
        //initializing custom roles 
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        string[] allRoles = [Roles.Admin, Roles.Manager, Roles.Employee, Roles.Shopper];

        void assertResult(IdentityResult result)
        {
            if (!result.Succeeded)
                throw new Exception(result.Errors.First().Description);
        }

        async Task EnsureUserAsync(ApplicationUser user, string password, string[]? roles = null)
        {
            var existingUser = await userManager.FindByEmailAsync(user.Email!);
            if (existingUser != null) return;

            await userManager!.CreateAsync(user, password);
            if (roles?.Length > 0)
            {
                var newUser = await userManager.FindByEmailAsync(user.Email!);
                assertResult(await userManager.AddToRolesAsync(user, roles));
            }
        }

        foreach (var roleName in allRoles)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                //Create the roles and seed them to the database
                assertResult(await roleManager.CreateAsync(new IdentityRole(roleName)));
            }
        }

        await EnsureUserAsync(new ApplicationUser
        {
            DisplayName = "Test Employee",
            Email = "employee@email.com",
            UserName = "employee@email.com",
            FirstName = "Test",
            LastName = "Employee",
            EmailConfirmed = true,
        }, "p@55wOrd", [Roles.Employee]);

        await EnsureUserAsync(new ApplicationUser
        {
            DisplayName = "Test Manager",
            Email = "manager@email.com",
            UserName = "manager@email.com",
            FirstName = "Test",
            LastName = "Manager",
            EmailConfirmed = true,
        }, "p@55wOrd", [Roles.Manager, Roles.Employee]);
         
        await EnsureUserAsync(new ApplicationUser
        {
            DisplayName = "Test Shopper",
            Email = "shopper@email.com",
            UserName = "shopper@email.com",
            FirstName = "Test",
            LastName = "Shopper",
            EmailConfirmed = true,
        }, "p@55wOrd", [Roles.Shopper]);
      
    }
}
