using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Testownik.Model;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace Testownik
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class SelectPage : Page, INotifyPropertyChanged
    {
        public ObservableCollection<FolderPath> folderPaths = new ObservableCollection<FolderPath>();
        public ObservableCollection<FolderPath> FolderPaths {
            get => folderPaths;
            set {
                folderPaths = value;
                RaisePropertyChanged(nameof(FolderPaths));
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                // Show UI in title bar if opted-in and in-app backstack is not empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
            }
            else
            {
                // Remove the UI from the title bar if in-app back stack is empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;
            }

            base.OnNavigatedTo(e);
        }

        public SelectPage()
        {
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

        private async void ReadFolderPathsFromJsonFile()
        {
            var jsonString = await DeserializeFileAsync("folderPaths.json");
            if (jsonString != null && !string.IsNullOrWhiteSpace(jsonString))
            {
                var list = (ObservableCollection<FolderPath>)JsonConvert.DeserializeObject(jsonString, typeof(ObservableCollection<FolderPath>));
                FolderPaths = list;
            }
        }

        private static async Task<string> DeserializeFileAsync(string fileName)
        {
            StorageFile localFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
            return await FileIO.ReadTextAsync(localFile);
        }

        private async void SelectFolder(StorageFolder folder = null)
        {
            if (folder == null)
                folder = await QuestionsReader.ShowFolderPicker();
            if (folder == null)
                return;

            ProgressGrid.Visibility = Visibility.Visible;
            var files = await QuestionsReader.ReadFiles(folder);
            var questions = await QuestionsReader.ReadQuestions(files);
            if (questions == null || !questions.Any())
            {
                ProgressGrid.Visibility = Visibility.Collapsed;
                var dialog = new ContentDialog()
                {
                    Title = "Brak pytań w bazie",
                    PrimaryButtonText = "Zamknij"
                };
                await dialog.ShowAsync();
                return;
            }

            // JSON

            var mru = Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList;
            string mruToken = mru.Add(folder, "folder");

            var newJsonItem = new FolderPath
            {
                Path = folder.Path,
                Token = mruToken,
                LastDate = DateTime.Now.ToString("R")
            };

            var toRemove = FolderPaths.FirstOrDefault(i => i.Path == newJsonItem.Path);
                FolderPaths.Remove(toRemove);

            FolderPaths.Insert(0, newJsonItem);

            var jsonString = JsonConvert.SerializeObject(FolderPaths.Take(5));

            // write string to a file
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("folderPaths.json", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, jsonString);

            // JSON END

            ProgressGrid.Visibility = Visibility.Collapsed;
            
            var testController = new TestController(questions);
            this.Frame.Navigate(typeof(MainPage), testController);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SelectFolder();
        }

        private async void Button_Drop(object sender, DragEventArgs e)
        {
            DragGrid.Visibility = Visibility.Collapsed;

            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                ProgressGrid.Visibility = Visibility.Visible;

                var storageItems = await e.DataView.GetStorageItemsAsync();
                IReadOnlyCollection<StorageFile> prepared;
                IDictionary<string, IQuestion> questions;

                var folder = storageItems.FirstOrDefault(i => i is IStorageFolder);
                if (folder != null)
                {
                    prepared = await ((StorageFolder)folder).GetFilesAsync();
                    questions = await QuestionsReader.ReadQuestions(prepared);
                }
                else
                {
                    questions = await QuestionsReader.ReadQuestions(storageItems.Where(i => i is IStorageFile).Cast<StorageFile>().ToList());
                }

                ProgressGrid.Visibility = Visibility.Collapsed;

                if (questions == null || !questions.Any())
                {
                    ProgressGrid.Visibility = Visibility.Collapsed;
                    var dialog = new ContentDialog()
                    {
                        Title = "Brak pytań w bazie",
                        PrimaryButtonText = "Zamknij"
                    };
                    await dialog.ShowAsync();
                    return;
                }

                var testController = new TestController(questions);
                this.Frame.Navigate(typeof(MainPage), testController);
            }
        }

        private void Grid_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
        }
       
        private void Grid_DragLeave(object sender, DragEventArgs e)
        {
            DragGrid.Visibility = Visibility.Collapsed;
        }

        private void Grid_DragEnter(object sender, DragEventArgs e)
        {
            DragGrid.Visibility = Visibility.Visible;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!(e.ClickedItem is FolderPath))
                return;

            var folderPath = e.ClickedItem as FolderPath;
            var mru = Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList;
            var folder = await mru.GetFolderAsync(folderPath.Token);
            if (folder == null)
                return;
            SelectFolder(folder);
        }
    }
}
