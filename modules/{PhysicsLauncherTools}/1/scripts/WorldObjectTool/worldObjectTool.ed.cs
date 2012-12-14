//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

$WorldObjectDefaultName = "New World Object";

$WorldObjectToolObjectScrollView = createVerticalScrollContainer(); 

new ScriptObject(WorldObjectTool)
{
};

function WorldObjectToolGui::onSleep()
{
    // Write prefabs to file
    WorldObjectTool.save();
    PhysicsLauncherTools::writePrefabs(); 
    WorldObjectTool.helpManager.stop();
    WorldObjectTool.helpManager.delete();
    $WorldObjectToolObjectScrollView.delete();
}

function WorldObjectTool::load(%this)
{  
    %this.helpManager = createHelpMarqueeObject("ObjectToolTips", 10000, "{PhysicsLauncherTools}");
    %this.helpManager.openHelpSet("objectToolHelp");
    %this.helpManager.start();

    %this.validateWorldObjectSet();
    %worldObjectSet = $PrefabSet.findObjectByInternalName("WorldObjectSet");    

    if ( isObject($WorldObjectToolObjectScrollView) )
        $WorldObjectToolObjectScrollView.delete();
    $WorldObjectToolObjectScrollView = createVerticalScrollContainer(); 

    EditorShellGui.clearViews();
    $WorldObjectToolObjectScrollView.setSpacing(2);
    $WorldObjectToolObjectScrollView.setScrollCallbacks(true);
    $WorldObjectToolObjectScrollView.setIndicatorImage("{EditorAssets}:indicationArrowImage");
    $WorldObjectToolObjectScrollView.setNormalProfile(GuiNarrowPanelContainer);
    $WorldObjectToolObjectScrollView.setHighlightProfile(GuiNarrowPanelContainerHighlight);
    $WorldObjectToolObjectScrollView.addHeader(%this.createHeader("World Object List"));
    EditorShellGui.addView($WorldObjectToolObjectScrollView, "medium");
    
    EditorShellGui.addView(WorldObjectToolGui, "large");    

    // Create World Object Tool Scene
    if (%this.previewScene $= "")
        %this.previewScene = new Scene();
        
    %sprite = new Sprite();
    %sprite.scene = %this.previewScene;
    //Wot_PreviewWindow.setSceneObject(%sprite);

    // Populate the scroll view with buttons
    %this.refreshObjectView();
    
    // Clear the appearance state popup and save a default value for the selected state 
    Wot_AppearanceStatePopup.clear();
    Wot_AppearanceStatePopup.setSelected(0);   
    
    // Select the object from the list
    if (WorldObjectTool.lastSelectedObject $= "")
        %this.selectObject(%worldObjectSet.getObject(0));
    else
        %this.selectObject(WorldObjectTool.lastSelectedObject);
    
    // Initialize sound buttons
    PhysicsLauncherTools::audioButtonInitialize(Wot_SoundPlayButton);
}

function WorldObjectTool::save(%this)
{
    %currentObject = %this.currentObject;
 
    if (!isObject(%currentObject)) 
        return;
        
    //
    // Properties
    //      
    
    // Save Name    
    Wot_NameField.onValidate();
    
    // Save Mass
    %massNumber = Wot_MassField.getText();
    WorldObjectBuilder::setMass(%currentObject, %massNumber);
    
    // Save Make Immovable
    %moveableFlag = Wot_MakeImmovableCheckbox.getValue() ? false : true;
    WorldObjectBuilder::setMoveableFlag(%currentObject, %moveableFlag);
    
    // Save Friction Level
    %frictionLevel = Wot_FrictionLevelField.getText();
    WorldObjectBuilder::setFrictionLevel(%currentObject, %frictionLevel);
    
    // Save Restitution Level
    %restitutionLevel = Wot_RestitutionLevelField.getText();
    WorldObjectBuilder::setRestitutionLevel(%currentObject, %restitutionLevel);
    
    // Save Hit Points
    %hitPoints = Wot_HitPointsField.getText();
    WorldObjectBuilder::setHitPoints(%currentObject, %hitPoints);
    
    // Save Total Points when Destroyed
    %pointsWhenDestroyed = Wot_PointsWhenDestroyedField.getText();
    WorldObjectBuilder::setPointsWhenDestroyed(%currentObject, %pointsWhenDestroyed);
    
    // Save Make Indestructable
    %indestructableFlag = Wot_MakeIndestructableCheckbox.getValue(); 
    WorldObjectBuilder::setIndestructableFlag(%currentObject, %indestructableFlag);

    // Save Damage State Count
    %damageStateCount = Wot_NumberOfDamageStatesField.getText();
    WorldObjectBuilder::setDamageStateCount(%currentObject, %damageStateCount);
    
    // Save Instant Kill by Projectiles flag
    %instantKillFlag = Wot_InstantKillCheckbox.getValue();
    WorldObjectBuilder::setInstantKillFlag(%currentObject, %instantKillFlag);
    
    // Save Win Condition flag
    %winConditionFlag = Wot_WinConditionCheckbox.getValue();
    WorldObjectBuilder::setWinConditionFlag(%currentObject, %winConditionFlag);
}

function WorldObjectTool::refresh(%this)
{
    %currentObject = %this.currentObject;
 
    if (!isObject(%currentObject)) 
        return;
    
    //
    // Properties
    //    
    
    // Refresh Name
    %name = WorldObjectBuilder::getName(%currentObject);
    Wot_NameField.setText(%name);
    
    // Refresh Mass
    %mass = WorldObjectBuilder::getMass(%currentObject);
    Wot_MassField.setText(%mass);
    
    // Refresh Make Immovable
    %moveableFlag = WorldObjectBuilder::getMoveableFlag(%currentObject);
    Wot_MakeImmovableCheckbox.setStateOn(%moveableFlag ? false : true);
    
    // Refresh Friction Level
    %frictionLevel = WorldObjectBuilder::getFrictionLevel(%currentObject);
    Wot_FrictionLevelField.setText(%frictionLevel);
    
    // Refresh Restitution Level
    %restitutionLevel = WorldObjectBuilder::getRestitutionLevel(%currentObject);
    Wot_RestitutionLevelField.setText(%restitutionLevel);
    
    // Refresh Hit Points
    %hitPoints = WorldObjectBuilder::getHitPoints(%currentObject);
    Wot_HitPointsField.setText(%hitPoints);
    
    // Refresh Total Points when Destroyed
    %pointsWhenDestroyed = WorldObjectBuilder::getPointsWhenDestroyed(%currentObject);
    Wot_PointsWhenDestroyedField.setText(%pointsWhenDestroyed);
    
    // Refresh Make Indestructable
    %indestructableFlag = WorldObjectBuilder::getIndestructableFlag(%currentObject);
    Wot_MakeIndestructableCheckbox.setStateOn(%indestructableFlag);

    // Refresh Damage State Count
    %damageStateCount = WorldObjectBuilder::getDamageStateCount(%currentObject);
    Wot_NumberOfDamageStatesField.setText(%damageStateCount);
    
    // Refresh Instant Kill by Projectiles flag
    %instantKillFlag = WorldObjectBuilder::getInstantKillFlag(%currentObject);
    Wot_InstantKillCheckbox.setStateOn(%instantKillFlag);
    
    // Refresh Win Condition flag
    %winConditionFlag = WorldObjectBuilder::getWinConditionFlag(%currentObject);
    Wot_WinConditionCheckbox.setStateOn(%winConditionFlag);
    
    // Update Active states on controls
    WorldObjectTool.updateControlActiveStates();
    
    //
    // Appearance
    //

    // Refresh Appearance State Popup Menu
    %selectedState = Wot_AppearanceStatePopup.getSelected();
    %this.refreshAppearanceState(%currentObject, %selectedState);
    %this.refreshAppearanceStatePopup(%currentObject, %selectedState);
}

function WorldObjectTool::selectObject(%this, %data)
{
    %this.save(); 
    
    // Write prefabs to file
    PhysicsLauncherTools::writePrefabs();   
    %object = getWord(%data, 0);
    %this.currentObject = %object;
    
    %this.lastSelectedObject = %object;  

    %index = getWord(%data, 1);
    %this.selectedIndex = %index;
    %updateScroll = false;
    if (%index $= "")
    {
        %worldObjectSet = $PrefabSet.findObjectByInternalName("WorldObjectSet");
        %count = %worldObjectSet.getCount();
        for ( %i = 0; %i < %count; %i++)
        {
            %obj = %worldObjectSet.getObject(%i);
            if (%obj.getId() == %object.getId())
            {
                %index = %i;
                %updateScroll = true;
                break;
            }
        }
    }
    %this.refresh();
    %this.SetSelectedObjectButton(%index);
    if (%updateScroll)
    {
        $WorldObjectToolObjectScrollView.scrollToButton(%index);
        $WorldObjectToolObjectScrollView.setIndicatorToButton(%index);
    }
}

function WorldObjectTool::createObject(%this, %data)
{
    %worldObjectSet = $PrefabSet.findObjectByInternalName("WorldObjectSet");
    
    // Create a new world object
    %newObject = WorldObjectBuilder::getNewWorldObject();
    
    if (isObject(%newObject))
    {
        // Assign a default name to the new object
        %newObjectName = $WorldObjectDefaultName;
        %nameCount = 1;
        while (isObject(%worldObjectSet.findObjectByInternalName(%newObjectName)))
        {
            %nameCount++;
            %newObjectName = $WorldObjectDefaultName @ %nameCount;
        }
        
        %newObject.setInternalName(%newObjectName);
    
        // Add the object to the WorldObjectSet
        %worldObjectSet.add(%newObject);
    }
    
    %this.refreshObjectView();
    %this.selectObject(%newObject SPC $WorldObjectToolObjectScrollView.getCount() - 2);
    $WorldObjectToolObjectScrollView.scrollToButton($WorldObjectToolObjectScrollView.getCount() - 2);
}

function WorldObjectTool::removeObject(%this, %object)
{
    // Remove objects that use the prefab from all levels
    PhysicsLauncherTools::purgePrefabFromAllLevels(%object.getName()); 
    
    %worldObjectSet.remove(%object);
    %object.safeDelete();
    
    WorldObjectTool.refreshObjectView();
    $WorldObjectToolObjectScrollView.setSelected(0);
}

function WorldObjectTool::refreshObjectView(%this)
{
    %worldObjectSet = $PrefabSet.findObjectByInternalName("WorldObjectSet");
    
    $WorldObjectToolObjectScrollView.clear();  
    
    // Only add delete flags if there are more than one objects in set
    if (%worldObjectSet.getCount() > 1)
        %addDeleteButton = true;
    else
        %addDeleteButton = false;
    
    // Add a button for each object
    for (%i = 0; %i < %worldObjectSet.getCount(); %i++)
    {
        %object = %worldObjectSet.getObject(%i);
        %button = %this.createObjectButtonGui(%object, %i, %addDeleteButton);
        
        $WorldObjectToolObjectScrollView.addButton(%button, WorldObjectTool, "selectObject", %object SPC %i);
    }
    
    // Add "New" button
    %button = %this.createAddObjectButtonGui();
    $WorldObjectToolObjectScrollView.addButton(%button, WorldObjectTool, "createObject", "");
}

/// <summary>
/// This creates a simple header gui control for use in the list containers.
/// </summary>
/// <param name="text">The text to assign to the header's label.</param>
function WorldObjectTool::createHeader(%this, %text)
{
    %control = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiTransparentProfile";
        HorizSizing="left";
        VertSizing="top";
        Position="0 0";
        Extent="266 24";
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
        Position="18 2";
        Extent="230 18";
        MinExtent="8 2";
        canSave = "0";
        Visible="1";
        hovertime="1000";
        maxLength="1024";
        text=%text;
    };
    %control.addGuiControl(%label);
    
    return %control;
}

function WorldObjectTool::createObjectButtonGui(%this, %object, %index, %addDeleteButton)
{
    %buttonText = "";
    if (isObject(%object)) 
        %buttonText = %object.getInternalName();
        
    // Get the image from the idle state of the object
    %asset = WorldObjectBuilder::getImageForState(%object, 0);
    %frame = WorldObjectBuilder::getImageFrameForState(%object, 0);
    
    %control = new GuiControl(){
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiLargePanelContainer";
        HorizSizing="left";
        VertSizing="top";
        Position="0 0";
        Extent="217 78";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        hovertime="1000";
    };

    %spriteContainer = new GuiControl(){
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTransparentProfile";
        HorizSizing="relative";
        VertSizing="relative";
        Position="11 11";
        Extent="55 55";
        MinExtent="8 2";
    };

    %image = new GuiSpriteCtrl(){
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTransparentProfile";
        HorizSizing="relative";
        VertSizing="relative";
        Position="0 0";
        Extent="55 55";
        MinExtent="8 2";
    };
        
    if (AssetDatabase.getAssetType(%asset) $= "AnimationAsset")
    {
        %image.Animation = %asset;
    }
    else  
    {  
        %image.Image = %asset;
        %image.Frame = %frame;
    }

    %aspectRatio = %object.getWidth()/%object.getHeight();
    PhysicsLauncherTools::setGuiControlAspectRatio(%image, %aspectRatio);

    %spriteContainer.add(%image);
    %control.addGuiControl(%spriteContainer);
    
    %text = new GuiTextCtrl(){
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTextProfile";
        HorizSizing="relative";
        VertSizing="relative";
        Position="70" SPC mRound(%control.extent.y/2) - 9;
        Extent="145 18";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        hovertime="1000";
        text=%buttonText;
        maxLength="1024";
        truncate="1";
    };
    %control.addGuiControl(%text);

    return %control;
}

function WorldObjectTool::createAddObjectButtonGui(%this)
{
    %control = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiTransparentProfile";
        HorizSizing="left";
        VertSizing="top";
        Position="0 0";
        Extent="217 78";
        MinExtent="8 2";
        canSave = "0";
        Visible="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Select to add a new Object.";
    };
    
    %addButton = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiNarrowPanelContainer";
        HorizSizing="left";
        VertSizing="top";
        Position="0 0";
        Extent="217 53";
        MinExtent="8 2";
        canSave = "0";
        Visible="1";
        hovertime="1000";
    };
    %control.addGuiControl(%addButton);
    
    %image = new GuiSpriteCtrl(){
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTransparentProfile";
        HorizSizing="relative";
        VertSizing="relative";
        Position="24 6";
        Extent="43 43";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        hovertime="1000";
        Image="{EditorAssets}:addButton_normalImageMap";
        wrap="0";
        useSourceRect="0";
        sourceRect="0 0 0 0";
    };
    %control.addGuiControl(%image);
    
    %text = new GuiTextCtrl(){
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTextProfile";
        HorizSizing="relative";
        VertSizing="relative";
        Position="75 4";
        Extent="143 43";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        hovertime="1000";
        text="Add New Object";
        maxLength="1024";
    };
    %control.addGuiControl(%text);
    
    return %control;
}

function WorldObjectTool::SetSelectedObjectButton(%this, %position)
{
    %targetButton = $WorldObjectToolObjectScrollView.getButton(%position);

    if ( isObject ( %this.objectSelectBtnCtrl ) )
    {
        if ($WorldObjectToolObjectScrollView.scrollCtrl.isMember(%this.objectSelectBtnCtrl))
            $WorldObjectToolObjectScrollView.scrollCtrl.remove(%this.objectSelectBtnCtrl);
        %this.objectSelectBtnCtrl.delete();
    }
    %this.objectSelectBtnCtrl = new GuiControl()
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
    %size = $WorldObjectToolObjectScrollView.contentPane.Extent;
    %this.objectSelectBtnCtrl.setExtent(%size.x, %size.y);
    %posY = $WorldObjectToolObjectScrollView.scrollCtrl.getScrollPositionY();
    %this.objectSelectBtnCtrl.setPosition(0, 0 - %posY);
    $WorldObjectToolObjectScrollView.scrollCtrl.add(%this.objectSelectBtnCtrl);

    %this.CreateObjectHighlightButton(%position);

    %this.selectedObjectBtn.SetVisible(true);
}

function WorldObjectTool::CreateObjectHighlightButton(%this, %i)
{
    if ( isObject( %this.selectedObjectBtn ) )
        %this.selectedObjectBtn.delete();

    %objectSet = $PrefabSet.findObjectByInternalName("WorldObjectSet");

    %object = %objectSet.getObject(%i); 

    %posY = $WorldObjectToolObjectScrollView.getButtonPosition(%i) + 2;

    // Get the image from the idle state of the projectile
    %asset = getWord(WorldObjectBuilder::getAnimationList(%object), 1);
    %frame = WorldObjectBuilder::getImageFrameForState(%object, 0);     
    
    %control = new GuiControl(){
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiLargePanelContainerHighlight";
        HorizSizing="right";
        VertSizing="bottom";
        Position="0" SPC %posY;
        Extent="218 78";
        MinExtent="8 2";
        canSave = "0";
        Visible="1";
        hovertime="1000";
    };

    %image = new GuiSpriteCtrl(){
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="9 9";
        Extent="60 60";
        MinExtent="8 2";
    };
    
    if (AssetDatabase.getAssetType(%asset) $= "AnimationAsset")
    {
        %image.Animation = %asset;
    }
    else  
    {  
        %image.Image = %asset;
        %image.Frame = %frame;
    }        
    
    %aspectRatio = %object.getWidth()/%object.getHeight();
    PhysicsLauncherTools::setGuiControlAspectRatio(%image, %aspectRatio);
    
    %control.addGuiControl(%image);
    
    %text = new GuiTextCtrl(){
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTextProfile";
        HorizSizing="relative";
        VertSizing="relative";
        Position="70" SPC mRound(%control.extent.y/2) - 9;
        Extent="133 18";
        MinExtent="8 2";
        canSave = "0";
        Visible="1";
        hovertime="1000";
        text=%object.getInternalName();
        maxLength="1024";
        truncate="1";
    };
    %control.addGuiControl(%text);
    %control.nameField = %text;

    if (%objectSet.getCount() > 1)
    {
        %remove = new GuiImageButtonCtrl()
        {
            canSaveDynamicFields="0";
            class="Wot_ObjectRemoveButton";
            isContainer="0";
            Profile="GuiTransparentProfile";
            HorizSizing="right";
            VertSizing="bottom";
            Position= %control.extent.x - 25 - 6 SPC 6;
            Extent="25 25";
            MinExtent="8 2";
            canSave="1";
            Visible="1";
            hovertime="1000";
            toolTipProfile="GuiToolTipProfile";
            toolTip="Delete the selected Object.";
            groupNum="-1";
            buttonType="PushButton";
            useMouseEvents="0";
            NormalImage="{EditorAssets}:redCloseImageMap";
            HoverImage="{EditorAssets}:redClose_hImageMap";
            DownImage="{EditorAssets}:redClose_dImageMap";
            InactiveImage="{EditorAssets}:redClose_iImageMap";
                index = %i;
        };
        %control.addGuiControl(%remove);
    }

    %this.selectedObjectBtn = %control;
    %control.setPosition(0, %posY);
    %this.objectSelectBtnCtrl.addGuiControl(%control);
}

function WorldObjectTool::validateWorldObjectSet()
{
    %worldObjectSet = $PrefabSet.findObjectByInternalName("WorldObjectSet");
    if (!isObject(%worldObjectSet))
    {
        %worldObjectSet = new SimSet();
        %worldObjectSet.setInternalName("WorldObjectSet");
        %worldObjectSet.setName("WorldObjectSet");
        $PrefabSet.add(%worldObjectSet);
    }
}


function WorldObjectTool::updateControlActiveStates(%this)
{
    %moveableFlag = Wot_MakeImmovableCheckbox.getValue() ? false : true;
    
    // Activate/deactivate mass controls
    Wot_MassField.setActive(%moveableFlag);
    Wot_MassDownButton.setActive(%moveableFlag);
    Wot_MassUpButton.setActive(%moveableFlag);
    
    %destructableFlag = Wot_MakeIndestructableCheckbox.getValue() ? false : true;    
    
    // Activate/Deactivate hitpoints controls
    Wot_HitPointsField.setActive(%destructableFlag);
    Wot_HitPointsDownButton.setActive(%destructableFlag);
    Wot_HitPointsUpButton.setActive(%destructableFlag);
    
    // Activate/Deactivate number of damage states controls
    Wot_NumberOfDamageStatesField.setActive(%destructableFlag);
    Wot_NumberOfDamageStatesDownButton.setActive(%destructableFlag);
    Wot_NumberOfDamageStatesUpButton.setActive(%destructableFlag);
    
    // Activate/Deactivate total points when destroyed controls
    Wot_PointsWhenDestroyedField.setActive(%destructableFlag);
    Wot_PointsWhenDestroyedDownButton.setActive(%destructableFlag);
    Wot_PointsWhenDestroyedUpButton.setActive(%destructableFlag);
}

function WorldObjectTool::refreshAppearanceStatePopup(%this, %currentObject, %selectedState)
{
    Wot_AppearanceStatePopup.clear();
    //%animationList = WorldObjectBuilder::getAnimationList(%currentObject);
    //for (%i = 0; %i < getFieldCount(%animationList)/2; %i++)
    //{
        //Wot_AppearanceStatePopup.add(getField(%animationList, %i*2), %i);
    //}
    
    // Add idle state to popup
    Wot_AppearanceStatePopup.add("Idle", 0);
    
    // Add damage states to popup
    for (%i = 0; %i < WorldObjectBuilder::getDamageStateCount(%currentObject); %i++)
    {
        Wot_AppearanceStatePopup.add("Damage State " @ %i + 1, %i + 1);
    }
    
    // Add disappear state to popup
    Wot_AppearanceStatePopup.add("Disappear", $WorldObjectBuilder::DisappearStateIndex);
    
    Wot_AppearanceStatePopup.setSelected(%selectedState);
}

function WorldObjectTool::refreshAppearanceState(%this, %currentObject, %selectedState)
{
    // Refresh Image for State
    %imageAsset = WorldObjectBuilder::getImageForState(%currentObject, %selectedState);
    Wot_ImageFileField.setText(AssetDatabase.getAssetName(%imageAsset));
    
    // Refresh Image Frame
    %imageFrame = WorldObjectBuilder::getImageFrameForState(%currentObject, %selectedState);  
    //Wot_ImageFrameField.setText(%imageFrame); 
    
    //%isStaticImage = AssetDatabase.getAssetType(%imageAsset) $= "ImageAsset";
    //Wot_ImageFrameContainer.setVisible(%isStaticImage);

    // Refresh play/stop buttons
    Wot_PreviewPlayButton.setVisible(!%isStaticImage);
    Wot_PreviewStopButton.setVisible(!%isStaticImage);
    
    // Refresh Sound for State
    %soundAsset = WorldObjectBuilder::getSoundForState(%currentObject, %selectedState);
    Wot_SoundFileField.setText(AssetDatabase.getAssetName(%soundAsset));   
    
    // Refresh PreviewWindow
    %this.refreshPreview(%imageAsset, %imageFrame); 
}

function WorldObjectTool::refreshPreview(%this, %asset, %frame)
{
    // Set the scene for the preview window
    if (!isObject(Wot_PreviewWindow.previewScene))
    {
        Wot_PreviewWindow.previewScene = new Scene()
        {
            cameraPosition = "0 0";
            cameraSize = "12 12";
            gravity = "0 0";
        }; 
        %windowExtents = Wot_PreviewWindow.getWindowExtents();
        %windowWidth = getWord(%windowExtents, 2);
        %windowHeight = getWord(%windowExtents, 3);
        
        %newMinX = (-1 * %windowWidth * $PhysicsLauncherTools::MetersPerPixel) / 2; 
        %newMaxX = (%windowWidth * $PhysicsLauncherTools::MetersPerPixel) / 2; 
        %newMinY = (-1 * %windowHeight * $PhysicsLauncherTools::MetersPerPixel) / 2;
        %newMaxY = (%windowHeight * $PhysicsLauncherTools::MetersPerPixel) / 2; 

        Wot_PreviewWindow.setCurrentCameraArea(%newMinX, %newMinY, %newMaxX, %newMaxY);  
    }
    Wot_PreviewWindow.setScene(Wot_PreviewWindow.previewScene);
    
    // Set the sprite
    if (!isObject(Wot_PreviewWindow.previewSprite))
    {
        Wot_PreviewWindow.previewSprite = new Sprite();
    }
    Wot_PreviewWindow.previewScene.add(Wot_PreviewWindow.previewSprite);
    

    // Set the asset    
    Wot_PreviewWindow.previewSprite.setAsset(%asset);
    %tempAsset = AssetDatabase.acquireAsset(%asset);
    %type = %tempAsset.getClassName();
    AssetDatabase.releaseAsset(%asset);
    switch$(%type)
    {
        case "ImageAsset":
            Wot_PreviewPlayButton.setActive(false);
            Wot_PreviewStopButton.setActive(false);

        case "AnimationAsset":
            Wot_PreviewPlayButton.setActive(true);
            Wot_PreviewStopButton.setActive(true);
    }

    // Size the sprite to the asset
    Wot_PreviewWindow.previewSprite.setSizeFromAsset(%asset, $PhysicsLauncherTools::MetersPerPixel);
    
    // Scale the asset if it doesn't fit in the window
    %windowWidth = Wot_PreviewWindow.extent.x * $PhysicsLauncherTools::MetersPerPixel;
    %windowHeight = Wot_PreviewWindow.extent.y * $PhysicsLauncherTools::MetersPerPixel;
    %scale = 1;
    %scale = mGetMin(%scale, %windowWidth / Wot_PreviewWindow.previewSprite.getWidth());
    %scale = mGetMin(%scale, %windowHeight / Wot_PreviewWindow.previewSprite.getHeight());

    Wot_PreviewWindow.previewSprite.setSize(Vector2Scale(Wot_PreviewWindow.previewSprite.getSize(), %scale));
    
    // Set the frame if the asset is an ImageAsset
    // Unpause the animation if the asset is an animation    
    if (AssetDatabase.getAssetType(%asset) $= "ImageAsset")
        Wot_PreviewWindow.previewSprite.setFrame(%frame);
    else
        Wot_PreviewWindow.previewSprite.pauseAnimation(false);
}


function Wot_AppearanceStatePopup::onSelect(%this)
{
    WorldObjectTool.refreshAppearanceState(WorldObjectTool.currentObject, Wot_AppearanceStatePopup.getSelected());
}

//------------------------------------------------------------------------------
// Name
//------------------------------------------------------------------------------
function Wot_NameField::onReturn(%this)
{
    %this.onValidate();
}

function Wot_NameField::onValidate(%this)
{
    %worldObjectSet = $PrefabSet.findObjectByInternalName("WorldObjectSet");
    
    %name = %this.getText();
    
    if (!isObject(%worldObjectSet.findObjectByInternalName(%name)))
    {
        WorldObjectBuilder::setName(WorldObjectTool.currentObject, %name);

        // Refresh the scroll view
        WorldObjectTool.refreshObjectView();
        $WorldObjectToolObjectScrollView.setSelected(WorldObjectTool.selectedIndex);
        $WorldObjectToolObjectScrollView.scrollToButton(WorldObjectTool.selectedIndex);
        WorldObjectTool.selectedObjectBtn.nameField.text = %name;
    }
    else
    {
        %this.setText(WorldObjectBuilder::getName(WorldObjectTool.currentObject));
    }
}

//------------------------------------------------------------------------------
// Mass
//------------------------------------------------------------------------------
function Wot_MassDownButton::onClick(%this)
{
    PhysicsLauncherTools::onFieldDownButton(Wot_MassField);
}

function Wot_MassUpButton::onClick(%this)
{
    PhysicsLauncherTools::onFieldUpButton(Wot_MassField);
}

function Wot_MassField::onValidate(%this)
{
    %currentObject = WorldObjectTool.currentObject;    
    
    PhysicsLauncherTools::validateIntField(%this, $WorldObjectBuilder::ToolMassMin, $WorldObjectBuilder::ToolMassMax);
    
    WorldObjectBuilder::setMass(%currentObject, %this.getText());
}

//------------------------------------------------------------------------------
// Make Immovable Checkbox
//------------------------------------------------------------------------------
function Wot_MakeImmovableCheckbox::onClick(%this)
{
    WorldObjectTool.updateControlActiveStates();
}

//------------------------------------------------------------------------------
// Make Indestructable Checkbox
//------------------------------------------------------------------------------
function Wot_MakeIndestructableCheckbox::onClick(%this)
{
    WorldObjectTool.updateControlActiveStates();
}

//------------------------------------------------------------------------------
// FrictionLevel
//------------------------------------------------------------------------------
function Wot_FrictionLevelDownButton::onClick(%this)
{
    PhysicsLauncherTools::onFieldDownButton(Wot_FrictionLevelField);
}

function Wot_FrictionLevelUpButton::onClick(%this)
{
    PhysicsLauncherTools::onFieldUpButton(Wot_FrictionLevelField);
}

function Wot_FrictionLevelField::onValidate(%this)
{
    %currentObject = WorldObjectTool.currentObject;    
    
    PhysicsLauncherTools::validateIntField(%this, $WorldObjectBuilder::ToolFrictionMin, $WorldObjectBuilder::ToolFrictionMax);
    
    WorldObjectBuilder::setFrictionLevel(%currentObject, %this.getText());
}

//------------------------------------------------------------------------------
// RestitutionLevel
//------------------------------------------------------------------------------
function Wot_RestitutionLevelDownButton::onClick(%this)
{
    PhysicsLauncherTools::onFieldDownButton(Wot_RestitutionLevelField);
}

function Wot_RestitutionLevelUpButton::onClick(%this)
{
    PhysicsLauncherTools::onFieldUpButton(Wot_RestitutionLevelField);
}

function Wot_RestitutionLevelField::onValidate(%this)
{
    %currentObject = WorldObjectTool.currentObject;    
    
    PhysicsLauncherTools::validateIntField(%this, $WorldObjectBuilder::ToolRestitutionMin, $WorldObjectBuilder::ToolRestitutionMax);
    
    WorldObjectBuilder::setRestitutionLevel(%currentObject, %this.getText());
}

//------------------------------------------------------------------------------
// HitPoints
//------------------------------------------------------------------------------
function Wot_HitPointsDownButton::onClick(%this)
{
    PhysicsLauncherTools::onFieldDownButton(Wot_HitPointsField);
}

function Wot_HitPointsUpButton::onClick(%this)
{
    PhysicsLauncherTools::onFieldUpButton(Wot_HitPointsField);
}

function Wot_HitPointsField::onValidate(%this)
{
    %currentObject = WorldObjectTool.currentObject;    
    
    PhysicsLauncherTools::validateIntField(%this, $WorldObjectBuilder::ToolHitPointsMin, $WorldObjectBuilder::ToolHitPointsMax);
    
    WorldObjectBuilder::setHitPoints(%currentObject, %this.getText());    
}

//------------------------------------------------------------------------------
// PointsWhenDestroyed
//------------------------------------------------------------------------------
function Wot_PointsWhenDestroyedDownButton::onClick(%this)
{
    PhysicsLauncherTools::onFieldDownButton(Wot_PointsWhenDestroyedField);
}

function Wot_PointsWhenDestroyedUpButton::onClick(%this)
{
    PhysicsLauncherTools::onFieldUpButton(Wot_PointsWhenDestroyedField);
}

function Wot_PointsWhenDestroyedField::onValidate(%this)
{
    %currentObject = WorldObjectTool.currentObject;    
    
    PhysicsLauncherTools::validateIntField(%this, $WorldObjectBuilder::ToolPointsWhenDestroyedMin, $WorldObjectBuilder::ToolPointsWhenDestroyedMax);
    
    WorldObjectBuilder::setPointsWhenDestroyed(%currentObject, %this.getText()); 
}

//------------------------------------------------------------------------------
// NumberOfDamageStates
//------------------------------------------------------------------------------
function Wot_NumberOfDamageStatesDownButton::onClick(%this)
{
    %oldDamageStateCount = WorldObjectBuilder::getDamageStateCount(WorldObjectTool.currentObject);    
    
    PhysicsLauncherTools::onFieldDownButton(Wot_NumberOfDamageStatesField);
    
    %newDamageStateCount = WorldObjectBuilder::getDamageStateCount(WorldObjectTool.currentObject); 
    
    //%selectedState = Wot_AppearanceStatePopup.getSelected();    
    
    // Check that the selected state is still valid
    %selectedState = 0;
    //if (%selectedState <= %oldDamageStateCount && %selectedState > %newDamageStateCount)
        //%selectedState = 0;
    //else if (%selectedState > %oldDamageStateCount && %oldDamageStateCount != %newDamageStateCount)
        //%selectedState = %newDamageStateCount + 1;
        
    WorldObjectTool.refreshAppearanceStatePopup(WorldObjectTool.currentObject, %selectedState);
    WorldObjectTool.refreshAppearanceState(WorldObjectTool.currentObject, %selectedState);
}

function Wot_NumberOfDamageStatesUpButton::onClick(%this)
{
    %oldDamageStateCount = WorldObjectBuilder::getDamageStateCount(WorldObjectTool.currentObject);
    
    PhysicsLauncherTools::onFieldUpButton(Wot_NumberOfDamageStatesField);
    
    %newDamageStateCount = WorldObjectBuilder::getDamageStateCount(WorldObjectTool.currentObject);
    
    //%selectedState = Wot_AppearanceStatePopup.getSelected();
    
    //if (%selectedState == %oldDamageStateCount + 1)
        //%selectedState = %newDamageStateCount + 1;
    %selectedState = 0;
        
    WorldObjectTool.refreshAppearanceStatePopup(WorldObjectTool.currentObject, %selectedState);
    WorldObjectTool.refreshAppearanceState(WorldObjectTool.currentObject, %selectedState);
}

function Wot_NumberOfDamageStatesField::onValidate(%this)
{
    %currentObject = WorldObjectTool.currentObject;    
    
    PhysicsLauncherTools::validateIntField(%this, $WorldObjectBuilder::ToolNumberOfDamageStatesMin, $WorldObjectBuilder::ToolNumberOfDamageStatesMax);
    
    WorldObjectBuilder::setDamageStateCount(%currentObject, %this.getText()); 
}

//------------------------------------------------------------------------------
// Preview
//------------------------------------------------------------------------------


function Wot_PreviewPlayButton::onClick(%this)
{    
    %imageAsset = WorldObjectBuilder::getImageForState(WorldObjectTool.currentObject, Wot_AppearanceStatePopup.getSelected());

    Wot_PreviewWindow.previewSprite.playAnimation(%imageAsset);
    Wot_PreviewWindow.previewSprite.pauseAnimation(false);
}

function Wot_PreviewStopButton::onClick(%this)
{
    Wot_PreviewWindow.previewSprite.pauseAnimation(true);
    Wot_PreviewWindow.previewSprite.setAnimationFrame(0);
}

//------------------------------------------------------------------------------
// Collision Editor Button
//------------------------------------------------------------------------------

function Wot_EditCollisionButton::onClick(%this)
{
    WorldObjectBuilder::openCollisionEditor(WorldObjectTool.currentObject, %this);
}

function Wot_EditCollisionButton::onCollisionEditSave(%this, %proxyObject)
{
    echo ("@@@ Wot_EditCollisionButton::onCollisionEditSave called...");   
    
    WorldObjectBuilder::setCollisionShapesFromProxy(WorldObjectTool.currentObject, %proxyObject);
}

//------------------------------------------------------------------------------
// Image File
//------------------------------------------------------------------------------


function Wot_ImageFileButton::onClick(%this)
{
    AssetPicker.open("AnimationAsset ImageAsset", "", "", %this);
}

function Wot_ImageFileButton::setSelectedAsset(%this, %assetID, %frame)
{
    %currentObject = WorldObjectTool.currentObject;
    %selectedState = Wot_AppearanceStatePopup.getSelected();
    WorldObjectBuilder::setImageForState(%currentObject, %selectedState, %assetID);
    WorldObjectBuilder::setImageFrameForState(%currentObject, %selectedState, %frame);
    %tempAsset = AssetDatabase.acquireAsset(%assetID);
    %type = %tempAsset.getClassName();
    AssetDatabase.releaseAsset(%assetID);
    switch$(%type)
    {
        case "ImageAsset":
            Wot_PreviewPlayButton.setActive(false);
            Wot_PreviewStopButton.setActive(false);

        case "AnimationAsset":
            Wot_PreviewPlayButton.setActive(true);
            Wot_PreviewStopButton.setActive(true);
    }
    
    // Refresh the Appearance state
    WorldObjectTool.refreshAppearanceState(%currentObject, %selectedState);
    
    // Refresh the scroll view
    WorldObjectTool.refreshObjectView();
    $WorldObjectToolObjectScrollView.setSelected(WorldObjectTool.selectedIndex);
    $WorldObjectToolObjectScrollView.scrollToButton(WorldObjectTool.selectedIndex);
}

function Wot_ImageFrameDownButton::onClick(%this)
{
    PhysicsLauncherTools::onFieldDownButton(Wot_ImageFrameField);
}

function Wot_ImageFrameUpButton::onClick(%this)
{
    PhysicsLauncherTools::onFieldUpButton(Wot_ImageFrameField);
}

function Wot_ImageFrameField::onValidate(%this)
{
    %currentObject = WorldObjectTool.currentObject;
    %selectedState = Wot_AppearanceStatePopup.getSelected();
    
    %frameCount = WorldObjectBuilder::getImageFrameCountForState(%currentObject, %selectedState);
    %frame = 0;    
    
    if (%frameCount != 0)
    {
        %frame = Wot_ImageFrameField.getText();
        
        if (%frame < 0)
            %frame = %frameCount - 1;
        
        if (%frame > %frameCount - 1)
            %frame = 0;
    }
    
    WorldObjectBuilder::setImageFrameForState(%currentObject, %selectedState, %frame);
    
    // Refresh the Appearance state
    WorldObjectTool.refreshAppearanceState(%currentObject, %selectedState);

    //// Refresh the scroll view
    //WorldObjectTool.refreshObjectView();
}

//------------------------------------------------------------------------------
// Sound File
//------------------------------------------------------------------------------
function Wot_SoundFileButton::onClick(%this)
{
    AssetPicker.open("AudioAsset", "", "", %this);
}

function Wot_SoundFileButton::setSelectedAsset(%this, %asset)
{
    %currentObject = WorldObjectTool.currentObject;
    %selectedState = Wot_AppearanceStatePopup.getSelected();
    
    WorldObjectBuilder::setSoundForState(%currentObject, %selectedState, %asset);    
    
    // Refresh the Appearance state
    WorldObjectTool.refreshAppearanceState(%currentObject, %selectedState);
}

function Wot_SoundPlayButton::onClick(%this)
{
    %currentObject = WorldObjectTool.currentObject;
    %selectedState = Wot_AppearanceStatePopup.getSelected();
    
    %soundAsset = WorldObjectBuilder::getSoundForState(%currentObject, %selectedState);
    PhysicsLauncherTools::audioButtonPairClicked(%this, %soundAsset);
}

function Wot_SoundStopButton::onClick(%this)
{
    PhysicsLauncherTools::audioButtonPairStop(Wot_SoundPlayButton);
}

//------------------------------------------------------------------------------
// Removing Objects
//------------------------------------------------------------------------------
function  Wot_ObjectRemoveButton::onClick(%this)
{
    %worldObjectSet = $PrefabSet.findObjectByInternalName("WorldObjectSet");
    %obj = %worldObjectSet.getObject(%this.index);

    Plt_ConfirmDeleteGui.display("", "WorldObjectTool", "removeObject", %obj);
}

