using System;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Testownik.Models.Test {
    public class ImageContent : IContent {
        public object Value {
            get {
                if (image == null)
                    image = new Image() {
                        Stretch = Windows.UI.Xaml.Media.Stretch.None
                    };
                SetNewBitmapImage();
                return image;
            }
        }
        public StorageFile File { get; set; }

        public ImageContent(StorageFile file) {
            File = file;
        }

        private Image image;
        private BitmapImage bitmapImage;
        private async void SetNewBitmapImage() {
            if (bitmapImage != null || File == null)
                return;

            bitmapImage = new BitmapImage();
            bitmapImage.SetSource(await File.OpenAsync(FileAccessMode.Read));
            image.Source = bitmapImage;
        }
    }
}
