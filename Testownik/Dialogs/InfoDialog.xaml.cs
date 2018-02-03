using Microsoft.Toolkit.Uwp.Helpers;
using Testownik.Helpers;

namespace Testownik.Dialogs {
    public sealed partial class InfoDialog {
        public string AppName = SystemInformation.ApplicationName;
        public string AppVersion = $"{SystemInformation.ApplicationVersion.Major}.{SystemInformation.ApplicationVersion.Minor}.{SystemInformation.ApplicationVersion.Build}.{SystemInformation.ApplicationVersion.Revision}";

        public InfoDialog() {
            InitializeComponent();
            RequestedTheme = SettingsHelper.AppTheme;
        }
    }
}