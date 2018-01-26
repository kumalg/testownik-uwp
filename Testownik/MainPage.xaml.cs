using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Testownik.Dialogs;
using Testownik.Model;
using Testownik.Model.Helpers;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Testownik {
    public sealed partial class MainPage : Page, INotifyPropertyChanged {
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

        public MainPage() {
            InitializeComponent();
        }

        protected override async void OnNavigatedFrom(NavigationEventArgs e) {
            if (!string.IsNullOrEmpty(TestController.FolderToken)) {
                var dialog = new ContentDialog {
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
            if (e.Parameter is TestController) {
                TestController = (TestController)e.Parameter;
                TestController.Start();
                NextQuestion();
            }

            if (Frame.CanGoBack) {
                // Show UI in title bar if opted-in and in-app backstack is not empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
            } else {
                // Remove the UI from the title bar if in-app back stack is empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;
            }

            base.OnNavigatedTo (e);
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
            var selectedAnswers = AnswersGridView.SelectedItems.Cast<AnswerBlock>().Select(i => i.Answer.Key);
            TestController.CheckAnswer(TextQuestion.Key, selectedAnswers);

            var gridViewItems = AnswersGridView
                .Items
                .Cast<AnswerBlock>()
                .Where (i => textQuestion.Value.CorrectAnswerKeys.Contains(i.Answer.Key));
            foreach (var item in gridViewItems)
                item.MarkAsCorrect();

            ShowNextQuestionButton();
        }

        private async void ButtonNextQuestion_Click(object sender, RoutedEventArgs e) {
            if (TestController.IsTestCompleted()) {
                var contentDialog = new ContentDialog {
                    Content = "Test zakończony!",
                    SecondaryButtonText = "Wyjdź"
                };
                var result = await contentDialog.ShowAsync();
                if (result == ContentDialogResult.Secondary) {
                    Frame.GoBack();
                }
            } else {
                NextQuestion();
            }
        }

        private void NextQuestion() {
            if (TestController == null)
                return;

            TextQuestion = TestController.RandQuestion();
            Answers = TextQuestion.Value.Answers
                .Select(i => new AnswerBlock { Answer = i })
                .OrderBy (a => Guid.NewGuid ())
                .ToList();

            ReoccurrencesOfQuestion = TestController.ReoccurrencesOfQuestion(TextQuestion.Key);
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
            } else if (e.Key == VirtualKey.X) {
                if (ButtonAcceptAnswer.Visibility == Visibility.Visible)
                    ButtonAcceptAnswer_Click(ButtonAcceptAnswer, null);
                else if (ButtonNextQuestion.Visibility == Visibility.Visible)
                    ButtonNextQuestion_Click(ButtonNextQuestion, null);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName) {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}