using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Testownik.Helpers;
using Testownik.Models;
using Testownik.Models.Test;
using Testownik.Services;
using Testownik.Views;

namespace Testownik.ViewModels {
    public class MainViewModel : ViewModelBase {
        private readonly Grid _dragGrid, _dragInfo, _dragBlurGrid, _loadingGrid, _loadingInfo, _loadingBlurGrid;
        private readonly INavigationService _navigationService;

        private ObservableCollection<FolderPath> _folderPaths = new ObservableCollection<FolderPath>(MostRecentlyUsedListHelper.AsFolderPathIEnumerable());
        public ObservableCollection<FolderPath> FolderPaths {
            get => _folderPaths;
            set {
                _folderPaths = value;
                OnPropertyChanged(nameof(FolderPaths));
            }
        }

        private bool _isProgressRingActive;
        public bool IsProgressRingActive {
            get => _isProgressRingActive;
            set {
                _isProgressRingActive = value;
                OnPropertyChanged(nameof(IsProgressRingActive));
            }

        }

        public MainViewModel(Grid dragGrid, Grid dragInfo, Grid dragBlurGrid, Grid loadingGrid, Grid loadingInfo, Grid loadingBlurGrid, INavigationService navigationService) {
            _dragGrid = dragGrid;
            _dragInfo = dragInfo;
            _dragBlurGrid = dragBlurGrid;
            _loadingGrid = loadingGrid;
            _loadingInfo = loadingInfo;
            _loadingBlurGrid = loadingBlurGrid;
            _navigationService = navigationService;
        }

        #region Drag&Drop

        public async void OnDrop(DragEventArgs e) {
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
                _navigationService.Navigate(typeof(TestView), testController);
            }
        }

        public void OnDragOver(DragEventArgs e) {
            e.AcceptedOperation = DataPackageOperation.Copy;
            ShowDragView();
        }

        public void OnDragLeave(DragEventArgs e) {
            HideDragView();
        }

        #endregion

       public void RemoveRecentFolder(string token) {
            MostRecentlyUsedListHelper.Remove(token);
            RemoveRecentFolderFromList(token);
        }

        public void RemoveRecentFolderFromList(string token) {
            var element = FolderPaths.FirstOrDefault(i => i.Token == token);
            if (element != null)
                FolderPaths.Remove(element);
        }
        
        public void RecentFoldersListViewItem_Click(object sender, RoutedEventArgs e) {
            if (e.OriginalSource is FrameworkElement frameworkElement && frameworkElement.DataContext is FolderPath folderPath)
                RemoveRecentFolder(folderPath.Token);
        }
        

        public async void RecentFolder_Click(object sender, ItemClickEventArgs e) {
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
        
        public void SelectFolderClick() => SelectFolder();

        public async void SelectFolder(StorageFolder folder = null) {
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
            _navigationService.Navigate(typeof(TestView), testController);
            //Frame.Navigate(typeof(TestView), testController);
        }

        private string UpdateRecentlyUsedFoldersList(IStorageItem folder) {
            var folderPath = MostRecentlyUsedListHelper.Add(folder);

            var toRemove = FolderPaths.FirstOrDefault(i => i.Path == folderPath.Path);
            FolderPaths.Remove(toRemove);
            FolderPaths.Insert(0, folderPath);

            return folderPath.Token;
        }


        // Overlay Views Section     

        public async void HideLoadingView() {
            await HideViewTask(_loadingGrid, _loadingInfo, _loadingBlurGrid);
            IsProgressRingActive = false;
        }

        public async void ShowLoadingView() {
            IsProgressRingActive = true;
            await ShowViewTask(_loadingGrid, _loadingInfo, _loadingBlurGrid);
        }

        public async void HideDragView() => await HideViewTask(_dragGrid, _dragInfo, _dragBlurGrid);
        public async void ShowDragView() => await ShowViewTask(_dragGrid, _dragInfo, _dragBlurGrid);

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
    }
}
