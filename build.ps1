param(
    [string]$ValheimPath = 'D:\SteamLibrary\steamapps\common\Valheim',
    [string]$BepInExPath = '',
    [switch]$Test
)
$ErrorActionPreference = 'Stop'
if (!$BepInExPath) { $BepInExPath = Join-Path $ValheimPath 'BepInEx' }
$managed = Join-Path $ValheimPath 'valheim_Data\Managed'
$core = Join-Path $BepInExPath 'core'
$sdkLine = @(& dotnet --list-sdks)[-1]
if (!$sdkLine -or $sdkLine -notmatch '^(\S+)\s+\[(.+)\]') { throw 'Install a .NET SDK first.' }
$compiler = Join-Path (Join-Path $Matches[2] $Matches[1]) 'Roslyn\bincore\csc.dll'
$artifactDir = Join-Path $PSScriptRoot 'artifacts'
$pluginDir = Join-Path $PSScriptRoot 'package\plugins\HarvestTimes'
New-Item -ItemType Directory -Force $artifactDir,$pluginDir | Out-Null
$references = @()
foreach ($name in @('mscorlib','netstandard','System','System.Core','UnityEngine','UnityEngine.CoreModule','assembly_valheim')) {
    $path = Join-Path $managed ($name + '.dll')
    if (!(Test-Path -LiteralPath $path)) { throw "Missing game library: $path" }
    $references += '-r:' + $path
}
foreach ($name in @('BepInEx','0Harmony')) {
    $path = Join-Path $core ($name + '.dll')
    if (!(Test-Path -LiteralPath $path)) { throw "Missing BepInEx library: $path" }
    $references += '-r:' + $path
}
$compilerArgs = @('-nologo','-target:library','-optimize+','-deterministic+','-langversion:latest','-nostdlib+',
    ('-out:' + (Join-Path $pluginDir 'HarvestTimes.dll')))
$compilerArgs += $references
$compilerArgs += @('Plugin.cs','Timing.cs','AssemblyInfo.cs') | ForEach-Object { Join-Path $PSScriptRoot $_ }
& dotnet $compiler @compilerArgs
if ($LASTEXITCODE -ne 0) { throw 'Compilation failed.' }
if ($Test) {
    # Run the pure timing logic against the same framework references used by the mod.
    $testExe = Join-Path $artifactDir 'TimingTests.exe'
    $testArgs = @('-nologo','-target:exe','-langversion:latest','-nostdlib+',('-out:' + $testExe))
    $testArgs += $references
    $testArgs += (Join-Path $PSScriptRoot 'Timing.cs'),(Join-Path $PSScriptRoot 'tests\TimingTests.cs')
    & dotnet $compiler @testArgs
    if ($LASTEXITCODE -ne 0) { throw 'Test compilation failed.' }
    & $testExe
    if ($LASTEXITCODE -ne 0) { throw 'Timing tests failed.' }
}
$zipPath = Join-Path $artifactDir 'HarvestTimes-1.0.0.zip'
Compress-Archive -Path (Join-Path $PSScriptRoot 'package\*') -DestinationPath $zipPath -Force
Write-Host "Built: $zipPath"
