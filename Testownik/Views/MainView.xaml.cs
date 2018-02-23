using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Testownik.Helpers;
using Testownik.Models.Test;
using Testownik.Services;
using Testownik.ViewModels;

namespace Testownik.Views {
    public sealed partial class MainView {
        public MainViewModel ViewModel { get; }

        public MainView() {
            InitializeComponent();
            SettingsHelper.SetSettings();
            ViewModel = new MainViewModel(DragGrid, DragInfo, DragBlurGrid, LoadingGrid, LoadingInfo, LoadingBlurGrid, new NavigationService());
            Window.Current.Dispatcher.AcceleratorKeyActivated += (d, e) => {
                if (e.VirtualKey != VirtualKey.T || Frame.CurrentSourcePageType == typeof(TestView))
                    return;

                var testController = TestController.GenerateSample();
                Frame.Navigate(typeof(TestView), testController);
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                AppViewBackButtonVisibility.Collapsed;
        }


        #region Drag&Drop

        protected override void OnDrop(DragEventArgs e) => ViewModel.OnDrop(e);
        protected override void OnDragOver(DragEventArgs e) => ViewModel.OnDragOver(e);
        protected override void OnDragLeave(DragEventArgs e) => ViewModel.OnDragLeave(e);

        #endregion

        private void Item_RightTapped(object sender, RightTappedRoutedEventArgs e) => FlyoutsHelper.ShowFlyoutBase(sender);
        private void Item_DragStarting(object sender, DragStartingEventArgs e) => FlyoutsHelper.ShowFlyoutBase(sender);

        private void RecentFoldersListViewItem_Click(object sender, RoutedEventArgs e) => ViewModel.RecentFoldersListViewItem_Click(sender, e);
        private void RecentFolder_Click(object sender, ItemClickEventArgs e) => ViewModel.RecentFolder_Click(sender, e);
    }
}