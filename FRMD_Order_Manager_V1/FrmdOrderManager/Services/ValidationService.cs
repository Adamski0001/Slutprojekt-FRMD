using FrmdOrderManager.Models;

namespace FrmdOrderManager.Services;

// Samlar valideringsreglerna på ett ställe så att forms-koden slipper kolla själv.
public class ValidationService
{
    // Validerar att en kund har namn och en e-post som åtminstone innehåller @.
    public ValidationResult ValidateCustomer(string name, string email)
    {
        ValidationResult result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(name))
        {
            result.AddError("Customer name is required.");
        }

        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
        {
            result.AddError("A valid email address is required.");
        }

        return result;
    }

    // Validerar att en order har kund, produkt och ett positivt antal.
    public ValidationResult ValidateOrder(Customer customer, Product product, int quantity)
    {
        ValidationResult result = new ValidationResult();

        if (customer == null)
        {
            result.AddError("Choose a customer before creating an order.");
        }

        if (product == null)
        {
            result.AddError("Choose a product before creating an order.");
        }

        if (quantity < 1)
        {
            result.AddError("Quantity must be at least 1.");
        }

        return result;
    }
}
