using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;
using Newtonsoft.Json;
using Testownik.Models;
using Testownik.Models.Test;

namespace Testownik.Helpers {
    public class SavedStatesHelper {
        public static string SavedStateFileName = "saved.state";

        public static async Task<JsonTestController> GetSavedStateOfTest(StorageFolder folder) {
            if (!(await folder.TryGetItemAsync(SavedStateFileName) is IStorageFile jsonFile))
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
            var folder = await MostRecentlyUsedListHelper.GetFolderAsync(testController.FolderToken);
            if ((folder.Attributes & FileAttributes.ReadOnly) == 0) {
                var jsonFile = await folder.CreateFileAsync(SavedStateFileName, CreationCollisionOption.OpenIfExists);
                await FileIO.WriteTextAsync(jsonFile, reocurrencesJson);
            }
            else {
                var savePicker = new FileSavePicker {
                    SuggestedStartLocation = PickerLocationId.Desktop,
                    SuggestedFileName = "saved"
                };
                savePicker.FileTypeChoices.Add("Zapis stanu", new List<string> { ".state" });
                var jsonFile = await savePicker.PickSaveFileAsync();
                if (jsonFile != null)
                    await FileIO.WriteTextAsync(jsonFile, reocurrencesJson);
            }
        }
    }
}
