param(
    [string]$Configuration = 'Release',
    [string]$ValheimPath = '',
    [string]$R2ProfileName = '',
    [string]$BepInExPath = '',
    [switch]$NoDeploy,
    [switch]$Test
)
$ErrorActionPreference = 'Stop'
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
if (!(Test-Path -LiteralPath $vswhere)) { throw 'Install Visual Studio 2022 with .NET desktop development and the .NET Framework 4.7.2 targeting pack.' }
$msbuild = & $vswhere -latest -products '*' -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1
if (!$msbuild) { throw 'Visual Studio MSBuild was not found.' }
$buildArgs = @((Join-Path $PSScriptRoot 'HarvestTimes.sln'), '-restore', '-nologo', '-verbosity:minimal', "-p:Configuration=$Configuration")
if ($ValheimPath) { $buildArgs += "-p:ValheimPath=$ValheimPath" }
if ($R2ProfileName) { $buildArgs += "-p:R2ProfileName=$R2ProfileName" }
if ($BepInExPath) { $buildArgs += "-p:BepInExPath=$BepInExPath" }
if ($NoDeploy) { $buildArgs += '-p:DeployToR2Modman=false' }
& $msbuild @buildArgs
if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }
if ($Test) {
    $compiler = Join-Path (Split-Path $msbuild) 'Roslyn\csc.exe'
    $framework = Join-Path ${env:ProgramFiles(x86)} 'Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2'
    $artifactDir = Join-Path $PSScriptRoot 'artifacts'
    New-Item -ItemType Directory -Force $artifactDir | Out-Null
    $testExe = Join-Path $artifactDir 'TimingTests.exe'
    & $compiler -nologo -target:exe -langversion:latest -nostdlib+ "-r:$framework\mscorlib.dll" "-r:$framework\System.dll" "-r:$framework\System.Core.dll" "-out:$testExe" (Join-Path $PSScriptRoot 'Timing.cs') (Join-Path $PSScriptRoot 'tests\TimingTests.cs')
    if ($LASTEXITCODE -ne 0) { throw 'Test compilation failed.' }
    & $testExe
    if ($LASTEXITCODE -ne 0) { throw 'Timing tests failed.' }
}
