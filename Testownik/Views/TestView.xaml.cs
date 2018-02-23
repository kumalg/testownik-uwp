using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using Testownik.Services;
using Testownik.ViewModels;

namespace Testownik.Views {
    public sealed partial class TestView {
        public TestViewModel ViewModel { get; }

        public TestView() {
            InitializeComponent();
            ViewModel = new TestViewModel(new NavigationService());
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = Frame.CanGoBack
                ? AppViewBackButtonVisibility.Visible
                : AppViewBackButtonVisibility.Collapsed;

            ViewModel.Activate(e.Parameter);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e) {
            base.OnNavigatedFrom(e);

            ViewModel.Deactivate(e.Parameter);
        }

        private void ButtonAcceptAnswer_Click(object sender, RoutedEventArgs e) => ViewModel.AcceptAnswer(AnswersGridView, AnswersListView);

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
                    ViewModel.AcceptAnswer(AnswersGridView, AnswersListView);
                else if (ButtonNextQuestion.Visibility == Visibility.Visible)
                    ViewModel.NextQuestion();
            }
            else if (e.Key == VirtualKey.D) {
                FocusTracker.Visibility = FocusTracker.Visibility == Visibility.Visible
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }
        }
    }
}