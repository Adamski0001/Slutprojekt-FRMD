using FrmdOrderManager.Data;
using FrmdOrderManager.Models;

namespace FrmdOrderManager.Services;

// Hanterar all produktlogik. Speglar CustomerService – ett repository sköter lagringen
// och ValidationService kontrollerar inmatningen innan något läggs till.
public class ProductService
{
    private readonly IRepository<Product> _repository;
    private readonly ValidationService _validationService;

    // Läser in tidigare sparade produkter. Om katalogen är tom (första körningen
    // eller om användaren rensat allt) fylls den med ett par standardprodukter.
    public ProductService(IRepository<Product> repository, ValidationService validationService)
    {
        _repository = repository;
        _validationService = validationService;
        _repository.Load();

        if (_repository.GetAll().Count == 0)
        {
            SeedDefaultProducts();
        }
    }

    // Returnerar hela produktkatalogen.
    public List<Product> GetAllProducts()
    {
        return _repository.GetAll();
    }

    // Försöker lägga till en produkt. Sparar bara om valideringen går igenom.
    public ValidationResult AddProduct(Product product)
    {
        ValidationResult result = _validationService.ValidateProduct(product);
        if (!result.IsValid)
        {
            return result;
        }

        _repository.Add(product);
        return result;
    }

    // Tar bort en produkt från katalogen.
    public void RemoveProduct(Product product)
    {
        _repository.Remove(product);
    }

    // Skapar de ursprungliga FRMD-produkterna första gången programmet startar.
    private void SeedDefaultProducts()
    {
        _repository.Add(new MapFrame("Åre", "Medium", 799));
        _repository.Add(new MapFrame("Trysil", "Medium", 799));
        _repository.Add(new MapFrame("Mont Blanc", "Large", 999));
        _repository.Add(new MapFrame("Kebnekaise", "Medium", 849));
        _repository.Add(new MapFrame("Sweden", "Large", 999));
        _repository.Add(new MapFrame("Italy", "Large", 999));
        _repository.Add(new Keyring("Åre", 149));
        _repository.Add(new CustomMapProduct("Customer request", 3, 899));
    }
}
