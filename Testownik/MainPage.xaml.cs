using Testownik.Model;
using Testownik.Model.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI;
using System;
using Windows.ApplicationModel.Core;
using Testownik.Elements;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x415

namespace Testownik
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public string appName = AppIdentity.AppName;

        public List<AnswerBlock> answers;
        public List<AnswerBlock> Answers {
            get => answers;
            set {
                answers = value;
                RaisePropertyChanged(nameof(Answers));
            }
        }

        public TestController testController;
        public TestController TestController {
            get => testController;
            set {
                testController = value;
                RaisePropertyChanged(nameof(TestController));
            }
        }

        public KeyValuePair<string, IQuestion> textQuestion;
        public KeyValuePair<string, IQuestion> TextQuestion {
            get => textQuestion;
            set {
                textQuestion = value;
                RaisePropertyChanged(nameof(TextQuestion));
            }
        }

        public int reoccurrencesOfQuestion;
        public int ReoccurrencesOfQuestion {
            get => reoccurrencesOfQuestion;
            set {
                reoccurrencesOfQuestion = value;
                RaisePropertyChanged(nameof(ReoccurrencesOfQuestion));
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
            SettingsHelper.SetSettings();
            Co();
        }

        private async void Co()
        {
            var co = await QuestionsReader.ReadQuestions();
            if (co == null || !co.Any())
            {
                var dialog = new ContentDialog()
                {
                    Content = "Nie wybrano pliku bądź baza jest pusta",
                    PrimaryButtonText = "Wyjdź z aplikacji",
                    SecondaryButtonText = "Powtórz"
                };
                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Secondary)
                    Co();
                else
                    CoreApplication.Exit();
            }
            else
            {
                TestController = new TestController(co);
                NextQuestion();
            }
        }

        private void ShowAcceptAnswerButton()
        {
            ButtonAcceptAnswer.Visibility = Visibility.Visible;
            ButtonNextQuestion.Visibility = Visibility.Collapsed;
        }

        private void ShowNextQuestionButton()
        {
            ButtonAcceptAnswer.Visibility = Visibility.Collapsed;
            ButtonNextQuestion.Visibility = Visibility.Visible;
        }

        private void ButtonAcceptAnswer_Click(object sender, RoutedEventArgs e)
        {
            var selectedAnswers = AnswersGridView.SelectedItems.Cast<AnswerBlock>().Select(i => i.Answer.Key);
            TestController.CheckAnswer(TextQuestion.Key, selectedAnswers);
            
            var gridViewItems = AnswersGridView
                .Items
                .Cast<AnswerBlock>()
                .Where(i => textQuestion.Value.CorrectAnswerKeys.Contains(i.Answer.Key));
            foreach (var item in gridViewItems)
                item.MarkAsCorrect();

            ShowNextQuestionButton();
        }

        private void ButtonNextQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (TestController.IsTestCompleted())
            {
                var contentDialog = new ContentDialog
                {
                    Content = "Test zakończony!",
                    PrimaryButtonText = "Spoko",
                    SecondaryButtonText = "Anuluj"
                };
                var result = contentDialog.ShowAsync();
            }
            else
            {
                NextQuestion();
            }
        }

        private void NextQuestion()
        {
            TextQuestion = TestController.RandQuestion();
            Answers = TextQuestion.Value.Answers
                .Select(i => new AnswerBlock { Answer = i })
                .OrderBy(a => Guid.NewGuid())
                .ToList();
            
            ReoccurrencesOfQuestion = TestController.ReoccurrencesOfQuestion(TextQuestion.Key);
            ShowAcceptAnswerButton();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        private async void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SettingsDialog();
            await dialog.ShowAsync();
        }
    }
}
