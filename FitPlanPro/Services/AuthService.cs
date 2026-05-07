using FitPlanPro.Models;

namespace FitPlanPro.Services;

public static class AuthService
{
    private static User? _currentUser;
    public static User? CurrentUser => _currentUser;
    
    public static bool Login(string username, string password)
    {
        var user = DataService.Users.FirstOrDefault(u => 
            u.Username == username && u.PasswordHash == password);
        
        if (user != null)
        {
            _currentUser = user;
            return true;
        }
        return false;
    }
    
    public static void Logout() => _currentUser = null;
    
    public static bool Register(string username, string password, string role = "User")
    {
        if (DataService.Users.Any(u => u.Username == username))
            return false;
        
        DataService.Users.Add(new User
        {
            Id = DataService.NextUserId,
            Username = username,
            PasswordHash = password,
            Role = role
        });
        DataService.SaveData();
        return true;
    }
}