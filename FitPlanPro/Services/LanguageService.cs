namespace FitPlanPro.Services;

public static class LanguageService
{
    private static string _currentLanguage = "ru"; 
    
    public static string CurrentLanguage => _currentLanguage;
    
    public static event Action? LanguageChanged;
    
    public static void SetLanguage(string languageCode)
    {
        if (languageCode == "ru" || languageCode == "en" || languageCode == "ro")
        {
            _currentLanguage = languageCode;
            LanguageChanged?.Invoke();
        }
    }
    
    public static void ToggleLanguage()
    {
        string next = _currentLanguage switch
        {
            "ru" => "en",
            "en" => "ro",
            "ro" => "ru",
            _ => "ru"
        };
        SetLanguage(next);
    }
    
    public static string GetString(string ruText, string enText, string roText = "")
    {
        return _currentLanguage switch
        {
            "ru" => ruText,
            "en" => enText,
            "ro" => string.IsNullOrEmpty(roText) ? enText : roText,
            _ => enText
        };
    }

    public static string GetNextLanguageLabel()
    {
        return _currentLanguage switch
        {
            "ru" => "🌐  English",
            "en" => "🌐  Română",
            "ro" => "🌐  Русский",
            _ => "🌐  English"
        };
    }
}
