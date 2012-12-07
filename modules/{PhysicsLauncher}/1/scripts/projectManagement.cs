//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function persistProject()
{
    PhysicsLauncher::persistProject();
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
    
    if (isWriteableFileName($PhysicsLauncher::WorldListFile))
        TamlWrite($WorldListData, $PhysicsLauncher::WorldListFile);
        
    //---------------------------------------------------------------
    // Tutorials
    //---------------------------------------------------------------
    %tutorialDataFile = expandPath("^PhysicsLauncherTemplate/managed/tutorialData.taml");
    
    if (isWriteableFileName(%tutorialDataFile))
        TamlWrite($TutorialDataSet, %tutorialDataFile);
}

function PhysicsLauncher::initializeProject()
{
    if (isFunction("_loadGameConfigurationData"))
        _loadGameConfigurationData();
        
    $PhysicsLauncher::UserHomeDirectory = getUserHomeDirectory();
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
    $PhysicsLauncher::WorldListFile = $PhysicsLauncher::UserHomeDirectory @ "/My Games/" @ $Game::CompanyName @ "/{PhysicsLauncher}/" @ $Game::ProductName @ "/worldList.taml";
    addResPath($PhysicsLauncher::WorldListFile); 

    if (!isObject($WorldListData) || $WorldDataChanged)
    {
        if (isFile($PhysicsLauncher::WorldListFile))
            $WorldListData = TamlRead($PhysicsLauncher::WorldListFile);

        else
        {
            $WorldListData = TamlRead("^PhysicsLauncherTemplate/managed/worldList.taml");
            createPath($PhysicsLauncher::WorldListFile);
            TamlWrite($WorldListData, $PhysicsLauncher::WorldListFile);
        }
    }

    if (!isObject($WorldListData))
        $WorldListData = new SimSet();

    //---------------------------------------------------------------
    // Tutorials
    //---------------------------------------------------------------
    $PhysicsLauncher::TutorialDataFile = $PhysicsLauncher::UserHomeDirectory @ "/My Games/" @ $Game::CompanyName @ "/{PhysicsLauncher}/" @ $Game::ProductName @ "/tutorialData.taml";
    addResPath($PhysicsLauncher::TutorialDataFile); 

    if (isFile($PhysicsLauncher::TutorialDataFile))
        $TutorialDataSet = TamlRead($PhysicsLauncher::TutorialDataFile);

    else
    {
        $TutorialDataSet = TamlRead("^PhysicsLauncherTemplate/managed/tutorialData.taml");
        createPath($PhysicsLauncher::TutorialDataFile);
        TamlWrite($TutorialDataSet, $PhysicsLauncher::TutorialDataFile);
    }
        
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