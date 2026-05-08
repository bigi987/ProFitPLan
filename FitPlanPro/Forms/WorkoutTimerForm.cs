using FitPlanPro.Models;
using FitPlanPro.Services;

namespace FitPlanPro.Forms;

public class WorkoutTimerForm : Form
{
    private Label lblTime = new();
    private Label lblTitle = new();
    private Button btnStartPause = new();
    private Button btnReset = new();
    private Button btnLap = new();
    private ListBox lbLaps = new();
    private Label lblLapsTitle = new();
    private System.Windows.Forms.Timer timer = new();
    
    private TimeSpan _elapsed = TimeSpan.Zero;
    private bool _isRunning = false;
    private int _lapCount = 0;
    private DateTime _startTime;
    private TimeSpan _previousElapsed = TimeSpan.Zero;
    
    public WorkoutTimerForm()
    {
        SetupForm();
        ThemeService.ApplyTheme(this);
        // Override the time label to always be bright
        lblTime.ForeColor = ThemeService.PrimaryColor;
    }

    private void SetupForm()
    {
        this.Text = LanguageService.GetString("⏱️ Таймер тренировки", "⏱️ Workout Timer", "⏱️ Cronometru antrenament");
        this.Size = new Size(550, 600);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.DoubleBuffered = true;

        lblTitle.Text = LanguageService.GetString("⏱️ Таймер тренировки", "⏱️ Workout Timer", "⏱️ Cronometru antrenament");
        lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
        lblTitle.AutoSize = true;
        lblTitle.Location = new Point(50, 20);

        lblTime.Text = "00:00:00";
        lblTime.Font = new Font("Consolas", 44F, FontStyle.Bold);
        lblTime.Location = new Point(20, 70);
        lblTime.Size = new Size(500, 80);
        lblTime.TextAlign = ContentAlignment.MiddleCenter;

        int btnW = 145;
        int btnH = 50;

        btnStartPause.Text = LanguageService.GetString("▶ Старт", "▶ Start", "▶ Start");
        btnStartPause.Location = new Point(40, 170);
        btnStartPause.Size = new Size(btnW, btnH);
        btnStartPause.Click += BtnStartPause_Click!;

        btnLap.Text = LanguageService.GetString("🏁 Круг", "🏁 Lap", "🏁 Tur");
        btnLap.Location = new Point(200, 170);
        btnLap.Size = new Size(btnW, btnH);
        btnLap.Enabled = false;
        btnLap.Click += BtnLap_Click!;

        btnReset.Text = LanguageService.GetString("🔄 Сброс", "🔄 Reset", "🔄 Resetare");
        btnReset.Location = new Point(360, 170);
        btnReset.Size = new Size(btnW, btnH);
        btnReset.Click += BtnReset_Click!;

        lblLapsTitle.Text = LanguageService.GetString("📋 Круги:", "📋 Laps:", "📋 Tururi:");
        lblLapsTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        lblLapsTitle.Location = new Point(50, 240);
        lblLapsTitle.AutoSize = true;

        lbLaps.Location = new Point(40, 270);
        lbLaps.Size = new Size(460, 260);
        lbLaps.Font = new Font("Consolas", 11F);

        timer.Interval = 50; // Update every 50ms for smooth display
        timer.Tick += Timer_Tick!;

        this.Controls.Add(lblTitle);
        this.Controls.Add(lblTime);
        this.Controls.Add(btnStartPause);
        this.Controls.Add(btnLap);
        this.Controls.Add(btnReset);
        this.Controls.Add(lblLapsTitle);
        this.Controls.Add(lbLaps);
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        _elapsed = _previousElapsed + (DateTime.Now - _startTime);
        lblTime.Text = _elapsed.ToString(@"hh\:mm\:ss");
    }

    private void BtnStartPause_Click(object sender, EventArgs e)
    {
        if (_isRunning)
        {
            // Pause
            timer.Stop();
            _previousElapsed = _elapsed;
            _isRunning = false;
            btnStartPause.Text = LanguageService.GetString("▶ Старт", "▶ Start", "▶ Start");
            btnStartPause.BackColor = Color.FromArgb(46, 204, 113); // Green
            btnLap.Enabled = false;
        }
        else
        {
            // Start
            _startTime = DateTime.Now;
            timer.Start();
            _isRunning = true;
            btnStartPause.Text = LanguageService.GetString("⏸ Пауза", "⏸ Pause", "⏸ Pauză");
            btnStartPause.BackColor = Color.FromArgb(231, 76, 60); // Red
            btnLap.Enabled = true;
        }
    }

    private void BtnLap_Click(object sender, EventArgs e)
    {
        _lapCount++;
        string lapTime = _elapsed.ToString(@"hh\:mm\:ss");
        string lapText = $"{LanguageService.GetString("Круг", "Lap", "Tur")} {_lapCount}:  {lapTime}";
        lbLaps.Items.Insert(0, lapText);
    }

    private void BtnReset_Click(object sender, EventArgs e)
    {
        timer.Stop();
        _isRunning = false;
        _elapsed = TimeSpan.Zero;
        _previousElapsed = TimeSpan.Zero;
        _lapCount = 0;
        lblTime.Text = "00:00:00";
        btnStartPause.Text = LanguageService.GetString("▶ Старт", "▶ Start", "▶ Start");
        btnStartPause.BackColor = ThemeService.PrimaryColor;
        btnLap.Enabled = false;
        lbLaps.Items.Clear();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        timer.Stop();
        timer.Dispose();
        base.OnFormClosing(e);
    }
}
