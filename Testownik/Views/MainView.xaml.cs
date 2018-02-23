using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Testownik.Dialogs;
using Testownik.Helpers;
using Testownik.Models;
using Testownik.Models.Test;

namespace Testownik.Views {
    public sealed partial class SelectPage : INotifyPropertyChanged {
        private ObservableCollection<FolderPath> _folderPaths = new ObservableCollection<FolderPath>(MostRecentlyUsedListHelper.AsFolderPathIEnumerable());
        public ObservableCollection<FolderPath> FolderPaths {
            get => _folderPaths;
            set {
                _folderPaths = value;
                RaisePropertyChanged(nameof(FolderPaths));
            }
        }

        public SelectPage() {
            InitializeComponent();
            SettingsHelper.SetSettings();
            Window.Current.Dispatcher.AcceleratorKeyActivated += (d, e) => {
                if (e.VirtualKey != VirtualKey.T || Frame.CurrentSourcePageType == typeof(TestView))
                    return;

                var testController = TestController.GenerateRand();
                Frame.Navigate(typeof(TestView), testController);
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                AppViewBackButtonVisibility.Collapsed;
            base.OnNavigatedTo(e);
        }


        #region Drag&Drop

        protected override async void OnDrop(DragEventArgs e) {
            HideDragView();

            if (e.DataView.Contains(StandardDataFormats.StorageItems)) {
                ShowLoadingView();

                var storageItems = await e.DataView.GetStorageItemsAsync();

                var folder = storageItems.FirstOrDefault(i => i is IStorageFolder);
                if (folder != null) {
                    SelectFolder((StorageFolder)folder);
                    return;
                }

                var questions = await QuestionsReader.ReadQuestions(storageItems.Where(i => i is IStorageFile).Cast<StorageFile>().ToList());

                HideLoadingView();

                if (questions == null || !questions.Any()) {
                    DialogsHelper.ShowBasicMessageDialogAsync("Brak pytań w bazie");
                    return;
                }

                var testController = new TestController(questions);
                Frame.Navigate(typeof(TestView), testController);
            }
        }

        protected override void OnDragOver(DragEventArgs e) {
            e.AcceptedOperation = DataPackageOperation.Copy;
            ShowDragView();
        }

        protected override void OnDragLeave(DragEventArgs e) {
            HideDragView();
        }

        #endregion

        private void Item_RightTapped(object sender, RightTappedRoutedEventArgs e) => FlyoutsHelper.ShowFlyoutBase(sender);
        private void Item_DragStarting(object sender, DragStartingEventArgs e) => FlyoutsHelper.ShowFlyoutBase(sender);

        private void RecentFoldersListViewItem_Click(object sender, RoutedEventArgs e) {
            if (e.OriginalSource is FrameworkElement frameworkElement && frameworkElement.DataContext is FolderPath folderPath)
                RemoveRecentFolder(folderPath.Token);
        }

        private void FolderPickerButton_Click(object sender, RoutedEventArgs e) => SelectFolder();

        private async void RecentFolder_Click(object sender, ItemClickEventArgs e) {
            if (!(e.ClickedItem is FolderPath))
                return;

            var folderPath = e.ClickedItem as FolderPath;
            var folder = await MostRecentlyUsedListHelper.GetFolderAsync(folderPath.Token);
            if (folder != null)
                SelectFolder(folder);
            else {
                DialogsHelper.ShowBasicMessageDialogAsync("Folder nie istnieje");
                RemoveRecentFolder(folderPath.Token);
            }
        }

        private void RemoveRecentFolder(string token) {
            MostRecentlyUsedListHelper.Remove(token);
            RemoveRecentFolderFromList(token);
        }

        private void RemoveRecentFolderFromList(string token) {
            var element = FolderPaths.FirstOrDefault(i => i.Token == token);
            if (element != null)
                FolderPaths.Remove(element);
        }

        private async void SettingsButton_Click(object sender, RoutedEventArgs e) {
            var dialog = new SettingsDialog();
            await dialog.ShowAsync();
        }

        private async void SelectFolder(StorageFolder folder = null) {
            folder = folder ?? await QuestionsReader.ShowFolderPicker();
            if (folder == null)
                return;

            ShowLoadingView();
            var files = await QuestionsReader.ReadFiles(folder);
            var questions = await QuestionsReader.ReadQuestions(files);
            if (questions == null || !questions.Any()) {
                HideLoadingView();
                DialogsHelper.ShowBasicMessageDialogAsync("Brak pytań w bazie");
                return;
            }

            var token = UpdateRecentlyUsedFoldersList(folder);
            var previousState = await SavedStatesHelper.GetSavedStateOfTest(token);
            HideLoadingView();

            var testController = previousState != null
                ? TestController.FromJson(previousState, questions, token)
                : new TestController(questions, token);
            Frame.Navigate(typeof(TestView), testController);
        }

        private string UpdateRecentlyUsedFoldersList(IStorageItem folder) {
            var folderPath = MostRecentlyUsedListHelper.Add(folder);

            var toRemove = FolderPaths.FirstOrDefault(i => i.Path == folderPath.Path);
            FolderPaths.Remove(toRemove);
            FolderPaths.Insert(0, folderPath);

            return folderPath.Token;
        }


        // Overlay Views Section     

        private async void HideLoadingView() {
            await HideViewTask(LoadingGrid, LoadingInfo, LoadingBlurGrid);
            ProgressRing.IsActive = false;
        }

        private async void ShowLoadingView() {
            ProgressRing.IsActive = true;
            await ShowViewTask(LoadingGrid, LoadingInfo, LoadingBlurGrid);
        }

        private async void HideDragView() => await HideViewTask(DragGrid, DragInfo, DragBlurGrid);
        private async void ShowDragView() => await ShowViewTask(DragGrid, DragInfo, DragBlurGrid);

        private static async Task HideViewTask(Grid view, Grid info, Grid blur) {
            info.Fade().Start();
            await blur.Blur().StartAsync();
            view.Visibility = Visibility.Collapsed;
        }

        private static async Task ShowViewTask(Grid view, Grid info, Grid blur) {
            view.Visibility = Visibility.Visible;
            info.Fade(value: 1f).Start();
            await blur.Blur(value: 10f).StartAsync();
        }

        // End of Overlay Views Section     


        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName) {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void ApplicationInfoButton_Click(object sender, RoutedEventArgs e) {
            var dialog = new InfoDialog();
            await dialog.ShowAsync();
        }
    }
}