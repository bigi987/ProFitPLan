using System.Text;

namespace FitPlanPro.Services;

public static class ExportService
{
    public static void ExportToCsv<T>(List<T> data, string filePath)
    {
        try
        {
            var properties = typeof(T).GetProperties();
            var lines = new List<string>();
            lines.Add(string.Join(",", properties.Select(p => p.Name)));
            foreach (var item in data)
            {
                var values = properties.Select(p => p.GetValue(item)?.ToString() ?? "");
                lines.Add(string.Join(",", values));
            }
            File.WriteAllLines(filePath, lines, Encoding.UTF8);
            System.Windows.Forms.MessageBox.Show($"Экспорт завершён: {filePath}", "Успех", 
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show($"Ошибка экспорта: {ex.Message}", "Ошибка", 
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
        }
    }
}
