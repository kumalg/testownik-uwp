using System.ComponentModel;
using Testownik.Models.Test;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Testownik.Model {
    public class AnswerBlock : INotifyPropertyChanged {
        public IAnswer Answer { get; set; }
        public Brush ImageBackground { get; set; } = new SolidColorBrush(Colors.Transparent);
        public Visibility CorrectVisibility { get; set; } = Visibility.Collapsed;

        public void MarkAsCorrect() {
            CorrectVisibility = Visibility.Visible;
            RaisePropertyChanged(nameof(CorrectVisibility));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName) {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}