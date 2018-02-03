using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Newtonsoft.Json;
using Testownik.Models;
using Testownik.Models.Test;

namespace Testownik.Helpers {
    public class SavedStatesHelper {
        public static async Task<JsonTestController> GetSavedStateOfTest(string token) {
            var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("SavedStates", CreationCollisionOption.OpenIfExists);
            if (!(await folder.TryGetItemAsync(token) is IStorageFile jsonFile))
                return null;

            var dialog = new ContentDialog {
                Title = "Wczytać poprzedni stan?",
                Content = $"Data ostatniego zapisu: {(await jsonFile.GetBasicPropertiesAsync()).DateModified.LocalDateTime}",
                PrimaryButtonText = "Tak",
                SecondaryButtonText = "Nie"
            };
            var dialogResult = await dialog.ShowAsync();
            if (dialogResult != ContentDialogResult.Primary)
                return null;

            var reoccurrencesJson = await FileIO.ReadTextAsync(jsonFile);
            var reoccurrencesDictionary = JsonConvert.DeserializeObject<JsonTestController>(reoccurrencesJson);
            return reoccurrencesDictionary;
        }

        public static async void SaveActualState(TestController testController) {
            if (string.IsNullOrEmpty(testController.FolderToken))
                return;

            var reocurrencesJson = JsonConvert.SerializeObject(testController.ToJson());
            var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("SavedStates", CreationCollisionOption.OpenIfExists);
            var jsonFile = await folder.CreateFileAsync(testController.FolderToken, CreationCollisionOption.OpenIfExists);
            await FileIO.WriteTextAsync(jsonFile, reocurrencesJson);
        }
    }
}
