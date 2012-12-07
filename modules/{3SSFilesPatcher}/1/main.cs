//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function initialize3SSFilesPatcher(%scopeSet)
{
    if(!isObject(OSFilesPatcher))
    {
        new ScriptObject(OSFilesPatcher)
        {
            class = "OSFilesPatcherClass";
        };
    }

    %scopeSet.add(OSFilesPatcher);
}

function destroy3SSFilesPatcher()
{
}

// Start building the files manifest
function OSFilesPatcherClass::StartFileManifest(%this, %applicationPath)
{
    %this.manifest = new FileObject();
    %result = %this.manifest.openForWrite("./filesmanifest.txt");

    // Write the header
    %this.manifest.writeLine("Manifest" TAB "1");

    // Write the OS
    if($platform $= "windows")        
        %this.manifest.writeLine("OS" TAB "Windows");
    else if($platform $= "macos")
        %this.manifest.writeLine("OS" TAB "OSX");

    // Write the 3SS app name
    %this.manifest.writeLine("Application" TAB %applicationPath);

    // Reset the file counter
    %this.fileCount = 0;
}

// Finished building the files manifest
function OSFilesPatcherClass::EndFileManifest(%this)
{
    // Write the file count
    %this.manifest.writeLine("Files" TAB %this.fileCount);

    // Write out each file
    for(%i=0; %i<%this.fileCount; %i++)
        %this.manifest.writeLine(%this.sourcePath[%i] TAB %this.destinationPath[%i]);

    %this.manifest.close();
}

// Add a file to the manifest
function OSFilesPatcherClass::AddFile(%this, %sourcePath, %destinationPath)
{
    %this.sourcePath[%this.fileCount] = %sourcePath;
    %this.destinationPath[%this.fileCount] = %destinationPath;

    %this.fileCount++;
}

// Launch the patcher and shut down 3SS
function OSFilesPatcherClass::LaunchPatcher(%this)
{
    if($platform $= "windows")
        %app = "3SSFilesPatcher.exe";
    else if($platform $= "macos")
        %app = "3SSFilesPatcher.app/Contents/MacOS/3SSFilesPatcher";
   
    // Launch the patcher
    %result = QtPlatformManager.LaunchModuleApp("{3SSFilesPatcher}", %app);
    if(!%result)
    {
        error("Could not launch OS file patcher");
        return;
    }

    // Shut down 3SS
    quit();
}
