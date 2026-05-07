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
        pnlGauge.BackColor = Color.Transparent;
        lblTitle.ForeColor = ThemeService.PrimaryColor;
        btnCalculate.ForeColor = Color.Black;
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
        pnlCard.Size = new Size(450, 550);
        pnlCard.Location = new Point((1100 - 450 + 100) / 2, (700 - 550) / 2);
        pnlCard.BackColor = ThemeService.IsDarkMode ? Color.FromArgb(28, 28, 28) : Color.White;
        pnlCard.Paint += (s, e) => {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new Rectangle(0, 0, pnlCard.Width - 1, pnlCard.Height - 1);
            using var path = new GraphicsPath();
            int radius = 16;
            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            
            Color parentBg = ThemeService.IsDarkMode ? Color.FromArgb(18, 18, 18) : Color.FromArgb(244, 247, 246);
            g.Clear(parentBg);
            
            using var brush = new SolidBrush(ThemeService.IsDarkMode ? Color.FromArgb(28, 28, 28) : Color.White);
            g.FillPath(brush, path);
            using var pen = new Pen(ThemeService.IsDarkMode ? Color.FromArgb(40, 40, 40) : Color.FromArgb(220, 220, 220));
            g.DrawPath(pen, path);
        };

        lblTitle.Font = new Font("Segoe UI Black", 20F, FontStyle.Bold);
        lblTitle.ForeColor = ThemeService.PrimaryColor;
        lblTitle.TextAlign = ContentAlignment.MiddleCenter;
        lblTitle.Location = new Point(0, 30);
        lblTitle.Size = new Size(pnlCard.Width, 80);

        lblSubtitle.Font = new Font("Segoe UI", 10F);
        lblSubtitle.ForeColor = Color.Gray;
        lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;
        lblSubtitle.Location = new Point(0, 110);
        lblSubtitle.Size = new Size(pnlCard.Width, 20);

        int inputX = 40;
        int inputW = pnlCard.Width - 80;

        lblWeightLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblWeightLabel.ForeColor = Color.Gray;
        lblWeightLabel.Location = new Point(inputX, 150);
        lblWeightLabel.AutoSize = true;

        txtWeight.Location = new Point(inputX, 175);
        txtWeight.Size = new Size(inputW, 35);
        txtWeight.Font = new Font("Segoe UI", 14F);
        txtWeight.BackColor = Color.FromArgb(40, 40, 40);
        txtWeight.ForeColor = Color.White;
        txtWeight.BorderStyle = BorderStyle.FixedSingle;

        lblHeightLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblHeightLabel.ForeColor = Color.Gray;
        lblHeightLabel.Location = new Point(inputX, 230);
        lblHeightLabel.AutoSize = true;

        txtHeight.Location = new Point(inputX, 255);
        txtHeight.Size = new Size(inputW, 35);
        txtHeight.Font = new Font("Segoe UI", 14F);
        txtHeight.BackColor = Color.FromArgb(40, 40, 40);
        txtHeight.ForeColor = Color.White;
        txtHeight.BorderStyle = BorderStyle.FixedSingle;

        btnCalculate.Location = new Point(inputX, 320);
        btnCalculate.Size = new Size(inputW, 55);
        btnCalculate.BackColor = ThemeService.PrimaryColor;
        btnCalculate.ForeColor = Color.Black;
        btnCalculate.FlatStyle = FlatStyle.Flat;
        btnCalculate.FlatAppearance.BorderSize = 0;
        btnCalculate.Font = new Font("Segoe UI Black", 12F, FontStyle.Bold);
        btnCalculate.Click += BtnCalculate_Click!;
        btnCalculate.Paint += (s, e) => {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new Rectangle(0, 0, btnCalculate.Width, btnCalculate.Height);
            using var path = new GraphicsPath();
            int radius = 12;
            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            g.Clear(pnlCard.BackColor);
            using var brush = new SolidBrush(btnCalculate.BackColor);
            g.FillPath(brush, path);
            using var font = new Font("Segoe UI Black", 12F, FontStyle.Bold);
            using var textBrush = new SolidBrush(btnCalculate.ForeColor);
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(btnCalculate.Text, font, textBrush, rect, sf);
        };

        lblResult.Text = "";
        lblResult.Font = new Font("Segoe UI", 28F, FontStyle.Bold);
        lblResult.Location = new Point(0, 380);
        lblResult.Size = new Size(pnlCard.Width, 40);
        lblResult.TextAlign = ContentAlignment.MiddleCenter;

        lblCategory.Text = "";
        lblCategory.Font = new Font("Segoe UI", 12F, FontStyle.Regular);
        lblCategory.Location = new Point(0, 420);
        lblCategory.Size = new Size(pnlCard.Width, 25);
        lblCategory.TextAlign = ContentAlignment.MiddleCenter;

        pnlGauge.Location = new Point(inputX, 470);
        pnlGauge.Size = new Size(inputW, 30);
        pnlGauge.Paint += PnlGauge_Paint!;

        pnlIndicator.Location = new Point(inputX, 502);
        pnlIndicator.Size = new Size(inputW, 20);
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

    private void UpdateTexts()
    {
        this.Text = LanguageService.GetString("Калькулятор ИМТ", "BMI Calculator", "Calculator IMC");
        lblTitle.Text = "⚖️\n" + LanguageService.GetString("КАЛЬКУЛЯТОР ИМТ", "BMI CALCULATOR", "CALCULATOR IMC");
        lblSubtitle.Text = LanguageService.GetString("Индекс массы тела", "Body Mass Index", "Indicele de masă corporală");
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
        if (double.TryParse(txtWeight.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double weight) &&
            double.TryParse(txtHeight.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double height))
        {
            if (weight <= 0 || height <= 0) return;

            double heightM = height / 100.0;
            _currentBmi = weight / (heightM * heightM);
            
            lblResult.Text = $"{_currentBmi:F1}";
            lblResult.ForeColor = GetBmiColor(_currentBmi);
            lblCategory.Text = GetBmiCategory(_currentBmi);
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
        if (bmi < 18.5) return Color.FromArgb(52, 152, 219);
        if (bmi < 25) return Color.FromArgb(173, 196, 50); // Matches the screenshot better for normal
        if (bmi < 30) return Color.FromArgb(241, 130, 50);
        return Color.FromArgb(231, 76, 60);
    }

    private void PnlGauge_Paint(object sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        int w = pnlGauge.Width;
        int h = pnlGauge.Height;

        int segW = w / 4;
        Color[] colors = { Color.FromArgb(52, 152, 219), Color.FromArgb(173, 196, 50), Color.FromArgb(241, 130, 50), Color.FromArgb(231, 76, 60) };

        for (int i = 0; i < 4; i++)
        {
            using var brush = new SolidBrush(colors[i]);
            int x = i * segW;
            int currentW = (i == 3) ? (w - x) : segW;
            
            if (i == 0)
            {
                var path = new GraphicsPath();
                path.AddArc(x, 0, h, h, 90, 180);
                path.AddLine(x + h / 2, 0, x + currentW, 0);
                path.AddLine(x + currentW, 0, x + currentW, h);
                path.AddLine(x + currentW, h, x + h / 2, h);
                path.CloseFigure();
                g.FillPath(brush, path);
            }
            else if (i == 3)
            {
                var path = new GraphicsPath();
                path.AddLine(x, 0, x + currentW - h / 2, 0);
                path.AddArc(x + currentW - h, 0, h, h, -90, 180);
                path.AddLine(x + currentW - h / 2, h, x, h);
                path.CloseFigure();
                g.FillPath(brush, path);
            }
            else
            {
                g.FillRectangle(brush, x, 0, currentW, h);
            }
        }

        string[] labels = { "< 18.5", "18.5 - 25", "25 - 30", "> 30" };
        using var labelFont = new Font("Segoe UI", 7F, FontStyle.Bold);
        using var whiteBrush = new SolidBrush(Color.White);
        var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        for (int i = 0; i < 4; i++)
        {
            int x = i * segW;
            int currentW = (i == 3) ? (w - x) : segW;
            g.DrawString(labels[i], labelFont, whiteBrush, new RectangleF(x, 0, currentW, h), sf);
        }
        
        string[] cats = {
            LanguageService.GetString("НЕДОСТАТОК", "UNDERWEIGHT", "SUBPONDERAL"),
            LanguageService.GetString("НОРМА", "NORMAL", "NORMAL"),
            LanguageService.GetString("ИЗБЫТОК", "OVERWEIGHT", "SUPRAPONDERAL"),
            LanguageService.GetString("ОЖИРЕНИЕ", "OBESE", "OBEZITATE")
        };
        using var catBrush = new SolidBrush(Color.Gray);
        for (int i = 0; i < 4; i++)
        {
            int x = i * segW;
            int currentW = (i == 3) ? (w - x) : segW;
            g.DrawString(cats[i], labelFont, catBrush, new RectangleF(x, h, currentW, 20), sf);
        }
    }

    private void PnlIndicator_Paint(object sender, PaintEventArgs e)
    {
        if (_currentBmi <= 0) return;
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        double clampedBmi = Math.Max(10, Math.Min(40, _currentBmi));
        int x = (int)((clampedBmi - 10) / 30.0 * pnlIndicator.Width);
        x = Math.Max(5, Math.Min(pnlIndicator.Width - 5, x));
        
        Point[] triangle = { new Point(x, 0), new Point(x - 8, 16), new Point(x + 8, 16) };
        using var brush = new SolidBrush(GetBmiColor(_currentBmi));
        g.FillPolygon(brush, triangle);
    }
}
