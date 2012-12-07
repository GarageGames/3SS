//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------
function Scene::onSafeDelete(%this, %obj)
{
    %obj.callOnBehaviors(onSafeDelete);
    
    if (%obj.isMethod(onSafeDelete))
        %obj.onSafeDelete();
}

function Scene::onLevelLoaded(%this)
{
    if ($platform $= "windows" || $platform $= "macos")
        showCursor();
        
    // @@@ temp to test world progress counting.
    if (isObject(HighScoreDisplay))
        HighScoreDisplay.setText(levelSelectGui.currentWorld.LevelHighScore[$LevelIndex]);
}

//------------------------------------------------------------------------------
// Scene.endLevel
// Clears a scene.
//------------------------------------------------------------------------------
function Scene::endLevel(%scene)
{
    if (!$LevelEditorActive)
    {
        %sceneObjectList = %scene.getSceneObjectList();
        
        // And finally, notify all the objects that they were loaded.
        for (%i = 0; %i < getWordCount(%sceneObjectList); %i++)
        {
            %sceneObject = getWord(%sceneObjectList, %i);
            //if (%sceneObject.isMethod("onLevelEnded") )
            %sceneObject.onLevelEnded(%scene);
        }

        // Notify the scene that the level ended.
        if (%scene.isMethod("onLevelEnded") )
            %scene.onLevelEnded();
    }

    %globalTileMap = %scene.getGlobalTileMap();
    
    if (isObject(%globalTileMap))
        %scene.removeFromScene(%globalTileMap);

    %scene.clearScene(true);

    if (isObject(%globalTileMap))
        %scene.addToScene(%globalTileMap);

    $lastLoadedScene = "";
}

//------------------------------------------------------------------------------
// Scene.loadLevel
// Loads a level file into a scene.
//------------------------------------------------------------------------------
function Scene::loadLevel(%scene, %levelFile)
{
   
   %scene.endLevel();
   
   //Get a datablock file name for this level - Sven
   %dbFileName = %levelFile;
   %dbFileName = strreplace(%dbFileName, %levelFileName @ ".taml", "datablocks/" @ %levelFileName @ "_datablocks.taml");   
   
   %newScene = %scene.addToLevel(%levelFile, %dbFileName);
   
   if (isObject(%newScene) && !$LevelEditorActive)
   {
      // Notify the scene that it was loaded.
      if (%newScene.isMethod("onLevelLoaded") )
         %newScene.onLevelLoaded();
      
      // And finally, notify all the objects that they were loaded.
      %sceneObjectList = %newScene.getSceneObjectList();
      %sceneObjectCount = getWordCount(%sceneObjectList);
      
      for (%i = 0; %i < %sceneObjectCount; %i++)
      {
         %sceneObject = getWord(%sceneObjectList, %i);
         %sceneObject.onLevelLoaded(%newScene);
      }
   }
      
   ScheduleManager.initialize();
   $lastLoadedScene = %newScene;
   
   return %newScene;
}

//------------------------------------------------------------------------------
// Scene.addToLevel
// Adds a level file to a scene, passes an extra datablock parameter per level
//------------------------------------------------------------------------------
function Scene::addToLevel(%scene, %levelFile, %dbFileName)
{
    // Reset this. It should always be false unless we are loading into a scenewindow.
    %useNewScene = $useNewScene;
    $useNewScene = false;

    // Prevent name clashes when loading a scene with the same name as this one
    %scene = %scene.getId();

    %level = expandPath("^project/game/template/data/levels/") @ %levelFile;
    // Make sure the file is valid.
    if ((!isFile(%level)) && (!isFile(%level @ ".dso")))
    {
        error("Error loading level " @ %levelFile @ ". Invalid file.");
        return 0;
    }
   
    // Load up the level.
    echo("--------Exec()ing level" SPC %levelFile);//-Mat iPhone debug output
    %levelContent = TamlRead(%level);

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
        %newScene.addToScene(%object);

        for (%i = 0; %i < %object.getCount(); %i++)
        {
            %obj = %object.getObject(%i);

            if (%obj.getClassName() $= "ParticleEffect")
            {
                %newScene.addToScene(%obj);
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
                {
                    error("Unable to find scene's global tile map.");
                }
            }
        }

        $LevelManagement::newObjects = %object;
    }
    else if (%object.getClassName() $= "SceneObjectSet")
    {
        // Add each object in the set to the scene.
        for (%i = 0; %i < %object.getCount(); %i++)
        {
            %obj = %object.getObject(%i);
            %newScene.addToScene(%obj);

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
                {
                    error("Unable to find scene's global tile map.");
                }
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
            {
                error("Unable to find scene's global tile map.");
            }
        }
        else
        {
            %newScene.addToScene(%object);

            $LevelManagement::newObjects = %object;
        }
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
        {
            // if it does not then simply move the objects over
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
        error("Error loading level " @ %levelFile @ ". " @ %object.getClassName() @ " is not a valid level object type.");
        return 0;
    }

    $lastLoadedScene = %newScene;
    return %newScene;
}

function Scene::getImageMapReferenceList(%this, %imageMap, %referenceList, %animations, %sprites, %animatedSprites, %scrollers)
{
    if(%animations $= "")
        %animations = true;

    if(%sprites $= "")
        %sprites = true;

    if(%animatedSprites $= "")
        %animatedSprites = true;

    if(%scrollers $= "")
        %scrollers = true;

    %objectCount = %this.getSceneObjectCount();

    for(%i = 0; %i < %objectCount; %i++)
    {
        %object = %this.getSceneObject(%i);

        %className = %object.getClassName();

        if(((%className $= "t2dStaticSprite") && %sprites) || ((%className $= "Scroller") && %scrollers))
        {
            %testImageMap = %object.getImageMap();

            if(%testImageMap.getId() == %imageMap.getId())
                %referenceList.add(%object);
        }
        else if((%className $= "t2dAnimatedSprite") && %animatedSprites)
        {
            if(%object.animationName.imageMap.getId() == %imageMap.getId())
                %referenceList.add(%object);
        }
    } 

    %datablocks = DataBlockGroup;
    %datablockCount = %datablocks.getCount();

    for(%i = 0; %i < %datablockCount; %i++)
    {
        %datablock = %datablocks.getObject(%i);
        %className = %datablock.getClassName();

        if((%className $= "AnimationAsset") && %animations)
        {
            if(%datablock.imageMap.getId() == %imageMap.getId())
                %referenceList.add(%datablock);
        }
        else if((%className $= "ImageAsset"))
        {
        }
    }

    return %referenceList.getCount();
}

function Scene::getAnimationReferenceList(%this, %animation, %referenceList)
{
    %objectCount = %this.getSceneObjectCount();

    for(%i = 0; %i < %objectCount; %i++)
    {
        %object = %this.getSceneObject(%i);

        if(%object.getClassName() $= "Sprite")
        {
            if(%object.animation.getId() == %animation.getId())
                %referenceList.add(%object);
        }
    }

    return %referenceList.getCount();
}