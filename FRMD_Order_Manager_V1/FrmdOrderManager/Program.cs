using FrmdOrderManager.Forms;

namespace FrmdOrderManager;

internal static class Program
{
    // Startpunkten för programmet. Initierar Windows Forms och öppnar huvudfönstret.
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
