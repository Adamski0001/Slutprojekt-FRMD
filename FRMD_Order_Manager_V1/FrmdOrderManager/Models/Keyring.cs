namespace FrmdOrderManager.Models;

// Nyckelring kopplad till en plats. Kan levereras i presentförpackning för ett tillägg.
public class Keyring : Product
{
    public string Location { get; set; } = "";
    public bool GiftPackaging { get; set; }

    // Parameterlös konstruktor för JSON-deserialisering.
    public Keyring()
    {
        Category = ProductCategory.Keyring;
    }

    // Skapar en nyckelring med plats, utgångspris och eventuell presentförpackning.
    public Keyring(string location, decimal basePrice, bool giftPackaging = false)
        : base($"{location} Keyring", basePrice, ProductCategory.Keyring)
    {
        Location = location;
        GiftPackaging = giftPackaging;
    }

    // Lägger på 25 kr om kunden valt presentförpackning.
    public override decimal CalculatePrice()
    {
        if (GiftPackaging)
        {
            return BasePrice + 25;
        }
        return BasePrice;
    }

    // Beskrivning nämner om presentförpackning ingår.
    public override string GetDescription()
    {
        if (GiftPackaging)
        {
            return $"{Location} keyring with gift packaging";
        }
        return $"{Location} keyring";
    }
}
