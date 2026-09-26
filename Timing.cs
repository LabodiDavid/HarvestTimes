using System;

namespace HarvestTimes
{
    internal static class Timing
    {
        internal static string BushName(string itemPrefab)
        {
            switch (itemPrefab)
            {
                case "Raspberry": return "Raspberry bush";
                case "Blueberries": return "Blueberry bush";
                case "Cloudberry": return "Cloudberry bush";
                default: return null;
            }
        }

        internal static bool TryRespawnRemaining(long pickedTicks, long nowTicks,
            double respawnMinutes, out double remaining)
        {
            remaining = 0;
            if (pickedTicks < 0 || pickedTicks > DateTime.MaxValue.Ticks ||
                nowTicks < 0 || nowTicks > DateTime.MaxValue.Ticks ||
                double.IsNaN(respawnMinutes) || double.IsInfinity(respawnMinutes) || respawnMinutes <= 0)
                return false;

            // Zero is uninitialized; one is the game's immediately eligible sentinel.
            if (pickedTicks == 0)
                return false;
            if (pickedTicks == 1)
                return true;

            remaining = respawnMinutes * 60 -
                (nowTicks - pickedTicks) / (double)TimeSpan.TicksPerSecond;
            return !double.IsNaN(remaining) && !double.IsInfinity(remaining);
        }

        internal static string FormatRemaining(double seconds)
        {
            var time = TimeSpan.FromSeconds(Math.Ceiling(Math.Max(0, seconds)));
            if (time.TotalHours >= 1)
                return $"{(long)time.TotalHours}h {time.Minutes:00}m {time.Seconds:00}s";
            return time.Minutes > 0
                ? $"{time.Minutes}m {time.Seconds:00}s"
                : $"{time.Seconds}s";
        }
    }
}
