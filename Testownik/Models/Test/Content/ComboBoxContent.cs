namespace Testownik.Models.Test {
    public class ComboBoxContent : IContent {
        public object Value { get; }

        public ComboBoxContent(string content) {
            Value = content;
        }
    }
}
