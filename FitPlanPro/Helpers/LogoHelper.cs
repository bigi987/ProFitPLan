using System.Drawing.Drawing2D;
using FitPlanPro.Services;

namespace FitPlanPro.Helpers;

/// <summary>
/// Generates the FitPlan Pro logo programmatically using GDI+ 
/// so no external image file is needed.
/// </summary>
public class LogoPanel : Panel
{
    public int LogoSize { get; set; } = 80;

    public LogoPanel()
    {
        this.DoubleBuffered = true;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
        this.BackColor = Color.Transparent;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;

        int s = LogoSize;
        int cx = (this.Width - s) / 2;
        int cy = (this.Height - s) / 2;

        // --- Outer circle with gradient ---
        var circleRect = new Rectangle(cx, cy, s, s);
        using var gradientBrush = new LinearGradientBrush(
            circleRect,
            Color.FromArgb(92, 92, 255),   // Primary purple/blue
            Color.FromArgb(155, 89, 182),  // Amethyst purple
            45F);

        // Color blend for richer gradient
        var blend = new ColorBlend(3);
        blend.Colors = new[] {
            Color.FromArgb(92, 92, 255),
            Color.FromArgb(123, 90, 218),
            Color.FromArgb(155, 89, 182)
        };
        blend.Positions = new[] { 0f, 0.5f, 1f };
        gradientBrush.InterpolationColors = blend;

        // Draw circle with shadow
        using var shadowBrush = new SolidBrush(Color.FromArgb(40, 0, 0, 0));
        g.FillEllipse(shadowBrush, cx + 3, cy + 3, s, s);
        g.FillEllipse(gradientBrush, circleRect);

        // --- Inner highlight arc (glass effect) ---
        using var highlightBrush = new SolidBrush(Color.FromArgb(45, 255, 255, 255));
        var highlightRect = new Rectangle(cx + (int)(s * 0.12), cy + (int)(s * 0.06), (int)(s * 0.76), (int)(s * 0.5));
        g.FillEllipse(highlightBrush, highlightRect);

        // --- "FP" Letters ---
        float fontSize = s * 0.36f;
        using var letterFont = new Font("Segoe UI", fontSize, FontStyle.Bold, GraphicsUnit.Pixel);
        using var letterBrush = new SolidBrush(Color.White);
        var letterFormat = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };
        var textRect = new RectangleF(cx, cy + s * 0.05f, s, s);
        g.DrawString("FP", letterFont, letterBrush, textRect, letterFormat);

        // --- Small heartbeat/pulse line under the letters ---
        float lineY = cy + s * 0.68f;
        float lineStartX = cx + s * 0.2f;
        float lineWidth = s * 0.6f;

        using var pulsePen = new Pen(Color.FromArgb(200, 255, 255, 255), Math.Max(1.5f, s * 0.025f));
        pulsePen.LineJoin = LineJoin.Round;
        pulsePen.StartCap = LineCap.Round;
        pulsePen.EndCap = LineCap.Round;

        // Create heartbeat/pulse shape
        var points = new PointF[]
        {
            new(lineStartX, lineY),
            new(lineStartX + lineWidth * 0.25f, lineY),
            new(lineStartX + lineWidth * 0.35f, lineY - s * 0.12f),
            new(lineStartX + lineWidth * 0.45f, lineY + s * 0.10f),
            new(lineStartX + lineWidth * 0.55f, lineY - s * 0.15f),
            new(lineStartX + lineWidth * 0.65f, lineY + s * 0.05f),
            new(lineStartX + lineWidth * 0.75f, lineY),
            new(lineStartX + lineWidth, lineY),
        };
        g.DrawLines(pulsePen, points);

        // --- Outer ring ---
        using var ringPen = new Pen(Color.FromArgb(60, 255, 255, 255), Math.Max(1.5f, s * 0.025f));
        g.DrawEllipse(ringPen, cx + 2, cy + 2, s - 4, s - 4);
    }
}

/// <summary>
/// Static helper to create a logo icon for the application window.
/// </summary>
public static class LogoHelper
{
    /// <summary>
    /// Creates a LogoPanel configured for the LoginForm (large logo).
    /// </summary>
    public static LogoPanel CreateLoginLogo()
    {
        return new LogoPanel
        {
            LogoSize = 80,
            Size = new Size(90, 90),
            Location = new Point(50, 20)
        };
    }

    /// <summary>
    /// Creates a LogoPanel configured for the DashboardForm header (medium logo).
    /// </summary>
    public static LogoPanel CreateDashboardLogo()
    {
        return new LogoPanel
        {
            LogoSize = 48,
            Size = new Size(56, 56),
            Location = new Point(40, 16)
        };
    }

    /// <summary>
    /// Generates a Bitmap of the logo that can be used as a window icon.
    /// </summary>
    public static Icon CreateWindowIcon()
    {
        int size = 48;
        using var bmp = new Bitmap(size, size);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        g.Clear(Color.Transparent);

        // Circle
        var rect = new Rectangle(1, 1, size - 3, size - 3);
        using var gradientBrush = new LinearGradientBrush(
            rect,
            Color.FromArgb(92, 92, 255),
            Color.FromArgb(155, 89, 182),
            45F);
        g.FillEllipse(gradientBrush, rect);

        // Letters
        using var font = new Font("Segoe UI", 16, FontStyle.Bold, GraphicsUnit.Pixel);
        using var brush = new SolidBrush(Color.White);
        var format = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };
        g.DrawString("FP", font, brush, new RectangleF(0, -1, size, size), format);

        // Convert to icon
        IntPtr hicon = bmp.GetHicon();
        return Icon.FromHandle(hicon);
    }
}
