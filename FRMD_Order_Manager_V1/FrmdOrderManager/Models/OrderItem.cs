namespace FrmdOrderManager.Models;

// En rad i en order. Sparar en kopia av produktens info så att gamla ordrar visar
// rätt pris/namn även om produkten ändras senare.
public class OrderItem
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public string Description { get; set; } = "";
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    // Totalpriset för raden - antal multiplicerat med styckpriset.
    public decimal LineTotal
    {
        get { return Quantity * UnitPrice; }
    }

    public OrderItem() { }

    // Skapar en orderrad utifrån en produkt och ett antal.
    public OrderItem(Product product, int quantity)
    {
        // Beskrivning och pris hämtas via polymorfism - det är subklassens version som körs.
        ProductId = product.Id;
        ProductName = product.Name;
        Description = product.GetDescription();
        Quantity = quantity;
        UnitPrice = product.CalculatePrice();
    }

    // Text som visas i orderdetaljernas ListBox.
    public override string ToString()
    {
        return $"{Quantity} x {Description} = {LineTotal} kr";
    }
}
