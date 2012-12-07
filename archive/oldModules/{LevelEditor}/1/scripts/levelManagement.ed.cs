//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

$currentScene = "";

$T2D::LevelSpec = "TGB Scene Files (*.t2d)|*.t2d|All Files (*.*)|*.*|";

//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

// This holds the last scene that was loaded.
$lastLoadedScene = "";
function getLastLoadedScene()
{
   return $lastLoadedScene;
}

// This variable determines whether the scene in the level file or the existing scene
// should be used.
$useNewScene = false;
//---------------------------------------------------------------------------------------------
// SceneWindow.loadLevel
// Loads a level file into a scene window.
//---------------------------------------------------------------------------------------------
function SceneWindow::loadLevel(%sceneWindow, %levelFile)
{
   
   // Clean up any previously loaded stuff.
   %sceneWindow.endLevel();
   
   // Load the level.
   $useNewScene = true;
   %scene = %sceneWindow.addToLevel(%levelFile);
   
   if (!isObject(%scene))
      return 0;
   
   %sceneWindow.setScene(%scene);
   
   // Set the window properties from the scene if they are available.
   %cameraPosition = %sceneWindow.getCurrentCameraPosition();
   
   //sventodo, use the prefs for resolution
   %cameraSize = Vector2Sub(getWords(%sceneWindow.getCurrentCameraArea(), 2, 3),
                              getWords(%sceneWindow.getCurrentCameraArea(), 0, 1));
                              
   if (%scene.cameraPosition !$= "")
      %cameraPosition = %scene.cameraPosition;
   if (%scene.cameraSize !$= "")
      %cameraSize = %scene.cameraSize;
      
   %sceneWindow.setCurrentCameraPosition(%cameraPosition, %cameraSize);
   
   // Only perform "onLevelLoaded" callbacks when we're NOT editing a level
   //
   //  This is so that objects that may have script associated with them that
   //  the level builder cannot undo will not be executed while using the tool.
   //
          
   if (!$LevelEditorActive)
   {
      // Notify the scene that it was loaded.
      if( %scene.isMethod( "onLevelLoaded" ) )
         %scene.onLevelLoaded();
      
      // And finally, notify all the objects that they were loaded.
      %sceneObjectList = %scene.getSceneObjectList();
      %sceneObjectCount = getWordCount(%sceneObjectList);
      for (%i = 0; %i < %sceneObjectCount; %i++)
      {
         %sceneObject = getWord(%sceneObjectList, %i);
         if( %sceneObject.isMethod( "onLevelLoaded" ) )
            %sceneObject.onLevelLoaded(%scene);
      }
   }
   
   $lastLoadedScene = %scene;
   return %scene;
}

//---------------------------------------------------------------------------------------------
// SceneWindow.addToLevel
// Adds a level file to a scene window's scene.
//---------------------------------------------------------------------------------------------
function SceneWindow::addToLevel(%sceneWindow, %levelFile)
{
   %scene = %sceneWindow.getScene();
   if (!isObject(%scene))
   {
      %scene = new Scene();
      %sceneWindow.setScene(%scene);
   }
   
   %newScene = %scene.addToLevel(%levelFile);
   
   $lastLoadedScene = %newScene;
   return %newScene;
}

//---------------------------------------------------------------------------------------------
// Scene.addToLevel
// Loads a level file into a scene.
//---------------------------------------------------------------------------------------------
function Scene::loadLevel(%scene, %levelFile)
{
   
   %scene.endLevel();
   %newScene = %scene.addToLevel(%levelFile);
   
   if (isObject(%newScene) && !$LevelEditorActive)
   {
      // Notify the scene that it was loaded.
      if( %newScene.isMethod( "onLevelLoaded" ) )
         %newScene.onLevelLoaded();
      
      // And finally, notify all the objects that they were loaded.
      %sceneObjectList = %newScene.getSceneObjectList();
      %sceneObjectCount = getWordCount(%sceneObjectList);
      for (%i = 0; %i < %sceneObjectCount; %i++)
      {
         %sceneObject = getWord(%sceneObjectList, %i);
         if( %sceneObject.isMethod( "onLevelLoaded" ) )
            %sceneObject.onLevelLoaded(%newScene);
      }
   }
   
   $lastLoadedScene = %newScene;
   return %newScene;
}

//---------------------------------------------------------------------------------------------
// Scene.addToLevel
// Adds a level file to a scene.
//---------------------------------------------------------------------------------------------
function Scene::addToLevel(%scene, %levelFile)
{
   // Reset this. It should always be false unless we are loading into a scenewindow.
   %useNewScene = $useNewScene;
   $useNewScene = false;
   
   // Prevent name clashes when loading a scene with the same name as this one
   %scene = %scene.getId();
   
   // Make sure the file is valid.
   if ((!isFile(%levelFile)) && (!isFile(%levelFile @ ".dso")))
   {
      error("Error loading level " @ %levelFile @ ". Invalid file.");
      return 0;
   }
   
   // Load up the level.
   %levelContent = TamlRead(%levelFile);
   
   // The level file should have contained a scene, which should now be in the instant
   // group. And, it should be the only thing in the group.
   if (!isObject(%levelContent))
   {
      error("Invalid level file specified: " @ %levelFile);
      return 0;
   }
   
   %newScene = %scene;
   %object = %levelContent;
   $LevelManagement::newObjects = "";
   
   if (%object.getClassName() $= "SceneObjectGroup")
   {
      // Get the next available mount ID  [KNM | 08/10/11 | ITGB-147]
      %nextMountID = %newScene.setMountIDs();
      
      %newScene.addToScene(%object);
      
      // Update clipboard mount IDs before pasting  [KNM | 08/10/11 | ITGB-147]
      updateClipboardMountIDs(%object, %nextMountID);
      
      for (%i = 0; %i < %object.getCount(); %i++)
      {
         %obj = %object.getObject(%i);
         if (%obj.getClassName() $= "ParticleEffect")
         {
            %oldPosition = %obj.getPosition();
            %oldSize = %obj.getSize();
            %obj.loadEffect(%obj.effectFile);
            %obj.setPosition(%oldPosition);
            %obj.setSize(%oldSize);
            %obj.playEffect();
         }
         
         else if (%obj.getClassName() $= "TileLayer")
         {
            %oldPosition = %obj.getPosition();
            %oldSize = %obj.getSize();
            %tileMap = %newScene.getGlobalTileMap();
            if (isObject(%tileMap))
            {
               %tileMap.addTileLayer(%obj);
               %obj.loadTileLayer(%obj.layerFile);
               %obj.setPosition(%oldPosition);
               %obj.setSize(%oldSize);
            }
            else
               error("Unable to find scene's global tile map.");
         }
      }
      $LevelManagement::newObjects = %object;
   }
   
   else if( %object.getClassName() $= "SceneObjectSet" )
   {
      // Get the next available mount ID  [KNM | 08/10/11 | ITGB-147]
      %nextMountID = %newScene.setMountIDs();
      
      // Update clipboard mount IDs before pasting  [KNM | 08/10/11 | ITGB-147]
      updateClipboardMountIDs(%object, %nextMountID);
      
      // Add each object in the set to the scene.
      for (%i = 0; %i < %object.getCount(); %i++)
      {
         %obj = %object.getObject(%i);
         %newScene.addToScene( %obj );
         if (%obj.getClassName() $= "ParticleEffect")
         {
            %oldPosition = %obj.getPosition();
            %oldSize = %obj.getSize();
            %obj.loadEffect(%obj.effectFile);
            %obj.setPosition(%oldPosition);
            %obj.setSize(%oldSize);
            %obj.playEffect();
         }
         
         else if (%obj.getClassName() $= "TileLayer")
         {
            %oldPosition = %obj.getPosition();
            %oldSize = %obj.getSize();
            %tileMap = %newScene.getGlobalTileMap();
            if (isObject(%tileMap))
            {
               %tileMap.addTileLayer(%obj);
               %obj.loadTileLayer(%obj.layerFile);
               %obj.setPosition(%oldPosition);
               %obj.setSize(%oldSize);
            }
            else
               error("Unable to find scene's global tile map.");
         }
      }
      $LevelManagement::newObjects = %object;
   }
   
   else if (%object.isMemberOfClass("SceneObject"))
   {
      if (%object.getClassName() $= "ParticleEffect")
      {
         %newScene.addToScene(%object);
         %oldPosition = %object.getPosition();
         %oldSize = %object.getSize();
         %object.loadEffect(%object.effectFile);
         %object.setPosition(%oldPosition);
         %object.setSize(%oldSize);
         %object.playEffect();
      }
      
      else if (%object.getClassName() $= "TileLayer")
      {
         %oldPosition = %object.getPosition();
         %oldSize = %object.getSize();
         %tileMap = %newScene.getGlobalTileMap();
         if (isObject(%tileMap))
         {
            %tileMap.addTileLayer(%object);
            %object.loadTileLayer(%object.layerFile);
            %object.setPosition(%oldPosition);
            %object.setSize(%oldSize);
         }
         else
            error("Unable to find scene's global tile map.");
      }
      else
         %newScene.addToScene(%object);
      
      $LevelManagement::newObjects = %object;
   }

   // If we got a scene...
   else if (%object.getClassName() $= "Scene")
   {
      %fromScene = 0;
      %toScene = 0;
      
      // If we are supposed to use the new scene, we need to copy from the existing scene
      // to the new one. Otherwise, we copy the loaded stuff into the existing one.
      if (%useNewScene)
      {
         %fromScene = %newScene;
         %toScene = %object;
      }
      else
      {
         %fromScene = %object;
         %toScene = %newScene;
      }

      if (isObject(%fromScene.getGlobalTileMap()))
         %fromScene.getGlobalTileMap().delete();

      // If the existing scene has objects in it, then the new stuff should probably be
      // organized nicely in its own group.
      if ((%toScene.getCount() > 0) && (%fromScene.getCount() > 0))
      {
         %newGroup = new SceneObjectGroup();
      
         while (%fromScene.getCount() > 0)
         {
            %obj = %fromScene.getObject(0);
            %fromScene.removeFromScene(%obj);
            %obj.setPosition(%obj.getPosition()); // This sets physics.dirty.... =)
            %newGroup.add(%obj);
         }
         
         %toScene.add(%newGroup);
         $LevelManagement::newObjects = %newGroup;
      }
      else
      // if it does not then simply move the objects over
      {
         while (%fromScene.getCount() > 0)
         {
            %obj = %fromScene.getObject(0);
            %fromScene.removeFromScene(%obj);
            %obj.setPosition(%obj.getPosition()); // This sets physics.dirty.... =)
            %toScene.addToScene(%obj);
         }
         $LevelManagement::newObjects = %toScene;
      }
      
      %newScene = %toScene;
      %fromScene.delete();
   }
   
   // Unsupported object type.
   else
   {
      error("Error loading level " @ %levelFile @ ". " @ %object.getClassName() @
            " is not a valid level object type.");
      return 0;
   }
   
   if( isObject( $persistentObjectSet ) )
   {
      // Now we need to move all the persistent objects into the scene.
      %count = $persistentObjectSet.getCount();
      for (%i = 0; %i < %count; %i++)
      {
         %object = $persistentObjectSet.getObject(%i);
         %sg = %object.getScene();
         if(%sg)
            %sg.removeFromScene(%object);
            
         %newScene.addToScene(%object);
         %object.setPosition( %object.getPosition() );
      }
   }
   
   $lastLoadedScene = %newScene;
   return %newScene;
}

//---------------------------------------------------------------------------------------------
// SceneWindow.endLevel
// Clears a scene window.
//---------------------------------------------------------------------------------------------
function SceneWindow::endLevel(%sceneWindow)
{
   %scene = %sceneWindow.getScene();
   
   if (!isObject(%scene))
      return;
   
   %scene.endLevel();
   
   if (isObject(%scene))
   {
      if( isObject( %scene.getGlobalTileMap() ) )
         %scene.getGlobalTileMap().delete();
         
      %scene.delete();
   }
   
   $lastLoadedScene = "";
}

//---------------------------------------------------------------------------------------------
// Scene.endLevel
// Clears a scene.
//---------------------------------------------------------------------------------------------
function Scene::endLevel(%scene)
{
   if (!$LevelEditorActive)
   {
      %sceneObjectList = %scene.getSceneObjectList();
      // And finally, notify all the objects that they were loaded.
      for (%i = 0; %i < getWordCount(%sceneObjectList); %i++)
      {
         %sceneObject = getWord(%sceneObjectList, %i);
         if( %sceneObject.isMethod( "onLevelEnded" ) )
            %sceneObject.onLevelEnded(%scene);
      }
      
      // Notify the scene that the level ended.
      if( %scene.isMethod( "onLevelEnded" ) )
         %scene.onLevelEnded();
   }
      
   if( isObject( $persistentObjectSet ) )
   {
      %count = $persistentObjectSet.getCount();
      for (%i = 0; %i < %count; %i++)
      {
         %object = $persistentObjectSet.getObject(%i);
         %scene.removeFromScene(%object);
      }
   }
   
   %globalTileMap = %scene.getGlobalTileMap();
   if (isObject(%globalTileMap))
      %scene.removeFromScene(%globalTileMap);
   
   %scene.clearScene(true);
   
   if (isObject(%globalTileMap))
      %scene.addToScene(%globalTileMap);
      
   $lastLoadedScene = "";
}

//---------------------------------------------------------------------------------------------
// New Level 
// Will create a new level after verifying changes to existing level if any.
//---------------------------------------------------------------------------------------------
function T2DProject::newLevel( %this )
{
   if( LevelBuilderUndoManager.getUndoCount() > 0 || LevelBuilderUndoManager.getRedoCount() > 0 )
   {
      %mbResult = checkSaveChanges( fileName(%this.currentLevelFile), true );
      if( %mbResult $= $MRCancel )
         return false;
         
      if( %mbResult $= $MROk )
      {
         if( !isFile( %this.currentLevelFile ) )
            %this.saveLevelAs();
         else
            %this.saveLevel();
      }
   }  

   // Generate valid filename (does not already exist)
   %levelPath = expandPath("^project/data/levels/");
   %levelName = "untitled";
   if( isFile( %levelPath @ %levelName @ ".t2d" ) )
   {
      // Add numbers starting with 1 until we find a valid one
      %i = 1;
      %fileBase = %levelPath @ %levelName;
      while( isFile( %fileBase @ %i @ ".t2d" ) )
         %i++;
      %levelName = %levelName @ %i;
   }
   %fileName = %levelPath @ %levelName @ ".t2d";
   
   // Make sure we're on selection tool (deactivate active tool so it cleans up)
   LevelBuilderToolManager::setTool( LevelEditorSelectionTool );
   
   // Open the Current Scene, if any.
   %lastWindow = ToolManager.getLastWindow();
   if( ! isObject( %lastWindow ) )
   {
      MessageBoxOk("Error","Cannot Create New Scene, No Scene Views" );
      return false;
   }
   
   %lastWindow.endLevel();
   
   %oldPosition = "0 0";
   %oldSize = "0 0";
   if( isObject( %lastWindow.getScene() ) )
   {
      %oldPosition = %lastWindow.getScene().cameraPosition;
      %oldSize = %lastWindow.getScene().cameraSize;
   }
   
   // Load the project config information  
   _loadGameConfigurationData(%this.gamePath);
   
   %lastWindow.scene = new Scene();
   %lastWindow.scene.cameraPosition = "0 0";
   
      //Fetch the size from the helper function based on the above game config! Sven
      %newsize = LevelBuilder::iOSResolutionFromSetting( $pref::iOS::DeviceType, $pref::iOS::ScreenOrientation );

      // Old code was = $levelEditor::DefaultCameraSize; THIS SHOULD BE DEFINED IN THE TEMPLATE
   %lastWindow.scene.cameraSize = %newsize;         
   %lastWindow.setScene(%lastWindow.scene);
      
   %lastWindow.setCurrentCameraPosition(%lastWindow.scene.cameraPosition,
                                                   %lastWindow.scene.cameraSize);

   ToolManager.onCameraChanged( %oldPosition, %oldSize, %lastWindow.scene.cameraPosition, %lastWindow.scene.cameraSize );

   $currentScene = %lastWindow.scene;
   %this.currentLevelFile = %fileName;
   
   if( isObject( $persistentObjectSet ) )
   {
      // Now we need to move all the persistent objects into the scene.
      %count = $persistentObjectSet.getCount();
      for (%i = 0; %i < %count; %i++)
      {
         %object = $persistentObjectSet.getObject(%i);
         %sg = %object.getScene();
         if(%sg)
            %sg.removeFromScene(%object);
            
         $currentScene.addToScene(%object);
         %object.setPosition( %object.getPosition() );
      }
   }
   
   // Notify Views of Load
   GuiFormManager::SendContentMessage( $LBSceneViewContent, 0, "updateScene " @ %lastWindow.scene );
   // Notify TreeView of Load
   GuiFormManager::SendContentMessage( $LBTreeViewContent, 0, "openCurrentGraph" );

   setCanvasTitle( fileName(%fileName) @ " - 3 Step Studio" );

   LevelBuilderUndoManager.clearAll();
   
   $NewLevel = 1;
   
   GuiFormManager::SendContentMessage( $LBQuickEdit, %this, "inspect" );
}

function T2DProject::doSave( %this )
{
   // Open the Current Scene, if any.
   %lastWindow = ToolManager.getLastWindow();
   if( !isObject( %lastWindow ) || !isObject( %lastWindow.getScene() ) )
      return false;
      
   // JDD - Defaulting to Save for this build... I don't get why it 
   //       shouldn't save as part of your level, ideally it wouldn't but
   //       it causes our users more harm to remind them of that inadequacy
   //       by prompting them to say yes to something they have no choice over
   //       if they want their layers and effects in their game.  
   //       [/soapbox]
   if( LevelEditor::getLayerNeedsSave() || LevelEditor::getEffectNeedsSave() )
   {
      // Save 
      LevelEditor::saveAllLayersAndEffects();
      // And Refresh
      GuiFormManager::BroadcastContentMessage( "LevelBuilderSidebarCreate", 0, "refresh" );
   }
   
   // write the font cache
   // rdbhack: this is a hack because its going to force the tool to write
   //   both the tools font cache, and the fonts used for the game when
   //   really all we need to do is write out the fonts the game needs
   populateAllFontCacheRange(32, 255);
   writeFontCache();

   %scene = %lastWindow.getScene();
   
   // Persist Objects
   if( isObject( $persistentObjectSet ) )
   {
      %count = $persistentObjectSet.getCount();
      for (%i = 0; %i < %count; %i++)
      {
         %obj = $persistentObjectSet.getObject(%i);
         %scene.removeFromScene(%obj);
      }
   }
   
   // Save the Level
   if( isWriteableFileName( %this.currentLevelFile ) )
   {
      %fo = new FileObject();
      if( %fo.openForWrite(%this.currentLevelFile) )
      {
         %fo.writeObject(%scene, "%levelContent = ");
         %fo.close();
      }
      %fo.delete();
   }

   if( isObject( $persistentObjectSet ) )
   {
      // Reload
      %count = $persistentObjectSet.getCount();
      for (%i = 0; %i < %count; %i++)
      {
         %obj = $persistentObjectSet.getObject(%i);
         %scene.addToScene(%obj);
         %obj.setPosition(%obj.getPosition());
      }
   }
   
   %this.persistToDisk(true, true, true, true, true, true);
   
   checkForMissingDatablocks();
   
   // Save the XML config information
   _saveGameConfigurationData();//%this.gamePath @ "/common/commonConfig.xml");
   
   $levelEditor::LastLevel = %this.currentLevelFile;
   %this.SaveProject( %this.projectFile );
   
   setCanvasTitle( fileName(%this.currentLevelFile) @ " - 3 Step Studio" );
   return true;
}

function T2DProject::saveLevel( %this )
{
   // Make sure we're on selection tool (deactivate active tool so it cleans up)
   LevelBuilderToolManager::setTool( LevelEditorSelectionTool );
      
   if( !isFile( %this.currentLevelFile ) )
   {
      // Try to get a name
      %levelName = %this.getLevelSaveName(%this.currentLevelFile);
      if( %levelName $= "" ) 
         return false;
         
      // Store
      %this.currentLevelFile = %levelName;      
   }
   
   // Save it.
   return %this.doSave();
}

function T2DProject::saveLevelAs( %this )
{
   // Make sure we're on selection tool (deactivate active tool so it cleans up)
   LevelBuilderToolManager::setTool( LevelEditorSelectionTool );
      
   // Try to get a name
   %levelName = %this.getLevelSaveName(%this.currentLevelFile);
   if( %levelName $= "" ) 
      return false;
      
   // Store
   %this.currentLevelFile = %levelName;
   
   // Save it.
   return %this.doSave();
}

function T2DProject::openLevel( %this, %levelName )
{
   // Try to get a name
   if( %levelName $= "" ) 
      %levelName = %this.getLevelOpenName( %this.currentLevelFile );
   if( %levelName $= "" ) 
      return false;
      
   %lastWindow = ToolManager.getLastWindow();
   if( !isObject( %lastWindow ) )
      return false;
 
    // Prompt to save changes
   if( LevelBuilderUndoManager.getUndoCount() > 0 || LevelBuilderUndoManager.getRedoCount() > 0 )
   {
      %mbResult = checkSaveChanges( fileName(%this.currentLevelFile), true );
      if( %mbResult $= $MRCancel )
         return false;
         
      if( %mbResult $= $MROk )
         %this.saveLevel();
   }
   
   // Make sure we're on selection tool (deactivate active tool so it cleans up)
   LevelBuilderToolManager::setTool( LevelEditorSelectionTool );  
   
   %oldPosition = "0 0";
   %oldSize = "0 0";
   if( isObject( %lastWindow.getScene() ) )
   {
      %oldPosition = %lastWindow.getScene().cameraPosition;
      %oldSize = %lastWindow.getScene().cameraSize;
   }

   %scene = %lastWindow.loadLevel(%levelName);
   
   if (isObject(%scene))
   {
      %this.currentLevelFile = %levelName;
      %this.currentScene = %scene;
      $currentScene = %scene;
      
      //populateLevelDB();

      // Notify Views of Load
      GuiFormManager::SendContentMessage( $LBSceneViewContent, 0, "updateScene " @ %scene );
      // Notify TreeView of Load
      GuiFormManager::SendContentMessage( $LBTreeViewContent, 0, "openCurrentGraph" );
      
      showTriggers(%scene);
   }
   else
      return false;
   
   // Load the project config information  
   _loadGameConfigurationData(%this.gamePath);
   
   $levelEditor::LastLevel = %this.currentLevelFile;
   
   // Must Save the Project to retain the last level  [KNM | 07/28/11 | ITGB-131]
   %this.SaveProject( %this.projectFile );

   LevelBuilderUndoManager.clearAll();
   
   ToolManager.onCameraChanged( %oldPosition, %oldSize, %scene.cameraPosition, %scene.cameraSize );
   
   setCanvasTitle( fileName( %this.currentLevelFile ) @ " - 3 Step Studio" );   
   GuiFormManager::SendContentMessage( $LBQuickEdit, %this, "inspect" );
   
   loadiOSSettings();
   
   levelBuilderEditLevelDatablocks(false);
   
   return true;
}

function addToLevelCallback(%filename)
{
   %lastWindow = ToolManager.getLastWindow();
   if( !isObject( %lastWindow ) )
   {
      error("loadLevelCallback - No Valid Window!");
      return false;
   }

   %scene = %lastWindow.addToLevel(%filename);

   // Notify TreeView of Load
   GuiFormManager::SendContentMessage( $LBTreeViewContent, 0, "openCurrentGraph" );
   
   showTriggers(%scene);
}

function showTriggers(%scene)
{
   %count = %scene.getSceneObjectCount();
   for (%i = 0; %i < %count; %i++)
   {
      %obj = %scene.getSceneObject(%i);
      if( !isObject( %obj ) ) 
         continue;
      if ((%obj.getClassName() $= "SceneObject") || (%obj.getClassName() $= "Trigger") || (%obj.getClassname() $= "Path"))
         %obj.setDebugOn(2, 3);
   }
}

function onExecuteDone(%thing)
{   
   $runningGame = false;
   
   $pref::iOS::DeviceType = $preLaunchDeviceType;
   $pref::T2D::fullscreen = $preLaunchFullsScreen;
   $pref::iOS::ScreenResolution = $preLaunchScreenResolution;
   
   restoreWindow();
   
   // position the cursor at the center of the editor
   %centerPos = levelBuilderContentFrame.getCenter();
   %posX = getWord(%centerPos, 0);
   %posY = getWord(%centerPos, 1);
   
   // rdbnote: this will prevent the re-launch bug
   // change the cursor position
   Canvas.setCursorPos(%posX, %posY);  
   
   // rdbnote: keeping this here since it may help prevent
   // the user from double clicking the play button
   schedule( 1000, 0, "resetPlayButton" );
}

function resetPlayButton()
{
   $PlayGameButton.setActive( true );
}

// [neo, 4/6/2007 - #3167]
// Get the project name from the project path
function getProjectNameFromPath()
{
   // As name is not stored but implicit in path we just rip it from there
   %path = LBProjectObj.gamePath;
   
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

// [neo, 4/6/2007 - #3167]
// Return the player file to execute
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
         echo("Not here, using default executable");
         return expandPath( "^project/" @ %default @ ".exe" );
      }
         
   }
   else
   {
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

   if( LevelBuilderUndoManager.getUndoCount() > 0 || LevelBuilderUndoManager.getRedoCount() > 0 )
   {
      if( !LBProjectObj.saveLevel() )
         return false;
   }
   
   // Post pre-run deploy event 
   Projects::GetEventManager().postEvent( "ProjectDeploy", expandPath( "^project" ) );
      
   $runningGame = true;
   
   minimizeWindow();
   
   //[neo, 4/6/2007 - #3208]
   // Make sure that any button mouse events or mouselock is not actioned again
   // afterwards for some reason.
   $ThisControl.setActive( false );
   
   $PlayGameButton = $ThisControl;
   
   // Stop Sim entirely?
   //$timeScale = 0.0;
   //setT2DSimTimeScale( 0.0 );
   
   %args = "-project" SPC "\"" @ expandPath("^project/") @ "\"";
   shellExecute( %playerExecutable, %args, expandPath("^project/"));
}

$runningGame = false;
$ScriptErrorMessageHash = 999;
function checkForScriptErrors()
{
   if ($levelEditor::DisplayScriptErrors == false)
   {
      $ScriptErrorMessageHash = $ScriptErrorHash;
      return;
   }
      
   if (($ScriptErrorHash != $ScriptErrorMessageHash) && ($ScriptErrorHash != 0))
   {
      %count = getFieldCount($ScriptError);
      %numErrors = $ScriptErrorHash - $ScriptErrorMessageHash;
      %errorString = "";
      for (%i = 0; %i < %numErrors; %i++)
      {
         %field = %count - %numErrors + %i;
         if (%errorString !$= "")
            %errorString = %errorString @ "\n" @ getField($ScriptError, %field);
         else
            %errorString = getField($ScriptError, %field);
      }
      MessageBoxOK("Script Errors Found!", %errorString @ "\n\n" @ "Check the console (Press '~') for details.");
         
      $ScriptErrorMessageHash = $ScriptErrorHash;
   }
}

function saveSelectedGroup()
{
   if (isObject(ToolManager.getAcquiredGroup()))
   {
      %dialog       =  SaveFileDlgEx;
      %filterList   =  %dialog.findObjectByInternalName("FilterList", true);

      %name = ToolManager.getAcquiredGroup().name;
      %currentFile = expandPath( "^project/data/levels/group.t2d" );
   
      // Clear Filters
      %dialog.ClearFilters();
      
      // Add Filters
      %filter = %dialog.AddFilter("*.t2d","TGB Scene Files");
    
      // Select Filter.
      %filterList.setSelected( %filter );
      
      getSaveFileName($T2D::LevelSpec, saveSelectedGroupCallback, %currentFile);   
         
   }
}

function saveSelectedGroupCallback(%filename)
{
   %group = ToolManager.getAcquiredGroup();
   
   %name = %group.getName();
   
   if (isObject(%group))
   {
      %fo = new FileObject();
      %fo.openForWrite(%filename);
      %fo.writeObject(%group, "%levelContent = ");
      %fo.close();
      %fo.delete();
   }
}

function saveSelectedObjects()
{
   if (ToolManager.getAcquiredObjectCount() > 0)
   {
      %dialog       =  SaveFileDlgEx;
      %filterList   =  %dialog.findObjectByInternalName("FilterList", true);

//      getSaveFilename("*.t2d", saveSelectedGroupCallback);
      %currentFile = expandPath("^project/data/levels/selected.t2d");
   
      // Clear Filters
      %dialog.ClearFilters();
      
      // Add Filters
      %filter = %dialog.AddFilter("*.t2d","TGB Scene Files");
    
      // Select Filter.
      %filterList.setSelected( %filter );

      getSaveFilename($T2D::LevelSpec, saveSelectedObjectsCallback, %currentFile);
   }
}

function saveSelectedObjectsCallback(%filename)
{
   %group = ToolManager.getAcquiredGroup();
   if (isObject(%group))
   {
      saveSelectedGroupCallback(%filename);
      return;
   }
   
   %objects = ToolManager.getAcquiredObjects();
   
   if (!isObject(%objects))
      return;

   if( isFile( %fileName @ ".dso" ) )
      fileDelete( %fileName @ ".dso" );

   %count = %objects.getCount();
   // A single object can just be saved out on its own.
   if (%count == 1)
   {
      %object = %objects.getObject(0);
      %fo = new FileObject();
      %fo.openForWrite(%filename);
      %fo.writeObject(%object, "%levelContent = ");
      %fo.close();
      %fo.delete();
   }
   
   // Multiple objects we're going to put in a single group for saving.
   else if (%count > 1)
   {
      // Set mount IDs before saving to clipboard  [KNM | 08/10/11 | ITGB-147]
      %sceneWindow = ToolManager.getLastWindow();
      %scene = %sceneWindow.getScene();
      %scene.setMountIDs();
      
      // Create a group to house the selected objects.
      %set = new SceneObjectSet();
      
      // Temporarily move stuff to the new group.
      for (%i = 0; %i < %count; %i++)
      {
         %object = %objects.getObject(%i);
         %set.add(%object);
      }
      
      %fo = new FileObject();
      %fo.openForWrite(%filename);
      %fo.writeObject(%set, "%levelContent = ");
      %fo.close();
      %fo.delete();
      
      // Kill the new group.
      %set.delete();
   }
}

$layerToSave = -1;
function saveLayer(%layer)
{
   $layerToSave = %layer;
   getSaveFilename("", saveLayerCallback);
}

function saveLayerCallback(%filename)
{
   // Create a group to house the selected objects.
   %newGroup = new SceneObjectGroup();

   // Open the Current Scene, if any.
   %lastWindow = ToolManager.getLastWindow();
   if( !isObject( %lastWindow ) || !isObject( %lastWindow.getScene() ) )
   {
      error("SaveGraphGroup - No Valid Scene Found!");
      return;
   }
   %scene = %lastWindow.getScene();

   %count = %scene.getSceneObjectCount();   
   // Temporarily move stuff to the new group.
   for (%i = 0; %i < %count; %i++)
   {
      %object = %scene.getSceneObject(%i);
      
      if (%object.getSceneLayer() == $layerToSave)
      {
         %oldGroup[%i] = %object.getGroup();
         %newGroup.add(%object);
      }
   }
   
   // Save the new group.
   if (%newGroup.getCount() > 0)
   {
      %fo = new FileObject();
      %fo.openForWrite(%filename);
      %fo.writeObject(%newGroup, "%levelContent = ");
      %fo.close();
      %fo.delete();
   }
   
   // And move stuff back to its original group, or at least out of the new group.
   for (%i = 0; %i < %count; %i++)
   {
      %object = %scene.getSceneObject(%i);
      
      if (%object.getSceneLayer() == $layerToSave)
      {
         %newGroup.remove(%object);
         if (isObject(%oldGroup[%i]))
            %oldGroup[%i].add(%object);
      }
   }
   
   // Kill the new group.
   %newGroup.delete();
   $layerToSave = -1;
}

$graphGroupToSave = -1;
function saveGraphGroup(%graphGroup)
{
   $graphGroupToSave = %graphGroup;
   getSaveFilename("", saveGraphGroupCallback);
}

function saveGraphGroupCallback(%filename)
{
   // Create a group to house the selected objects.
   %newGroup = new SceneObjectGroup();

   // Open the Current Scene, if any.
   %lastWindow = ToolManager.getLastWindow();
   if( !isObject( %lastWindow ) || !isObject( %lastWindow.getScene() ) )
   {
      error("SaveGraphGroup - No Valid Scene Found!");
      return;
   }
   %scene = %lastWindow.getScene();

   %count = %scene.getSceneObjectCount();   
   // Temporarily move stuff to the new group.
   for (%i = 0; %i < %count; %i++)
   {
      %object = %scene.getSceneObject(%i);
      
      if (%object.getSceneGroup() == $groupToSave)
      {
         %oldGroup[%i] = %object.getGroup();
         %newGroup.add(%object);
      }
   }
   
   // Save the new group.
   if (%newGroup.getCount() > 0)
   {
      %fo = new FileObject();
      %fo.openForWrite(%filename);
      %fo.writeObject(%newGroup, "%levelContent = ");
      %fo.close();
      %fo.delete();
   }
   
   // And move stuff back to its original group, or at least out of the new group.
   for (%i = 0; %i < %count; %i++)
   {
      %object = %scene.getSceneObject(%i);
      
      if (%object.getSceneGroup() == $groupToSave)
      {
         %newGroup.remove(%object);
         if (isObject(%oldGroup[%i]))
            %oldGroup[%i].add(%object);
      }
   }
   
   // Kill the new group.
   %newGroup.delete();
   $graphGroupToSave = -1;
}