using FitPlanPro.Forms;
using FitPlanPro.Models;
using FitPlanPro.Services;

namespace FitPlanPro;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        
        DataService.LoadData();
        
        if (DataService.Users.Count == 0)
        {
            DataService.Users.Add(new User 
            { 
                Id = 1, 
                Username = "admin", 
                PasswordHash = "admin", 
                Role = "Admin" 
            });
            DataService.SaveData();
        }
        
        Application.Run(new LoginForm());
    }
}