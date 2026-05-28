namespace FrmdOrderManager.Models;

// Specialbeställd karta. Komplexitet 1–5 där varje steg lägger på 150 kr.
public class CustomMapProduct : Product
{
    public string RequestedLocation { get; set; } = "";
    public int DesignComplexity { get; set; } = 1;

    // Parameterlös konstruktor för JSON-deserialisering.
    public CustomMapProduct()
    {
        Category = ProductCategory.CustomMap;
    }

    // Skapar en specialbeställd karta med plats, komplexitet och utgångspris.
    public CustomMapProduct(string requestedLocation, int designComplexity, decimal basePrice)
        : base($"Custom Map - {requestedLocation}", basePrice, ProductCategory.CustomMap)
    {
        RequestedLocation = requestedLocation;
        DesignComplexity = designComplexity;
    }

    // Pris = utgångspris + 150 kr per komplexitetsnivå.
    public override decimal CalculatePrice()
    {
        return BasePrice + (DesignComplexity * 150);
    }

    // Beskrivning på formen "Custom FRMD By You map: ... (complexity 3/5)".
    public override string GetDescription()
    {
        return $"Custom FRMD By You map: {RequestedLocation} (complexity {DesignComplexity}/5)";
    }
}
