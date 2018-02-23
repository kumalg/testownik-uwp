using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Microsoft.Toolkit.Uwp.Helpers;
using Testownik.Dialogs;
using Testownik.Helpers;
using Testownik.Models;
using Testownik.Models.Test;
using Testownik.Services;

namespace Testownik.ViewModels {
    public class TestViewModel : ViewModelBase, INavigable {
        public string AppName = SystemInformation.ApplicationName;

        private readonly INavigationService _navigationService;

        private List<AnswerBlock> _answers;
        public List<AnswerBlock> Answers {
            get => _answers;
            set {
                _answers = value;
                OnPropertyChanged(nameof(Answers));
            }
        }

        private List<ComboBox> _multiAnswers;
        public List<ComboBox> MultiAnswers {
            get => _multiAnswers;
            set {
                _multiAnswers = value;
                OnPropertyChanged(nameof(MultiAnswers));
            }
        }

        private TestController _testController;
        public TestController TestController {
            get => _testController;
            set {
                _testController = value;
                OnPropertyChanged(nameof(TestController));
            }
        }

        private KeyValuePair<string, IQuestion> _question;
        public KeyValuePair<string, IQuestion> Question {
            get => _question;
            set {
                _question = value;
                OnPropertyChanged(nameof(Question));
            }
        }

        private int _reoccurrencesOfQuestion;

        public int ReoccurrencesOfQuestion {
            get => _reoccurrencesOfQuestion;
            set {
                _reoccurrencesOfQuestion = value;
                OnPropertyChanged(nameof(ReoccurrencesOfQuestion));
            }
        }

        private Visibility _buttonAcceptAnswerVisibility = Visibility.Visible;
        public Visibility ButtonAcceptAnswerVisibility {
            get => _buttonAcceptAnswerVisibility;
            set {
                _buttonAcceptAnswerVisibility = value;
                OnPropertyChanged(nameof(ButtonAcceptAnswerVisibility));
            }
        }

        private Visibility _buttonNextQuestionVisibility = Visibility.Collapsed;
        public Visibility ButtonNextQuestionVisibility {
            get => _buttonNextQuestionVisibility;
            set {
                _buttonNextQuestionVisibility = value;
                OnPropertyChanged(nameof(ButtonNextQuestionVisibility));
            }
        }

        public TestViewModel(INavigationService navigationService) {
            _navigationService = navigationService;
        }

        public void Activate(object parameter) {
            if (!(parameter is TestController controller))
                return;

            TestController = controller;
            TestController.StartTimer();
            NextQuestion();
        }

        public async void Deactivate(object parameter) {
            if (string.IsNullOrEmpty(TestController.FolderToken))
                return;

            var dialog = new MessageDialog {
                Title = "Czy chcesz zapisać aktualny stan?",
                PrimaryButtonText = "Tak",
                SecondaryButtonText = "Nie"
            };
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
                SavedStatesHelper.SaveActualState(TestController);
        }

        public void AcceptAnswer(GridView answersGridView, ListView answersListView) {
            IEnumerable<string> selectedAnswers;
            if (_question.Value is Question)
                selectedAnswers = answersGridView.SelectedItems.Cast<AnswerBlock>().Select(i => i.Answer.Key);
            else if (_question.Value is MultiQuestion)
                selectedAnswers = answersListView.Items?.Cast<ComboBox>().Select(i => i.SelectedValue == null
                    ? ""
                    : (i.SelectedValue as ComboBoxItem)?.Tag.ToString());
            else
                selectedAnswers = new List<string>();

            TestController.CheckAnswer(Question.Key, selectedAnswers);

            var gridViewItems = answersGridView
                .Items
                .Cast<AnswerBlock>()
                .Where(i => _question.Value.CorrectAnswerKeys.Contains(i.Answer.Key));
            foreach (var item in gridViewItems)
                item.MarkAsCorrect();

            ShowNextQuestionButton();
        }

        public async void NextQuestion() {
            if (TestController == null)
                return;

            if (TestController.IsTestCompleted()) {
                var contentDialog = new TestFinishedContentDialog(_testController.Time);
                await contentDialog.ShowAsync();
                //Frame.GoBack();
                _navigationService.GoBack();
                return;
            }

            Question = TestController.RandQuestion();
            if (Question.Value is Question) {
                MultiAnswers = null;
                Answers = (Question.Value as Question).Answers
                    .Select(i => new AnswerBlock { Answer = i, ImageBackground = new SolidColorBrush((i.Content is ImageContent) ? Colors.White : Colors.Transparent) })
                    .OrderBy(a => Guid.NewGuid())
                    .ToList();
            }
            else if (Question.Value is MultiQuestion) {
                Answers = null;
                MultiAnswers = (Question.Value as MultiQuestion).Answers
                    .Select(i => new ComboBox { ItemsSource = i.OrderBy(a => Guid.NewGuid()).Select(x => new ComboBoxItem { Content = x.Content.Value, Tag = x.Key }) })
                    .ToList();
            }

            ReoccurrencesOfQuestion = TestController.ReoccurrencesOfQuestion(Question.Key);
            ShowAcceptAnswerButton();
        }

        private void ShowAcceptAnswerButton() {
            ButtonAcceptAnswerVisibility = Visibility.Visible;
            ButtonNextQuestionVisibility = Visibility.Collapsed;
        }

        private void ShowNextQuestionButton() {
            ButtonAcceptAnswerVisibility = Visibility.Collapsed;
            ButtonNextQuestionVisibility = Visibility.Visible;
        }

        public async void ShowInfoDialog() {
            var dialog = new InfoDialog();
            await dialog.ShowAsync();
        }

        public async void ShowSettingsDialog() {
            var dialog = new SettingsDialog();
            await dialog.ShowAsync();
        }
    }
}