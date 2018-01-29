using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Testownik.Models
{
    public class ThemeAwareFrame : Frame
    {
        private static readonly ThemeProxyClass _themeProxyClass = new ThemeProxyClass();

        public static readonly DependencyProperty AppThemeProperty = DependencyProperty.Register(
            "AppTheme", typeof(ElementTheme), typeof(ThemeAwareFrame), new PropertyMetadata(default(ElementTheme), (d, e) => _themeProxyClass.Theme = (ElementTheme)e.NewValue));


        public ElementTheme AppTheme {
            get => (ElementTheme)GetValue(AppThemeProperty);
            set => SetValue(AppThemeProperty, value);
        }

        public ThemeAwareFrame()
        {
            var themeBinding = new Binding { Source = _themeProxyClass, Path = new PropertyPath("Theme"), Mode = BindingMode.OneWay };
            SetBinding(RequestedThemeProperty, themeBinding);
        }

        // Proxy class to be used as singleton
        sealed class ThemeProxyClass : INotifyPropertyChanged
        {
            private ElementTheme _theme;

            public ElementTheme Theme {
                get => _theme;
                set {
                    _theme = value;
                    OnPropertyChanged();
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
