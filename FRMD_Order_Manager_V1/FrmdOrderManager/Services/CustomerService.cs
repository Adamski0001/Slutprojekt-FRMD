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

    // Lägger till en ny kund. ValidateCustomer kastar ValidationException om namn eller
    // e-post är ogiltiga, då hoppas Add över och felet bubblar upp till forms-koden.
    public void AddCustomer(string name, string email, string phone)
    {
        _validationService.ValidateCustomer(name, email);

        Customer customer = new Customer(name.Trim(), email.Trim(), phone.Trim());
        _repository.Add(customer);
    }

    // Uppdaterar en befintlig kund så att en feltaggad e-post inte är permanent.
    // Sparar bara om de nya värdena är giltiga, annars kastar ValidateCustomer ett
    // undantag och inget skrivs över.
    public void UpdateCustomer(Customer customer, string name, string email, string phone)
    {
        _validationService.ValidateCustomer(name, email);

        customer.Name = name.Trim();
        customer.Email = email.Trim();
        customer.Phone = phone.Trim();
        _repository.Save();
    }
}
