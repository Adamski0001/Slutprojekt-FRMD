namespace FrmdOrderManager.Models;

// Nyckelring. Kan eventuellt levereras i presentförpackning, vilket kostar lite extra.
public class Keyring : Product
{
    public string Location { get; set; } = "";
    public bool GiftPackaging { get; set; }

    public Keyring()
    {
        Category = ProductCategory.Keyring;
    }

    public Keyring(string location, decimal basePrice, bool giftPackaging = false)
        : base($"{location} Keyring", basePrice, ProductCategory.Keyring)
    {
        Location = location;
        GiftPackaging = giftPackaging;
    }

    public override decimal CalculatePrice()
    {
        if (GiftPackaging)
        {
            return BasePrice + 25;
        }
        return BasePrice;
    }

    public override string GetDescription()
    {
        if (GiftPackaging)
        {
            return $"{Location} keyring with gift packaging";
        }
        return $"{Location} keyring";
    }
}
