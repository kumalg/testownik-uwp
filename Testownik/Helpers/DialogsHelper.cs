using System;
using Testownik.Dialogs;

namespace Testownik.Helpers {
    public class DialogsHelper {
        public static async void ShowBasicMessageDialog(string message) {
            var dialog = new MessageDialog {
                Title = message,
                PrimaryButtonText = "Zamknij"
            };
            await dialog.ShowAsync();
        }
    }
}
