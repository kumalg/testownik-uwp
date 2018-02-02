using System;

namespace Testownik.Converters
{
    public static class LongExtensions
    {
        public static string ToTimeString(this long ticks)
        {
            var timeSpan = TimeSpan.FromTicks(ticks);
            return timeSpan.ToTimeString();
        }
    }
}
