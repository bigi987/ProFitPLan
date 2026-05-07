using System.Drawing.Drawing2D;
using FitPlanPro.Services;

namespace FitPlanPro.Controls;

public class CircularProgressBar : Control
{
    private int _value = 0;
    public int Value
    {
        get => _value;
        set { _value = value; Invalidate(); }
    }

    private int _maximum = 100;
    public int Maximum
    {
        get => _maximum;
        set { _maximum = value; Invalidate(); }
    }

    public Color ProgressColor { get; set; } = ThemeService.PrimaryColor;
    public Color TrackColor { get; set; } = Color.FromArgb(40, 40, 40);
    public int LineWidth { get; set; } = 15;
    
    public string CenterText { get; set; } = "";
    public string SubText { get; set; } = "";

    public CircularProgressBar()
    {
        this.DoubleBuffered = true;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
        this.Size = new Size(150, 150);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        var rect = new Rectangle(LineWidth / 2, LineWidth / 2, Width - LineWidth, Height - LineWidth);

        // Draw track
        Color currentTrackColor = ThemeService.IsDarkMode ? Color.FromArgb(40, 40, 40) : Color.FromArgb(220, 220, 220);
        using var trackPen = new Pen(currentTrackColor, LineWidth);
        g.DrawArc(trackPen, rect, 0, 360);

        // Draw progress
        if (Maximum > 0 && Value > 0)
        {
            float sweepAngle = 360f * ((float)Value / Maximum);
            using var progressPen = new Pen(ProgressColor, LineWidth);
            progressPen.StartCap = LineCap.Round;
            progressPen.EndCap = LineCap.Round;
            g.DrawArc(progressPen, rect, -90, sweepAngle);
        }

        // Draw Text
        if (!string.IsNullOrEmpty(CenterText))
        {
            using var font = new Font("Segoe UI", 24F, FontStyle.Bold);
            using var brush = new SolidBrush(ThemeService.IsDarkMode ? Color.White : Color.Black);
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(CenterText, font, brush, new Rectangle(0, -10, Width, Height), sf);
        }
        
        if (!string.IsNullOrEmpty(SubText))
        {
            using var font = new Font("Segoe UI", 10F, FontStyle.Regular);
            using var brush = new SolidBrush(Color.Gray);
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(SubText, font, brush, new Rectangle(0, 30, Width, Height), sf);
        }
    }
}
