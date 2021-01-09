REM call $(ProjectDir)..\Base-Mod\Post-Build.bat "$(OutDir)" "$(PkgLib_Harmony)"
REM %1 == $(OutDir)
REM %2 == $(PkgLib_Harmony)

setlocal enabledelayedexpansion

SET MonoPath=C:\Program Files\Unity\Editor\Data\MonoBleedingEdge

for %%f in ("%1*.dll") do (
    echo "Making mdb for %%f"
    REM exe but still needs to be run with mono as the interpreter else it'll wrongly default to .net.
    "%MonoPath%\bin\mono.exe" "%MonoPath%\lib\mono\4.5\pdb2mdb.exe" "%%f"
)

copy "%1\*" "%LOCALAPPDATA%Low\Volcanoid\Volcanoids\Mods\"

if NOT [%2]==[] (
    copy "%2\lib\net45\*" "%LOCALAPPDATA%Low\Volcanoid\Volcanoids\Mods\"
)