using Windows.ApplicationModel;

namespace Testownik.Model.Helpers {
    public static class AppIdentity {
        public static string AppName => Package.Current.DisplayName;
    }
}
