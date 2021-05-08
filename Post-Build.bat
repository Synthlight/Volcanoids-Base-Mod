REM call $(ProjectDir)..\Base-Mod\Post-Build.bat "$(TargetName)" "$(OutDir)" "$(PkgLib_Harmony)"
REM %1 == $(TargetName)
REM %2 == $(OutDir)
REM %3 == $(PkgLib_Harmony)
REM %4 == /LocalMods/ or /Mods/ if set.

REM You need a full pdb, not a portable one. Add `<DebugType>full</DebugType>` in the top level `<PropertyGroup>`.

REM If you need harmony, here's the package ref: `<PackageReference Include="Lib.Harmony" Version="2.0.4" GeneratePathProperty="true" />`
REM The `GeneratePathProperty="true"` is required to generate `$(PkgLib_Harmony)`.

setlocal enabledelayedexpansion

set MonoPath=C:\Program Files\Unity\Editor\Data\MonoBleedingEdge

for %%f in ("%~2*.dll") do (
    echo "Making mdb for %%f"
    REM exe but still needs to be run with mono as the interpreter else it'll wrongly default to .net.
    "%MonoPath%\bin\mono.exe" "%MonoPath%\lib\mono\4.5\pdb2mdb.exe" "%%f"
)

set TargetDir=%LOCALAPPDATA%Low\Volcanoid\Volcanoids\
set ModsDir=%TargetDir%Mods\
set LocalModsDir=%TargetDir%LocalMods\

mkdir "%ModsDir%%~1\"
copy "%~2*" "%ModsDir%%~1\"

REM Copy to the LocalMods dir too, but clean out everything except the `.workshop` file first.
mkdir "%LocalModsDir%%~1\"
for %%a in ("%LocalModsDir%%~1\*") do if /i not "%%~nxa"==".workshop" del "%%a"
copy "%~2*" "%LocalModsDir%%~1\"

if not [%~3]==[] (
    mkdir "%ModsDir%0Harmony\"
    copy "%~3\lib\net45\*" "%ModsDir%0Harmony\"

    mkdir "%LocalModsDir%0Harmony\"
    copy "%~3\lib\net45\*" "%LocalModsDir%0Harmony\"
)