using System;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Testownik.Models.Test {
    public class ImageContent : IContent {
        public object Value {
            get {
                if (_image == null)
                    _image = new Image {
                        Stretch = Windows.UI.Xaml.Media.Stretch.None
                    };
                SetNewBitmapImage();
                return _image;
            }
        }
        public StorageFile File { get; set; }

        public ImageContent(StorageFile file) {
            File = file;
        }

        private Image _image;
        private BitmapImage _bitmapImage;
        private async void SetNewBitmapImage() {
            if (_bitmapImage != null || File == null)
                return;

            _bitmapImage = new BitmapImage();
            _bitmapImage.SetSource(await File.OpenAsync(FileAccessMode.Read));
            _image.Source = _bitmapImage;
        }
    }
}
