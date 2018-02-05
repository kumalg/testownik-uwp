using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Testownik.Dialogs {
    public sealed partial class TestFinishedContentDialog : INotifyPropertyChanged {
        private long _time;
        public long Time {
            get => _time;
            set {
                _time = value;
                RaisePropertyChanged(nameof(Time));
            }
        }
        public TestFinishedContentDialog(long time) {
            InitializeComponent();
            Time = time;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
