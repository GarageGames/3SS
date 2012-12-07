//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

// Declare the Project Target
Projects::DeclareProjectTarget(T2DProject, LBProjectObj);

// If there is no startup project, we are likely to be reloading a project.
// If there is neither startup or last project something else is horribly wrong,
// New project sets the startup project, reload sets the last project and reload = true;
function createProjectResPaths(%extra)
{
    $gamepath = "";

    if (%extra !$= "")
        $pref::startupProject = %extra;

    // If we are reloading, we only care about startup project.
    if ($pref::reloadLastProject)
    {
        $gamepath = filePath($pref::lastProject);
    } 
    else
    {
        // If we arent reloading a project, theres 2 choices. 
        // 1) startup project path is set via cmd line or other method .
        // 2) Load last project preference is set and is autoloadinga project.

        // In the case of 1, this overrides the load last preference anyway.
        // In the case of 2, it doesnt use the startup path as its null.

        if ($pref::startupProject $= "")
        {
            // Likely option 2 above.
            $gamepath = filePath($pref::lastProject);
        }
        else
        {
            // Likely that the startup is lingering, check the pref
            if ($pref::loadLastProject)
                $gamepath = filePath($pref::lastProject);
            else
                $gamepath = filePath($pref::startupProject);
        }
    }

    if ($gamepath !$= "")
        addResPath(expandPath($gamepath));
}

// Call this 
createProjectResPaths("");

// Deal with non-Windows platforms
if ($platform $= "windows")
    $LB::PlayerExecutable = expandPath("^templates/projectFiles/3StepStudioGame.exe");

if ($platform $= "macos")
    $LB::PlayerExecutable = expandPath("^templates/projectFiles/3StepStudioGame.app/Contents/MacOS/3StepStudioGame");

function getProjectNameFromPath()
{
   // As name is not stored but implicit in path we just rip it from there
   %path = LBProjectObj.projectName;
   
   return %path;
   
   if( %path !$= "" )
   {      
      // Replace forward slashes with spaces so we can use getWord but
      // first replace spaces with invalid/unlikely character sequence
      // in case we have spaces in the path or project name and just 
      // restore spaces after (if any). Rather pedantic but this will
      // catch irregular paths and filenames as well as those with spaces.
      %tmp = strreplace( %path, " ", "&$" );
      %tmp = strreplace( %tmp, "/", " " );      
      %cnt = getWordCount( %tmp );   
      %n   = getWord( %tmp, %cnt - 2 );
      %n   = strreplace( %n, "&$", " " );
   }
   else
      %n = "";
   
   return %n;
}

function getGameExecutableFile()
{  
   // Something new, it returns the folder name, the executable is always called 3StepStudioGame.exe by default, so try the prject name, 
   // if that fails try the default.
   %default = "3StepStudioGame";
   %gname = getProjectNameFromPath();      
      
   // Don't look for custom game on mac
   if( $platform !$= "macos" )
   {
      %gfile = expandPath( "^project/" @ %gname @ ".exe" );
     
      echo("Game executable is : " @ %gfile);
         
      // If it exists use it...
      if( isFile( %gfile ) )
      {
         echo("Game executable found, using it.");
         return %gfile;
      }
      else
      {
        // Try the debug default executable.
        %gfile = expandPath( "^project/" @ %default @ "_DEBUG.exe" );
        if ( isFile(%gfile) )
        {
            echo( "Using DEBUG executable." );
            return %gfile;
        }
        
        echo("Not here, using default executable");
        return expandPath( "^project/" @ %default @ ".exe" );
      }
         
   }
   else
   {
        if (isDebugBuild())
            %gfile = expandPath( "^project/" @ %gname @ ".app/Contents/MacOS/3StepStudioGame_Debug" );
        else
            %gfile = expandPath( "^project/" @ %gname @ ".app/Contents/MacOS/3StepStudioGame" );
      
      // If it exists use it...
      if( isFile( %gfile ) )
      {
         echo("Game executable found, using it.");
         return %gfile;
      }
      else
      {
         echo("Not here, using default executable");
         return expandPath("^project/") @ "3StepStudioGame.app/Contents/MacOS/3StepStudioGame";
      }
   }
   
   //Return an error instead, no defaults here yet.
   return "";
}

function runGame()
{
   if(! isObject(LBProjectObj))
   {
      error("runGame - No project loaded");
      return;
   }
 
   //By default not file, And run the correct one on osx. 
   %playerExecutable = ""; 
   
   %playerExecutable = getGameExecutableFile();
   
   %exists = false;
   %exists = isFile( %playerExecutable );
   
   if( %playerExecutable $= "" || %exists == false )
   {
      echo(%playerExecutable @ " is not found? Why? ");
      messageBox("Scene Builder", "Could not find player executable:" NL %playerExecutable, "Ok", "Stop");
      return;
   }
   
   minimizeWindow();
   
   %args = "-project" SPC "\"" @ expandPath("^project/") @ "\"";
   %path = expandPath("^project/");
   shellExecute(%playerExecutable, %args, %path);
}
    
// Save the project
function T2DProject::SaveProject(%this, %projectFile)
{
    %lastLevel = LBProjectObj.currentLevelFile;

    // 201210513:38:59
    %dateTime = stripChars(getCurrentDate(true), "/");
    %date = getSubStr(%dateTime, 0, strstr(%dateTime, ":"));
    %time = getSubStr(%dateTime, strstr(%dateTime, ":")+1, strlen(%dateTime));
    %strippedTime = stripChars(%time, ":");
    
    %project = new ScriptObject()
    {
        type = "TSSProject";
        version = "1";
        creator = "3 Step Studio";
        lastLevel = %lastLevel;
        projectName = %this.projectName;
        templateModule = %this.templateModule;
        sourceModule = %this.sourceModule;
        lastModified = %date @ "." @ %strippedTime;
    };
    
    %writeSuccessful = TamlWrite(%project, %projectFile);
    
    if (!%writeSuccessful)
    {
        error("T2DProject::SaveProject - Failed to write to file: " @ %projectFile);
        return false;
    }
    
    // store the version number
    %this.projectVersion = getEngineVersion();

    return true;
}

// Load the project
function T2DProject::LoadProject(%this, %projectFile)
{
    %project = TamlRead(%projectFile);

    if (%lastLevel !$= "")
        %this.lastLevel = filePath(%projectFile) @ "/" @ %lastLevel;
    else    
        %this.lastLevel = %project.lastLevel;
    
    %this.projectFile = %projectFile;
    %this.projectVersion = %project.version;

    %this.templateModule = "";
    %this.sourceModule = "";
    
    if (%project.templateModule !$= "")
        %this.templateModule = %project.templateModule;

    if (%project.sourceModule !$= "")
        %this.sourceModule = %project.sourceModule;
        
    %this.projectName = %project.projectName;

    $Game::ProductName = %this.projectName;

    %this.lastModified = %project.lastModified;
    
    return true;
}

// Persist managed to disk
function T2DProject::persistToDisk(%this)
{
    %brushesFile = expandPath("^gameTemplate/managed/brushes.taml");
    %validFiles = %this.validateManagedSet($brushSet, "Brushes");
    
    if (%validFiles && isWriteableFileName(%brushesFile))
        TamlWrite($brushSet, %brushesFile);
        
    %tagsFile = expandPath("^gameTemplate/managed/tags.taml");

    if (isWriteableFileName(%tagsFile))
        TamlWrite(ProjectNameTags, %tagsFile);
    
    // Template specific data persistence
    persistProject();
}

// Validate a managed set
function T2DProject::validateManagedSet(%this, %object, %type)
{
    // Must Exist
    if (!isObject(%object)) 
        return false;

    // Correct Token Type?
    if (%object.setType !$= %type)
        return false;

    // Success, it's valid.
    return true;
}

// Force a valid token
function T2DProject::createValidatorToken(%this, %object, %type)
{
    // Must Exist
    if (!isObject(%object)) 
        return false;

    // Tag as valid. - This should be improved at some point - JDD
    %object.setType = %type;

    // Success, it's now valid.
    return true;
}

function T2DProject::LoadProjectData(%this, %data)
{
    // Load the game module
    // This will initialize the behaviors and load template specific data
    ModuleDatabase.scanModules(expandPath("^project/game"));
    ModuleDatabase.loadExplicit(%this.templateModule);
    
    %launcherModule = ModuleDatabase.findModule( %this.templateModule, 1 );
    addPathExpando( %this.templateModule, %launcherModule.ModulePath );
    AssetDatabase.compileReferencedAssets( %launcherModule );
    
    %tagsFile = expandPath("^gameTemplate/managed/tags.taml");
    %brushFile = expandPath("^gameTemplate/managed/brushes.taml");   
    %persistFile = expandEscape("^gameTemplate/managed/persistent.taml");
    
    // Need to know about various data files.
    addResPath(expandPath("^gameTemplate/data"));
   
    if (!isObject($managedDatablockSet))   
        $managedDatablockSet = new SimSet();

    %this.createValidatorToken($managedDatablockSet, "Datablocks");

    //---------------------------------------------------------------------------
    // Project Tags
    //---------------------------------------------------------------------------
    %found = isFile(%tagsFile);
    if (%found || isFile(%tagsFile))
    {
        TamlRead(%tagsFile);

        for(%i = 0; %i < $managedDatablockSet.getCount(); %i++)
        {
            %object = $managedDatablockSet.getObject(%i);

            if (%object.NameTags !$= "")
                ProjectNameTags.add(%object);
        }
    }
    else
    {
        error("% Project tags file missing.");
    }

    //---------------------------------------------------------------------------
    // Managed Brushes
    //---------------------------------------------------------------------------
    if (isFile(%brushFile))
        $brushSet = TamlRead(%brushFile);

    if (!isObject($brushSet))   
        $brushSet = new SimSet();

    // Mark Set Valid   
    %this.createValidatorToken($brushSet, "Brushes");
   
    ////---------------------------------------------------------------------------
    //// Managed Persistent Objects
    ////---------------------------------------------------------------------------
    //if (isFile(%persistFile))
        //$persistentObjectSet = TamlRead(%persistFile);
//
    //if (!isObject($persistentObjectSet))   
        //$persistentObjectSet = new SimSet();
//
    //// Mark Set Valid   
    //%this.createValidatorToken($persistentObjectSet, "Persistent");
}

function T2DProject::validateDatablocks(%this)
{
    %killGroup = new SimGroup();
    %count = $managedDatablockSet.getCount();

    for(%i = %count - 1; %i >= 0; %i--)
    {
        %datablock = $managedDatablockSet.getObject(%i);
    }

    %count = %killGroup.getCount();
    
    if (%count > 0)
    {
        %badString = %killGroup.getObject(0).getName();
        for(%i = 1; %i < %count; %i++)
        %badString = %badString NL %killGroup.getObject(%i).getName();

        %string = getCountString("This project contains {count} invalid datablock{s}.", %count);
        
        messageBox("Invalid Project Data", %string SPC "This is likely because a referenced image file" NL
        "was deleted from the project directory. The invalid datablocks are:\n" NL %badString, "Ok");
    }

    %killGroup.delete();
}

// Datablock Set Manipulators
function T2DProject::addDatablock(%this, %datablock, %persistNow)
{
    if (!isObject($managedDatablockSet))
        return false;

    if (!%this.validateManagedSet($managedDatablockSet, "Datablocks"))
        return false;

    if (!$managedDatablockSet.isMember(%datablock))
        $managedDatablockSet.add(%datablock);

    // Persist if desired
    if (%persistNow == true)
        %this.persistToDisk();

    // Success      
    return true;
}

function T2DProject::removeDatablock(%this, %datablock, %persistNow)
{
    if (!isObject($managedDatablockSet))
        return false;

    if (!%this.validateManagedSet($managedDatablockSet, "Datablocks"))
        return false;

    if ($managedDatablockSet.isMember(%datablock))
        $managedDatablockSet.remove(%datablock);

    // Persist if desired
    if (%persistNow == true)
        %this.persistToDisk();

    // Success      
    return true;
}

function T2DProject::addBrush(%this, %brush, %persistNow)
{
    if (!isObject($brushSet))
        return false;

    if (!%this.validateManagedSet($brushSet, "Brushes"))
        return false;

    if (!$brushSet.isMember(%brush))
        $brushSet.add(%brush);

    if (%persistNow == true)
        %this.persistToDisk();

    return true;
}

function T2DProject::removeBrush(%this, %brush, %persistNow)
{
    if (!isObject($brushSet))
        return false;

    if (!%this.validateManagedSet($brushSet, "Brushes"))
        return false;

    if ($brushSet.isMember(%brush))
        $brushSet.remove(%brush);

    if (%persistNow == true)
        %this.persistToDisk();

    return true;
}

// Persistent Object Set Manipulators
function T2DProject::addPersistent(%this, %datablock, %persistNow)
{
    if (!isObject($persistentObjectSet))
        return false;

    if (!%this.validateManagedSet($persistentObjectSet, "Persistent"))
        return false;

    if (!$persistentObjectSet.isMember(%datablock))
        $persistentObjectSet.add(%datablock);

    // Persist if desired
    if (%persistNow == true)
        %this.persistToDisk();

    // Success      
    return true;

}

function T2DProject::removePersistent(%this, %datablock, %persistNow)
{
    if (!isObject($persistentObjectSet))
        return false;

    if (!%this.validateManagedSet($persistentObjectSet, "Persistent"))
        return false;

    if ($persistentObjectSet.isMember(%datablock))
        $persistentObjectSet.remove(%datablock);

    // Persist if desired
    if (%persistNow == true)
        %this.persistToDisk();

    // Success      
    return true;   
}

//
// Animation Set Manipulators
//
function T2DProject::addAnimationSet(%this, %set, %persistNow)
{
    if (!isObject($animationSets))
        return false;

    if (!%this.validateManagedSet($animationSets, "AnimationSets"))
        return false;

    if (!$animationSets.isMember(%set))
        $animationSets.add(%set);

    // Persist if desired
    if (%persistNow == true)
        %this.persistToDisk();

    // Success      
    return true;
}

function T2DProject::removeAnimationSet(%this, %set, %persistNow)
{
    if (!isObject($animationSets))
        return false;

    if (!%this.validateManagedSet($animationSets, "AnimationSets"))
        return false;

    if ($animationSets.isMember(%set))
    $animationSets.remove(%set);

    // Persist if desired
    if (%persistNow == true)
        %this.persistToDisk();

    // Success      
    return true;
}

function SceneObject::setPersistent(%this, %value)
{
    if (%this.getClassName() $= "TileLayer")
        return;

    if (%value)
        LBProjectObj.addPersistent(%this, true);
    else
        LBProjectObj.removePersistent(%this, true);
}

function SceneObject::getPersistent(%this, %value)
{
    if (!isObject($persistentObjectSet))
        return false;

    if (!LBProjectObj.validateManagedSet($persistentObjectSet, "Persistent"))
        return false;

    if ($persistentObjectSet.isMember(%this))
        return true;

    return false;
}

function T2DProject::onAdd(%this)
{
    Parent::onAdd(%this);

    // Set flag to true to always persist objects when we're deleted
    %this.persistOnRemove = false;

}

function T2DProject::onRemove(%this) 
{   
    // Persist at option
    if (%this.persistOnRemove == true)
        %this.persistToDisk();

    Parent::onRemove(%this);
}

// ProjectOpen Event Handler
// - %data is the project object to be opened 
function T2DProject::onProjectOpen(%this, %data)
{   
    echo("% Opening Project " @ %data @ " ...");

    // Load Project Data
    %this.LoadProjectData(%data);

    return true;
}

/// ProjectClose Event Handler
function T2DProject::onProjectClose(%this, %data)
{
    %module = ModuleDatabase.findLoadedModule(%this.templateModule);
    %toolsGroup = %module.ToolsGroup;
    
    ModuleDatabase.unloadGroup(%toolsGroup);
    ModuleDatabase.unloadGroup(%this.templateModule);
}

// ProjectAddFile Event Handler
function T2DProject::onProjectAddFile(%this, %data)
{
    error("onProjectAddFile Handler not implemented for class -" SPC %this.class);
}

// ProjectRemoveFile Event Handler
function T2DProject::onProjectRemoveFile(%this, %data)
{
    error("onProjectRemoveFile Handler not implemented for class -" SPC %this.class);
}