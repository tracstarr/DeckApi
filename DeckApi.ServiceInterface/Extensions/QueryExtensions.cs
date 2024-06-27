using System.Linq;
using System.Threading.Tasks;
using DeckApi.ServiceInterface.Data;
using DeckApi.ServiceModel.Types.Entity;
using Microsoft.EntityFrameworkCore;

namespace DeckApi.ServiceInterface.Extensions;

public static class QueryExtensions
{
    public static async Task<CartEntity> GetActiveCart(this ApplicationDbContext db, string userId)
    {
        
        // note: this query is not supported by SQLite in memory so the unit testing fail. Due to time
        // constraints I won't look at alternative test solutions but modify how we query instead.
        
        /*
        return await db.Carts
            .Where(c=> c.Items.Any(i=> !i.IsRemovedFromCart))
            .Include(c => c.Items.Where(i=> !i.IsRemovedFromCart))
            .SingleOrDefaultAsync(c => c.UserId == userId && c.IsDeleted == false && c.IsActive == true);
        */
        
        
        // This query is supported by SQLite in memory but isn't as efficient as the one above. I didn't use a 
        // projection because it would still require filtering afterwards and items not being tracked by ef core.
        
        var cart = await db.Carts
            .SingleOrDefaultAsync(c => c.UserId == userId && c.IsDeleted == false && c.IsActive == true);

        if (cart != null)
        {
            cart.Items = await db.Entry(cart)
                .Collection(c => c.Items)
                .Query()
                .Where(i => !i.IsRemovedFromCart)
                .ToListAsync();
        }
        
        return cart;
    }
    
}