using FitPlanPro.Models;
using FitPlanPro.Services;
using FitPlanPro.Helpers;

namespace FitPlanPro.Forms;

public class RegisterForm : Form
{
    private LogoPanel logo = null!;
    private Label lblTitle = new();
    private TextBox txtUsername = new();
    private TextBox txtPassword = new();
    private TextBox txtConfirmPassword = new();
    private Button btnRegister = new();
    private Button btnCancel = new();
    private Button btnLanguage = new();
    
    public RegisterForm()
    {
        SetupForm();
        LanguageService.LanguageChanged += UpdateTexts;
        UpdateTexts();
        ThemeService.ApplyTheme(this);
    }
    
    private void SetupForm()
    {
        this.Size = new Size(450, 570);
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
        
        txtUsername.Location = new Point(50, 170);
        txtUsername.Size = new Size(330, 35);
        txtUsername.Font = new Font("Segoe UI", 12F);
        
        txtPassword.PasswordChar = '*';
        txtPassword.Location = new Point(50, 230);
        txtPassword.Size = new Size(330, 35);
        txtPassword.Font = new Font("Segoe UI", 12F);
        
        txtConfirmPassword.PasswordChar = '*';
        txtConfirmPassword.Location = new Point(50, 290);
        txtConfirmPassword.Size = new Size(330, 35);
        txtConfirmPassword.Font = new Font("Segoe UI", 12F);
        
        btnRegister.Location = new Point(50, 360);
        btnRegister.Size = new Size(160, 45);
        btnRegister.Click += BtnRegister_Click!;
        
        btnCancel.Location = new Point(220, 360);
        btnCancel.Size = new Size(160, 45);
        btnCancel.Click += (s, e) => this.Close();
        
        btnLanguage.Location = new Point(50, 430);
        btnLanguage.Size = new Size(330, 40);
        btnLanguage.BackColor = Color.Transparent;
        btnLanguage.Click += (s, e) => 
        {
            LanguageService.ToggleLanguage();
        };
        
        this.Controls.Add(logo);
        this.Controls.Add(lblTitle);
        this.Controls.Add(txtUsername);
        this.Controls.Add(txtPassword);
        this.Controls.Add(txtConfirmPassword);
        this.Controls.Add(btnRegister);
        this.Controls.Add(btnCancel);
        this.Controls.Add(btnLanguage);
    }
    
    private void UpdateTexts()
    {
        this.Text = LanguageService.GetString("Регистрация", "Registration", "Înregistrare");
        txtUsername.PlaceholderText = LanguageService.GetString("Логин", "Username", "Utilizator");
        txtPassword.PlaceholderText = LanguageService.GetString("Пароль", "Password", "Parolă");
        txtConfirmPassword.PlaceholderText = LanguageService.GetString("Подтвердите пароль", "Confirm password", "Confirmați parola");
        btnRegister.Text = LanguageService.GetString("Регистрация", "Register", "Înregistrare");
        btnCancel.Text = LanguageService.GetString("Отмена", "Cancel", "Anulare");
        btnLanguage.Text = LanguageService.GetNextLanguageLabel();
    }
    
    private void BtnRegister_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtUsername.Text))
        {
            MessageBox.Show(LanguageService.GetString("Введите логин!", "Enter username!", "Introduceți utilizatorul!"));
            return;
        }
        
        if (txtPassword.Text != txtConfirmPassword.Text)
        {
            MessageBox.Show(LanguageService.GetString("Пароли не совпадают!", "Passwords do not match!", "Parolele nu coincid!"));
            return;
        }
        
        if (txtPassword.Text.Length < 3)
        {
            MessageBox.Show(LanguageService.GetString("Пароль должен быть не менее 3 символов!", "Password must be at least 3 characters!", "Parola trebuie să aibă cel puțin 3 caractere!"));
            return;
        }
        
        if (AuthService.Register(txtUsername.Text, txtPassword.Text))
        {
            MessageBox.Show(
                LanguageService.GetString("Регистрация успешна! Теперь войдите.", "Registration successful! Please login.", "Înregistrare reușită! Vă rugăm să vă autentificați."));
            this.Close();
        }
        else
        {
            MessageBox.Show(LanguageService.GetString("Пользователь с таким логином уже существует!", "User with this username already exists!", "Un utilizator cu acest nume există deja!"));
        }
    }
}