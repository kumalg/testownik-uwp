using Windows.UI.Xaml.Media;

namespace Testownik.Model
{
    public interface IAnswer
    {
        object Content { get; set; }
        int Key { get; set; }
    }
}
