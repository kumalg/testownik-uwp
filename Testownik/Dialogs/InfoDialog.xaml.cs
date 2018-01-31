using Testownik.Model;
using Windows.UI.Xaml.Controls;

namespace Testownik.Dialogs {
    public sealed partial class InfoDialog : ContentDialog {

        public InfoDialog() {
            InitializeComponent();
            RequestedTheme = SettingsHelper.AppTheme;
        }
    }
}