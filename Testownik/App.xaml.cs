using System;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Testownik.Helpers;
using Testownik.Model;
using Testownik.Models;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Testownik {
    /// <summary>
    /// Zapewnia zachowanie specyficzne dla aplikacji, aby uzupełnić domyślną klasę aplikacji.
    /// </summary>
    sealed partial class App : Application {
        /// <summary>
        /// Inicjuje pojedynczy obiekt aplikacji. Jest to pierwszy wiersz napisanego kodu
        /// wykonywanego i jest logicznym odpowiednikiem metod main() lub WinMain().
        /// </summary>
        public App() {
            this.InitializeComponent();
            AppCenter.Start("07b9dcb3-38f6-4413-9587-961e01b296df", typeof(Analytics));
            RequestedTheme = SettingsHelper.AppTheme.ToApplicationTheme();
        }

        /// Extend acrylic into the title bar. 
        private void ExtendAcrylicIntoTitleBar () {
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            // Set active window colors
            titleBar.ForegroundColor = Colors.Gray;
            //titleBar.BackgroundColor = Windows.UI.Colors.Green;
            titleBar.ButtonForegroundColor = Colors.White;
            //titleBar.ButtonBackgroundColor = Windows.UI.Colors.SeaGreen;
            titleBar.ButtonHoverForegroundColor = Colors.White;
            //titleBar.ButtonHoverBackgroundColor = Windows.UI.Colors.DarkSeaGreen;
            titleBar.ButtonPressedForegroundColor = Colors.Gray;
            //titleBar.ButtonPressedBackgroundColor = Windows.UI.Colors.LightGreen;

            // Set inactive window colors
            titleBar.InactiveForegroundColor = Colors.Gray;
            //titleBar.InactiveBackgroundColor = Windows.UI.Colors.SeaGreen;
            titleBar.ButtonInactiveForegroundColor = Colors.Gray;
            //titleBar.ButtonInactiveBackgroundColor = Windows.UI.Colors.SeaGreen;
        }

        private void App_BackRequested (object sender,
            Windows.UI.Core.BackRequestedEventArgs e) {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
                return;

            // Navigate back if possible, and if the event has not 
            // already been handled .
            if (rootFrame.CanGoBack && e.Handled == false) {
                e.Handled = true;
                rootFrame.GoBack ();
            }
        }

        /// <summary>
        /// Wywoływane, gdy aplikacja jest uruchamiana normalnie przez użytkownika końcowego. Inne punkty wejścia
        /// będą używane, kiedy aplikacja zostanie uruchomiona w celu otworzenia określonego pliku.
        /// </summary>
        /// <param name="e">Szczegóły dotyczące żądania uruchomienia i procesu.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e) {
            Frame rootFrame = Window.Current.Content as Frame;

            // Nie powtarzaj inicjowania aplikacji, gdy w oknie znajduje się już zawartość,
            // upewnij się tylko, że okno jest aktywne
            if (rootFrame == null) {
                // Utwórz ramkę, która będzie pełnić funkcję kontekstu nawigacji, i przejdź do pierwszej strony
                rootFrame = new ThemeAwareFrame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated) {
                    //TODO: Załaduj stan z wstrzymanej wcześniej aplikacji
                }

                // Umieść ramkę w bieżącym oknie
                Window.Current.Content = rootFrame;
                Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;
            }

            if (e.PrelaunchActivated == false) {
                if (rootFrame.Content == null) {
                    // Kiedy stos nawigacji nie jest przywrócony, przejdź do pierwszej strony,
                    // konfigurując nową stronę przez przekazanie wymaganych informacji jako
                    // parametr
                    rootFrame.Navigate(typeof(SelectPage), e.Arguments);
                }
                // Upewnij się, ze bieżące okno jest aktywne
                Window.Current.Activate();

                // Extend acrylic
                ExtendAcrylicIntoTitleBar();
            }
        }
       
        /// <summary>
        /// Wywoływane, gdy nawigacja do konkretnej strony nie powiedzie się
        /// </summary>
        /// <param name="sender">Ramka, do której nawigacja nie powiodła się</param>
        /// <param name="e">Szczegóły dotyczące niepowodzenia nawigacji</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e) {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Wywoływane, gdy wykonanie aplikacji jest wstrzymywane. Stan aplikacji jest zapisywany
        /// bez wiedzy o tym, czy aplikacja zostanie zakończona, czy wznowiona z niezmienioną zawartością
        /// pamięci.
        /// </summary>
        /// <param name="sender">Źródło żądania wstrzymania.</param>
        /// <param name="e">Szczegóły żądania wstrzymania.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e) {
            var deferral = e.SuspendingOperation.GetDeferral ();
            //TODO: Zapisz stan aplikacji i zatrzymaj wszelkie aktywności w tle
            deferral.Complete();
        }
    }
}