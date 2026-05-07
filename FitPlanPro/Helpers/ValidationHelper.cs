public static class ValidationHelper
{
    public static bool ValidateUsername(string username) => !string.IsNullOrWhiteSpace(username) && username.Length >= 3;
    public static bool ValidatePassword(string password) => !string.IsNullOrWhiteSpace(password) && password.Length >= 6;
}