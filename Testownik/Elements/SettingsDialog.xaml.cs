using System.Collections.Generic;
using System.Linq;
using Testownik.Model;
using Windows.UI.Xaml.Controls;

//Szablon elementu Okno dialogowe zawartości jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace Testownik.Elements
{
    public sealed partial class SettingsDialog : ContentDialog
    {
        public IList<int> ReoccurrencesOnStart { get; } = new []{ 1, 2, 3, 4, 5 }.ToList();
        public IList<int> ReoccurrencesIfBad { get; } = new[] { 0, 1, 2, 3, 4 }.ToList();
        public IList<int> MaxReoccurrences { get; } = new[] { 2, 3, 4, 5, 6, 7, 8, 9, 10 }.ToList();

        public SettingsDialog()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            SetReoccurrencesOnStart();
            SetReoccurrencesIfBad();
            SetMaxReoccurrences();
        }

        private void SetReoccurrencesOnStart()
        {
            var index = ReoccurrencesOnStartComboBox.SelectedIndex;
            if (index == -1)
                return;
            SettingsHelper.ReoccurrencesOnStart = ReoccurrencesOnStart.ElementAt(index);
        }

        private void SetReoccurrencesIfBad()
        {
            var index = ReoccurrencesIfBadComboBox.SelectedIndex;
            if (index == -1)
                return;
            SettingsHelper.ReoccurrencesIfBad = ReoccurrencesIfBad.ElementAt(index);
        }

        private void SetMaxReoccurrences()
        {
            var index = MaxReoccurrencesComboBox.SelectedIndex;
            if (index == -1)
                return;
            SettingsHelper.MaxReoccurrences = MaxReoccurrences.ElementAt(index);
        }

        private void ContentDialog_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var onStartIndex = ReoccurrencesOnStart.IndexOf(SettingsHelper.ReoccurrencesOnStart);
            var onBadIndex = ReoccurrencesIfBad.IndexOf(SettingsHelper.ReoccurrencesIfBad);
            var onMaxIndex = MaxReoccurrences.IndexOf(SettingsHelper.MaxReoccurrences);

            ReoccurrencesOnStartComboBox.SelectedIndex = onStartIndex;
            ReoccurrencesIfBadComboBox.SelectedIndex = onBadIndex;
            MaxReoccurrencesComboBox.SelectedIndex = onMaxIndex;
        }
    }
}
