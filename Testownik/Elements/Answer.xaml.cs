using Testownik.Model;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace Testownik.Elements
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class Answer : UserControl
    {
        private IAnswer TextAnswer => DataContext as IAnswer;
        public Answer()
        {
            this.InitializeComponent();
            //DataContextChanged += (s, e) => Bindings.Update();
        }
    }
}
