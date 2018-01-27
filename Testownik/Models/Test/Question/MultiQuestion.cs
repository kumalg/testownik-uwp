using System.Collections.Generic;

namespace Testownik.Models.Test {
    class MultiQuestion : IQuestion {
        public IContent Content { get; set; }
        public IList<IList<IAnswer>> Answers { get; set; }
        public IList<int> CorrectAnswerKeys { get; set; }
    }
}