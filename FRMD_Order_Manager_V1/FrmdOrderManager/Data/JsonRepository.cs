using System.Text.Json;

namespace FrmdOrderManager.Data;

// Generisk JSON-lagring. Sparar och läser en lista med objekt av typ T
// till en fil. Används både för kunder och ordrar i programmet.
public class JsonRepository<T> : IRepository<T> where T : class
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _options;
    private List<T> _items = new List<T>();

    public JsonRepository(string filePath)
    {
        _filePath = filePath;
        // WriteIndented gör att JSON-filen blir lättare att läsa för en människa.
        _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };
    }

    public List<T> GetAll()
    {
        return _items;
    }

    public void Add(T item)
    {
        _items.Add(item);
        Save();
    }

    public void Remove(T item)
    {
        _items.Remove(item);
        Save();
    }

    public void Save()
    {
        // Skapar mappen om den inte finns, annars kraschar File.WriteAllText.
        string directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string json = JsonSerializer.Serialize(_items, _options);
        File.WriteAllText(_filePath, json);
    }

    public void Load()
    {
        // Om filen inte finns startar vi med en tom lista och skapar filen direkt.
        if (!File.Exists(_filePath))
        {
            _items = new List<T>();
            Save();
            return;
        }

        try
        {
            string json = File.ReadAllText(_filePath);
            _items = JsonSerializer.Deserialize<List<T>>(json, _options);
            if (_items == null)
            {
                _items = new List<T>();
            }
        }
        catch
        {
            // Om JSON-filen är trasig börjar vi om med en tom lista
            // i stället för att låta programmet krascha.
            _items = new List<T>();
        }
    }
}
