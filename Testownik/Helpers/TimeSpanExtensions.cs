using System;

namespace Testownik.Helpers {
    public static class TimeSpanHelper {
        public static string ToTimeString(this TimeSpan time) {
            return string.Format("{0:D2}:{1:D2}:{2:D2}",
                time.Hours,
                time.Minutes,
                time.Seconds);
        }

        public static string ToTimeString(this long ticks) {
            var timeSpan = TimeSpan.FromTicks(ticks);
            return timeSpan.ToTimeString();
        }
    }
}
