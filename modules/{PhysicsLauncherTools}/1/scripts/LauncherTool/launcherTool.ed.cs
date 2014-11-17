//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

$LauncherDefaultName = "New Launcher";

$LauncherToolObjectScrollView = createVerticalScrollContainer();

new ScriptObject(LauncherTool)
{
};

function LauncherToolGui::onSleep()
{
    // Write prefabs to file
    PhysicsLauncherTools::writePrefabs(); 
    
    if (isObject(LauncherTool.currentObject))
        SlingshotLauncherBuilder::updateLauncherPrefabInAllLevels(LauncherTool.currentObject);

    LauncherTool.helpManager.stop();
    LauncherTool.helpManager.delete();
    $LauncherToolObjectScrollView.delete();
}

function LauncherToolGui::onWake()
{
    LauncherTool.schedule(50, refresh); 
}

function LauncherTool::load(%this, %object)
{
    %this.launcherSimSet = "";
    %this.helpManager = createHelpMarqueeObject("LauncherToolTips", 10000, "{PhysicsLauncherTools}");
    %this.helpManager.openHelpSet("launcherToolHelp");
    %this.helpManager.start();

    if ( isObject($LauncherToolObjectScrollView) )
        $LauncherToolObjectScrollView.delete();
    $LauncherToolObjectScrollView = createVerticalScrollContainer(); 

    EditorShellGui.clearViews();

    EditorShellGui.addView($LauncherToolObjectScrollView, "smallMedium");
    $LauncherToolObjectScrollView.setSpacing(2);
    $LauncherToolObjectScrollView.setScrollCallbacks(true);
    $LauncherToolObjectScrollView.setIndicatorImage("{EditorAssets}:indicationArrowImage");
    $LauncherToolObjectScrollView.setNormalProfile(GuiLargePanelContainer);
    $LauncherToolObjectScrollView.setHighlightProfile(GuiLargePanelContainerHighlight);
    $LauncherToolObjectScrollView.addHeader(%this.createHeader("Launcher List"));

    EditorShellGui.addView(LauncherToolGui);

    %this.validateLauncherSet();

    %this.refreshObjectView();

    // Initialize the Pullback Effect Popup
    Lt_PullbackEffectPopup.clear();
    Lt_PullbackEffectPopup.add("Stretch", 0);
    Lt_PullbackEffectPopup.add("Stretch & Shrink", 1);

    // Select the first object in the list
    if (%object !$= "")
        $LauncherToolObjectScrollView.setSelected(%this.getLauncherIndex(%object));
    else
        $LauncherToolObjectScrollView.setSelected(0);

    // Initialize sound buttons
    PhysicsLauncherTools::audioButtonInitialize(Lt_PullbackSoundButtonPlay);
}

function LauncherTool::getLauncherIndex(%this, %data)
{
    %launcherGroup = $PrefabSet.findObjectByInternalName("LauncherSet");
    %objCount = %launcherGroup.getCount();
    %id = 0;
    for (%i = 0; %i < %objCount; %i++)
    {
        %obj = %launcherGroup.getObject(%i);
        if (%obj == %data)
        {
            %id = %i;
            break;
        }
    }
    return %id;
}

function LauncherTool::save(%this)
{
    // Check that we have a launcher group selected
    if (!isObject(%this.currentObject))
        return;

    // Save Name
    %name = Lt_NameField.getText();
    SlingshotLauncherBuilder::setName(%this.currentObject, %name);
    
    // Save Power
    %power = Lt_PowerField.getText();
    SlingshotLauncherBuilder::setPower(%this.currentObject, %power);
    
    // Save Stretch Distance
    %distance = Lt_StretchDistanceField.getText();
    SlingshotLauncherBuilder::setStretchDistance(%this.currentObject, %distance);
}

function LauncherTool::refresh(%this)
{
    // Check that we have a launcher group selected
    if (!isObject(%this.currentObject))
        return;

    // Refresh Name
    %name = SlingshotLauncherBuilder::getName(%this.currentObject);
    Lt_NameField.setText(%name);
    
    // Refresh Power
    %power = SlingshotLauncherBuilder::getPower(%this.currentObject);
    Lt_PowerField.setText(%power);
    
    // Refresh Stretch Distance
    %distance = SlingshotLauncherBuilder::getStretchDistance(%this.currentObject);
    Lt_StretchDistanceField.setText(%distance);
    
    // Refresh Pullback Effect
    %effectIndex = SlingshotLauncherBuilder::getPullbackEffect(%this.currentObject);
    Lt_PullbackEffectPopup.setSelected(%effectIndex);
    
    // Refresh Pullback Sound
    %soundAsset = SlingshotLauncherBuilder::getPullbackSound(%this.currentObject); 
    Lt_PullbackSoundFileField.setText(AssetDatabase.getAssetName(%soundAsset)); 
    
    // Refresh Fork Foreground
    %forkForeground = SlingshotLauncherBuilder::getForkForegroundAsset(%this.currentObject);
    Lt_ForkForegroundFileField.setText(AssetDatabase.getAssetName(%forkForeground));
    
    // Refresh Fork Background 
    %forkBackground = SlingshotLauncherBuilder::getForkBackgroundAsset(%this.currentObject);
    Lt_ForkBackgroundFileField.setText(AssetDatabase.getAssetName(%forkBackground));
    
    // Refresh Band0
    %band0 = SlingshotLauncherBuilder::getBandAsset(%this.currentObject, 0);
    Lt_BandForegroundFileField.setText(AssetDatabase.getAssetName(%band0));
    
    // Refresh Band1
    %band1 = SlingshotLauncherBuilder::getBandAsset(%this.currentObject, 1); 
    Lt_BandBackgroundFileField.setText(AssetDatabase.getAssetName(%band1));
    
    // Refresh Sling 
    %sling = SlingshotLauncherBuilder::getSeatAsset(%this.currentObject);
    Lt_SlingFileField.setText(AssetDatabase.getAssetName(%sling));
    
    // Preview Window
    refreshLauncherPreview(Lt_PreviewWindow, %this.currentObject);
    
    // Refresh Scroll bar preview
    %this.refreshScrollBarPreview();
}

/// <summary>
/// This creates a simple header gui control for use in the list containers.
/// </summary>
/// <param name="text">The text to assign to the header's label.</param>
function LauncherTool::createHeader(%this, %text)
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
        HorizSizing="left";
        VertSizing="top";
        Position="0 3";
        Extent=%control.Extent.x @ " 16";
        MinExtent="8 2";
        canSave = "0";
        Visible="1";
        hovertime="1000";
        maxLength="1024";
        text=%text;
        truncate="1";
    };
    %control.addGuiControl(%label);
    
    return %control;
}

function LauncherTool::refreshScrollBarPreview(%this)
{
    %launcherGroup = $PrefabSet.findObjectByInternalName("LauncherSet");
    
    for (%i = 0; %i < %launcherGroup.getCount(); %i++)
    {
        if (%launcherGroup.getObject(%i) == %this.currentObject)
        {
            %button = $LauncherToolObjectScrollView.getButton(%i);
            refreshLauncherPreview(%button.getObject(1), %this.currentObject);
            return;
        }
    }
    
    echo("LauncherTool::refreshScrollBarPreview - failed to find button to refresh for current object.");
}

//
//function LauncherTool::selectView(%this, %view)
//{
    //EditorShellGui.setView(%this.currentView.getId(), %view);
    //%this.currentView = %view;
//}

function LauncherTool::selectLauncherObject(%this, %data)
{
    %this.save(); 
    
    // Write prefabs to file
    PhysicsLauncherTools::writePrefabs();
    
    if (isObject(%this.currentObject))
        SlingshotLauncherBuilder::updateLauncherPrefabInAllLevels(%this.currentObject); 

    %words = getWordCount(%data);
    if (%words > 1)
    {
        %index = getWord(%data, 0);
        %object = getWord(%data, 1);
    }
    else
    {
        %launcherGroup = $PrefabSet.findObjectByInternalName("LauncherSet");
        %objCount = %launcherGroup.getCount();
        for (%i = 0; %i < %objCount; %i++)
        {
            %obj = %launcherGroup.getObject(%i);
            if (%obj == %data)
            {
                %id = %i;
                break;
            }
        }
        %index = %id;
        %object = %data;
    }

    %this.selectedIndex = %index;
    %this.currentObject = %object;   
    %this.refresh();
    %this.SetSelectedLauncherButton(%index);
}

function LauncherTool::createLauncherObject(%this, %data)
{
    %launcherGroup = $PrefabSet.findObjectByInternalName("LauncherSet");
    
    %newGroup = SlingshotLauncherBuilder::getNewSlingshotLauncher();
    
    if (isObject(%launcherGroup))
    {
        // Assign a default name to the new object
        %newName = $LauncherDefaultName;
        %nameCount = 1;
        while (isObject(%launcherGroup.findObjectByInternalName(%newName)))
        {
            %nameCount++;
            %newName = $LauncherDefaultName @ %nameCount;
        }
        
        %newGroup.setInternalName(%newName);
    
        // Add the object to the WorldObjectSet
        %launcherGroup.add(%newGroup);
    }
    
    %this.refreshObjectView();

    %this.selectLauncherObject($LauncherToolObjectScrollView.getCount() - 2 SPC %newGroup);
    $LauncherToolObjectScrollView.scrollToButton($LauncherToolObjectScrollView.getCount() - 2);
}

function LauncherTool::refreshObjectView(%this)
{
    if ( !isObject(%this.launcherSimSet) )
        %this.validateLauncherSet();

    $LauncherToolObjectScrollView.clear(); 

    // Add a button for each object
    for (%i = 0; %i < %this.launcherSimSet.getCount(); %i++)
    {
        %object = %this.launcherSimSet.getObject(%i);
        %button = %this.createObjectButtonGui(%object, %i);
        
        $LauncherToolObjectScrollView.addButton(%button, LauncherTool, "selectLauncherObject", %i SPC %object);
    }
    
    // Add "New" button
    %button = %this.createAddObjectButtonGui();
    $LauncherToolObjectScrollView.addButton(%button, LauncherTool, "createLauncherObject", "");
}

function LauncherTool::createObjectButtonGui(%this, %object, %index)
{
    if (%index == -1)    
        %buttonText = "Add New";
    else
        %buttonText = %object.getInternalName();

    %control = new GuiControl(){
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiLargeButtonContainer";
        HorizSizing="left";
        VertSizing="top";
        Position="0 0";
        Extent="122 90";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        hovertime="1000";
        index = %index;
    };
    
    %text = new GuiTextCtrl(){
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTextCenterProfile";
        HorizSizing="relative";
        VertSizing="relative";
        Position="4" SPC %control.extent.y - 16 - 4;
        Extent=%control.Extent.x - 8 @ " 16";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        hovertime="1000";
        text=%buttonText;
        maxLength="1024";
        truncate="1";
    };
    %control.addGuiControl(%text);
    %posY = %control.Extent.y - %text.Extent.y - 6;
    %extX = %control.Extent.x - 8;
    %text.setPosition(4, %posY);
    %text.setExtent(%extX, 18);
    
    %previewPosX = mRound(%control.extent.x/2 - 55/2);
    %previewPosY = 8;

    %preview = new SceneWindow()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position= %previewPosX SPC %previewPosY;
        Extent="55 55";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
        lockMouse="0";
        UseWindowInputEvents="1";
        UseObjectInputEvents="0";
    };
    %control.addGuiControl(%preview);
    %control.previewGui = %preview;    
    
    %preview.scene = new Scene()
    {
        cameraPosition = "0 0";
        cameraSize = "12 12";
        gravity = "0 0";
    };
    %preview.setScene(%preview.scene);
    initializeLauncherPreview(%preview);
    refreshLauncherPreview(%preview, %object);
    
    return %control;
}

function LauncherTool::createAddObjectButtonGui(%this)
{
    %control = new GuiControl(){
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiTransparentProfile";
        HorizSizing="left";
        VertSizing="top";
        Position="0 0";
        Extent="122 90";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Select to add a new Launcher.";
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

    %image = new GuiSpriteCtrl(){
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="left";
        VertSizing="top";
        Position="4 4";
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
    %addButton.addGuiControl(%image);

    %text = new GuiTextCtrl(){
        
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTextAddBtnProfile";
        HorizSizing="left";
        VertSizing="top";
        Position="8 12";
        Extent= (%control.Extent.x - %image.Extent.x - %image.Position.x - 6) @ " 16";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        hovertime="1000";
        text="Add New";
        maxLength="1024";
        truncate="1";
    };
    %addButton.addGuiControl(%text);
    %posX = %image.Position.x + %image.Extent.x + 4;
    %posY = (%image.Position.y + (%image.Extent.y / 2)) - (%text.Extent.y / 2);
    %text.setPosition(%posX, %posY);
    
    return %control;
}

function LauncherTool::SetSelectedLauncherButton(%this, %position)
{
    %targetButton = $LauncherToolObjectScrollView.getButton(%position);
    //%levelName = %this.currentWorlds[%this.worldIndex].LevelList[%position];

    if ( isObject ( %this.launcherSelectBtnCtrl ) )
    {
        if ($LauncherToolObjectScrollView.scrollCtrl.isMember(%this.launcherSelectBtnCtrl))
            $LauncherToolObjectScrollView.scrollCtrl.remove(%this.launcherSelectBtnCtrl);
        %this.launcherSelectBtnCtrl.delete();
    }
    %this.launcherSelectBtnCtrl = new GuiControl()
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
    %size = $LauncherToolObjectScrollView.contentPane.Extent;
    %this.launcherSelectBtnCtrl.setExtent(%size.x, %size.y);
    %posY = $LauncherToolObjectScrollView.scrollCtrl.getScrollPositionY();
    %this.launcherSelectBtnCtrl.setPosition(0, 0 - %posY);
    $LauncherToolObjectScrollView.scrollCtrl.add(%this.launcherSelectBtnCtrl);

    %this.CreateLauncherHighlightButton(%position);

    %this.selectedLauncherBtn.SetVisible(true);
}

function LauncherTool::CreateLauncherHighlightButton(%this, %i)
{
    if ( isObject( %this.selectedLauncherBtn ) )
        %this.selectedLauncherBtn.delete();

    %launcher = %this.launcherSimSet.getObject(%i); 

    %posY = $LauncherToolObjectScrollView.getButtonPosition(%i) + 2;

    %control = new GuiControl(){
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiLargePanelContainerHighlight";
        HorizSizing="relative";
        VertSizing="relative";
        Position="1 2";
        Extent="122 90";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        hovertime="1000";
        index = %index;
    };
    
    %text = new GuiTextCtrl(){
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTextCenterProfile";
        HorizSizing="left";
        VertSizing="top";
        Position="4" SPC %control.extent.y - 16 - 4;
        Extent=%control.Extent.x - 8 @ " 16";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        hovertime="1000";
        text=%launcher.getInternalName();
        maxLength="1024";
        truncate="1";
    };
    %control.addGuiControl(%text);
    %textPosY = %control.Extent.y - %text.Extent.y - 6;
    %textExtX = %control.Extent.x - 8;
    %text.setPosition(4, %textPosY);
    %text.setExtent(%textExtX, 18);
    %control.nameField = %text;

    %previewPosX = mRound(%control.extent.x/2 - 55/2);
    %previewPosY = 8;

    %preview = new SceneWindow()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position= %previewPosX SPC %previewPosY;
        Extent="55 55";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
        lockMouse="0";
        UseWindowInputEvents="1";
        UseObjectInputEvents="0";
    };
    %control.addGuiControl(%preview);
    %control.previewGui = %preview;    
    
    %preview.scene = new Scene()
    {
        cameraPosition = "0 0";
        cameraSize = "12 12";
        gravity = "0 0";
    };
    %preview.setScene(%preview.scene);
    initializeLauncherPreview(%preview);
    refreshLauncherPreview(%preview, %launcher);
    
    if ($LauncherToolObjectScrollView.getCount() > 2)
    {
        %remove = new GuiImageButtonCtrl()
        {
            canSaveDynamicFields="0";
            class="Lt_LauncherRemove";
            isContainer="0";
            Profile="GuiTransparentProfile";
            HorizSizing="right";
            VertSizing="bottom";
            Position="92 5";
            Extent="25 25";
            MinExtent="8 2";
            canSave = "0";
            Visible="1";
            hovertime="1000";
            toolTipProfile="GuiToolTipProfile";
            toolTip="Delete the selected Launcher.";
            groupNum="-1";
            buttonType="PushButton";
            useMouseEvents="0";
            isLegacyVersion="0";
            NormalImage="{EditorAssets}:redCloseImageMap";
            HoverImage="{EditorAssets}:redClose_hImageMap";
            DownImage="{EditorAssets}:redClose_dImageMap";
                index = %i;
        };
        %control.addGuiControl(%remove);
    }

    %this.selectedLauncherBtn = %control;
    %control.setPosition(0, %posY);
    %this.launcherSelectBtnCtrl.addGuiControl(%control);
}

function LauncherTool::validateLauncherSet(%this)
{
    %launcherSet = $PrefabSet.findObjectByInternalName("LauncherSet");
    if (!isObject(%launcherSet))
    {
        %launcherSet = new SimSet();
        %launcherSet.setInternalName("LauncherSet");
        %launcherSet.setName("LauncherSet");
        $PrefabSet.add(%launcherSet);
        %this.launcherSimSet = %launcherSet;
    }
    %this.launcherSimSet = %launcherSet;
}

function LauncherTool::onLauncherRemove(%this, %data)
{    
    %this.refreshObjectView();
    %this.refresh();
    
    if(%data > 0)
    {
       $LauncherToolObjectScrollView.setSelected(%data - 1);
       $LauncherToolObjectScrollView.scrollToButton(%data - 1);
    }
    else
    {
       $LauncherToolObjectScrollView.setSelected(%data);
       $LauncherToolObjectScrollView.scrollToButton(%data);
    }
}

//------------------------------------------------------------------------------
// Name
//------------------------------------------------------------------------------
function Lt_NameField::onReturn(%this)
{
    %this.onValidate();
}

function Lt_NameField::onValidate(%this)
{
    %launcherSet = LauncherTool.launcherSimSet;
    %index = LauncherTool.selectedIndex;
    %name = %this.getText();
    if (!isObject(%launcherSet.findObjectByInternalName(%name)))
    {
        SlingshotLauncherBuilder::setName(LauncherTool.currentObject, %name);    

        LauncherTool.save();
        LauncherTool.selectedLauncherBtn.nameField.text = %name;
        // Refresh the scroll view
        LauncherTool.refreshObjectView();
        $LauncherToolObjectScrollView.setSelected(%index);
    }
}

//------------------------------------------------------------------------------
// Power
//------------------------------------------------------------------------------
function Lt_PowerDownButton::onClick(%this)
{
    PhysicsLauncherTools::onFieldDownButton(Lt_PowerField);
}

function Lt_PowerUpButton::onClick(%this)
{
    PhysicsLauncherTools::onFieldUpButton(Lt_PowerField);
}

function Lt_PowerField::onValidate(%this)
{
    if (!isObject(LauncherTool.currentObject))
        return;
    
    PhysicsLauncherTools::validateIntField(%this, $SlingshotLauncherBuilder::ToolPowerMin, $SlingshotLauncherBuilder::ToolPowerMax);
    
    SlingshotLauncherBuilder::setPower(LauncherTool.currentObject, %this.getText());
}

//------------------------------------------------------------------------------
// Stretch Distance
//------------------------------------------------------------------------------
function Lt_StretchDistanceDownButton::onClick(%this)
{
    PhysicsLauncherTools::onFieldDownButton(Lt_StretchDistanceField);
}

function Lt_StretchDistanceUpButton::onClick(%this)
{
    PhysicsLauncherTools::onFieldUpButton(Lt_StretchDistanceField);
}

function Lt_StretchDistanceField::onValidate(%this)
{
    if (!isObject(LauncherTool.currentObject))
        return;
    
    PhysicsLauncherTools::validateIntField(%this, $SlingshotLauncherBuilder::ToolDistanceMin, $SlingshotLauncherBuilder::ToolDistanceMax);
    
    SlingshotLauncherBuilder::setStretchDistance(LauncherTool.currentObject, %this.getText());
        
    // Update Touch Target Shape
    SlingshotLauncherBuilder::updateTouchTargetShape(LauncherTool.currentObject);
    
    // Refresh Preview
    refreshLauncherPreview(Lt_PreviewWindow, LauncherTool.currentObject);
}


//------------------------------------------------------------------------------
// Pullback Effect
//------------------------------------------------------------------------------
function Lt_PullbackEffectPopup::onSelect(%this, %id, %text)
{
    SlingshotLauncherBuilder::setPullbackEffect(LauncherTool.currentObject, %id);
    
    // Refresh Preview
    refreshLauncherPreview(Lt_PreviewWindow, LauncherTool.currentObject);
    
    // Refresh Scroll bar preview
    LauncherTool.refreshScrollBarPreview();
}

//------------------------------------------------------------------------------
// Pullback Sound
//------------------------------------------------------------------------------
function Lt_PullbackSoundButtonPlay::onClick(%this)
{
    %soundAsset = SlingshotLauncherBuilder::getPullbackSound(LauncherTool.currentObject);
    PhysicsLauncherTools::audioButtonPairClicked(%this, %soundAsset);
}

function Lt_PullbackSoundButtonStop::onClick(%this)
{
    PhysicsLauncherTools::audioButtonPairStop(Lt_PullbackSoundButtonPlay);
}

function Lt_PullbackSoundFileButton::onClick(%this)
{
    AssetPicker.open("AudioAsset", "", "", %this);
}

function Lt_PullbackSoundFileButton::setSelectedAsset(%this, %asset)
{
    SlingshotLauncherBuilder::setPullbackSound(LauncherTool.currentObject, %asset);   
    
    // Refresh field text
    %soundAsset = SlingshotLauncherBuilder::getPullbackSound(LauncherTool.currentObject); 
    Lt_PullbackSoundFileField.setText(AssetDatabase.getAssetName(%soundAsset));
}

//------------------------------------------------------------------------------
// Fork Foreground
//------------------------------------------------------------------------------
function Lt_ForkForegroundFileButton::onClick(%this)
{
    AssetPicker.open("ImageAsset", "", "", %this);
}

function Lt_ForkForegroundFileButton::setSelectedAsset(%this, %asset, %frame)
{
    %currentObject = LauncherTool.currentObject;
    SlingshotLauncherBuilder::setForkForegroundAsset(%currentObject, %asset, %frame);
    
    LauncherTool.refresh();
    $LauncherToolObjectScrollView.setSelected(LauncherTool.selectedIndex);
}

//------------------------------------------------------------------------------
// Fork Background
//------------------------------------------------------------------------------
function Lt_ForkBackgroundFileButton::onClick(%this)
{
    AssetPicker.open("ImageAsset", "", "", %this);
}

function Lt_ForkBackgroundFileButton::setSelectedAsset(%this, %asset, %frame)
{
    %currentObject = LauncherTool.currentObject;
    SlingshotLauncherBuilder::setForkBackgroundAsset(%currentObject, %asset, %frame);

    LauncherTool.refresh();
    $LauncherToolObjectScrollView.setSelected(LauncherTool.selectedIndex);
}

//------------------------------------------------------------------------------
// Band (foreground)
//------------------------------------------------------------------------------
function Lt_BandForegroundFileButton::onClick(%this)
{
    AssetPicker.open("ImageAsset", "", "", %this);
}

function Lt_BandForegroundFileButton::setSelectedAsset(%this, %asset, %frame)
{
    %currentObject = LauncherTool.currentObject;
    SlingshotLauncherBuilder::setBandAsset(%currentObject, 0, %asset, %frame);

    LauncherTool.refresh();
    $LauncherToolObjectScrollView.setSelected(LauncherTool.selectedIndex);
}

//------------------------------------------------------------------------------
// Band (background)
//------------------------------------------------------------------------------
function Lt_BandBackgroundFileButton::onClick(%this)
{
    AssetPicker.open("ImageAsset", "", "", %this);
}

function Lt_BandBackgroundFileButton::setSelectedAsset(%this, %asset, %frame)
{
    %currentObject = LauncherTool.currentObject;
    SlingshotLauncherBuilder::setBandAsset(%currentObject, 1, %asset, %frame);

    LauncherTool.refresh();
    $LauncherToolObjectScrollView.setSelected(LauncherTool.selectedIndex);
}

//------------------------------------------------------------------------------
// Sling
//------------------------------------------------------------------------------
function Lt_SlingFileButton::onClick(%this)
{
    AssetPicker.open("ImageAsset", "", "", %this);
}

function Lt_SlingFileButton::setSelectedAsset(%this, %asset)
{
    %currentObject = LauncherTool.currentObject;
    SlingshotLauncherBuilder::setSeatAsset(%currentObject, %asset, %frame);

    LauncherTool.refresh();
    $LauncherToolObjectScrollView.setSelected(LauncherTool.selectedIndex);
}

//------------------------------------------------------------------------------
// Collision Editor
//------------------------------------------------------------------------------
function Lt_EditCollisionButton::onClick(%this)
{
    Tt_CollisionButton.setStateOn(true);
    SlingshotLauncherBuilder::openCollisionEditor(LauncherTool.currentObject, %this);
}

function Lt_EditCollisionButton::onCollisionEditSave(%this, %proxyObject)
{
    SlingshotLauncherBuilder::setCollisionShapesFromProxy(LauncherTool.currentObject, %proxyObject);
}

//------------------------------------------------------------------------------
// Removing Objects
//------------------------------------------------------------------------------
function Lt_LauncherRemove::onClick(%this)
{
    %group = LauncherTool.launcherSimSet.getObject(%this.index);
    
    // Remove the launcher group from the launcherSet
    LauncherTool.launcherSimSet.remove(%group);
    
    // Replace any references to the deleted launcher group
    SlingshotLauncherBuilder::replaceLauncherInAllLevels(%group, LauncherTool.launcherSimSet.getObject(0));
    
    // Remove group and all its children
    %child = %group.getObject(0);
    while (isObject(%child))
    {
        %group.remove(%child);
        %child.safeDelete();
        %child = %group.getObject(0);
    }
    
    // Delete the launcher group
    %group.delete();

    PhysicsLauncherToolsEventManager.postEvent("_LauncherRemoveRequest", %this.index);
}
