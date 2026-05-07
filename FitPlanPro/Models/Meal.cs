namespace FitPlanPro.Models;

public class Meal
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int Calories { get; set; }
    public double Protein { get; set; }
    public double Fat { get; set; }
    public double Carbs { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
}