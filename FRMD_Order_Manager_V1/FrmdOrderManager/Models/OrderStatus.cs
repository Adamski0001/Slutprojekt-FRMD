namespace FrmdOrderManager.Models;

// Status som en order kan ha. Används i statusdropdown i Orders-fliken.
public enum OrderStatus
{
    New,
    InProduction,
    Completed,
    Shipped,
    Cancelled
}
