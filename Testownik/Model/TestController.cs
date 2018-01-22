using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testownik.Model
{
    public class TestController
    {
        private Random random = new Random();
        public IDictionary<string, IQuestion> Questions { get; set; }
        public IDictionary<string, int> Reoccurrences { get; set; }

        public TestController(IDictionary<string, IQuestion> questions)
        {
            Questions = questions;
            //Reoccurrences = questions.Select(q => new KeyValuePair<string, int>( q.Key, 2)).ToDictionary()
        }

        public TestController(string path)
        {

        }

        public TestController()
        {

        }

        public KeyValuePair<string, IQuestion> RandQuestion()
        {
            var question = Questions.ElementAt(random.Next(Questions.Count));
            question.Value.Answers = question.Value.Answers.OrderBy(a => Guid.NewGuid()).ToList();
            return question;
        }

        internal static TestController GenerateRand() {
            var questions = new Dictionary<string, IQuestion> {
                {"001.txt", new TextQuestion {
                        Content = "Pierwsze pytanie",
                        Answers = new List<IAnswer>() {
                            new TextAnswer { Content = "Jedno", Key = 1 },
                            new TextAnswer { Content = "Drugie", Key = 2 }
                        },
                        CorrectAnswerKeys = new[] { 2 }.ToList()
                    }
                },
                {"002.txt", new TextQuestion {
                        Content = "Czy weszło drugie pytanie?",
                        Answers = new List<IAnswer>() {
                            new TextAnswer { Content = "Jedno", Key = 1 },
                            new TextAnswer { Content = "Drugie", Key = 2 },
                            new TextAnswer { Content = "Trzecie", Key = 3 }
                        },
                        CorrectAnswerKeys = new[] { 2 }.ToList()
                    }
                },
                {"003.txt", new TextQuestion {
                        Content = "Czy weszło trzecie pytanie?",
                        Answers = new List<IAnswer>() {
                            new TextAnswer { Content = "Jedno", Key = 1 },
                            new TextAnswer { Content = "Drugie", Key = 2 },
                            new TextAnswer { Content = "Trzecie", Key = 3 }
                        },
                        CorrectAnswerKeys = new[] { 2 }.ToList()
                    }
                },
            };
            var reoqurrences = new Dictionary<string, int>() {
                { "001.txt", 2},
                { "002.txt", 2}
            };
            return new TestController { Questions = questions, Reoccurrences = reoqurrences };
        }
    }
}
