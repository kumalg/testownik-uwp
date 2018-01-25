using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Windows.UI.Xaml;

namespace Testownik.Model {
    public class TestController : INotifyPropertyChanged {
        private Random random = new Random();
        public IDictionary<string, IQuestion> Questions { get; set; }
        public IDictionary<string, int> Reoccurrences { get; set; }

        public int NumberOfQuestions { get; private set; } = 0;
        public int NumberOfAnswers { get; private set; } = 0;
        public int NumberOfCorrectAnswers { get; private set; } = 0;
        public int NumberOfBadAnswers { get; private set; } = 0;
        public int NumberOfLearnedQuestions { get; private set; } = 0;
        public int NumberOfRemainingQuestions { get; private set; } = 0;
        public long Time { get; private set; } = 0;
        public string TimeString { get; private set; } = "00:00:00";
        private long startTime;
        private DispatcherTimer timer;

        public TestController(IDictionary<string, IQuestion> questions) {
            Questions = questions;
            NumberOfQuestions = questions.Count;
            NumberOfRemainingQuestions = NumberOfQuestions - NumberOfLearnedQuestions;
            Reoccurrences = questions.ToDictionary(node => node.Key, node => SettingsHelper.ReoccurrencesOnStart);

            startTime = DateTime.Now.Ticks;
            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += (s, e) => { RefreshTimer(); };
            timer.Start ();
        }

        private void RefreshTimer() {
            Time = DateTime.Now.Ticks - startTime;
            TimeSpan t = TimeSpan.FromTicks(Time);
            TimeString = string.Format("{0:D2}:{1:D2}:{2:D2}",
                t.Hours,
                t.Minutes,
                t.Seconds,
                t.Milliseconds);
            RaisePropertyChanged(nameof(Time));
            RaisePropertyChanged(nameof(TimeString));
        }

        public TestController(string path) {

        }

        public TestController() {

        }

        public void CheckAnswer(string key, IEnumerable<int> answerKeys) {
            NumberOfAnswers++;
            RaisePropertyChanged(nameof(NumberOfAnswers));
            if (!Questions.ContainsKey(key)) {

            } else if (Questions[key].CorrectAnswerKeys.OrderBy(i => i).SequenceEqual(answerKeys.OrderBy(i => i))) {
                Reoccurrences[key] -= 1;
                NumberOfCorrectAnswers++;
                RaisePropertyChanged(nameof(NumberOfCorrectAnswers));
            } else {
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
        }

        public bool IsTestCompleted() {
            return Reoccurrences.All(i => i.Value == 0);
        }

        public int ReoccurrencesOfQuestion(string key) => (Reoccurrences.ContainsKey(key)) 
            ? Reoccurrences[key] 
            : 0;

        public KeyValuePair<string, IQuestion> RandQuestion() {
            var remainingQuestions = Questions.Where(q => Reoccurrences[q.Key] != 0);
            if (!remainingQuestions.Any())
                return new KeyValuePair<string, IQuestion>(string.Empty, new Question());
            return remainingQuestions.ElementAt(random.Next(remainingQuestions.Count()));
        }

        internal static TestController GenerateRand() {
            var questions = new Dictionary<string, IQuestion> {
                {
                    "001.txt",
                    new Question {
                        Content = "First question",
                        Answers = new List<IAnswer> () {
                            new Answer { Content = "First answer", Key = 1 },
                            new Answer { Content = "Second answer (Correct)", Key = 2 }
                        },
                        CorrectAnswerKeys = new [] { 2 }.ToList ()
                    }
                }, {
                    "002.txt",
                    new Question {
                        Content = "Second question",
                        Answers = new List<IAnswer> () {
                            new Answer { Content = "First answer", Key = 1 },
                            new Answer { Content = "Second answer", Key = 2 },
                            new Answer { Content = "Third answer (Correct)", Key = 3 }
                        },
                        CorrectAnswerKeys = new [] { 3 }.ToList ()
                    }
                }, {
                    "003.txt",
                    new Question {
                        Content = "Third question",
                        Answers = new List<IAnswer> () {
                            new Answer { Content = "First answer (Correct)", Key = 1 },
                            new Answer { Content = "Second answer", Key = 2 },
                            new Answer { Content = "Third answer", Key = 3 }
                        },
                        CorrectAnswerKeys = new [] { 1 }.ToList ()
                    }
                },
            };
            return new TestController (questions);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged (string propertyName) {
            var handler = PropertyChanged;
            handler?.Invoke (this, new PropertyChangedEventArgs (propertyName));
        }
    }
}