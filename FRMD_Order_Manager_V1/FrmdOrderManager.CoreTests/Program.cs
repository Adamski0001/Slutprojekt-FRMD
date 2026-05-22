using FrmdOrderManager.Data;
using FrmdOrderManager.Models;
using FrmdOrderManager.Services;

// Enkla tester för programmets logik (utan att starta något grafiskt fönster).
// Körs som ett vanligt konsolprogram. Kraschar med ett tydligt felmeddelande
// om något test går fel, och skriver "alla tester gick igenom" annars.
internal class Program
{
    private static void Main(string[] args)
    {
        // Tester skriver till en tillfällig mapp så att riktiga datafiler inte påverkas.
        string tempDir = Path.Combine(Path.GetTempPath(), "frmd-tests-" + Guid.NewGuid());
        Directory.CreateDirectory(tempDir);

        ValidationService validation = new ValidationService();
        CustomerService customers = new CustomerService(
            new JsonRepository<Customer>(Path.Combine(tempDir, "customers.json")),
            validation);
        OrderService orders = new OrderService(
            new JsonRepository<Order>(Path.Combine(tempDir, "orders.json")),
            validation);
        ProductService products = new ProductService();

        // Kontrollerar att seed-produkterna finns och att arvet används.
        Check(products.GetAllProducts().Count >= 8, "Det ska finnas minst 8 seed-produkter.");

        bool hasMapFrame = false;
        bool hasKeyring = false;
        bool hasCustomMap = false;
        foreach (Product p in products.GetAllProducts())
        {
            if (p is MapFrame) hasMapFrame = true;
            if (p is Keyring) hasKeyring = true;
            if (p is CustomMapProduct) hasCustomMap = true;
        }
        Check(hasMapFrame, "Det ska finnas minst en MapFrame.");
        Check(hasKeyring, "Det ska finnas minst en Keyring.");
        Check(hasCustomMap, "Det ska finnas minst en CustomMapProduct.");

        // Validering av kunder.
        ValidationResult invalidCustomer = customers.AddCustomer("", "inte-en-epost", "");
        Check(!invalidCustomer.IsValid, "Ogiltig kund ska få fel.");

        ValidationResult validCustomer = customers.AddCustomer("Adam Hessel", "adam@example.com", "0700000000");
        Check(validCustomer.IsValid, "Giltig kund ska gå igenom.");
        Check(customers.GetAllCustomers().Count == 1, "Kunden ska sparas i listan.");

        // Skapa en order, kontrollera summor och status.
        Customer customer = customers.GetAllCustomers()[0];
        Product product = products.GetAllProducts()[0];

        ValidationResult invalidOrder = orders.CreateOrder(null, product, 1, "");
        Check(!invalidOrder.IsValid, "Order utan kund ska få fel.");

        ValidationResult validOrder = orders.CreateOrder(customer, product, 2, "Test-order från skoltest");
        Check(validOrder.IsValid, "Giltig order ska gå igenom.");
        Check(orders.GetAllOrders().Count == 1, "Ordern ska sparas.");
        Check(orders.GetAllOrders()[0].Notes == "Test-order från skoltest", "Notes ska sparas.");
        Check(orders.CalculateTotalSales() == product.CalculatePrice() * 2, "Total försäljning ska matcha ordertotal.");

        Order order = orders.GetAllOrders()[0];
        orders.UpdateStatus(order, OrderStatus.InProduction);
        Check(orders.CountActiveOrders() == 1, "InProduction räknas som aktiv.");

        orders.UpdateStatus(order, OrderStatus.Cancelled);
        Check(orders.CalculateTotalSales() == 0, "Avbrutna ordrar ska inte räknas som försäljning.");

        Console.WriteLine("All FRMD Order Manager business-logic tests passed.");
    }

    // Liten hjälpmetod. Kastar ett undantag med ett tydligt meddelande
    // om villkoret inte stämmer, så att felet syns direkt i konsolen.
    private static void Check(bool condition, string message)
    {
        if (!condition)
        {
            throw new Exception("Test misslyckades: " + message);
        }
    }
}
