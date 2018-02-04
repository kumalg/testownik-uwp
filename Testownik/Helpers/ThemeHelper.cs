using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace Testownik.Helpers {
    public class ThemeHelper {
        /// Extend acrylic into the title bar. 
        public static void ExtendAcrylicIntoTitleBar() {
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            if (SettingsHelper.AppTheme == ElementTheme.Dark)
                SetTitleBarDarkTheme(titleBar);
            else
                SetTitleBarLightTheme(titleBar);
        }

        private static void SetTitleBarDarkTheme(ApplicationViewTitleBar titleBar) {
            // Set active window colors
            titleBar.ForegroundColor = Colors.Gray;
            titleBar.ButtonForegroundColor = Colors.Gray;
            titleBar.ButtonHoverForegroundColor = Colors.White;
            titleBar.ButtonHoverBackgroundColor = Colors.Gray;
            titleBar.ButtonPressedForegroundColor = Colors.White;

            // Set inactive window colors
            titleBar.InactiveForegroundColor = Colors.Gray;
            titleBar.ButtonInactiveForegroundColor = Colors.Gray;
        }

        private static void SetTitleBarLightTheme(ApplicationViewTitleBar titleBar) {
            // Set active window colors
            titleBar.ForegroundColor = Colors.Black;
            titleBar.ButtonForegroundColor = Colors.Black;
            titleBar.ButtonHoverForegroundColor = Colors.Black;
            titleBar.ButtonHoverBackgroundColor = Colors.Gray;
            titleBar.ButtonPressedForegroundColor = Colors.Black;

            // Set inactive window colors
            titleBar.InactiveForegroundColor = Colors.Gray;
            titleBar.ButtonInactiveForegroundColor = Colors.Gray;
        }
    }
}
