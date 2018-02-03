using Testownik.Helpers;

namespace Testownik.Dialogs {
    public sealed partial class MessageDialog {
        public MessageDialog() {
            InitializeComponent();
            RequestedTheme = SettingsHelper.AppTheme;
        }
    }
}
