namespace FrmdOrderManager.Models;

// Basklass för alla produkter. Abstrakt – man måste skapa en MapFrame, Keyring
// eller CustomMapProduct för att få ett objekt.
public abstract class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public decimal BasePrice { get; set; }
    public ProductCategory Category { get; set; }

    // Parameterlös konstruktor för JSON-deserialisering.
    protected Product() { }

    // Sätter de gemensamma fälten – anropas av subklassernas konstruktorer.
    protected Product(string name, decimal basePrice, ProductCategory category)
    {
        Name = name;
        BasePrice = basePrice;
        Category = category;
    }

    // Räknar fram det faktiska priset. Varje subklass har sin egen formel (polymorfism).
    public abstract decimal CalculatePrice();

    // Returnerar en läsbar beskrivning av produkten.
    public abstract string GetDescription();

    // Beskrivning följt av aktuellt pris.
    public override string ToString()
    {
        return $"{GetDescription()} - {CalculatePrice()} kr";
    }
}
