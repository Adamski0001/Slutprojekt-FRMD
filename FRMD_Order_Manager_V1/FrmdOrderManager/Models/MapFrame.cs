namespace FrmdOrderManager.Models;

// Topografisk karttavla. Storleken påverkar priset.
public class MapFrame : Product
{
    public string Location { get; set; } = "";
    public string Size { get; set; } = "Medium";

    // Parameterlös konstruktor för JSON-deserialisering.
    public MapFrame()
    {
        Category = ProductCategory.MapFrame;
    }

    // Skapar en karttavla med plats, storlek och utgångspris.
    public MapFrame(string location, string size, decimal basePrice)
        : base($"{location} Map Frame", basePrice, ProductCategory.MapFrame)
    {
        Location = location;
        Size = size;
    }

    // Lägger på ett tillägg på utgångspriset beroende på storleken.
    public override decimal CalculatePrice()
    {
        switch (Size)
        {
            case "Small":
                return BasePrice;
            case "Medium":
                return BasePrice + 100;
            case "Large":
                return BasePrice + 250;
            default:
                return BasePrice;
        }
    }

    // Beskrivning på formen "Åre topographic frame (Medium)".
    public override string GetDescription()
    {
        return $"{Location} topographic frame ({Size})";
    }
}
