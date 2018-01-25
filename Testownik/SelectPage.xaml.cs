using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Testownik.Dialogs;
using Testownik.Model;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Testownik {
    public sealed partial class SelectPage : Page, INotifyPropertyChanged {
        public ObservableCollection<FolderPath> folderPaths = new ObservableCollection<FolderPath>();
        public ObservableCollection<FolderPath> FolderPaths {
            get => folderPaths;
            set {
                folderPaths = value;
                RaisePropertyChanged(nameof(FolderPaths));
            }
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            if (Frame.CanGoBack) {
                // Show UI in title bar if opted-in and in-app backstack is not empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
            } else {
                // Remove the UI from the title bar if in-app back stack is empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;
            }

            base.OnNavigatedTo(e);
        }

        public SelectPage() {
            this.InitializeComponent();
            SettingsHelper.SetSettings();
            ReadFolderPathsFromJsonFile();
            Window.Current.Dispatcher.AcceleratorKeyActivated += (d, e) => {
                if (e.VirtualKey != VirtualKey.T || Frame.CurrentSourcePageType == typeof(MainPage))
                    return;

                var testController = TestController.GenerateRand();
                this.Frame.Navigate(typeof(MainPage), testController);
            };
        }

        private void ReadFolderPathsFromJsonFile() {
            var mru = Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList;
            FolderPaths = new ObservableCollection<FolderPath>(mru.Entries.Select(i => new FolderPath { Path = i.Metadata, Token = i.Token }));
        }

        private static async Task<string> DeserializeFileAsync(string fileName) {
            StorageFile localFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
            return await FileIO.ReadTextAsync(localFile);
        }

        private async void SelectFolder(StorageFolder folder = null) {
            if (folder == null)
                folder = await QuestionsReader.ShowFolderPicker();
            if (folder == null)
                return;

            ShowLoadingView();
            var files = await QuestionsReader.ReadFiles(folder);
            var questions = await QuestionsReader.ReadQuestions(files);
            if (questions == null || !questions.Any()) {
                HideLoadingView();
                var dialog = new ContentDialog() {
                    Title = "Brak pytań w bazie",
                    PrimaryButtonText = "Zamknij"
                };
                await dialog.ShowAsync();
                return;
            }

            UpdateMRU(folder);
            HideLoadingView();

            var testController = new TestController(questions);
            Frame.Navigate(typeof(MainPage), testController);
        }

        private void UpdateMRU(IStorageItem folder) {
            var mru = Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList;
            var folderPathItem = new FolderPath {
                Path = folder.Path,
                Token = mru.Add(folder, folder.Path)
            };

            var toRemove = FolderPaths.FirstOrDefault(i => i.Path == folderPathItem.Path);
            FolderPaths.Remove(toRemove);
            FolderPaths.Insert(0, folderPathItem);
        }

        private void Button_Click(object sender, RoutedEventArgs e) => SelectFolder();

        private async void Button_Drop(object sender, DragEventArgs e) {
            HideDragItemView();

            if (e.DataView.Contains(StandardDataFormats.StorageItems)) {
                ShowLoadingView();

                var storageItems = await e.DataView.GetStorageItemsAsync();
                IReadOnlyCollection<StorageFile> prepared;
                IDictionary<string, IQuestion> questions;

                var folder = storageItems.FirstOrDefault(i => i is IStorageFolder);
                if (folder != null) {
                    prepared = await ((StorageFolder)folder).GetFilesAsync();
                    questions = await QuestionsReader.ReadQuestions(prepared);
                } else {
                    questions = await QuestionsReader.ReadQuestions(storageItems.Where(i => i is IStorageFile).Cast<StorageFile>().ToList());
                }

                HideLoadingView();

                if (questions == null || !questions.Any()) {
                    var dialog = new ContentDialog() {
                        Title = "Brak pytań w bazie",
                        PrimaryButtonText = "Zamknij"
                    };
                    await dialog.ShowAsync();
                    return;
                }

                if (folder != null)
                    UpdateMRU(folder);

                var testController = new TestController(questions);
                this.Frame.Navigate(typeof(MainPage), testController);
            }
        }

        private void Grid_DragOver(object sender, DragEventArgs e) {
            e.AcceptedOperation = DataPackageOperation.Copy;
            ShowDragItemView();
        }

        private void Grid_DragLeave(object sender, DragEventArgs e) {
            HideDragItemView();
        }

        private async void HideLoadingView()
        {
            LoadingInfo
                .Fade(value: 0f, duration: 500, delay: 0)
                .StartAsync();
            await LoadingBlurGrid
                .Blur(value: 0, duration: 500, delay: 0)
                .StartAsync();
            LoadingGrid.Visibility = Visibility.Collapsed;
            ProgressRing.IsActive = false;
        }

        private void ShowLoadingView()
        {
            LoadingGrid.Visibility = Visibility.Visible;
            LoadingInfo
                .Fade(value: 1f, duration: 500, delay: 0)
                .StartAsync();
            LoadingBlurGrid
                .Blur(value: 10, duration: 500, delay: 0)
                .StartAsync();
            ProgressRing.IsActive = true;
        }

        private async void HideDragItemView() {
            DragInfo
                .Fade(value: 0f, duration: 500, delay: 0)
                .StartAsync();
            await DragBlurGrid
                .Blur(value: 0, duration: 500, delay: 0)
                .StartAsync();
            DragGrid.Visibility = Visibility.Collapsed;
        }

        private void ShowDragItemView() {
            DragGrid.Visibility = Visibility.Visible;
            DragInfo
                .Fade(value: 1f, duration: 500, delay: 0)
                .StartAsync();
            DragBlurGrid
                .Blur(value: 10, duration: 500, delay: 0)
                .StartAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName) {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void ListView_ItemClick(object sender, ItemClickEventArgs e) {
            if (!(e.ClickedItem is FolderPath))
                return;

            var folderPath = e.ClickedItem as FolderPath;
            var mru = Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList;
            var folder = await mru.GetFolderAsync(folderPath.Token);
            if (folder == null)
                return;
            SelectFolder(folder);
        }

        private async void SettingsButton_Click(object sender, RoutedEventArgs e) {
            var dialog = new SettingsDialog();
            await dialog.ShowAsync();
        }
    }
}