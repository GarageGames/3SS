//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

/// <summary>
/// This handles tool shutdown.
/// </summary>
function WorldTool::onSleep(%this)
{
    %this.saveData();
    %this.clearLevelData(true);
    Wt_LevelMusicStopBtn.onClick();
    if ( isObject(%this.helpManager) )
    {
        %this.helpManager.stop();
        %this.helpManager.delete();
    }
}

/// <summary>
/// This handles tool wake - just refreshes the tool at the moment.
/// </summary>
function WorldTool::onWake(%this)
{
    %this.refresh();
}

/// <summary>
/// This reloads the current world data from the project's managed worldList.taml file.
/// </summary>
function WorldTool::getWorldData(%this)
{
    %this.currentWorldData = TamlRead("^PhysicsLauncherTemplate/managed/worldList.taml");
    loadGameData(%this.currentWorldData);

    %this.worldCount = %this.currentWorldData.getCount();
    
    for (%i = 0; %i < %this.worldCount; %i++)
    {
        %this.currentWorlds[%i] = %this.currentWorldData.getObject(%i);
    }
}

/// <summary>
/// This handles preparing the tool for use.  It creates the tool views, then loads
/// the world and level data from the project.  Then it selects the first world after
/// the Unmapped Worlds list and the first level in that world and displays the 
/// relevant data.
/// </summary>
function WorldTool::load(%this)
{
    // keep dropdowns from firing onSelect during initialization, etc.
    $Wt_ToolInitialized = false;

    if (%this.selectedLevelIndex !$= "")
        %levelIndex = %this.selectedLevelIndex;
    else
        %levelIndex = 0;

    if (%this.selectedWorld !$= "")
        %worldIndex = %this.selectedWorld;
    else
        %worldIndex = 0;

    %this.helpManager = createHelpMarqueeObject("WorldToolTips", 10000, "{PhysicsLauncherTools}");
    %this.helpManager.openHelpSet("worldToolHelp");
    %this.helpManager.start();

    // Clear any other tools from the view
    EditorShellGui.clearViews();

    // Create and prepare the World List Container
    %this.WorldListContainer = createVerticalScrollContainer();
    EditorShellGui.addView(%this.WorldListContainer, "smallMedium");
    %this.WorldListContainer.setSpacing(2);
    %this.WorldListContainer.setScrollRepeat(70);
    %this.WorldListContainer.setScrollCallbacks(true);
    %this.WorldListContainer.setScrollHandler("Wt_WorldListScrollHandler");
    %this.WorldListContainer.setIndicatorImage("{EditorAssets}:indicationArrowImage");
    %this.WorldListContainer.setNormalProfile(GuiLargePanelContainer);
    %this.WorldListContainer.setHighlightProfile(GuiLargePanelContainerHighlight);
    %this.WorldListContainer.addFooter(%this.createWorldContainerFooter("Edit World Icon", "WorldTool"));
    %this.WorldListContainer.addHeader(%this.createHeader("World List"));

    // Create and prepare the Level List Container
    %this.LevelListContainer = createVerticalScrollContainer();
    EditorShellGui.addView(%this.LevelListContainer, "smallMedium");
    %this.LevelListContainer.setSpacing(2);
    %this.LevelListContainer.setScrollRepeat(50);
    %this.LevelListContainer.setScrollSpeed(0.25);
    %this.LevelListContainer.setScrollCallbacks(true);
    %this.LevelListContainer.setScrollHandler("Wt_LevelListScrollHandler");
    %this.LevelListContainer.setIndicatorImage("{EditorAssets}:indicationArrowImage");
    %this.LevelListContainer.setNormalProfile(GuiNarrowPanelContainer);
    %this.LevelListContainer.setHighlightProfile(GuiNarrowPanelContainerHighlight);
    %this.LevelListContainer.addFooter(%this.createLevelContainerFooter());
    %this.LevelListContainer.addHeader(%this.createHeader("Level List"));

    // Push the World Tool view
    EditorShellGui.addView(WorldTool, "");
    %this.currentView = WorldTool.getId();

    // Load and hold a copy of relevant data sets from the prefabs list
    %this.projectileSet = $PrefabSet.findObjectByInternalName("ProjectileSet"); 
    %this.launcherSet = $PrefabSet.findObjectByInternalName("LauncherSet");

    // prepare the launcher selection dropdown
    $Wt_LauncherDropdownLoading = true;
    Wt_LauncherDropdown.clear();
    for (%i = 0; %i < %this.launcherSet.getCount(); %i++)
    {
        %launcher = %this.launcherSet.getObject(%i);
        %launcherName = SlingshotLauncherBuilder::getName(%launcher);
        Wt_LauncherDropdown.add(%launcherName, %i);
    }
    $Wt_LauncherDropdownLoading = false;

    // prep the current world and level dataset, then display everything.
    %this.worldIndex = %worldIndex;
    %this.refresh();

    // Select the world and level
    if (%this.WorldListContainer.contentPane.getCount() > 2)
      %this.WorldListContainer.setSelected(1);
    else
      %this.WorldListContainer.setSelected(%this.worldIndex);
      
    $Wt_ToolInitialized = true;
    
    // The level list ends with the "Add Level" button so make sure we don't
    // select that if there are no levels for this world.
    %levelCount = %this.currentWorlds[%this.worldIndex].WorldLevelCount;
    if(%levelCount > 0)
    {
        // There is at least one valid level for this world
        %this.LevelListContainer.setSelected(%levelIndex);
        %this.LevelListContainer.scrollToButton(%levelIndex);
    }
    else
    {
        // There are no levels for this world
        %this.clearLevelData(false);
    }
    
    PhysicsLauncherTools::audioButtonInitialize(Wt_LevelMusicPlayBtn);
}

/// <summary>
/// This refreshes the tool view after changing selections
/// </summary>
function WorldTool::refresh(%this)
{
    // clear out the world list container and update its contents
    %this.WorldListContainer.clear();

    if (!isObject(%this.currentWorldData))
        %this.getWorldData();

    // Create and add buttons to the world list container for all current worlds 
    // and the Unmapped World.
    %this.refreshWorldList();

    // Now the Level List....
    %this.refreshLevelList();
}

/// <summary>
/// This saves all changes to the current level and to the managed world list.
/// </summary>
function WorldTool::saveData(%this)
{
    if ( $RefreshInProgress || !$Wt_ToolInitialized )
        return;

    if (isObject(%this.currentWorldData))
    {
        %worldCount = %this.currentWorldData.getCount();
        for ( %i = 0; %i < %worldCount; %i++ )
        {
            %world = %this.currentWorldData.getObject(%i);
            %levelCount = %world.WorldLevelCount;
            for ( %j = 0; %j < %levelCount; %j++ )
            {
                if ( %i == 1 && %j == 0 )
                    %world.LevelLocked[%j] = false;
                else
                    %world.LevelLocked[%j] = true;
                
                if ( %i == 1 )
                    %world.WorldLocked = false;
                else
                    %world.WorldLocked = true;
            }
            if ( %world.WorldLevelCount == 0 )
                %world.WorldLocked = true;
        }
        TamlWrite(%this.currentWorldData, "^PhysicsLauncherTemplate/managed/worldList.taml");

        if ( isFile( $PhysicsLauncher::WorldListFile ) )
        {
            TamlWrite(%this.currentWorldData, $PhysicsLauncher::WorldListFile);
        }
    }
    // If there is no loaded level, don't save it....
    if (%this.lastLevelName !$= "")
    {
        for (%i = 0; %i < 5; %i++)
        {
            if (%i < 3)
            {
                %rewardEdit = "Wt_Reward" @ (%i + 1) @ "ValueEdit";
                %this.loadedLevel.RewardScore[%i] =  %rewardEdit.getText();
            }
            %dropdown = "Wt_Projectile" @ (%i + 1) @ "Dropdown";
            %projectileName = %dropdown.getValue();
            %projectileCountEdit = "Wt_Projectile" @ (%i + 1) @ "CountEdit";
            %projectile = %this.projectileSet.findObjectByInternalName(%dropdown.getText());
            %numAvailable = %projectileCountEdit.getText();
            if ( %projectileName !$= "Empty" && %numAvailable )
            {
                %availProjectile = %projectile.getName();
                %this.loadedLevel.AvailProjectile[%i] = %availProjectile;
                %this.loadedLevel.NumAvailable[%i] = %numAvailable;
            }
            else
            {
                %this.loadedLevel.AvailProjectile[%i] = "";
                %this.loadedLevel.NumAvailable[%i] = "0";
            }
        }
        //echo(" @@@ saving level data: " @ %this.lastLevelName);
        TamlWrite(%this.loadedLevel, "^PhysicsLauncherTemplate/data/levels/" @ %this.lastLevelName @ ".scene.taml");
    }
    else
        echo(" @@@ No level selected - bypassing WorldTool::save()");
}

/// <summary>
/// This clears and updates the contents of the World List Container.
/// </summary>
function WorldTool::refreshWorldList(%this)
{
    %this.WorldListContainer.clear();
    for (%i = 0; %i < %this.currentWorldData.getCount(); %i++)
    {
        %worldName = %this.currentWorlds[%i].getInternalName();
        %worldImage = %this.currentWorlds[%i].WorldImage;
        if (%worldName $= "Unmapped Levels")
        {
            %button = %this.createUnassignedWorldButton();
            %this.WorldListContainer.addButton(%button, "WorldTool", "selectWorld", %i);
        }
        else
        {
            %button = %this.createWorldButton(%i, %worldName, %worldImage);
            %this.WorldListContainer.addButton(%button, "WorldTool", "selectWorld", %i);
        }
    }
    
    %button = WorldTool.createAddWorldButton(%i);
    %this.WorldListContainer.addButton(%button, "WorldTool", "addWorld", %i);
}

/// <summary>
/// This clears and updates the contents of the Level List Container.
/// </summary>
function WorldTool::refreshLevelList(%this)
{
    %this.LevelListContainer.clear();
    %this.getWorldLevels();
    %levelCount = %this.currentWorlds[%this.worldIndex].WorldLevelCount;

    %this.LevelListContainer.toggleBatch(true);
    for (%i = 0; %i < %levelCount; %i++)
    {
        %levelName = %this.currentLevelList[%i];
        %button = %this.createLevelButton(%i, %levelName);
        %this.LevelListContainer.addButton(%button, "WorldTool", "selectLevel", %i);
    }
    %button = %this.createAddLevelButton(%i);
    %this.LevelListContainer.addButton(%button, "WorldTool", "addLevel", %i);
    %this.LevelListContainer.toggleBatch(false);
}

/// <summary>
/// This gets a list of levels in the tool's currently selected world.
/// </summary>
function WorldTool::getWorldLevels(%this)
{
    %levelCount = %this.currentWorlds[%this.worldIndex].WorldLevelCount;
    
    for (%i = 0; %i < %levelCount; %i++)
    {
        %this.currentLevelList[%i] = %this.currentWorlds[%this.worldIndex].LevelList[%i];
    }
}

/// <summary>
/// This selects a world and updates the World Tool display.  It selects the world specified
/// by the data passed back from the World List Container's button (in this case, just an index).
/// </summary>
/// <param name="data">This is passed back from the world list container button</param>
function WorldTool::selectWorld(%this, %data)
{
    %this.selectedWorld = %data;
    %this.worldIndex = %data;
    %this.getWorldLevels();
    %this.refreshLevelList();
    if (%this.LevelListContainer.getCount() > 1)
    {
        %this.selectLevel(0);
        %this.LevelListContainer.setSelected(0);
    }
    else
    {
        %this.clearLevelData();
        %this.lastLevelName = "";
        if ( isObject( %this.selectedLevelBtn ) )
            %this.selectedLevelBtn.delete();

        if ( isObject ( %this.levelSelectBtnCtrl ) )
        {
            %this.LevelListContainer.scrollCtrl.remove(%this.levelSelectBtnCtrl);
            %this.levelSelectBtnCtrl.delete();
        }
    }
    %this.SetSelectedWorldButton(%data);
}

/// <summary>
/// This selects a level and updates the World Tool display.  It selects the level specified
/// by the data passed back from the Level List Container's button (in this case, just an index).
/// </summary>
/// <param name="data">This is passed back from the level list container button</param>
function WorldTool::selectLevel(%this, %data)
{
    Wt_LevelMusicStopBtn.onClick();

    if (%this.lastLevelName !$= "")
    {
        %this.saveData();
    }

    %levelName = %this.currentLevelList[%data];
    %this.lastLevelName = %levelName;
    %this.selectedLevelIndex = %data;

    if (%levelName $= "")
        return;

    %scene = Wt_WorldBackgroundImagePreview.getScene();
    if ( isObject( %scene ) )
        %scene.delete();

    if ( isObject( %this.loadedLevel ) )
    {
        PhysicsLauncherTools::deleteSceneContents(%this.loadedLevel);
        %this.loadedLevel.delete();
    }

    %this.updateLevelData(%levelName);
    %this.SetSelectedLevelButton(%data);
}

/// <summary>
/// This clears and locks the World Tool display when there is no level selected, 
/// such as when selecting a world with no levels in it.
/// </summary>
function WorldTool::clearLevelData(%this, %sleeping)
{
    if ( %sleeping )
    {
        %this.lastLevelName = %level;
        if (isObject(%this.loadedLevel))
        {
            PhysicsLauncherTools::deleteSceneContents(%this.loadedLevel);
            %this.loadedLevel.delete();
        }
    }
    else
    {
        Wt_LevelNameEdit.setText("");
        Wt_LevelNameEdit.setActive(false);

        for (%i = 1; %i < 6; %i++)
        {
            %dropdown = "Wt_Projectile" @ %i @ "Dropdown";
            %projectileEdit = "Wt_Projectile" @ %i @ "CountEdit";
            %projectileUpBtn = "Wt_Projectile" @ %i @ "CountUpBtn";
            %projectileDownBtn = "Wt_Projectile" @ %i @ "CountDownBtn";
            %rewardEdit = "Wt_Reward" @ %i @ "ValueEdit";
            %rewardUpBtn = "Wt_Reward" @ %i @ "UpBtn";
            %rewardDownBtn = "Wt_Reward" @ %i @ "DownBtn";

            %dropdown.clear();
            %dropdown.add("Empty", 0);
            for (%j = 0; %j < %this.projectileSet.getCount(); %j++)
            {
                %projectile = %this.projectileSet.getObject(%j);
                if (%projectile !$= "")
                {
                    %projectileName = %projectile.getInternalName();
                    %dropdown.add(%projectileName, %j + 1);
                }
            }
            %dropdown.setSelected(0);
            %projectileEdit.setText("");
            if (%i < 4)
            {
                %rewardEdit.setText("0");
                %rewardEdit.setActive(false);
                %rewardUpBtn.setActive(false);
                %rewardDownBtn.setActive(false);
            }
        }

        // populate launcher drop down from prefab launcherSet
        Wt_LauncherDropdown.clear();
        %launcherIndex = 0;
        for (%i = 0; %i < %this.launcherSet.getCount(); %i++)
        {
            %name = SlingshotLauncherBuilder::getName(%this.launcherSet.getObject(%i));
            Wt_LauncherDropdown.add(%name, %i+1);
        }
        Wt_LauncherDropdown.setSelected(%launcherIndex);
        Wt_LauncherDropdown.setActive(false);
        
        Wt_LevelEditToolBtn.setActive(false);

        Wt_LevelMusicEdit.setText("");
        Wt_LevelMusicEdit.setActive(false);

        %this.refreshProjectilePane();
    }
}

/// <summary>
/// This loads and unlocks the World Tool display, populating the appropriate fields from the 
/// level passed to it.
/// </summary>
/// <param name="level">The selected level name.</param>
function WorldTool::updateLevelData(%this, %level)
{
    if ( !$Wt_ToolInitialized )
        return;

    if (%level $= "")
    {
        %this.clearLevelData();
        return;
    }
    $RefreshInProgress = true;

    Wt_LevelNameEdit.setActive(true);
    Wt_LevelNameEdit.setText(%level);
    %this.lastLevelName = %level;
    %levelName = expandPath("^PhysicsLauncherTemplate/data/levels/" @ %level @ ".scene.taml");

    %this.loadedLevel = TamlRead(%levelName);
    MainScene.setIsEditorScene(true);
    Wt_WorldBackgroundImagePreview.setScene(%this.loadedLevel);
    %sceneCameraSize = MainScene.cameraSize;
    Wt_WorldBackgroundImagePreview.setCurrentCameraPosition(0, -2, %sceneCameraSize.x, %sceneCameraSize.y);

    %this.populateProjectilePane();
    %this.refreshProjectilePane();

    // get the scene launcher group
    Wt_LauncherDropdown.setActive(true);
    %this.launcherName = LauncherSceneGroup.getInternalName();
    %selected = Wt_LauncherDropdown.findText(%this.launcherName);
    Wt_LauncherDropdown.setSelected(%selected);
    
    // populate launcher drop down from prefab launcherSet
    %launcherIndex = 0;
    for (%i = 0; %i < %this.launcherSet.getCount(); %i++)
    {
        %name = SlingshotLauncherBuilder::getName(%this.launcherSet.getObject(%i));
        %currentLauncher = %this.loadedLevel.findObjectByInternalName(%name);
        if (isObject(%currentLauncher))
        {
            %launcherIndex = %i;
            break;
        }
    }
    Wt_LauncherDropdown.setSelected(%selected);

    Wt_LevelEditToolBtn.setActive(true);

    if (MainScene.music !$= "")
    {
        %tempAsset = MainScene.music;
        %temp = AssetDatabase.acquireAsset(%tempAsset);
        Wt_LevelMusicEdit.setText(%temp.AssetName);
        Wt_LevelMusicEdit.setActive(true);
        AssetDatabase.releaseAsset(%tempAsset);
    }
    Wt_LevelMusicEdit.setActive(true);

    $RefreshInProgress = false;
}

/// <summary>
/// This creates the world button for the unmapped levels world in the world list
/// container.
/// </summary>
function WorldTool::createUnassignedWorldButton(%this)
{
    %control = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiLargeButtonContainer";
        HorizSizing="left";
        VertSizing="top";
        Extent="122 90";
        MinExtent="8 2";
        canSave = "0";
        Visible="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Select to view any levels that are not assigned to a World.";
    };
    
    %worldLabel = new GuiTextCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTextCenterProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="7 67";
        Extent="110 18";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
        text="Unused Levels";
        maxLength="1024";
        truncate="0";
    };
    %control.addGuiControl(%worldLabel);

    %worldNumber = new GuiTextCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTextCenterProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="6 10";
        Extent="24 18";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
        text="0";
        maxLength="1024";
        truncate="0";
    };
    %control.addGuiControl(%worldNumber);

    %worldSelectBtnPreview = new GuiSpriteCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="33 10";
        Extent="55 55";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
    };
    %worldSelectBtnPreview.Image = "{PhysicsLauncherTools}:unusedLevelsImage";
    %control.addGuiControl(%worldSelectBtnPreview);

    return %control;
}


/// <summary>
/// This creates a simple header gui control for use in the list containers.
/// </summary>
/// <param name="text">The text to assign to the header's label.</param>
function WorldTool::createHeader(%this, %text)
{
    %control = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiTransparentProfile";
        HorizSizing="left";
        VertSizing="top";
        Position="0 0";
        Extent="170 24";
        MinExtent="8 2";
        canSave = "0";
        Visible="1";
        hovertime="1000";
    };
    
    %label = new GuiTextCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTextCenterProfile";
        HorizSizing="center";
        VertSizing="center";
        Position="18 3";
        Extent="134 16";
        MinExtent="8 2";
        canSave = "0";
        Visible="1";
        Active="1";
        hovertime="1000";
        maxLength="1024";
        text=%text;
    };
    %control.addGuiControl(%label);
    
    return %control;
}

function WorldTool::createWorldContainerFooter(%this, %buttonText, %tool)
{
    %control = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiTransparentProfile";
        HorizSizing="left";
        VertSizing="top";
        Position="0 0";
        Extent="152 48";
        MinExtent="8 2";
        canSave = "0";
        Visible="1";
        hovertime="1000";
    };

    %button = new GuiButtonCtrl()
    {
        class="WorldIconEditBtn";
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="BlueButtonProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="25 16";
        Extent="120 30";
        MinExtent="8 2";
        canSave = "0";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Select to open the Interface Tool - World Select Tab.";
        groupNum="-1";
        buttonType="PushButton";
        useMouseEvents="0";
        text=%buttonText;
            toolID=%tool;
    };
    %control.addGuiControl(%button);

    return %control;
}

function WorldTool::createLevelContainerFooter(%this)
{
    %control = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiTransparentProfile";
        HorizSizing="left";
        VertSizing="top";
        Position="0 0";
        Extent="152 48";
        MinExtent="8 2";
        canSave = "0";
        Visible="1";
        hovertime="1000";
    };

    %label1 = new GuiTextCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiFooterTextProfile";
        HorizSizing="center";
        VertSizing="center";
        Position="18 10";
        Extent="24 16";
        MinExtent="8 2";
        canSave = "0";
        Visible="1";
        Active="1";
        hovertime="1000";
        maxLength="1024";
        text="Click";
    };
    %control.addGuiControl(%label1);

    %button = new GuiSpriteCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="48 12";
        Extent="16 13";
        MinExtent="8 2";
        canSave = "0";
        Visible="1";
        Active="1";
        hovertime="1000";
        Image="{EditorAssets}:moveLevelArrowImageMap";
    };
    %control.addGuiControl(%button);

    %label2 = new GuiTextCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiFooterTextProfile";
        HorizSizing="center";
        VertSizing="center";
        Position="66 10";
        Extent="100 16";
        MinExtent="8 2";
        canSave = "0";
        Visible="1";
        Active="1";
        hovertime="1000";
        maxLength="1024";
        text="to move a level";
    };
    %control.addGuiControl(%label2);

    %label3 = new GuiTextCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiFooterTextProfile";
        HorizSizing="center";
        VertSizing="center";
        Position="18 30";
        Extent="108 16";
        MinExtent="8 2";
        canSave = "0";
        Visible="1";
        Active="1";
        hovertime="1000";
        maxLength="1024";
        text="to another World";
    };
    %control.addGuiControl(%label3);

    return %control;
}

/// <summary>
/// This creates the Add World button for the World List Container.  The index is used
/// by the assigned button callback to identify which button in the container is requesting
/// the callback.
/// </summary>
/// <param name="index">The index of the button being created.</param>
function WorldTool::createAddWorldButton(%this, %index)
{
    %control = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiTransparentProfile";
        HorizSizing="left";
        VertSizing="top";
        Position="0 0";
        Extent="122 90";
        MinExtent="8 2";
        canSave = "0";
        Visible="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Select to add a new World to this list and the game.";
    };
    
    %addButton = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiNarrowPanelContainer";
        HorizSizing="left";
        VertSizing="top";
        Position="0 0";
        Extent="122 53";
        MinExtent="8 2";
        canSave = "0";
        Visible="1";
        hovertime="1000";
    };
    %control.addGuiControl(%addButton);

    %previewImage = new GuiSpriteCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="left";
        VertSizing="top";
        Position="5 5";
        Extent="43 43";
        MinExtent="8 2";
        canSave = "0";
        Visible="1";
        hovertime="1000";
        wrap="0";
        useSourceRect="0";
        sourceRect="0 0 0 0";
        Image="{EditorAssets}:addButton_normalImageMap";
    };
    %control.addGuiControl(%previewImage);
    
    %worldLabel = new GuiTextCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTextAddBtnProfile";
        HorizSizing="left";
        VertSizing="top";
        Position="48 17";
        Extent="70 18";
        MinExtent="8 2";
        canSave = "0";
        Visible="1";
        Active="1";
        hovertime="1000";
        text="Add World";
        maxLength="1024";
    };
    %control.addGuiControl(%worldLabel);

    return %control;
}

/// <summary>
/// This creates a new, uniquely named ScriptObject world data collection and adds it
/// to the World List.  It selects the new world on creation using the Add World button's
/// index, since the new world will occupy this space and the Add World button will be moved
/// up a spot.
/// </summary>
/// <param name="index">The index of the Add World button.</param>
function WorldTool::addWorld(%this, %index)
{
    %newWorld = new ScriptObject()
    {
        ContainerImage="{PhysicsLauncherAssets}:ScoreContainerImageMap";
        WorldBackground0="{PhysicsLauncherAssets}:LevelSelectMenuBackgroundImageMap";
        WorldImage="{PhysicsLauncherAssets}:WorldImageOneImageMap";
        WorldImageBlank="{PhysicsLauncherAssets}:worldPanelBackgroundBlankImageMap";
        WorldLevelCount="0";
        WorldLocked="1";
        WorldLockedImage="{PhysicsLauncherAssets}:LockedWorldImageOneImageMap";
        WorldProgress="0";
        WorldSelectBackground="{PhysicsLauncherAssets}:WorldSelectMenuBackgroundImageImageMap";
    };
    PhysicsLauncherToolsEventManager.postEvent("_AddWorldButtonRequest", %index SPC %newWorld);
    PhysicsLauncherTools::getWorldData();
}

function WorldTool::finalizeWorldAdd(%this, %data)
{
    %index = getWord(%data, 0);
    %newWorld = getWord(%data, 1);

    %nameData = %this.getValidWorldName();
    %newWorld.Name = getWord(%nameData, 0);
    %newWorld.internalName = getWord(%nameData, 1) SPC getWord(%nameData, 2);
    %this.currentWorldData.add(%newWorld);
    %this.saveData();
    %this.refreshWorldData();
    %this.refreshWorldList();
    %this.WorldListContainer.scrollToButton(%index);
    %this.selectWorld(%index);
    %this.lastLevelName = "";
}

/// <summary>
/// This clears the working copy of the world list data and refreshes it from the
/// World List.
/// </summary>
function WorldTool::refreshWorldData(%this)
{
    %this.getWorldData();
    for (%i = 0; %i < %this.worldCount; %i++)
    {
        %this.currentWorlds[%i] = "";
    }
    for (%i = 0; %i < %this.currentWorldData.getCount(); %i++)
    {
        %this.currentWorlds[%i] = %this.currentWorldData.getObject(%i);
    }
    %this.worldCount = %this.currentWorldData.getCount();
}

/// <summary>
/// This creates a World List Container button for selecting a world from the 
/// world list.
/// </summary>
/// <param name="index">The index of the button - used in selection callback.</param>
/// <param name="name">The name of the world - displayed in the text control.</param>
/// <param name="image">The World Select button image for the represented world.</param>
function WorldTool::createWorldButton(%this, %index, %name, %image)
{
    %control = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiLargeButtonContainer";
        HorizSizing="left";
        VertSizing="top";
        Position="0 0";
        Extent="122 90";
        MinExtent="8 2";
        canSave = "0";
        Visible="1";
        hovertime="1000";
    };
    
    %worldLabel = new GuiTextCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTextCenterProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="7 67";
        Extent="110 18";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
        text=%name;
        maxLength="1024";
        truncate="0";
    };
    %control.addGuiControl(%worldLabel);

    %worldNumber = new GuiTextCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTextCenterProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="6 10";
        Extent="24 18";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
        text=%index;
        maxLength="1024";
        truncate="0";
    };
    %control.addGuiControl(%worldNumber);

    %worldSelectBtnPreview = new GuiSpriteCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="33 10";
        Extent="55 55";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
        Image=%image;
    };
    %control.addGuiControl(%worldSelectBtnPreview);

    return %control;
}

/// <summary>
/// This handles creating a copy of the template level, assigning it a new default
/// name and appending it to the current world's level list.
/// </summary>
function WorldTool::addLevel(%this)
{
    %levelCount = %this.currentWorlds[%this.worldIndex].WorldLevelCount;
    if (%levelCount >= 105)
    {
        // display whine screen here
        Canvas.pushDialog(Wt_LevelLimitGui);
        return;
    }

    %newLevel = TamlRead("^{PhysicsLauncherTools}/data/levels/templateLevel.taml");
    %newLevelName = %this.getValidLevelName();
    %levelName = "^PhysicsLauncherTemplate/data/levels/" @ %newLevelName @ ".scene.taml";
    TamlWrite(%newLevel, %levelName);

    %this.currentWorlds[%this.worldIndex].WorldLevelCount = %levelCount + 1;
    %this.currentWorlds[%this.worldIndex].LevelList[%levelCount] = %newLevelName;
    %this.currentWorlds[%this.worldIndex].LevelImageList[%levelCount] = "{PhysicsLauncherAssets}:LevelImageImageMap";
    %this.currentWorlds[%this.worldIndex].LevelHighScore[%levelCount] = "0";
    %this.currentWorlds[%this.worldIndex].LevelLocked[%levelCount] = "1";
    %this.currentWorlds[%this.worldIndex].LevelLockedImage[%levelCount] = "{PhysicsLauncherAssets}:LockedLevelImageImageMap";
    %this.currentWorlds[%this.worldIndex].LevelStars[%levelCount] = "0";

    %pageCount = mFloor((%this.currentWorlds[%this.worldIndex].WorldLevelCount - 1) / 15);
    if (%this.currentWorlds[%this.worldIndex].WorldLevelCount > 0)
        %pageCount++;

    for (%i = 0; %i < %pageCount; %i++)
    {
        if ( %this.currentWorlds[%this.worldIndex].WorldBackground[%i] $= "" && %this.currentWorlds[%this.worldIndex].WorldBackground[0] !$= "" )
            %this.currentWorlds[%this.worldIndex].WorldBackground[%i] = %this.currentWorlds[%this.worldIndex].WorldBackground[0];
        else if ( %this.currentWorlds[%this.worldIndex].WorldBackground[%i] $= "" )
            %this.currentWorlds[%this.worldIndex].WorldBackground[%i] = "{PhysicsLauncherAssets}:LevelSelectMenuBackgroundImageMap";
            
    }

    %this.saveData();
    %this.getWorldData();
    %this.getWorldLevels(%this.worldIndex);
    %this.selectedLevelIndex = %levelCount;
    %this.refreshLevelList();
    %this.selectLevel(%levelCount);
    %this.LevelListContainer.scrollToButton(%this.LevelListContainer.contentPane.getCount());
    %this.SetSelectedLevelButton(%levelCount);
    PhysicsLauncherTools::getWorldData();
}

/// <summary>
/// This function finds the next valid generic level name available.
/// </summary>
/// <return>A valid, unused level name.</return>
function WorldTool::getValidLevelName(%this)
{
    %levelCount = %this.currentWorlds[%this.worldIndex].WorldLevelCount;
    %name = "Level" @ (%levelCount + 1);
    while (%this.findName(%name))
    {
        %name = "Level" @ (%levelCount++);
    }
    return %name;
}

/// <summary>
/// This searches the current world data for duplicate names.
/// </summary>
/// <param name="name">The name to search for</param>
/// <return>Returns true if the name already exists</return>
function WorldTool::findName(%this, %name)
{
    %found = false;
    for (%i = 0; %i < %this.currentWorldData.getCount(); %i++)
    {
        for (%j = 0; %j < %this.currentWorlds[%i].WorldLevelCount; %j++)
        {
            if (%name $= %this.currentWorlds[%i].LevelList[%j])
            {
                %found = true;
            }
        }
    }
    return %found;
}

/// <summary>
/// This function finds the next valid generic world name available.
/// </summary>
/// <return>Returns a valid unused world name.</return>
function WorldTool::getValidWorldName(%this)
{
    %worldCount = %this.currentWorldData.getCount();
    %name = "World" @ (%worldCount);
    while (%this.findWorldName(%name))
    {
        %name = "World" @ (%worldCount++);
    }
    %displayName = "World " @ %worldCount;
    return (%name SPC %displayName);
}

/// <summary>
/// This searches the current world data for a name.
/// </summary>
/// <param name="name">The name to search for</param>
/// <return>Returns true if the name already exists</return>
function WorldTool::findWorldName(%this, %name)
{
    %found = false;
    %obj = %this.currentWorldData.findObjectByInternalName(%name);
    if ( isObject(%obj) )
        %found = true;
    //for (%i = 0; %i < %this.currentWorldData.getCount(); %i++)
    //{
        //if (%name $= %this.currentWorlds[%i].WorldName)
        //{
            //%found = true;
        //}
    //}
    return %found;
}

/// <summary>
/// This handles creating the Add Level button for the Level List Container.
/// </summary>
/// <param name="index">List position of the button</param>
/// <return>Returns a GUI object to be passed to the List Container as a button.</return>
function WorldTool::createAddLevelButton(%this, %index)
{
    %control = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiNarrowPanelContainer";
        HorizSizing="left";
        VertSizing="top";
        Position="0 0";
        Extent="122 38";
        MinExtent="8 2";
        canSave = "0";
        Visible="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Create a new blank Level.";
    };
    
    %previewImage = new GuiSpriteCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTransparentProfile";
        HorizSizing="left";
        VertSizing="top";
        Position="5 5";
        Extent="30 30";
        MinExtent="8 2";
        canSave = "0";
        Visible="1";
        hovertime="1000";
        wrap="0";
        useSourceRect="0";
        sourceRect="0 0 0 0";
        Image="{EditorAssets}:addButton_normalImageMap";
    };
    %control.addGuiControl(%previewImage);

    %levelLabel = new GuiTextCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTextAddBtnProfile";
        HorizSizing="left";
        VertSizing="top";
        Position="36 10";
        Extent="96 18";
        MinExtent="8 2";
        canSave = "0";
        Visible="1";
        Active="1";
        hovertime="1000";
        text="Add Level";
        maxLength="1024";
    };
    %control.addGuiControl(%levelLabel);

    return %control;
}

/// <summary>
/// This handles creating a level selection button for the Level List Container.
/// </summary>
/// <param name="index">List position of the button</param>
/// <return>Returns a GUI object to be passed to the List Container as a button.</return>
function WorldTool::createLevelButton(%this, %index, %levelName)
{
    %button = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiNarrowPanelContainer";
        HorizSizing="left";
        VertSizing="top";
        Position="0 0";
        Extent="122 38";
        MinExtent="8 2";
        canSave = "0";
        Visible="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Select to edit this Level's properties.";
    };

    %worldNumber = new GuiTextCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTextCenterProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="6 10";
        Extent="24 18";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
        text=%index + 1;
        maxLength="1024";
        truncate="0";
    };
    %button.addGuiControl(%worldNumber);

    %levelLabel = new GuiTextCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTextCenterProfile";
        HorizSizing="left";
        VertSizing="top";
        Position="24 10";
        Extent="92 18";
        MinExtent="8 2";
        canSave = "0";
        Visible="1";
        Active="1";
        hovertime="1000";
        text=%levelName;
        maxLength="1024";
        truncate="1";
    };
    %button.addGuiControl(%levelLabel);

    return %button;
}

/// <summary>
/// This handles moving a level from one world list to another
/// </summary>
/// <param name="targetWorld">The world to move the level to.</param>
function WorldTool::assignLevelToWorld(%this, %targetWorld)
{
    if (%this.currentWorlds[%targetWorld].WorldLevelCount >= 105)
    {
        Canvas.pushDialog(Wt_LevelLimitGui);
        return;
    }

    %level = %this.currentLevelList[%this.selectedLevel];
    %this.currentLevelList[%this.selectedLevel] = "";

    // Clear out the empty slot and shrink list
    %k = 0;
    for (%i = 0; %i < %this.currentWorlds[%this.worldIndex].WorldLevelCount; %i++)
    {
        %tempLevel = %this.currentLevelList[%i];
        if (%tempLevel !$= "")
        {
            %levels[%k] = %tempLevel;
            %highScores[%k] = %this.currentWorlds[%this.worldIndex].LevelHighScore[%i];
            %buttonImage[%k] = %this.currentWorlds[%this.worldIndex].LevelImageList[%i];
            %levelLocked[%k] = %this.currentWorlds[%this.worldIndex].LevelLocked[%i];
            %levelLockedImage[%k] = %this.currentWorlds[%this.worldIndex].LevelLockedImage[%i];
            %levelStars[%k] = %this.currentWorlds[%this.worldIndex].LevelStars[%i];
            %k++;
        }
        else
        {
            %highScore = %this.currentWorlds[%this.worldIndex].LevelHighScore[%i];
            %btnImage = %this.currentWorlds[%this.worldIndex].LevelImageList[%i];
            %levelLock = %this.currentWorlds[%this.worldIndex].LevelLocked[%i];
            %levelLockImage = %this.currentWorlds[%this.worldIndex].LevelLockedImage[%i];
            %stars = %this.currentWorlds[%this.worldIndex].LevelStars[%i];
        }
    }
    %this.currentLevelList[%k] = "";
    %this.currentWorlds[%this.worldIndex].LevelList[%k] = "";
    %this.currentWorlds[%this.worldIndex].LevelHighScore[%k] = "";
    %this.currentWorlds[%this.worldIndex].LevelImageList[%k] = "";
    %this.currentWorlds[%this.worldIndex].LevelLocked[%k] = "";
    %this.currentWorlds[%this.worldIndex].LevelLockedImage[%k] = "";
    %this.currentWorlds[%this.worldIndex].LevelStars[%k] = "";

    %this.currentWorlds[%this.worldIndex].WorldLevelCount = %k;

    // Update the world's level list
    for (%i = 0; %i < %k; %i++)
    {
        %this.currentWorlds[%this.worldIndex].LevelList[%i] = %levels[%i];
        %this.currentWorlds[%this.worldIndex].LevelImageList[%i] = %buttonImage[%i];
        %this.currentWorlds[%this.worldIndex].LevelHighScore[%i] = %highScores[%i];
        %this.currentWorlds[%this.worldIndex].LevelLocked[%i] = %levelLocked[%i];
        %this.currentWorlds[%this.worldIndex].LevelLockedImage[%i] = %levelLockedImage[%i];
        %this.currentWorlds[%this.worldIndex].LevelStars[%i] = %levelStars[%i];
        
        %this.currentLevelList[%i] = %levels[%i];
    }
    %this.currentWorlds[%this.worldIndex].WorldLevelCount = %k;

    // Add the level to its new world
    %levelIndex = %this.currentWorlds[%targetWorld].WorldLevelCount;
    %this.currentWorlds[%targetWorld].LevelList[%levelIndex] = %level;
    %this.currentWorlds[%targetWorld].LevelImageList[%levelIndex] = %btnImage;
    %this.currentWorlds[%targetWorld].LevelHighScore[%levelIndex] = %highScore;
    %this.currentWorlds[%targetWorld].LevelLocked[%levelIndex] = %levelLock;
    %this.currentWorlds[%targetWorld].LevelLockedImage[%levelIndex] = %levelLockImage;
    %this.currentWorlds[%targetWorld].LevelStars[%levelIndex] = %stars;

    %this.currentWorlds[%targetWorld].WorldLevelCount += 1;
    
    %this.refresh();
    %this.selectWorld(%this.worldIndex);
}

/// <summary>
/// This handles deleting a levels associated level file and removing it from
/// the world lists.
/// </summary>
/// <param name="index">List position of the level to delete.</param>
function WorldTool::deleteLevel(%this, %index)
{
    %levelName = %this.currentLevelList[%index];
    %this.currentLevelList[%index] = "";

    // Clear out the empty slot and shrink list
    %newLevelCount = 0;
    for (%i = 0; %i < %this.currentWorlds[%this.worldIndex].WorldLevelCount; %i++)
    {
        %tempLevel = %this.currentLevelList[%i];
        if (%tempLevel !$= "")
        {
            // This is a level to keep so we'll store it
            %levels[%newLevelCount] = %tempLevel;
            %highScores[%newLevelCount] = %this.currentWorlds[%this.worldIndex].LevelHighScore[%i];
            %buttonImage[%newLevelCount] = %this.currentWorlds[%this.worldIndex].LevelImageList[%i];
            %levelLocked[%newLevelCount] = %this.currentWorlds[%this.worldIndex].LevelLocked[%i];
            %levelLockedImage[%newLevelCount] = %this.currentWorlds[%this.worldIndex].LevelLockedImage[%i];
            %levelStars[%newLevelCount] = %this.currentWorlds[%this.worldIndex].LevelStars[%i];

            %newLevelCount++;
        }
    }
    
    %this.currentLevelList[%newLevelCount] = "";
    %this.currentWorlds[%this.worldIndex].LevelList[%newLevelCount] = "";
    %this.currentWorlds[%this.worldIndex].LevelHighScore[%newLevelCount] = "";
    %this.currentWorlds[%this.worldIndex].LevelImageList[%newLevelCount] = "";
    %this.currentWorlds[%this.worldIndex].LevelLocked[%newLevelCount] = "";
    %this.currentWorlds[%this.worldIndex].LevelLockedImage[%newLevelCount] = "";
    %this.currentWorlds[%this.worldIndex].LevelStars[%newLevelCount] = "";

    %this.currentWorlds[%this.worldIndex].WorldLevelCount = %newLevelCount;

    // Update the world's level list
    for (%i = 0; %i < %newLevelCount; %i++)
    {
        // Copy data from saved level that we'll be keeping
        %this.currentWorlds[%this.worldIndex].LevelList[%i] = %levels[%i];
        %this.currentWorlds[%this.worldIndex].LevelImageList[%i] = %buttonImage[%i];
        %this.currentWorlds[%this.worldIndex].LevelHighScore[%i] = %highScores[%i];
        %this.currentWorlds[%this.worldIndex].LevelLocked[%i] = %levelLocked[%i];
        %this.currentWorlds[%this.worldIndex].LevelLockedImage[%i] = %levelLockedImage[%i];
        %this.currentWorlds[%this.worldIndex].LevelStars[%i] = %levelStars[%i];
        
        %this.currentLevelList[%i] = %levels[%i];
    }

    // Save the new level list
    %levelFile = expandPath("^PhysicsLauncherTemplate/data/levels/" @ %levelName @ ".scene.taml");
    fileDelete(%levelFile);
    %this.saveData();
    
    // Rebuild the GUI level list.  This will also add the "Add Level" button at the bottom
    // of the list.
    %this.refreshLevelList();

    if ( isObject ( %this.levelSelectBtnCtrl ) )
    {
        if (%this.LevelListContainer.scrollCtrl.isMember(%this.levelSelectBtnCtrl))
        {
            %this.LevelListContainer.scrollCtrl.remove(%this.levelSelectBtnCtrl);
        }
        %this.levelSelectBtnCtrl.delete();
    }

    if (%newLevelCount > 0)
    {
        // There is at least one level for this world
        if (%index > 0)
        {
            %this.LevelListContainer.setSelected(%index - 1);
            %this.LevelListContainer.scrollToButton(%index - 1);
        }
        else
        {
        %this.LevelListContainer.setSelected(0);
        %this.LevelListContainer.scrollToButton(0);
        }
    }
    else
    {
        // There are no more levels for this world
        %this.clearLevelData(false);
    }
    
    PhysicsLauncherTools::getWorldData();
}

/// <summary>
/// This function moves all of the selected world's levels to the Unmapped World list and
/// removes the world from the world list.
/// </summary>
/// <param name="index">The index of the world to remove.</param>
function WorldTool::removeWorld(%this, %index)
{
    %world = %this.currentWorldData.getObject(%index);
    if (%world.getName() $= "UnmappedLevels")
        return;

    %startIndex = %this.currentWorlds[0].WorldLevelCount;

    for (%i = 0; %i < %world.WorldLevelCount; %i++)
    {
        // Add the level to the unmapped world
        %levelIndex = %startIndex + %i;
        %this.currentWorlds[0].LevelList[%levelIndex] = %world.LevelList[%i];
        %this.currentWorlds[0].LevelImageList[%levelIndex] = %world.LevelImageList[%i];
        %this.currentWorlds[0].LevelLockedImage[%levelIndex] = %world.LevelLockedImage[%i];
        %this.currentWorlds[0].LevelHighScore[%levelIndex] = %world.LevelHighScore[%i];
        %this.currentWorlds[0].LevelLocked[%levelIndex] = %world.LevelLocked[%i];
        %this.currentWorlds[0].LevelStars[%levelIndex] = %world.LevelStars[%i];

        %this.currentWorlds[0].WorldLevelCount += 1;
    }

    %this.currentWorldData.remove(%world);
    %this.saveData();
    %this.refreshWorldData();
    %this.refreshWorldList();
    %this.refresh();
    %this.WorldListContainer.setSelected(0);
    PhysicsLauncherTools::getWorldData();
}

/// <summary>
/// This function ensures that the desired level file name is valid.
/// </summary>
/// <param name="fileName">The file name we want to validate.</param>
/// <return>Returns true if the file name is valid, or false if not.</return>
function WorldTool::checkValidLevelFileName(%this, %fileName)
{
    // Check for empty filename 
    if (strreplace(%fileName, " ", "") $= "")  
    {
        // Show message dialog
        NoticeGui.display("Level name cannot be empty!", 
        "WorldTool", "resetLevelName", %this.selectedLevelIndex);

        return false;   
    }

    // Check for spaces
    if (strreplace(%fileName, " ", "") !$= %fileName)
    {
        // Show message dialog
        NoticeGui.display("Level name cannot contain spaces!", 
        "WorldTool", "resetLevelName", %this.selectedLevelIndex);
        
        return false;
    }
    
    // Check for invalid characters
    %invalidCharacters = "-+*/%$&§=()[].?\"#,;!~<>|°^{}";
    %strippedName = stripChars(%fileName, %invalidCharacters);
    if (%strippedName !$= %fileName)
    {
        // Show message dialog
        NoticeGui.display("Level name contains invalid symbols!", 
        "WorldTool", "resetLevelName", %this.selectedLevelIndex);
         
        return false;   
    }
    
    return true;
}

/// <summary>
/// This function resets the name of the level at the indicated index to the name
/// currently stored in the world list.
/// </summary>
/// <param name="index">The index of the level to restore.</param>
function WorldTool::resetLevelName(%this, %index)
{
    if (%index $= "")
        return;
    %this.currentWorlds[%this.worldIndex].LevelList[%index] = %this.lastLevelName;
    Wt_LevelNameEdit.setText(%this.lastLevelName);
}

/// <summary>
/// This function sets the name stored in the world list to the new level name
/// chosen by the user.
/// </summary>
/// <param name="index">The index of the level to rename.</param>
/// <param name="fileName">The level name we want to assign.</param>
function WorldTool::renameLevel(%this, %index, %levelName)
{
    if ($RefreshInProgress || !$Wt_ToolInitialized)
        return;
    if (%index !$= "" && %levelName !$= "")
    {
        %this.currentWorlds[%this.worldIndex].LevelList[%index] = %levelName;
        %this.lastLevelName = %levelName;
        %this.saveData();
        %this.getWorldLevels();
        %this.refreshLevelList();
        %this.selectWorld(%this.worldIndex);
        %this.LevelListContainer.setSelected(%index);
    }
}

function WorldTool::editWorldIcon(%this)
{
    echo(" @@@ pretend we edited the world icon");
}

function WorldTool::SetSelectedLevelButton(%this, %position)
{
    %targetButton = %this.LevelListContainer.getButton(%position);
    %levelName = %this.currentWorlds[%this.worldIndex].LevelList[%position];

    if ( isObject ( %this.levelSelectBtnCtrl ) )
    {
        if (%this.LevelListContainer.scrollCtrl.isMember(%this.levelSelectBtnCtrl))
            %this.LevelListContainer.scrollCtrl.remove(%this.levelSelectBtnCtrl);
        %this.levelSelectBtnCtrl.delete();
    }
    %this.levelSelectBtnCtrl = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiTransparentProfileModeless";
        HorizSizing="right";
        VertSizing="bottom";
        Position="0 0";
        Extent="122 500";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
    };
    %size = %this.LevelListContainer.contentPane.Extent;
    %this.levelSelectBtnCtrl.setExtent(%size.x, %size.y);
    %posY = %this.LevelListContainer.scrollCtrl.getScrollPositionY();
    %this.levelSelectBtnCtrl.setPosition(0, 0 - %posY);
    %this.LevelListContainer.scrollCtrl.add(%this.levelSelectBtnCtrl);

    %listCount = %this.LevelListContainer.contentPane.getCount();
    if ( %listCount == 2 )
        %this.CreateSingleLevelHighlightButton(%levelName);
    else if ( %position == 0 )
        %this.CreateTopLevelHighlightButton(%levelName);
    else if ( %position == ( %listCount - 2 ) )
        %this.CreateBottomLevelHighlightButton(%position, %levelName);
    else
        %this.CreateMidLevelHighlightButton(%position, %levelName);

    %this.selectedLevelBtn.SetVisible(true);
}

function WorldTool::CreateSingleLevelHighlightButton(%this, %levelName)
{
    if ( isObject( %this.selectedLevelBtn ) )
        %this.selectedLevelBtn.delete();

    %base = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiTransparentProfileModeless";
        HorizSizing="right";
        VertSizing="bottom";
        Position="0 0";
        Extent="122 38";
        MinExtent="8 2";
        canSave="1";
        Visible="0";
        Active="0";
        hovertime="1000";
    };

    %button = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiNarrowPanelContainerHighlight";
        HorizSizing="right";
        VertSizing="bottom";
        Position="0 0";
        Extent="122 38";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
    };
    %base.addGuiControl(%button);

    %levelName = new GuiTextCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTextProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="31 10";
        Extent="74 18";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
        text=%levelName;
        maxLength="1024";
        truncate="1";
    };
    %button.addGuiControl(%levelName);
    %button.nameField = %levelName;

    %moveLevelBtn = new GuiImageButtonCtrl()
    {
        canSaveDynamicFields="0";
        class="Wt_LevelWorldSelect";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="8 12";
        Extent="16 13";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Move this Level to a new World.";
        groupNum="-1";
        buttonType="PushButton";
        useMouseEvents="0";
        NormalImage="{EditorAssets}:moveLevelArrow_normalImage";
        HoverImage="{EditorAssets}:moveLevelArrow_hoverImage";
        DownImage="{EditorAssets}:moveLevelArrow_downImage";
        InactiveImage="{EditorAssets}:moveLevelArrow_inactiveImage";
            index=0;
    };
    %button.addGuiControl(%moveLevelBtn);

    %removeLevelBtn = new GuiImageButtonCtrl()
    {
        class="Wt_LevelRemove";
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="92 5";
        Extent="25 25";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Delete this Level - this can not be undone.";
        groupNum="-1";
        buttonType="PushButton";
        useMouseEvents="0";
        NormalImage="{EditorAssets}:redCloseImageMap";
        HoverImage="{EditorAssets}:redClose_hImageMap";
        DownImage="{EditorAssets}:redClose_dImageMap";
        InactiveImage="{EditorAssets}:redClose_iImageMap";
            index=0;
    };
    %button.addGuiControl(%removeLevelBtn);

    %this.selectedLevelBtn = %base;
    %base.setPosition(0, 2);
    %this.levelSelectBtnCtrl.addGuiControl(%base);
}

function WorldTool::CreateTopLevelHighlightButton(%this, %levelName)
{
    if ( isObject( %this.selectedLevelBtn ) )
        %this.selectedLevelBtn.delete();

    %base = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiTransparentProfileModeless";
        HorizSizing="right";
        VertSizing="bottom";
        Position="0 0";
        Extent="122 56";
        MinExtent="8 2";
        canSave="1";
        Visible="0";
        Active="0";
        hovertime="1000";
    };

    %button = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiNarrowPanelContainerHighlight";
        HorizSizing="right";
        VertSizing="bottom";
        Position="0 0";
        Extent="122 38";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
    };
    %base.addGuiControl(%button);

    %levelName = new GuiTextCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTextProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="31 10";
        Extent="74 18";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
        text=%levelName;
        maxLength="1024";
        truncate="1";
    };
    %button.addGuiControl(%levelName);
    %button.nameField = %levelName;

    %moveLevelBtn = new GuiImageButtonCtrl()
    {
        canSaveDynamicFields="0";
        class="Wt_LevelWorldSelect";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="8 12";
        Extent="16 13";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Move this Level to a new World.";
        groupNum="-1";
        buttonType="PushButton";
        useMouseEvents="0";
        NormalImage="{EditorAssets}:moveLevelArrow_normalImage";
        HoverImage="{EditorAssets}:moveLevelArrow_hoverImage";
        DownImage="{EditorAssets}:moveLevelArrow_downImage";
        InactiveImage="{EditorAssets}:moveLevelArrow_inactiveImage";
            index=0;
    };
    %button.addGuiControl(%moveLevelBtn);

    %removeLevelBtn = new GuiImageButtonCtrl()
    {
        class="Wt_LevelRemove";
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="92 5";
        Extent="25 25";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Delete this Level - this can not be undone.";
        groupNum="-1";
        buttonType="PushButton";
        useMouseEvents="0";
        NormalImage="{EditorAssets}:redCloseImageMap";
        HoverImage="{EditorAssets}:redClose_hImageMap";
        DownImage="{EditorAssets}:redClose_dImageMap";
        InactiveImage="{EditorAssets}:redClose_iImageMap";
            index=0;
    };
    %button.addGuiControl(%removeLevelBtn);

    %moveDownBtn = new GuiImageButtonCtrl()
    {
        class="Wt_LevelMoveDown";
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="44 33";
        Extent="36 22";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Move this Level down in the Level List order.";
        groupNum="-1";
        buttonType="PushButton";
        useMouseEvents="0";
        NormalImage="{EditorAssets}:downArrow_normalImage";
        HoverImage="{EditorAssets}:downArrow_hoverImage";
        DownImage="{EditorAssets}:downArrow_downImage";
        InactiveImage="{EditorAssets}:downArrow_inactiveImage";
            index=0;
    };
    %base.addGuiControl(%moveDownBtn);

    %this.selectedLevelBtn = %base;
    %base.setPosition(0, 2);
    %this.levelSelectBtnCtrl.addGuiControl(%base);
}

function WorldTool::CreateBottomLevelHighlightButton(%this, %position, %levelName)
{
    if ( isObject( %this.selectedLevelBtn ) )
        %this.selectedLevelBtn.delete();

    %posY = %this.LevelListContainer.getButtonPosition(%position) - 16;
    %base = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiTransparentProfileModeless";
        HorizSizing="right";
        VertSizing="bottom";
        Position="0" SPC %posY;
        Extent="122 56";
        MinExtent="8 2";
        canSave="1";
        Visible="0";
        Active="0";
        hovertime="1000";
    };

    %button = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiNarrowPanelContainerHighlight";
        HorizSizing="right";
        VertSizing="bottom";
        Position="0 18";
        Extent="122 38";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
    };
    %base.addGuiControl(%button);

    %levelName = new GuiTextCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTextProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="31 10";
        Extent="74 18";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
        text=%levelName;
        maxLength="1024";
        truncate="1";
    };
    %button.addGuiControl(%levelName);
    %button.nameField = %levelName;

    %moveLevelBtn = new GuiImageButtonCtrl()
    {
        canSaveDynamicFields="0";
        class="Wt_LevelWorldSelect";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="8 12";
        Extent="16 13";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Move this Level to a new World.";
        groupNum="-1";
        buttonType="PushButton";
        useMouseEvents="0";
        NormalImage="{EditorAssets}:moveLevelArrow_normalImage";
        HoverImage="{EditorAssets}:moveLevelArrow_hoverImage";
        DownImage="{EditorAssets}:moveLevelArrow_downImage";
        InactiveImage="{EditorAssets}:moveLevelArrow_inactiveImage";
            index=%position;
    };
    %button.addGuiControl(%moveLevelBtn);

    %removeLevelBtn = new GuiImageButtonCtrl()
    {
        class="Wt_LevelRemove";
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="92 5";
        Extent="25 25";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Delete this Level - this can not be undone.";
        groupNum="-1";
        buttonType="PushButton";
        useMouseEvents="0";
        NormalImage="{EditorAssets}:redCloseImageMap";
        HoverImage="{EditorAssets}:redClose_hImageMap";
        DownImage="{EditorAssets}:redClose_dImageMap";
        InactiveImage="{EditorAssets}:redClose_iImageMap";
            index=%position;
    };
    %button.addGuiControl(%removeLevelBtn);

    %moveUpBtn = new GuiImageButtonCtrl()
    {
        class="Wt_LevelMoveUp";
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="44 3";
        Extent="36 20";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Move this Level up in the Level List order.";
        groupNum="-1";
        buttonType="PushButton";
        useMouseEvents="0";
        NormalImage="{EditorAssets}:upArrow_normalImage";
        HoverImage="{EditorAssets}:upArrow_hoverImage";
        DownImage="{EditorAssets}:upArrow_downImage";
        InactiveImage="{EditorAssets}:upArrow_inactiveImage";
            index=%position;
    };
    %base.addGuiControl(%moveUpBtn);

    %this.selectedLevelBtn = %base;
    %this.levelSelectBtnCtrl.addGuiControl(%base);
}

function WorldTool::CreateMidLevelHighlightButton(%this, %position, %levelName)
{
    if ( isObject( %this.selectedLevelBtn ) )
        %this.selectedLevelBtn.delete();

    %posY = %this.LevelListContainer.getButtonPosition(%position) - 16;
    %base = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiTransparentProfileModeless";
        HorizSizing="right";
        VertSizing="bottom";
        Position="0" SPC %posY;
        Extent="122 72";
        MinExtent="8 2";
        canSave="1";
        Visible="0";
        Active="0";
        hovertime="1000";
    };

    %button = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiNarrowPanelContainerHighlight";
        HorizSizing="right";
        VertSizing="bottom";
        Position="0 18";
        Extent="122 38";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
    };
    %base.addGuiControl(%button);

    %levelName = new GuiTextCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTextProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="31 10";
        Extent="74 18";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
        text=%levelName;
        maxLength="1024";
        truncate="1";
    };
    %button.addGuiControl(%levelName);
    %button.nameField = %levelName;

    %moveLevelBtn = new GuiImageButtonCtrl()
    {
        canSaveDynamicFields="0";
        class="Wt_LevelWorldSelect";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="8 12";
        Extent="16 13";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Move this Level to a new World.";
        groupNum="-1";
        buttonType="PushButton";
        useMouseEvents="0";
        NormalImage="{EditorAssets}:moveLevelArrow_normalImage";
        HoverImage="{EditorAssets}:moveLevelArrow_hoverImage";
        DownImage="{EditorAssets}:moveLevelArrow_downImage";
        InactiveImage="{EditorAssets}:moveLevelArrow_inactiveImage";
            index=%position;
    };
    %button.addGuiControl(%moveLevelBtn);

    %removeLevelBtn = new GuiImageButtonCtrl()
    {
        class="Wt_LevelRemove";
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="92 5";
        Extent="25 25";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Delete this Level - this can not be undone.";
        groupNum="-1";
        buttonType="PushButton";
        useMouseEvents="0";
        NormalImage="{EditorAssets}:redCloseImageMap";
        HoverImage="{EditorAssets}:redClose_hImageMap";
        DownImage="{EditorAssets}:redClose_dImageMap";
        InactiveImage="{EditorAssets}:redClose_iImageMap";
            index=%position;
    };
    %button.addGuiControl(%removeLevelBtn);

    %moveUpBtn = new GuiImageButtonCtrl()
    {
        class="Wt_LevelMoveUp";
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="44 3";
        Extent="36 20";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Move this Level up in the Level List order.";
        groupNum="-1";
        buttonType="PushButton";
        useMouseEvents="0";
        NormalImage="{EditorAssets}:upArrow_normalImage";
        HoverImage="{EditorAssets}:upArrow_hoverImage";
        DownImage="{EditorAssets}:upArrow_downImage";
        InactiveImage="{EditorAssets}:upArrow_inactiveImage";
            index=%position;
    };
    %base.addGuiControl(%moveUpBtn);

    %moveDownBtn = new GuiImageButtonCtrl()
    {
        class="Wt_LevelMoveDown";
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="44 51";
        Extent="36 22";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Move this Level down in the Level List order.";
        groupNum="-1";
        buttonType="PushButton";
        useMouseEvents="0";
        NormalImage="{EditorAssets}:downArrow_normalImage";
        HoverImage="{EditorAssets}:downArrow_hoverImage";
        DownImage="{EditorAssets}:downArrow_downImage";
        InactiveImage="{EditorAssets}:downArrow_inactiveImage";
            index=%position;
    };
    %base.addGuiControl(%moveDownBtn);

    %this.selectedLevelBtn = %base;
    %this.levelSelectBtnCtrl.addGuiControl(%base);
}

function WorldTool::SetSelectedWorldButton(%this, %position)
{
    %targetButton = %this.WorldListContainer.getButton(%position);
    %worldName = %this.currentWorlds[%this.worldIndex].getInternalName();

    if ( isObject ( %this.worldSelectBtnCtrl ) )
    {
        if (%this.WorldListContainer.scrollCtrl.isMember(%this.worldSelectBtnCtrl))
            %this.WorldListContainer.scrollCtrl.remove(%this.worldSelectBtnCtrl);
        %this.worldSelectBtnCtrl.delete();
    }

    if (%position == 0)
        return;

    %this.worldSelectBtnCtrl = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiTransparentProfileModeless";
        HorizSizing="right";
        VertSizing="bottom";
        Position="0 0";
        Extent="122 500";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
    };
    %size = %this.WorldListContainer.contentPane.Extent;
    %this.worldSelectBtnCtrl.setExtent(%size.x, %size.y);
    %posY = %this.WorldListContainer.scrollCtrl.getScrollPositionY();
    %this.worldSelectBtnCtrl.setPosition(0, 0 - %posY);
    %this.WorldListContainer.scrollCtrl.add(%this.worldSelectBtnCtrl);

    %listCount = %this.WorldListContainer.contentPane.getCount();
    if ( %listCount == 3 )
        %this.CreateSingleWorldHighlightButton(1, %worldName);
    else if ( %position == 1  && %listCount > 3 )
        %this.CreateTopWorldHighlightButton(%position, %worldName);
    else if ( %position == ( %listCount - 2 ) )
        %this.CreateBottomWorldHighlightButton(%position, %worldName);
    else
        %this.CreateMidWorldHighlightButton(%position, %worldName);

    %this.selectedWorldBtn.SetVisible(true);
}

function WorldTool::CreateSingleWorldHighlightButton(%this,%position, %worldName)
{
    if ( isObject( %this.selectedWorldBtn ) )
        %this.selectedWorldBtn.delete();

    %posY = %this.WorldListContainer.getButtonPosition(%position) + 2;

    %base = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiTransparentProfileModeless";
        HorizSizing="right";
        VertSizing="bottom";
        Position="0" SPC %posY;
        Extent="122 90";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
    };

    %button = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiLargePanelContainerHighlight";
        HorizSizing="right";
        VertSizing="bottom";
        Position="0 0";
        Extent="122 90";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
    };
    %base.addGuiControl(%button);

    %removeBtn = new GuiImageButtonCtrl()
    {
        class="Wt_WorldRemoveBtn";
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="92 5";
        Extent="25 25";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Delete this World - any Levels assigned to this World will become Unused.";
        groupNum="-1";
        buttonType="PushButton";
        useMouseEvents="0";
        NormalImage="{EditorAssets}:redCloseImageMap";
        HoverImage="{EditorAssets}:redClose_hImageMap";
        DownImage="{EditorAssets}:redClose_dImageMap";
        InactiveImage="{EditorAssets}:redClose_iImageMap";
            index=%position;
    };
    %button.addGuiControl(%removeBtn);

    %worldLabel = new GuiTextCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTextCenterProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="7 67";
        Extent="110 18";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
        text=%worldName;
        maxLength="1024";
        truncate="0";
    };
    %button.addGuiControl(%worldLabel);

    %worldNumber = new GuiTextCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTextCenterProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="6 10";
        Extent="24 18";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
        text=%position;
        maxLength="1024";
        truncate="0";
    };
    %button.addGuiControl(%worldNumber);

    %worldSelectBtnPreview = new GuiSpriteCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="33 10";
        Extent="55 55";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
        Image=%this.CurrentWorlds[%this.selectedWorld].WorldImage;
    };
    %button.addGuiControl(%worldSelectBtnPreview);

    %this.selectedWorldBtn = %base;
    %this.worldSelectBtnCtrl.addGuiControl(%base);
}

function WorldTool::CreateTopWorldHighlightButton(%this,%position, %worldName)
{
    if ( isObject( %this.selectedWorldBtn ) )
        %this.selectedWorldBtn.delete();

    %posY = %this.WorldListContainer.getButtonPosition(%position) + 2;

    %base = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiTransparentProfileModeless";
        HorizSizing="right";
        VertSizing="bottom";
        Position="0" SPC %posY;
        Extent="122 108";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
    };

    %button = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiLargePanelContainerHighlight";
        HorizSizing="right";
        VertSizing="bottom";
        Position="0 0";
        Extent="122 90";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
    };
    %base.addGuiControl(%button);

    %removeBtn = new GuiImageButtonCtrl()
    {
        class="Wt_WorldRemoveBtn";
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="92 5";
        Extent="25 25";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Delete this World - any Levels assigned to this World will become Unused.";
        groupNum="-1";
        buttonType="PushButton";
        useMouseEvents="0";
        NormalImage="{EditorAssets}:redCloseImageMap";
        HoverImage="{EditorAssets}:redClose_hImageMap";
        DownImage="{EditorAssets}:redClose_dImageMap";
        InactiveImage="{EditorAssets}:redClose_iImageMap";
            index=%position;
    };
    %button.addGuiControl(%removeBtn);

    %worldLabel = new GuiTextCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTextCenterProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="7 67";
        Extent="110 18";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
        text=%worldName;
        maxLength="1024";
        truncate="0";
    };
    %button.addGuiControl(%worldLabel);

    %worldNumber = new GuiTextCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTextCenterProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="6 10";
        Extent="24 18";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
        text=%position;
        maxLength="1024";
        truncate="0";
    };
    %button.addGuiControl(%worldNumber);

    %worldSelectBtnPreview = new GuiSpriteCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="33 10";
        Extent="55 55";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
        Image=%this.CurrentWorlds[%this.selectedWorld].WorldImage;
    };
    %button.addGuiControl(%worldSelectBtnPreview);

    %moveDownBtn = new GuiImageButtonCtrl()
    {
        class="Wt_WorldMoveDownBtn";
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="44 85";
        Extent="36 22";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Move this World down in the World List order.";
        groupNum="-1";
        buttonType="PushButton";
        useMouseEvents="0";
        NormalImage="{EditorAssets}:downArrow_normalImage";
        HoverImage="{EditorAssets}:downArrow_hoverImage";
        DownImage="{EditorAssets}:downArrow_downImage";
        InactiveImage="{EditorAssets}:downArrow_inactiveImage";
            index=%position;
    };
    %base.addGuiControl(%moveDownBtn);

    %this.selectedWorldBtn = %base;
    %this.worldSelectBtnCtrl.addGuiControl(%base);
}

function WorldTool::CreateBottomWorldHighlightButton(%this, %position, %worldName)
{
    if ( isObject( %this.selectedWorldBtn ) )
        %this.selectedWorldBtn.delete();

    %posY = %this.WorldListContainer.getButtonPosition(%position) - 16;

    %base = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiTransparentProfileModeless";
        HorizSizing="right";
        VertSizing="bottom";
        Position="0" SPC %posY;
        Extent="122 108";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
    };

    %button = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiLargePanelContainerHighlight";
        HorizSizing="right";
        VertSizing="bottom";
        Position="0 18";
        Extent="122 90";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
    };
    %base.addGuiControl(%button);

    %removeBtn = new GuiImageButtonCtrl()
    {
        class="Wt_WorldRemoveBtn";
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="92 5";
        Extent="25 25";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Delete this World - any Levels assigned to this World will become Unused.";
        groupNum="-1";
        buttonType="PushButton";
        useMouseEvents="0";
        NormalImage="{EditorAssets}:redCloseImageMap";
        HoverImage="{EditorAssets}:redClose_hImageMap";
        DownImage="{EditorAssets}:redClose_dImageMap";
        InactiveImage="{EditorAssets}:redClose_iImageMap";
            index=%position;
    };
    %button.addGuiControl(%removeBtn);

    %worldLabel = new GuiTextCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTextCenterProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="7 67";
        Extent="110 18";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
        text=%worldName;
        maxLength="1024";
        truncate="0";
    };
    %button.addGuiControl(%worldLabel);

    %worldIndex = new GuiTextCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTextCenterProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="6 10";
        Extent="24 18";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
        text=%position;
        maxLength="1024";
        truncate="0";
    };
    %button.addGuiControl(%worldIndex);

    %worldBtnPreview = new GuiSpriteCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="33 10";
        Extent="55 55";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
        Image=%this.CurrentWorlds[%this.selectedWorld].WorldImage;
    };
    %button.addGuiControl(%worldBtnPreview);

    %moveUpBtn = new GuiImageButtonCtrl()
    {
        class="Wt_WorldMoveUpBtn";
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="44 3";
        Extent="36 20";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Move this World up in the World List order.";
        groupNum="-1";
        buttonType="PushButton";
        useMouseEvents="0";
        NormalImage="{EditorAssets}:upArrow_normalImage";
        HoverImage="{EditorAssets}:upArrow_hoverImage";
        DownImage="{EditorAssets}:upArrow_downImage";
        InactiveImage="{EditorAssets}:upArrow_inactiveImage";
            index = %position;
    };
    %base.addGuiControl(%moveUpBtn);

    %this.selectedWorldBtn = %base;
    %this.worldSelectBtnCtrl.addGuiControl(%base);
}

function WorldTool::CreateMidWorldHighlightButton(%this, %position, %worldName)
{
    if ( isObject( %this.selectedWorldBtn ) )
        %this.selectedWorldBtn.delete();

    %posY = %this.WorldListContainer.getButtonPosition(%position) - 16;

    %base = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiTransparentProfileModeless";
        HorizSizing="right";
        VertSizing="bottom";
        Position="0" SPC %posY;
        Extent="122 125";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
    };

    %button = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiLargePanelContainerHighlight";
        HorizSizing="right";
        VertSizing="bottom";
        Position="0 18";
        Extent="122 90";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
    };
    %base.addGuiControl(%button);

    %removeBtn = new GuiImageButtonCtrl()
    {
        class="Wt_WorldRemoveBtn";
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="92 5";
        Extent="25 25";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Delete this World - any Levels assigned to this World will become Unused.";
        groupNum="-1";
        buttonType="PushButton";
        useMouseEvents="0";
        NormalImage="{EditorAssets}:redCloseImageMap";
        HoverImage="{EditorAssets}:redClose_hImageMap";
        DownImage="{EditorAssets}:redClose_dImageMap";
        InactiveImage="{EditorAssets}:redClose_iImageMap";
            index=%position;
    };
    %button.addGuiControl(%removeBtn);

    %worldLabel = new GuiTextCtrl()
    {
            canSaveDynamicFields="0";
            isContainer="0";
            Profile="GuiTextCenterProfile";
            HorizSizing="right";
            VertSizing="bottom";
            Position="7 67";
            Extent="110 18";
            MinExtent="8 2";
            canSave="1";
            Visible="1";
            Active="0";
            hovertime="1000";
            text=%worldName;
            maxLength="1024";
            truncate="0";
    };
    %button.addGuiControl(%worldLabel);

    %worldIndex = new GuiTextCtrl()
    {
            canSaveDynamicFields="0";
            isContainer="0";
            Profile="GuiTextCenterProfile";
            HorizSizing="right";
            VertSizing="bottom";
            Position="6 10";
            Extent="24 18";
            MinExtent="8 2";
            canSave="1";
            Visible="1";
            Active="0";
            hovertime="1000";
            text=%position;
            maxLength="1024";
            truncate="0";
    };
    %button.addGuiControl(%worldIndex);

    %worldBtnPreview = new GuiSpriteCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="33 10";
        Extent="55 55";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
        Image=%this.CurrentWorlds[%this.selectedWorld].WorldImage;
    };
    %button.addGuiControl(%worldBtnPreview);

    %moveDownBtn = new GuiImageButtonCtrl()
    {
        class="Wt_WorldMoveDownBtn";
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="44 103";
        Extent="36 22";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Move this World down in the World List order.";
        groupNum="-1";
        buttonType="PushButton";
        useMouseEvents="0";
        NormalImage="{EditorAssets}:downArrow_normalImage";
        HoverImage="{EditorAssets}:downArrow_hoverImage";
        DownImage="{EditorAssets}:downArrow_downImage";
        InactiveImage="{EditorAssets}:downArrow_inactiveImage";
            index = %position;
    };
    %base.addGuiControl(%moveDownBtn);

    %moveUpBtn = new GuiImageButtonCtrl()
    {
        class="Wt_WorldMoveUpBtn";
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="44 3";
        Extent="36 20";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Move this World up in the World List order.";
        groupNum="-1";
        buttonType="PushButton";
        useMouseEvents="0";
        NormalImage="{EditorAssets}:upArrow_normalImage";
        HoverImage="{EditorAssets}:upArrow_hoverImage";
        DownImage="{EditorAssets}:upArrow_downImage";
        InactiveImage="{EditorAssets}:upArrow_inactiveImage";
            index = %position;
    };
    %base.addGuiControl(%moveUpBtn);

    %this.selectedWorldBtn = %base;
    %this.worldSelectBtnCtrl.addGuiControl(%base);
}

function WorldTool::populateProjectilePane(%this)
{
    if ( !isObject(%this.projectileSet) || !isObject(%this.loadedLevel) )
        return;

    for (%i = 1; %i < 6; %i++)
    {
        %dropdown = "Wt_Projectile" @ %i @ "Dropdown";
        %projectileEdit = "Wt_Projectile" @ %i @ "CountEdit";
        %rewardEdit = "Wt_Reward" @ %i @ "ValueEdit";
        %rewardUpBtn = "Wt_Reward" @ %i @ "UpBtn";
        %rewardDownBtn = "Wt_Reward" @ %i @ "DownBtn";

        %dropdown.initialize();
        %currentProjectile = %this.loadedLevel.AvailProjectile[%i - 1];
        %duplicate = false;
        for(%k = 0; %k < 5; %k++)
        {
            if (%k == (%i - 1))
                continue;
            if (%this.loadedLevel.AvailProjectile[%k] $= %currentProjectile)
                %duplicate = true;
        }
        if (%this.loadedLevel.AvailProjectile[%i - 1] !$= "")
        {
            if (!isObject(%this.loadedLevel.AvailProjectile[%i - 1]) || %duplicate)
            {
                %dropdown.setFirstSelected();
                %this.loadedLevel.AvailProjectile[%i - 1] = "";
                %this.loadedLevel.NumAvailable[%i - 1] = "0";
                %projectileEdit.setText("");
            }
            else
            {
                %name = %this.loadedLevel.AvailProjectile[%i - 1].getInternalName();
                %index = %dropdown.findText(%name);
                %dropdown.setSelected(%index);
                %projectileEdit.setText(%this.loadedLevel.NumAvailable[%i - 1]);
            }
        }
        else
        {
            %dropdown.setFirstSelected();
            %this.loadedLevel.AvailProjectile[%i - 1] = "";
            %this.loadedLevel.NumAvailable[%i - 1] = "0";
            %projectileEdit.setText("");
        }

        %projectileEdit.setActive(true);
        if (%i < 4)
        {
            %rewardEdit.setActive(true);
            %rewardEdit.setText(%this.loadedLevel.RewardScore[%i - 1]);
            %rewardUpBtn.setActive(true);
            %rewardDownBtn.setActive(true);
        }
    }
}

function WorldTool::refreshProjectilePane(%this)
{
    %firstEmpty = false;
    for (%index = 1; %index < 6; %index++)
    {
        %dropdown = "Wt_Projectile" @ %index @ "Dropdown";
        %edit = "Wt_Projectile" @ %index @ "CountEdit";
        %pane = %dropdown.getParent();

        %edit.setActive(true);
        %projectileName = %dropdown.getText();

        if (%projectileName $= "Empty")
        {
            %projectileName = "";
            %edit.setText("");
            if (!%firstEmpty)
            {
                // set projectile pane to firstEmpty state and set firstEmpty true
                %firstEmpty = true;
                Wt_SetProjectilePaneState(%pane, "firstEmpty");
            }
            else
            {
                Wt_SetProjectilePaneState(%pane, "empty");
            }
        }
        else if ( %projectileName !$= "" && %firstEmpty )
        {
            %projectileName = "";
            %edit.setText("");
            %this.moveProjectileEntries(%index - 1);
            break;
        }
        else
        {
            Wt_SetProjectilePaneState(%pane, "normal");
            if (%edit.getText() $= "")
                %edit.setText("1");
        }

        %projectile = WorldTool.projectileSet.findObjectByInternalName(%projectileName);
        if ( isObject( %projectile ) )
        {
            %this.loadedLevel.AvailProjectile[%index - 1] = %projectile.getName();
        }
    }
}

function WorldTool::moveProjectileEntries(%this, %index)
{
    // There is an empty projectile slot in the middle of the list, clear it and move
    // everything up a spot in the list.
    %index--;
    %k = 0;
    for ( %i = 0; %i < 5; %i++ )
    {
        if ( %i != %index )
        {
            %projectileList[%k] = %this.loadedLevel.AvailProjectile[%i];
            %countList[%k] = %this.loadedLevel.NumAvailable[%i];
            %k++;
        }
    }
    %projectileList[%k] = "";
    %countList[%k] = 0;
    for ( %i = 0; %i < 5; %i++ )
    {
        //echo(" @@@ list[" @ %i @ "] = " @ %projectileList[%i]);
        %this.loadedLevel.AvailProjectile[%i] = %projectileList[%i];
        %this.loadedLevel.NumAvailable[%i] = %countList[%i];
        %dropdown = "Wt_Projectile" @ %i + 1 @ "Dropdown";
        if ( %this.loadedLevel.AvailProjectile[%i] !$= "" )
        {
            %this.loadedLevel.NumAvailable[%i] = "1";
            %dropdown.select(%this.loadedLevel.AvailProjectile[%i].getInternalName());
        }
        else
            %dropdown.select("Empty");
    }
    
    %this.populateProjectilePane();
}

//-----------------------------------------------------------------------------
// World Tool text object callback handlers
//-----------------------------------------------------------------------------

function Wt_LevelNameEdit::onReturn(%this)
{
    %this.onValidate();
}

/// <summary>
/// This function validates the text in the level name editbox and attempts to 
/// rename the level to the editbox's text value.
/// </summary>
function Wt_LevelNameEdit::onValidate(%this)
{
    if ($RefreshInProgress || !$Wt_ToolInitialized)
        return;
    if (WorldTool.lastLevelName $= "")
        return;

    %newLevelName = %this.getText();
    %validName = WorldTool.checkValidLevelFileName(%newLevelName);
    %duplicate = WorldTool.findName(%newLevelName);
    if (%validName && !%duplicate)
    {
        %oldName = WorldTool.lastLevelName;
        %oldLevel = WorldTool.loadedLevel;
        if (PhysicsLauncherTools::renameCurrentLevelFile(%oldName, %newLevelName, %oldLevel))
            WorldTool.renameLevel(WorldTool.selectedLevelIndex, %newLevelName);
    }
    else if (%duplicate)
    {
        if (%newLevelName $= WorldTool.lastLevelName)
            return;

        NoticeGui.display("The level name you have chosen already exists.  Please choose another name.");
        WorldTool.resetLevelName();
    }
}

//-----------------------------------------------------------------------------
// World Select Buttons
//-----------------------------------------------------------------------------
/// <summary>
/// This function handles the world remove button - it sets up the data for 
/// the desired world to remove and pushes the confirmation dialog.
/// </summary>
function Wt_WorldRemoveBtn::onClick(%this)
{
    %worldName = WorldTool.CurrentWorlds[WorldTool.selectedWorld].getInternalName();
    %levelList = %this.index;
    %levelCount = WorldTool.CurrentWorlds[WorldTool.selectedWorld].WorldLevelCount;
    for (%i = 0; %i < %levelCount; %i++)
    {
        %level = WorldTool.CurrentWorlds[WorldTool.selectedWorld].LevelList[%i];
        %temp = %levelList SPC %level;
        %levelList = %temp;
    }
    Wt_WorldConfirmDeleteGui.display(%worldName, "WorldTool", "RemoveWorld", %levelList);
}

/// <summary>
/// This function handles the button to move the associated world up in the 
/// world list.
/// </summary>
function Wt_WorldMoveUpBtn::onClick(%this)
{
    %target = %this.index - 1;
    if (%target < 1)
        return;

    moveWorldData(%this.index, %target);

    WorldTool.refresh();
    WorldTool.WorldListContainer.scrollToButton(%target);
    PhysicsLauncherToolsEventManager.postEvent("_WorldButtonUpdateComplete", %target);
}

/// <summary>
/// This function handles the button to move the associated world down in the 
/// world list.
/// </summary>
function Wt_WorldMoveDownBtn::onClick(%this)
{
    %target = %this.index + 1;
    if (%target >= WorldTool.currentWorldData.getCount())
        return;

    moveWorldData(%this.index, %target);

    WorldTool.refresh();
    WorldTool.WorldListContainer.scrollToButton(%target);
    PhysicsLauncherToolsEventManager.postEvent("_WorldButtonUpdateComplete", %target);
}

/// <summary>
/// This function handles moving the world data in the world list
/// </summary>
/// <param name="startIndex">The index that the data currently occupies.</param>
/// <param name="targetIndex">The index to move the world data to.</param>
function moveWorldData(%startIndex, %targetIndex)
{
    %temp = WorldTool.currentWorlds[%targetIndex];
    WorldTool.currentWorlds[%targetIndex] = WorldTool.currentWorlds[%startIndex];
    WorldTool.currentWorlds[%startIndex] = %temp;
    
    if (%targetIndex > %startIndex)
        WorldTool.currentWorldData.reorderChild(WorldTool.currentWorlds[%startIndex], WorldTool.currentWorlds[%targetIndex]);
    else
        WorldTool.currentWorldData.reorderChild(WorldTool.currentWorlds[%targetIndex], WorldTool.currentWorlds[%startIndex]);

    %worldCount = WorldTool.currentWorldData.getCount();
    %i = 1;
    while (%i < %worldCount)
    {
        %world = WorldTool.currentWorldData.getObject(%i);
        %world.WorldLocked = %i == 1 ? 0 : 1;
        %levelCount = %world.WorldLevelCount;
        for (%j = 0; %j < %levelCount; %j++)
        {
            %world.LevelLocked[%j] = (%i == 1 && %j == 0) ? 0 : 1;
        }
        %i++;
    }
}

//-----------------------------------------------------------------------------
// Level Select Buttons
//-----------------------------------------------------------------------------
/// <summary>
/// This handles button clicks for contained buttons.  It checks to ensure that 
/// the object has the assigned method, then calls that method with the assigned
/// data.
/// </summary>
function Wt_LevelSelectButton::onMouseUp(%this)
{
    %object = %this.object;
    if (%object.isMethod(%this.handler))
        %object.call(%this.handler, %this.data);
}

/// <summary>
/// This handles loading the currently selected world into the Level Builder Tool.
/// </summary>
function Wt_LevelEditToolBtn::onClick(%this)
{
    %level = WorldTool.lastLevelName;
    LevelBuilderToolPresenter.load();
    LevelBuilderToolPresenter.loadLevel(WorldTool.currentWorldData.getObject(WorldTool.worldIndex).getInternalName(), %level);
    Tt_LevelBuilderToolButton.setStateOn(true);
}

/// <summary>
/// This handles the Move Level button on the level select button in the levels 
/// container.  It displays the confirmation dialog, the dialog handles actually
/// moving the level to its new world if necessary.
/// </summary>
function Wt_LevelWorldSelect::onClick(%this)
{
    WorldTool.selectedLevel = %this.index;
    Wt_AssignLevelToWorldGui.display(WorldTool.lastLevelName);
}

/// <summary>
/// This handles moving the level up in the level list for reordering the levels.
/// </summary>
function Wt_LevelMoveUp::onClick(%this)
{
    WorldTool.selectedLevelBtn.setVisible(false);
    %target = %this.index - 1;
    if (%target < 0)
        return;

    moveLevelData(%this.index, %target);

    WorldTool.refreshLevelList();
    WorldTool.LevelListContainer.scrollToButton(%target);
    PhysicsLauncherToolsEventManager.postEvent("_LevelButtonUpdateComplete", %target);
}

/// <summary>
/// This handles moving the level down in the level list for reordering the levels.
/// </summary>
function Wt_LevelMoveDown::onClick(%this)
{
    WorldTool.selectedLevelBtn.setVisible(false);
    %target = %this.index + 1;
    if (WorldTool.currentLevelList[%target] $= "")
        return;

    moveLevelData(%this.index, %target);

    WorldTool.refreshLevelList();
    WorldTool.LevelListContainer.scrollToButton(%this.index);
    PhysicsLauncherToolsEventManager.postEvent("_LevelButtonUpdateComplete", %target);
}

/// <summary>
/// This function handles reordering the levels and associated data in the world list.
/// </summary>
/// <param name="startIndex">The level's original position in the list</param>
/// <param name="targetIndex">The desired new position of the level</param>
function moveLevelData(%startIndex, %targetIndex)
{
    %levelList = WorldTool.currentWorlds[WorldTool.worldIndex].LevelList[%targetIndex];
    %levelHighScore = WorldTool.currentWorlds[WorldTool.worldIndex].LevelHighScore[%targetIndex];
    %levelImageList = WorldTool.currentWorlds[WorldTool.worldIndex].LevelImageList[%targetIndex];
    %levelLockedImage = WorldTool.currentWorlds[WorldTool.worldIndex].LevelLockedImage[%targetIndex];
    %levelLocked = WorldTool.currentWorlds[WorldTool.worldIndex].LevelLocked[%targetIndex];
    %levelStars = WorldTool.currentWorlds[WorldTool.worldIndex].LevelStars[%targetIndex];

    WorldTool.currentWorlds[WorldTool.worldIndex].LevelList[%targetIndex] = WorldTool.currentWorlds[WorldTool.worldIndex].LevelList[%startIndex];
    WorldTool.currentWorlds[WorldTool.worldIndex].LevelHighScore[%targetIndex] = WorldTool.currentWorlds[WorldTool.worldIndex].LevelHighScore[%startIndex];
    WorldTool.currentWorlds[WorldTool.worldIndex].LevelImageList[%targetIndex] = WorldTool.currentWorlds[WorldTool.worldIndex].LevelImageList[%startIndex];
    WorldTool.currentWorlds[WorldTool.worldIndex].LevelLockedImage[%targetIndex] = WorldTool.currentWorlds[WorldTool.worldIndex].LevelLockedImage[%startIndex];
    WorldTool.currentWorlds[WorldTool.worldIndex].LevelLocked[%targetIndex] = (WorldTool.worldIndex == 1 && %targetIndex) ? 1 : 0;
    WorldTool.currentWorlds[WorldTool.worldIndex].LevelStars[%targetIndex] = WorldTool.currentWorlds[WorldTool.worldIndex].LevelStars[%startIndex];

    WorldTool.currentWorlds[WorldTool.worldIndex].LevelList[%startIndex] = %levelList;
    WorldTool.currentWorlds[WorldTool.worldIndex].LevelHighScore[%startIndex] = %levelHighScore;
    WorldTool.currentWorlds[WorldTool.worldIndex].LevelImageList[%startIndex] = %levelImageList;
    WorldTool.currentWorlds[WorldTool.worldIndex].LevelLockedImage[%startIndex] = %levelLockedImage;
    WorldTool.currentWorlds[WorldTool.worldIndex].LevelLocked[%startIndex] = (WorldTool.worldIndex == 1 && %startIndex) ? 1 : 0;
    WorldTool.currentWorlds[WorldTool.worldIndex].LevelStars[%startIndex] = %levelStars;
    
    %temp = WorldTool.currentLevelList[%targetIndex];
    WorldTool.currentLevelList[%targetIndex] = WorldTool.currentLevelList[%startIndex];
    WorldTool.currentLevelList[%startIndex] = %temp;
}

/// <summary>
/// This function displays a level deletion confirmation dialog.  The dialog then handles
/// deletion or cancellation based on user input.
/// </summary>
function Wt_LevelRemove::onClick(%this)
{
    %levelName = WorldTool.currentLevelList[%this.index];
    Wt_ConfirmLevelDeleteGui.display("You are about to permanently delete " @ %levelName @ ".  Do you wish to continue?", "WorldTool", "deleteLevel", %this.index);
}

/// <summary>
/// This opens the projectile editor
/// </summary>
function Wt_ProjectileToolBtn::onClick(%this)
{
    // open projectile tool
    ProjectileTool.load();
    Tt_ProjectileToolButton.setStateOn(true);
}

/// <summary>
/// This handles selecting a launcher for use in a level from the Launcher Selection
/// dropdown.
/// </summary>
function Wt_LauncherDropdown::onSelect(%this)
{
    if (WorldTool.lastLevelName $= "")
        return;

    if (!$Wt_LauncherDropdownLoading && !$RefreshInProgress)
    {
        // handle launcher selection
        %name = %this.getText();

        %group = WorldTool.launcherSet.findObjectByInternalName(%name);
        SlingshotLauncherBuilder::setLevelLauncher(%group);

        // Save the level information
        WorldTool.saveData();
    }
}

/// <summary>
/// This handles selecting projectiles for the available slots in the selected level.
/// </summary>
function Wt_ProjectileDropdown::onSelect(%this)
{
    if (WorldTool.lastLevelName $= "" || !$Wt_ToolInitialized || $RefreshInProgress || %this.ignoreSelection)
        return;

    for ( %i = 1; %i < 6; %i++ )
    {
        %dropdown = "Wt_Projectile" @ %i @ "Dropdown";
        if ( %dropdown.getName() $= %this.getName() )
            continue;
        %selected = %this.getSelected();
        if ( %selected == %dropdown.getSelected() && %selected > 0 )
        {
            %this.ignoreSelection = true;
            %this.setSelected(0);
            %this.ignoreSelection = false;
        }
    }

    if ( %this.getSelected() > 0 )
        %this.setProfile("GuiPopUpMenuProfile");
    else
        %this.setProfile("GuiPopUpMenuEmptyProfile");

    WorldTool.refreshProjectilePane();
}

function Wt_ProjectileDropdown::initialize(%this)
{
    %this.clear();
    %this.setProfile("GuiPopUpMenuProfile");
    %this.add("Empty", 0);
    for (%j = 0; %j < WorldTool.projectileSet.getCount(); %j++)
    {
        %projectile = WorldTool.projectileSet.getObject(%j);
        if (%projectile !$= "")
        {
            %projectileName = %projectile.getInternalName();
            %this.add(%projectileName, %j + 1);
        }
    }
}

function Wt_ProjectileDropdown::select(%this, %text)
{
    %index = %this.findText(%text);
    %this.ignoreSelection = true;
    %this.setSelected(%index);
    %this.ignoreSelection = false;
}

/// <summary>
/// This handles increasing the reward target score for the associated reward stage
/// in the selected level.
/// </summary>
function Wt_RewardUpBtn::onClick(%this)
{
    %index = stripChars(%this.getName(), "Wt_RewardUpBtn");
    %edit = "Wt_Reward" @ %index @ "ValueEdit";

    %currentCount = %edit.getText();
    %currentCount++;

    if (%currentCount > 100000)
        %currentCount = 100000;

    %edit.setText(%currentCount);
    WorldTool.loadedLevel.RewardScore[%index - 1] = %currentCount;
}

/// <summary>
/// This handles decreasing the reward target score for the associated reward stage
/// in the selected level.
/// </summary>
function Wt_RewardDownBtn::onClick(%this)
{
    %index = stripChars(%this.getName(), "Wt_RewardDownBtn");
    %edit = "Wt_Reward" @ %index @ "ValueEdit";
    
    %currentCount = %edit.getText();
    %currentCount--;
    if (%currentCount < 0)
        %currentCount = 0;
    
    %edit.setText(%currentCount);
    WorldTool.loadedLevel.RewardScore[%index - 1] = %currentCount;
}

/// <summary>
/// This handles increasing the available number of projectiles for the associated 
/// projectile slot in the selected level.
/// </summary>
function Wt_ProjectileCountUpBtn::onClick(%this)
{
    %index = stripChars(%this.getName(), "Wt_ProjectileCountUpBtn");
    %edit = "Wt_Projectile" @ %index @ "CountEdit";
    %dropdown = "Wt_Projectile" @ %index @ "Dropdown";

    if (%dropdown.getText() $= "Empty")
        return;

    %currentCount = %edit.getText();
    %currentCount++;
    %edit.setText(%currentCount);

    %edit.onValidate();
    WorldTool.loadedLevel.NumAvailable[%index - 1] = %currentCount;
}

/// <summary>
/// This handles decreasing the available number of projectiles for the associated 
/// projectile slot in the selected level.
/// </summary>
function Wt_ProjectileCountDownBtn::onClick(%this)
{
    %index = stripChars(%this.getName(), "Wt_ProjectileCountDownBtn");
    %edit = "Wt_Projectile" @ %index @ "CountEdit";
    %dropdown = "Wt_Projectile" @ %index @ "Dropdown";

    if (%dropdown.getText() $= "Empty")
        return;

    %currentCount = %edit.getText();
    %currentCount--;
    %edit.setText(%currentCount);

    %edit.onValidate();
    WorldTool.loadedLevel.NumAvailable[%index - 1] = %currentCount;
}

function Wt_ProjectileCountEdit::onReturn(%this)
{
    %this.onValidate();
}

function Wt_ProjectileCountEdit::onValidate(%this)
{
    %currentCount = %this.getText();
    if (%currentCount < 1)
        %currentCount = 1;
    
    if (%currentCount > 99)
        %currentCount = 99;

    %this.setText(%currentCount);
}

/// <summary>
/// This opens the asset library for selection of music to play in the current level.
/// </summary>
function Wt_LevelMusicSelectBtn::onClick(%this)
{
    AssetPicker.open("AudioAsset", "Any", "", %this);
}

/// <summary>
/// This handles storing the selected music to the current level.
/// </summary>
function Wt_LevelMusicSelectBtn::setSelectedAsset(%this, %asset)
{
    WorldTool.loadedLevel.music = %asset;
    %temp = AssetDatabase.acquireAsset(%asset);
    Wt_LevelMusicEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(%asset);
}

/// <summary>
/// This plays the currently selected level's background music.
/// </summary>
function Wt_LevelMusicPlayBtn::onClick(%this)
{
    PhysicsLauncherTools::audioButtonPairClicked(%this, WorldTool.loadedLevel.music);
}

/// <summary>
/// This stops level music playback.
/// </summary>
function Wt_LevelMusicStopBtn::onClick(%this)
{
    PhysicsLauncherTools::audioButtonPairStop(Wt_LevelMusicPlayBtn);
}

/// <summary>
/// This launches the Level Builder Tool to edit the currently selected level.
/// </summary>
function WorldIconEditBtn::onClick(%this)
{
    Tt_InterfaceButton.setStateOn(true);
    InterfaceTool.load();
    InterfaceToolTabBook.selectPage(1);
}

/// <summary>
/// This handles the attached button's appearance state and lets the mouse
/// event control make it appear that the covered edit box control is an 
/// extension of the attached button.
/// </summary>
function Wt_LevelMusicEditEvent::onMouseEnter(%this)
{
    Wt_LevelMusicSelectBtn.setNormalImage(Wt_LevelMusicSelectBtn.HoverImage);
}

/// <summary>
/// This handles the attached button's appearance state and lets the mouse
/// event control make it appear that the covered edit box control is an 
/// extension of the attached button.
/// </summary>
function Wt_LevelMusicEditEvent::onMouseLeave(%this)
{
    Wt_LevelMusicSelectBtn.setNormalImage(Wt_LevelMusicSelectBtn.NormalImageCache);
}

/// <summary>
/// This handles the attached button's appearance state and lets the mouse
/// event control make it appear that the covered edit box control is an 
/// extension of the attached button.
/// </summary>
function Wt_LevelMusicEditEvent::onMouseDown(%this)
{
    Wt_LevelMusicSelectBtn.setNormalImage(Wt_LevelMusicSelectBtn.DownImage);
}

/// <summary>
/// This handles the attached button's appearance state and lets the mouse
/// event control make it appear that the covered edit box control is an 
/// extension of the attached button.
/// </summary>
function Wt_LevelMusicEditEvent::onMouseUp(%this)
{
    AssetLibrary.open(%this, $SoundsPage);
    Wt_LevelMusicSelectBtn.setNormalImage(Wt_LevelMusicSelectBtn.HoverImage);
}

/// <summary>
/// This handles storing the selected music to the current level.
/// </summary>
function Wt_LevelMusicEditEvent::setSelectedAsset(%this, %asset)
{
    WorldTool.loadedLevel.music = %asset;
    Wt_LevelMusicEdit.setText(%asset);
}

function Wt_LevelListScrollHandler(%childStart, %childRelStart, %childPos, %childRelPos)
{
    //echo(" @@@ Wt_LevelListScrollHandler("@%childStart@" "@%childRelStart@" "@%childPos@" "@%childRelPos@")");
}

function Wt_WorldListScrollHandler(%childStart, %childRelStart, %childPos, %childRelPos)
{
    //echo(" @@@ Wt_WorldListScrollHandler("@%childStart@" "@%childRelStart@" "@%childPos@" "@%childRelPos@")");
}

/// <summary>
/// This function is used to set the state of the projectile panes in the world tool based
/// on the projectile selection for the selected level.
/// States:
/// "normal" - the pane is displayed as active, all controls are active.
/// "firstEmpty" - the pane is displayed as active, only the projectile dropdown is active.
/// "empty" - the entire pane is displayed as inactive.
/// </summary>
/// <param name="pane">The pane to set state on.</param>
/// <param name="state">The desired pane state.</param>
function Wt_SetProjectilePaneState(%pane, %state)
{
    switch$(%state)
    {
        case "normal":
            %count = %pane.getCount();
            for (%i = 0; %i < %count; %i++)
            {
                %obj = %pane.getObject(%i);
                %obj.setActive(true);
                if (%obj.getClassName() $= "GuiPopUpMenuCtrl")
                {
                    %obj.setActive(true);
                    %obj.setProfile("GuiPopUpMenuProfile");
                    %text = %obj.getText();
                    %obj.setText(%text);
                }
                if (%obj.Profile $= "GuiInactiveSpinnerProfile")
                    %obj.Profile = "GuiSpinnerProfile";
                if (%obj.getClassName() $= "GuiTextCtrl")
                    %obj.Profile = "GuiModelessTextProfile";
            }

        case "firstEmpty":
            %count = %pane.getCount();
            for (%i = 0; %i < %count; %i++)
            {
                %obj = %pane.getObject(%i);
                %obj.setActive(false);
                if (%obj.getClassName() $= "GuiPopUpMenuCtrl")
                {
                    %obj.setActive(true);
                    %obj.setProfile("GuiPopUpMenuEmptyProfile");
                    %obj.setText("Empty");
                }
                if (%obj.Profile $= "GuiSpinnerProfile")
                    %obj.Profile = "GuiInactiveSpinnerProfile";
                if (%obj.getClassName() $= "GuiTextCtrl" && %obj.text $= "Amount")
                    %obj.Profile = "GuiModelessInactiveTextProfile";
                if (%obj.getClassName() $= "GuiTextCtrl" && %obj.text !$= "Amount")
                    %obj.Profile = "GuiModelessTextProfile";
            }

        case "empty":
            %count = %pane.getCount();
            for (%i = 0; %i < %count; %i++)
            {
                %obj = %pane.getObject(%i);
                %obj.setActive(false);
                if (%obj.getClassName() $= "GuiPopUpMenuCtrl")
                    %obj.setProfile("GuiPopUpMenuProfile");
                if (%obj.Profile $= "GuiSpinnerProfile")
                    %obj.Profile = "GuiInactiveSpinnerProfile";
                if (%obj.getClassName() $= "GuiTextCtrl")
                    %obj.Profile = "GuiModelessInactiveTextProfile";
            }
    }
}
