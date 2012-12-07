//-----------------------------------------------------------------------------
// Three Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

// This variable determines whether the scene in the level file or the existing scene
// should be used.
$useNewScene = false;

//------------------------------------------------------------------------------
// SceneWindow.loadLevel
// Loads a level file into a scene window.
//------------------------------------------------------------------------------
function SceneWindow::loadLevel(%sceneWindow, %levelFile)
{
    // Clean up any previously loaded stuff.
    %sceneWindow.endLevel();

    // Load the level.
    $useNewScene = true;

    alxStopAll();//kill any leftovers
    $TitleMusicHandle = "";
    if ($TitleMusicSchedule)
        cancel($TitleMusicSchedule);

    //-Mat loadlevel stuff can go here for now
    hideCursor();

    //now get level specific resources 
    //Get just filename, then strip out extension         -Mat
    %levelFileName = fileName(%levelFile);
    %levelFileName = strreplace(%levelFileName, ".scene.taml", "");

    $CurrentLevel = %levelFileName;
   
    %newLevelFile = %levelFile;
    echo("--------Adding to Level" SPC %newlevelfile);//-Mat iPhone debug output	   

    //Get a datablock file name for this level - Sven
    %dbFileName = %newlevelfile;
    %dbFileName = strreplace(%dbFileName, %levelFileName @ ".scene.taml", "datablocks/" @ %levelFileName @ "_datablocks.taml");   

    //load level         -Mat
    %scene = %sceneWindow.addToLevel(%newlevelfile, %dbFileName);

    //Load any level specific GUI
    %LevelGUIfunction = "load" @ %levelFileName @ "GUI";
    
    //call the function with no parameters
    if (isFunction(%LevelGUIfunction) )
        eval(%LevelGUIfunction @ "();");
   
    if (!isObject(%scene))
        return 0;
   
    %sceneWindow.setScene(%scene);

    // Set the window properties from the scene if they are available.
    %cameraPosition = %sceneWindow.getCurrentCameraPosition();
    %cameraSize = Vector2Sub(getWords(%sceneWindow.getCurrentCameraArea(), 2, 3), getWords(%sceneWindow.getCurrentCameraArea(), 0, 1));

    if (%scene.cameraPosition !$= "")
        %cameraPosition = %scene.cameraPosition;
    if (%scene.cameraSize !$= "")
        %cameraSize = %scene.cameraSize;
      
    %sceneWindow.setCurrentCameraPosition(%cameraPosition, %cameraSize);
   
    // Only perform "onLevelLoaded" callbacks when we're NOT editing a level
    // This is so that objects that may have script associated with them that
    // the level builder cannot undo will not be executed while using the tool.
          
    if (!$LevelEditorActive)
    {
        // Notify the scene that it was loaded.
        if (%scene.isMethod("onLevelLoaded") )
            %scene.onLevelLoaded(%levelFile);

        // And finally, notify all the objects that they were loaded.
        %sceneObjectList = %scene.getSceneObjectList();
        %sceneObjectCount = getWordCount(%sceneObjectList);
        for (%i = 0; %i < %sceneObjectCount; %i++)
        {
            %sceneObject = getWord(%sceneObjectList, %i);
            
            %sceneObject.onLevelLoaded(%scene);
        }
        
        GameEventManager.postEvent("_LevelLoaded", %scene);
    }

    if (!isObject(%sceneWindow.lockedObjectSet))
        %sceneWindow.lockedObjectSet = new SimSet();   

    $lastLoadedScene = %scene;

    return %scene;
}

//------------------------------------------------------------------------------
// SceneWindow.addToLevel
// Adds a level file to a scene window's scene.
//------------------------------------------------------------------------------
function SceneWindow::addToLevel(%sceneWindow, %levelFile, %dbFileName)
{
    %scene = %sceneWindow.getScene();
    if (!isObject(%scene))
    {
        %scene = new Scene();
        %sceneWindow.setScene(%scene);
    }

    %newScene = %scene.addToLevel(%levelFile, %dbFileName);

    $lastLoadedScene = %newScene;
    return %newScene;
}



//------------------------------------------------------------------------------
// SceneWindow.endLevel
// Clears a scene window.
//------------------------------------------------------------------------------
function SceneWindow::endLevel(%sceneWindow)
{
    %scene = %sceneWindow.getScene();

    if (!isObject(%scene))
        return;

    %scene.endLevel();

    if (isObject(%this.lockedObjectSet))
        %this.lockedObjectSet.clear();

    if (isObject(%scene))
    {
        if (isObject(%scene.getGlobalTileMap()) )
            %scene.getGlobalTileMap().delete();

        %scene.delete();
    }

    $lastLoadedScene = "";
    ScheduleManager.initialize();
}


function SceneWindow::addLockedObject(%this, %objectID)
{
    %this.lockedObjectSet.add(%objectID);
}

function SceneWindow::removeLockedObject(%this, %objectID)
{
    if (%this.lockedObjectSet.isMember(%objectID))
        %this.lockedObjectSet.remove(%objectID);
}

function SceneWindow::onTouchDown(%this, %touchID, %worldPos)
{
    // Retract the Projectile Selection menu if it's currently expanded
    ProjectileSlotContainer.setProjectileSelectionMenuVisiblity(false);
    
    if (!isObject(%this.lockedObjectSet))
        return;  
        
    for(%i = 0; %i < %this.lockedObjectSet.getCount(); %i++)
    {
      %object = %this.lockedObjectSet.getObject(%i);
      
      if (%object.isMethod(onTouchDown))
        %object.onTouchDown(%touchID, %worldPos);
    }
    
    Game::processTouchDown(%touchId, %worldPos);
}

function SceneWindow::onTouchUp(%this, %touchID, %worldPos)
{
    if (!isObject(%this.lockedObjectSet))
        return;  
        
    for(%i = 0; %i < %this.lockedObjectSet.getCount(); %i++)
    {
      %object = %this.lockedObjectSet.getObject(%i);
      
      if (%object.isMethod(onTouchUp))
        %object.onTouchUp(%touchID, %worldPos);
    }
    
    Game::processTouchUp(%touchId, %worldPos);
}

function SceneWindow::onTouchMoved(%this, %touchID, %worldPos)
{
    if (!isObject(%this.lockedObjectSet))
        return;    
    
    for(%i = 0; %i < %this.lockedObjectSet.getCount(); %i++)
    {
        %object = %this.lockedObjectSet.getObject(%i);

        if (%object.isMethod(onTouchMoved))
            %object.onTouchMoved(%touchID, %worldPos);
    }
    
    Game::processTouchMove(%touchId, %worldPos);
}

function SceneWindow::onTouchDragged(%this, %touchID, %worldPos)
{
    if (!isObject(%this.lockedObjectSet))
        return;     
    
    for(%i = 0; %i < %this.lockedObjectSet.getCount(); %i++)
    {
        %object = %this.lockedObjectSet.getObject(%i);

        if (%object.isMethod(onTouchDragged))
            %object.onTouchDragged(%touchID, %worldPos);
    }
    
    Game::processTouchDrag(%touchId, %worldPos);
}