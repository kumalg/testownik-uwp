using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Testownik
{
    /// <summary>
    /// Zapewnia zachowanie specyficzne dla aplikacji, aby uzupełnić domyślną klasę aplikacji.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Inicjuje pojedynczy obiekt aplikacji. Jest to pierwszy wiersz napisanego kodu
        /// wykonywanego i jest logicznym odpowiednikiem metod main() lub WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// Extend acrylic into the title bar. 
        private void extendAcrylicIntoTitleBar() {
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            // Set active window colors
            titleBar.ForegroundColor = Windows.UI.Colors.White;
            //titleBar.BackgroundColor = Windows.UI.Colors.Green;
            titleBar.ButtonForegroundColor = Windows.UI.Colors.White;
            //titleBar.ButtonBackgroundColor = Windows.UI.Colors.SeaGreen;
            titleBar.ButtonHoverForegroundColor = Windows.UI.Colors.White;
            //titleBar.ButtonHoverBackgroundColor = Windows.UI.Colors.DarkSeaGreen;
            titleBar.ButtonPressedForegroundColor = Windows.UI.Colors.Gray;
            //titleBar.ButtonPressedBackgroundColor = Windows.UI.Colors.LightGreen;

            // Set inactive window colors
            titleBar.InactiveForegroundColor = Windows.UI.Colors.Gray;
            //titleBar.InactiveBackgroundColor = Windows.UI.Colors.SeaGreen;
            titleBar.ButtonInactiveForegroundColor = Windows.UI.Colors.Gray;
            //titleBar.ButtonInactiveBackgroundColor = Windows.UI.Colors.SeaGreen;
        }

        /// <summary>
        /// Wywoływane, gdy aplikacja jest uruchamiana normalnie przez użytkownika końcowego. Inne punkty wejścia
        /// będą używane, kiedy aplikacja zostanie uruchomiona w celu otworzenia określonego pliku.
        /// </summary>
        /// <param name="e">Szczegóły dotyczące żądania uruchomienia i procesu.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Nie powtarzaj inicjowania aplikacji, gdy w oknie znajduje się już zawartość,
            // upewnij się tylko, że okno jest aktywne
            if (rootFrame == null)
            {
                // Utwórz ramkę, która będzie pełnić funkcję kontekstu nawigacji, i przejdź do pierwszej strony
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Załaduj stan z wstrzymanej wcześniej aplikacji
                }

                // Umieść ramkę w bieżącym oknie
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // Kiedy stos nawigacji nie jest przywrócony, przejdź do pierwszej strony,
                    // konfigurując nową stronę przez przekazanie wymaganych informacji jako
                    // parametr
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Upewnij się, ze bieżące okno jest aktywne
                Window.Current.Activate();       
                
                // Extend acrylic
                extendAcrylicIntoTitleBar();
            }
        }

        /// <summary>
        /// Wywoływane, gdy nawigacja do konkretnej strony nie powiedzie się
        /// </summary>
        /// <param name="sender">Ramka, do której nawigacja nie powiodła się</param>
        /// <param name="e">Szczegóły dotyczące niepowodzenia nawigacji</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Wywoływane, gdy wykonanie aplikacji jest wstrzymywane. Stan aplikacji jest zapisywany
        /// bez wiedzy o tym, czy aplikacja zostanie zakończona, czy wznowiona z niezmienioną zawartością
        /// pamięci.
        /// </summary>
        /// <param name="sender">Źródło żądania wstrzymania.</param>
        /// <param name="e">Szczegóły żądania wstrzymania.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Zapisz stan aplikacji i zatrzymaj wszelkie aktywności w tle
            deferral.Complete();
        }
    }
}
