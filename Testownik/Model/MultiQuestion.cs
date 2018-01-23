using System.Collections.Generic;

namespace Testownik.Model {
    class MultiQuestion {
        public object Content { get; set; }
        public IList<IList<IAnswer>> Choices { get; set; }
        public IList<int> CorrectAnswerKeys { get; set; }
    }
}
