using FitPlanPro.Models;
using FitPlanPro.Services;
using FitPlanPro.Controls;
using System.Drawing.Drawing2D;

namespace FitPlanPro.Forms;

public class MealForm : Form
{
    private SidebarControl sidebar = null!;
    private Label lblTitle = new();
    private TextBox txtSearch = new();
    private DataGridView dgvMeals = new();
    
    // Right panel controls
    private Panel pnlEdit = new();
    private Label lblEditTitle = new();
    private TextBox txtName = new();
    private TextBox txtCalories = new();
    private TextBox txtProtein = new();
    private TextBox txtFat = new();
    private TextBox txtCarbs = new();
    private Button btnAdd = new();
    private Button btnUpdate = new();
    private Button btnDelete = new();
    
    private Label lblNameLabel = new();
    private Label lblCaloriesLabel = new();
    private Label lblProteinLabel = new();
    private Label lblFatLabel = new();
    private Label lblCarbsLabel = new();
    
    // Preview panel
    private Panel pnlPreview = new();
    private Label lblPreviewName = new();
    
    private Meal? _selectedMeal;
    
    public MealForm()
    {
        SetupForm();
        LoadMeals();
        ThemeService.ApplyTheme(this);
        lblTitle.ForeColor = ThemeService.PrimaryColor;
        lblEditTitle.ForeColor = ThemeService.PrimaryColor;
        btnAdd.ForeColor = Color.Black;
        LanguageService.LanguageChanged += UpdateTexts;
        UpdateTexts();
    }
    
    private void SetupForm()
    {
        this.Size = new Size(1100, 700);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.FormClosing += (s, e) => LanguageService.LanguageChanged -= UpdateTexts;
        
        sidebar = new SidebarControl(this, SidebarControl.ActivePage.Meals);
        this.Controls.Add(sidebar);

        int contentX = 140;
        
        lblTitle.Text = "УПРАВЛЕНИЕ ПИТАНИЕМ";
        lblTitle.Font = new Font("Segoe UI Black", 20F, FontStyle.Bold);
        lblTitle.Location = new Point(contentX, 30);
        lblTitle.AutoSize = true;

        txtSearch.Location = new Point(550, 35);
        txtSearch.Size = new Size(200, 30);
        txtSearch.Font = new Font("Segoe UI", 11F);
        txtSearch.PlaceholderText = "🔍 Поиск по названию...";
        txtSearch.TextChanged += TxtSearch_TextChanged!;

        // DataGridView
        dgvMeals.Location = new Point(contentX, 90);
        dgvMeals.Size = new Size(610, 530);
        dgvMeals.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvMeals.MultiSelect = false;
        dgvMeals.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvMeals.RowTemplate.Height = 45;
        dgvMeals.SelectionChanged += DgvMeals_SelectionChanged!;

        // Right Panel (Edit)
        int panelX = 780;
        pnlEdit.Location = new Point(panelX, 30);
        pnlEdit.Size = new Size(280, 400);
        
        lblEditTitle.Text = "РЕДАКТИРОВАТЬ";
        lblEditTitle.Font = new Font("Segoe UI Black", 14F, FontStyle.Bold);
        lblEditTitle.ForeColor = ThemeService.PrimaryColor;
        lblEditTitle.Location = new Point(0, 0);
        lblEditTitle.AutoSize = true;

        int y = 40;
        CreateInput(pnlEdit, lblNameLabel, "НАЗВАНИЕ", txtName, ref y, 280);
        CreateInput(pnlEdit, lblCaloriesLabel, "КАЛОРИИ (ККАЛ)", txtCalories, ref y, 280);
        
        int smallW = 85;
        int tempY = y;
        CreateInput(pnlEdit, lblProteinLabel, "БЕЛКИ (Г)", txtProtein, ref tempY, smallW, 0);
        tempY = y;
        CreateInput(pnlEdit, lblFatLabel, "ЖИРЫ (Г)", txtFat, ref tempY, smallW, 95);
        tempY = y;
        CreateInput(pnlEdit, lblCarbsLabel, "УГЛ. (Г)", txtCarbs, ref tempY, smallW, 190);
        y = tempY + 20;

        btnAdd.Text = "+ Добавить";
        btnAdd.Location = new Point(0, y);
        btnAdd.Size = new Size(130, 45);
        btnAdd.BackColor = ThemeService.PrimaryColor;
        btnAdd.ForeColor = Color.Black;
        btnAdd.FlatStyle = FlatStyle.Flat;
        btnAdd.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnAdd.Click += BtnAdd_Click!;

        btnUpdate.Text = "🔄 Обновить";
        btnUpdate.Location = new Point(140, y);
        btnUpdate.Size = new Size(140, 45);
        btnUpdate.FlatStyle = FlatStyle.Flat;
        btnUpdate.BackColor = Color.FromArgb(28, 28, 28);
        btnUpdate.ForeColor = ThemeService.PrimaryColor;
        btnUpdate.Click += BtnUpdate_Click!;
        
        y += 60;
        
        btnDelete.Text = "🗑 Удалить запись";
        btnDelete.Location = new Point(0, y);
        btnDelete.Size = new Size(280, 40);
        btnDelete.FlatStyle = FlatStyle.Flat;
        btnDelete.BackColor = Color.FromArgb(28, 28, 28);
        btnDelete.ForeColor = Color.IndianRed;
        btnDelete.Click += BtnDelete_Click!;

        pnlEdit.Controls.Add(lblEditTitle);
        pnlEdit.Controls.Add(btnAdd);
        pnlEdit.Controls.Add(btnUpdate);
        pnlEdit.Controls.Add(btnDelete);

        this.Controls.Add(lblTitle);
        this.Controls.Add(txtSearch);
        this.Controls.Add(dgvMeals);
        this.Controls.Add(pnlEdit);
    }
    
    private void CreateInput(Control parent, Label lbl, string labelText, TextBox txt, ref int y, int w, int xOffset = 0)
    {
        lbl.Text = labelText;
        lbl.Location = new Point(xOffset, y);
        lbl.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
        lbl.ForeColor = Color.Gray;
        lbl.AutoSize = true;
        y += 20;
        txt.Location = new Point(xOffset, y);
        txt.Size = new Size(w, 35);
        txt.Font = new Font("Segoe UI", 12F);
        txt.BackColor = Color.FromArgb(40, 40, 40);
        txt.ForeColor = Color.White;
        txt.BorderStyle = BorderStyle.FixedSingle;
        parent.Controls.Add(lbl);
        parent.Controls.Add(txt);
        y += 45;
    }
    
    private void UpdateTexts()
    {
        this.Text = LanguageService.GetString("Управление питанием", "Meal Management", "Gestionarea meselor");
        lblTitle.Text = LanguageService.GetString("УПРАВЛЕНИЕ ПИТАНИЕМ", "MEAL MANAGEMENT", "GESTIONAREA MESELOR");
        lblEditTitle.Text = LanguageService.GetString("РЕДАКТИРОВАТЬ", "EDIT", "EDITARE");
        btnAdd.Text = "+ " + LanguageService.GetString("Добавить", "Add", "Adăugare");
        btnUpdate.Text = "🔄 " + LanguageService.GetString("Обновить", "Update", "Actualizare");
        btnDelete.Text = "🗑 " + LanguageService.GetString("Удалить запись", "Delete Record", "Ștergere");
        txtSearch.PlaceholderText = "🔍 " + LanguageService.GetString("Поиск по названию...", "Search by name...", "Căutare după nume...");
        
        lblNameLabel.Text = LanguageService.GetString("НАЗВАНИЕ", "NAME", "NUME");
        lblCaloriesLabel.Text = LanguageService.GetString("КАЛОРИИ (ККАЛ)", "CALORIES (KCAL)", "CALORII (KCAL)");
        lblProteinLabel.Text = LanguageService.GetString("БЕЛКИ (Г)", "PROTEIN (G)", "PROTEINE (G)");
        lblFatLabel.Text = LanguageService.GetString("ЖИРЫ (Г)", "FAT (G)", "GRĂSIMI (G)");
        lblCarbsLabel.Text = LanguageService.GetString("УГЛ. (Г)", "CARBS (G)", "CARB. (G)");
        
        UpdateGridHeaders();
    }
    
    private void UpdateGridHeaders()
    {
        if (dgvMeals.Columns.Contains("Name"))
            dgvMeals.Columns["Name"].HeaderText = LanguageService.GetString("НАЗВАНИЕ", "NAME", "NUME");
        if (dgvMeals.Columns.Contains("Calories"))
            dgvMeals.Columns["Calories"].HeaderText = LanguageService.GetString("КАЛОРИИ", "CALORIES", "CALORII");
        if (dgvMeals.Columns.Contains("Protein"))
            dgvMeals.Columns["Protein"].HeaderText = LanguageService.GetString("БЕЛКИ", "PROTEIN", "PROTEINE");
        if (dgvMeals.Columns.Contains("Fat"))
            dgvMeals.Columns["Fat"].HeaderText = LanguageService.GetString("ЖИРЫ", "FAT", "GRĂSIMI");
        if (dgvMeals.Columns.Contains("Carbs"))
            dgvMeals.Columns["Carbs"].HeaderText = LanguageService.GetString("УГЛЕВОДЫ", "CARBS", "CARBOHIDRAȚI");
        if (dgvMeals.Columns.Contains("Date"))
            dgvMeals.Columns["Date"].HeaderText = LanguageService.GetString("ДАТА", "DATE", "DATA");
    }
    
    private void LoadMeals()
    {
        dgvMeals.DataSource = null;
        dgvMeals.DataSource = DataService.Meals.ToList();
        if (dgvMeals.Columns.Contains("Id")) dgvMeals.Columns["Id"].Visible = false;
        UpdateGridHeaders();
    }
    
    private void LoadFilteredMeals(string searchText)
    {
        var filtered = DataService.Meals
            .Where(m => string.IsNullOrEmpty(searchText) || m.Name.ToLower().Contains(searchText.ToLower()))
            .ToList();
        dgvMeals.DataSource = null;
        dgvMeals.DataSource = filtered;
        if (dgvMeals.Columns.Contains("Id")) dgvMeals.Columns["Id"].Visible = false;
        UpdateGridHeaders();
    }
    
    private void DgvMeals_SelectionChanged(object sender, EventArgs e)
    {
        if (dgvMeals.SelectedRows.Count > 0)
        {
            _selectedMeal = (Meal)dgvMeals.SelectedRows[0].DataBoundItem;
            txtName.Text = _selectedMeal.Name;
            txtCalories.Text = _selectedMeal.Calories.ToString();
            txtProtein.Text = _selectedMeal.Protein.ToString();
            txtFat.Text = _selectedMeal.Fat.ToString();
            txtCarbs.Text = _selectedMeal.Carbs.ToString();
        }
    }
    
    private void BtnAdd_Click(object sender, EventArgs e)
    {
        try
        {
            if (ValidateInput())
            {
                var meal = new Meal
                {
                    Id = DataService.NextMealId,
                    Name = txtName.Text,
                    Calories = int.Parse(txtCalories.Text),
                    Protein = double.Parse(txtProtein.Text),
                    Fat = double.Parse(txtFat.Text),
                    Carbs = double.Parse(txtCarbs.Text),
                    Date = DateTime.Now
                };
                DataService.AddMeal(meal);
                LoadMeals();
                ClearInputs();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{LanguageService.GetString("Ошибка", "Error", "Eroare")}: {ex.Message}");
        }
    }
    
    private void BtnUpdate_Click(object sender, EventArgs e)
    {
        try
        {
            if (_selectedMeal != null && ValidateInput())
            {
                _selectedMeal.Name = txtName.Text;
                _selectedMeal.Calories = int.Parse(txtCalories.Text);
                _selectedMeal.Protein = double.Parse(txtProtein.Text);
                _selectedMeal.Fat = double.Parse(txtFat.Text);
                _selectedMeal.Carbs = double.Parse(txtCarbs.Text);
                DataService.UpdateMeal(_selectedMeal.Id, _selectedMeal);
                LoadMeals();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{LanguageService.GetString("Ошибка", "Error", "Eroare")}: {ex.Message}");
        }
    }
    
    private void BtnDelete_Click(object sender, EventArgs e)
    {
        if (_selectedMeal != null)
        {
            if (MessageBox.Show($"{LanguageService.GetString("Удалить", "Delete", "Ștergere")} '{_selectedMeal.Name}'?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                DataService.DeleteMeal(_selectedMeal.Id);
                LoadMeals();
                ClearInputs();
            }
        }
    }
    
    private void TxtSearch_TextChanged(object sender, EventArgs e)
    {
        LoadFilteredMeals(txtSearch.Text);
    }
    
    private bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(txtName.Text)) return false;
        if (!int.TryParse(txtCalories.Text, out _) || !double.TryParse(txtProtein.Text, out _) || !double.TryParse(txtFat.Text, out _) || !double.TryParse(txtCarbs.Text, out _))
        {
            MessageBox.Show(LanguageService.GetString("Введите числовые значения!", "Enter numeric values!", "Introduceți valori numerice!"));
            return false;
        }
        return true;
    }
    
    private void ClearInputs()
    {
        txtName.Text = "";
        txtCalories.Text = "";
        txtProtein.Text = "";
        txtFat.Text = "";
        txtCarbs.Text = "";
        _selectedMeal = null;
    }
}