@echo off

REM Determine the correct Program Files directory

IF EXIST "%ProgramFiles%" SET ROOT=%ProgramFiles%
IF EXIST "%ProgramFiles(x86)%" SET ROOT=%ProgramFiles(x86)%

REM Setup some default build settings

SET BUILDCMD=vcbuild
SET OPTIONS= /useenv "Release|Win32"

REM Detect the presence of IncrediBuild

IF EXIST "%ROOT%\Xoreax\IncrediBuild\BuildConsole.exe" SET BUILDCMD="%ROOT%\Xoreax\IncrediBuild\BuildConsole.exe"
IF EXIST "%ROOT%\Xoreax\IncrediBuild\BuildConsole.exe" SET OPTIONS=/build "Release|Win32"

echo - Setting environment variables
call "%ROOT%\Microsoft Visual Studio 9.0\Common7\Tools\vsvars32.bat"

echo - Building
call %BUILDCMD% "..\..\engine\compilers\VisualStudio 2008\T2D SDK.sln" %OPTIONS%