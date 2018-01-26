using System.Collections.Generic;
using Testownik.Models;

namespace Testownik.Model {
    public interface IQuestion {
        IContent Content { get; set; }
        IList<IAnswer> Answers { get; set; }
        IList<int> CorrectAnswerKeys { get; set; }
    }
}