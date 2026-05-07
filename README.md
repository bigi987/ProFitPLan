# FitPlan Pro

FitPlan Pro is a Windows Forms fitness planning application built with .NET 8.0. It provides a local desktop experience for managing workout plans, meal records, user accounts, and tracking fitness progress.

## Key Features

- Login and registration system
- Default admin account creation on first run
- Dashboard with fitness statistics and calorie progress
- Meal management (add, update, delete meals)
- Workout management (add, update, delete workouts)
- BMI calculator and workout timer tools
- User management available for admin users
- Theme toggle (light/dark mode)
- Multi-language support: Russian, English, Romanian
- Local data storage in `data.json`

## Default Credentials

The application creates a default admin user automatically when `data.json` does not exist or when no users are present:

- Username: `admin`
- Password: `admin`
- Role: `Admin`

## Project Structure

- `Program.cs` - application startup and data initialization
- `FitPLanPro.csproj` - .NET WinForms project file
- `FitPlanPro/Forms/` - UI forms for login, dashboard, registration, meals, workouts, timer, BMI, and user management
- `FitPlanPro/Services/` - authentication, data load/save, language, theme services
- `FitPlanPro/Models/` - domain models for users, meals, workouts
- `FitPlanPro/Controls/` - custom UI controls and progress components
- `data.json` - local app data storage file generated at runtime

## Requirements

- Windows OS
- .NET 8.0 SDK

## Build and Run

1. Open a terminal in the project folder.
2. Restore and build the project:
   ```powershell
   dotnet build
   ```
3. Run the application:
   ```powershell
   dotnet run --project FitPLanPro.csproj
   ```

## Notes

- Passwords are stored as plain text in `data.json` in the current implementation.
- The application uses simple JSON persistence, so the `data.json` file should stay in the same folder as the executable.
- Admin users can manage other users through the dashboard.

## License

You can add a license section here if you want to specify usage and distribution terms.
