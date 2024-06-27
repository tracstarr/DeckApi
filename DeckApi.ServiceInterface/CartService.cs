using System;
using System.Linq;
using System.Threading.Tasks;
using DeckApi.ServiceInterface.Data;
using DeckApi.ServiceInterface.Extensions;
using ServiceStack;
using DeckApi.ServiceModel;
using DeckApi.ServiceModel.Types;
using DeckApi.ServiceModel.Types.Entity;
using DeckApi.ServiceModel.Types.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DeckApi.ServiceInterface;

public class CartService(ApplicationDbContext dbContext, ILogger<CartService> logger) : Service
{
    public async Task<CartResponse> Get(CartRequest request)
    {
        logger.LogDebug("Getting cart for user {UserId}", request.UserId);
        var session = await GetSessionAsync();
        if (request.UserId.IsNullOrEmpty())
        {
            logger.LogDebug("Getting user id from session");
            request.UserId = session.UserAuthId;
        }
        
        if (request.UserId != session.UserAuthId )
        {
            if (!session.Roles.Contains(Roles.Admin))
            {
                logger.LogError("User {UserId} is not authorized to view cart for user {request.UserId}", session.UserAuthId, request.UserId);
                throw HttpError.Unauthorized("User is not authorized to view cart");
            }
            logger.LogInformation("User is admin");
        }

        var cart = await dbContext.GetActiveCart(request.UserId);
            
        if (cart == null)
        {
            logger.LogError("Cart not found for user {UserId}", request.UserId);
            throw HttpError.NotFound("Cart not found");
        }

        return new CartResponse()
        {

            CartId = cart.Id,
            UserId = cart.UserId,
            Cart = cart.Items.Select(i => new CartItem()
            {
                Id = i.Id,
                ProductId = i.ProductId,
                Name = i.Name,
                Price = i.Price,
                Quantity = i.Quantity,
                IsRemovedFromCart = i.IsRemovedFromCart
            }).ToList(),
            TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity)
        };
    }

    public async Task Put(CartUpdateRequest request)
    {
        logger.LogDebug("Updating cart for user {UserId}", request.UserId);
        var session = await GetSessionAsync();
        
        if (request.UserId.IsNullOrEmpty())
        {
            logger.LogDebug("Getting user id from session");
            request.UserId = session.UserAuthId;
        }
        
        if (request.UserId != session.UserAuthId)
        {
            logger.LogError("User {UserId} is not authorized to update cart for user {request.UserId}", session.UserAuthId, request.UserId);
            throw HttpError.Unauthorized("User is not authorized to update cart");
        }
        
        var cart = await dbContext.Carts
            .Include(c=> c.Items)
            .SingleOrDefaultAsync(c=> c.UserId == request.UserId && c.IsDeleted == false && c.IsActive == true);
        
        if (cart == null)
        {
            logger.LogError("Cart not found for user {UserId}", request.UserId);
            throw HttpError.NotFound("Cart not found");
        } 
        
        // remove all existing items by setting the IsRemovedFromCart flag to true and setting the modified date
        logger.LogDebug("Removing existing items from cart");
        foreach (var existingItem in cart.Items.Where(c=>c.IsRemovedFromCart == false))
        {
            existingItem.IsRemovedFromCart = true;
            existingItem.ModifiedDate = DateTime.UtcNow;
        }

        await dbContext.SaveChangesAsync();
         
        
        // add new items to the cart
        logger.LogDebug("Adding new items to cart");
        foreach (var newItem in request.Items)
        {
            var product = await dbContext.Products.FindAsync(newItem.ProductId);
            if (product == null)
            {
                throw HttpError.NotFound("Product not found");
            }
            
            cart.Items.Add(new CartItemEntity()
            {
                ProductId = newItem.ProductId,
                Name = product.Name,
                Price = product.Price,
                Quantity = newItem.Quantity,
                CreatedDate = DateTime.UtcNow,
            });
        }

        await dbContext.SaveChangesAsync();
    }
}  