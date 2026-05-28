using System.Drawing;

namespace FrmdOrderManager.Forms;

// Färgpalett för MainForm. Samlar alla färger på ett ställe så att hela utseendet
// kan justeras genom att ändra här.
internal static class UiTheme
{
    public static readonly Color Accent = Color.FromArgb(31, 78, 92);
    public static readonly Color AccentSoft = Color.FromArgb(42, 104, 120);
    public static readonly Color CardBackground = Color.FromArgb(244, 241, 236);
    public static readonly Color CardBorder = Color.FromArgb(201, 193, 181);
    public static readonly Color Success = Color.FromArgb(46, 125, 50);
    public static readonly Color InProgress = Color.FromArgb(21, 101, 192);
    public static readonly Color Warning = Color.FromArgb(183, 28, 28);
    public static readonly Color TextDark = Color.FromArgb(26, 26, 26);
}
