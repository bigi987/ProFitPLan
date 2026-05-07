using FitPlanPro.Services;
using FitPlanPro.Helpers;

namespace FitPlanPro.Forms;

public class LoginForm : Form
{
    private LogoPanel logo = null!;
    private Label lblTitle = new();
    private TextBox txtUsername = new();
    private TextBox txtPassword = new();
    private Button btnLogin = new();
    private Button btnRegister = new();
    private Button btnLanguage = new();
    
    public LoginForm()
    {
        SetupForm();
        LanguageService.LanguageChanged += UpdateTexts;
        UpdateTexts();
        ThemeService.ApplyTheme(this);
    }
    
    private void SetupForm()
    {
        this.Text = "FitPlan Pro";
        this.Size = new Size(450, 520);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.Icon = LogoHelper.CreateWindowIcon();
        this.FormClosing += (s, e) => LanguageService.LanguageChanged -= UpdateTexts;
        
        // Logo
        logo = LogoHelper.CreateLoginLogo();
        logo.Location = new Point((this.ClientSize.Width - 90) / 2, 18);
        
        lblTitle.Text = "FitPlan Pro";
        lblTitle.Font = new Font("Segoe UI", 24F, FontStyle.Bold, GraphicsUnit.Point);
        lblTitle.AutoSize = true;
        lblTitle.Location = new Point(50, 115);
        
        txtUsername.PlaceholderText = "Login";
        txtUsername.Location = new Point(50, 170);
        txtUsername.Size = new Size(330, 35);
        txtUsername.Font = new Font("Segoe UI", 12F);
        
        txtPassword.PasswordChar = '*';
        txtPassword.Location = new Point(50, 230);
        txtPassword.Size = new Size(330, 35);
        txtPassword.Font = new Font("Segoe UI", 12F);
        
        btnLogin.Location = new Point(50, 300);
        btnLogin.Size = new Size(160, 45);
        btnLogin.Click += BtnLogin_Click!;
        
        btnRegister.Location = new Point(220, 300);
        btnRegister.Size = new Size(160, 45);
        btnRegister.Click += BtnRegister_Click!;
        
        btnLanguage.Text = LanguageService.GetNextLanguageLabel();
        btnLanguage.Location = new Point(50, 370);
        btnLanguage.Size = new Size(330, 40);
        btnLanguage.BackColor = Color.Transparent;
        btnLanguage.Click += (s, e) => 
        {
            LanguageService.ToggleLanguage();
            btnLanguage.Text = LanguageService.GetNextLanguageLabel();
        };
        
        this.Controls.Add(logo);
        this.Controls.Add(lblTitle);
        this.Controls.Add(txtUsername);
        this.Controls.Add(txtPassword);
        this.Controls.Add(btnLogin);
        this.Controls.Add(btnRegister);
        this.Controls.Add(btnLanguage);
    }
    
    private void UpdateTexts()
    {
        this.Text = LanguageService.GetString("FitPlan Pro - Вход", "FitPlan Pro - Login", "FitPlan Pro - Autentificare");
        txtUsername.PlaceholderText = LanguageService.GetString("Логин", "Username", "Utilizator");
        txtPassword.PlaceholderText = LanguageService.GetString("Пароль", "Password", "Parolă");
        btnLogin.Text = LanguageService.GetString("Войти", "Login", "Autentificare");
        btnRegister.Text = LanguageService.GetString("Регистрация", "Register", "Înregistrare");
        btnLanguage.Text = LanguageService.GetNextLanguageLabel();
    }
    
    private void BtnLogin_Click(object sender, EventArgs e)
    {
        try
        {
            if (AuthService.Login(txtUsername.Text, txtPassword.Text))
            {
                var dashboard = new DashboardForm();
                dashboard.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show(
                    LanguageService.GetString("Неверный логин или пароль!", "Invalid username or password!", "Utilizator sau parolă incorectă!"),
                    LanguageService.GetString("Ошибка", "Error", "Eroare"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{LanguageService.GetString("Ошибка", "Error", "Eroare")}: {ex.Message}");
        }
    }
    
    private void BtnRegister_Click(object sender, EventArgs e)
    {
        var registerForm = new RegisterForm();
        registerForm.ShowDialog();
    }
}