using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testownik.Model
{
    public class TestController : INotifyPropertyChanged
    {
        private Random random = new Random();
        public IDictionary<string, IQuestion> Questions { get; set; }
        public IDictionary<string, int> Reoccurrences { get; set; }

        public int NumberOfQuestions { get; private set; } = 0;
        public int NumberOfAnswers { get; private set; } = 0;
        public int NumberOfCorrectAnswers { get; private set; } = 0;
        public int NumberOfBadAnswers { get; private set; } = 0;
        public int NumberOfLearnedQuestions { get; private set; } = 0;
        public int NumberOfRemainingQuestions { get; private set; } = 0;

        public TestController(IDictionary<string, IQuestion> questions)
        {
            Questions = questions;
            NumberOfQuestions = questions.Count;
            NumberOfRemainingQuestions = NumberOfQuestions - NumberOfLearnedQuestions;
            //RaisePropertyChanged(nameof(NumberOfQuestions));
            Reoccurrences = questions.ToDictionary(node => node.Key, node => 2);
        }

        public TestController(string path)
        {

        }

        public TestController()
        {

        }

        public void CheckAnswer(string key, IEnumerable<int> answerKeys)
        {
            NumberOfAnswers++;
            RaisePropertyChanged(nameof(NumberOfAnswers));

            if (Questions[key].CorrectAnswerKeys.OrderBy(i => i).SequenceEqual(answerKeys.OrderBy(i => i))){
                Reoccurrences[key] -= 1;
                NumberOfCorrectAnswers++;
                RaisePropertyChanged(nameof(NumberOfCorrectAnswers));
            }
            else
            {
                Reoccurrences[key] += 2;
                NumberOfBadAnswers++;
                RaisePropertyChanged(nameof(NumberOfBadAnswers));
            }
            if (Reoccurrences[key] == 0)
            {
                NumberOfLearnedQuestions++;
                RaisePropertyChanged(nameof(NumberOfLearnedQuestions));
                NumberOfRemainingQuestions--;
                RaisePropertyChanged(nameof(NumberOfRemainingQuestions));
            }
        }

        public bool IsTestCompleted()
        {
            return Reoccurrences.All(i => i.Value == 0);
        }

        public int ReoccurrencesOfQuestion(string key) => Reoccurrences[key];

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

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
