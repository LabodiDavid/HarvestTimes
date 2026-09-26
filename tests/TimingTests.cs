using System;
using HarvestTimes;

internal static class TimingTests
{
    private static int checks;
    private static void Check(bool condition, string label)
    {
        if (!condition) throw new Exception(label);
        checks++;
    }
    private static void Main()
    {
        Check(Timing.FormatRemaining(0.01) == "1s", "Never show zero early");
        Check(Timing.FormatRemaining(59.1) == "1m 00s", "Minute rollover");
        Check(Timing.FormatRemaining(3599.1) == "1h 00m 00s", "Hour rollover");
        Check(Timing.FormatRemaining(90061) == "25h 01m 01s", "Durations beyond a day");
        Check(Timing.FormatRemaining(-1) == "0s", "Clamp negative time");
        long start = new DateTime(2026, 1, 1).Ticks;
        double remaining;
        Check(Timing.TryRespawnRemaining(start, start + 30 * TimeSpan.TicksPerMinute, 300, out remaining)
            && remaining == 16200, "Respawn minutes to seconds");
        Check(Timing.TryRespawnRemaining(start, start + 301 * TimeSpan.TicksPerMinute, 300, out remaining)
            && remaining < 0, "Overdue respawn");
        Check(!Timing.TryRespawnRemaining(0, start, 300, out remaining), "Missing timestamp");
        Check(Timing.TryRespawnRemaining(1, start, 300, out remaining) && remaining == 0, "Game sentinel");
        Check(!Timing.TryRespawnRemaining(-1, start, 300, out remaining), "Invalid timestamp");
        Check(!Timing.TryRespawnRemaining(long.MaxValue, start, 300, out remaining), "Timestamp overflow");
        Check(!Timing.TryRespawnRemaining(start, start, double.NaN, out remaining), "Invalid duration");
        Check(!Timing.TryRespawnRemaining(start, start, 0, out remaining), "Non-respawning pickable");
        Check(Timing.BushName("Raspberry") == "Raspberry bush", "Raspberry filter");
        Check(Timing.BushName("Blueberries") == "Blueberry bush", "Blueberry filter");
        Check(Timing.BushName("Cloudberry") == "Cloudberry bush", "Cloudberry filter");
        Check(Timing.BushName("Mushroom") == null, "Exclude other pickables");
        System.Console.WriteLine("Passed " + checks + " timing and berry-filter checks.");
    }
}
