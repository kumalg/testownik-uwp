using System.Collections.Generic;
using Testownik.Models;

namespace Testownik.Model {
    public class Question : IQuestion {
        public IContent Content { get; set; }
        public IList<IAnswer> Answers { get; set; }
        public IList<int> CorrectAnswerKeys { get; set; }
    }
}