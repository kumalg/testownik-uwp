using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace Testownik.Helpers {
    public class FlyoutsHelper {
        public static void ShowFlyoutBase(object sender) {
            var senderElement = sender as FrameworkElement;
            var flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
            flyoutBase.ShowAt(senderElement);
        }
    }
}
