using System.Collections.Generic;
using DeckApi.ServiceModel.Types.Models;

namespace DeckApi.ServiceModel;

public class CartResponse
{   
    public int CartId { get; set; }
    public string UserId { get; set; }
    public decimal TotalPrice { get; set; }
    public List<CartItem> Cart { get; set; }
}