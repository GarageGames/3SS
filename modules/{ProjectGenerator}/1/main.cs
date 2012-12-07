//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function initializeProjectGenerator()
{
    if(!isObject(ProjectGenerator))
    {
        new ScriptObject(ProjectGenerator);
        
        ProjectGenerator.init();
    }
}

function destroyProjectGenerator()
{
    if(isObject(ProjectGenerator))
    {
        ProjectGenerator.delete();
    }
}

//-----------------------------------------------------------------------------

function ProjectGenerator::init(%this)
{
    if($platform $= "windows")
    {
        %this.app = expandPath("^{ProjectGenerator}/ProjectGenerator.exe");
    }
    else if($platform $= "macos")
    {
        %this.app = expandPath("^{ProjectGenerator}/ProjectGenerator");
    }
}

// Valid %poject values are:
//      iOSDevice
//      iOSSimulator
// An example output is '3StepStudioGame.xcodeproj'
function ProjectGenerator::generateXcodeProject(%this, %project, %output)
{
    // Set up the arguments
    %args = "-xcode -project projects/" @ %project @ ".xml -output " @ %output;
    
    shellExecute(%this.app, %args, expandPath("^{ProjectGenerator}"));
}