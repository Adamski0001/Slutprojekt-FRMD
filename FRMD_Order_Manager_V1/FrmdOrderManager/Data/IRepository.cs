namespace FrmdOrderManager.Data;

// Generiskt kontrakt för datalagring. Samma interface fungerar för Customer, Order
// och andra typer utan att vi behöver en ny repository-klass per typ.
public interface IRepository<T> where T : class
{
    List<T> GetAll();
    void Add(T item);
    void Remove(T item);
    void Save();
    void Load();
}
