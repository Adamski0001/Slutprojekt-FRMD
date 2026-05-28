using FrmdOrderManager.Data;
using FrmdOrderManager.Models;

namespace FrmdOrderManager.Services;

// Hanterar all orderlogik – skapa, byta status och räkna statistik till dashboarden.
public class OrderService
{
    private readonly IRepository<Order> _repository;
    private readonly ValidationService _validationService;

    // Läser in tidigare sparade ordrar från lagringen direkt vid start.
    public OrderService(IRepository<Order> repository, ValidationService validationService)
    {
        _repository = repository;
        _validationService = validationService;
        _repository.Load();
    }

    // Hämtar alla ordrar.
    public List<Order> GetAllOrders()
    {
        return _repository.GetAll();
    }

    // Skapar en ny order. Sparar bara om valideringen går igenom.
    public ValidationResult CreateOrder(Customer customer, Product product, int quantity, string notes)
    {
        ValidationResult result = _validationService.ValidateOrder(customer, product, quantity);
        if (!result.IsValid)
        {
            return result;
        }

        Order order = new Order();
        order.CustomerId = customer.Id;
        order.CustomerName = customer.Name;
        order.Notes = notes.Trim();
        order.Items.Add(new OrderItem(product, quantity));

        _repository.Add(order);
        return result;
    }

    // Sätter en ny status på ordern och sparar ändringen direkt.
    public void UpdateStatus(Order order, OrderStatus status)
    {
        order.Status = status;
        _repository.Save();
    }

    // Räknar ihop värdet på alla ordrar som inte är avbrutna.
    public decimal CalculateTotalSales()
    {
        decimal total = 0;
        foreach (Order order in _repository.GetAll())
        {
            if (order.Status != OrderStatus.Cancelled)
            {
                total += order.TotalPrice;
            }
        }
        return total;
    }

    // Räknar antalet ordrar som fortfarande är aktiva (nya eller i produktion).
    public int CountActiveOrders()
    {
        int count = 0;
        foreach (Order order in _repository.GetAll())
        {
            if (order.IsActive)
            {
                count++;
            }
        }
        return count;
    }
}
