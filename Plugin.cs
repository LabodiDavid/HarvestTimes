using System;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace HarvestTimes
{
    [BepInPlugin(Id, "HarvestTimes", VersionInfo.Version)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string Id = "simplifydave.harvesttimes";
        private Harmony harmony;
        private static ConfigEntry<bool> timerEnabled;
        private static ConfigEntry<bool> showPlantGrowth;
        private static ConfigEntry<bool> showBushRespawn;

        private void Awake()
        {
            timerEnabled = Config.Bind("General", "Enabled", true,
                "Enable all HarvestTimes hover information.");
            showPlantGrowth = Config.Bind("Timers", "ShowPlantGrowth", true,
                "Show remaining growth time on planted crops and saplings.");
            showBushRespawn = Config.Bind("Timers", "ShowBushRespawn", false,
                "Show remaining respawn time on harvested raspberry, blueberry, and cloudberry bushes. Disabled by default.");
            try
            {
                HoverPatch.Initialize();
                harmony = new Harmony(Id);
                harmony.PatchAll(typeof(Plugin).Assembly);
                Logger.LogInfo("HarvestTimes loaded.");
            }
            catch (Exception ex)
            {
                harmony?.UnpatchSelf();
                Logger.LogError("Could not initialize HarvestTimes: " + ex);
            }
        }

        private void OnDestroy() => harmony?.UnpatchSelf();

        [HarmonyPatch(typeof(Plant), nameof(Plant.GetHoverText))]
        private static class HoverPatch
        {
            private static Func<Plant, float> growTime;
            private static Func<Plant, double> elapsedTime;

            internal static void Initialize()
            {
                growTime = AccessTools.MethodDelegate<Func<Plant, float>>(
                    AccessTools.Method(typeof(Plant), "GetGrowTime"));
                elapsedTime = AccessTools.MethodDelegate<Func<Plant, double>>(
                    AccessTools.Method(typeof(Plant), "TimeSincePlanted"));
            }

            [HarmonyPostfix]
            private static void Postfix(Plant __instance, ZNetView ___m_nview, ref string __result)
            {
                if (!timerEnabled.Value || !showPlantGrowth.Value || __instance == null || ___m_nview == null || !___m_nview.IsValid() || ZNet.instance == null)
                    return;

                if (__instance.GetStatus() != Plant.Status.Healthy)
                {
                    __result += "\n<color=#FFB66B>Cannot mature under current conditions.</color>";
                    return;
                }

                double remaining = growTime(__instance) - elapsedTime(__instance);
                if (double.IsNaN(remaining) || double.IsInfinity(remaining))
                    return;

                __result += remaining <= 0
                    ? "\n<color=#A8E080>Ready soon…</color>"
                    : "\n<color=#A8E080>Ready in: " + Timing.FormatRemaining(remaining) + "</color>";
            }
        }

        [HarmonyPatch(typeof(Pickable), nameof(Pickable.GetHoverText))]
        private static class BushHoverPatch
        {
            [HarmonyPostfix]
            private static void Postfix(Pickable __instance, ZNetView ___m_nview,
                bool ___m_picked, int ___m_enabled, ref string __result)
            {
                if (!timerEnabled.Value || !showBushRespawn.Value || __instance == null ||
                    !___m_picked || ___m_enabled == 0 || ___m_nview == null ||
                    !___m_nview.IsValid() || ZNet.instance == null ||
                    !(__instance.m_respawnTimeMinutes > 0) || __instance.m_itemPrefab == null)
                    return;

                // Identify berries by their drop prefab, never by a localized display name.
                string bushName = Timing.BushName(__instance.m_itemPrefab.name);
                if (bushName == null)
                    return;

                // Read synchronized data: the cached m_pickedTime can be stale on clients.
                long pickedTicks = ___m_nview.GetZDO().GetLong(ZDOVars.s_pickedTime, 0L);
                double remaining;
                if (!Timing.TryRespawnRemaining(pickedTicks, ZNet.instance.GetTime().Ticks,
                    __instance.m_respawnTimeMinutes, out remaining))
                    return;

                // Vanilla returns an empty hover string for harvested bushes.
                if (string.IsNullOrEmpty(__result))
                    __result = bushName;
                __result += remaining <= 0
                    ? "\n<color=#A8E080>Respawning soon…</color>"
                    : "\n<color=#A8E080>Respawns in: " + Timing.FormatRemaining(remaining) + "</color>";
            }
        }
    }
}


