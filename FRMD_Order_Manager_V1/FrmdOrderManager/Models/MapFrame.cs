namespace FrmdOrderManager.Models;

// Topografisk karttavla. Storleken påverkar priset.
public class MapFrame : Product
{
    public string Location { get; set; } = "";
    public string Size { get; set; } = "Medium";

    public MapFrame()
    {
        Category = ProductCategory.MapFrame;
    }

    public MapFrame(string location, string size, decimal basePrice)
        : base($"{location} Map Frame", basePrice, ProductCategory.MapFrame)
    {
        Location = location;
        Size = size;
    }

    public override decimal CalculatePrice()
    {
        // Större karttavlor kostar mer.
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

    public override string GetDescription()
    {
        return $"{Location} topographic frame ({Size})";
    }
}
