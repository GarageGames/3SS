//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function persistProject()
{
    Blackjack::persistProject();
}

function Blackjack::persistProject()
{
    //---------------------------------------------------------------
    // Prefab objects
    //---------------------------------------------------------------
    %prefabsFile = expandPath("^BlackjackTemplate/managed/prefabs.taml");
    addResPath(%prefabsFile); 
    
    if (isWriteableFileName(%prefabsFile))
        TamlWrite($prefabSet, %prefabsFile);
    
    //---------------------------------------------------------------
    // Animation sets
    //---------------------------------------------------------------
    %animationSetsFile = expandPath("^BlackjackTemplate/managed/animationSets.taml");
    
    if (isWriteableFileName(%animationSetsFile))
        TamlWrite($animationSets, %animationSetsFile);
}

function Blackjack::initializeProject()
{
    //_loadGameConfigurationData();
    
    //---------------------------------------------------------------
    // Recursive behavior loading
    //---------------------------------------------------------------
    %behaviorsDirectory = expandPath("^BlackjackTemplate/scripts/behaviors");   
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
    %prefabsFile = expandPath("^BlackjackTemplate/managed/prefabs.taml");
    addResPath(%prefabsFile); 
    
    if (isFile(%prefabsFile))
        $prefabSet = TamlRead(%prefabsFile);
        
    if (!isObject($prefabSet) )   
        $prefabSet = new SimSet();
}

function Blackjack::closeProject()
{
    // Save the default player profile
   if (isObject($DefaultPlayerProfile))
   {
      $DefaultPlayerProfile.updateFromPlayer($UserControlledPlayer);
      $DefaultPlayerProfile.createProfileXMLFile();
   }
   
   if($platform $= "windows" || $platform $= "macos")
      _saveGameConfigurationData();
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