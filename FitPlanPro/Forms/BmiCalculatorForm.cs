using FitPlanPro.Models;
using FitPlanPro.Services;
using FitPlanPro.Controls;
using System.Drawing.Drawing2D;

namespace FitPlanPro.Forms;

public class BmiCalculatorForm : Form
{
    private SidebarControl sidebar = null!;
    private Panel pnlCard = new();
    private Label lblTitle = new();
    private Label lblSubtitle = new();
    
    private TextBox txtWeight = new();
    private TextBox txtHeight = new();
    private Label lblWeightLabel = new();
    private Label lblHeightLabel = new();
    
    private Button btnCalculate = new();
    private Label lblResult = new();
    private Label lblCategory = new();
    private Panel pnlGauge = new();
    private Panel pnlIndicator = new();
    
    private double _currentBmi = 0;

    public BmiCalculatorForm()
    {
        SetupForm();
        ThemeService.ApplyTheme(this);
        this.BackColor = Color.White; // Force whole form white
        
        LanguageService.LanguageChanged += UpdateTexts;
        UpdateTexts();
        this.FormClosing += (s, e) => LanguageService.LanguageChanged -= UpdateTexts;
    }

    private void SetupForm()
    {
        this.Text = LanguageService.GetString("Калькулятор ИМТ", "BMI Calculator", "Calculator IMC");
        this.Size = new Size(1100, 700);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;

        sidebar = new SidebarControl(this, SidebarControl.ActivePage.Bmi);
        this.Controls.Add(sidebar);

        // Center card
        pnlCard.Size = new Size(500, 600);
        pnlCard.Location = new Point((1100 - 500 + 100) / 2, (700 - 600) / 2);
        pnlCard.BackColor = Color.Transparent;
        pnlCard.Paint += (s, e) => {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new Rectangle(0, 0, pnlCard.Width - 1, pnlCard.Height - 1);
            
            // Subtle shadow
            using (var shadowPath = GetRoundedRectPath(new Rectangle(4, 4, rect.Width, rect.Height), 24))
            {
                using var shadowBrush = new SolidBrush(Color.FromArgb(20, 0, 0, 0));
                g.FillPath(shadowBrush, shadowPath);
            }

            using var path = GetRoundedRectPath(rect, 24);
            using var brush = new SolidBrush(ThemeService.SurfaceColor);
            g.FillPath(brush, path);
            using var pen = new Pen(ThemeService.BorderColor, 1);
            g.DrawPath(pen, path);
        };

        lblTitle.Font = new Font("Segoe UI Black", 24F, FontStyle.Bold);
        lblTitle.TextAlign = ContentAlignment.MiddleCenter;
        lblTitle.Location = new Point(0, 35);
        lblTitle.Size = new Size(pnlCard.Width, 90);
        lblTitle.BackColor = Color.Transparent;

        lblSubtitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblSubtitle.ForeColor = ThemeService.SecondaryText;
        lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;
        lblSubtitle.Location = new Point(0, 125);
        lblSubtitle.Size = new Size(pnlCard.Width, 25);
        lblSubtitle.BackColor = Color.Transparent;

        int inputX = 60;
        int inputW = pnlCard.Width - 120;

        lblWeightLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblWeightLabel.ForeColor = ThemeService.SecondaryText;
        lblWeightLabel.Location = new Point(inputX, 170);
        lblWeightLabel.AutoSize = true;
        lblWeightLabel.BackColor = Color.Transparent;

        txtWeight.Location = new Point(inputX, 195);
        txtWeight.Size = new Size(inputW, 40);
        txtWeight.Font = new Font("Segoe UI", 16F);
        txtWeight.BorderStyle = BorderStyle.FixedSingle;

        lblHeightLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblHeightLabel.ForeColor = ThemeService.SecondaryText;
        lblHeightLabel.Location = new Point(inputX, 255);
        lblHeightLabel.AutoSize = true;
        lblHeightLabel.BackColor = Color.Transparent;

        txtHeight.Location = new Point(inputX, 280);
        txtHeight.Size = new Size(inputW, 40);
        txtHeight.Font = new Font("Segoe UI", 16F);
        txtHeight.BorderStyle = BorderStyle.FixedSingle;

        btnCalculate.Location = new Point(inputX, 350);
        btnCalculate.Size = new Size(inputW, 60);
        btnCalculate.BackColor = ThemeService.PrimaryColor;
        btnCalculate.ForeColor = Color.Black;
        btnCalculate.FlatStyle = FlatStyle.Flat;
        btnCalculate.FlatAppearance.BorderSize = 0;
        btnCalculate.Font = new Font("Segoe UI Black", 14F, FontStyle.Bold);
        btnCalculate.Cursor = Cursors.Hand;
        btnCalculate.Click += BtnCalculate_Click!;
        btnCalculate.Paint += (s, e) => {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new Rectangle(0, 0, btnCalculate.Width, btnCalculate.Height);
            using var path = GetRoundedRectPath(rect, 15);
            g.Clear(ThemeService.SurfaceColor);
            using var brush = new SolidBrush(btnCalculate.BackColor);
            g.FillPath(brush, path);
            
            // Text with better positioning
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            using var textBrush = new SolidBrush(btnCalculate.ForeColor);
            g.DrawString(btnCalculate.Text, btnCalculate.Font, textBrush, rect, sf);
        };

        lblResult.Text = "";
        lblResult.Font = new Font("Segoe UI Black", 42F, FontStyle.Bold);
        lblResult.Location = new Point(0, 420);
        lblResult.Size = new Size(pnlCard.Width, 75);
        lblResult.TextAlign = ContentAlignment.MiddleCenter;
        lblResult.BackColor = Color.Transparent;

        lblCategory.Text = "";
        lblCategory.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        lblCategory.Location = new Point(0, 495);
        lblCategory.Size = new Size(pnlCard.Width, 35);
        lblCategory.TextAlign = ContentAlignment.MiddleCenter;
        lblCategory.BackColor = Color.Transparent;

        pnlGauge.Location = new Point(inputX, 545);
        pnlGauge.Size = new Size(inputW, 12);
        pnlGauge.Paint += PnlGauge_Paint!;

        pnlIndicator.Location = new Point(inputX, 560);
        pnlIndicator.Size = new Size(inputW, 15);
        pnlIndicator.BackColor = Color.Transparent;
        pnlIndicator.Paint += PnlIndicator_Paint!;

        pnlCard.Controls.Add(lblTitle);
        pnlCard.Controls.Add(lblSubtitle);
        pnlCard.Controls.Add(lblWeightLabel);
        pnlCard.Controls.Add(txtWeight);
        pnlCard.Controls.Add(lblHeightLabel);
        pnlCard.Controls.Add(txtHeight);
        pnlCard.Controls.Add(btnCalculate);
        pnlCard.Controls.Add(lblResult);
        pnlCard.Controls.Add(lblCategory);
        pnlCard.Controls.Add(pnlGauge);
        pnlCard.Controls.Add(pnlIndicator);

        this.Controls.Add(pnlCard);
    }

    private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
    {
        var path = new GraphicsPath();
        int d = radius * 2;
        path.AddArc(rect.X, rect.Y, d, d, 180, 90);
        path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
        path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
        path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }

    private void UpdateTexts()
    {
        this.Text = LanguageService.GetString("Калькулятор ИМТ", "BMI Calculator", "Calculator IMC");
        lblTitle.Text = LanguageService.GetString("⚖️ КАЛЬКУЛЯТОР ИМТ", "⚖️ BMI CALCULATOR", "⚖️ CALCULATOR IMC");
        lblTitle.ForeColor = ThemeService.PrimaryColor;
        lblSubtitle.Text = LanguageService.GetString("ИНДЕКС МАССЫ ТЕЛА", "BODY MASS INDEX", "INDICE DE MASĂ CORPORALĂ");
        lblWeightLabel.Text = LanguageService.GetString("ВЕС (КГ)", "WEIGHT (KG)", "GREUTATE (KG)");
        txtWeight.PlaceholderText = LanguageService.GetString("Введите ваш вес", "Enter your weight", "Introduceți greutatea");
        lblHeightLabel.Text = LanguageService.GetString("РОСТ (СМ)", "HEIGHT (CM)", "ÎNĂLȚIME (CM)");
        txtHeight.PlaceholderText = LanguageService.GetString("Введите ваш рост", "Enter your height", "Introduceți înălțimea");
        btnCalculate.Text = LanguageService.GetString("РАССЧИТАТЬ", "CALCULATE", "CALCULARE");
        
        if (_currentBmi > 0)
        {
            lblCategory.Text = GetBmiCategory(_currentBmi);
        }
        btnCalculate.Invalidate();
        pnlGauge.Invalidate();
    }

    private void BtnCalculate_Click(object sender, EventArgs e)
    {
        string wText = txtWeight.Text.Replace(',', '.');
        string hText = txtHeight.Text.Replace(',', '.');

        if (double.TryParse(wText, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double weight) &&
            double.TryParse(hText, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double height))
        {
            if (weight <= 0 || height <= 0) return;

            double heightM = height / 100.0;
            _currentBmi = weight / (heightM * heightM);
            
            lblResult.Text = $"{_currentBmi:F1}";
            lblResult.ForeColor = GetBmiColor(_currentBmi);
            
            string category = GetBmiCategory(_currentBmi);
            string emoji = _currentBmi < 18.5 ? "🎈" : (_currentBmi < 25 ? "✅" : (_currentBmi < 30 ? "⚠️" : "🚨"));
            lblCategory.Text = $"{emoji} {category}";
            lblCategory.ForeColor = GetBmiColor(_currentBmi);
            
            pnlGauge.Invalidate();
            pnlIndicator.Invalidate();
        }
    }

    private string GetBmiCategory(double bmi)
    {
        if (bmi < 18.5) return LanguageService.GetString("Недостаточный вес", "Underweight", "Subponderal");
        if (bmi < 25) return LanguageService.GetString("Нормальный вес", "Normal weight", "Greutate normală");
        if (bmi < 30) return LanguageService.GetString("Избыточный вес", "Overweight", "Supraponderal");
        return LanguageService.GetString("Ожирение", "Obesity", "Obezitate");
    }

    private Color GetBmiColor(double bmi)
    {
        if (bmi < 18.5) return Color.FromArgb(52, 152, 219);  // Blue
        if (bmi < 25) return Color.FromArgb(46, 204, 113);    // Emerald Green
        if (bmi < 30) return Color.FromArgb(241, 196, 15);    // Sunflower Yellow
        return Color.FromArgb(231, 76, 60);                  // Alizarin Red
    }

    private void PnlGauge_Paint(object sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        int w = pnlGauge.Width;
        int h = pnlGauge.Height;

        Color[] colors = { 
            Color.FromArgb(52, 152, 219), 
            Color.FromArgb(46, 204, 113), 
            Color.FromArgb(241, 196, 15), 
            Color.FromArgb(231, 76, 60) 
        };

        int segW = w / 4;
        for (int i = 0; i < 4; i++)
        {
            int x = i * segW;
            using var brush = new SolidBrush(colors[i]);
            
            if (i == 0) // Left rounded
            {
                using var path = GetRoundedRectPath(new Rectangle(x, 0, segW + 10, h), h / 2);
                g.FillPath(brush, path);
            }
            else if (i == 3) // Right rounded
            {
                using var path = GetRoundedRectPath(new Rectangle(x - 10, 0, segW + 10, h), h / 2);
                g.FillPath(brush, path);
            }
            else
            {
                g.FillRectangle(brush, x - 5, 0, segW + 10, h);
            }
        }
    }

    private void PnlIndicator_Paint(object sender, PaintEventArgs e)
    {
        if (_currentBmi <= 0) return;
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        
        // Clamp BMI for display
        double displayBmi = Math.Max(15, Math.Min(35, _currentBmi));
        float ratio = (float)((displayBmi - 15) / 20.0);
        int x = (int)(ratio * pnlIndicator.Width);
        x = Math.Max(10, Math.Min(pnlIndicator.Width - 10, x));
        
        using var brush = new SolidBrush(GetBmiColor(_currentBmi));
        using var pen = new Pen(ThemeService.SurfaceColor, 2);
        
        // Draw a nice circle indicator
        g.FillEllipse(brush, x - 6, 2, 12, 12);
        g.DrawEllipse(pen, x - 6, 2, 12, 12);
    }
}
