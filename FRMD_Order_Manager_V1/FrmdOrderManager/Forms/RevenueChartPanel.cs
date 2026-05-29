using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using FrmdOrderManager.Models;

namespace FrmdOrderManager.Forms;

// Egen Panel som ritar en linjediagram över kumulativa intäkter över tid.
// Data sätts via SetOrders - panelen ritar om sig själv automatiskt.
internal sealed class RevenueChartPanel : Panel
{
    private static readonly CultureInfo SwedishCulture = new CultureInfo("sv-SE");

    private List<(DateTime Date, decimal Cumulative)> _points = new List<(DateTime, decimal)>();

    public RevenueChartPanel()
    {
        DoubleBuffered = true;
        BackColor = Color.White;
        BorderStyle = BorderStyle.FixedSingle;
    }

    // Tar emot orderlistan och bygger upp datapunkterna (datum + kumulativt belopp).
    // Avbrutna ordrar exkluderas - de räknas inte som intäkt.
    public void SetOrders(IEnumerable<Order> orders)
    {
        List<Order> sorted = orders
            .Where(o => o.Status != OrderStatus.Cancelled)
            .OrderBy(o => o.CreatedAt)
            .ToList();

        _points = new List<(DateTime, decimal)>();
        decimal running = 0m;
        foreach (Order order in sorted)
        {
            running += order.TotalPrice;
            _points.Add((order.CreatedAt, running));
        }

        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        Size size = ClientSize;

        // Titel i toppen av panelen.
        using (Font titleFont = new Font("Segoe UI", 12F, FontStyle.Bold))
        using (SolidBrush titleBrush = new SolidBrush(UiTheme.Accent))
        {
            g.DrawString("Revenue over time", titleFont, titleBrush, 14, 10);
        }

        // Tomt tillstånd: visa hjälptext i mitten av panelen.
        if (_points.Count == 0)
        {
            using (Font emptyFont = new Font("Segoe UI", 10F, FontStyle.Italic))
            using (SolidBrush emptyBrush = new SolidBrush(UiTheme.TextDark))
            {
                string message = "No orders yet. Create one to see revenue grow.";
                SizeF textSize = g.MeasureString(message, emptyFont);
                float x = (size.Width - textSize.Width) / 2f;
                float y = (size.Height - textSize.Height) / 2f;
                g.DrawString(message, emptyFont, emptyBrush, x, y);
            }
            return;
        }

        // Definiera plottytan med marginaler för axel-etiketter.
        const int padLeft = 70;
        const int padRight = 25;
        const int padTop = 45;
        const int padBottom = 40;

        Rectangle plot = new Rectangle(
            padLeft,
            padTop,
            size.Width - padLeft - padRight,
            size.Height - padTop - padBottom);

        if (plot.Width <= 10 || plot.Height <= 10)
        {
            return;
        }

        DateTime firstDate = _points.First().Date;
        DateTime lastDate = _points.Last().Date;
        // Ge X-axeln en miniminbredd även när alla ordrar lagts på samma sekund.
        TimeSpan totalSpan = lastDate - firstDate;
        if (totalSpan <= TimeSpan.Zero)
        {
            totalSpan = TimeSpan.FromHours(1);
        }

        decimal maxRevenue = _points.Last().Cumulative;
        if (maxRevenue <= 0m)
        {
            maxRevenue = 1m;
        }

        // Räkna ut pixel-positionen för varje datapunkt.
        PointF[] linePoints = new PointF[_points.Count];
        for (int i = 0; i < _points.Count; i++)
        {
            double xRatio = (_points[i].Date - firstDate).TotalMilliseconds / totalSpan.TotalMilliseconds;
            double yRatio = (double)(_points[i].Cumulative / maxRevenue);

            float px = plot.Left + (float)(xRatio * plot.Width);
            float py = plot.Bottom - (float)(yRatio * plot.Height);
            linePoints[i] = new PointF(px, py);
        }

        // Rita rutnät (horisontella linjer) + Y-axel-etiketter.
        using (Pen gridPen = new Pen(UiTheme.CardBorder) { DashStyle = DashStyle.Dot })
        using (Font axisFont = new Font("Segoe UI", 8.5F, FontStyle.Regular))
        using (SolidBrush axisBrush = new SolidBrush(UiTheme.TextDark))
        {
            const int yTicks = 4;
            for (int i = 0; i <= yTicks; i++)
            {
                float ratio = i / (float)yTicks;
                float y = plot.Bottom - ratio * plot.Height;
                g.DrawLine(gridPen, plot.Left, y, plot.Right, y);

                decimal value = maxRevenue * (decimal)ratio;
                string label = FormatMoney(value);
                SizeF labelSize = g.MeasureString(label, axisFont);
                g.DrawString(label, axisFont, axisBrush, plot.Left - labelSize.Width - 6, y - labelSize.Height / 2f);
            }

            // X-axelns etiketter: första vänsterställd, mitten centrerad, sista högerställd.
            // Det hindrar att första/sista datum klipps mot panelens kanter.
            DrawXLabel(g, axisFont, axisBrush, plot, firstDate, 0f, XLabelAlignment.Left);
            DrawXLabel(g, axisFont, axisBrush, plot, firstDate.AddTicks(totalSpan.Ticks / 2), 0.5f, XLabelAlignment.Center);
            DrawXLabel(g, axisFont, axisBrush, plot, lastDate, 1f, XLabelAlignment.Right);
        }

        // Rita själva linjen och accentens fyllning under den.
        using (Pen linePen = new Pen(UiTheme.Accent, 2.5f))
        {
            linePen.LineJoin = LineJoin.Round;
            linePen.StartCap = LineCap.Round;
            linePen.EndCap = LineCap.Round;

            if (linePoints.Length >= 2)
            {
                // Mjuk yta under linjen för att förstärka känslan av växande intäkter.
                PointF[] areaPoints = new PointF[linePoints.Length + 2];
                Array.Copy(linePoints, areaPoints, linePoints.Length);
                areaPoints[linePoints.Length] = new PointF(linePoints[^1].X, plot.Bottom);
                areaPoints[linePoints.Length + 1] = new PointF(linePoints[0].X, plot.Bottom);

                using (SolidBrush areaBrush = new SolidBrush(Color.FromArgb(30, UiTheme.Accent)))
                {
                    g.FillPolygon(areaBrush, areaPoints);
                }

                g.DrawLines(linePen, linePoints);
            }
            else if (linePoints.Length == 1)
            {
                // Endast en datapunkt - rita en liten markör.
                g.FillEllipse(new SolidBrush(UiTheme.Accent), linePoints[0].X - 4, linePoints[0].Y - 4, 8, 8);
            }

            // Markörer på varje datapunkt så att enskilda ordrar syns även när det är många.
            using (SolidBrush dotBrush = new SolidBrush(UiTheme.Accent))
            {
                foreach (PointF p in linePoints)
                {
                    g.FillEllipse(dotBrush, p.X - 3.5f, p.Y - 3.5f, 7, 7);
                }
            }
        }

        // Rita Y- och X-axlarna ovanpå rutnätet.
        using (Pen axisPen = new Pen(UiTheme.TextDark, 1.2f))
        {
            g.DrawLine(axisPen, plot.Left, plot.Top, plot.Left, plot.Bottom);
            g.DrawLine(axisPen, plot.Left, plot.Bottom, plot.Right, plot.Bottom);
        }
    }

    private enum XLabelAlignment { Left, Center, Right }

    // Ritar ett datumvärde under X-axeln. Justeringen styr att text vid kanterna inte klipps.
    private static void DrawXLabel(Graphics g, Font font, SolidBrush brush, Rectangle plot, DateTime date, float ratio, XLabelAlignment alignment)
    {
        string label = date.ToString("yyyy-MM-dd", SwedishCulture);
        SizeF labelSize = g.MeasureString(label, font);
        float anchor = plot.Left + ratio * plot.Width;
        float x = alignment switch
        {
            XLabelAlignment.Left => anchor,
            XLabelAlignment.Right => anchor - labelSize.Width,
            _ => anchor - labelSize.Width / 2f
        };
        float y = plot.Bottom + 6;
        g.DrawString(label, font, brush, x, y);
    }

    // Samma format som resten av appen - sv-SE-valuta.
    private static string FormatMoney(decimal value)
    {
        return value.ToString("C0", SwedishCulture);
    }
}
