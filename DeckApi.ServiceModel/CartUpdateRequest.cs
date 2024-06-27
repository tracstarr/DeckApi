using System.Collections.Generic;
using DeckApi.ServiceModel.Types;
using DeckApi.ServiceModel.Types.Models;
using ServiceStack;
using ServiceStack.DataAnnotations;

namespace DeckApi.ServiceModel;

[Route("/cart/{UserId}", "PUT")]
[ValidateIsAuthenticated]
[ValidateHasRole(Roles.Shopper)]
public class CartUpdateRequest: IPut, IReturnVoid
{
    public string UserId { get; set; }
    [Required]
    public List<CartProductItem> Items { get; set; }  
}