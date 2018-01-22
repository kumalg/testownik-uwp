using System.Collections.Generic;

namespace Testownik.Model
{
    public interface IQuestion
    {
        object Content { get; set; }
        IList<IAnswer> Answers { get; set; }
        IList<int> CorrectAnswerKeys { get; set; }
    }
}
