using Windows.UI.Xaml.Controls;

namespace Testownik.Models.Test {
    public class TextContent : IContent {

        public object Value => _textBlock;
        private readonly TextBlock _textBlock;

        public TextContent(string content) {
            _textBlock = new TextBlock {
                Text = content,
                TextAlignment = Windows.UI.Xaml.TextAlignment.Center,
                TextWrapping = Windows.UI.Xaml.TextWrapping.WrapWholeWords
            };
        }
    }
}
