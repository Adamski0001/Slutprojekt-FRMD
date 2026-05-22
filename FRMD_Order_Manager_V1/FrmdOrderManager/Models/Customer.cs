namespace FrmdOrderManager.Models;

// Vanlig modellklass för en kund. Inget arv, bara properties.
public class Customer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";

    public Customer() { }

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
