using System.Collections.Generic;

namespace Testownik.Model {
    public class JsonTestController {
        public IDictionary<string, int> Reoccurrences { get; set; }

        public int NumberOfQuestions { get; set; }
        public int NumberOfAnswers { get; set; }
        public int NumberOfCorrectAnswers { get; set; }
        public int NumberOfBadAnswers { get; set; }
        public int NumberOfLearnedQuestions { get; set; }
        public int NumberOfRemainingQuestions { get; set; }
        public long Time { get; set; }
    }
}
