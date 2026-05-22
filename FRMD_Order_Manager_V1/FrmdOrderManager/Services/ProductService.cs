using FrmdOrderManager.Models;

namespace FrmdOrderManager.Services;

// Håller en lista över produkterna som FRMD säljer. Just nu fylls listan
// med exempelprodukter i konstruktorn (seedning), så att programmet alltid
// har något att visa när det startar.
public class ProductService
{
    private readonly List<Product> _products = new List<Product>();

    public ProductService()
    {
        SeedDefaultProducts();
    }

    public List<Product> GetAllProducts()
    {
        return _products;
    }

    // Skapar produkter genom alla tre subklasser av Product (visar arvet).
    private void SeedDefaultProducts()
    {
        _products.Add(new MapFrame("Åre", "Medium", 799));
        _products.Add(new MapFrame("Trysil", "Medium", 799));
        _products.Add(new MapFrame("Mont Blanc", "Large", 999));
        _products.Add(new MapFrame("Kebnekaise", "Medium", 849));
        _products.Add(new MapFrame("Sweden", "Large", 999));
        _products.Add(new MapFrame("Italy", "Large", 999));
        _products.Add(new Keyring("Åre", 149));
        _products.Add(new CustomMapProduct("Customer request", 3, 899));
    }
}
