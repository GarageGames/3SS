//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

/*
PackageMaker command line help
packagemaker --root | -r root-path
                 --doc | -d pmdoc-path
                 --out | -o destination-path
                 --id | -i package-identifier
                 --version | -n version
                 --title | -t title
                 --install-to | -l install-to-path
                 --info | -f info-path
                 --resources | -e resources-path
                 --scripts | -s scripts-path
                 --certificate | -c certificate-name
                 (--filter | -x regular-expression)*
                 (--target | -g 10.5 | 10.4 | 10.3)*
                 (--domain | -h system | user | anywhere)*
                 --verbose | -v
                 --no-recommend | -m
                 --root-volume-only | -b
                 --discard-forks | -k
                 --temp-root

Creating a snapshot package:
    packagemaker --watch [normal options, except --doc and --root]

Signing an existing flat package/metapackage:
    packagemaker --sign path-to-pkg
                 --certificate | -c certificate-name
                 [--out | -o] destination-path

Backwards compatible mode:
    packagemaker -build -proj pmproj-path
                        -p destination-path
                        [-b build-path]
                        [-s] [-ds] [-v] [-u]

    packagemaker -build -f root-path
                        -p destination-path
                        -i info-plist-path
                        [-r resources-path]
                        [-d description-plist-path]
                        [-b build-path]
                        [-s] [-ds] [-v] [-u]

    packagemaker -build -mi | -mc | -ms packages-directory-patch
                        -p destination-path
                        -i info-plist-path
                        [-r resources-path]
                        [-d description-plist-path]
                        [-b build-path]
                        [-v]
*/

function publishOSX( %outputPath, %stagingPath, %executableFilePath, %gameName )
{
    // Generate the UUID for the package
    %uuid = generatePackageUUID();
    %packageMakerPath = expandPath("^{PackageMaker}/PackageMaker/PackageMaker.app/Contents/MacOS/");
    %installPath = "/";
    %output = %outputPath @ "/" @ %gameName @ ".pkg";
    %identifier = "com.usergame." @ %gameName;
    
    // Create OS X file structure
    %path = createOSXInstallDirectories(%stagingPath, %gameName);
    
    if (%path $= "")
    {
        error("% - createOSXInstallDirectories failed. Check console log for messages");
        return;
    }
    
    // Apple...enough said
    %packageArgs = "\"-r\"" SPC "\"" @ %path @ "\"" SPC "\"-i\"" SPC "\"" @ %identifier @ "\"" SPC "\"-o\"" SPC "\"" @ %output @ "\"" SPC "\"-t\"" SPC "\"" @ %gameName @ "\"" SPC "\"-l\"" SPC "\"" @ %installPath @ "\"" SPC "\"-n\" \"1.0\" \"-w\"";

    //echo("@@@ PackageMaker arguments:" SPC %packageArgs);

    shellExecuteBlocking( %packageMakerPath @ "PackageMaker", %packageArgs, %packageMakerPath );
    
    // Do we have a staging path?
    if (%path !$= "")
    {
        // Yes, so delete it.
        directoryDelete(%path);
    }
    
    // If we are running a debug build, generate an Xcode project with full source
    if (isDebugBuild())
    {
        %gamePath = LBProjectObj.gamePath;
        %buildFilesPath = %gamePath @ "/OSX/";
        %xCodePath = %buildFilesPath @ "3StepStudioGame.xcodeproj";
        ProjectGenerator.generateXcodeProject("OSXSource", %xCodePath);
    }
}

function createOSXInstallDirectories(%stagingPath, %gameName)
{
    // Base folder is always /
    %root = expandPath(%stagingPath @ "/../") @ "/:";
    
    echo("% - New root is: " @ %root);
    
    // Applications are always /Applications
    %applicationsPath = %root @ "/Applications/" @ %gameName;
    
    // Best font installation is /Library/Fonts
    %fontsPath = %root @ "/Library/Fonts";
    
    // Copy staging contents
    pathCopy(%stagingPath, %applicationsPath, false);
    
    // Copy fonts
    copyOSXInstallerFonts(%fontsPath);
    
    return %root;
}

function copyOSXInstallerFonts(%path)
{
    echo(" % - Now copying fonts for OS X installer: " @ %path);
    
    // Fetch font count.
    %fontCount = AvailableFonts.getCount();

    // Iterate fonts.
    for (%fontIndex = 0; %fontIndex < %fontCount; %fontIndex++)
    {    
        // Fetch font object.
        %font = AvailableFonts.getObject(%fontIndex);
 
        %fromFile = expandPath(%font.File);
        %toFile = expandPath(%path) @ "/" @ fileName(%font.File);
        
        echo("% - Copying " @ %fromFile @ " to " @ %toFile);
        pathCopy(%fromFile, %toFile, true);
        
        if (!isFile(%toFile))
        {
            error("% - Error copying " @ %toFile @ " for OS X publisher");
        }
    }
}

function generatePackageUUID()
{
    return strupr( strreplace( createUUID(), "-", "" ) );
}

function generatePmdoc(%outputPath, %stagingPath)
{
}

function generatePackageOutput(%path)
{
}
function generatePackageOutputContents(%path)
{
}
function generatePackageIndex(%path)
{
}