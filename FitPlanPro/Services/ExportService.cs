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
            
            // Header
            lines.Add(string.Join(",", properties.Select(p => p.Name)));
            
            // Data
            foreach (var item in data)
            {
                var values = properties.Select(p => p.GetValue(item)?.ToString() ?? "");
                lines.Add(string.Join(",", values));
            }
            
            File.WriteAllLines(filePath, lines, Encoding.UTF8);
            MessageBox.Show($"Экспорт завершён: {filePath}", "Успех", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка экспорта: {ex.Message}", "Ошибка", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}