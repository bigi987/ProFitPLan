using FitPlanPro.Models;
using FitPlanPro.Services;

namespace FitPlanPro.Controls;

public class SidebarControl : Panel
{
    private Button btnDashboard = new();
    private Button btnMeals = new();
    private Button btnBmi = new();

    public enum ActivePage { Dashboard, Meals, Bmi }
    private ActivePage _currentPage;
    private Form _parentForm;

    public SidebarControl(Form parentForm, ActivePage currentPage)
    {
        _parentForm = parentForm;
        _currentPage = currentPage;
        
        this.Width = 100;
        this.Dock = DockStyle.Left;
        this.BackColor = Color.FromArgb(25, 25, 25); // Slightly lighter dark for better contrast
        
        this.Paint += (s, e) => {
            var g = e.Graphics;
            using var pen = new Pen(Color.FromArgb(40, 40, 40), 1);
            g.DrawLine(pen, this.Width - 1, 0, this.Width - 1, this.Height);
        };

        SetupControls();
    }

    private void SetupControls()
    {
        int btnY = 30;
        int btnHeight = 85;

        btnDashboard = CreateNavButton("Панель", "Dashboard", "Panou", "📊", btnY, _currentPage == ActivePage.Dashboard);
        btnDashboard.Click += (s, e) => NavigateTo(ActivePage.Dashboard);
        
        btnY += btnHeight;
        btnMeals = CreateNavButton("Питание", "Meals", "Mese", "🍏", btnY, _currentPage == ActivePage.Meals);
        btnMeals.Click += (s, e) => NavigateTo(ActivePage.Meals);
        
        btnY += btnHeight;
        btnBmi = CreateNavButton("ИМТ", "BMI", "IMC", "⚖️", btnY, _currentPage == ActivePage.Bmi);
        btnBmi.Click += (s, e) => NavigateTo(ActivePage.Bmi);

        this.Controls.Add(btnDashboard);
        this.Controls.Add(btnMeals);
        this.Controls.Add(btnBmi);
    }

    private Button CreateNavButton(string ruText, string enText, string roText, string icon, int y, bool isActive)
    {
        var btn = new Button
        {
            Location = new Point(10, y),
            Size = new Size(80, 75),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            BackColor = Color.Transparent
        };
        btn.FlatAppearance.BorderSize = 0;
        btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 40, 40);
        btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(50, 50, 50);
        
        btn.Paint += (s, e) => {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            
            if (isActive)
            {
                var rect = new Rectangle(0, 0, btn.Width, btn.Height);
                using var path = new System.Drawing.Drawing2D.GraphicsPath();
                int radius = 12;
                int d = radius * 2;
                path.AddArc(rect.X, rect.Y, d, d, 180, 90);
                path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
                path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
                path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
                path.CloseFigure();
                
                using var brush = new SolidBrush(ThemeService.PrimaryColor);
                g.FillPath(brush, path);
            }

            using var iconFont = new Font("Segoe UI Emoji", 18F);
            using var iconBrush = new SolidBrush(isActive ? Color.Black : Color.LightGray);
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(icon, iconFont, iconBrush, new Rectangle(0, -12, btn.Width, btn.Height), sf);
            
            using var textFont = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            string currentText = LanguageService.GetString(ruText, enText, roText);
            g.DrawString(currentText, textFont, iconBrush, new Rectangle(0, 22, btn.Width, btn.Height), sf);
        };
        
        Action updateAction = () => btn.Invalidate();
        LanguageService.LanguageChanged += updateAction;
        btn.Disposed += (s, e) => LanguageService.LanguageChanged -= updateAction;

        return btn;
    }

    private void NavigateTo(ActivePage page)
    {
        if (page == _currentPage) return;
        
        if (_currentPage != ActivePage.Dashboard)
        {
            _parentForm.DialogResult = DialogResult.OK; 
            _parentForm.Tag = page; 
            _parentForm.Close();
        }
        else
        {
            Form? frm = null;
            if (page == ActivePage.Meals) frm = new Forms.MealForm();
            if (page == ActivePage.Bmi) frm = new Forms.BmiCalculatorForm();
            
            if (frm != null)
            {
                frm.ShowDialog();
                if (frm.Tag is ActivePage next && next != ActivePage.Dashboard)
                {
                    NavigateTo(next);
                }
            }
        }
    }
}
