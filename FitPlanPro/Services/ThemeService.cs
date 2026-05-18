namespace FitPlanPro.Services;

public static class ThemeService
{
    private static bool _isDarkMode = false;
    public static bool IsDarkMode => _isDarkMode;
    
    public static Color PrimaryColor = Color.FromArgb(120, 180, 0); 
    
    private static Color LightBg = Color.FromArgb(242, 245, 249);
    private static Color LightControlBg = Color.White;
    private static Color LightText = Color.FromArgb(20, 20, 20);
    
    private static Color DarkBg = Color.FromArgb(18, 18, 18);
    private static Color DarkControlBg = Color.FromArgb(28, 28, 28);
    private static Color DarkText = Color.White;

    public static Color SurfaceColor => _isDarkMode ? Color.FromArgb(32, 32, 32) : Color.White;
    public static Color BorderColor => _isDarkMode ? Color.FromArgb(45, 45, 45) : Color.FromArgb(230, 230, 230);
    public static Color SecondaryText => _isDarkMode ? Color.FromArgb(160, 160, 160) : Color.FromArgb(100, 100, 100);

    public static void ToggleTheme(Form form)
    {
        _isDarkMode = !_isDarkMode;
        ApplyTheme(form);
    }
    
    public static void ApplyTheme(Form form)
    {
        Color backColor = _isDarkMode ? DarkBg : LightBg;
        Color foreColor = _isDarkMode ? DarkText : LightText;
        Color controlBg = _isDarkMode ? DarkControlBg : LightControlBg;
        
        form.BackColor = backColor;
        form.ForeColor = foreColor;
        form.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        
        ApplyThemeToControls(form.Controls, backColor, controlBg, foreColor);
    }
    
    private static void ApplyThemeToControls(Control.ControlCollection controls, Color backColor, Color controlBg, Color foreColor)
    {
        foreach (Control c in controls)
        {
            if (c is Button btn)
            {
                btn.BackColor = PrimaryColor;
                btn.ForeColor = Color.Black;
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
                btn.Cursor = Cursors.Hand;
            }
            else if (c is TextBox txt)
            {
                txt.BackColor = controlBg;
                txt.ForeColor = foreColor;
                txt.BorderStyle = BorderStyle.FixedSingle;
            }
            else if (c is DataGridView dgv)
            {
                dgv.BackgroundColor = backColor;
                dgv.ForeColor = foreColor;
                dgv.GridColor = BorderColor;
                dgv.DefaultCellStyle.BackColor = backColor;
                dgv.DefaultCellStyle.ForeColor = foreColor;
                dgv.DefaultCellStyle.SelectionBackColor = PrimaryColor;
                dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
                dgv.ColumnHeadersDefaultCellStyle.BackColor = controlBg;
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = SecondaryText;
                dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
                dgv.RowHeadersVisible = false;
                dgv.EnableHeadersVisualStyles = false;
                dgv.BorderStyle = BorderStyle.None;
                dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            }
            else if (c is ListBox lb)
            {
                lb.BackColor = controlBg;
                lb.ForeColor = foreColor;
                lb.BorderStyle = BorderStyle.FixedSingle;
            }
            else if (c is ComboBox cb)
            {
                cb.BackColor = controlBg;
                cb.ForeColor = foreColor;
                cb.FlatStyle = FlatStyle.Flat;
            }
            else if (c is NumericUpDown num)
            {
                num.BackColor = controlBg;
                num.ForeColor = foreColor;
            }
            else
            {
                c.BackColor = backColor;
                c.ForeColor = foreColor;
            }

            if (c.HasChildren && !(c is DataGridView))
            {
                ApplyThemeToControls(c.Controls, backColor, controlBg, foreColor);
            }
        }
    }
}
