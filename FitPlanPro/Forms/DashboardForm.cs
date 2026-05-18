using FitPlanPro.Models;
using FitPlanPro.Services;
using FitPlanPro.Helpers;
using FitPlanPro.Controls;
using System.Drawing.Drawing2D;

namespace FitPlanPro.Forms;

public class StatCard : Panel
{
    public string Emoji { get; set; } = "";
    public string Title { get; set; } = "";
    public string Value { get; set; } = "0";
    public Color AccentColor { get; set; } = ThemeService.PrimaryColor;

    public StatCard()
    {
        this.Size = new Size(210, 120);
        this.DoubleBuffered = true;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        // Rounded rectangle background
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        using var path = CreateRoundedRect(rect, 16);

        Color parentBg = ThemeService.IsDarkMode ? Color.FromArgb(18, 18, 18) : Color.FromArgb(244, 247, 246);
        g.Clear(parentBg);

        Color bg = ThemeService.IsDarkMode ? Color.FromArgb(28, 28, 28) : Color.White;
        using var bgBrush = new SolidBrush(bg);
        g.FillPath(bgBrush, path);

        // Emoji at top right corner
        using var emojiFont = new Font("Segoe UI Emoji", 14F);
        using var emojiBrush = new SolidBrush(AccentColor);
        g.DrawString(Emoji, emojiFont, emojiBrush, new PointF(Width - 35, 15));
        
        // Small circle behind emoji
        using var circleBrush = new SolidBrush(Color.FromArgb(40, AccentColor));
        g.FillEllipse(circleBrush, Width - 40, 10, 30, 30);

        // Title at top left
        Color textColor = ThemeService.IsDarkMode ? Color.FromArgb(170, 170, 170) : Color.FromArgb(120, 130, 150);
        using var titleFont = new Font("Segoe UI", 9F, FontStyle.Bold);
        using var titleBrush = new SolidBrush(textColor);
        g.DrawString(Title.ToUpper(), titleFont, titleBrush, new PointF(15, 20));

        // Value in middle left
        Color valColor = ThemeService.IsDarkMode ? AccentColor : Color.FromArgb(30, 30, 40);
        using var valFont = new Font("Segoe UI Black", 24F, FontStyle.Bold);
        using var valBrush = new SolidBrush(valColor);
        g.DrawString(Value, valFont, valBrush, new PointF(12, 45));
        
        // Small unit text next to value
        using var unitFont = new Font("Segoe UI", 9F, FontStyle.Regular);
        string unitText = LanguageService.GetString("ккал", "kcal", "kcal");
        g.DrawString(unitText, unitFont, titleBrush, new PointF(15 + g.MeasureString(Value, valFont).Width, 65));

        // Subtitle at bottom
        using var subFont = new Font("Segoe UI", 8F, FontStyle.Bold);
        string subText = LanguageService.GetString("📈 +5% за день", "📈 +5% today", "📈 +5% astăzi");
        g.DrawString(subText, subFont, emojiBrush, new PointF(15, 90));

        // Accent top border
        using var accentBrush = new SolidBrush(AccentColor);
        var stripPath = CreateRoundedRect(new Rectangle(0, 0, Width - 1, 4), 2);
        g.FillPath(accentBrush, stripPath);
        
        // Border
        Color borderColor = ThemeService.IsDarkMode ? Color.FromArgb(40, 40, 40) : Color.FromArgb(230, 230, 235);
        using var borderPen = new Pen(borderColor, 1);
        g.DrawPath(borderPen, path);
    }

    private static GraphicsPath CreateRoundedRect(Rectangle rect, int radius)
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
}

public class DashboardForm : Form
{
    private SidebarControl sidebar = null!;
    private Button btnMeals = new();
    private Button btnWorkouts = new();
    private Button btnTheme = new();
    private Button btnUsers = new();
    private Button btnExport = new();
    private Button btnLogout = new();
    private Button btnLanguage = new();
    private Button btnBmi = new();
    private Button btnTimer = new();
    
    private Label lblWelcome = new();
    private Label lblSubtitle = new();
    
    private Panel pnlProgress = new();
    private Label lblProgressLabel = new();
    private CircularProgressBar pbCalories = new();

    private StatCard cardCalories = new();
    private StatCard cardBurned = new();
    private StatCard cardBalance = new();
    private StatCard cardWorkouts = new();

    public DashboardForm()
    {
        SetupForm();
        ThemeService.ApplyTheme(this);
        LanguageService.LanguageChanged += UpdateTexts;
        UpdateTexts();
        this.FormClosing += (s, e) => LanguageService.LanguageChanged -= UpdateTexts;
    }

    private void SetupForm()
    {
        this.Text = "FitPlan Pro - Панель управления";
        this.Size = new Size(1100, 700);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.DoubleBuffered = true;
        this.Icon = LogoHelper.CreateWindowIcon();
        
        sidebar = new SidebarControl(this, SidebarControl.ActivePage.Dashboard);
        this.Controls.Add(sidebar);

        int contentX = 140;

        lblWelcome.Text = $"👋 {LanguageService.GetString("Добро пожаловать", "Welcome", "Bun venit")}, {AuthService.CurrentUser?.Username}!";
        lblWelcome.Font = new Font("Segoe UI Black", 24F, FontStyle.Bold);
        lblWelcome.Location = new Point(contentX, 30);
        lblWelcome.AutoSize = true;

        lblSubtitle.Text = LanguageService.GetString("Ваша панель управления фитнесом", "Your fitness dashboard", "Panoul tău de fitness");
        lblSubtitle.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
        lblSubtitle.ForeColor = Color.Gray;
        lblSubtitle.Location = new Point(contentX, 75);
        lblSubtitle.AutoSize = true;

        // Stat Cards Row
        int cardY = 120;
        int cardWidth = 210;
        int cardSpacing = 20;

        SetupStatCards(contentX, cardY, cardWidth, cardSpacing);

        // Progress section
        pnlProgress.Location = new Point(contentX, 300);
        pnlProgress.Size = new Size(240, 320);
        pnlProgress.BackColor = ThemeService.IsDarkMode ? Color.FromArgb(28, 28, 28) : Color.White;
        pnlProgress.Paint += (s, e) => {
             var g = e.Graphics;
             g.SmoothingMode = SmoothingMode.AntiAlias;
             var rect = new Rectangle(0, 0, pnlProgress.Width - 1, pnlProgress.Height - 1);
             int radius = 16;
             int d = radius * 2;
             var path = new GraphicsPath();
             path.AddArc(rect.X, rect.Y, d, d, 180, 90);
             path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
             path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
             path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
             path.CloseFigure();
             
             Color parentBg = ThemeService.IsDarkMode ? Color.FromArgb(18, 18, 18) : Color.FromArgb(244, 247, 246);
             g.Clear(parentBg);
             
             Color pnlBg = ThemeService.IsDarkMode ? Color.FromArgb(28, 28, 28) : Color.White;
             g.FillPath(new SolidBrush(pnlBg), path);
             g.DrawPath(new Pen(ThemeService.IsDarkMode ? Color.FromArgb(40, 40, 40) : Color.FromArgb(220, 220, 220), 1), path);
        };
        
        lblProgressLabel.Text = LanguageService.GetString("ПРОГРЕСС КАЛОРИЙ", "CALORIE PROGRESS", "PROGRES CALORII");
        lblProgressLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblProgressLabel.ForeColor = Color.Gray;
        lblProgressLabel.Location = new Point(40, 20);
        lblProgressLabel.AutoSize = true;
        lblProgressLabel.BackColor = Color.Transparent;

        pbCalories.Location = new Point(20, 60);
        pbCalories.Size = new Size(200, 200);
        pbCalories.LineWidth = 20;

        pnlProgress.Controls.Add(lblProgressLabel);
        pnlProgress.Controls.Add(pbCalories);

        // Buttons grid next to progress
        int btnWidth = 220;
        int btnHeight = 70;
        int col1 = contentX + 270;
        int col2 = col1 + btnWidth + 20;
        int col3 = col2 + btnWidth + 20;
        int row1 = 300;
        int row2 = row1 + btnHeight + 20;
        int row3 = row2 + btnHeight + 20;

        SetupButton(btnMeals, "🍏", "Управление\nпитанием", "Meal\nManagement", "Gestionare\nmaselor", col1, row1, btnWidth, btnHeight);
        btnMeals.Click += (s, e) => { new MealForm().ShowDialog(); RefreshStats(); };

        SetupButton(btnWorkouts, "💪", "Управление\nтренировками", "Workout\nManagement", "Gestionare\nantrenamente", col2, row1, btnWidth, btnHeight);
        btnWorkouts.Click += (s, e) => { new WorkoutForm().ShowDialog(); RefreshStats(); };

        SetupButton(btnUsers, "👥", "Управление\nпользователями", "User\nManagement", "Gestionare\nutilizatori", col3, row1, btnWidth, btnHeight);

        SetupButton(btnTimer, "⏱️", "Таймер\nтренировки", "Workout\nTimer", "Cronometru\nantrenament", col1, row2, btnWidth, btnHeight);
        btnTimer.Click += (s, e) => new WorkoutTimerForm().ShowDialog();

        SetupButton(btnBmi, "⚖️", "Калькулятор\nИМТ", "BMI\nCalculator", "Calculator\nIMC", col2, row2, btnWidth, btnHeight);
        btnBmi.Click += (s, e) => new BmiCalculatorForm().ShowDialog();

        SetupButton(btnExport, "📥", "Экспорт\nданных", "Export\nData", "Exportare\ndate", col3, row2, btnWidth, btnHeight);
        btnExport.Click += BtnExport_Click!;

        SetupButton(btnTheme, "🎨", "Сменить\nтему", "Change\nTheme", "Schimbă\ntema", col1, row3, btnWidth, btnHeight);
        btnTheme.Click += BtnTheme_Click!;

        SetupButton(btnLanguage, "🌐", "Язык / Language", "Language / Язык", "Limbă / Limba", col2, row3, btnWidth, btnHeight);
        btnLanguage.Click += (s, e) => { LanguageService.ToggleLanguage(); };

        SetupButton(btnLogout, "🚪", "Выйти", "Logout", "Deconectare", col3, row3, btnWidth, btnHeight);
        btnLogout.Click += BtnLogout_Click!;

        this.Controls.Add(lblWelcome);
        this.Controls.Add(lblSubtitle);
        this.Controls.Add(cardCalories);
        this.Controls.Add(cardBurned);
        this.Controls.Add(cardBalance);
        this.Controls.Add(cardWorkouts);
        this.Controls.Add(pnlProgress);
        this.Controls.Add(btnMeals);
        this.Controls.Add(btnWorkouts);
        this.Controls.Add(btnUsers);
        this.Controls.Add(btnTimer);
        this.Controls.Add(btnBmi);
        this.Controls.Add(btnExport);
        this.Controls.Add(btnTheme);
        this.Controls.Add(btnLanguage);
        this.Controls.Add(btnLogout);

        if (AuthService.CurrentUser?.Role == "Admin")
        {
            btnUsers.Visible = true;
            btnUsers.Click += (s, e) => new UserManagementForm().ShowDialog();
        }
        else
        {
            btnUsers.Visible = false;
        }

        this.Activated += (s, e) => RefreshStats();
    }

    private void SetupButton(Button btn, string icon, string ru, string en, string ro, int x, int y, int w, int h)
    {
        btn.Location = new Point(x, y);
        btn.Size = new Size(w, h);
        btn.FlatStyle = FlatStyle.Flat;
        btn.FlatAppearance.BorderSize = 0;
        btn.BackColor = ThemeService.IsDarkMode ? Color.FromArgb(28, 28, 28) : Color.White;
        btn.Cursor = Cursors.Hand;
        
        btn.Paint += (s, e) => {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new Rectangle(0, 0, btn.Width, btn.Height);
            
            // Rounded corners
            using var path = new GraphicsPath();
            int radius = 10;
            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            
            g.Clear(ThemeService.IsDarkMode ? Color.FromArgb(18, 18, 18) : Color.FromArgb(244, 247, 246));
            Color bgCol = ThemeService.IsDarkMode ? Color.FromArgb(28, 28, 28) : Color.White;
            using var bgBrush = new SolidBrush(bgCol);
            g.FillPath(bgBrush, path);
            using var pen = new Pen(ThemeService.IsDarkMode ? Color.FromArgb(40, 40, 40) : Color.FromArgb(220, 220, 220));
            g.DrawPath(pen, path);

            // Icon
            using var iconFont = new Font("Segoe UI Emoji", 16F);
            using var iconBrush = new SolidBrush(ThemeService.PrimaryColor);
            g.DrawString(icon, iconFont, iconBrush, new Point(20, h/2 - 15));
            
            // Text
            string text = LanguageService.GetString(ru, en, ro);
            using var textFont = new Font("Segoe UI", 9F, FontStyle.Bold);
            using var textBrush = new SolidBrush(ThemeService.IsDarkMode ? Color.White : Color.Black);
            g.DrawString(text, textFont, textBrush, new Point(60, h/2 - (text.Contains('\n') ? 15 : 8)));
        };
    }

    private void SetupStatCards(int startX, int y, int width, int spacing)
    {
        cardCalories.Location = new Point(startX, y);
        cardBurned.Location = new Point(startX + width + spacing, y);
        cardBalance.Location = new Point(startX + (width + spacing) * 2, y);
        cardWorkouts.Location = new Point(startX + (width + spacing) * 3, y);
        RefreshStats();
    }

    private void RefreshStats()
    {
        int totalMeals = DataService.Meals.Sum(m => m.Calories);
        int totalBurned = DataService.Workouts.Sum(w => w.CaloriesBurned);
        int balance = totalMeals - totalBurned;
        int workoutCount = DataService.Workouts.Count;

        cardCalories.Emoji = "🔥";
        cardCalories.Title = LanguageService.GetString("Потреблено", "Consumed", "Consumat");
        cardCalories.Value = totalMeals.ToString("N0");
        cardCalories.AccentColor = Color.FromArgb(255, 120, 50);

        cardBurned.Emoji = "💨";
        cardBurned.Title = LanguageService.GetString("Сожжено", "Burned", "Ars");
        cardBurned.Value = totalBurned.ToString("N0");
        cardBurned.AccentColor = ThemeService.PrimaryColor; // Lime Green

        cardBalance.Emoji = "📊";
        cardBalance.Title = LanguageService.GetString("Баланс", "Balance", "Balanța");
        cardBalance.Value = (balance >= 0 ? "+" : "") + balance.ToString("N0");
        cardBalance.AccentColor = Color.FromArgb(231, 76, 60);

        cardWorkouts.Emoji = "📈";
        cardWorkouts.Title = LanguageService.GetString("Тренировок", "Workouts", "Antrenamente");
        cardWorkouts.Value = workoutCount.ToString();
        cardWorkouts.AccentColor = Color.FromArgb(52, 152, 219); // Blue

        cardCalories.Invalidate();
        cardBurned.Invalidate();
        cardBalance.Invalidate();
        cardWorkouts.Invalidate();

        UpdateProgress(totalMeals, totalBurned);
    }

    private void UpdateProgress(int consumed, int burned)
    {
        int totalTarget = AuthService.CurrentUser?.TargetCalories ?? 2000;
        int percent = (int)((double)consumed / totalTarget * 100);
        percent = Math.Min(100, Math.Max(0, percent));
        
        pbCalories.Value = percent;
        pbCalories.Maximum = 100;
        pbCalories.CenterText = $"{percent}%";
        
        string leftText = LanguageService.GetString("Осталось", "Remaining", "Rămas");
        string unitText = LanguageService.GetString("ккал", "kcal", "kcal");
        pbCalories.SubText = $"{leftText}\n{totalTarget - consumed} {unitText}";
    }

    private void BtnTheme_Click(object sender, EventArgs e)
    {
        ThemeService.ToggleTheme(this);
        cardCalories.Invalidate();
        cardBurned.Invalidate();
        cardBalance.Invalidate();
        cardWorkouts.Invalidate();
        pnlProgress.Invalidate();
        foreach (Control c in Controls) if (c is Button) c.Invalidate();
    }

    private void BtnExport_Click(object sender, EventArgs e)
    {
        using var saveDialog = new SaveFileDialog();
        saveDialog.Filter = "CSV files (*.csv)|*.csv";
        saveDialog.Title = LanguageService.GetString("Экспорт данных", "Export Data", "Exportare date");

        if (saveDialog.ShowDialog() == DialogResult.OK)
        {
            ExportService.ExportToCsv(DataService.Meals, saveDialog.FileName);
        }
    }

    private void BtnLogout_Click(object sender, EventArgs e)
    {
        if (MessageBox.Show(
                LanguageService.GetString("Вы уверены, что хотите выйти?", "Are you sure you want to logout?", "Ești sigur că vrei să te deconectezi?"),
                LanguageService.GetString("Подтверждение", "Confirmation", "Confirmare"),
                MessageBoxButtons.YesNo) == DialogResult.Yes)
        {
            AuthService.Logout();
            var login = new LoginForm();
            login.Show();
            this.Close();
        }
    }

    private void UpdateTexts()
    {
        this.Text = LanguageService.GetString("FitPlan Pro - Панель управления", "FitPlan Pro - Dashboard", "FitPlan Pro - Panou de control");
        lblWelcome.Text = $"👋 {LanguageService.GetString("Добро пожаловать", "Welcome", "Bun venit")}, {AuthService.CurrentUser?.Username}!";
        lblSubtitle.Text = LanguageService.GetString("Ваша панель управления фитнесом", "Your fitness dashboard", "Panoul tău de fitness");

        lblProgressLabel.Text = LanguageService.GetString("ПРОГРЕСС КАЛОРИЙ", "CALORIE PROGRESS", "PROGRES CALORII");

        RefreshStats();
        foreach (Control c in Controls) if (c is Button) c.Invalidate();
    }
}
