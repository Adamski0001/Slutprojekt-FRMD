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

    // Validerar en produkt innan den läggs till i katalogen. Reglerna är gemensamma
    // (namn + pris) plus subklass-specifika kontroller via pattern matching.
    public ValidationResult ValidateProduct(Product product)
    {
        ValidationResult result = new ValidationResult();

        if (product == null)
        {
            result.AddError("Product is missing.");
            return result;
        }

        if (string.IsNullOrWhiteSpace(product.Name))
        {
            result.AddError("Product name is required.");
        }

        if (product.BasePrice < 0)
        {
            result.AddError("Base price cannot be negative.");
        }

        if (product is MapFrame mapFrame)
        {
            if (string.IsNullOrWhiteSpace(mapFrame.Location))
            {
                result.AddError("Map frame location is required.");
            }
            if (mapFrame.Size != "Small" && mapFrame.Size != "Medium" && mapFrame.Size != "Large")
            {
                result.AddError("Size must be Small, Medium or Large.");
            }
        }
        else if (product is Keyring keyring)
        {
            if (string.IsNullOrWhiteSpace(keyring.Location))
            {
                result.AddError("Keyring location is required.");
            }
        }
        else if (product is CustomMapProduct customMap)
        {
            if (string.IsNullOrWhiteSpace(customMap.RequestedLocation))
            {
                result.AddError("Requested location is required.");
            }
            if (customMap.DesignComplexity < 1 || customMap.DesignComplexity > 5)
            {
                result.AddError("Design complexity must be between 1 and 5.");
            }
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
