namespace FrmdOrderManager.Services;

// Kastas när en validering misslyckas. Bär med sig ett eller flera felmeddelanden
// som services och forms-koden fångar i ett try-catch och visar i en MessageBox.
public class ValidationException : Exception
{
    // Alla felmeddelanden som hör till den misslyckade valideringen.
    public List<string> Errors { get; }

    // Skapar ett undantag med ett enda felmeddelande.
    public ValidationException(string message) : base(message)
    {
        Errors = new List<string> { message };
    }

    // Skapar ett undantag med flera felmeddelanden. Message får ett fel per rad
    // så att MessageBox kan visa hela listan på samma sätt som tidigare.
    public ValidationException(List<string> errors)
        : base(string.Join(Environment.NewLine, errors))
    {
        Errors = errors;
    }
}
