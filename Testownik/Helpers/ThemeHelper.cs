using Windows.UI.Xaml;

namespace Testownik.Helpers {
    public static class ThemeHelper {
        public static ApplicationTheme ToApplicationTheme(this ElementTheme elementTheme) {
            switch (elementTheme) {
                case ElementTheme.Dark:
                    return ApplicationTheme.Dark;
                default:
                    return ApplicationTheme.Light;
            }
        }
    }
}
