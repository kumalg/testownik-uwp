using Windows.Storage;
using Windows.UI.Xaml;

namespace Testownik.Model {
    public class SettingsHelper {
        private static readonly ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;

        public static void SetSettings() {
            if (LocalSettings.Values["reoccurrencesIfBad"] == null)
                ReoccurrencesIfBad = 2;

            if (LocalSettings.Values["reoccurrencesOnStart"] == null)
                ReoccurrencesOnStart = 2;

            if (LocalSettings.Values["maxReoccurrences"] == null)
                MaxReoccurrences = 10;

            if (LocalSettings.Values["appTheme"] == null)
                AppTheme = ElementTheme.Light;
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

        public static ElementTheme AppTheme {
            get {
                switch (LocalSettings.Values["appTheme"]) {
                    case "Dark": return ElementTheme.Dark;
                    case "Light": return ElementTheme.Light;
                    default: return ElementTheme.Default;
                }
            }
            set {
                switch (value) {
                    case ElementTheme.Dark: LocalSettings.Values["appTheme"] = "Dark"; break;
                    case ElementTheme.Light: LocalSettings.Values["appTheme"] = "Light"; break;
                    default: LocalSettings.Values["appTheme"] = "Default"; break;
                }
            }
        }
    }
}