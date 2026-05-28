using FrmdOrderManager.Data;
using FrmdOrderManager.Models;

namespace FrmdOrderManager.Services;

// Hanterar all kundlogik så att MainForm slipper blanda affärslogik med UI-kod.
public class CustomerService
{
    private readonly IRepository<Customer> _repository;
    private readonly ValidationService _validationService;

    // Läser in tidigare sparade kunder från lagringen direkt vid start.
    public CustomerService(IRepository<Customer> repository, ValidationService validationService)
    {
        _repository = repository;
        _validationService = validationService;
        _repository.Load();
    }

    // Hämtar alla kunder.
    public List<Customer> GetAllCustomers()
    {
        return _repository.GetAll();
    }

    // Försöker lägga till en ny kund. Sparar bara om valideringen går igenom.
    public ValidationResult AddCustomer(string name, string email, string phone)
    {
        ValidationResult result = _validationService.ValidateCustomer(name, email);
        if (!result.IsValid)
        {
            return result;
        }

        Customer customer = new Customer(name.Trim(), email.Trim(), phone.Trim());
        _repository.Add(customer);
        return result;
    }
}
