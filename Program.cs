using FitPlanPro.Models;
using FitPlanPro.Services;
using FitPlanPro.Forms;
using MySqlConnector;

namespace FitPlanPro;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        
        DataService.InitializeDatabase();
        DataService.LoadData();
        
        Application.Run(new LoginForm());
    }
}