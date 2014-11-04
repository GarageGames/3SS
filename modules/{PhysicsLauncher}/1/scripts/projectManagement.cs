//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function persistProject()
{
    PhysicsLauncher::persistProject();
}

// Save game data to a file
// %dataSource = The source of the saved game data
function saveGameData(%dataSource)
{
    PhysicsLauncher::saveGameData(%dataSource);
}

// Load game data from the saved file.
// %dataDestination = The destination of the saved game data
function loadGameData(%dataDestination)
{
    PhysicsLauncher::loadGameData(%dataDestination);
}

// Save tutorial data to a file
// %dataSource = The source of the saved tutorial data
function saveTutorialData(%dataSource)
{
    PhysicsLauncher::saveTutorialData(%dataSource);
}

// Load game tutorial from the saved file.
// %dataDestination = The destination of the saved tutorial data
function loadTutorialData(%dataDestination)
{
    PhysicsLauncher::loadTutorialData(%dataDestination);
}

function PhysicsLauncher::persistProject()
{
    //---------------------------------------------------------------
    // Prefab objects
    //---------------------------------------------------------------
    %prefabsFile = expandPath("^PhysicsLauncherTemplate/managed/prefabs.taml");
    
    if (isWriteableFileName(%prefabsFile))
        TamlWrite($prefabSet, %prefabsFile);
    
    //---------------------------------------------------------------
    // World lists
    //---------------------------------------------------------------
    %worldListFile = expandPath("^PhysicsLauncherTemplate/managed/worldList.taml");
    
    if (isWriteableFileName(%worldListFile))
    {
        TamlWrite($WorldListData, %worldListFile);
    }
        
    //---------------------------------------------------------------
    // Tutorials
    //---------------------------------------------------------------
    %tutorialDataFile = expandPath("^PhysicsLauncherTemplate/managed/tutorialData.taml");
    
    if (isWriteableFileName(%tutorialDataFile))
        TamlWrite($TutorialDataSet, %tutorialDataFile);
}

// Extract the game data from the world data and save it
function PhysicsLauncher::saveGameData(%dataSource)
{
    %gameData = new SimSet();
    %gameData.version = 1;  // For future use so we know when the save format changes
    %gameData.rewardCount = %dataSource.rewardCount;
   
    for(%i=0; %i<%dataSource.getCount(); %i++)
    {
        %worldData = %dataSource.getObject(%i);
        if(%worldData.internalName $= "Unused Levels")
        {
            // Skip the special Unused Levels world as it doesn't take
            // part in the game, only the editor.
            continue;
        }
      
        %saveData = new ScriptObject();
        %saveData.internalName = %worldData.internalName;
        %saveData.WorldLevelCount = %worldData.WorldLevelCount;
        %saveData.WorldLocked = %worldData.WorldLocked;
        %saveData.WorldProgress = %worldData.WorldProgress;
      
        // Go through each level that belongs to this world
        for(%j=0; %j<%worldData.WorldLevelCount; %j++)
        {
            %saveData.LevelHighScore[%j] = %worldData.LevelHighScore[%j];
            %saveData.LevelList[%j] = %worldData.LevelList[%j];
            %saveData.LevelLocked[%j] = %worldData.LevelLocked[%j];
            %saveData.LevelStars[%j] = %worldData.LevelStars[%j];
        }
      
        %gameData.add(%saveData);
    }
   
    // Write out the game data
    echo("Saving game data...");
    TamlWrite(%gameData, $PhysicsLauncher::WorldListFile);
}

// Load in the game data and merge with the world data
function PhysicsLauncher::loadGameData(%dataDestination)
{
    // Check if the game data file exists.  If not, then we
    // will just use the existing game data located in the world data.
    if (!isFile($PhysicsLauncher::WorldListFile))
    {
        return;
    }
    
    // Load in the game data
    echo("Loading game data...");
    %gameData = TamlRead($PhysicsLauncher::WorldListFile);
    
    // Check the save data version.  If it is higher than we support then we have
    // no choice but to stop and just use the default world data as the game data.
    if(%gameData.version > 1)
    {
        warn("PhysicsLauncher::loadGameData(): Game data version " @ %gameData.version @ " doesn't match game version 1. Using default data.");
        return;
    }
    
    // Set game level variables
    %dataDestination.rewardCount = %gameData.rewardCount;
    
    // Set up world game data
    for(%i=0; %i<%gameData.getCount(); %i++)
    {
        // Get the world's saved game data
        %saveData = %gameData.getObject(%i);
        
        // Find the saved world in the game's defined world list
        %worldData = %dataDestination.findObjectByInternalName(%saveData.internalName);
        if(!isObject(%worldData))
        {
            // World exists in game save data but is not part of the current game.
            // We have no choice but to skip this save data.
            continue;
        }
        
        // Transfer world data
        %worldData.WorldLocked = %saveData.WorldLocked;
        %worldData.WorldProgress = (%saveData.WorldProgress > %worldData.WorldLevelCount) ? %worldData.WorldLevelCount : %saveData.WorldProgress;

        // Transfer world level data
        for(%j=0; %j<%worldData.WorldLevelCount; %j++)
        {
            // There could be more levels defined for this world than have
            // been previously saved.  Skip trying to transfer over levels from
            // the save data that doesn't exist.
            if(%j >= %saveData.WorldLevelCount)
            {
                continue;
            }
            
            %worldData.LevelHighScore[%j] = %saveData.LevelHighScore[%j];
            %worldData.LevelList[%j] = %saveData.LevelList[%j];
            %worldData.LevelLocked[%j] = %saveData.LevelLocked[%j];
            %worldData.LevelStars[%j] = %saveData.LevelStars[%j];
        }
    }
}

// Extract the tutorial data from the world data and save it
function PhysicsLauncher::saveTutorialData(%dataSource)
{
    %tutorialData = new SimSet();
    %tutorialData.version = 1;  // For future use so we know when the save format changes
   
    for(%i=0; %i<%dataSource.getCount(); %i++)
    {
        %tutorialObject = %dataSource.getObject(%i);
      
        %saveData = new ScriptObject();
        %saveData.internalName = %tutorialObject.internalName;
        %saveData.TutorialRead = %tutorialObject.TutorialRead;
      
        %tutorialData.add(%saveData);
    }
   
    // Write out the tutorial data
    echo("Saving tutorial data...");
    TamlWrite(%tutorialData, $PhysicsLauncher::TutorialDataFile);
}

// Load in the game tutorial and merge with the world data
function PhysicsLauncher::loadTutorialData(%dataDestination)
{
    // Check if the game data file exists.  If not, then we
    // will just use the existing game data located in the world data.
    if (!isFile($PhysicsLauncher::TutorialDataFile))
    {
        return;
    }
    
    // Load in the tutorial data
    echo("Loading tutorial data...");
    %tutorialData = TamlRead($PhysicsLauncher::TutorialDataFile);
    
    // Check the save data version.  If it is higher than we support then we have
    // no choice but to stop and just use the default tutorial data.
    if(%tutorialData.version > 1)
    {
        warn("PhysicsLauncher::loadTutorialData(): Tutorial data version " @ %tutorialData.version @ " doesn't match game version 1. Using default data.");
        return;
    }
    
    // Set up tutorial data
    for(%i=0; %i<%tutorialData.getCount(); %i++)
    {
        // Get the saved tutorial data
        %saveData = %tutorialData.getObject(%i);
        
        // Find the saved tutorial in the game's defined tutorial list
        %tutorialObject = %dataDestination.findObjectByInternalName(%saveData.internalName);
        if(!isObject(%tutorialObject))
        {
            // Tutorial exists in game save data but is not part of the current game.
            // We have no choice but to skip this save data.
            continue;
        }
        
        // Transfer tutorial data
        %tutorialObject.WorldLocked = %saveData.TutorialRead;
    }
}

function PhysicsLauncher::initializeProject()
{
    if (isFunction("_loadGameConfigurationData"))
        _loadGameConfigurationData();
        
    $PhysicsLauncher::UserHomeDirectory = expandPath("");
    //---------------------------------------------------------------
    // Recursive behavior loading
    //---------------------------------------------------------------
    %behaviorsDirectory = expandPath("^PhysicsLauncherTemplate/scripts/behaviors");   
    addResPath(%behaviorsDirectory);

    // Compile all the cs files.
    %behaviorsSpec = %behaviorsDirectory @ "/*.cs";
    for (%file = findFirstFile(%behaviorsSpec); %file !$= ""; %file = findNextFile(%behaviorsSpec))
        compile(%file);

    // And exec all the dsos.
    %behaviorsSpec = %behaviorsDirectory @ "/*.cs.dso";
    for (%file = findFirstFile(%behaviorsSpec); %file !$= ""; %file = findNextFile(%behaviorsSpec))
        exec(strreplace(%file, ".cs.dso", ".cs"));
                
    //---------------------------------------------------------------
    // Prefab objects
    //---------------------------------------------------------------
    %prefabsFile = expandPath("^PhysicsLauncherTemplate/managed/prefabs.taml");
    addResPath(%prefabsFile); 
    
    if (isFile(%prefabsFile))
        $prefabSet = TamlRead(%prefabsFile);
        
    if (!isObject($prefabSet) )   
        $prefabSet = new SimSet();
        
    //---------------------------------------------------------------
    // World lists
    //---------------------------------------------------------------
    $PhysicsLauncher::WorldListFile = $PhysicsLauncher::UserHomeDirectory @ "/Save Files/" @ $Game::CompanyName @ "/{PhysicsLauncher}/" @ $Game::ProductName @ "/worldList.taml";
    addResPath($PhysicsLauncher::WorldListFile); 

    if (!isObject($WorldListData) || $WorldDataChanged)
    {
        $WorldListData = TamlRead("^PhysicsLauncherTemplate/managed/worldList.taml");
        loadGameData($WorldListData);
    }

    if (!isObject($WorldListData))
        $WorldListData = new SimSet();

    //---------------------------------------------------------------
    // Tutorials
    //---------------------------------------------------------------
    $PhysicsLauncher::TutorialDataFile = $PhysicsLauncher::UserHomeDirectory @ "/Save Files/" @ $Game::CompanyName @ "/{PhysicsLauncher}/" @ $Game::ProductName @ "/tutorialData.taml";
    addResPath($PhysicsLauncher::TutorialDataFile); 

    $TutorialDataSet = TamlRead("^PhysicsLauncherTemplate/managed/tutorialData.taml");
    loadTutorialData($TutorialDataSet);
        
    if (!isObject($TutorialDataSet) )   
        $TutorialDataSet = new SimSet();
}

function PhysicsLauncher::closeProject()
{
    // Currently bugged. Need to fix the loop
    echo("% - Cleaning up project datablocks and sets");
    return;
    
    if ($LevelEditorActive)
    {
        while($WorldListData.getCount())
        {
            %world = $WorldListData.getObject(0);
            
            %world.delete();
        }
        
        while($TutorialDataSet.getCount())
        {
            %tutorial = $TutorialDataSet.getObject(0);
            
            %tutorial.delete();
        }
    }
}

// Save config data
function _saveGameConfigurationData(%projectFile)
{
    export("$Game::*", "^gameTemplate/scripts/prefs.cs", true, false);
    export("$pref::iOS::ScreenOrientation", "^gameTemplate/scripts/prefs.cs", true, true);
    export("$pref::iOS::ScreenDepth", "^gameTemplate/scripts/prefs.cs", true, true);
    export("$pref::iOS::UseGameKit", "^gameTemplate/scripts/prefs.cs", true, true);
    export("$pref::iOS::UseMusic", "^gameTemplate/scripts/prefs.cs", true, true);
    export("$pref::iOS::UseMoviePlayer", "^gameTemplate/scripts/prefs.cs", true, true);
    export("$pref::iOS::UseAutoRotate", "^gameTemplate/scripts/prefs.cs", true, true);
    export("$pref::iOS::EnableOrientationRotation", "^gameTemplate/scripts/prefs.cs", true, true);
    export("$pref::iOS::StatusBarType", "^gameTemplate/scripts/prefs.cs", true, true);
    export("$pref::Audio::*", "^gameTemplate/scripts/prefs.cs", true, true);
    export("$pref::T2D::particleEngineQuantityScale", "^gameTemplate/scripts/prefs.cs", true, true);
    export("$pref::T2D::warnFileDeprecated", "^gameTemplate/scripts/prefs.cs", true, true);
    export("$pref::T2D::warnSceneOccupancy", "^gameTemplate/scripts/prefs.cs", true, true);
    export("$pref::OpenGL::*", "^gameTemplate/scripts/prefs.cs", true, true);
    export("$pref::Video::fullScreen", "^gameTemplate/scripts/prefs.cs", true, true);
    export("$pref::Video::defaultResolution", "^gameTemplate/scripts/prefs.cs", true, true);
}