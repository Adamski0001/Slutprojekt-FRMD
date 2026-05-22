using FrmdOrderManager.Forms;

namespace FrmdOrderManager;

internal static class Program
{
    // Startpunkten för programmet. Visar formuläret MainForm.
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
