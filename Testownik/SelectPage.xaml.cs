using System;
using System.Linq;
using Testownik.Model;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace Testownik
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class SelectPage : Page
    {
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                // Show UI in title bar if opted-in and in-app backstack is not empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
            }
            else
            {
                // Remove the UI from the title bar if in-app back stack is empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;
            }

            base.OnNavigatedTo(e);
        }

        public SelectPage()
        {
            this.InitializeComponent();
        }

        private async void SelectFolder()
        {
            ProgressGrid.Visibility = Visibility.Visible;
            var co = await QuestionsReader.ReadQuestions();
            if (co == null || !co.Any())
            {
                var dialog = new ContentDialog()
                {
                    Content = "Nie wybrano pliku bądź baza jest pusta",
                    PrimaryButtonText = "Wyjdź z aplikacji",
                    SecondaryButtonText = "Powtórz"
                };
                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Secondary)
                    SelectFolder();
                else
                    CoreApplication.Exit();
            }
            else
            {
                var testController = new TestController(co);
                ProgressGrid.Visibility = Visibility.Collapsed;
                this.Frame.Navigate(typeof(MainPage), testController);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SelectFolder();
        }
    }
}
