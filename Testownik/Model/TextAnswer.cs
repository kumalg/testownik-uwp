using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Testownik.Model
{
    public class TextAnswer : IAnswer
    {
        public object Content { get; set; }
        public int Key { get; set; }
    }
}
