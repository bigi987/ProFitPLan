using FitPlanPro.Models;
using FitPlanPro.Services;
using FitPlanPro.Controls;
using System.Drawing.Drawing2D;

namespace FitPlanPro.Forms;

public class WorkoutForm : Form
{
    private SidebarControl sidebar = null!;
    private Label lblTitle = new();
    private TextBox txtSearch = new();
    private DataGridView dgvWorkouts = new();
    
    private Panel pnlEdit = new();
    private Label lblEditTitle = new();
    private TextBox txtName = new();
    private NumericUpDown numDuration = new();
    private NumericUpDown numCalories = new();
    private Button btnAdd = new();
    private Button btnUpdate = new();
    private Button btnDelete = new();
    
    private Label lblNameLabel = new();
    private Label lblDurationLabel = new();
    private Label lblCaloriesLabel = new();
    
    private Workout? _selectedWorkout;

    public WorkoutForm()
    {
        SetupForm();
        LoadWorkouts();
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

        sidebar = new SidebarControl(this, SidebarControl.ActivePage.Dashboard); // Not ideal, but Workout isn't in ActivePage enum yet
        this.Controls.Add(sidebar);

        int contentX = 140;

        lblTitle.Font = new Font("Segoe UI Black", 20F, FontStyle.Bold);
        lblTitle.Location = new Point(contentX, 30);
        lblTitle.AutoSize = true;

        txtSearch.Location = new Point(550, 35);
        txtSearch.Size = new Size(200, 30);
        txtSearch.Font = new Font("Segoe UI", 11F);
        txtSearch.TextChanged += TxtSearch_TextChanged!;

        dgvWorkouts.Location = new Point(contentX, 90);
        dgvWorkouts.Size = new Size(610, 530);
        dgvWorkouts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvWorkouts.MultiSelect = false;
        dgvWorkouts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvWorkouts.RowTemplate.Height = 45;
        dgvWorkouts.SelectionChanged += DgvWorkouts_SelectionChanged!;

        // Right Panel (Edit)
        int panelX = 780;
        pnlEdit.Location = new Point(panelX, 30);
        pnlEdit.Size = new Size(280, 400);

        lblEditTitle.Font = new Font("Segoe UI Black", 14F, FontStyle.Bold);
        lblEditTitle.Location = new Point(0, 0);
        lblEditTitle.AutoSize = true;

        int y = 40;
        CreateInput(pnlEdit, lblNameLabel, "НАЗВАНИЕ", txtName, ref y, 280);
        
        lblNameLabel.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
        lblNameLabel.ForeColor = Color.Gray;
        
        lblDurationLabel.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
        lblDurationLabel.ForeColor = Color.Gray;
        lblDurationLabel.Location = new Point(0, y);
        lblDurationLabel.AutoSize = true;
        y += 20;
        numDuration.Location = new Point(0, y);
        numDuration.Size = new Size(280, 35);
        numDuration.Font = new Font("Segoe UI", 12F);
        numDuration.BackColor = Color.FromArgb(40, 40, 40);
        numDuration.ForeColor = Color.White;
        numDuration.Maximum = 1000;
        pnlEdit.Controls.Add(lblDurationLabel);
        pnlEdit.Controls.Add(numDuration);
        y += 55;

        lblCaloriesLabel.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
        lblCaloriesLabel.ForeColor = Color.Gray;
        lblCaloriesLabel.Location = new Point(0, y);
        lblCaloriesLabel.AutoSize = true;
        y += 20;
        numCalories.Location = new Point(0, y);
        numCalories.Size = new Size(280, 35);
        numCalories.Font = new Font("Segoe UI", 12F);
        numCalories.BackColor = Color.FromArgb(40, 40, 40);
        numCalories.ForeColor = Color.White;
        numCalories.Maximum = 10000;
        pnlEdit.Controls.Add(lblCaloriesLabel);
        pnlEdit.Controls.Add(numCalories);
        y += 65;

        btnAdd.Location = new Point(0, y);
        btnAdd.Size = new Size(130, 45);
        btnAdd.BackColor = ThemeService.PrimaryColor;
        btnAdd.FlatStyle = FlatStyle.Flat;
        btnAdd.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnAdd.Click += BtnAdd_Click!;

        btnUpdate.Location = new Point(140, y);
        btnUpdate.Size = new Size(140, 45);
        btnUpdate.FlatStyle = FlatStyle.Flat;
        btnUpdate.BackColor = Color.FromArgb(28, 28, 28);
        btnUpdate.ForeColor = ThemeService.PrimaryColor;
        btnUpdate.Click += BtnUpdate_Click!;

        y += 60;

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
        this.Controls.Add(dgvWorkouts);
        this.Controls.Add(pnlEdit);
    }

    private void CreateInput(Control parent, Label lbl, string labelText, TextBox txt, ref int y, int w)
    {
        lbl.Text = labelText;
        lbl.Location = new Point(0, y);
        lbl.AutoSize = true;
        y += 20;
        txt.Location = new Point(0, y);
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
        this.Text = LanguageService.GetString("Управление тренировками", "Workout Management", "Gestionarea antrenamentelor");
        lblTitle.Text = LanguageService.GetString("УПРАВЛЕНИЕ ТРЕНИРОВКАМИ", "WORKOUT MANAGEMENT", "GESTIONAREA ANTRENAMENTELOR");
        lblEditTitle.Text = LanguageService.GetString("РЕДАКТИРОВАТЬ", "EDIT", "EDITARE");
        btnAdd.Text = "+ " + LanguageService.GetString("Добавить", "Add", "Adăugare");
        btnUpdate.Text = "🔄 " + LanguageService.GetString("Обновить", "Update", "Actualizare");
        btnDelete.Text = "🗑 " + LanguageService.GetString("Удалить запись", "Delete Record", "Ștergere");
        txtSearch.PlaceholderText = "🔍 " + LanguageService.GetString("Поиск...", "Search...", "Căutare...");
        
        lblNameLabel.Text = LanguageService.GetString("НАЗВАНИЕ", "NAME", "NUME");
        lblDurationLabel.Text = LanguageService.GetString("ДЛИТЕЛЬНОСТЬ (МИН)", "DURATION (MIN)", "DURATĂ (MIN)");
        lblCaloriesLabel.Text = LanguageService.GetString("СОЖЖЕНО (ККАЛ)", "BURNED (KCAL)", "ARSE (KCAL)");
        
        UpdateGridHeaders();
    }

    private void UpdateGridHeaders()
    {
        if (dgvWorkouts.Columns.Contains("Name"))
            dgvWorkouts.Columns["Name"].HeaderText = LanguageService.GetString("НАЗВАНИЕ", "NAME", "NUME");
        if (dgvWorkouts.Columns.Contains("DurationMinutes"))
            dgvWorkouts.Columns["DurationMinutes"].HeaderText = LanguageService.GetString("МИНУТЫ", "MINUTES", "MINUTE");
        if (dgvWorkouts.Columns.Contains("CaloriesBurned"))
            dgvWorkouts.Columns["CaloriesBurned"].HeaderText = LanguageService.GetString("КАЛОРИИ", "CALORIES", "CALORII");
        if (dgvWorkouts.Columns.Contains("Date"))
            dgvWorkouts.Columns["Date"].HeaderText = LanguageService.GetString("ДАТА", "DATE", "DATA");
    }

    private void LoadWorkouts()
    {
        dgvWorkouts.DataSource = null;
        dgvWorkouts.DataSource = DataService.Workouts.ToList();
        if (dgvWorkouts.Columns.Contains("Id")) dgvWorkouts.Columns["Id"].Visible = false;
        UpdateGridHeaders();
    }

    private void DgvWorkouts_SelectionChanged(object sender, EventArgs e)
    {
        if (dgvWorkouts.SelectedRows.Count > 0)
        {
            _selectedWorkout = (Workout)dgvWorkouts.SelectedRows[0].DataBoundItem;
            txtName.Text = _selectedWorkout.Name;
            numDuration.Value = _selectedWorkout.DurationMinutes;
            numCalories.Value = _selectedWorkout.CaloriesBurned;
        }
    }

    private void BtnAdd_Click(object sender, EventArgs e)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(txtName.Text))
            {
                var workout = new Workout
                {
                    Id = DataService.NextWorkoutId,
                    Name = txtName.Text,
                    DurationMinutes = (int)numDuration.Value,
                    CaloriesBurned = (int)numCalories.Value,
                    Date = DateTime.Now
                };
                DataService.AddWorkout(workout);
                LoadWorkouts();
                ClearInputs();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void BtnUpdate_Click(object sender, EventArgs e)
    {
        if (_selectedWorkout != null && !string.IsNullOrWhiteSpace(txtName.Text))
        {
            _selectedWorkout.Name = txtName.Text;
            _selectedWorkout.DurationMinutes = (int)numDuration.Value;
            _selectedWorkout.CaloriesBurned = (int)numCalories.Value;
            DataService.UpdateWorkout(_selectedWorkout.Id, _selectedWorkout);
            LoadWorkouts();
        }
    }

    private void BtnDelete_Click(object sender, EventArgs e)
    {
        if (_selectedWorkout != null)
        {
            DataService.DeleteWorkout(_selectedWorkout.Id);
            LoadWorkouts();
            ClearInputs();
        }
    }

    private void TxtSearch_TextChanged(object sender, EventArgs e)
    {
        var filtered = DataService.Workouts
            .Where(w => w.Name.ToLower().Contains(txtSearch.Text.ToLower()))
            .ToList();
        dgvWorkouts.DataSource = null;
        dgvWorkouts.DataSource = filtered;
        if (dgvWorkouts.Columns.Contains("Id")) dgvWorkouts.Columns["Id"].Visible = false;
        UpdateGridHeaders();
    }

    private void ClearInputs()
    {
        txtName.Text = "";
        numDuration.Value = 0;
        numCalories.Value = 0;
        _selectedWorkout = null;
    }
}