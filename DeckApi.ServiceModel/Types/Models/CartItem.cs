namespace DeckApi.ServiceModel.Types.Models;

// for passing over the wire. Shouldn't be using EF models. This allows for flexibility in the future and 
// for additional properties to be added to the model without affecting the database
public class CartItem
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public bool IsRemovedFromCart { get; set; }
}