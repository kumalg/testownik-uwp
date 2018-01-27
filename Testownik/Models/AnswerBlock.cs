using System.ComponentModel;
using Testownik.Models.Test;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Testownik.Model {
    public class AnswerBlock : INotifyPropertyChanged {
        public IAnswer Answer { get; set; }
        public Brush Brush { get; set; } = new SolidColorBrush(Colors.White);

        public void MarkAsCorrect() {
            Brush = (SolidColorBrush) Application.Current.Resources["GreenColorLightest"];
            RaisePropertyChanged(nameof(Brush));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName) {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}