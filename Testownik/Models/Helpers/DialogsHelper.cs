using System;
using Windows.UI.Xaml.Controls;

namespace Testownik.Model.Helpers {
    public class DialogsHelper {
        public static async void ShowBasicMessageDialog(string message) {
            var dialog = new ContentDialog() {
                Title = message,
                PrimaryButtonText = "Zamknij"
            };
            await dialog.ShowAsync();
        }
    }
}
