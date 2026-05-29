using System.Text.Json.Serialization;

namespace FrmdOrderManager.Models;

// Basklass för alla produkter. Product klassen är absktrakt vilket betyder att man måste skapa en MapFrame, Keyring
// eller CustomMapProduct för att få ett objekt, vilket gör att vi sparar tid när man skapar produkter.

// Product är abstract - JSON vet inte själv vilken subklass varje rad ska bli.
// Attributen skriver "$type" till varje produkt så inläsningen kan välja rätt.
[JsonDerivedType(typeof(MapFrame), "MapFrame")]
[JsonDerivedType(typeof(Keyring), "Keyring")]
[JsonDerivedType(typeof(CustomMapProduct), "CustomMap")]
public abstract class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public decimal BasePrice { get; set; }
    public ProductCategory Category { get; set; }

    // Parameterlös konstruktor för JSON-deserialisering.
    protected Product() { }

    // Sätter de gemensamma fälten - anropas av subklassernas konstruktorer.
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
