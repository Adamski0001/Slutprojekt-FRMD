namespace FrmdOrderManager.Services;

// Hjälpklass som samlar ihop felmeddelanden från en validering.
// Forms-koden kan sedan kontrollera IsValid och visa ToMessage() i en MessageBox.
public class ValidationResult
{
    public List<string> Errors { get; } = new List<string>();

    public bool IsValid
    {
        get { return Errors.Count == 0; }
    }

    public void AddError(string message)
    {
        Errors.Add(message);
    }

    public string ToMessage()
    {
        return string.Join(Environment.NewLine, Errors);
    }
}
