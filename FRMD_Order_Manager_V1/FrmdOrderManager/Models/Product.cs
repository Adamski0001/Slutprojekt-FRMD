namespace FrmdOrderManager.Models;

// Basklass för alla produkter. Abstrakt, så man måste skapa
// en MapFrame, Keyring eller CustomMapProduct för att få ett objekt.
public abstract class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public decimal BasePrice { get; set; }
    public ProductCategory Category { get; set; }

    protected Product() { }

    protected Product(string name, decimal basePrice, ProductCategory category)
    {
        Name = name;
        BasePrice = basePrice;
        Category = category;
    }

    // Två abstrakta metoder. Tvingar varje subklass att räkna ut
    // sitt eget pris och sin egen beskrivning (polymorfism).
    public abstract decimal CalculatePrice();
    public abstract string GetDescription();

    public override string ToString()
    {
        return $"{GetDescription()} - {CalculatePrice()} kr";
    }
}
