using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using ServiceStack;
using ServiceStack.Testing;
using DeckApi.ServiceInterface;
using DeckApi.ServiceInterface.Data;
using DeckApi.ServiceInterface.Extensions;
using DeckApi.ServiceModel;
using DeckApi.ServiceModel.Types;
using DeckApi.ServiceModel.Types.Entity;
using DeckApi.ServiceModel.Types.Models;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ServiceStack.Host;

namespace DeckApi.Tests;

public class UnitTest
{
    private readonly ServiceStackHost appHost;
    
    public UnitTest()
    {
        appHost = new BasicAppHost().Init();
        appHost.Container.AddTransient<CartService>();
        
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;
        
        appHost.Container.AddSingleton(new ApplicationDbContext(options));
        
        // Add Null Logger to the container
        appHost.Container.AddSingleton<ILogger<CartService>, NullLogger<CartService>>();

    }
    
    [OneTimeSetUp]
    public void Setup()
    {
        var dbContext = appHost.Container.Resolve<ApplicationDbContext>();

        // Ensure the database is created
        dbContext.Database.EnsureCreated();
     
        // add some seed data
        dbContext.Products.AddRange(new List<ProductEntity> {
            new() { Name = "Product 1", Price = 10.99m, CreatedDate = DateTime.UtcNow},
            new() { Name = "Product 2", Price = 20.99m, CreatedDate = DateTime.UtcNow },
            new() { Name = "Product 3", Price = 30.99m, CreatedDate = DateTime.UtcNow },
        });

        dbContext.SaveChanges();

        dbContext.Carts.Add(new CartEntity()
        {
            UserId = "1",
            CreatedDate = DateTime.UtcNow,
            IsActive = true
        });
        
        foreach (var product in dbContext.Products)
        {
            dbContext.Items.Add(new CartItemEntity()
            {
                ProductId = product.Id, Quantity = 1, CreatedDate = DateTime.UtcNow, CartId = 1, Name = product.Name, Price = product.Price
            });
        }
        
        dbContext.Items.Add(new CartItemEntity()
        {
            ProductId = 1, Quantity = 1, CreatedDate = DateTime.UtcNow, CartId = 1, Name = "Product 1", Price = 10.99m, IsRemovedFromCart = true
        });
        
      
        dbContext.SaveChanges();
    }

    
    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        appHost.Dispose();
    }

    [Test]
    [Order(1)]
    public async Task Can_call_get_cart()
    {
        // mock authenticated user
        var req = new BasicRequest { 
            Verb = HttpMethods.Get, 
            Items = {
                [Keywords.Session] = new AuthUserSession { UserAuthId = "1", IsAuthenticated = true, Roles = new List<string>(){ Roles.Shopper}}
            }
        };

        await using var service = HostContext.ResolveService<CartService>(req);
        
        // act
        var response = await service.Get(new CartRequest(){ UserId = "1"});

        // assert
        response.CartId.Should().Be(1);
        response.Cart.Should().HaveCount(3, "because we added 3 products to the cart");
    }

    [Test]
    [Order(2)]
    public async Task Can_update_cart()
    {
        
        // mock the session since we use it in the service
        var req = new BasicRequest { 
            Verb = HttpMethods.Put, 
            Items = {
                [Keywords.Session] = new AuthUserSession { UserAuthId = "1", IsAuthenticated = true, Roles = new List<string>(){ Roles.Shopper}}
            }
        };
 
        await using var service = HostContext.ResolveService<CartService>(req);
        
        // act
        await service.Put(new CartUpdateRequest()
        {
            UserId = "1",
            Items = new List<CartProductItem>()
            {
                new() { ProductId = 1, Quantity = 1},  
            }
        });

        // assert
        // we pull directly from the db to confirm the expected data updates rather than relying on the service
        var db = appHost.Container.Resolve<ApplicationDbContext>();

        var cart = await db.GetActiveCart("1");
        
        cart.Items.Should().HaveCount(1, "because we added 1 product to the cart which removes all existing");

        var historyItem = db.Items.Count(i => i.CartId == 1 && i.IsRemovedFromCart == true);
        historyItem.Should().Be(3, "because we removed the initial 3 items from the cart");
        
    }
}