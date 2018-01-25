using Windows.Storage;

namespace Testownik.Model {
    public class SettingsHelper {
        private static readonly ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;

        public static void SetSettings() {
            if (LocalSettings.Values["reoccurrencesIfBad"] == null)
                LocalSettings.Values["reoccurrencesIfBad"] = 2;

            if (LocalSettings.Values["reoccurrencesOnStart"] == null)
                LocalSettings.Values["reoccurrencesOnStart"] = 2;

            if (LocalSettings.Values["maxReoccurrences"] == null)
                LocalSettings.Values["maxReoccurrences"] = 10;
        }

        public static int ReoccurrencesIfBad {
            get => (int)LocalSettings.Values["reoccurrencesIfBad"];
            set => LocalSettings.Values["reoccurrencesIfBad"] = value;
        }

        public static int ReoccurrencesOnStart {
            get => (int)LocalSettings.Values["reoccurrencesOnStart"];
            set => LocalSettings.Values["reoccurrencesOnStart"] = value;
        }

        public static int MaxReoccurrences {
            get => (int)LocalSettings.Values["maxReoccurrences"];
            set => LocalSettings.Values["maxReoccurrences"] = value;
        }
    }
}