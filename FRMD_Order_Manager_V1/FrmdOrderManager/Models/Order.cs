namespace FrmdOrderManager.Models;

// En order knyts till en kund och innehåller en lista med orderrader.
public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public OrderStatus Status { get; set; } = OrderStatus.New;
    public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    public string Notes { get; set; } = "";

    // Räknar ihop totalpriset från alla rader i ordern.
    public decimal TotalPrice
    {
        get { return Items.Sum(item => item.LineTotal); }
    }

    // Aktiva ordrar är de som inte är färdiga, skickade eller avbrutna.
    public bool IsActive
    {
        get
        {
            return Status == OrderStatus.New || Status == OrderStatus.InProduction;
        }
    }

    // Visar bara första delen av Guid så det blir lättare att läsa i tabellen.
    public string ShortId
    {
        get
        {
            return Id.ToString().Substring(0, 8).ToUpper();
        }
    }

    public override string ToString()
    {
        return $"{ShortId} - {CustomerName} - {Status} - {TotalPrice} kr";
    }
}
