using Windows.Foundation.Metadata;

namespace Testownik.Services {
    class PlatformApiService {
        private static readonly bool _isAcrylicBrushAvailable = ApiInformation.IsTypePresent("Windows.UI.Xaml.Media.AcrylicBrush");
        private static readonly bool _isApiContractV3Available = ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 3);

        public static bool IsAcrylicBrushAvailable => _isAcrylicBrushAvailable;
        public static bool IsApiContractV3Available => _isApiContractV3Available;
    }
}
