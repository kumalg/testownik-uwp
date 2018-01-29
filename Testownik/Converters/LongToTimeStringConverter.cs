using System;
using Windows.UI.Xaml.Data;

namespace Testownik.Converters {
    public class LongToTimeStringConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            if (value == null)
                return null;

            var miliseconds = long.Parse(value.ToString());

            TimeSpan t = TimeSpan.FromTicks(miliseconds);

            return string.Format("{0:D2}:{1:D2}:{2:D2}",
                t.Hours,
                t.Minutes,
                t.Seconds,
                t.Milliseconds);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotSupportedException();
        }
    }
}