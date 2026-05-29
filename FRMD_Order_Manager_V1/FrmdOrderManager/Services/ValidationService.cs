using FrmdOrderManager.Models;

namespace FrmdOrderManager.Services;

// Samlar valideringsreglerna på ett ställe. Vid ogiltig indata kastas ett
// ValidationException som services och forms-koden fångar med try-catch.
public class ValidationService
{
    // Validerar att en kund har namn och en e-post som åtminstone innehåller @.
    public void ValidateCustomer(string name, string email)
    {
        List<string> errors = new List<string>();

        if (string.IsNullOrWhiteSpace(name))
        {
            errors.Add("Customer name is required.");
        }

        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
        {
            errors.Add("A valid email address is required.");
        }

        if (errors.Count > 0)
        {
            throw new ValidationException(errors);
        }
    }

    // Validerar en produkt innan den läggs till i katalogen. Reglerna är gemensamma
    // (namn + pris) plus subklass-specifika kontroller via pattern matching.
    public void ValidateProduct(Product product)
    {
        // Saknas produkten helt finns det inget mer att validera - kasta direkt.
        if (product == null)
        {
            throw new ValidationException("Product is missing.");
        }

        List<string> errors = new List<string>();

        if (string.IsNullOrWhiteSpace(product.Name))
        {
            errors.Add("Product name is required.");
        }

        if (product.BasePrice <= 0)
        {
            errors.Add("Base price must be greater than 0.");
        }

        if (product is MapFrame mapFrame)
        {
            if (string.IsNullOrWhiteSpace(mapFrame.Location))
            {
                errors.Add("Map frame location is required.");
            }
            if (mapFrame.Size != "Small" && mapFrame.Size != "Medium" && mapFrame.Size != "Large")
            {
                errors.Add("Size must be Small, Medium or Large.");
            }
        }
        else if (product is Keyring keyring)
        {
            if (string.IsNullOrWhiteSpace(keyring.Location))
            {
                errors.Add("Keyring location is required.");
            }
        }
        else if (product is CustomMapProduct customMap)
        {
            if (string.IsNullOrWhiteSpace(customMap.RequestedLocation))
            {
                errors.Add("Requested location is required.");
            }
            if (customMap.DesignComplexity < 1 || customMap.DesignComplexity > 5)
            {
                errors.Add("Design complexity must be between 1 and 5.");
            }
        }

        if (errors.Count > 0)
        {
            throw new ValidationException(errors);
        }
    }

    // Validerar att en order har kund, produkt och ett positivt antal.
    public void ValidateOrder(Customer customer, Product product, int quantity)
    {
        List<string> errors = new List<string>();

        if (customer == null)
        {
            errors.Add("Choose a customer before creating an order.");
        }

        if (product == null)
        {
            errors.Add("Choose a product before creating an order.");
        }

        if (quantity < 1)
        {
            errors.Add("Quantity must be at least 1.");
        }

        if (errors.Count > 0)
        {
            throw new ValidationException(errors);
        }
    }
}
