using System.Collections.Generic;

namespace Testownik.Models.Test {
    public class Question : IQuestion {
        public IContent Content { get; set; }
        public IList<IAnswer> Answers { get; set; }
        public IList<string> CorrectAnswerKeys { get; set; }
    }
}