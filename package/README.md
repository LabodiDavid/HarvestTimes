# HarvestTimes

Know when your next harvest is ready.

HarvestTimes adds a countdown to the normal hover text when you look at planted crops and saplings. Optionally, it also shows when harvested berry bushes will produce berries again.

**By simplifydave | Version 1.0.0**

## Features

- Plant growth countdown: **Ready in: 12m 34s**.
- Optional berry bush countdown: **Respawns in: 4h 28m 15s**.
- Supports raspberry, blueberry, and cloudberry bushes.
- Separate switches for plant growth and berry respawn timers.
- English mod text and configuration descriptions.
- Client-side display only; no server installation required.
- Does not change growth speed, respawn behavior, or saved world data.

## Installation

Install with a Thunderstore-compatible mod manager, or install BepInEx 5 for Valheim and copy `plugins/HarvestTimes/HarvestTimes.dll` into `BepInEx/plugins/HarvestTimes/`.

If you used the earlier PlantReadyTimer prototype, remove `PlantReadyTimer.dll` first to avoid duplicate timers. Its configuration does not migrate to HarvestTimes.

## Configuration

Start the game once to generate `BepInEx/config/simplifydave.harvesttimes.cfg`.

```ini
[General]
Enabled = true

[Timers]
ShowPlantGrowth = true
ShowBushRespawn = false
```

Set **ShowBushRespawn = true** to enable berry bush respawn timers. Restart the game after editing the configuration file. Configuration is local to each player.

Look at a plant or a harvested bush within the game's normal hover distance. Unharvested bushes keep their normal interaction text.

## How time is calculated

The mod reads each plant's own growth duration and planting time. Bush timers use the synchronized last-picked timestamp and the bush's respawn duration.

Times are shown in ordinary hours, minutes, and seconds at normal world speed. The countdown follows Valheim's world clock: sleeping, pausing, offline worlds, and time-changing mods can affect it. It is not an independent real-world stopwatch.

If a plant is unhealthy, the mod displays **Cannot mature under current conditions.** alongside the game's original status. At the end of a countdown, **Ready soon…** or **Respawning soon…** means the timer has elapsed but the game still needs to process growth or respawning. Berry checks can take roughly another minute in a loaded area; unloaded areas or additional spawn conditions can delay them further.

## Compatibility and scope

Requires BepInEx 5. Timers apply to standard `Plant` crops/saplings and respawning `Pickable` objects that drop Raspberry, Blueberries, or Cloudberry. Other resources, custom berry types, and custom growth systems are not covered.

The mod's added text is English. Existing game text follows your selected game language. Other mods that add timers may produce duplicate information. Compatibility with mods that replace growth, respawn, or hover behavior is not guaranteed.
