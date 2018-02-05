using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Testownik.Dialogs;
using Testownik.Models.Test;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Testownik.Extensions;
using Testownik.Helpers;
using Testownik.Models;

namespace Testownik {
    public sealed partial class MainPage : INotifyPropertyChanged {
        public string AppName = SystemInformation.ApplicationName;

        private List<AnswerBlock> _answers;
        public List<AnswerBlock> Answers {
            get => _answers;
            set {
                _answers = value;
                RaisePropertyChanged(nameof(Answers));
            }
        }

        private List<ComboBox> _multiAnswers;
        public List<ComboBox> MultiAnswers {
            get => _multiAnswers;
            set {
                _multiAnswers = value;
                RaisePropertyChanged(nameof(MultiAnswers));
            }
        }

        private TestController _testController;
        public TestController TestController {
            get => _testController;
            set {
                _testController = value;
                RaisePropertyChanged(nameof(TestController));
            }
        }

        private KeyValuePair<string, IQuestion> _question;
        public KeyValuePair<string, IQuestion> Question {
            get => _question;
            set {
                _question = value;
                RaisePropertyChanged(nameof(Question));
            }
        }

        private int _reoccurrencesOfQuestion;
        public int ReoccurrencesOfQuestion {
            get => _reoccurrencesOfQuestion;
            set {
                _reoccurrencesOfQuestion = value;
                RaisePropertyChanged(nameof(ReoccurrencesOfQuestion));
            }
        }

        public MainPage() {
            InitializeComponent();
        }

        protected override async void OnNavigatedFrom(NavigationEventArgs e) {
            if (!string.IsNullOrEmpty(TestController.FolderToken)) {
                var dialog = new MessageDialog {
                    Title = "Czy chcesz zapisać aktualny stan?",
                    PrimaryButtonText = "Tak",
                    SecondaryButtonText = "Nie"
                };
                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                    SavedStatesHelper.SaveActualState(TestController);
            }
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            if (e.Parameter is TestController controller) {
                TestController = controller;
                TestController.StartTimer();
                NextQuestion();
            }

            if (Frame.CanGoBack) {
                // Show UI in title bar if opted-in and in-app backstack is not empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
            }
            else {
                // Remove the UI from the title bar if in-app back stack is empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;
            }

            base.OnNavigatedTo(e);
        }

        private void ShowAcceptAnswerButton() {
            ButtonAcceptAnswer.Visibility = Visibility.Visible;
            ButtonNextQuestion.Visibility = Visibility.Collapsed;
        }

        private void ShowNextQuestionButton() {
            ButtonAcceptAnswer.Visibility = Visibility.Collapsed;
            ButtonNextQuestion.Visibility = Visibility.Visible;
        }

        private void ButtonAcceptAnswer_Click(object sender, RoutedEventArgs e) {
            IEnumerable<string> selectedAnswers;
            if (_question.Value is Question)
                selectedAnswers = AnswersGridView.SelectedItems.Cast<AnswerBlock>().Select(i => i.Answer.Key);
            else if (_question.Value is MultiQuestion)
                selectedAnswers = AnswersListView.Items?.Cast<ComboBox>().Select(i => i.SelectedValue == null 
                    ? "" 
                    : (i.SelectedValue as ComboBoxItem)?.Tag.ToString());
            else
                selectedAnswers = new List<string>();

            TestController.CheckAnswer(Question.Key, selectedAnswers);

            var gridViewItems = AnswersGridView
                .Items
                .Cast<AnswerBlock>()
                .Where(i => _question.Value.CorrectAnswerKeys.Contains(i.Answer.Key));
            foreach (var item in gridViewItems)
                item.MarkAsCorrect();

            ShowNextQuestionButton();
        }

        private async void ButtonNextQuestion_Click(object sender, RoutedEventArgs e) {
            if (TestController.IsTestCompleted()) {
                var contentDialog = new TestFinishedContentDialog(_testController.Time);
                await contentDialog.ShowAsync();
                Frame.GoBack();
            }
            else {
                NextQuestion();
            }
        }

        private void NextQuestion() {
            if (TestController == null)
                return;

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

        private async void SettingsButton_Click(object sender, RoutedEventArgs e) {
            var dialog = new SettingsDialog();
            await dialog.ShowAsync();
        }

        private void AnswersGridView_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e) {
            if (e.Key >= VirtualKey.Number1 && e.Key <= VirtualKey.Number9) {
                var index = e.Key - VirtualKey.Number1;

                var actualSelected = AnswersGridView.SelectedItems;
                if (index < 0 || index >= AnswersGridView.Items.Count)
                    return;
                var itemToChangeSelection = AnswersGridView.Items[index];
                if (actualSelected.Contains(itemToChangeSelection))
                    actualSelected.Remove(itemToChangeSelection);
                else
                    actualSelected.Add(itemToChangeSelection);
            }
            else if (e.Key == VirtualKey.X) {
                if (ButtonAcceptAnswer.Visibility == Visibility.Visible)
                    ButtonAcceptAnswer_Click(ButtonAcceptAnswer, null);
                else if (ButtonNextQuestion.Visibility == Visibility.Visible)
                    ButtonNextQuestion_Click(ButtonNextQuestion, null);
            }
            else if (e.Key == VirtualKey.D) {
                FocusTracker.Visibility = FocusTracker.Visibility == Visibility.Visible
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName) {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void InfoButton_Click(object sender, RoutedEventArgs e) {
            var dialog = new InfoDialog();
            await dialog.ShowAsync();
        }
    }
}