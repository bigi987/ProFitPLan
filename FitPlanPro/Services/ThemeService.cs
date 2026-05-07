namespace FitPlanPro.Services;

public static class ThemeService
{
    private static bool _isDarkMode = false;
    public static bool IsDarkMode => _isDarkMode;
    
    // Modern Color Palette
    public static Color PrimaryColor = Color.FromArgb(212, 255, 0); // Lime Green Accent
    
    // Light Theme Colors
    private static Color LightBg = Color.FromArgb(244, 247, 246);
    private static Color LightControlBg = Color.White;
    private static Color LightText = Color.FromArgb(51, 51, 51);
    
    // Dark Theme Colors
    private static Color DarkBg = Color.FromArgb(18, 18, 18);
    private static Color DarkControlBg = Color.FromArgb(28, 28, 28);
    private static Color DarkText = Color.White;

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
                // Is this a primary action button like Login or Register? We can color it primary!
                // We'll just color all buttons with PrimaryColor for a colorful look, 
                // or just standard styling and let specific forms override if needed.
                btn.BackColor = PrimaryColor;
                btn.ForeColor = Color.Black; // Dark text on lime green
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
                dgv.GridColor = _isDarkMode ? Color.FromArgb(40, 40, 40) : Color.LightGray;
                dgv.DefaultCellStyle.BackColor = backColor;
                dgv.DefaultCellStyle.ForeColor = foreColor;
                dgv.DefaultCellStyle.SelectionBackColor = PrimaryColor;
                dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
                dgv.ColumnHeadersDefaultCellStyle.BackColor = controlBg;
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.Gray;
                dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = controlBg;
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
            else
            {
                c.BackColor = backColor;
                c.ForeColor = foreColor;
            }

            if (c.HasChildren && !(c is DataGridView))
            {
                // For panels and groupboxes, they should probably have backColor or controlBg
                // We'll pass them down.
                ApplyThemeToControls(c.Controls, backColor, controlBg, foreColor);
            }
        }
    }
}