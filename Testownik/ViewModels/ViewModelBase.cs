using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Testownik.Dialogs;

namespace Testownik.ViewModels {
    public class ViewModelBase : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public async void ShowInfoDialog() {
            var dialog = new InfoDialog();
            await dialog.ShowAsync();
        }

        public async void ShowSettingsDialog() {
            var dialog = new SettingsDialog();
            await dialog.ShowAsync();
        }
    }
}