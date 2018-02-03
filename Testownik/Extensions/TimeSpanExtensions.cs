using System;

namespace Testownik.Extensions {
    public static class TimeSpanExtensions {
        public static string ToTimeString(this TimeSpan time) {
            return string.Format("{0:D2}:{1:D2}:{2:D2}",
                time.Hours,
                time.Minutes,
                time.Seconds);
        }
    }
}
