using System.Collections.Generic;

namespace Testownik.Models.Test {
    public interface IQuestion {
        IContent Content { get; set; }
        //IList<IAnswer> Answers { get; set; }
        IList<string> CorrectAnswerKeys { get; set; }
    }
}