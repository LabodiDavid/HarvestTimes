# HarvestTimes

Valheim BepInEx 5 plugin by simplifydave. Version **1.0.0**, plugin GUID **simplifydave.harvesttimes**.

See [the player README](package/README.md) for features, installation, configuration, and timing limitations.

## Build and test

Requires Windows, a .NET SDK, the .NET Framework runtime for the test executable, Valheim, and BepInEx 5.

```powershell
./build.ps1 -ValheimPath 'D:\SteamLibrary\steamapps\common\Valheim' -Test
```

For a mod-manager profile, add `-BepInExPath 'C:\path\to\profile\BepInEx'`.

This script compiles against the installed game's own framework and BepInEx libraries without downloading packages. It produces `package/plugins/HarvestTimes/HarvestTimes.dll` and `artifacts/HarvestTimes-1.0.0.zip`. Game and BepInEx DLLs are not redistributed.

The included `.csproj` also supports ordinary SDK builds:

```powershell
dotnet build HarvestTimes.csproj -c Release -p:ValheimPath='D:\SteamLibrary\steamapps\common\Valheim'
```

Use `build.ps1` to create the release archive. Local paths can be placed in an ignored `Directory.Build.props` file. Build outputs, local configuration, libraries, and IDE files are excluded by `.gitignore`.

## Release checklist

1. Run `./build.ps1 -Test`.
2. Test in Valheim: healthy and unhealthy plants, each supported harvested bush, picking a bush again, countdown completion, save/reload, multiplayer client behavior, and each configuration switch.
3. Optionally set `website_url` in `package/manifest.json` to the real GitHub repository URL, then rebuild.
4. Upload `artifacts/HarvestTimes-1.0.0.zip` to your Thunderstore team. The plugin GUID is independent of your Thunderstore team name.

Compilation and pure timing tests do not replace an in-game test. No in-game or multiplayer validation has been performed yet.

Package structure follows the [Thunderstore package requirements](https://wiki.thunderstore.io/mods/creating-a-package) and [BepInEx packaging rules](https://wiki.thunderstore.io/mods/packaging-your-mods).
