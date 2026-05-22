namespace FrmdOrderManager.Models;

// Specialbeställning där kunden anger vilken plats kartan ska visa
// och hur komplex designen ska vara (1-5). Högre komplexitet kostar mer.
public class CustomMapProduct : Product
{
    public string RequestedLocation { get; set; } = "";
    public int DesignComplexity { get; set; } = 1;

    public CustomMapProduct()
    {
        Category = ProductCategory.CustomMap;
    }

    public CustomMapProduct(string requestedLocation, int designComplexity, decimal basePrice)
        : base($"Custom Map - {requestedLocation}", basePrice, ProductCategory.CustomMap)
    {
        RequestedLocation = requestedLocation;
        DesignComplexity = designComplexity;
    }

    public override decimal CalculatePrice()
    {
        // Varje extra komplexitetsnivå lägger på 150 kr.
        return BasePrice + (DesignComplexity * 150);
    }

    public override string GetDescription()
    {
        return $"Custom FRMD By You map: {RequestedLocation} (complexity {DesignComplexity}/5)";
    }
}
