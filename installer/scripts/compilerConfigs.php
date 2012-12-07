<?php

// Determine the proper Program Files folder
$PROGRAMROOT = getenv("ProgramFiles");

if (file_exists(getenv("ProgramFiles(x86)")))
   $PROGRAMROOT = getenv("ProgramFiles(x86)");

echo("Program Files = ".$PROGRAMROOT);

// Is IncrediBuild available?
//$INCREDIBUILD_AVAILABLE = file_exists($PROGRAMROOT."\\Xoreax\\IncrediBuild\\BuildConsole.exe");

$INCREDIBUILD_AVAILABLE = false;

// Setup compiler info arrays
$VS2005 = array();
$VS2005['envvar'] = "\"".$PROGRAMROOT."\\Microsoft Visual Studio 8\\VC\\vcvarsall.bat\"";
$VS2005['buildcmd'] = "vcbuild";
$VS2005['options'] = '/useenv "Release|Win32"';
$VS2005['builddir'] = "VisualStudio 2005";

$VS2008 = array();
$VS2008['envvar'] = "\"".$PROGRAMROOT."\\Microsoft Visual Studio 9.0\\VC\\vcvarsall.bat\"";
$VS2008['buildcmd'] = "vcbuild";
$VS2008['options'] = '/useenv "Release|Win32"';
$VS2008['builddir'] = "VisualStudio 2008";

$XCODE = array();
$XCODE['buildcmd'] = "xcodebuild";
$XCODE['options'] = "-buildstyle Release";
$XCODE['builddir'] = "Xcode";

// Enable IncrediBuild if it is available
if ($INCREDIBUILD_AVAILABLE)
{
   $COMPILER = 'VS2008';
   $VS2008['buildcmd'] = "\"".$PROGRAMROOT."Xoreax\\IncrediBuild\\BuildConsole.exe\"";
   $VS2008['options'] = '/build "Release|Win32"';
}

$COMPILERS = array();
$COMPILERS['VS2005'] = $VS2005;
$COMPILERS['VS2008'] = $VS2008;
$COMPILERS['XCODE'] = $XCODE;

?>