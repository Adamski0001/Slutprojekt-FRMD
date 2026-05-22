namespace FrmdOrderManager.Data;

// Generiskt interface för datalagring. Tanken är att samma kontrakt
// ska fungera för olika klasser (Customer, Order, ...) utan att man
// behöver skriva en ny repository-klass för varje typ.
public interface IRepository<T> where T : class
{
    List<T> GetAll();
    void Add(T item);
    void Remove(T item);
    void Save();
    void Load();
}
