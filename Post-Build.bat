REM call $(ProjectDir)..\Base-Mod\Post-Build.bat "$(TargetName)" "$(OutDir)"
REM %1 == $(TargetName)
REM %2 == $(OutDir)

REM You need a full pdb, not a portable one. Add `<DebugType>full</DebugType>` in the top level `<PropertyGroup>`.

setlocal enabledelayedexpansion

set MonoPath=C:\Program Files\Unity\Editor\Data\MonoBleedingEdge

for %%f in ("%~2*.dll") do (
    echo "Making mdb for %%f"
    REM exe but still needs to be run with mono as the interpreter else it'll wrongly default to .net.
    "%MonoPath%\bin\mono.exe" "%MonoPath%\lib\mono\4.5\pdb2mdb.exe" "%%f"
)

set TargetDir=%LOCALAPPDATA%Low\Volcanoid\Volcanoids\
set LocalModsDir=%TargetDir%LocalMods\

REM Copy to the LocalMods dir, but clean out everything except the `.workshop` file first.
mkdir "%LocalModsDir%%~1\"
for %%a in ("%LocalModsDir%%~1\*") do if /i not "%%~nxa"==".workshop" del "%%a"
copy "%~2*" "%LocalModsDir%%~1\"