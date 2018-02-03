using System.Collections.Generic;
using System.Linq;
using Testownik.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Testownik.Helpers;

namespace Testownik.Dialogs {
    public sealed partial class SettingsDialog {
        public IList<int> ReoccurrencesOnStart { get; } = new[] { 1, 2, 3, 4, 5 }.ToList();
        public IList<int> ReoccurrencesIfBad { get; } = new[] { 0, 1, 2, 3, 4 }.ToList();
        public IList<int> MaxReoccurrences { get; } = new[] { 2, 3, 4, 5, 6, 7, 8, 9, 10 }.ToList();

        public SettingsDialog() {
            InitializeComponent();
            InitializeThemeRadioButtons();
            RequestedTheme = SettingsHelper.AppTheme;
        }

        private void InitializeThemeRadioButtons() {
            if (SettingsHelper.AppTheme == ElementTheme.Dark)
                DarkThemeRadioButton.IsChecked = true;
            else
                LightThemeRadioButton.IsChecked = true;
            DarkThemeRadioButton.Checked += DarkThemeRadioButton_Checked;
            LightThemeRadioButton.Checked += LightThemeRadioButton_Checked;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
            SetReoccurrencesOnStart();
            SetReoccurrencesIfBad();
            SetMaxReoccurrences();
        }

        private void SetReoccurrencesOnStart() {
            var index = ReoccurrencesOnStartComboBox.SelectedIndex;
            if (index == -1)
                return;
            SettingsHelper.ReoccurrencesOnStart = ReoccurrencesOnStart.ElementAt(index);
        }

        private void SetReoccurrencesIfBad() {
            var index = ReoccurrencesIfBadComboBox.SelectedIndex;
            if (index == -1)
                return;
            SettingsHelper.ReoccurrencesIfBad = ReoccurrencesIfBad.ElementAt(index);
        }

        private void SetMaxReoccurrences() {
            var index = MaxReoccurrencesComboBox.SelectedIndex;
            if (index == -1)
                return;
            SettingsHelper.MaxReoccurrences = MaxReoccurrences.ElementAt(index);
        }

        private void ContentDialog_Loaded(object sender, RoutedEventArgs e) {
            var onStartIndex = ReoccurrencesOnStart.IndexOf(SettingsHelper.ReoccurrencesOnStart);
            var onBadIndex = ReoccurrencesIfBad.IndexOf(SettingsHelper.ReoccurrencesIfBad);
            var onMaxIndex = MaxReoccurrences.IndexOf(SettingsHelper.MaxReoccurrences);

            ReoccurrencesOnStartComboBox.SelectedIndex = onStartIndex;
            ReoccurrencesIfBadComboBox.SelectedIndex = onBadIndex;
            MaxReoccurrencesComboBox.SelectedIndex = onMaxIndex;
        }

        private void LightThemeRadioButton_Checked(object sender, RoutedEventArgs e) => SetTheme(ElementTheme.Light);
        private void DarkThemeRadioButton_Checked(object sender, RoutedEventArgs e) => SetTheme(ElementTheme.Dark);

        private void SetTheme(ElementTheme newTheme) {
            SettingsHelper.AppTheme = RequestedTheme = newTheme;
            var frame = (Window.Current.Content as Frame);
            if (frame != null && frame is ThemeAwareFrame themeAwareFrame)
                themeAwareFrame.AppTheme = newTheme;
        }
    }
}