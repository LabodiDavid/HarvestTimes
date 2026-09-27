# Building

## Visual Studio on Windows

1. Install Visual Studio 2022 with the **.NET desktop development** workload and **.NET Framework 4.7.2 targeting pack**.
2. Open `HarvestTimes.sln`.
3. Check `ValheimPath` and `R2ProfileName` in `HarvestTimes.csproj`. Defaults are `D:\SteamLibrary\steamapps\common\Valheim` and the r2modman `Default` profile.
4. Install BepInEx 5 in that profile and close Valheim before building.
5. Select **Release / Any CPU**, then **Build Solution**.

Each successful build:

- Produces `bin/Release/net472/HarvestTimes.dll` (or `bin/Debug/net472/` in Debug).
- Copies the DLL into `%APPDATA%/r2modmanPlus-local/Valheim/profiles/Default/BepInEx/plugins/HarvestTimes/` using the selected profile.
- Creates `dist/HarvestTimes-1.0.0.zip` for Thunderstore.

Game and BepInEx dependency DLLs are referenced locally and are not copied into the release.

## One version setting

Change only `PluginVersion` in `HarvestTimes.csproj`. The BepInEx version constant (`obj/.../VersionInfo.cs`), assembly version, file version, informational version, assembly metadata, manifest version, and ZIP filename are generated from it. Keep a three-part version such as `1.0.0` and update the changelog when releasing.

`Thunderstore/manifest.json` intentionally contains `__VERSION__`. Upload the generated ZIP in `dist`, not the template directory.

## Local settings

Create an ignored `Directory.Build.props` next to the project to override machine-specific paths without committing them:

```xml
<Project>
  <PropertyGroup>
    <ValheimPath>D:\SteamLibrary\steamapps\common\Valheim</ValheimPath>
    <R2ProfileName>Default</R2ProfileName>
    <!-- Optional: disable copying the built DLL into the profile. -->
    <DeployToR2Modman>false</DeployToR2Modman>
  </PropertyGroup>
</Project>
```

You may also override `R2BasePath`, `BepInExPath`, and `PluginPath`. `CreatePackage=false` disables ZIP generation. Solution and direct project builds both use project-relative packaging paths.

## Command-line build and tests

The script uses Visual Studio's MSBuild and the same project as the IDE:

```powershell
./build.ps1 -Test
./build.ps1 -R2ProfileName Default -NoDeploy -Test
```

`-NoDeploy` skips copying into the game profile. The 17 timing/filter checks run against .NET Framework 4.7.2. Generated output, DLLs, archives, runtime configuration, and local IDE settings are excluded by `.gitignore`.

## Release checks

Test healthy/unhealthy crops, each supported harvested berry bush, repeated harvesting, timer completion, save/reload, multiplayer, and configuration switches in Valheim. In-game and multiplayer validation have not yet been performed.

Optionally set the real GitHub URL in `Thunderstore/manifest.json`, then rebuild and upload the generated ZIP to your Thunderstore team. The GUID is independent of the Thunderstore team name.

Packaging follows the [Thunderstore requirements](https://wiki.thunderstore.io/mods/creating-a-package). No GitHub repository or Thunderstore release is published automatically.
