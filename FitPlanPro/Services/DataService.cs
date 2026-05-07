using FitPlanPro.Models;
using System.Text.Json;
using System.IO;

namespace FitPlanPro.Services;

public static class DataService
{
    private const string DataFilePath = "data.json";

    public static List<User> Users = new();
    public static List<Meal> Meals = new();
    public static List<Workout> Workouts = new();
    private static int _nextUserId = 2;
    private static int _nextMealId = 1;
    private static int _nextWorkoutId = 1;
    
    public static int NextUserId => _nextUserId++;
    public static int NextMealId => _nextMealId++;
    public static int NextWorkoutId => _nextWorkoutId++;

    private class AppData
    {
        public List<User> Users { get; set; } = new();
        public List<Meal> Meals { get; set; } = new();
        public List<Workout> Workouts { get; set; } = new();
        public int NextUserId { get; set; } = 2;
        public int NextMealId { get; set; } = 1;
        public int NextWorkoutId { get; set; } = 1;
    }

    public static void SaveData()
    {
        var data = new AppData
        {
            Users = Users,
            Meals = Meals,
            Workouts = Workouts,
            NextUserId = _nextUserId,
            NextMealId = _nextMealId,
            NextWorkoutId = _nextWorkoutId
        };
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(DataFilePath, json);
    }

    public static void LoadData()
    {
        if (File.Exists(DataFilePath))
        {
            var json = File.ReadAllText(DataFilePath);
            try
            {
                var data = JsonSerializer.Deserialize<AppData>(json);
                if (data != null)
                {
                    Users = data.Users ?? new List<User>();
                    Meals = data.Meals ?? new List<Meal>();
                    Workouts = data.Workouts ?? new List<Workout>();
                    _nextUserId = data.NextUserId > 0 ? data.NextUserId : 2;
                    _nextMealId = data.NextMealId > 0 ? data.NextMealId : 1;
                    _nextWorkoutId = data.NextWorkoutId > 0 ? data.NextWorkoutId : 1;
                }
            }
            catch
            {
                // Если файл поврежден, просто продолжаем с пустыми данными
            }
        }
    }
    
    // Meal CRUD
    public static void AddMeal(Meal meal) { Meals.Add(meal); SaveData(); }
    public static void UpdateMeal(int id, Meal updated)
    {
        var index = Meals.FindIndex(m => m.Id == id);
        if (index != -1) { Meals[index] = updated; SaveData(); }
    }
    public static void DeleteMeal(int id) { Meals.RemoveAll(m => m.Id == id); SaveData(); }
    
    // Workout CRUD
    public static void AddWorkout(Workout workout) { Workouts.Add(workout); SaveData(); }
    public static void UpdateWorkout(int id, Workout updated)
    {
        var index = Workouts.FindIndex(w => w.Id == id);
        if (index != -1) { Workouts[index] = updated; SaveData(); }
    }
    public static void DeleteWorkout(int id) { Workouts.RemoveAll(w => w.Id == id); SaveData(); }
}