using Testownik.Model;
using Windows.UI.Xaml.Controls;

//Szablon elementu Okno dialogowe zawartości jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace Testownik.Dialogs
{
    public sealed partial class MessageDialog : ContentDialog
    {
        public MessageDialog()
        {
            this.InitializeComponent();
            RequestedTheme = SettingsHelper.AppTheme;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
