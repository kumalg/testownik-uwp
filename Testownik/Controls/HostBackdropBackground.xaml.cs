﻿using System.Reflection;
using Testownik.Services;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

// Szablon elementu Kontrolka użytkownika jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234236

namespace Testownik.Controls {
    public sealed partial class HostBackdropBackground
    {
        public HostBackdropBackground() {
            InitializeComponent();
            DataContext = this;
        }


        public Color BackdropColor {
            get => (Color)GetValue(BackdropColorProperty);
            set => SetValue(BackdropColorProperty, value);
        }

        public static readonly DependencyProperty BackdropColorProperty =
            DependencyProperty.Register("BackdropColor", typeof(Color), typeof(HostBackdropBackground), new PropertyMetadata(Colors.Yellow, OnBackdropColorChanged));


        private static void OnBackdropColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (!(e.NewValue is Color color))
                return;

            var type = d.GetType();
            var backgroundProperty = type.GetProperty("Background");
            if (backgroundProperty == null)
                return;

            var fullColor = Color.FromArgb(0xff, color.R, color.G, color.B);

            if (PlatformApiService.IsAcrylicBrushAvailable) {
                var myBrush = new AcrylicBrush {
                    BackgroundSource = AcrylicBackgroundSource.HostBackdrop,
                    TintColor = fullColor,
                    FallbackColor = fullColor,
                    TintOpacity = color.A / 255d
                };

                backgroundProperty.SetValue(d, myBrush);
            }
            else {
                backgroundProperty.SetValue(d, new SolidColorBrush(fullColor));
            }
        }
    }
}
