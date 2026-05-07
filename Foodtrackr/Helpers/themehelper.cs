namespace Foodtrackr.Helpers
{
    public static class ThemeHelper
    {
        public static void SetTheme(bool isDark)
        {
            Application.Current!.UserAppTheme = isDark
                ? AppTheme.Dark
                : AppTheme.Light;
        }

        public static bool IsDarkMode()
        {
            return Application.Current!.UserAppTheme == AppTheme.Dark;
        }
    }
}