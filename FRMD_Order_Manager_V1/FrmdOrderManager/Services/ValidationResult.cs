namespace FrmdOrderManager.Services;

// Samlar ihop felmeddelanden från en validering. Forms-koden kollar IsValid
// och visar ToMessage() i en MessageBox.
public class ValidationResult
{
    public List<string> Errors { get; } = new List<string>();

    // True så länge inga fel har lagts till.
    public bool IsValid
    {
        get { return Errors.Count == 0; }
    }

    // Lägger till ett felmeddelande i listan.
    public void AddError(string message)
    {
        Errors.Add(message);
    }

    // Slår ihop alla felmeddelanden till en sträng med ett fel per rad.
    public string ToMessage()
    {
        return string.Join(Environment.NewLine, Errors);
    }
}
