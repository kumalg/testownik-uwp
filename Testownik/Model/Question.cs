using System.Collections.Generic;

namespace Testownik.Model
{
    public class Question : IQuestion
    {
        public object Content { get; set; }
        public IList<IAnswer> Answers { get; set; }
        public IList<int> CorrectAnswerKeys { get; set; }
    }
}
