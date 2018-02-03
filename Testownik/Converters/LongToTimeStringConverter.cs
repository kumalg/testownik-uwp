using System;
using Windows.UI.Xaml.Data;
using Testownik.Extensions;

namespace Testownik.Converters {
    public class LongToTimeStringConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            if (value == null)
                return null;

            var miliseconds = long.Parse(value.ToString());
            var timeSpan = TimeSpan.FromTicks(miliseconds);
            return timeSpan.ToTimeString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotSupportedException();
        }
    }
}