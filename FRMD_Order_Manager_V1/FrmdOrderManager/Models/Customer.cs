namespace FrmdOrderManager.Models;

// Modellklass för en kund. Innehåller bara properties, inget arv.
public class Customer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";

    // Parameterlös konstruktor för JSON-deserialisering.
    public Customer() { }

    // Skapar en kund med ifyllda kontaktuppgifter.
    public Customer(string name, string email, string phone)
    {
        Name = name;
        Email = email;
        Phone = phone;
    }

    // Används av ListBox/ComboBox för att visa kunden i listan.
    public override string ToString()
    {
        return $"{Name} ({Email})";
    }
}
