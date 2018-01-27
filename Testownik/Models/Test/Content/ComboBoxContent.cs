namespace Testownik.Models.Test {
    class ComboBoxContent : IContent {
        public object Value { get; }

        public ComboBoxContent(string content) {
            Value = content;
        }
    }
}
