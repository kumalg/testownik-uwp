using Windows.UI.Xaml.Controls;

namespace Testownik.Models {
    public class TextContent : IContent {

        public object Value { get => textBlock; }
        private TextBlock textBlock;

        public TextContent(string content) {
            textBlock = new TextBlock {
                Text = content,
                TextAlignment = Windows.UI.Xaml.TextAlignment.Center,
                TextWrapping = Windows.UI.Xaml.TextWrapping.WrapWholeWords
            };
        }
    }
}
