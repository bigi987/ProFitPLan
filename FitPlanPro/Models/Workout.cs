namespace FitPlanPro.Models;

public class Workout
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = "";
    public int DurationMinutes { get; set; }
    public int CaloriesBurned { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
}