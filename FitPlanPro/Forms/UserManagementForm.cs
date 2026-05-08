using FitPlanPro.Models;
using FitPlanPro.Services;

namespace FitPlanPro.Forms;

public class UserManagementForm : Form
{
    private DataGridView dgvUsers = new();
    private ComboBox cboRole = new();
    private Button btnUpdateRole = new();
    private Button btnDeleteUser = new();
    private Button btnRefresh = new();
    
    private User? _selectedUser;
    
    private Label lblRole = new();
    private Button btnLanguage = new();
    
    public UserManagementForm()
    {
        SetupForm();
        LoadUsers();
        ThemeService.ApplyTheme(this);
        LanguageService.LanguageChanged += UpdateTexts;
        UpdateTexts();
    }
    
    private void SetupForm()
    {
        this.Size = new Size(850, 550);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.FormClosing += (s, e) => LanguageService.LanguageChanged -= UpdateTexts;
        
        dgvUsers.Location = new Point(20, 20);
        dgvUsers.Size = new Size(500, 450);
        dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvUsers.MultiSelect = false;
        dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvUsers.RowTemplate.Height = 35;
        dgvUsers.SelectionChanged += DgvUsers_SelectionChanged!;
        
        Font labelFont = new Font("Segoe UI", 10F, FontStyle.Bold);
        Font inputFont = new Font("Segoe UI", 11F);
        
        int inputX = 550;
        int inputWidth = 250;
        
        lblRole.Location = new Point(inputX, 50);
        lblRole.Size = new Size(150, 25);
        lblRole.Font = labelFont;
        
        cboRole.Location = new Point(inputX, 80);
        cboRole.Size = new Size(inputWidth, 30);
        cboRole.Font = inputFont;
        cboRole.DropDownStyle = ComboBoxStyle.DropDownList;
        cboRole.Items.AddRange(new[] { "User", "Admin" });
        
        btnUpdateRole.Location = new Point(inputX, 130);
        btnUpdateRole.Size = new Size(inputWidth, 40);
        btnUpdateRole.Click += BtnUpdateRole_Click!;
        
        btnDeleteUser.Location = new Point(inputX, 190);
        btnDeleteUser.Size = new Size(inputWidth, 40);
        btnDeleteUser.Click += BtnDeleteUser_Click!;
        
        btnRefresh.Location = new Point(inputX, 250);
        btnRefresh.Size = new Size(inputWidth, 40);
        btnRefresh.Click += (s, e) => LoadUsers();
        
        btnLanguage.Location = new Point(inputX, 310);
        btnLanguage.Size = new Size(inputWidth, 40);
        btnLanguage.BackColor = Color.Transparent; // Set by ThemeService
        btnLanguage.Click += (s, e) => 
        {
            LanguageService.ToggleLanguage();
        };

        this.Controls.Add(dgvUsers);
        this.Controls.Add(lblRole);
        this.Controls.Add(cboRole);
        this.Controls.Add(btnUpdateRole);
        this.Controls.Add(btnDeleteUser);
        this.Controls.Add(btnRefresh);
        this.Controls.Add(btnLanguage);
    }
    
    private void UpdateTexts()
    {
        this.Text = LanguageService.GetString("Управление пользователями (Админ)", "User Management (Admin)", "Gestionarea utilizatorilor (Admin)");
        lblRole.Text = LanguageService.GetString("Роль:", "Role:", "Rol:");
        btnUpdateRole.Text = LanguageService.GetString("Обновить роль", "Update Role", "Actualizare rol");
        btnDeleteUser.Text = LanguageService.GetString("Удалить пользователя", "Delete User", "Ștergere utilizator");
        btnRefresh.Text = LanguageService.GetString("Обновить", "Refresh", "Actualizare");
        btnLanguage.Text = LanguageService.GetNextLanguageLabel();
    }
    
    private void LoadUsers()
    {
        dgvUsers.DataSource = null;
        dgvUsers.DataSource = DataService.Users.ToList();
        if (dgvUsers.Columns.Contains("PasswordHash"))
            dgvUsers.Columns["PasswordHash"].Visible = false;
        if (dgvUsers.Columns.Contains("Id"))
            dgvUsers.Columns["Id"].Visible = false;
    }
    
    private void DgvUsers_SelectionChanged(object sender, EventArgs e)
    {
        if (dgvUsers.SelectedRows.Count > 0)
        {
            _selectedUser = (User)dgvUsers.SelectedRows[0].DataBoundItem;
            cboRole.SelectedItem = _selectedUser.Role;
        }
    }
    
    private void BtnUpdateRole_Click(object sender, EventArgs e)
    {
        try
        {
            if (_selectedUser != null && AuthService.CurrentUser?.Id != _selectedUser.Id)
            {
                if (MessageBox.Show($"{LanguageService.GetString("Изменить роль пользователя", "Change role for user", "Modificare rol pentru utilizator")} '{_selectedUser.Username}' {LanguageService.GetString("на", "to", "în")} {cboRole.SelectedItem}?", 
                    LanguageService.GetString("Подтверждение", "Confirmation", "Confirmare"), MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _selectedUser.Role = cboRole.SelectedItem?.ToString() ?? "User";
                    DataService.UpdateUser(_selectedUser);
                    LoadUsers();
                    MessageBox.Show(LanguageService.GetString("Роль обновлена!", "Role updated!", "Rol actualizat!"));
                }
            }
            else if (_selectedUser != null && AuthService.CurrentUser?.Id == _selectedUser.Id)
            {
                MessageBox.Show(LanguageService.GetString("Нельзя изменить свою собственную роль!", "Cannot change your own role!", "Nu puteți schimba propriul rol!"), LanguageService.GetString("Предупреждение", "Warning", "Avertisment"), 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{LanguageService.GetString("Ошибка", "Error", "Eroare")}: {ex.Message}");
        }
    }
    
    private void BtnDeleteUser_Click(object sender, EventArgs e)
    {
        try
        {
            if (_selectedUser != null && AuthService.CurrentUser?.Id != _selectedUser.Id)
            {
                if (MessageBox.Show($"{LanguageService.GetString("Удалить пользователя", "Delete user", "Ștergere utilizator")} '{_selectedUser.Username}'?", 
                    LanguageService.GetString("Подтверждение", "Confirmation", "Confirmare"), MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    DataService.Users.Remove(_selectedUser);
                    DataService.DeleteUser(_selectedUser.Id);
                    LoadUsers();
                    _selectedUser = null;
                    MessageBox.Show(LanguageService.GetString("Пользователь удалён!", "User deleted!", "Utilizator șters!"));
                }
            }
            else if (_selectedUser != null && AuthService.CurrentUser?.Id == _selectedUser.Id)
            {
                MessageBox.Show(LanguageService.GetString("Нельзя удалить самого себя!", "Cannot delete yourself!", "Nu vă puteți șterge pe voi înșivă!"), LanguageService.GetString("Предупреждение", "Warning", "Avertisment"), 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{LanguageService.GetString("Ошибка", "Error", "Eroare")}: {ex.Message}");
        }
    }
}