call "c:/Program Files/Microsoft Visual Studio 8/Common7/Tools/vsvars32.bat"

@echo off

rem We need to add this to the path so devenv can find cmd.exe when this is called from cygwin.
@set PATH=c:\WINDOWS\system32;%PATH%

echo      - Using normal (slow) builder

IF EXIST "c:/Program Files/Microsoft Visual Studio 8/Common7/IDE/devenv.exe" (GOTO :full) ELSE ECHO devenv is missing, checking for vcexpress

IF EXIST "c:/Program Files/Microsoft Visual Studio 8/Common7/IDE/vcexpress.exe" (GOTO :express)ELSE ECHO vcexpress.exe is missing, exiting

:full
   echo "DEVENV"
   devenv /rebuild "Release" "..\..\engine\compilers\VisualStudio 2005\T2D SDK.sln" /project TorqueGameBuilder
   GOTO end
   
:express
   ECHO "EXPRESS"
   vcexpress /rebuild "Release" "..\..\engine\compilers\VisualStudio 2005\T2D SDK.sln"
   GOTO end

:end
