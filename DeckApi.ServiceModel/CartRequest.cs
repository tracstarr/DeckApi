using DeckApi.ServiceModel.Types;
using ServiceStack;

namespace DeckApi.ServiceModel;

[Route("/cart/{UserId}", "GET", Summary = "For completeness of the task. This would allow only admin users to get the cart for a specific user. ")]
[Route("/cart", "GET", Summary = "Get the cart for the current user - no need to pass the UserId")]
[ValidateIsAuthenticated]
[ValidateHasRole(Roles.Shopper)]
public class CartRequest: IGet, IReturn<CartResponse>
{
    public string UserId { get; set; }
}