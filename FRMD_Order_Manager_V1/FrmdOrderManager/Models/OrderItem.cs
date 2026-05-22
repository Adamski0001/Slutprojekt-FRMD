namespace FrmdOrderManager.Models;

// En rad i en order. Sparar en kopia av produktens info, så att gamla ordrar
// fortsätter att visa rätt pris/namn även om produkten ändras senare.
public class OrderItem
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public string Description { get; set; } = "";
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public decimal LineTotal
    {
        get { return Quantity * UnitPrice; }
    }

    public OrderItem() { }

    public OrderItem(Product product, int quantity)
    {
        // Hämtar info från produkten via polymorfism (GetDescription/CalculatePrice).
        ProductId = product.Id;
        ProductName = product.Name;
        Description = product.GetDescription();
        Quantity = quantity;
        UnitPrice = product.CalculatePrice();
    }

    public override string ToString()
    {
        return $"{Quantity} x {Description} = {LineTotal} kr";
    }
}
