using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Windows.UI.Xaml;
using Testownik.Helpers;

namespace Testownik.Models.Test {
    public class TestController : INotifyPropertyChanged {
        private static TimeSpan _timerInterval = TimeSpan.FromSeconds(1);
        private readonly Random _random = new Random();

        public IDictionary<string, IQuestion> Questions { get; set; }
        public IDictionary<string, int> Reoccurrences { get; set; }

        public string FolderToken { get; private set; } = "";
        public int NumberOfQuestions { get; private set; }
        public int NumberOfAnswers { get; private set; }
        public int NumberOfCorrectAnswers { get; private set; }
        public int NumberOfBadAnswers { get; private set; }
        public int NumberOfLearnedQuestions { get; private set; }
        public int NumberOfRemainingQuestions { get; private set; }
        public long Time { get; private set; }
        private DispatcherTimer _timer;

        public TestController(IDictionary<string, IQuestion> questions, string folderToken = "", IDictionary<string, int> previousState = null) {
            FolderToken = folderToken;
            Questions = questions;
            NumberOfQuestions = questions.Count;
            NumberOfRemainingQuestions = NumberOfQuestions - NumberOfLearnedQuestions;
            Reoccurrences = previousState ?? questions.ToDictionary(node => node.Key, node => SettingsHelper.ReoccurrencesOnStart);
            PrepareTimer();
        }

        public void PrepareTimer() {
            _timer = new DispatcherTimer { Interval = _timerInterval };
            _timer.Tick += (s, e) => { RefreshTimer(); };
        }

        public void StartTimer() {
            _timer.Start();
        }

        public void StopTimer() {
            _timer.Stop();
        }

        private void RefreshTimer() {
            Time += _timerInterval.Ticks;
            RaisePropertyChanged(nameof(Time));
        }
        
        public TestController() { }

        public void CheckAnswer(string key, IEnumerable<string> answerKeys) {
            NumberOfAnswers++;
            RaisePropertyChanged(nameof(NumberOfAnswers));
            if (!Questions.ContainsKey(key)) {

            }
            else if (Questions[key].CorrectAnswerKeys.OrderBy(i => i).SequenceEqual(answerKeys.OrderBy(i => i))) {
                Reoccurrences[key] -= 1;
                NumberOfCorrectAnswers++;
                RaisePropertyChanged(nameof(NumberOfCorrectAnswers));
            }
            else {
                Reoccurrences[key] += SettingsHelper.ReoccurrencesIfBad;
                if (Reoccurrences[key] > SettingsHelper.MaxReoccurrences)
                    Reoccurrences[key] = SettingsHelper.MaxReoccurrences;

                NumberOfBadAnswers++;
                RaisePropertyChanged(nameof(NumberOfBadAnswers));
            }
            if (Reoccurrences.ContainsKey(key) && Reoccurrences[key] == 0) {
                NumberOfLearnedQuestions++;
                RaisePropertyChanged(nameof(NumberOfLearnedQuestions));
                NumberOfRemainingQuestions--;
                RaisePropertyChanged(nameof(NumberOfRemainingQuestions));
            }
            if (IsTestCompleted())
                StopTimer();
        }

        public bool IsTestCompleted() {
            return Reoccurrences.All(i => i.Value == 0);
        }

        public int ReoccurrencesOfQuestion(string key) => (Reoccurrences.ContainsKey(key))
            ? Reoccurrences[key]
            : 0;

        public KeyValuePair<string, IQuestion> RandQuestion() {
            var remainingQuestions = Questions.Where(q => ReoccurrencesOfQuestion(q.Key) != 0);
            return !remainingQuestions.Any() 
                ? new KeyValuePair<string, IQuestion>(string.Empty, new Question()) 
                : remainingQuestions.ElementAt(_random.Next(remainingQuestions.Count()));
        }

        internal static TestController GenerateSample() {
            var questions = new Dictionary<string, IQuestion> {
                {
                    "001.txt",
                    new Question {
                        Content = new TextContent("First question"),
                        Answers = new List<IAnswer> () {
                            new Answer { Content = new TextContent("First answer"), Key = "1" },
                            new Answer { Content = new TextContent("Second answer (Correct)"), Key = "2" }
                        },
                        CorrectAnswerKeys = new [] { "2" }.ToList()
                    }
                }, {
                    "002.txt",
                    new Question {
                        Content = new TextContent("Second question"),
                        Answers = new List<IAnswer> () {
                            new Answer { Content = new TextContent("First answer"), Key = "1" },
                            new Answer { Content = new TextContent("Second answer"), Key = "2" },
                            new Answer { Content = new TextContent("Third answer (Correct)"), Key = "3" }
                        },
                        CorrectAnswerKeys = new [] { "3" }.ToList()
                    }
                }, {
                    "003.txt",
                    new Question {
                        Content = new TextContent("Third question"),
                        Answers = new List<IAnswer> () {
                            new Answer { Content = new TextContent("First answer (Correct)"), Key = "1" },
                            new Answer { Content = new TextContent("Second answer"), Key = "2" },
                            new Answer { Content = new TextContent("Third answer"), Key = "3" }
                        },
                        CorrectAnswerKeys = new [] { "1" }.ToList ()
                    }
                },
            };
            return new TestController(questions);
        }

        public JsonTestController ToJson() {
            var test = new JsonTestController {
                Reoccurrences = Reoccurrences,
                NumberOfQuestions = NumberOfQuestions,
                NumberOfAnswers = NumberOfAnswers,
                NumberOfCorrectAnswers = NumberOfCorrectAnswers,
                NumberOfBadAnswers = NumberOfBadAnswers,
                NumberOfLearnedQuestions = NumberOfLearnedQuestions,
                NumberOfRemainingQuestions = NumberOfRemainingQuestions,
                Time = Time
            };
            return test;
        }

        public static TestController FromJson(JsonTestController jsonTestController, IDictionary<string, IQuestion> questions, string token) {
            //TODO should recalculate integers instead of get from json
            var test = new TestController {
                FolderToken = token,
                Questions = questions,
                Reoccurrences = jsonTestController.Reoccurrences, //TODO only reoccurrences where key is in questions
                NumberOfQuestions = jsonTestController.NumberOfQuestions,
                NumberOfAnswers = jsonTestController.NumberOfAnswers,
                NumberOfCorrectAnswers = jsonTestController.NumberOfCorrectAnswers,
                NumberOfBadAnswers = jsonTestController.NumberOfBadAnswers,
                NumberOfLearnedQuestions = jsonTestController.NumberOfLearnedQuestions,
                NumberOfRemainingQuestions = jsonTestController.NumberOfRemainingQuestions,
                Time = jsonTestController.Time
            };
            test.PrepareTimer();
            return test;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName) {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}