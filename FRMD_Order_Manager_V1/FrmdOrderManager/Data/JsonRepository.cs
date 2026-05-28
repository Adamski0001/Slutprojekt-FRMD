using System.Text.Json;

namespace FrmdOrderManager.Data;

// Generisk JSON-lagring. Sparar och läser en lista med objekt av typ T till en fil.
public class JsonRepository<T> : IRepository<T> where T : class
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _options;
    private List<T> _items = new List<T>();

    // Skapar lagringen för en specifik fil. Filen behöver inte finnas i förväg.
    public JsonRepository(string filePath)
    {
        _filePath = filePath;
        // WriteIndented gör JSON-filen lättläst om man öppnar den i en texteditor.
        _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };
    }

    // Returnerar listan med alla inlästa objekt.
    public List<T> GetAll()
    {
        return _items;
    }

    // Lägger till ett objekt och sparar direkt till disk.
    public void Add(T item)
    {
        _items.Add(item);
        Save();
    }

    // Tar bort ett objekt och sparar direkt till disk.
    public void Remove(T item)
    {
        _items.Remove(item);
        Save();
    }

    // Skriver hela listan till JSON-filen.
    public void Save()
    {
        // Skapar målmappen om den saknas, annars kraschar File.WriteAllText.
        string directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string json = JsonSerializer.Serialize(_items, _options);
        File.WriteAllText(_filePath, json);
    }

    // Läser in listan från JSON-filen. Skapar en tom fil om den saknas.
    public void Load()
    {
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
            // Trasig JSON ska inte stoppa programmet – vi börjar om från noll.
            _items = new List<T>();
        }
    }
}
