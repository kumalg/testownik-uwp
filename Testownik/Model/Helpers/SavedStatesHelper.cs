using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace Testownik.Model.Helpers
{
    public class SavedStatesHelper {
        public static async Task<IDictionary<string, int>> GetSavedStateOfTest(string token)
        {
            var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("SavedStates", CreationCollisionOption.OpenIfExists);
            var jsonFile = await folder.TryGetItemAsync(token) as IStorageFile;
            if (jsonFile == null)
                return null;

            var dialog = new ContentDialog
            {
                Title = "Wczytać poprzedni stan?",
                Content = $"Data ostatniego zapisu: {jsonFile.DateCreated}",
                PrimaryButtonText = "Tak",
                SecondaryButtonText = "Nie"
            };
            var dialogResult = await dialog.ShowAsync();
            if (dialogResult != ContentDialogResult.Primary)
                return null;

            var reoccurrencesJson = await FileIO.ReadTextAsync(jsonFile);
            var reoccurrencesDictionary = JsonConvert.DeserializeObject<Dictionary<string, int>>(reoccurrencesJson);
            return reoccurrencesDictionary;
        }

        public static async void SaveActualState(TestController testController) {
            if (string.IsNullOrEmpty(testController.FolderToken))
                return;

            var reocurrencesJson = JsonConvert.SerializeObject(testController.Reoccurrences);
            var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("SavedStates", CreationCollisionOption.OpenIfExists);
            var jsonFile = await folder.CreateFileAsync(testController.FolderToken, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(jsonFile, reocurrencesJson);
        }
    }
}
