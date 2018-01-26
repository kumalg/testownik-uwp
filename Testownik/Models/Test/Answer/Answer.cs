using Testownik.Models;

namespace Testownik.Model {
    public class Answer : IAnswer {
        public IContent Content { get; set; }
        public int Key { get; set; }
    }
}