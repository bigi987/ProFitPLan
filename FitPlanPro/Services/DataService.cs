using FitPlanPro.Models;
using MySqlConnector;
using Dapper;
using System.Data;
using System.Windows.Forms;


namespace FitPlanPro.Services;

public static class DataService
{
    private static readonly string ConnectionString = "Server=127.0.0.1;Port=3306;Database=fitplanpro_db;User ID=root;Password=Basarabeasca1029;";

    public static List<User> Users = new();
    public static List<Meal> Meals = new();
    public static List<Workout> Workouts = new();

    public static void InitializeDatabase()
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        try
        {
            string baseConnString = "Server=127.0.0.1;Port=3306;User ID=root;Password=Basarabeasca1029;";
            Console.WriteLine($"[DB] Попытка подключения к: {baseConnString.Replace("Basarabeasca1029", "********")}");
            using (var conn = new MySqlConnection(baseConnString))
            {
                conn.Open();
                conn.Execute("CREATE DATABASE IF NOT EXISTS fitplanpro_db;");
            }

            using var dbConn = new MySqlConnection(ConnectionString);
            dbConn.Open();

            string schema = @"
                CREATE TABLE IF NOT EXISTS users (
                    id INT PRIMARY KEY AUTO_INCREMENT,
                    username VARCHAR(255) NOT NULL UNIQUE,
                    password_hash TEXT NOT NULL,
                    role VARCHAR(50) DEFAULT 'User'
                );

                CREATE TABLE IF NOT EXISTS meals (
                    id INT PRIMARY KEY AUTO_INCREMENT,
                    user_id INT,
                    name VARCHAR(255) NOT NULL,
                    calories INT NOT NULL,
                    protein DOUBLE DEFAULT 0,
                    fat DOUBLE DEFAULT 0,
                    carbs DOUBLE DEFAULT 0,
                    date VARCHAR(50),
                    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
                );

                CREATE TABLE IF NOT EXISTS workouts (
                    id INT PRIMARY KEY AUTO_INCREMENT,
                    user_id INT,
                    name VARCHAR(255) NOT NULL,
                    duration_minutes INT NOT NULL,
                    calories_burned INT NOT NULL,
                    date VARCHAR(50),
                    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
                );

                INSERT INTO users (id, username, password_hash, role) 
                VALUES (1, 'admin', 'admin', 'Admin')
                ON DUPLICATE KEY UPDATE password_hash = 'admin';
            ";
            dbConn.Execute(schema);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка БД: {ex.Message}\n\nЕсли вы видите 'Access denied', значит у вашего MySQL ЕСТЬ пароль. Вспомните его и впишите в DataService.cs в поле Pwd=.", "Ошибка подключения", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public static void LoadData()
    {
        try
        {
            using var conn = new MySqlConnection(ConnectionString);
            Users = conn.Query<User>("SELECT * FROM users").ToList();
            Console.WriteLine($"[DB] Загружено пользователей: {Users.Count}");
            foreach(var u in Users) Console.WriteLine($" - Пользователь: {u.Username}, Пароль (длина): {u.PasswordHash?.Length}, Роль: {u.Role}");
            
            if (AuthService.CurrentUser != null)
            {
                RefreshUserData();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading data: {ex.Message}");
        }
    }

    public static void RefreshUserData()
    {
        if (AuthService.CurrentUser == null) return;
        
        try
        {
            using var conn = new MySqlConnection(ConnectionString);
            int userId = AuthService.CurrentUser.Id;
            
            Meals = conn.Query<Meal>("SELECT * FROM meals WHERE user_id = @userId", new { userId }).ToList();
            Workouts = conn.Query<Workout>("SELECT * FROM workouts WHERE user_id = @userId", new { userId }).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error refreshing user data: {ex.Message}");
        }
    }

    public static void AddUser(User user)
    {
        using var conn = new MySqlConnection(ConnectionString);
        string sql = "INSERT INTO users (username, password_hash, role) VALUES (@Username, @PasswordHash, @Role); SELECT LAST_INSERT_ID();";
        user.Id = conn.ExecuteScalar<int>(sql, user);
        Users.Add(user);
    }

    public static void UpdateUser(User user)
    {
        using var conn = new MySqlConnection(ConnectionString);
        string sql = "UPDATE users SET username=@Username, password_hash=@PasswordHash, role=@Role WHERE id=@Id";
        conn.Execute(sql, user);
        LoadData();
    }

    public static void DeleteUser(int id)
    {
        using var conn = new MySqlConnection(ConnectionString);
        conn.Execute("DELETE FROM users WHERE id = @id", new { id });
        LoadData();
    }

    public static void AddMeal(Meal meal)
    {
        if (AuthService.CurrentUser == null) return;
        meal.UserId = AuthService.CurrentUser.Id;
        
        using var conn = new MySqlConnection(ConnectionString);
        string sql = "INSERT INTO meals (user_id, name, calories, protein, fat, carbs, date) VALUES (@UserId, @Name, @Calories, @Protein, @Fat, @Carbs, @Date); SELECT LAST_INSERT_ID();";
        meal.Id = conn.ExecuteScalar<int>(sql, meal);
        Meals.Add(meal);
    }

    public static void UpdateMeal(int id, Meal updated)
    {
        using var conn = new MySqlConnection(ConnectionString);
        string sql = "UPDATE meals SET name=@Name, calories=@Calories, protein=@Protein, fat=@Fat, carbs=@Carbs, date=@Date WHERE id=@Id AND user_id=@UserId";
        conn.Execute(sql, updated);
        RefreshUserData();
    }

    public static void DeleteMeal(int id)
    {
        if (AuthService.CurrentUser == null) return;
        using var conn = new MySqlConnection(ConnectionString);
        conn.Execute("DELETE FROM meals WHERE id = @id AND user_id = @userId", new { id, userId = AuthService.CurrentUser.Id });
        RefreshUserData();
    }

    public static void AddWorkout(Workout workout)
    {
        if (AuthService.CurrentUser == null) return;
        workout.UserId = AuthService.CurrentUser.Id;
        
        using var conn = new MySqlConnection(ConnectionString);
        string sql = "INSERT INTO workouts (user_id, name, duration_minutes, calories_burned, date) VALUES (@UserId, @Name, @DurationMinutes, @CaloriesBurned, @Date); SELECT LAST_INSERT_ID();";
        workout.Id = conn.ExecuteScalar<int>(sql, workout);
        Workouts.Add(workout);
    }

    public static void UpdateWorkout(int id, Workout updated)
    {
        using var conn = new MySqlConnection(ConnectionString);
        string sql = "UPDATE workouts SET name=@Name, duration_minutes=@DurationMinutes, calories_burned=@CaloriesBurned, date=@Date WHERE id=@Id AND user_id=@UserId";
        conn.Execute(sql, updated);
        RefreshUserData();
    }

    public static void DeleteWorkout(int id)
    {
        if (AuthService.CurrentUser == null) return;
        using var conn = new MySqlConnection(ConnectionString);
        conn.Execute("DELETE FROM workouts WHERE id = @id AND user_id = @userId", new { id, userId = AuthService.CurrentUser.Id });
        RefreshUserData();
    }
}
