using Windows.Foundation.Metadata;

namespace Testownik.Services {
    public class PlatformApiService {
        public static bool IsAcrylicBrushAvailable { get; } = ApiInformation.IsTypePresent("Windows.UI.Xaml.Media.AcrylicBrush");
        public static bool IsApiContractV3Available { get; } = ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 3);
    }
}
