using Testownik.Model;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.Helpers;

namespace Testownik.Dialogs {
    public sealed partial class InfoDialog : ContentDialog {
        public string AppName = SystemInformation.ApplicationName;
        public string AppVersion = $"{SystemInformation.ApplicationVersion.Major}.{SystemInformation.ApplicationVersion.Minor}.{SystemInformation.ApplicationVersion.Build}.{SystemInformation.ApplicationVersion.Revision}";


        public InfoDialog() {
            InitializeComponent();
            RequestedTheme = SettingsHelper.AppTheme;
        }
    }
}