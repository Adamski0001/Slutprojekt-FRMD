using FrmdOrderManager.Data;
using FrmdOrderManager.Models;

namespace FrmdOrderManager.Services;

// Hanterar all logik runt kunder, så att MainForm bara behöver anropa metoder
// i stället för att blanda ihop logik med UI-koden.
public class CustomerService
{
    private readonly IRepository<Customer> _repository;
    private readonly ValidationService _validationService;

    public CustomerService(IRepository<Customer> repository, ValidationService validationService)
    {
        _repository = repository;
        _validationService = validationService;
        // Läser in sparade kunder från JSON-filen direkt vid start.
        _repository.Load();
    }

    public List<Customer> GetAllCustomers()
    {
        return _repository.GetAll();
    }

    public ValidationResult AddCustomer(string name, string email, string phone)
    {
        // Validerar först. Avbryter och returnerar felmeddelandena om något är fel.
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
