namespace FrmdOrderManager.Models;

// De statusvärden en order kan ha. Används i statusdropdownen i Orders-fliken.
public enum OrderStatus
{
    New,
    InProduction,
    Completed,
    Shipped,
    Cancelled
}
