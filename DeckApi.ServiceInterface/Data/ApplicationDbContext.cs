using DeckApi.ServiceModel.Types.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DeckApi.ServiceInterface.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
    : IdentityDbContext<ApplicationUser>(options)
{
    
    public DbSet<CartItemEntity> Items { get; set; }
    public DbSet<CartEntity> Carts { get; set; }
    public DbSet<ProductEntity> Products { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
 
        modelBuilder.Entity<CartEntity>()
            .HasMany(c => c.Items)
            .WithOne()
            .HasForeignKey(i => i.CartId)
            .IsRequired();

        modelBuilder.Entity<CartItemEntity>()
            .HasOne<ProductEntity>()
            .WithMany()
            .HasForeignKey(p => p.ProductId);
            
        //This will ensure that only one active and not deleted cart can exist at a time for a user.
        modelBuilder.Entity<CartEntity>()
            .HasIndex(c => new { c.UserId, c.IsActive })
            .IsUnique()
            .HasFilter("[IsActive] = 1 AND [IsDeleted] = 0"); 
    }
}