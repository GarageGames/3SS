//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function publishIOS( %outputPath, %stagingPath, %executableFilePath )
{
    %gamePath = LBProjectObj.gamePath;
    %buildFilesPath = %gamePath @ "/iOS/";
    %xCodePath = %buildFilesPath @ "3StepStudioGame.xcodeproj";
    %iosDirectory = expandPath("^tool/Templates/iOSFiles/");
    
    %outputDirectory = %buildFilesPath @ "3StepStudioGame/";
    
    ProjectGenerator.generateXcodeProject("iOSDevice", %xCodePath);

    createPath(%outputDirectory);
    pathCopy(%iosDirectory @ "AppDelegate.h", %outputDirectory @ "AppDelegate.h", false);
    pathCopy(%iosDirectory @ "AppDelegate.mm", %outputDirectory @ "AppDelegate.mm", false);
    pathCopy(%iosDirectory @ "main.mm", %outputDirectory @ "main.mm", false);
    pathCopy(%iosDirectory @ "ViewController.h", %outputDirectory @ "ViewController.h", false);
    pathCopy(%iosDirectory @ "ViewController.mm", %outputDirectory @ "ViewController.mm", false);
    pathCopy(%iosDirectory @ "3StepStudio-Info.plist", %outputDirectory @ "3StepStudio-Info.plist", false);
    pathCopy(%iosDirectory @ "3StepStudio-Prefix.pch", %outputDirectory @ "3StepStudio-Prefix.pch", false);
    pathCopy(%iosDirectory @ "en.lproj", %outputDirectory @ "en.lproj", false);
    
    updateIosConfigFile(%outputDirectory @ "en.lproj/Config.xcconfig");
    
    openiOSCodeProject(%xCodePath);
    
    // If we are running a debug build, generate an iOS xcode project with full source
    if (isDebugBuild())
        generateiOSSourceProject();
}

function generateiOSSourceProject()
{
    %gamePath = LBProjectObj.gamePath;
    %buildFilesPath = %gamePath @ "/iOS_Source/";
    %xCodePath = %buildFilesPath @ "3StepStudioGame.xcodeproj";
    %iosDirectory = expandPath("^tool/Templates/iOSFiles/");
    
    %outputDirectory = %buildFilesPath @ "3StepStudioGame/";
    
    ProjectGenerator.generateXcodeProject("iOSSource", %xCodePath);
    
    createPath(%outputDirectory);
    pathCopy(%iosDirectory @ "AppDelegate.h", %outputDirectory @ "AppDelegate.h", false);
    pathCopy(%iosDirectory @ "AppDelegate.mm", %outputDirectory @ "AppDelegate.mm", false);
    pathCopy(%iosDirectory @ "main.mm", %outputDirectory @ "main.mm", false);
    pathCopy(%iosDirectory @ "ViewController.h", %outputDirectory @ "ViewController.h", false);
    pathCopy(%iosDirectory @ "ViewController.mm", %outputDirectory @ "ViewController.mm", false);
    pathCopy(%iosDirectory @ "3StepStudio-Info.plist", %outputDirectory @ "3StepStudio-Info.plist", false);
    pathCopy(%iosDirectory @ "3StepStudio-Prefix.pch", %outputDirectory @ "3StepStudio-Prefix.pch", false);
    pathCopy(%iosDirectory @ "en.lproj", %outputDirectory @ "en.lproj", false);
    
    updateIosConfigFile(%outputDirectory @ "en.lproj/Config.xcconfig");
}

//-----------------------------------------------------------------------------

function openiOSCodeProject(%xCodePath)
{
    if($platform $= "macos")
    {
        %batchFile = expandPath("./openXcode.sh");
        %cmds = expandPath("./openXcode.sh") @ "; " @ %xCodePath;

        echo("Doing : " @ %cmds);

        runBatchFile("sh", %cmds, true);
        echo("Finished running batchfile");   
    }
    else
    {
        messageBox("Error", "Cannot open XCode projects on Windows, Sorry!");
    }
}