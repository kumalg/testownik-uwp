using Windows.UI.Xaml;

namespace Testownik.Extensions {
    public static class ApplicationThemeExtensions {
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
