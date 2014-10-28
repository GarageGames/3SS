//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

$ProjectGuiPath = expandPath("^PhysicsLauncherTemplate/gui/");

$InterfaceToolInitialized = false;

//--------------------------------
// Interface Tool Help
//--------------------------------
/// <summary>
/// This function opens a browser and navigates to the Tower Defense Template 
/// Interface Tool help page.
/// </summary>
function GuiToolHelpButton::onClick(%this)
{
    gotoWebPage("http://docs.3stepstudio.com/towerdefense/interface/");
}

function InterfaceTool::load(%this)
{
    $InterfaceToolInitialized = false;
    EditorShellGui.clearViews();

    EditorShellGui.addView(InterfaceTool, "");
    
    InterfaceToolTabBook.selectPage(0);

    exec("^PhysicsLauncherTemplate/gui/MLTextCenteredLgProfile.cs");
    exec("^PhysicsLauncherTemplate/gui/TextCenteredLgProfile.cs");
    exec("^PhysicsLauncherTemplate/gui/MLTextCenteredSmProfile.cs");
    exec("^PhysicsLauncherTemplate/gui/TextCenteredSmProfile.cs");
    exec("^PhysicsLauncherTemplate/gui/TextLeftLgProfile.cs");
    exec("^PhysicsLauncherTemplate/gui/TextRightLgProfile.cs");
    exec("^PhysicsLauncherTemplate/gui/TextLeftSmProfile.cs");
    exec("^PhysicsLauncherTemplate/gui/TextRightSmProfile.cs");

    %this.worldData = TamlRead("^PhysicsLauncherTemplate/managed/worldList.taml");
    %this.initCreditsTab();
    %this.initHUDTab();
    %this.initLevelSelectTab();
    %this.initMenuTab();
    %this.initPauseTab();
    %this.initSettingsTab();
    %this.initWinTab();
    $InterfaceToolInitialized = true;
}

function InterfaceTool::onWake(%this)
{
    %this.getWorldData();
    $InterfaceToolInitialized = true;
}

function InterfaceTool::onSleep(%this)
{
    %this.saveData();
    alxStopAll();
    if ( isObject(%this.helpManager) )
    {
        %this.helpManager.stop();
        %this.helpManager.delete();
    }
    $InterfaceToolInitialized = false;
}

function InterfaceTool::initMenuTab(%this)
{
    $It_TabInitializing = true;

    if (!isObject(mainMenuGui))
        TamlRead("^PhysicsLauncherTemplate/gui/mainMenu.gui.taml");

    menuExitButton.setVisible(true);

    if (!isObject(helpGui))
        TamlRead("^PhysicsLauncherTemplate/gui/helpDialog.gui.taml");

    while (It_MenuPreviewContainer.getCount())
    {
        It_MenuPreviewContainer.getObject(0).delete();
    }

    %mainMenu = GuiUtils::duplicateGuiObject(mainMenuGui, "Preview", "It_GuiPreviewDuplicateButton");
    if ( It_MenuPreviewContainer.getCount() )
        It_MenuPreviewContainer.getObject(0).delete();
    GuiUtils::resizeGuiObject(%mainMenu, %mainMenu.Extent, It_MenuPreviewContainer.Extent);
    It_MenuPreviewContainer.addGuiControl(%mainMenu);

    It_CreditsToggleCheckbox.setStateOn(mainMenuGui.showCreditsButton);
    It_CreditsToggleCheckbox.onClick();

    initializeButtonStateDropdown(It_MenuPlayBtnStateDropdown);
    initializeButtonStateDropdown(It_MenuCreditsBtnStateDropdown);
    initializeButtonStateDropdown(It_MenuExitBtnStateDropdown);

    It_MenuSoundBtnStateDropdown.clear();
    It_MenuSoundBtnStateDropdown.add("On", 0);
    It_MenuSoundBtnStateDropdown.add("Off", 1);
    It_MenuSoundBtnStateDropdown.setFirstSelected();

    It_MenuBackgroundSelectEdit.stateAsset[0] = mainMenuBackground.Image;
    It_MenuBackgroundSelectEdit.stateControl = "mainMenuBackground";
    It_MenuBackgroundSelectEdit.stateField[0] = "Image";
    It_MenuBackgroundSelectEdit.preview = "mainMenuBackgroundPreview";
    It_MenuBackgroundSelectEdit.preview.setImage(It_MenuBackgroundSelectEdit.stateAsset[0]);
    %temp = AssetDatabase.acquireAsset(It_MenuBackgroundSelectEdit.stateAsset[0]);
    It_MenuBackgroundSelectEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(It_MenuBackgroundSelectEdit.stateAsset[0]);
    It_MenuBackgroundSelectEdit.type = "gui";

    It_MenuPlayBtnSelectEdit.stateControl = "menuPlayButton";
    It_MenuPlayBtnSelectEdit.stateAsset[0] = menuPlayButton.NormalImage;
    It_MenuPlayBtnSelectEdit.stateField[0] = "NormalImage";
    It_MenuPlayBtnSelectEdit.stateAsset[1] = menuPlayButton.HoverImage;
    It_MenuPlayBtnSelectEdit.stateField[1] = "HoverImage";
    It_MenuPlayBtnSelectEdit.stateAsset[2] = menuPlayButton.DownImage;
    It_MenuPlayBtnSelectEdit.stateField[2] = "DownImage";
    It_MenuPlayBtnSelectEdit.stateAsset[3] = menuPlayButton.InactiveImage;
    It_MenuPlayBtnSelectEdit.stateField[3] = "InactiveImage";
    It_MenuPlayBtnSelectEdit.assetSize = menuPlayButton.extent;
    It_MenuPlayBtnSelectEdit.preview = "menuPlayButtonPreview";
    It_MenuPlayBtnSelectEdit.preview.NormalImage = It_MenuPlayBtnSelectEdit.stateAsset[0];
    %temp = AssetDatabase.acquireAsset(It_MenuPlayBtnSelectEdit.stateAsset[0]);
    It_MenuPlayBtnSelectEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(It_MenuPlayBtnSelectEdit.stateAsset[0]);
    It_MenuPlayBtnSelectEdit.type = "gui";

    It_MenuExitBtnSelectEdit.stateControl = "menuExitButton";
    It_MenuExitBtnSelectEdit.stateAsset[0] = menuExitButton.NormalImage;
    It_MenuExitBtnSelectEdit.stateField[0] = "NormalImage";
    It_MenuExitBtnSelectEdit.stateAsset[1] = menuExitButton.HoverImage;
    It_MenuExitBtnSelectEdit.stateField[1] = "HoverImage";
    It_MenuExitBtnSelectEdit.stateAsset[2] = menuExitButton.DownImage;
    It_MenuExitBtnSelectEdit.stateField[2] = "DownImage";
    It_MenuExitBtnSelectEdit.stateAsset[3] = menuExitButton.InactiveImage;
    It_MenuExitBtnSelectEdit.stateField[3] = "InactiveImage";
    It_MenuExitBtnSelectEdit.assetSize = menuExitButton.extent;
    It_MenuExitBtnSelectEdit.preview = "menuExitButtonPreview";
    It_MenuExitBtnSelectEdit.preview.NormalImage = It_MenuExitBtnSelectEdit.stateAsset[0];
    %temp = AssetDatabase.acquireAsset(It_MenuExitBtnSelectEdit.stateAsset[0]);
    It_MenuExitBtnSelectEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(It_MenuExitBtnSelectEdit.stateAsset[0]);
    It_MenuExitBtnSelectEdit.type = "gui";

    It_MenuSoundBtnSelectEdit.stateControl = "mainMenuGui";
    It_MenuSoundBtnSelectEdit.stateAsset[0] = mainMenuGui.soundBtnOn;
    It_MenuSoundBtnSelectEdit.stateField[0] = "soundBtnOn";
    It_MenuSoundBtnSelectEdit.stateAsset[1] = mainMenuGui.soundBtnOff;
    It_MenuSoundBtnSelectEdit.stateField[1] = "soundBtnOff";
    It_MenuSoundBtnSelectEdit.assetSize = menuSoundButton.extent;
    It_MenuSoundBtnSelectEdit.preview = "menuSoundButtonPreview";
    It_MenuSoundBtnSelectEdit.preview.NormalImage = mainMenuGui.soundBtnOn;
    %temp = AssetDatabase.acquireAsset(It_MenuSoundBtnSelectEdit.stateAsset[0]);
    It_MenuSoundBtnSelectEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(It_MenuSoundBtnSelectEdit.stateAsset[0]);
    It_MenuSoundBtnSelectEdit.type = "gui";

    It_MenuCreditsBtnSelectEdit.stateControl = "menuCreditsButton";
    It_MenuCreditsBtnSelectEdit.stateAsset[0] = menuCreditsButton.NormalImage;
    It_MenuCreditsBtnSelectEdit.stateField[0] = "NormalImage";
    It_MenuCreditsBtnSelectEdit.stateAsset[1] = menuCreditsButton.HoverImage;
    It_MenuCreditsBtnSelectEdit.stateField[1] = "HoverImage";
    It_MenuCreditsBtnSelectEdit.stateAsset[2] = menuCreditsButton.DownImage;
    It_MenuCreditsBtnSelectEdit.stateField[2] = "DownImage";
    It_MenuCreditsBtnSelectEdit.stateAsset[3] = menuCreditsButton.InactiveImage;
    It_MenuCreditsBtnSelectEdit.stateField[3] = "InactiveImage";
    It_MenuCreditsBtnSelectEdit.assetSize = menuCreditsButton.extent;
    It_MenuCreditsBtnSelectEdit.preview = "menuCreditsButtonPreview";
    It_MenuCreditsBtnSelectEdit.preview.NormalImage = It_MenuCreditsBtnSelectEdit.stateAsset[0];
    %temp = AssetDatabase.acquireAsset(It_MenuCreditsBtnSelectEdit.stateAsset[0]);
    It_MenuCreditsBtnSelectEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(It_MenuCreditsBtnSelectEdit.stateAsset[0]);
    It_MenuCreditsBtnSelectEdit.type = "gui";

    $It_TabInitializing = false;
}

function It_CreditsToggleCheckbox::onClick(%this)
{
    %active = %this.getValue();
    mainMenuGui.showCreditsButton = %active;
    menuCreditsButtonPreview.setVisible(mainMenuGui.showCreditsButton);
    It_MenuCreditsBtnContainer.setActive(%active);
    %count = It_MenuCreditsBtnContainer.getCount();
    for (%i = 0; %i < %count; %i++)
    {
        %obj = It_MenuCreditsBtnContainer.getObject(%i);
        %obj.setActive(%active);
        if (%active && %obj.Profile $= "GuiFileEditInactiveProfile")
            %obj.Profile = "GuiFileEditBoxProfile";
        else if (!%active && %obj.Profile $= "GuiFileEditBoxProfile")
            %obj.Profile = "GuiFileEditInactiveProfile";
        if (%active && %obj.getClassName() $= "GuiSeparatorCtrl")
            %obj.Profile = "GuiTransparentProfile";
        else if (!%active && %obj.getClassName() $= "GuiSeparatorCtrl")
            %obj.Profile = "GuiSeparatorInactiveProfile";
        if (%active && %obj.getClassName() $= "GuiTextCtrl")
            %obj.Profile = "GuiTextProfile";
        else if (!%active && %obj.getClassName() $= "GuiTextCtrl")
            %obj.Profile = "GuiInactiveTextProfile";
    }
    if (%active)
        It_MenuCreditsBtnContainer.setProfile("GuiLargePanelContainer");
    else
        It_MenuCreditsBtnContainer.setProfile("GuiLargePanelContainerInactive");
    It_CreditsTab.setActive(%active);
}

function InterfaceTool::initWorldSelectTab(%this)
{
    $It_TabInitializing = true;

    %this.getWorldData();

    if (!isObject(worldSelectGui))
        TamlRead("^PhysicsLauncherTemplate/gui/worldSelect.gui.taml");

    %this.scoreText = "Score";
    %this.levelsText = "Levels";
    %this.totalScore = "0";
    %this.scoreCount = "0/0";

    initializeButtonStateDropdown(It_WorldNextPageBtnStateDropdown);
    initializeButtonStateDropdown(It_WorldPrevPageBtnStateDropdown);
    initializeButtonStateDropdown(It_WorldBackBtnStateDropdown);

    %this.populateWorldPane();

    It_WorldToggleCheckbox.setValue(worldSelectGui.showContainers);
    It_WorldToggleCheckbox.update();

    It_WorldNextPageBtnSelectEdit.stateControl = "WorldSelectNextBtn";
    It_WorldNextPageBtnSelectEdit.stateAsset[0] = WorldSelectNextBtn.NormalImage;
    It_WorldNextPageBtnSelectEdit.stateField[0] = "NormalImage";
    It_WorldNextPageBtnSelectEdit.stateAsset[1] = WorldSelectNextBtn.HoverImage;
    It_WorldNextPageBtnSelectEdit.stateField[1] = "HoverImage";
    It_WorldNextPageBtnSelectEdit.stateAsset[2] = WorldSelectNextBtn.DownImage;
    It_WorldNextPageBtnSelectEdit.stateField[2] = "DownImage";
    It_WorldNextPageBtnSelectEdit.stateAsset[3] = WorldSelectNextBtn.InactiveImage;
    It_WorldNextPageBtnSelectEdit.stateField[3] = "InactiveImage";
    It_WorldNextPageBtnSelectEdit.assetSize = WorldSelectNextBtn.extent;
    %temp = AssetDatabase.acquireAsset(It_WorldNextPageBtnSelectEdit.stateAsset[0]);
    It_WorldNextPageBtnSelectEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(It_WorldNextPageBtnSelectEdit.stateAsset[0]);
    It_WorldNextPageBtnSelectEdit.type = "gui";

    It_WorldPrevPageBtnSelectEdit.stateControl = "WorldSelectBackBtn";
    It_WorldPrevPageBtnSelectEdit.stateAsset[0] = WorldSelectBackBtn.NormalImage;
    It_WorldPrevPageBtnSelectEdit.stateField[0] = "NormalImage";
    It_WorldPrevPageBtnSelectEdit.stateAsset[1] = WorldSelectBackBtn.HoverImage;
    It_WorldPrevPageBtnSelectEdit.stateField[1] = "HoverImage";
    It_WorldPrevPageBtnSelectEdit.stateAsset[2] = WorldSelectBackBtn.DownImage;
    It_WorldPrevPageBtnSelectEdit.stateField[2] = "DownImage";
    It_WorldPrevPageBtnSelectEdit.stateAsset[3] = WorldSelectBackBtn.InactiveImage;
    It_WorldPrevPageBtnSelectEdit.stateField[3] = "InactiveImage";
    It_WorldPrevPageBtnSelectEdit.assetSize = WorldSelectBackBtn.extent;
    %temp = AssetDatabase.acquireAsset(It_WorldPrevPageBtnSelectEdit.stateAsset[0]);
    It_WorldPrevPageBtnSelectEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(It_WorldPrevPageBtnSelectEdit.stateAsset[0]);
    It_WorldPrevPageBtnSelectEdit.type = "gui";

    It_WorldBackBtnSelectEdit.stateControl = "WorldSelectHomeBtn";
    It_WorldBackBtnSelectEdit.stateAsset[0] = WorldSelectHomeBtn.NormalImage;
    It_WorldBackBtnSelectEdit.stateField[0] = "NormalImage";
    It_WorldBackBtnSelectEdit.stateAsset[1] = WorldSelectHomeBtn.HoverImage;
    It_WorldBackBtnSelectEdit.stateField[1] = "HoverImage";
    It_WorldBackBtnSelectEdit.stateAsset[2] = WorldSelectHomeBtn.DownImage;
    It_WorldBackBtnSelectEdit.stateField[2] = "DownImage";
    It_WorldBackBtnSelectEdit.stateAsset[3] = WorldSelectHomeBtn.InactiveImage;
    It_WorldBackBtnSelectEdit.stateField[3] = "InactiveImage";
    It_WorldBackBtnSelectEdit.assetSize = WorldSelectHomeBtn.extent;
    %temp = AssetDatabase.acquireAsset(It_WorldBackBtnSelectEdit.stateAsset[0]);
    It_WorldBackBtnSelectEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(It_WorldBackBtnSelectEdit.stateAsset[0]);
    It_WorldBackBtnSelectEdit.type = "gui";

    $It_TabInitializing = false;
}

function InterfaceTool::initLevelSelectTab(%this)
{
    $It_TabInitializing = true;

    %this.getWorldData();

    if (!isObject(levelSelectGui))
        TamlRead("^PhysicsLauncherTemplate/gui/levelSelect.gui.taml");

    initializeButtonStateDropdown(It_LevelNextPageBtnStateDropdown);
    initializeButtonStateDropdown(It_LevelPrevPageBtnStateDropdown);
    initializeButtonStateDropdown(It_LevelBackBtnStateDropdown);

    It_LevelSelectToggleCheckbox.setValue(levelSelectGui.showScore);
    %this.rewardCount = %this.worldData.rewardCount;
    %this.rewardImageSmall = levelSelectGui.rewardImage;
    %this.noRewardImageSmall = levelSelectGui.noRewardImage;

    It_LevelListWorldSelectDropdown.clear();
    for(%i = 1; %i < %this.worldData.getCount(); %i++)
    {
        %obj = %this.worldData.getObject(%i);
        It_LevelListWorldSelectDropdown.add(%obj.getInternalName(), %i);
        %this.levelTabSelectedWorld = %obj;
    }
    It_LevelListWorldSelectDropdown.setFirstSelected();
    It_LevelIconBtnSelectEdit.assetSize = "128 128";

    It_LevelPageIndicatorBtnStateDropdown.clear();
    It_LevelPageIndicatorBtnStateDropdown.add("Unselected", 0);
    It_LevelPageIndicatorBtnStateDropdown.add("Selected", 1);
    It_LevelPageIndicatorBtnStateDropdown.setSelected(0);

    It_LevelPageIndicatorBtnSelectEdit.stateControl = "levelSelectGui";
    It_LevelPageIndicatorBtnSelectEdit.stateAsset[0] = levelSelectGui.pageIndicatorOff;
    It_LevelPageIndicatorBtnSelectEdit.stateField[0] = "pageIndicatorOff";
    It_LevelPageIndicatorBtnSelectEdit.stateAsset[1] = levelSelectGui.pageIndicatorOn;
    It_LevelPageIndicatorBtnSelectEdit.stateField[1] = "pageIndicatorOn";
    It_LevelPageIndicatorBtnSelectEdit.assetSize = levelSelectGui.pageIndicatorSize;
    %temp = AssetDatabase.acquireAsset(It_LevelPageIndicatorBtnSelectEdit.stateAsset[0]);
    It_LevelPageIndicatorBtnSelectEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(It_LevelPageIndicatorBtnSelectEdit.stateAsset[0]);
    It_LevelPageIndicatorBtnSelectEdit.type = "gui";

    It_LevelSmallRewardBtnStateDropdown.clear();
    It_LevelSmallRewardBtnStateDropdown.add("Empty", 0);
    It_LevelSmallRewardBtnStateDropdown.add("Earned", 1);
    It_LevelSmallRewardBtnStateDropdown.setSelected(0);

    It_LevelSmallRewardBtnSelectEdit.stateControl = "levelSelectGui";
    It_LevelSmallRewardBtnSelectEdit.stateAsset[0] = levelSelectGui.noRewardImage;
    It_LevelSmallRewardBtnSelectEdit.stateField[0] = "noRewardImage";
    It_LevelSmallRewardBtnSelectEdit.stateAsset[1] = levelSelectGui.rewardImage;
    It_LevelSmallRewardBtnSelectEdit.stateField[1] = "rewardImage";
    It_LevelSmallRewardBtnSelectEdit.assetSize = levelSelectGui.rewardImageSize;
    %temp = AssetDatabase.acquireAsset(It_LevelSmallRewardBtnSelectEdit.stateAsset[0]);
    It_LevelSmallRewardBtnSelectEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(It_LevelSmallRewardBtnSelectEdit.stateAsset[0]);
    It_LevelSmallRewardBtnSelectEdit.type = "gui";

    It_LevelIconBtnStateDropdown.clear();
    It_LevelIconBtnStateDropdown.add("Unlocked", 0);
    It_LevelIconBtnStateDropdown.add("Locked", 1);
    It_LevelIconBtnStateDropdown.setSelected(0);

    It_LevelNextPageBtnSelectEdit.stateControl = "LevelSelectNextBtn";
    It_LevelNextPageBtnSelectEdit.stateAsset[0] = LevelSelectNextBtn.NormalImage;
    It_LevelNextPageBtnSelectEdit.stateField[0] = "NormalImage";
    It_LevelNextPageBtnSelectEdit.stateAsset[1] = LevelSelectNextBtn.HoverImage;
    It_LevelNextPageBtnSelectEdit.stateField[1] = "HoverImage";
    It_LevelNextPageBtnSelectEdit.stateAsset[2] = LevelSelectNextBtn.DownImage;
    It_LevelNextPageBtnSelectEdit.stateField[2] = "DownImage";
    It_LevelNextPageBtnSelectEdit.stateAsset[3] = LevelSelectNextBtn.InactiveImage;
    It_LevelNextPageBtnSelectEdit.stateField[3] = "InactiveImage";
    It_LevelNextPageBtnSelectEdit.assetSize = LevelSelectNextBtn.extent;
    %temp = AssetDatabase.acquireAsset(It_LevelNextPageBtnSelectEdit.stateAsset[0]);
    It_LevelNextPageBtnSelectEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(It_LevelNextPageBtnSelectEdit.stateAsset[0]);
    It_LevelNextPageBtnSelectEdit.type = "gui";

    It_LevelPrevPageBtnSelectEdit.stateControl = "LevelSelectBackBtn";
    It_LevelPrevPageBtnSelectEdit.stateAsset[0] = LevelSelectBackBtn.NormalImage;
    It_LevelPrevPageBtnSelectEdit.stateField[0] = "NormalImage";
    It_LevelPrevPageBtnSelectEdit.stateAsset[1] = LevelSelectBackBtn.HoverImage;
    It_LevelPrevPageBtnSelectEdit.stateField[1] = "HoverImage";
    It_LevelPrevPageBtnSelectEdit.stateAsset[2] = LevelSelectBackBtn.DownImage;
    It_LevelPrevPageBtnSelectEdit.stateField[2] = "DownImage";
    It_LevelPrevPageBtnSelectEdit.stateAsset[3] = LevelSelectBackBtn.InactiveImage;
    It_LevelPrevPageBtnSelectEdit.stateField[3] = "InactiveImage";
    It_LevelPrevPageBtnSelectEdit.assetSize = LevelSelectBackBtn.extent;
    %temp = AssetDatabase.acquireAsset(It_LevelPrevPageBtnSelectEdit.stateAsset[0]);
    It_LevelPrevPageBtnSelectEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(It_LevelPrevPageBtnSelectEdit.stateAsset[0]);
    It_LevelPrevPageBtnSelectEdit.type = "gui";

    It_LevelBackBtnSelectEdit.stateControl = "LevelSelectHomeBtn";
    It_LevelBackBtnSelectEdit.stateAsset[0] = LevelSelectHomeBtn.NormalImage;
    It_LevelBackBtnSelectEdit.stateField[0] = "NormalImage";
    It_LevelBackBtnSelectEdit.stateAsset[1] = LevelSelectHomeBtn.HoverImage;
    It_LevelBackBtnSelectEdit.stateField[1] = "HoverImage";
    It_LevelBackBtnSelectEdit.stateAsset[2] = LevelSelectHomeBtn.DownImage;
    It_LevelBackBtnSelectEdit.stateField[2] = "DownImage";
    It_LevelBackBtnSelectEdit.stateAsset[3] = LevelSelectHomeBtn.InactiveImage;
    It_LevelBackBtnSelectEdit.stateField[3] = "InactiveImage";
    It_LevelBackBtnSelectEdit.assetSize = LevelSelectHomeBtn.extent;
    %temp = AssetDatabase.acquireAsset(It_LevelBackBtnSelectEdit.stateAsset[0]);
    It_LevelBackBtnSelectEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(It_LevelBackBtnSelectEdit.stateAsset[0]);
    It_LevelBackBtnSelectEdit.type = "gui";

    %this.initLevelIcons();
    It_LevelSelectPageTabBook.selectPage(0);

    $It_TabInitializing = false;
}

function InterfaceTool::initLevelIcons(%this)
{
    %levelImage = %this.worldData.LevelImage;
    %levelLockedImage = %this.worldData.LevelLockedImage;
    if (%levelImage $= "")
        return;

    It_LevelIconBtnSelectEdit.stateControl = "%this.levelTabSelectedWorld";
    It_LevelIconBtnSelectEdit.stateAsset[0] = %levelImage;
    It_LevelIconBtnSelectEdit.stateField[0] = "LevelImageList";
    It_LevelIconBtnSelectEdit.stateAsset[1] = %levelLockedImage;
    It_LevelIconBtnSelectEdit.stateField[1] = "LevelLockedImage";
    
    %temp = AssetDatabase.acquireAsset(%levelImage);
    It_LevelIconBtnSelectEdit.setText(%temp.AssetName);
    %width = %temp.getCellWidth();
    %height = %temp.getCellHeight();
    if (%width < 1)
    {
        %width = %temp.getImageWidth();
        %height = %temp.getImageHeight();
    }
    
    It_LevelIconBtnSelectEdit.assetSize = %width SPC %height;
    It_LevelIconBtnSelectEdit.type = "gui";
    AssetDatabase.releaseAsset(%levelImage);
}

function InterfaceTool::setLevelIcons(%this, %icon, %lockedIcon)
{
    %this.worldData.LevelImage = %icon;
    %this.worldData.LevelLockedImage = %lockedIcon;

    %temp = AssetDatabase.acquireAsset(%icon);
    %width = %temp.getCellWidth();
    %height = %temp.getCellHeight();
    if (%width < 1)
    {
        %width = %temp.getImageWidth();
        %height = %temp.getImageHeight();
    }
    It_LevelIconBtnSelectEdit.setText(%temp.AssetName);
    It_LevelIconBtnSelectEdit.assetSize = %width SPC %height;
    AssetDatabase.releaseAsset(%icon);
    %paneCount = It_LevelListContentPane.getCount();
    for (%i = 0; %i < %paneCount; %i++)
    {
        for (%j = 1; %j < 16; %j++)
        {
            %levelButton = "LevelSelectButton" @ %j @ %i;
            if (%j < 3)
                %levelButton.NormalImage = %this.worldData.LevelImage;
            else
                %levelButton.NormalImage = %this.worldData.LevelLockedImage;
        }
    }
}

function InterfaceTool::initHUDTab(%this)
{
    $It_TabInitializing = true;

    if (!isObject(HudGui))
        TamlRead("^PhysicsLauncherTemplate/gui/hud.gui.taml");

    if (!isObject(helpGui))
        TamlRead("^PhysicsLauncherTemplate/gui/helpDialog.gui.taml");

    if (isObject(HudGuiPreview))
        HudGuiPreview.delete();
    if (isObject(helpGuiPreview))
        helpGuiPreview.delete();

    %sceneWindow = It_LevelPreviewWindow;
    It_HUDPreviewContainer.remove(%sceneWindow);
    while (It_HUDPreviewContainer.getCount())
    {
        It_HUDPreviewContainer.getObject(0).delete();
    }
    It_HUDPreviewContainer.add(%sceneWindow);

    %hud = GuiUtils::duplicateGuiObject(HudGui, "Preview", "It_GuiPreviewDuplicateButton");
    GuiUtils::resizeGuiObject(%hud, %hud.Extent, It_HUDPreviewContainer.Extent);
    It_HUDPreviewContainer.addGuiControl(%hud);

    %event = new GuiMouseEventCtrl()
    {
        canSaveDynamicFields = "0";
        isContainer = "0";
        class = "It_GuiContainerPreviewButton";
        Profile = "GuiTransparentProfile";
        HorizSizing = "relative";
        VertSizing = "relative";
        Extent = HudScoreContainerPreview.Extent;
        MinExtent = "8 2";
        canSave = "1";
        Visible = "1";
        hovertime = "1000";
        parentPane = HudScoreContainerPreview;
    };
    %event.setPosition(HudScoreContainerPreview.Position.x, HudScoreContainerPreview.Position.y);
    %hud.add(%event);

    %help = GuiUtils::duplicateGuiObject(helpGui, "Preview", "It_GuiPreviewDuplicateButton");
    GuiUtils::resizeGuiObject(%help, %help.Extent, It_HUDPreviewContainer.Extent);
    %projectileSet = $PrefabSet.findObjectByInternalName("ProjectileSet");
    %projectileName = %projectileSet.getObject(0).getName();
    helpScreenDisplayPreview.Image = TutorialDataBuilder::getTutorial(%projectileName@"Tutorial").Image[0];
    It_HUDPreviewContainer.addGuiControl(%help);

    helpBackgroundPreview.setProfile(GuiModelessDialogProfile);
    helpGui.setProfile(GuiModelessDialogProfile);

    %prefabSet = TamlRead("^PhysicsLauncherTemplate/managed/prefabs.taml");
    %projectileSet = %prefabSet.findObjectByInternalName("ProjectileSet");
    %projectile = %projectileSet.getObject(0);
    ProjectileSlot0Preview.NormalImage = %projectile.IconNormal;
    ProjectileSlot0Preview.HoverImage = %projectile.IconHover;
    ProjectileSlot0Preview.DownImage = %projectile.IconDepressed;

    initializeButtonStateDropdown(It_HudPauseBtnStateDropdown);
    initializeButtonStateDropdown(It_HudTutorialOKBtnStateDropdown);

    %this.loadLevelPreview();

    It_HudToggleCheckbox.setValue(HudGui.showScore);
    HudScoreContainerPreview.setVisible(HudGui.showScore);

    %hudPauseBtnImageUp = hudPauseButton.NormalImage;
    %hudPauseBtnImageHover = hudPauseButton.HoverImage;
    %hudPauseBtnImageDepressed = hudPauseButton.DownImage;
    %hudPauseBtnImageInactive = hudPauseButton.InactiveImage;
    It_HudPauseBtnSelectEdit.stateControl = "hudPauseButton";
    It_HudPauseBtnSelectEdit.stateAsset[0] = %hudPauseBtnImageUp;
    It_HudPauseBtnSelectEdit.stateField[0] = "NormalImage";
    It_HudPauseBtnSelectEdit.stateAsset[1] = %hudPauseBtnImageHover;
    It_HudPauseBtnSelectEdit.stateField[1] = "HoverImage";
    It_HudPauseBtnSelectEdit.stateAsset[2] = %hudPauseBtnImageDepressed;
    It_HudPauseBtnSelectEdit.stateField[2] = "DownImage";
    It_HudPauseBtnSelectEdit.stateAsset[3] = %hudPauseBtnImageInactive;
    It_HudPauseBtnSelectEdit.stateField[3] = "InactiveImage";
    It_HudPauseBtnSelectEdit.assetSize = hudPauseButton.extent;
    It_HudPauseBtnSelectEdit.preview = "hudPauseButtonPreview";
    %temp = AssetDatabase.acquireAsset(It_HudPauseBtnSelectEdit.stateAsset[0]);
    It_HudPauseBtnSelectEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(It_HudPauseBtnSelectEdit.stateAsset[0]);
    It_HudPauseBtnSelectEdit.type = "gui";

    It_HudTutorialOKBtnSelectEdit.stateControl = "helpOKButton";
    It_HudTutorialOKBtnSelectEdit.stateAsset[0] = helpOKButton.NormalImage;
    It_HudTutorialOKBtnSelectEdit.stateField[0] = "NormalImage";
    It_HudTutorialOKBtnSelectEdit.stateAsset[1] = helpOKButton.HoverImage;
    It_HudTutorialOKBtnSelectEdit.stateField[1] = "HoverImage";
    It_HudTutorialOKBtnSelectEdit.stateAsset[2] = helpOKButton.DownImage;
    It_HudTutorialOKBtnSelectEdit.stateField[2] = "DownImage";
    It_HudTutorialOKBtnSelectEdit.stateAsset[3] = helpOKButton.InactiveImage;
    It_HudTutorialOKBtnSelectEdit.stateField[3] = "InactiveImage";
    It_HudTutorialOKBtnSelectEdit.preview = "helpOKButtonPreview";
    It_HudTutorialOKBtnSelectEdit.assetSize = helpOKButton.extent;
    %temp = AssetDatabase.acquireAsset(It_HudTutorialOKBtnSelectEdit.stateAsset[0]);
    It_HudTutorialOKBtnSelectEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(It_HudTutorialOKBtnSelectEdit.stateAsset[0]);
    It_HudTutorialOKBtnSelectEdit.type = "gui";

    %scoreContainerImage = HudScoreContainer.Image;
    It_HudScoreContainerBtnSelectEdit.stateControl = "HudScoreContainer";
    It_HudScoreContainerBtnSelectEdit.stateAsset[0] = %scoreContainerImage;
    It_HudScoreContainerBtnSelectEdit.stateField[0] = "Image";
    It_HudScoreContainerBtnSelectEdit.assetSize = HudScoreContainer.extent;
    It_HudScoreContainerBtnSelectEdit.preview = "HudScoreContainerPreview";
    It_HudScoreContainerBtnSelectEdit.preview.setImage(It_HudScoreContainerBtnSelectEdit.stateAsset[0]);
    %temp = AssetDatabase.acquireAsset(It_HudScoreContainerBtnSelectEdit.stateAsset[0]);
    It_HudScoreContainerBtnSelectEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(It_HudScoreContainerBtnSelectEdit.stateAsset[0]);
    It_HudScoreContainerBtnSelectEdit.type = "gui";

    $It_TabInitializing = false;
}

function InterfaceTool::loadLevelPreview(%this, %levelName)
{
    if (!$InterfaceToolInitialized)
        return;

    // load It_LevelPreviewWindow
    %scene = It_LevelPreviewWindow.getScene();
    if (isObject(%scene))
        %scene.delete();

    %this.getWorldData();
    %world = %this.worldData.getObject(1);
    %this.levelPreview = TamlRead("^PhysicsLauncherTemplate/data/levels/" @ %world.LevelList[0] @ ".scene.taml");

    MainScene.setIsEditorScene(true);
    It_LevelPreviewWindow.setScene(MainScene);
    %sceneCameraSize = MainScene.cameraSize;
    It_LevelPreviewWindow.setCurrentCameraPosition(0, -2, %sceneCameraSize.x, %sceneCameraSize.y);
}

function InterfaceTool::initWinTab(%this)
{
    // NOTE! Much of the initialization for this page depends on the state of the
    // It_WinLoseBackgroundStateDropdown, such as the background to display, the
    // buttons to display and the initialization of these previews.
    $It_TabInitializing = true;

    if (!isObject(%this.worldData))
        %this.worldData = TamlRead("^PhysicsLauncherTemplate/managed/worldList.taml");

    if (!isObject(winGui))
        TamlRead("^PhysicsLauncherTemplate/gui/win.gui.taml");

    if (isObject(%this.tempWinGui))
        %this.tempWinGui.delete();
    %this.tempWinGui = GuiUtils::duplicateGuiObject(winGui, "Preview", "It_GuiPreviewDuplicateButton");
    GuiUtils::resizeGuiObject(%this.tempWinGui, %this.tempWinGui.Extent, It_WinLosePreviewContainer.Extent);

    if (!isObject(loseGui))
        TamlRead("^PhysicsLauncherTemplate/gui/lose.gui.taml");

    if (isObject(%this.tempLoseGui))
        %this.tempLoseGui.delete();
    %this.tempLoseGui = GuiUtils::duplicateGuiObject(loseGui, "Preview", "It_GuiPreviewDuplicateButton");
    GuiUtils::resizeGuiObject(%this.tempLoseGui, %this.tempLoseGui.Extent, It_WinLosePreviewContainer.Extent);

    It_WinLoseRewardsToggleCheckbox.setValue(winGui.showRewards);

    initializeButtonStateDropdown(It_WinLoseReplayBtnStateDropdown);
    initializeButtonStateDropdown(It_WinLoseLevelBtnStateDropdown);
    initializeButtonStateDropdown(It_WinLoseNextLevelBtnStateDropdown);

    It_WinLoseLargeRewardBtnStateDropdown.clear();
    It_WinLoseLargeRewardBtnStateDropdown.add("Empty", 0);
    It_WinLoseLargeRewardBtnStateDropdown.add("Earned", 1);
    It_WinLoseLargeRewardBtnStateDropdown.setSelected(0);

    It_WinLoseLargeRewardBtnSelectEdit.stateControl = "winGui";
    It_WinLoseLargeRewardBtnSelectEdit.stateAsset[0] = winGui.noRewardImage;
    It_WinLoseLargeRewardBtnSelectEdit.stateField[0] = "noRewardImage";
    It_WinLoseLargeRewardBtnSelectEdit.stateAsset[1] = winGui.rewardImage;
    It_WinLoseLargeRewardBtnSelectEdit.stateField[1] = "rewardImage";
    It_WinLoseLargeRewardBtnSelectEdit.assetSize = winGui.rewardImageSize;
    It_WinLoseLargeRewardBtnSelectEdit.preview = "RewardImage_0_preview";
    %temp = AssetDatabase.acquireAsset(It_WinLoseLargeRewardBtnSelectEdit.stateAsset[0]);
    It_WinLoseLargeRewardBtnSelectEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(It_WinLoseLargeRewardBtnSelectEdit.stateAsset[0]);
    It_WinLoseLargeRewardBtnSelectEdit.type = "gui";

    %this.rewardImageSize = winGui.rewardImageSize;
    %this.rewardCount = %this.worldData.rewardCount;
    %this.rewards = %this.createWinRewardImages();

    It_WinLoseBackgroundSelectEdit.stateControl = "winBackground";
    It_WinLoseBackgroundSelectEdit.stateAsset[0] = winBackground.Image;
    It_WinLoseBackgroundSelectEdit.stateField[0] = "Image";
    It_WinLoseBackgroundSelectEdit.stateAsset[1] = loseBackground.Image;
    It_WinLoseBackgroundSelectEdit.stateField[1] = "Image";
    It_WinLoseBackgroundSelectEdit.assetSize = winBackground.extent;
    It_WinLoseBackgroundSelectEdit.preview = "winBackgroundPreview";
    %temp = AssetDatabase.acquireAsset(It_WinLoseBackgroundSelectEdit.stateAsset[0]);
    It_WinLoseBackgroundSelectEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(It_WinLoseBackgroundSelectEdit.stateAsset[0]);
    It_WinLoseBackgroundSelectEdit.type = "gui";

    It_WinLoseBackgroundStateDropdown.clear();
    It_WinLoseBackgroundStateDropdown.add("Win Screen", 0);
    It_WinLoseBackgroundStateDropdown.add("Lose Screen", 1);
    It_WinLoseBackgroundStateDropdown.setFirstSelected();

    It_WinLoseReplayBtnSelectEdit.preview = winRestartButtonPreview;
    It_WinLoseReplayBtnSelectEdit.preview.NormalImage = It_WinLoseReplayBtnSelectEdit.stateAsset[0];
    It_WinLoseReplayBtnSelectEdit.type = "gui";

    It_WinLoseLevelBtnSelectEdit.preview = winLevelSelectButtonPreview;
    It_WinLoseLevelBtnSelectEdit.preview.NormalImage = It_WinLoseLevelBtnSelectEdit.stateAsset[0];
    It_WinLoseLevelBtnSelectEdit.type = "gui";

    It_WinLoseNextLevelBtnSelectEdit.preview = winNextLevelButtonPreview;
    It_WinLoseNextLevelBtnSelectEdit.preview.NormalImage = It_WinLoseNextLevelBtnSelectEdit.stateAsset[0];
    It_WinLoseNextLevelBtnSelectEdit.type = "gui";

    $It_TabInitializing = false;
}

function InterfaceTool::initPauseTab(%this)
{
    $It_TabInitializing = true;

    if (!isObject(pauseGui))
        TamlRead("^PhysicsLauncherTemplate/gui/pause.gui.taml");

    while (It_PausePreviewContainer.getCount())
    {
        It_PausePreviewContainer.getObject(0).delete();
    }

    %this.tempPauseGui = GuiUtils::duplicateGuiObject(pauseGui, "Preview", "It_GuiPreviewDuplicateButton");
    GuiUtils::resizeGuiObject(%this.tempPauseGui, %this.tempPauseGui.Extent, It_PausePreviewContainer.Extent);
    It_PausePreviewContainer.addGuiControl(%this.tempPauseGui);

    initializeButtonStateDropdown(It_PauseResumeBtnStateDropdown);
    initializeButtonStateDropdown(It_PauseRestartBtnStateDropdown);
    initializeButtonStateDropdown(It_PauseLevelBtnStateDropdown);
    initializeButtonStateDropdown(It_PauseHelpBtnStateDropdown);

    It_PauseSoundBtnStateDropdown.clear();
    It_PauseSoundBtnStateDropdown.add("On", 0);
    It_PauseSoundBtnStateDropdown.add("Off", 1);
    It_PauseSoundBtnStateDropdown.setSelected(0);

    It_PauseBackgroundSelectEdit.stateControl = "pauseBackground";
    It_PauseBackgroundSelectEdit.stateAsset[0] = pauseBackground.Image;
    It_PauseBackgroundSelectEdit.stateField[0] = "Image";
    It_PauseBackgroundSelectEdit.assetSize = "1024 768";
    It_PauseBackgroundSelectEdit.preview = "pauseBackgroundPreview";
    It_PauseBackgroundSelectEdit.preview.setImage(It_PauseBackgroundSelectEdit.stateAsset[0]);
    %temp = AssetDatabase.acquireAsset(It_PauseBackgroundSelectEdit.stateAsset[0]);
    It_PauseBackgroundSelectEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(It_PauseBackgroundSelectEdit.stateAsset[0]);
    It_PauseBackgroundSelectEdit.type = "gui";

    It_PauseResumeBtnSelectEdit.stateControl = "pauseResumeButton";
    It_PauseResumeBtnSelectEdit.stateAsset[0] = pauseResumeButton.NormalImage;
    It_PauseResumeBtnSelectEdit.stateField[0] = "NormalImage";
    It_PauseResumeBtnSelectEdit.stateAsset[1] = pauseResumeButton.HoverImage;
    It_PauseResumeBtnSelectEdit.stateField[1] = "HoverImage";
    It_PauseResumeBtnSelectEdit.stateAsset[2] = pauseResumeButton.DownImage;
    It_PauseResumeBtnSelectEdit.stateField[2] = "DownImage";
    It_PauseResumeBtnSelectEdit.stateAsset[3] = pauseResumeButton.InactiveImage;
    It_PauseResumeBtnSelectEdit.stateField[3] = "InactiveImage";
    It_PauseResumeBtnSelectEdit.assetSize = pauseResumeButton.extent;
    It_PauseResumeBtnSelectEdit.preview = "pauseResumeButtonPreview";
    It_PauseResumeBtnSelectEdit.preview.NormalImage = It_PauseResumeBtnSelectEdit.stateAsset[0];
    %temp = AssetDatabase.acquireAsset(It_PauseResumeBtnSelectEdit.stateAsset[0]);
    It_PauseResumeBtnSelectEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(It_PauseResumeBtnSelectEdit.stateAsset[0]);
    It_PauseResumeBtnSelectEdit.type = "gui";

    It_PauseRestartBtnSelectEdit.stateControl = "pauseRestartButton";
    It_PauseRestartBtnSelectEdit.stateAsset[0] = pauseRestartButton.NormalImage;
    It_PauseRestartBtnSelectEdit.stateField[0] = "NormalImage";
    It_PauseRestartBtnSelectEdit.stateAsset[1] = pauseRestartButton.HoverImage;
    It_PauseRestartBtnSelectEdit.stateField[1] = "HoverImage";
    It_PauseRestartBtnSelectEdit.stateAsset[2] = pauseRestartButton.DownImage;
    It_PauseRestartBtnSelectEdit.stateField[2] = "DownImage";
    It_PauseRestartBtnSelectEdit.stateAsset[3] = pauseRestartButton.InactiveImage;
    It_PauseRestartBtnSelectEdit.stateField[3] = "InactiveImage";
    It_PauseRestartBtnSelectEdit.assetSize = pauseRestartButton.extent;
    It_PauseRestartBtnSelectEdit.preview = "pauseRestartButtonPreview";
    It_PauseRestartBtnSelectEdit.preview.NormalImage = It_PauseRestartBtnSelectEdit.stateAsset[0];
    %temp = AssetDatabase.acquireAsset(It_PauseRestartBtnSelectEdit.stateAsset[0]);
    It_PauseRestartBtnSelectEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(It_PauseRestartBtnSelectEdit.stateAsset[0]);
    It_PauseRestartBtnSelectEdit.type = "gui";

    It_PauseLevelBtnSelectEdit.stateControl = "pauseLevelSelectButton";
    It_PauseLevelBtnSelectEdit.stateAsset[0] = pauseLevelSelectButton.NormalImage;
    It_PauseLevelBtnSelectEdit.stateField[0] = "NormalImage";
    It_PauseLevelBtnSelectEdit.stateAsset[1] = pauseLevelSelectButton.HoverImage;
    It_PauseLevelBtnSelectEdit.stateField[1] = "HoverImage";
    It_PauseLevelBtnSelectEdit.stateAsset[2] = pauseLevelSelectButton.DownImage;
    It_PauseLevelBtnSelectEdit.stateField[2] = "DownImage";
    It_PauseLevelBtnSelectEdit.stateAsset[3] = pauseLevelSelectButton.InactiveImage;
    It_PauseLevelBtnSelectEdit.stateField[3] = "InactiveImage";
    It_PauseLevelBtnSelectEdit.assetSize = pauseLevelSelectButton.extent;
    It_PauseLevelBtnSelectEdit.preview = "pauseLevelSelectButtonPreview";
    It_PauseLevelBtnSelectEdit.preview.NormalImage = It_PauseLevelBtnSelectEdit.stateAsset[0];
    %temp = AssetDatabase.acquireAsset(It_PauseLevelBtnSelectEdit.stateAsset[0]);
    It_PauseLevelBtnSelectEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(It_PauseLevelBtnSelectEdit.stateAsset[0]);
    It_PauseLevelBtnSelectEdit.type = "gui";

    It_PauseHelpBtnSelectEdit.stateControl = "pauseHelpButton";
    It_PauseHelpBtnSelectEdit.stateAsset[0] = pauseHelpButton.NormalImage;
    It_PauseHelpBtnSelectEdit.stateField[0] = "NormalImage";
    It_PauseHelpBtnSelectEdit.stateAsset[1] = pauseHelpButton.HoverImage;
    It_PauseHelpBtnSelectEdit.stateField[1] = "HoverImage";
    It_PauseHelpBtnSelectEdit.stateAsset[2] = pauseHelpButton.DownImage;
    It_PauseHelpBtnSelectEdit.stateField[2] = "DownImage";
    It_PauseHelpBtnSelectEdit.stateAsset[3] = pauseHelpButton.InactiveImage;
    It_PauseHelpBtnSelectEdit.stateField[3] = "InactiveImage";
    It_PauseHelpBtnSelectEdit.assetSize = pauseHelpButton.extent;
    It_PauseHelpBtnSelectEdit.preview = "pauseHelpButtonPreview";
    It_PauseHelpBtnSelectEdit.preview.NormalImage = It_PauseHelpBtnSelectEdit.stateAsset[0];
    %temp = AssetDatabase.acquireAsset(It_PauseHelpBtnSelectEdit.stateAsset[0]);
    It_PauseHelpBtnSelectEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(It_PauseHelpBtnSelectEdit.stateAsset[0]);
    It_PauseHelpBtnSelectEdit.type = "gui";

    It_PauseSoundBtnSelectEdit.stateControl = "pauseGui";
    It_PauseSoundBtnSelectEdit.stateAsset[0] = pauseGui.soundBtnOn;
    It_PauseSoundBtnSelectEdit.stateField[0] = "soundBtnOn";
    It_PauseSoundBtnSelectEdit.stateAsset[1] = pauseGui.soundBtnOff;
    It_PauseSoundBtnSelectEdit.stateField[1] = "soundBtnOff";
    It_PauseSoundBtnSelectEdit.assetSize = pauseSoundButton.extent;
    It_PauseSoundBtnSelectEdit.preview = "pauseSoundButtonPreview";
    It_PauseSoundBtnSelectEdit.preview.NormalImage = It_PauseSoundBtnSelectEdit.stateAsset[0];
    %temp = AssetDatabase.acquireAsset(It_PauseSoundBtnSelectEdit.stateAsset[0]);
    It_PauseSoundBtnSelectEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(It_PauseSoundBtnSelectEdit.stateAsset[0]);
    It_PauseSoundBtnSelectEdit.type = "gui";

    $It_TabInitializing = false;
}

function InterfaceTool::initCreditsTab(%this)
{
    $It_TabInitializing = true;

    if (!isObject(creditsGui))
        TamlRead("^PhysicsLauncherTemplate/gui/credits.gui.taml");

    %this.tempCreditsGui = GuiUtils::duplicateGuiObject(creditsGui, "Preview", "It_GuiPreviewDuplicateButton");
    GuiUtils::resizeGuiObject(%this.tempCreditsGui, %this.tempCreditsGui.Extent, It_CreditsPreviewContainer.Extent);
    if (It_CreditsPreviewContainer.getCount())
        It_CreditsPreviewContainer.getObject(0).delete();
    It_CreditsPreviewContainer.addGuiControl(%this.tempCreditsGui);

    initializeButtonStateDropdown(It_CreditsCloseBtnStateDropdown);

    It_CreditsBackgroundSelectEdit.stateControl = "CreditsBackground";
    It_CreditsBackgroundSelectEdit.stateAsset[0] = CreditsBackground.Image;
    It_CreditsBackgroundSelectEdit.stateField[0] = "Image";
    It_CreditsBackgroundSelectEdit.assetSize = CreditsBackground.extent;
    It_CreditsBackgroundSelectEdit.preview = "creditsBackgroundPreview";
    It_CreditsBackgroundSelectEdit.preview.setImage(It_CreditsBackgroundSelectEdit.stateAsset[0]);
    %temp = AssetDatabase.acquireAsset(It_CreditsBackgroundSelectEdit.stateAsset[0]);
    It_CreditsBackgroundSelectEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(It_CreditsBackgroundSelectEdit.stateAsset[0]);
    It_CreditsBackgroundSelectEdit.type = "gui";

    It_CreditsOverlaySelectEdit.stateControl = "creditsGuiCreditsImage";
    It_CreditsOverlaySelectEdit.stateAsset[0] = creditsGuiCreditsImage.Image;
    It_CreditsOverlaySelectEdit.stateField[0] = "Image";
    It_CreditsOverlaySelectEdit.assetSize = creditsGuiCreditsImage.extent;
    It_CreditsOverlaySelectEdit.preview = "creditsGuiCreditsImagePreview";
    It_CreditsOverlaySelectEdit.preview.setImage(It_CreditsOverlaySelectEdit.stateAsset[0]);
    %temp = AssetDatabase.acquireAsset(It_CreditsOverlaySelectEdit.stateAsset[0]);
    It_CreditsOverlaySelectEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(It_CreditsOverlaySelectEdit.stateAsset[0]);
    It_CreditsOverlaySelectEdit.type = "gui";

    It_CreditsCloseBtnSelectEdit.stateControl = "closeCreditsButton";
    It_CreditsCloseBtnSelectEdit.stateAsset[0] = closeCreditsButton.NormalImage;
    It_CreditsCloseBtnSelectEdit.stateField[0] = "NormalImage";
    It_CreditsCloseBtnSelectEdit.stateAsset[1] = closeCreditsButton.HoverImage;
    It_CreditsCloseBtnSelectEdit.stateField[1] = "HoverImage";
    It_CreditsCloseBtnSelectEdit.stateAsset[2] = closeCreditsButton.DownImage;
    It_CreditsCloseBtnSelectEdit.stateField[2] = "DownImage";
    It_CreditsCloseBtnSelectEdit.stateAsset[3] = closeCreditsButton.InactiveImage;
    It_CreditsCloseBtnSelectEdit.stateField[3] = "InactiveImage";
    It_CreditsCloseBtnSelectEdit.assetSize = closeCreditsButton.extent;
    It_CreditsCloseBtnSelectEdit.preview = "closeCreditsButtonPreview";
    It_CreditsCloseBtnSelectEdit.preview.NormalImage = It_CreditsCloseBtnSelectEdit.stateAsset[0];
    %temp = AssetDatabase.acquireAsset(It_CreditsCloseBtnSelectEdit.stateAsset[0]);
    It_CreditsCloseBtnSelectEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(It_CreditsCloseBtnSelectEdit.stateAsset[0]);
    It_CreditsCloseBtnSelectEdit.type = "gui";

    $It_TabInitializing = false;
}

function InterfaceTool::initSettingsTab(%this)
{
    $It_TabInitializing = true;

    if ( !isObject(GuiGameMLTextCenteredLgProfile) )
        exec(expandPath("^PhysicsLauncherTemplate/gui/MLTextCenteredLgProfile.cs"));
    if ( !isObject(GuiGameMLTextCenteredSmProfile) )
        exec(expandPath("^PhysicsLauncherTemplate/gui/MLTextCenteredSmProfile.cs"));

    if (%this.fontList $= "")
    {
        It_SettingsLgFontSelectDropdown.clear();
        It_SettingsSmFontSelectDropdown.clear();
        %fontCount = AvailableFonts.getCount();
        for (%i = 0; %i < %fontCount; %i++)
        {
            It_SettingsLgFontSelectDropdown.add(AvailableFonts.getObject(%i).Font, %i);
            It_SettingsSmFontSelectDropdown.add(AvailableFonts.getObject(%i).Font, %i);
        }
        It_SettingsLgFontSelectDropdown.sort();
        It_SettingsLgFontSelectDropdown.setSelected(It_SettingsLgFontSelectDropdown.findText(GuiGameMLTextCenteredLgProfile.fontType));
        It_SettingsSmFontSelectDropdown.sort();
        It_SettingsSmFontSelectDropdown.setSelected(It_SettingsSmFontSelectDropdown.findText(GuiGameMLTextCenteredSmProfile.fontType));

        It_SettingsLgFontSizeSpinner.setText(GuiGameMLTextCenteredLgProfile.fontSize @ "pt");
        It_SettingsSmFontSizeSpinner.setText(GuiGameMLTextCenteredSmProfile.fontSize @ "pt");

        %this.fontTestString = "The quick brown fox jumps over the lazy dog 0123456789" NL
        "THE QUICK BROWN FOX JUMPS OVER THE LAZY DOG" NL
        "0123456789" NL
        "!@#$%^&*()_+-=<>?,./";
    }

    GuiMLTextCenteredLgPreviewProfile.fontType = GuiGameMLTextCenteredLgProfile.fontType;
    GuiMLTextCenteredLgPreviewProfile.fontSize = GuiGameMLTextCenteredLgProfile.fontSize;
    GuiMLTextCenteredLgPreviewProfile.fontColor = GuiGameMLTextCenteredLgProfile.fontColor;

    It_SettingsLgFontPreview.setProfile(GuiMLTextCenteredLgPreviewProfile);
    It_SettingsLgFontPreview.setText(%this.fontTestString);

    GuiMLTextCenteredSmPreviewProfile.fontType = GuiGameMLTextCenteredSmProfile.fontType;
    GuiMLTextCenteredSmPreviewProfile.fontSize = GuiGameMLTextCenteredSmProfile.fontSize;
    GuiMLTextCenteredSmPreviewProfile.fontColor = GuiGameMLTextCenteredSmProfile.fontColor;

    It_SettingsSmFontPreview.setProfile(GuiMLTextCenteredSmPreviewProfile);
    It_SettingsSmFontPreview.setText(%this.fontTestString);

    It_SettingsMenuMusicBtnSelectEdit.sound = mainMenuGui.music;
    It_SettingsMenuMusicBtnSelectEdit.stateControl = "mainMenuGui";
    It_SettingsMenuMusicBtnSelectEdit.stateAsset[0] = mainMenuGui.music;
    It_SettingsMenuMusicBtnSelectEdit.stateField[0] = "music";
    It_SettingsMenuMusicBtnSelectEdit.type = "sound";
    %temp = AssetDatabase.acquireAsset(It_SettingsMenuMusicBtnSelectEdit.stateAsset[0]);
    It_SettingsMenuMusicBtnSelectEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(It_SettingsMenuMusicBtnSelectEdit.stateAsset[0]);
    PhysicsLauncherTools::audioButtonInitialize(It_SettingsMenuMusicBtnPlayBtn);

    It_SettingsButtonSoundBtnSelectEdit.sound = mainMenuGui.buttonSound;
    It_SettingsButtonSoundBtnSelectEdit.stateControl = "mainMenuGui";
    It_SettingsButtonSoundBtnSelectEdit.stateAsset[0] = mainMenuGui.buttonSound;
    It_SettingsButtonSoundBtnSelectEdit.stateField[0] = "buttonSound";
    It_SettingsButtonSoundBtnSelectEdit.type = "sound";
    %temp = AssetDatabase.acquireAsset(It_SettingsButtonSoundBtnSelectEdit.stateAsset[0]);
    It_SettingsButtonSoundBtnSelectEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(It_SettingsButtonSoundBtnSelectEdit.stateAsset[0]);
    PhysicsLauncherTools::audioButtonInitialize(It_SettingsButtonSoundBtnPlayBtn);

    It_SettingsWinMusicBtnSelectEdit.sound = winGui.music;
    It_SettingsWinMusicBtnSelectEdit.stateControl = "winGui";
    It_SettingsWinMusicBtnSelectEdit.stateAsset[0] = winGui.music;
    It_SettingsWinMusicBtnSelectEdit.stateField[0] = "music";
    It_SettingsWinMusicBtnSelectEdit.type = "sound";
    %temp = AssetDatabase.acquireAsset(It_SettingsWinMusicBtnSelectEdit.stateAsset[0]);
    It_SettingsWinMusicBtnSelectEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(It_SettingsWinMusicBtnSelectEdit.stateAsset[0]);
    PhysicsLauncherTools::audioButtonInitialize(It_SettingsWinMusicBtnPlayBtn);

    It_SettingsRewardSoundBtnSelectEdit.sound = winGui.rewardSound;
    It_SettingsRewardSoundBtnSelectEdit.stateControl = "winGui";
    It_SettingsRewardSoundBtnSelectEdit.stateAsset[0] = winGui.rewardSound;
    It_SettingsRewardSoundBtnSelectEdit.stateField[0] = "rewardSound";
    It_SettingsRewardSoundBtnSelectEdit.type = "sound";
    %temp = AssetDatabase.acquireAsset(It_SettingsRewardSoundBtnSelectEdit.stateAsset[0]);
    It_SettingsRewardSoundBtnSelectEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(It_SettingsRewardSoundBtnSelectEdit.stateAsset[0]);
    PhysicsLauncherTools::audioButtonInitialize(It_SettingsRewardSoundBtnPlayBtn);

    It_SettingsScoreSoundBtnSelectEdit.sound = winGui.scoreSound;
    It_SettingsScoreSoundBtnSelectEdit.stateControl = "winGui";
    It_SettingsScoreSoundBtnSelectEdit.stateAsset[0] = winGui.scoreSound;
    It_SettingsScoreSoundBtnSelectEdit.stateField[0] = "scoreSound";
    It_SettingsScoreSoundBtnSelectEdit.type = "sound";
    %temp = AssetDatabase.acquireAsset(It_SettingsScoreSoundBtnSelectEdit.stateAsset[0]);
    It_SettingsScoreSoundBtnSelectEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(It_SettingsScoreSoundBtnSelectEdit.stateAsset[0]);
    PhysicsLauncherTools::audioButtonInitialize(It_SettingsScoreSoundBtnPlayBtn);

    It_SettingsLoseMusicBtnSelectEdit.sound = loseGui.music;
    It_SettingsLoseMusicBtnSelectEdit.stateControl = "loseGui";
    It_SettingsLoseMusicBtnSelectEdit.stateAsset[0] = loseGui.music;
    It_SettingsLoseMusicBtnSelectEdit.stateField[0] = "music";
    It_SettingsLoseMusicBtnSelectEdit.type = "sound";
    %temp = AssetDatabase.acquireAsset(It_SettingsLoseMusicBtnSelectEdit.stateAsset[0]);
    It_SettingsLoseMusicBtnSelectEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(It_SettingsLoseMusicBtnSelectEdit.stateAsset[0]);
    PhysicsLauncherTools::audioButtonInitialize(It_SettingsLoseMusicBtnPlayBtn);

    It_SettingsLgFontPreview.forceReflow();
    It_SettingsSmFontPreview.forceReflow();
    It_LgFontScroll.computeSizes();
    It_SmFontScroll.computeSizes();
    It_LgFontScroll.setScrollPosition(0, 0);
    It_SmFontScroll.setScrollPosition(0, 0);

    $It_TabInitializing = false;
}

function InterfaceTool::updateRewardPreviews(%this, %visible)
{
    RewardImage_0.setVisible(%visible);
    RewardImage_0.setImage(It_WinLoseLargeRewardBtnSelectEdit.stateAsset[1]);
    RewardImage_1.setVisible(%visible);
    RewardImage_1.setImage(It_WinLoseLargeRewardBtnSelectEdit.stateAsset[1]);
    RewardImage_2.setVisible(%visible);
    RewardImage_2.setImage(It_WinLoseLargeRewardBtnSelectEdit.stateAsset[0]);
    RewardImage_3.setVisible(%visible);
    RewardImage_3.setImage(It_WinLoseLargeRewardBtnSelectEdit.stateAsset[0]);
    RewardImage_4.setVisible(%visible);
    RewardImage_4.setImage(It_WinLoseLargeRewardBtnSelectEdit.stateAsset[0]);
}

// This method builds each world panel in the pane
function InterfaceTool::populateWorldPane(%this)
{
    It_WorldListContentPane.clear();

    %worldSelectGuiSize = worldSelectGui.Extent;
    %previewSize = "186 138";

    for(%i = 1; %i < %this.worldData.getCount(); %i++)
    {
        %world = %this.worldData.getObject(%i);
        WorldSelectBackground.Image = %world.WorldSelectBackground;
        WorldSelectButton.NormalImage = %world.WorldImage;
        %control = GuiUtils::duplicateGuiObject(worldSelectGui, %i);
        GuiUtils::resizeGuiObject(%control, %worldSelectGuiSize, %previewSize);
        %worldPane = %this.createWorldPane(%i, %world, %control);
        It_WorldListContentPane.add(%worldPane);
    }

   %position = It_WorldListContentPane.Position;
   %width = getWord(It_WorldListContentPane.Extent, 0);
   %rowCount = mFloor(((It_WorldListContentPane.getCount() - 1) / It_WorldListContentPane.colCount)) + 1;
   It_WorldListContentPane.resize(%position.x, %position.y, %width, %rowCount * It_WorldListContentPane.rowSize);
   It_WorldListContentPane.refresh();
}

// This method build an individual world panel for the world list pane
function InterfaceTool::createWorldPane(%this, %index, %worldData, %controlPreview)
{
    // The panel into which all of the other controls go
    %panel = new GuiControl()
    {
        canSaveDynamicFields="1";
        isContainer="1";
        Profile="GuiWorldPaneContainer";
        HorizSizing="right";
        VertSizing="bottom";
        Position="0 0";
        Extent="206 263";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
            index = %index;
    };

    // The preview image for the world
    %previewImage = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="10 37";
        Extent="186 138";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
    };
    %panel.addGuiControl(%previewImage);
    %previewImage.addGuiControl(%controlPreview);

    // The current Container asset name.  The user cannot actually edit this
    // text as a GuiMouseEventCtrl is placed over it.
    %containerSelectEdit = new GuiTextEditCtrl()
    {
        canSaveDynamicFields="1";
        isContainer="0";
        Profile="GuiFileEditBoxProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="90 230";
        Extent="86 25";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        maxLength="1024";
        historySize="0";
        password="0";
        tabComplete="0";
        sinkAllKeyEvents="0";
        passwordMask="*";
        truncate="1";
    };
    %panel.addGuiControl(%containerSelectEdit);
    %containerSelectEdit.asset = %worldData.ContainerImage;
    %temp = AssetDatabase.acquireAsset(%containerSelectEdit.asset);
    %containerSelectEdit.setText(%temp.AssetName);
    for ( %i = 1; %i < 4; %i++ )
    {
        %container = "ScoreContainer" @ %i @ %index;
        %container.setImage(%containerSelectEdit.asset);
    }
    AssetDatabase.releaseAsset(%containerSelectEdit.asset);

    // The Container button to open the asset picker
    %containerSelectBtn = new GuiImageButtonCtrl()
    {
        Class="It_WorldPaneContainerSelectBtn";
        canSaveDynamicFields="1";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="176 230";
        Extent="22 25";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Select the image file from the Asset Picker for Icon Containers.";
        groupNum="-1";
        buttonType="PushButton";
        useMouseEvents="0";
        isLegacyVersion="0";
        NormalImage="{EditorAssets}:assetImageMap";
        HoverImage="{EditorAssets}:asset_hImageMap";
        DownImage="{EditorAssets}:asset_dImageMap";
        InactiveImage="{EditorAssets}:asset_iImageMap";
            edit = %containerSelectEdit;
            world = %worldData;
            index = %index;
    };
    %panel.addGuiControl(%containerSelectBtn);

    // Covers %containerSelectEdit to open the asset picker
    %containerMouseEvent = new GuiMouseEventCtrl()
    {
        class="It_AssetBrowseEditClick";
        canSaveDynamicFields="1";
        isContainer="0";
        Profile="GuiTransparentProfile";
        HorizSizing="left";
        VertSizing="top";
        Position="90 230";
        Extent="86 25";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Select the image file from the Asset Picker for Icon Containers.";
        groupNum="-1";
            button=%containerSelectBtn;
    };
    %panel.addGuiControl(%containerMouseEvent);

    // The label for the Container line
    %containerLabel = new GuiTextCtrl()
    {
        canSaveDynamicFields="1";
        isContainer="0";
        Profile="GuiTextProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="9 234";
        Extent="72 16";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        text="Containers";
        maxLength="1024";
    };
    %panel.addGuiControl(%containerLabel);

    // The label for the Icon line
    %iconLabel = new GuiTextCtrl()
    {
        canSaveDynamicFields="1";
        isContainer="0";
        Profile="GuiTextProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="9 208";
        Extent="32 16";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        text="Icon";
        maxLength="1024";
    };
    %panel.addGuiControl(%iconLabel);

    // The current Icon asset name.  The user cannot actually edit this
    // text as a GuiMouseEventCtrl is placed over it.
    %iconSelectEdit = new GuiTextEditCtrl()
    {
        canSaveDynamicFields="1";
        isContainer="0";
        Profile="GuiFileEditBoxProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="90 203";
        Extent="86 25";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        maxLength="1024";
        historySize="0";
        password="0";
        tabComplete="0";
        sinkAllKeyEvents="0";
        passwordMask="*";
        truncate="1";
    };
    %panel.addGuiControl(%iconSelectEdit);
    %iconSelectEdit.asset = %worldData.WorldImage;
    %temp = AssetDatabase.acquireAsset(%iconSelectEdit.asset);
    %iconSelectEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(%iconSelectEdit.asset);

    // The Icon button to open the asset picker
    %iconSelectBtn = new GuiImageButtonCtrl()
    {
        Class="It_WorldPaneIconSelectBtn";
        canSaveDynamicFields="1";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="176 203";
        Extent="22 25";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Select the image file from the Asset Picker for the World Icon button.";
        groupNum="-1";
        buttonType="PushButton";
        useMouseEvents="0";
        isLegacyVersion="0";
        NormalImage="{EditorAssets}:assetImageMap";
        HoverImage="{EditorAssets}:asset_hImageMap";
        DownImage="{EditorAssets}:asset_dImageMap";
        InactiveImage="{EditorAssets}:asset_iImageMap";
            edit=%iconSelectEdit;
            world = %worldData;
            preview = "WorldSelectButton" @ %index;
            lockedDropdown = %lockState;
    };
    %panel.addGuiControl(%iconSelectBtn);

    // Covers %iconSelectEdit to open the asset picker
    %iconMouseEvent = new GuiMouseEventCtrl()
    {
        class="It_AssetBrowseEditClick";
        canSaveDynamicFields="1";
        isContainer="0";
        Profile="GuiTransparentProfile";
        HorizSizing="left";
        VertSizing="top";
        Position="90 203";
        Extent="86 25";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Select the image file from the Asset Picker for the World Icon button.";
        groupNum="-1";
            button=%iconSelectBtn;
    };
    %panel.addGuiControl(%iconMouseEvent);

    // The label for the Background line
    %backgroundLabel = new GuiTextCtrl()
    {
        canSaveDynamicFields="1";
        isContainer="0";
        Profile="GuiTextProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="9 182";
        Extent="80 18";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        text="Background";
        maxLength="1024";
    };
    %panel.addGuiControl(%backgroundLabel);

    // The current Background asset name.  The user cannot actually edit this
    // text as a GuiMouseEventCtrl is placed over it.
    %backgroundSelectEdit = new GuiTextEditCtrl()
    {
        canSaveDynamicFields="1";
        isContainer="0";
        Profile="GuiFileEditBoxProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="90 177";
        Extent="86 25";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        maxLength="1024";
        historySize="0";
        password="0";
        tabComplete="0";
        sinkAllKeyEvents="0";
        passwordMask="*";
        truncate="1";
    };
    %panel.addGuiControl(%backgroundSelectEdit);
    %backgroundSelectEdit.asset = %worldData.WorldSelectBackground;
    %temp = AssetDatabase.acquireAsset(%backgroundSelectEdit.asset);
    %backgroundSelectEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(%backgroundSelectEdit.asset);

    // The Background button to open the asset picker
    %backgroundSelectBtn = new GuiImageButtonCtrl()
    {
        Class="It_WorldPaneBackgroundSelectBtn";
        canSaveDynamicFields="1";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="176 177";
        Extent="22 25";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Select the image file from the Asset Picker for the World Background button.";
        groupNum="-1";
        buttonType="PushButton";
        useMouseEvents="0";
        isLegacyVersion="0";
        NormalImage="{EditorAssets}:assetImageMap";
        HoverImage="{EditorAssets}:asset_hImageMap";
        DownImage="{EditorAssets}:asset_dImageMap";
        InactiveImage="{EditorAssets}:asset_iImageMap";
            edit=%backgroundSelectEdit;
            world = %worldData;
            preview = "WorldSelectBackground" @ %index;
            lockedDropdown = %lockState;
    };
    %panel.addGuiControl(%backgroundSelectBtn);

    // Covers %backgroundSelectEdit to open the asset picker
    %backgroundMouseEvent = new GuiMouseEventCtrl()
    {
        class="It_AssetBrowseEditClick";
        canSaveDynamicFields="1";
        isContainer="0";
        Profile="GuiTransparentProfile";
        HorizSizing="left";
        VertSizing="top";
        Position="90 177";
        Extent="86 25";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Select the image file from the Asset Picker for the World Background button.";
        groupNum="-1";
            button=%backgroundSelectBtn;
    };
    %panel.addGuiControl(%backgroundMouseEvent);

    // The Locked/Unlocked popup
    %lockState = new GuiPopUpMenuCtrl()
    {
        class="It_WorldPaneLockStateDropdown";
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiPopUpMenuProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="109 8";
        Extent="89 25";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Select to preview and edit the state Locked or Unlocked.";
        maxLength="1024";
        maxPopupHeight="200";
        sbUsesNAColor="0";
        reverseTextList="0";
        bitmapBounds="16 16";
            world=%worldData;
            preview = "WorldSelectButton" @ %index;
            edit = %iconSelectEdit;
    };
    %lockState.initialize();
    %lockState.setSelected(!%worldData.WorldLocked);
    %panel.addGuiControl(%lockState);

    // The preview button
    %previewBtn = new GuiImageButtonCtrl()
    {
        Class="It_WorldPanePreviewBtn";        
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="156 108";
        Extent="29 29";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Zoom to display the selected image at 100%.";
        groupNum="-1";
        buttonType="PushButton";
        useMouseEvents="0";
        NormalImage="{EditorAssets}:magnifyingGlassBackingImageMap";
        HoverImage="{EditorAssets}:magnifyingGlassBacking_hImageMap";
        DownImage="{EditorAssets}:magnifyingGlassBacking_dImageMap";
        InactiveImage="{EditorAssets}:magnifyingGlassBacking_iImageMap";
            preview=%previewImage;
    };
    %previewImage.addGuiControl(%previewBtn);

    // The name of the world
    %worldNameLabel = new GuiTextEditCtrl()
    {
        canSaveDynamicFields="1";
        isContainer="0";
        Profile="GuiObjectEditBoxProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="8 7";
        Extent="104 25";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        text=%worldData.getInternalName();
        maxLength="1024";
        truncate="1";
    };
    %panel.addGuiControl(%worldNameLabel);

    return %panel;
}

function InterfaceTool::populateLevelPane(%this, %index)
{
    It_LevelListContentPane.clear();
    %levelSelectSize = levelSelectGui.Extent;
    %previewSize = "186 138";
    %this.getWorldData();
    %world = %this.worldData.getObject(%index);

    %count = mFloor((%world.WorldLevelCount - 1) / 15);
    if (%world.WorldLevelCount > 0)
        %count++;

    for(%i = 0; %i < %count; %i++)
    {
        %pageName = "Page " @ %i + 1;
        %pageControl = GuiUtils::duplicateGuiObject(levelSelectGui, %i);
        %background = "LevelSelectBackground" @ %i;
        %background.Image = %world.WorldBackground[%i];
        for(%j = 1; %j < 16; %j++)
        {
            %levelButton = "LevelSelectButton" @ %j @ %i;
            if (%j < 3)
                %levelButton.NormalImage = %this.worldData.LevelImage;
            else
                %levelButton.NormalImage = %this.worldData.LevelLockedImage;
        }
        GuiUtils::resizeGuiObject(%pageControl, %levelSelectSize, %previewSize);
        %levelPane = %this.createLevelPane(%i, %pageName, %pageControl, %world.WorldBackground[%i], (%count > 1 ? true : false));
        It_LevelListContentPane.add(%levelPane);
    }

    %position = It_LevelListContentPane.Position;
    %width = It_LevelListScroller.Extent.x - 12;
    %rowCount = mFloor(((It_LevelListContentPane.getCount() - 1) / It_LevelListContentPane.colCount)) + 1;
    It_LevelListContentPane.resize(%position.x, %position.y, %width, %rowCount * (It_LevelListContentPane.rowSize + It_LevelListContentPane.rowSpacing));
    It_LevelListContentPane.refresh();
}

function InterfaceTool::createLevelPane(%this, %index, %page, %pageControl, %imageName, %eventFlag)
{
    %pane = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiLevelPanelContainer";
        HorizSizing="right";
        VertSizing="bottom";
        Position="0 0";
        Extent="204 208";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
            pageIndex = %index;
            page = %page;
            previewCtrl = %pageControl;
    };
    %pane.normalProfile = %pane.Profile;
    %pane.highlightProfile = %pane.Profile @ "Highlight";

    %levelNameLabel = new GuiTextCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTextProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="8 8";
        Extent="176 18";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        text=%page;
        maxLength="1024";
    };
    %pane.addGuiControl(%levelNameLabel);

    %previewImage = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiTransparentProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="8 30";
        Extent="186 138";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        wrap="0";
        useSourceRect="0";
        sourceRect="0 0 0 0";
    };
    %pane.addGuiControl(%previewImage);
    %previewImage.addGuiControl(%pageControl);

    %event = new GuiMouseEventCtrl()
    {
	    class="It_LevelScreenSelectButton";
		canSaveDynamicFields="0";
		isContainer="0";
		Profile="GuiTransparentProfile";
        HorizSizing="left";
        VertSizing="top";
        Position="0 0";
        Extent="204 208";
		MinExtent="8 2";
		canSave="1";
		Visible="1";
		hovertime="1000";
		groupNum="-1";
		    index = %index;
    };
    %pane.addGuiControl(%event);

    %previewSelectBtn = new GuiImageButtonCtrl()
    {
        Class="It_LevelPanePreviewBtn";
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="164 138";
        Extent="29 29";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Zoom to display the selected image at 100%.";
        groupNum="-1";
        buttonType="PushButton";
        useMouseEvents="0";
        isLegacyVersion="0";
        NormalImage="{EditorAssets}:magnifyingGlassBackingImageMap";
        HoverImage="{EditorAssets}:magnifyingGlassBacking_hImageMap";
        DownImage="{EditorAssets}:magnifyingGlassBacking_dImageMap";
        InactiveImage="{EditorAssets}:magnifyingGlassBacking_iImageMap";
            index = %index;
            preview = %previewImage;
    };
    %pane.addGuiControl(%previewSelectBtn);
    %pane.previewGuiBtn = %previewSelectBtn;

    %selectEdit = new GuiTextEditCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiFileEditBoxProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="8 172";
        Extent="157 25";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        maxLength="1024";
        historySize="0";
        password="0";
        tabComplete="0";
        sinkAllKeyEvents="0";
        passwordMask="*";
        truncate="1";
    };
    %pane.addGuiControl(%selectEdit);
    %pane.backgroundEdit = %selectEdit;
    %temp = AssetDatabase.acquireAsset(%imageName);
    %selectEdit.setText(%temp.AssetName);
    %pane.imageName = %temp.AssetName;
    AssetDatabase.releaseAsset(%imageName);

    %selectBtn = new GuiImageButtonCtrl()
    {
        Class="It_LevelSelectPaneIconSelectBtn";
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="164 172";
        Extent="22 25";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Select the image file from the Asset Picker for the background image for the selected page of levels.";
        groupNum="-1";
        buttonType="PushButton";
        useMouseEvents="0";
        isLegacyVersion="0";
        NormalImage="{EditorAssets}:assetImageMap";
        HoverImage="{EditorAssets}:asset_hImageMap";
        DownImage="{EditorAssets}:asset_dImageMap";
        InactiveImage="{EditorAssets}:asset_iImageMap";
            edit=%selectEdit;
            preview=LevelSelectBackground @ %index;
            index=%index;
    };
    %pane.addGuiControl(%selectBtn);
    %pane.selectBtn = %selectBtn;

    %iconMouseEvent = new GuiMouseEventCtrl()
    {
        class="It_AssetBrowseEditClick";
        canSaveDynamicFields="1";
        isContainer="0";
        Profile="GuiTransparentProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="8 172";
        Extent="157 25";
        MinExtent="8 2";
        canSave="0";
        Active="1";
        Visible="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Select the image file from the Asset Picker for the background image for the selected page of levels.";
        groupNum="-1";
            button=%selectBtn;
    };
    %pane.addGuiControl(%iconMouseEvent);
    %pane.mouseEvnt = %iconMouseEvent;
    %pane.pushToBack(%iconMouseEvent);

    if (%eventFlag)
    {
        %pane.pushToBack(%event);
        %event.toolTipProfile="GuiToolTipProfile";
        %event.toolTip="Select to edit this level page's properties.";
    }
    else
    {
        %event.toolTip="";
    }

    return %pane;
}

/// <summary>
/// This function handles visibility and layout of the win screen reward images
/// </summary>
function InterfaceTool::createWinRewardImages(%this)
{
    // Create and add the win screen reward images.
    %xScale = winGuiPreview.Extent.x / winGui.Extent.x;
    %yScale = winGuiPreview.Extent.y / winGui.Extent.y;
    // First, create our reward image container if it does not exist.
    %rewardWidth = %this.rewardImageSize.x;
    %rewardHeight = %this.rewardImageSize.y;

    %container = new GuiControl()
    {
        canSaveDynamicFields = "0";
        isContainer = "1";
        Profile = "GuiTransparentProfile";
        HorizSizing = "relative";
        VertSizing = "relative";
        MinExtent = "8 2";
        canSave = "1";
        Visible = "1";
        hovertime = "1000";
    };
    %container.setExtent(((%rewardWidth * %this.rewardCount) * %xScale), (%rewardHeight * %yScale));
    %containerX = (winGuiPreview.Extent.x / 2) - (%container.Extent.x / 2);
    %containerY = 180.0 * %yScale;
    %container.setPosition(%containerX, %containerY);
    %container.setName("RewardPreview" @ %container.getId());

    %event = new GuiMouseEventCtrl()
    {
        canSaveDynamicFields = "0";
        isContainer = "0";
        class = "It_GuiRewardPreview";
        Profile = "GuiTransparentProfile";
        HorizSizing = "relative";
        VertSizing = "relative";
        MinExtent = "8 2";
        canSave = "1";
        Visible = "1";
        hovertime = "1000";
        parentPane = %container;
    };
    %event.setExtent(%container.Extent.x, %container.Extent.y);

    // Next, create bitmap control instances as needed
    for (%i = 0; %i < %this.rewardCount; %i++)
    {
        %rewardImage = "RewardImage_" @ %i @ "_preview";
        new GuiSpriteCtrl(%rewardImage) {
            canSaveDynamicFields = "1";
            isContainer = "0";
            Profile = "GuiTransparentProfile";
            HorizSizing = "relative";
            VertSizing = "relative";
            Position = "0 8";
            MinExtent = "8 2";
            canSave = "1";
            Active = "1";
            Visible = "1";
            hovertime = "1000";
            Image = %i > 0 ? winGui.noRewardImage : winGui.rewardImage;
                index = %i;
        };
        %rewardImage.setExtent(%rewardWidth * %xScale, %rewardHeight * %yScale);
        %container.add(%rewardImage);
    }
    %container.add(%event);

    // Finally, set the images on the bitmap controls to correctly indicate the stars 
    // earned in the completed level.
    %containerWidth = %container.Extent.x;
    %startX = (%containerWidth / 2) - ((%this.rewardCount * (%rewardWidth * %xScale)) / 2);
    for (%i = 0; %i < %this.rewardCount; %i++)
    {
        %rewardImage = "RewardImage_" @ %i @ "_preview";
        %rewardImage.setPosition(((%startX + (%i * %rewardWidth)) * %xScale), 0);
        %rewardImage.setVisible(true);
    }
    return %container;
}

/// <summary>
/// This function handles visibility and layout of the level reward images
/// </summary>
/// <param name="index">The index of the level selection button to display images on.</param>
function InterfaceTool::displayRewardImages(%this, %currentButton)
{
    // Create and add the page indicator/selector buttons.
    // Take the number of pages, figure a total width, spread the buttons out
    // so that they're centered on the page.

    // First, create our reward image container
    %ExtX = getWord(%currentButton.Extent, 0) - 4;
    %posX = getWord(%currentButton.Position, 0);
    %posX += 5;
    %sizeMult = %ExtX / 128;
    %container = new GuiControl() {
        canSaveDynamicFields = "1";
        isContainer = "1";
        Profile = "GuiTransparentProfile";
        HorizSizing = "relative";
        VertSizing = "relative";
        Position = "2" SPC getWord(%currentButton.Extent, 1) - mFloor(42 * %sizeMult);
        Extent = %ExtX SPC mFloor(32 * %sizeMult);
        MinExtent = "8 2";
        canSave = "0";
        Visible = "1";
        hovertime = "1000";
    };
    %currentButton.addGuiControl(%container);

    // Next, create reward image instances as needed
    %rewardX = mFloor(%sizeMult * 24);
    %rewardY = mFloor(%sizeMult * 24);
    %rewardSize = %rewardX SPC %rewardY;
    for (%i = 0; %i < %this.rewardCount; %i++)
    {
        %rewardImage = "RewardImage_" @ %i;
        %rewardImageInstance = new GuiSpriteCtrl(%rewardImage) {
            canSaveDynamicFields = "1";
            isContainer = "0";
            Profile = "GuiTransparentProfile";
            HorizSizing = "relative";
            VertSizing = "relative";
            Position = "0" SPC mFloor((%container.Extent.y / 2) - (%rewardImage.Extent.y / 2));
            Extent = %rewardSize;
            MinExtent = "8 2";
            canSave = "0";
            Visible = "1";
            hovertime = "1000";
            Image = %this.noRewardImageSmall;
                index = %i;
        };
        %container.addGuiControl(%rewardImageInstance);
    }

    // Finally, set the images on the bitmap controls to correctly indicate the number of 
    // earned stars
    %containerWidth = getWord(%container.Extent, 0);
    %startX = (%containerWidth / 2) - (((%this.rewardCount * mFloor(12 * %sizeMult)) + mFloor((%this.rewardCount - 1) * (10 * %sizeMult)) / 2) - (13 * %sizeMult));
    %earnedStars = %this.currentWorld.LevelStars[%levelIndex];
    for (%i = 0; %i < %this.rewardCount; %i++)
    {
        %rewardImage = "RewardImage_" @ %i;
        %rewardImage.Position = (%startX + (%i * mFloor(24 * %sizeMult))) SPC mFloor(8 * %sizeMult);
        %rewardImage.bitmap = (%i < 2 ? expandPath(%this.rewardImageSmall) : expandPath(%this.noRewardImageSmall));
        %rewardImage.setVisible(true);
    }
}

function InterfaceTool::getWorldData(%this)
{
    if (isFile($PhysicsLauncher::WorldListFile))
        %this.worldData = TamlRead($PhysicsLauncher::WorldListFile);
    else
        %this.worldData = TamlRead("^PhysicsLauncherTemplate/managed/worldList.taml");
}

function InterfaceTool::saveData(%this)
{
    if ($InterfaceToolInitialized)
    {
        if (isObject(%this.worldData))
        {
            TamlWrite(%this.worldData, "^PhysicsLauncherTemplate/managed/worldList.taml");

            if (isFile($PhysicsLauncher::WorldListFile))
                TamlWrite(%this.worldData, $PhysicsLauncher::WorldListFile);
        }

        %this.saveFonts();
        %this.saveAllGuiObjects();
    }
}

function InterfaceTool::propertyPaneSelect(%this, %control)
{
    // this feels hackish, but I had to avoid highlighting the property pane
    // parent control - and it would be the whole tab page if I didn't have the
    // property panes in a gui control.
    %controlName = %control.getParent().getName();
    if (%controlName $= "It_MainMenuPaneContainer" ||
        %controlName $= "It_WorldSelectPaneContainer" ||
        %controlName $= "It_WinLosePaneContainer" ||
        %controlName $= "It_PausePaneContainer" ||
        %controlName $= "It_CreditsPaneContainer" ||
        %controlName $= "It_SettingsPaneContainer")
        return;

    // This was added because the reward image preview is actually the 
    // first sprite in the dynamic set - which is a child of the dynamic
    // reward display control.  We want to highlight the display control
    // instead of the reward preview image.
    %getPreviewParent = false;
    %controlName = %control.getName();
    if (%controlName $= "It_WinLoseLargeRewardBtnPreviewBtn" ||
        %controlName $= "It_WinLoseLargeRewardBtnSelectBtn" ||
        %controlName $= "It_WinLoseLargeRewardBtnStateDropdown")
            %getPreviewParent = true;

    if (isObject($InterfaceTool::highlightedPreview.highlight))
    {
        $InterfaceTool::highlightedPreview.remove($InterfaceTool::highlight);
        $InterfaceTool::highlightedPane.setProfile($InterfaceTool::oldPaneProfile);
    }
    if (!isObject($InterfaceTool::highlight))
    {
        $InterfaceTool::highlight = new GuiControl()
        {
            Profile = "GuiSelectedElementHighlight";
        };
    }

    %pane = %control.getParent();
    %count = %pane.getCount();
    %i = 0;
    while ( %i < %count )
    {
        %obj = %pane.getObject(%i);
        if (%obj.preview !$= "")
        {
            %preview = %obj.preview;
            break;
        }
        %i++;
    }

    $InterfaceTool::highlightedPreview = %preview;
    if ( isObject(%preview) )
    {
        if (%getPreviewParent)
        {
            %obj = %preview.getParent();
            %preview = %obj;
        }
        %preview.getParent().add($InterfaceTool::highlight);
        $InterfaceTool::highlight.setPosition(%preview.Position.x - 10, %preview.Position.y - 10);
        $InterfaceTool::highlight.Extent = %preview.Extent.x + 20 SPC %preview.Extent.y + 20;
    }

    if ( isObject( $InterfaceTool::highlightedPane ) )
        $InterfaceTool::highlightedPane.setProfile($InterfaceTool::oldPaneProfile);

    $InterfaceTool::highlightedPane = %pane;
    $InterfaceTool::oldPaneProfile = %pane.Profile;
    $InterfaceTool::highlightedPane.setProfile(GuiLargePanelContainerHighlight);
}

function InterfaceTool::clearPaneSelection(%this)
{
    if (isObject($InterfaceTool::highlightedPreview.highlight))
    {
        $InterfaceTool::highlightedPreview.remove($InterfaceTool::highlight);
        $InterfaceTool::highlightedPane.setProfile($InterfaceTool::oldPaneProfile);
    }
    if ( isObject( $InterfaceTool::highlightedPane ) )
        $InterfaceTool::highlightedPane.setProfile($InterfaceTool::oldPaneProfile);
}

function InterfaceTool::findPreviewObject(%this, %control)
{
    %page = %this.selectedTab;
    %targetName = %control.getName();
    %countPage = %page.getCount();
    %i = 0;
    while ( %i < %countPage )
    {
        %obja = %page.getObject(%i);
        if (!isObject(%obja))
        {
            %i++;
            continue;
        }
        if (%obja.preview $= %targetName)
            return %obja.getParent();
        %countObja = %obja.getCount();
        %j = 0;
        while ( %j < %countObja )
        {
            %objb = %obja.getObject(%j);
            if (!isObject(%objb))
            {
                %j++;
                continue;
            }
            if (%objb.preview $= %targetName)
                return %objb.getParent();
            %countObjb = %objb.getCount();
            %k = 0;
            while ( %k < %countObjb )
            {
                %objc = %objb.getObject(%k);
                if (!isObject(%objc))
                {
                    %k++;
                    continue;
                }
                if (%objc.preview $= %targetName)
                    return %objc.getParent();
                %countObjc = %objc.getCount();
                %l = 0;
                while ( %l < %countObjc )
                {
                    %objd = %objc.getObject(%l);
                    if (!isObject(%objd))
                    {
                        %l++;
                        continue;
                    }
                    if (%objd.preview $= %targetName)
                        return %objd.getParent();
                    %l++;
                }
                %k++;
            }
            %j++;
        }
        %i++;
    }
}

function It_RewardCountSelectDropdown::onSelect(%this)
{
    InterfaceTool.worldData.rewardCount = %this.getSelected();
    InterfaceTool.rewardCount = %this.getSelected();
    if (isObject(InterfaceTool.rewards))
    {
        InterfaceTool.rewards.delete();
        InterfaceTool.rewards = InterfaceTool.createWinRewardImages();
        InterfaceTool.tempWinGui.addGuiControl(InterfaceTool.rewards);
    }
}

function It_WinLoseBackgroundStateDropdown::onSelect(%this)
{
    InterfaceTool::clearPaneSelection();
    %selected = %this.getSelected();
    switch(%selected)
    {
        case 0:
            if (isObject(InterfaceTool.tempWinGui))
                InterfaceTool.tempWinGui.delete();
            InterfaceTool.tempWinGui = GuiUtils::duplicateGuiObject(winGui, "Preview", "It_GuiPreviewDuplicateButton");
            GuiUtils::resizeGuiObject(InterfaceTool.tempWinGui, InterfaceTool.tempWinGui.Extent, It_WinLosePreviewContainer.Extent);

            It_WinLoseBackgroundSelectEdit.preview = "winBackgroundPreview";
            It_WinLoseBackgroundSelectEdit.preview.setImage(It_WinLoseBackgroundSelectEdit.stateAsset[0]);
            It_WinLoseBackgroundSelectEdit.stateControl="winBackground";

            It_WinLoseReplayBtnSelectEdit.stateControl = "winRestartButton";
            It_WinLoseReplayBtnSelectEdit.stateAsset[0] = winRestartButton.NormalImage;
            It_WinLoseReplayBtnSelectEdit.stateField[0] = "NormalImage";
            It_WinLoseReplayBtnSelectEdit.stateAsset[1] = winRestartButton.HoverImage;
            It_WinLoseReplayBtnSelectEdit.stateField[1] = "HoverImage";
            It_WinLoseReplayBtnSelectEdit.stateAsset[2] = winRestartButton.DownImage;
            It_WinLoseReplayBtnSelectEdit.stateField[2] = "DownImage";
            It_WinLoseReplayBtnSelectEdit.stateAsset[3] = winRestartButton.InactiveImage;
            It_WinLoseReplayBtnSelectEdit.stateField[3] = "InactiveImage";
            It_WinLoseReplayBtnSelectEdit.assetSize = winRestartButton.extent;
            It_WinLoseReplayBtnSelectEdit.preview = winRestartButtonPreview;
            %temp = AssetDatabase.acquireAsset(It_WinLoseReplayBtnSelectEdit.stateAsset[0]);
            It_WinLoseReplayBtnSelectEdit.setText(%temp.AssetName);
            AssetDatabase.releaseAsset(It_WinLoseReplayBtnSelectEdit.stateAsset[0]);
            It_WinLoseReplayBtnSelectEdit.type = "gui";

            It_WinLoseLevelBtnSelectEdit.stateControl = "winLevelSelectButton";
            It_WinLoseLevelBtnSelectEdit.stateAsset[0] = winLevelSelectButton.NormalImage;
            It_WinLoseLevelBtnSelectEdit.stateField[0] = "NormalImage";
            It_WinLoseLevelBtnSelectEdit.stateAsset[1] = winLevelSelectButton.HoverImage;
            It_WinLoseLevelBtnSelectEdit.stateField[1] = "HoverImage";
            It_WinLoseLevelBtnSelectEdit.stateAsset[2] = winLevelSelectButton.DownImage;
            It_WinLoseLevelBtnSelectEdit.stateField[2] = "DownImage";
            It_WinLoseLevelBtnSelectEdit.stateAsset[3] = winLevelSelectButton.InactiveImage;
            It_WinLoseLevelBtnSelectEdit.stateField[3] = "InactiveImage";
            It_WinLoseLevelBtnSelectEdit.assetSize = winLevelSelectButton.extent;
            It_WinLoseLevelBtnSelectEdit.preview = winLevelSelectButtonPreview;
            %temp = AssetDatabase.acquireAsset(It_WinLoseLevelBtnSelectEdit.stateAsset[0]);
            It_WinLoseLevelBtnSelectEdit.setText(%temp.AssetName);
            AssetDatabase.releaseAsset(It_WinLoseLevelBtnSelectEdit.stateAsset[0]);
            It_WinLoseLevelBtnSelectEdit.type = "gui";

            It_WinLoseLargeRewardBtnSelectEdit.stateControl = "winGui";
            It_WinLoseLargeRewardBtnSelectEdit.stateAsset[0] = winGui.noRewardImage;
            It_WinLoseLargeRewardBtnSelectEdit.stateField[0] = "noRewardImage";
            It_WinLoseLargeRewardBtnSelectEdit.stateAsset[1] = winGui.rewardImage;
            It_WinLoseLargeRewardBtnSelectEdit.stateField[1] = "rewardImage";
            It_WinLoseLargeRewardBtnSelectEdit.assetSize = "128 128";
            %temp = AssetDatabase.acquireAsset(It_WinLoseLargeRewardBtnSelectEdit.stateAsset[0]);
            It_WinLoseLargeRewardBtnSelectEdit.setText(%temp.AssetName);
            AssetDatabase.releaseAsset(%assetID);
            It_WinLoseLargeRewardBtnSelectEdit.type = "gui";

            It_WinLoseNextLevelBtnSelectEdit.stateControl="winNextLevelButton";
            It_WinLoseNextLevelBtnSelectEdit.stateAsset[0] = winNextLevelButton.NormalImage;
            It_WinLoseNextLevelBtnSelectEdit.stateField[0] = "NormalImage";
            It_WinLoseNextLevelBtnSelectEdit.stateAsset[1] = winNextLevelButton.HoverImage;
            It_WinLoseNextLevelBtnSelectEdit.stateField[1] = "HoverImage";
            It_WinLoseNextLevelBtnSelectEdit.stateAsset[2] = winNextLevelButton.DownImage;
            It_WinLoseNextLevelBtnSelectEdit.stateField[2] = "DownImage";
            It_WinLoseNextLevelBtnSelectEdit.stateAsset[3] = winNextLevelButton.InactiveImage;
            It_WinLoseNextLevelBtnSelectEdit.stateField[3] = "InactiveImage";
            It_WinLoseNextLevelBtnSelectEdit.assetSize = winNextLevelButton.extent;
            It_WinLoseNextLevelBtnSelectEdit.preview = winNextLevelButtonPreview;
            %temp = AssetDatabase.acquireAsset(It_WinLoseNextLevelBtnSelectEdit.stateAsset[0]);
            It_WinLoseNextLevelBtnSelectEdit.setText(%temp.AssetName);
            AssetDatabase.releaseAsset(It_WinLoseNextLevelBtnSelectEdit.stateAsset[0]);
            It_WinLoseNextLevelBtnSelectEdit.type = "gui";

            if (It_WinLosePreviewContainer.getCount())
                It_WinLosePreviewContainer.remove(It_WinLosePreviewContainer.getObject(0));

            It_WinLosePreviewContainer.addGuiControl(InterfaceTool.tempWinGui);
            InterfaceTool.rewards = InterfaceTool.createWinRewardImages();
            InterfaceTool.tempWinGui.addGuiControl(InterfaceTool.rewards);
            InterfaceTool.rewards.bringToFront(InterfaceTool.rewards);
            InterfaceTool.rewards.setVisible(winGui.showRewards);

        case 1:
            if (isObject(InterfaceTool.tempLoseGui))
                InterfaceTool.tempLoseGui.delete();
            InterfaceTool.tempLoseGui = GuiUtils::duplicateGuiObject(loseGui, "Preview", "It_GuiPreviewDuplicateButton");
            GuiUtils::resizeGuiObject(InterfaceTool.tempLoseGui, InterfaceTool.tempLoseGui.Extent, It_WinLosePreviewContainer.Extent);

            It_WinLoseBackgroundSelectEdit.preview = "loseBackgroundPreview";
            It_WinLoseBackgroundSelectEdit.preview.setImage(It_WinLoseBackgroundSelectEdit.stateAsset[1]);
            It_WinLoseBackgroundSelectEdit.stateControl="loseBackground";

            It_WinLoseReplayBtnSelectEdit.stateControl="loseRestartButton";
            It_WinLoseReplayBtnSelectEdit.stateAsset[0] = loseRestartButton.NormalImage;
            It_WinLoseReplayBtnSelectEdit.stateField[0] = "NormalImage";
            It_WinLoseReplayBtnSelectEdit.stateAsset[1] = loseRestartButton.HoverImage;
            It_WinLoseReplayBtnSelectEdit.stateField[1] = "HoverImage";
            It_WinLoseReplayBtnSelectEdit.stateAsset[2] = loseRestartButton.DownImage;
            It_WinLoseReplayBtnSelectEdit.stateField[2] = "DownImage";
            It_WinLoseReplayBtnSelectEdit.stateAsset[3] = loseRestartButton.InactiveImage;
            It_WinLoseReplayBtnSelectEdit.stateField[3] = "InactiveImage";
            It_WinLoseReplayBtnSelectEdit.assetSize = loseRestartButton.extent;
            It_WinLoseReplayBtnSelectEdit.preview = loseRestartButtonPreview;
            %temp = AssetDatabase.acquireAsset(It_WinLoseReplayBtnSelectEdit.stateAsset[0]);
            It_WinLoseReplayBtnSelectEdit.setText(%temp.AssetName);
            AssetDatabase.releaseAsset(It_WinLoseReplayBtnSelectEdit.stateAsset[0]);
            It_WinLoseReplayBtnSelectEdit.type = "gui";

            It_WinLoseLevelBtnSelectEdit.stateControl="loseLevelSelectButton";
            It_WinLoseLevelBtnSelectEdit.stateAsset[0] = loseLevelSelectButton.NormalImage;
            It_WinLoseLevelBtnSelectEdit.stateField[0] = "NormalImage";
            It_WinLoseLevelBtnSelectEdit.stateAsset[1] = loseLevelSelectButton.HoverImage;
            It_WinLoseLevelBtnSelectEdit.stateField[1] = "HoverImage";
            It_WinLoseLevelBtnSelectEdit.stateAsset[2] = loseLevelSelectButton.DownImage;
            It_WinLoseLevelBtnSelectEdit.stateField[2] = "DownImage";
            It_WinLoseLevelBtnSelectEdit.stateAsset[3] = loseLevelSelectButton.InactiveImage;
            It_WinLoseLevelBtnSelectEdit.stateField[3] = "InactiveImage";
            It_WinLoseLevelBtnSelectEdit.assetSize = loseLevelSelectButton.extent;
            It_WinLoseLevelBtnSelectEdit.preview = loseLevelSelectButtonPreview;
            %temp = AssetDatabase.acquireAsset(It_WinLoseLevelBtnSelectEdit.stateAsset[0]);
            It_WinLoseLevelBtnSelectEdit.setText(%temp.AssetName);
            AssetDatabase.releaseAsset(It_WinLoseLevelBtnSelectEdit.stateAsset[0]);
            It_WinLoseLevelBtnSelectEdit.type = "gui";

            if (It_WinLosePreviewContainer.getCount())
                It_WinLosePreviewContainer.remove(It_WinLosePreviewContainer.getObject(0));

            It_WinLosePreviewContainer.addGuiControl(InterfaceTool.tempLoseGui);
    }
    %temp = AssetDatabase.acquireAsset(It_WinLoseBackgroundSelectEdit.stateAsset[%selected]);
    It_WinLoseBackgroundSelectEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(It_WinLoseBackgroundSelectEdit.stateAsset[%selected]);

    %active = !%this.getSelected();
    It_WinLoseNextLevelBtnContainer.setActive(%active);
    %count = It_WinLoseNextLevelBtnContainer.getCount();
    for (%i = 0; %i < %count; %i++)
    {
        %obj = It_WinLoseNextLevelBtnContainer.getObject(%i);
        %obj.setActive(%active);
        if (%active && %obj.Profile $= "GuiFileEditInactiveProfile")
            %obj.Profile = "GuiFileEditBoxProfile";
        else if (!%active && %obj.Profile $= "GuiFileEditBoxProfile")
            %obj.Profile = "GuiFileEditInactiveProfile";
        if (%active && %obj.getClassName() $= "GuiSeparatorCtrl")
            %obj.Profile = "GuiTransparentProfile";
        else if (!%active && %obj.getClassName() $= "GuiSeparatorCtrl")
            %obj.Profile = "GuiSeparatorInactiveProfile";
        if (%active && %obj.getClassName() $= "GuiTextCtrl")
            %obj.Profile = "GuiTextProfile";
        else if (!%active && %obj.getClassName() $= "GuiTextCtrl")
            %obj.Profile = "GuiInactiveTextProfile";
    }
    It_WinLoseLargeRewardBtnContainer.setActive(%active);
    %count = It_WinLoseLargeRewardBtnContainer.getCount();
    for (%i = 0; %i < %count; %i++)
    {
        %obj = It_WinLoseLargeRewardBtnContainer.getObject(%i);
        %obj.setActive(%active);
        if (%active && %obj.Profile $= "GuiFileEditInactiveProfile")
            %obj.Profile = "GuiFileEditBoxProfile";
        else if (!%active && %obj.Profile $= "GuiFileEditBoxProfile")
            %obj.Profile = "GuiFileEditInactiveProfile";
        if (%active && %obj.getClassName() $= "GuiSeparatorCtrl")
            %obj.Profile = "GuiTransparentProfile";
        else if (!%active && %obj.getClassName() $= "GuiSeparatorCtrl")
            %obj.Profile = "GuiSeparatorInactiveProfile";
        if (%active && %obj.getClassName() $= "GuiTextCtrl")
            %obj.Profile = "GuiTextProfile";
        else if (!%active && %obj.getClassName() $= "GuiTextCtrl")
            %obj.Profile = "GuiInactiveTextProfile";
    }
    if (%active)
    {
        It_WinLoseNextLevelBtnContainer.Profile = "GuiLargePanelContainer";
        It_WinLoseLargeRewardBtnContainer.Profile = "GuiLargePanelContainer";
    }
    else
    {
        It_WinLoseNextLevelBtnContainer.Profile = "GuiLargePanelContainerInactive";
        It_WinLoseLargeRewardBtnContainer.Profile = "GuiLargePanelContainerInactive";
    }
}

function  It_LevelListWorldSelectDropdown::onSelect(%this)
{
    InterfaceTool.populateLevelPane(%this.getSelected());
    %world = InterfaceTool.worldData.getObject(%this.getSelected());

    It_LevelIconBtnSelectEdit.stateAsset[0] = %world.LevelImageList[0];
    It_LevelIconBtnSelectEdit.stateAsset[1] = %world.LevelLockedImage[0];

    %temp = AssetDatabase.acquireAsset(It_LevelIconBtnSelectEdit.stateAsset[0]);
    %width = %temp.getCellWidth();
    %height = %temp.getCellHeight();
    if (%width < 1)
    {
        %width = %temp.getImageWidth();
        %height = %temp.getImageHeight();
    }
    It_LevelIconBtnSelectEdit.assetSize = %width SPC %height;
    It_LevelIconBtnStateDropdown.setFirstSelected();
    AssetDatabase.releaseAsset(It_LevelIconBtnSelectEdit.stateAsset[0]);
    It_LevelListContentPane.clearSelectedPane();
}

function It_SettingsLgFontSelectDropdown::onSelect(%this)
{
    if ($It_TabInitializing || !$InterfaceToolInitialized)
        return;

    %fontType = AvailableFonts.getObject(%this.getSelected()).Font;

    GuiMLTextCenteredLgPreviewProfile.fontType = %fontType;
    GuiMLTextCenteredLgPreviewProfile.justify = "center";
    GuiTextCenteredLgPreviewProfile.fontType = %fontType;
    GuiTextCenteredLgPreviewProfile.justify = "center";
    GuiGameMLTextCenteredLgProfile.fontType = %fontType;
    GuiGameMLTextCenteredLgProfile.justify = "center";
    GuiGameTextCenteredLgProfile.fontType = %fontType;
    GuiGameTextCenteredLgProfile.justify = "center";
    GuiGameTextLeftLgProfile.fontType = %fontType;
    GuiGameTextLeftLgProfile.justify = "left";
    GuiGameTextRightLgProfile.fontType = %fontType;
    GuiGameTextRightLgProfile.justify = "right";
    It_SettingsLgFontPreview.setProfile(GuiMLTextCenteredLgPreviewProfile);
    It_SettingsLgFontPreview.setText(InterfaceTool.fontTestString);

    It_SettingsLgFontPreview.forceReflow();
    It_LgFontScroll.computeSizes();
    It_LgFontScroll.setScrollPosition(0, 0);
    InterfaceTool.saveFonts();
}

function It_SettingsLgFontSizeSpinner::onValidate(%this)
{
    if ($It_TabInitializing || !$InterfaceToolInitialized)
        return;

    %text = %this.getText();
    %size = strreplace(%text, "pt", "");
    if ( %size < 6 )
    {
        %size = 6;
        %this.setText( %size @ "pt" );
        return;
    }
    if ( %size > 60 )
    {
        %size = 60;
        %this.setText( %size @ "pt" );
        return;
    }

    GuiMLTextCenteredLgPreviewProfile.fontSize = %size;
    GuiMLTextCenteredLgPreviewProfile.justify = "center";
    GuiTextCenteredLgPreviewProfile.fontSize = %size;
    GuiTextCenteredLgPreviewProfile.justify = "center";
    GuiGameMLTextCenteredLgProfile.fontSize = %size;
    GuiGameMLTextCenteredLgProfile.justify = "center";
    GuiGameTextCenteredLgProfile.fontSize = %size;
    GuiGameTextCenteredLgProfile.justify = "center";
    GuiGameTextLeftLgProfile.fontSize = %size;
    GuiGameTextLeftLgProfile.justify = "left";
    GuiGameTextRightLgProfile.fontSize = %size;
    GuiGameTextRightLgProfile.justify = "right";
    It_SettingsLgFontPreview.setProfile(GuiMLTextCenteredLgPreviewProfile);
    It_SettingsLgFontPreview.setText(InterfaceTool.fontTestString);

    It_SettingsLgFontPreview.forceReflow();
    It_LgFontScroll.computeSizes();
    It_LgFontScroll.setScrollPosition(0, 0);
    InterfaceTool.saveFonts();
}

function It_SettingsLgFontSizeDownBtn::onClick(%this)
{
    if ($It_TabInitializing || !$InterfaceToolInitialized)
        return;

    %text = It_SettingsLgFontSizeSpinner.getText();
    %value = strreplace(%text, "pt", "");
    %value -= 1;
    It_SettingsLgFontSizeSpinner.setText(%value @ "pt");
    It_SettingsLgFontSizeSpinner.onValidate();
}

function It_SettingsLgFontSizeUpBtn::onClick(%this)
{
    if ($It_TabInitializing || !$InterfaceToolInitialized)
        return;

    %text = It_SettingsLgFontSizeSpinner.getText();
    %value = strreplace(%text, "pt", "");
    %value += 1;
    It_SettingsLgFontSizeSpinner.setText(%value @ "pt");
    It_SettingsLgFontSizeSpinner.onValidate();
}

function It_SettingsSmFontSelectDropdown::onSelect(%this)
{
    if ($It_TabInitializing || !$InterfaceToolInitialized)
        return;

    %fontType = AvailableFonts.getObject(%this.getSelected()).Font;

    GuiMLTextCenteredSmPreviewProfile.fontType = %fontType;
    GuiMLTextCenteredSmPreviewProfile.justify = "center";
    GuiTextCenteredSmPreviewProfile.fontType = %fontType;
    GuiTextCenteredSmPreviewProfile.justify = "center";
    GuiGameMLTextCenteredSmProfile.fontType = %fontType;
    GuiGameMLTextCenteredSmProfile.justify = "center";
    GuiGameTextCenteredSmProfile.fontType = %fontType;
    GuiGameTextCenteredSmProfile.justify = "center";
    GuiGameTextLeftSmProfile.fontType = %fontType;
    GuiGameTextLeftSmProfile.justify = "left";
    GuiGameTextRightSmProfile.fontType = %fontType;
    GuiGameTextRightSmProfile.justify = "right";
    It_SettingsSmFontPreview.setProfile(GuiMLTextCenteredSmPreviewProfile);
    It_SettingsSmFontPreview.setText(InterfaceTool.fontTestString);

    It_SettingsSmFontPreview.forceReflow();
    It_SmFontScroll.computeSizes();
    It_SmFontScroll.setScrollPosition(0, 0);
    InterfaceTool.saveFonts();
}

function It_SettingsSmFontSizeSpinner::onValidate(%this)
{
    if ($It_TabInitializing || !$InterfaceToolInitialized)
        return;

    %text = %this.getText();
    %size = strreplace(%text, "pt", "");
    if ( %size < 6 )
    {
        %size = 6;
        %this.setText( %size @ "pt" );
        return;
    }
    if ( %size > 60 )
    {
        %size = 60;
        %this.setText( %size @ "pt" );
        return;
    }

    GuiMLTextCenteredSmPreviewProfile.fontSize = %size;
    GuiMLTextCenteredSmPreviewProfile.justify = "center";
    GuiTextCenteredSmPreviewProfile.fontSize = %size;
    GuiTextCenteredSmPreviewProfile.justify = "center";
    GuiGameMLTextCenteredSmProfile.fontSize = %size;
    GuiGameMLTextCenteredSmProfile.justify = "center";
    GuiGameTextCenteredSmProfile.fontSize = %size;
    GuiGameTextCenteredSmProfile.justify = "center";
    GuiGameTextLeftSmProfile.fontSize = %size;
    GuiGameTextLeftSmProfile.justify = "left";
    GuiGameTextRightSmProfile.fontSize = %size;
    GuiGameTextRightSmProfile.justify = "right";
    It_SettingsSmFontPreview.setProfile(GuiMLTextCenteredSmPreviewProfile);
    It_SettingsSmFontPreview.setText(InterfaceTool.fontTestString);

    It_SettingsSmFontPreview.forceReflow();
    It_SmFontScroll.computeSizes();
    It_SmFontScroll.setScrollPosition(0, 0);
    InterfaceTool.saveFonts();
}

function It_SettingsSmFontSizeDownBtn::onClick(%this)
{
    if ($It_TabInitializing || !$InterfaceToolInitialized)
        return;

    %text = It_SettingsSmFontSizeSpinner.getText();
    %value = strreplace(%text, "pt", "");
    %value -= 1;
    It_SettingsSmFontSizeSpinner.setText(%value @ "pt");
    It_SettingsSmFontSizeSpinner.onValidate();
}

function It_SettingsSmFontSizeUpBtn::onClick(%this)
{
    if ($It_TabInitializing || !$InterfaceToolInitialized)
        return;

    %text = It_SettingsSmFontSizeSpinner.getText();
    %value = strreplace(%text, "pt", "");
    %value += 1;
    It_SettingsSmFontSizeSpinner.setText(%value @ "pt");
    It_SettingsSmFontSizeSpinner.onValidate();
}

function It_SettingsLgFontColorSelectBtn::onClick(%this)
{
    GetColorI(GuiMLTextCenteredLgPreviewProfile.fontColor, setLgFontColor);
}

function setLgFontColor(%color)
{
    GuiMLTextCenteredLgPreviewProfile.fontColor = %color;
    GuiMLTextCenteredLgPreviewProfile.justify = "center";
    GuiTextCenteredLgPreviewProfile.fontColor = %color;
    GuiTextCenteredLgPreviewProfile.justify = "center";
    GuiGameMLTextCenteredLgProfile.fontColor = %color;
    GuiGameMLTextCenteredLgProfile.justify = "center";
    GuiGameTextCenteredLgProfile.fontColor = %color;
    GuiGameTextCenteredLgProfile.justify = "center";
    GuiGameTextLeftLgProfile.fontColor = %color;
    GuiGameTextLeftLgProfile.justify = "left";
    GuiGameTextRightLgProfile.fontColor = %color;
    GuiGameTextRightLgProfile.justify = "right";
    It_SettingsLgFontPreview.setProfile(GuiMLTextCenteredLgPreviewProfile);
    It_SettingsLgFontPreview.setText(InterfaceTool.fontTestString);

    It_SettingsLgFontPreview.forceReflow();
    It_LgFontScroll.computeSizes();
    It_LgFontScroll.setScrollPosition(0, 0);
    InterfaceTool.saveFonts();
}

function It_SettingsSmFontColorSelectBtn::onClick(%this)
{
    GetColorI(GuiMLTextCenteredSmPreviewProfile.fontColor, setSmFontColor);
}

function setSmFontColor(%color)
{
    GuiMLTextCenteredSmPreviewProfile.fontColor = %color;
    GuiMLTextCenteredSmPreviewProfile.justify = "center";
    GuiTextCenteredSmPreviewProfile.fontColor = %color;
    GuiTextCenteredSmPreviewProfile.justify = "center";
    GuiGameMLTextCenteredSmProfile.fontColor = %color;
    GuiGameMLTextCenteredSmProfile.justify = "center";
    GuiGameTextCenteredSmProfile.fontColor = %color;
    GuiGameTextCenteredSmProfile.justify = "center";
    GuiGameTextLeftSmProfile.fontColor = %color;
    GuiGameTextLeftSmProfile.justify = "left";
    GuiGameTextRightSmProfile.fontColor = %color;
    GuiGameTextRightSmProfile.justify = "right";
    It_SettingsSmFontPreview.setProfile(GuiMLTextCenteredSmPreviewProfile);
    It_SettingsSmFontPreview.setText(InterfaceTool.fontTestString);

    It_SettingsSmFontPreview.forceReflow();
    It_SmFontScroll.computeSizes();
    It_SmFontScroll.setScrollPosition(0, 0);
    InterfaceTool.saveFonts();
}

function It_LevelSelectToggleCheckbox::onClick(%this)
{
    %visible = %this.getValue();
    levelSelectGui.showScore = %visible;
    %paneCount = It_LevelListContentPane.getCount();
    for (%i = 0; %i < %paneCount; %i++)
    {
        for (%j = 1; %j < 16; %j++)
        {
            %score = "LevelSelectScore" @ %j @ %i;
            %score.visible = %visible;
        }
    }
}

function It_WorldToggleCheckbox::onClick(%this)
{
    %this.update();
}

function It_WorldToggleCheckbox::update(%this)
{
    %visible = %this.getValue();
    worldSelectGui.showContainers = %visible;
    %paneCount = It_WorldListContentPane.getCount() + 1;
    for (%i = 1; %i < %paneCount; %i++)
    {
        %scoreGroup = "ScoreGroupContainer" @ %i;
        %container1 = "ScoreContainer1" @ %i;
        %container2 = "ScoreContainer2" @ %i;
        %container3 = "ScoreContainer3" @ %i;
        %levelLabel = "LevelLabel" @ %i;
        %levelCount = "WorldSelectLevelCount" @ %i;
        %worldScoreLabel = "WorldScoreLabel" @ %i;
        %worldScore = "WorldSelectScore" @ %i;
        %starDisplay = "StarDisplay" @ %i;
        %starCount = "WorldSelectStarCount" @ %i;

        %scoreGroup.setVisible(%visible);
        %container1.setVisible(%visible);
        %container2.setVisible(%visible);
        %container3.setVisible(%visible);
        %levelLabel.setVisible(%visible);
        %levelCount.setVisible(%visible);
        %worldScoreLabel.setVisible(%visible);
        %worldScore.setVisible(%visible);
        %starDisplay.setVisible(%visible);
        %starCount.setVisible(%visible);
    }
}

function It_HudToggleCheckbox::onClick(%this)
{
    HudGui.showScore = %this.getValue();
    HudScoreContainerPreview.setVisible(%this.getValue());
}

function InterfaceToolTabBook::onTabSelected(%this, %data)
{
    if ($InterfaceToolInitialized)
    {
        $InterfaceTool::TabSwitch = true;

        InterfaceTool.saveData();
        InterfaceTool::clearPaneSelection();

        if ( isObject(InterfaceTool.helpManager) )
        {
            InterfaceTool.helpManager.stop();
            InterfaceTool.helpManager.delete();
        }

        switch$( %data )
        {
            case "Main Menu  ":
                InterfaceTool.selectedPage = 0;
                InterfaceTool.initMenuTab();
                InterfaceTool.selectedTab = "It_MainMenuPaneContainer";
                InterfaceTool.helpManager = createHelpMarqueeObject("InterfaceToolTips", 10000, "{PhysicsLauncherTools}");
                InterfaceTool.helpManager.openHelpSet("interfaceToolMainMenuHelp");
                InterfaceTool.helpManager.start();

            case "World Select  ":
                InterfaceTool.selectedPage = 1;
                InterfaceTool.initWorldSelectTab();
                InterfaceTool.selectedTab = "It_WorldSelectPaneContainer";
                InterfaceTool.helpManager = createHelpMarqueeObject("InterfaceToolTips", 10000, "{PhysicsLauncherTools}");
                InterfaceTool.helpManager.openHelpSet("interfaceToolWorldSelectHelp");
                InterfaceTool.helpManager.start();

            case "Level Select  ":
                InterfaceTool.selectedPage = 2;
                InterfaceTool.initLevelSelectTab();
                InterfaceTool.selectedTab = "It_LevelSelectPaneContainer";
                InterfaceTool.helpManager = createHelpMarqueeObject("InterfaceToolTips", 10000, "{PhysicsLauncherTools}");
                InterfaceTool.helpManager.openHelpSet("interfaceToolLevelSelectHelp");
                InterfaceTool.helpManager.start();

            case "Game HUD  ":
                InterfaceTool.selectedPage = 3;
                InterfaceTool.initHUDTab();
                InterfaceTool.selectedTab = "It_HudPaneContainer";
                InterfaceTool.helpManager = createHelpMarqueeObject("InterfaceToolTips", 10000, "{PhysicsLauncherTools}");
                InterfaceTool.helpManager.openHelpSet("interfaceToolGameHUDHelp");
                InterfaceTool.helpManager.start();

            case "Win / Lose  ":
                InterfaceTool.selectedPage = 4;
                InterfaceTool.initWinTab();
                InterfaceTool.selectedTab = "It_WinLosePaneContainer";
                InterfaceTool.helpManager = createHelpMarqueeObject("InterfaceToolTips", 10000, "{PhysicsLauncherTools}");
                InterfaceTool.helpManager.openHelpSet("interfaceToolWinScreenHelp");
                InterfaceTool.helpManager.start();

            case "Pause  ":
                InterfaceTool.selectedPage = 5;
                InterfaceTool.initPauseTab();
                InterfaceTool.selectedTab = "It_PausePaneContainer";
                InterfaceTool.helpManager = createHelpMarqueeObject("InterfaceToolTips", 10000, "{PhysicsLauncherTools}");
                InterfaceTool.helpManager.openHelpSet("interfaceToolPauseHelp");
                InterfaceTool.helpManager.start();

            case "Credits  ":
                InterfaceTool.selectedPage = 6;
                InterfaceTool.initCreditsTab();
                InterfaceTool.selectedTab = "It_CreditsPaneContainer";
                InterfaceTool.helpManager = createHelpMarqueeObject("InterfaceToolTips", 10000, "{PhysicsLauncherTools}");
                InterfaceTool.helpManager.openHelpSet("interfaceToolCreditsHelp");
                InterfaceTool.helpManager.start();

            case "Fonts / Sounds  ":
                InterfaceTool.selectedPage = 7;
                InterfaceTool.initSettingsTab();
                InterfaceTool.selectedTab = "It_SettingsPaneContainer";
                InterfaceTool.helpManager = createHelpMarqueeObject("InterfaceToolTips", 10000, "{PhysicsLauncherTools}");
                InterfaceTool.helpManager.openHelpSet("interfaceToolSettingsHelp");
                InterfaceTool.helpManager.start();
        }
    }
    $InterfaceTool::TabSwitch = false;
}

function It_WorldPanePreviewBtn::onClick(%this)
{
    GuiPreviewGui.display(%this.preview.getObject(0));
}

function It_WorldPaneBackgroundSelectBtn::onClick(%this)
{
    AssetPicker.open("ImageAsset", "", "", %this);
}

function It_WorldPaneBackgroundSelectBtn::setSelectedAsset(%this, %assetID)
{
    %asset = AssetDatabase.acquireAsset(%assetID);

    %temp = AssetDatabase.acquireAsset(%assetID);
    %this.edit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(%assetID);
    %this.edit.asset = %assetID;

    %this.preview.Image = %assetID;
    %this.world.WorldSelectBackground = %assetID;

    InterfaceTool.saveData();
}

function It_WorldPaneIconSelectBtn::onClick(%this)
{
    AssetPicker.open("ImageAsset", "", "", %this);
}

function It_WorldPaneIconSelectBtn::setSelectedAsset(%this, %assetID)
{
    %asset = AssetDatabase.acquireAsset(%assetID);

    %temp = AssetDatabase.acquireAsset(%assetID);
    %this.edit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(%assetID);
    %this.edit.asset = %assetID;
    %this.preview.NormalImage = %assetID;

    switch (%this.lockedDropdown.getSelected())
    {
        // locked
        case 0:
            %this.world.WorldLockedImage = %assetID;
        // unlocked
        case 1:
            %this.world.WorldImage = %assetID;
    }

    InterfaceTool.saveData();
}

function It_WorldPaneContainerSelectBtn::onClick(%this)
{
    AssetPicker.open("ImageAsset", "", "", %this);
}

function It_WorldPaneContainerSelectBtn::setSelectedAsset(%this, %assetID)
{
    %asset = AssetDatabase.acquireAsset(%assetID);
    %this.edit.asset = %assetID;

    %temp = AssetDatabase.acquireAsset(%assetID);
    %this.edit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(%assetID);
    //%this.edit.preview.setImage(%assetID);
    for ( %i = 1; %i < 4; %i++ )
    {
        %container = "ScoreContainer" @ %i @ %this.index;
        %container.setImage(%assetID);
    }

    %this.world.ContainerImage = %assetID;

    InterfaceTool.saveData();
}

function It_LevelPanePreviewBtn::onClick(%this)
{
    GuiPreviewGui.display(%this.preview.getObject(0));
}

function It_LevelSelectPaneIconSelectBtn::onClick(%this)
{
    // need to find a way to store or determine asset type before reaching this
    echo(" @@@ background name: " @ %this.selectedPreview);
    AssetPicker.open("ImageAsset", "", "", %this);
}

function It_LevelSelectPaneIconSelectBtn::setSelectedAsset(%this, %assetID)
{
    %asset = AssetDatabase.acquireAsset(%assetID);

    %temp = AssetDatabase.acquireAsset(%assetID);
    %this.edit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(%assetID);
    %this.edit.asset = %assetID;
    %this.preview.setImage(%assetID);
    if ( isObject(%this.selectedPreview) )
        %this.selectedPreview.setImage(%assetID);

    %index = It_LevelListWorldSelectDropdown.getSelected();
    %world = InterfaceTool.worldData.getObject(%index);
    %world.WorldBackground[%this.index] = %assetID;

    InterfaceTool.saveData();
}

function It_PreviewDisplayButton::onClick(%this)
{
    InterfaceTool.propertyPaneSelect(%this);
    %nameTag = strreplace(%this.getName(), "PreviewBtn", "");
    %dropdown = %nameTag @ "StateDropdown";

    if (isObject(%dropdown))
        %index = %dropdown.getSelected();
    else
        %index = 0;

    %edit = %nameTag @ "SelectEdit";
    %image = %edit.stateAsset[%index];
    %size = %edit.assetSize;

    if (%image $= "")
        %image = "{EditorAssets}:noImageImageMap";

    ImagePreviewGui.display(%image);
}

function It_AssetSelectButton::onClick(%this)
{
    if (!%this.active)
        return;
    InterfaceTool.propertyPaneSelect(%this);
    // need to find a way to store or determine asset type before reaching this
    %nameTag = strreplace(%this.getName(), "SelectBtn", "");

    %edit = %nameTag @ "SelectEdit";
    switch$(%edit.type)
    {
        case "gui":
            AssetPicker.open("ImageAsset", "", "gui", %this);

        case "sound":
            AssetPicker.open("AudioAsset", "", "", %this);
    }
}

function It_AssetSelectButton::setSelectedAsset(%this, %assetID, %frame)
{
    %nameTag = strreplace(%this.getName(), "SelectBtn", "");
    %dropdown = %nameTag @ "StateDropdown";

    if (isObject(%dropdown))
        %index = %dropdown.getSelected();
    else
        %index = 0;

    %edit = %nameTag @ "SelectEdit";

    if (%edit.stateControl $= "%this.levelTabSelectedWorld")
    {
        switch(%index)
        {
            case 0:
                %edit.stateAsset[0] = %assetID;
                InterfaceTool.setLevelIcons(%edit.stateAsset[0], %edit.stateAsset[1]);
                InterfaceTool.saveAllGuiObjects();

            case 1:
                %edit.stateAsset[1] = %assetID;
                InterfaceTool.setLevelIcons(%edit.stateAsset[0], %edit.stateAsset[1]);
                InterfaceTool.saveAllGuiObjects();
        }
        %temp = AssetDatabase.acquireAsset(%assetID);
        %edit.setText(%temp.AssetName);
        AssetDatabase.releaseAsset(%assetID);
        return;
    }
    if (%edit.type $= "sound")
    {
        %edit.stateAsset[%index] = %assetID;
        %temp = AssetDatabase.acquireAsset(%assetID);
        %edit.setText(%temp.AssetName);
        AssetDatabase.releaseAsset(%assetID);
    }
    else
    {
        %temp = AssetDatabase.acquireAsset(%assetID);
        %edit.setText(%temp.AssetName);
        AssetDatabase.releaseAsset(%assetID);
        %edit.stateAsset[%index] = %assetID;

        if (%edit.preview !$= "")
        {
            %previewType = %edit.preview.getClassName();
            switch$(%previewType)
            {
                case "GuiImageButtonCtrl":
                    // immediately preview change
                    %edit.preview.NormalImage = %assetID;
                    // now, set up so button reacts properly when not selected.
                    switch (%index)
                    {
                        case 0:
                            %edit.preview.NormalImage = %assetID;
                        case 1:
                            %edit.preview.HoverImage = %assetID;
                        case 2:
                            %edit.preview.DepressedImage = %assetID;
                        case 3:
                            %edit.preview.InactiveImage = %assetID;
                    }

                case "GuiSpriteCtrl":
                    %edit.preview.setImage(%assetID);
            }
        }
    }
    eval(%edit.stateControl  @ "." @ %edit.stateField[%index] @ " = \"" @ %assetID @ "\";");
    InterfaceTool.saveAllGuiObjects();
    if ( InterfaceTool.selectedPage == 1 )
        InterfaceTool.populateWorldPane();
    if ( InterfaceTool.selectedPage == 2 )
        InterfaceTool.populateLevelPane(It_LevelListWorldSelectDropdown.getSelected());
}

function It_LevelIconBtnSelectBtn::setSelectedAsset(%this, %assetID)
{
    %state = It_LevelIconBtnStateDropdown.getSelected();
    switch (%state)
    {
        case 0:
            InterfaceTool.worldData.LevelImage = %assetID;
        case 1:
            InterfaceTool.worldData.LevelLockedImage = %assetID;
    }
    %normalIcon = InterfaceTool.worldData.LevelImage;
    %lockedIcon = InterfaceTool.worldData.LevelLockedImage;
    InterfaceTool.setLevelIcons(%normalIcon, %lockedIcon);
}

function It_LargeRewardSelectButton::onClick(%this)
{
    InterfaceTool.propertyPaneSelect(%this);
    // need to find a way to store or determine asset type before reaching this
    %nameTag = strreplace(%this.getName(), "SelectBtn", "");

    %edit = %nameTag @ "SelectEdit";
    AssetPicker.open("ImageAsset", "", "gui", %this);
}

function It_LargeRewardSelectButton::setSelectedAsset(%this, %assetID)
{
    %nameTag = strreplace(%this.getName(), "SelectBtn", "");
    %dropdown = %nameTag @ "StateDropdown";

    if ( isObject(%dropdown) )
        %index = %dropdown.getSelected();
    else
        %index = 0;

    %edit = %nameTag @ "SelectEdit";

    %temp = AssetDatabase.acquireAsset(%assetID);
    %edit.setText(%temp.AssetName);
    %size = %temp.getImageSize();
    AssetDatabase.releaseAsset(%assetID);
    InterfaceTool.rewardImageSize = %size;
    %edit.stateAsset[%index] = %assetID;
    eval(%edit.stateControl  @ "." @ %edit.stateField[%index] @ " = \"" @ %assetID @ "\";");
    eval(%edit.stateControl @ ".rewardImageSize" @ " = " @ %size @ ";");
    InterfaceTool.saveAllGuiObjects();

    if (isObject(InterfaceTool.tempWinGui))
        InterfaceTool.tempWinGui.delete();
    InterfaceTool.tempWinGui = GuiUtils::duplicateGuiObject(winGui, "Preview", "It_GuiPreviewDuplicateButton");
    GuiUtils::resizeGuiObject(InterfaceTool.tempWinGui, InterfaceTool.tempWinGui.Extent, It_WinLosePreviewContainer.Extent);

    if (It_WinLosePreviewContainer.getCount())
        It_WinLosePreviewContainer.remove(It_WinLosePreviewContainer.getObject(0));

    It_WinLosePreviewContainer.addGuiControl(InterfaceTool.tempWinGui);
    InterfaceTool.rewards = InterfaceTool.createWinRewardImages();
    InterfaceTool.tempWinGui.addGuiControl(InterfaceTool.rewards);
    InterfaceTool.rewards.bringToFront(InterfaceTool.rewards);
    InterfaceTool.rewards.setVisible(winGui.showRewards);

    InterfaceTool.propertyPaneSelect(%this);
}

function It_SoundPlayButton::onClick(%this)
{
    %nameTag = strreplace(%this.getName(), "PlayBtn", "");

    %edit = %nameTag @ "SelectEdit";

    PhysicsLauncherTools::audioButtonPairClicked(%this, %edit.stateAsset[0]);
}

function It_SoundStopButton::onClick(%this)
{
    %nameTag = strreplace(%this.getName(), "StopBtn", "");
    %playBtn = %nameTag @ "PlayBtn";
    PhysicsLauncherTools::audioButtonPairStop(%playBtn);
}

function It_ButtonStateDropdown::onSelect(%this)
{
    if ($It_TabInitializing)
        return;

    %nameTag = strreplace(%this.getName(), "StateDropdown", "");
    %index = %this.getSelected();
    
    %edit = %nameTag @ "SelectEdit";
    %assetID = %edit.stateAsset[%index];
    %temp = AssetDatabase.acquireAsset(%assetID);
    %edit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(%assetID);
    if (%edit.preview !$= "")
    {
        %type = %edit.preview.getClassName();
        switch$(%type)
        {
            case "GuiSpriteCtrl":
                %edit.preview.setImage(%assetID);
            case "GuiImageButtonCtrl":
                %edit.preview.NormalImage = %assetID;
        }
    }
    InterfaceTool.propertyPaneSelect(%this);
}

function It_LargeRewardStateDropdown::onSelect(%this)
{
    if ($It_TabInitializing)
        return;

    %nameTag = strreplace(%this.getName(), "StateDropdown", "");
    %index = %this.getSelected();
    
    %edit = %nameTag @ "SelectEdit";
    %assetID = %edit.stateAsset[%index];
    %temp = AssetDatabase.acquireAsset(%assetID);
    %edit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(%assetID);
    InterfaceTool.propertyPaneSelect(%this);
}

function It_WorldPaneEditWorldBtn::onClick(%this)
{
    WorldTool.load();
    WorldTool.WorldListContainer.setSelected(%this.index);
}

function initializeButtonStateDropdown(%dropdown)
{
    %dropdown.clear();
    %dropdown.add("Up", 0);
    %dropdown.add("Hover", 1);
    %dropdown.add("Down", 2);
    %dropdown.add("Inactive", 3);
    %dropdown.setFirstSelected();
}

function setStateAsset(%dropdown, %assetID)
{
    %nameTag = strreplace(%dropdown, "StateDropdown", "");
    %index = %dropdown.getSelected();
    
    %edit = %nameTag @ "SelectEdit";
    %edit.stateAsset[%index] = %assetID;
    %temp = AssetDatabase.acquireAsset(%assetID);
    %edit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(%assetID);
}

function InterfaceTool::saveAllGuiObjects(%this)
{
    if (isObject(mainMenuGui))
    {
        mainMenuGui.canSaveDynamicFields = true;
        TamlWrite(mainMenuGui, expandPath("^PhysicsLauncherTemplate/gui/mainMenu.gui.taml"));
    }

    if (isObject(worldSelectGui))
    {
        worldSelectGui.canSaveDynamicFields = true;
        TamlWrite(worldSelectGui, expandPath("^PhysicsLauncherTemplate/gui/worldSelect.gui.taml"));
    }

    if (isObject(levelSelectGui))
    {
        levelSelectGui.canSaveDynamicFields = true;
        TamlWrite(levelSelectGui, expandPath("^PhysicsLauncherTemplate/gui/levelSelect.gui.taml"));
    }

    if (isObject(HudGui))
    {
        HudGui.canSaveDynamicFields = true;
        TamlWrite(HudGui, expandPath("^PhysicsLauncherTemplate/gui/hud.gui.taml"));
    }

    if (isObject(winGui))
    {
        winGui.canSaveDynamicFields = true;
        %temp = AssetDatabase.acquireAsset(winGui.noRewardImage);
        %width = %temp.getCellWidth();
        %height = %temp.getCellHeight();
        if (%width < 1)
        {
            %width = %temp.getImageWidth();
            %height = %temp.getImageHeight();
        }
        winGui.rewardImageSize = %width SPC %height;
        AssetDatabase.releaseAsset(winGui.noRewardImage);
        TamlWrite(winGui, expandPath("^PhysicsLauncherTemplate/gui/win.gui.taml"));

        loseGui.canSaveDynamicFields = true;
        TamlWrite(loseGui, expandPath("^PhysicsLauncherTemplate/gui/lose.gui.taml"));
    }

    if (isObject(pauseGui))
    {
        pauseGui.canSaveDynamicFields = true;
        TamlWrite(pauseGui, expandPath("^PhysicsLauncherTemplate/gui/pause.gui.taml"));
    }

    if (isObject(creditsGui))
    {
        creditsGui.canSaveDynamicFields = true;
        TamlWrite(creditsGui, expandPath("^PhysicsLauncherTemplate/gui/credits.gui.taml"));
    }
}

function InterfaceTool::saveFonts(%this)
{
    if ($It_TabInitializing || !$InterfaceToolInitialized || $InterfaceTool::TabSwitch)
        return;

    if (isObject(GuiGameMLTextCenteredLgProfile))
        GuiGameMLTextCenteredLgProfile.save(expandPath("^PhysicsLauncherTemplate/gui/MLTextCenteredLgProfile.cs"));

    if (isObject(GuiGameTextCenteredLgProfile))
        GuiGameTextCenteredLgProfile.save(expandPath("^PhysicsLauncherTemplate/gui/TextCenteredLgProfile.cs"));

    if (isObject(GuiGameTextLeftLgProfile))
        GuiGameTextLeftLgProfile.save(expandPath("^PhysicsLauncherTemplate/gui/TextLeftLgProfile.cs"));

    if (isObject(GuiGameTextRightLgProfile))
        GuiGameTextRightLgProfile.save(expandPath("^PhysicsLauncherTemplate/gui/TextRightLgProfile.cs"));

    if (isObject(GuiGameMLTextCenteredSmProfile))
        GuiGameMLTextCenteredSmProfile.save(expandPath("^PhysicsLauncherTemplate/gui/MLTextCenteredSmProfile.cs"));

    if (isObject(GuiGameTextCenteredSmProfile))
        GuiGameTextCenteredSmProfile.save(expandPath("^PhysicsLauncherTemplate/gui/TextCenteredSmProfile.cs"));

    if (isObject(GuiGameTextLeftSmProfile))
        GuiGameTextLeftSmProfile.save(expandPath("^PhysicsLauncherTemplate/gui/TextLeftSmProfile.cs"));

    if (isObject(GuiGameTextRightSmProfile))
        GuiGameTextRightSmProfile.save(expandPath("^PhysicsLauncherTemplate/gui/TextRightSmProfile.cs"));
}

function It_WinLoseRewardsToggleCheckbox::onClick(%this)
{
    winGui.showRewards = %this.getValue();

    InterfaceTool.rewards.setVisible(%this.getValue());
}

function It_WorldPaneLockStateDropdown::initialize(%this)
{
    %this.add("Locked", 0);
    %this.add("Unlocked", 1);
}

function It_WorldPaneLockStateDropdown::onSelect(%this)
{
    %this.world.WorldLocked = !(%this.getSelected());
    switch (%this.getSelected())
    {
        case 0:
            %this.preview.NormalImage = %this.world.WorldLockedImage;
        case 1:
            %this.preview.NormalImage = %this.world.WorldImage;
    }
    %temp = AssetDatabase.acquireAsset(%this.preview.NormalImage);
    %this.edit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(%this.preview.NormalImage);
    %this.edit.asset = %this.preview.NormalImage;
}

function It_AssetBrowseEditClick::onMouseEnter(%this)
{
    if (!%this.active)
        return;
    %this.button.NormalImageCache = %this.button.NormalImage;
    %this.button.NormalImage = %this.button.HoverImage;
}

function It_AssetBrowseEditClick::onMouseLeave(%this)
{
    if (!%this.active)
        return;
    %this.button.NormalImage = %this.button.NormalImageCache;
}

function It_AssetBrowseEditClick::onMouseUp(%this)
{
    if (!%this.active)
        return;
    %this.button.NormalImage = %this.button.HoverImage;
    %this.button.onClick();
}

function It_AssetBrowseEditClick::onMouseDown(%this)
{
    if (!%this.active)
        return;
    %this.button.NormalImage = %this.button.DownImage;
}

function It_GuiContainerPreviewButton::onMouseDown(%this)
{
    if (isObject($InterfaceTool::highlightedPreview.highlight))
    {
        $InterfaceTool::highlightedPreview.remove($InterfaceTool::highlight);
        $InterfaceTool::highlightedPane.setProfile($InterfaceTool::oldPaneProfile);
    }
    if (!isObject($InterfaceTool::highlight))
    {
        $InterfaceTool::highlight = new GuiControl()
        {
            Profile = "GuiSelectedElementHighlight";
        };
    }

    $InterfaceTool::highlightedPreview = %this;
    %this.getParent().add($InterfaceTool::highlight);
    $InterfaceTool::highlight.setPosition(%this.Position.x - 10, %this.Position.y - 10);
    $InterfaceTool::highlight.Extent = %this.Extent.x + 20 SPC %this.Extent.y + 20;

    %pane = InterfaceTool.findPreviewObject(%this.parentPane.getName());
    if ($InterfaceTool::highlightedPane !$= "")
        $InterfaceTool::highlightedPane.setProfile($InterfaceTool::oldPaneProfile);

    $InterfaceTool::oldPaneProfile = %pane.Profile;
    $InterfaceTool::highlightedPane = %pane;
    $InterfaceTool::highlightedPane.setProfile(GuiNarrowPanelContainerHighlight);
}

function It_GuiPreviewDuplicateButton::onClick(%this)
{
    if (%this.getName() $= "ProjectileSlot0Preview")
        return;

    if ( isObject($InterfaceTool::highlightedPreview) )
    {
        %type = $InterfaceTool::highlightedPreview.getClassName();
        if ( %type $= "GuiImageButtonCtrl" )
        {
            %sourceName = strreplace($InterfaceTool::highlightedPreview.getName(), %this.data, "");
            $InterfaceTool::highlightedPreview.NormalImage = %sourceName.NormalImage;
            $InterfaceTool::highlightedPreview.HoverImage = %sourceName.HoverImage;
            $InterfaceTool::highlightedPreview.DownImage = %sourceName.DownImage;
            $InterfaceTool::highlightedPreview.InactiveImage = %sourceName.InactiveImage;
        }
    }
    if (isObject($InterfaceTool::highlightedPreview.highlight))
    {
        $InterfaceTool::highlightedPreview.remove($InterfaceTool::highlight);
        $InterfaceTool::highlightedPane.setProfile($InterfaceTool::oldPaneProfile);
    }
    if (!isObject($InterfaceTool::highlight))
    {
        $InterfaceTool::highlight = new GuiControl()
        {
            Profile = "GuiSelectedElementHighlight";
        };
    }

    $InterfaceTool::highlightedPreview = %this;
    %this.getParent().add($InterfaceTool::highlight);
    $InterfaceTool::highlight.setPosition(%this.Position.x - 10, %this.Position.y - 10);
    $InterfaceTool::highlight.Extent = %this.Extent.x + 20 SPC %this.Extent.y + 20;

    %pane = InterfaceTool.findPreviewObject(%this.getName());
    if ($InterfaceTool::highlightedPane !$= "")
        $InterfaceTool::highlightedPane.setProfile($InterfaceTool::oldPaneProfile);

    $InterfaceTool::oldPaneProfile = %pane.Profile;
    $InterfaceTool::highlightedPane = %pane;
    if ( isObject($InterfaceTool::highlightedPane) )
        $InterfaceTool::highlightedPane.setProfile(GuiLargePanelContainerHighlight);
}

function It_GuiPropertyPaneSelect::onMouseDown(%this)
{
    if ( !%this.isActive() )
        return;

    if (isObject($InterfaceTool::highlightedPreview.highlight))
    {
        $InterfaceTool::highlightedPreview.remove($InterfaceTool::highlight);
        $InterfaceTool::highlightedPane.setProfile($InterfaceTool::oldPaneProfile);
    }
    if (!isObject($InterfaceTool::highlight))
    {
        $InterfaceTool::highlight = new GuiControl()
        {
            Profile = "GuiSelectedElementHighlight";
        };
    }

    %count = %this.getCount();
    %i = 0;
    while ( %i < %count )
    {
        %obj = %this.getObject(%i);
        if (%obj.preview !$= "")
        {
            %preview = %obj.preview;
            break;
        }
        %i++;
    }

    if ( %preview !$= "" )
    {
        %previewParentName = %preview.getParent().getName();
        %rewardName = strreplace(%previewParentName, "RewardPreview", "");
        if ( %previewParentName $= %rewardName )
        {
            %preview.getParent().add($InterfaceTool::highlight);
            $InterfaceTool::highlightedPreview = %preview;
            $InterfaceTool::highlight.setPosition(%preview.Position.x - 10, %preview.Position.y - 10);
            $InterfaceTool::highlight.Extent = %preview.Extent.x + 20 SPC %preview.Extent.y + 20;
        }
        else
        {
            %parent = %preview.getParent();
            %parent.getParent().add($InterfaceTool::highlight);
            $InterfaceTool::highlightedPreview = %parent;
            $InterfaceTool::highlight.setPosition(%parent.Position.x - 10, %parent.Position.y - 10);
            $InterfaceTool::highlight.Extent = %parent.Extent.x + 20 SPC %parent.Extent.y + 20;
        }
    }
    if ( isObject( $InterfaceTool::highlightedPane ) )
        $InterfaceTool::highlightedPane.setProfile($InterfaceTool::oldPaneProfile);
    $InterfaceTool::highlightedPane = %this;
    $InterfaceTool::oldPaneProfile = %this.Profile;
    %test = strreplace(%this.Profile, "Narrow", "");
    if (%test $= %this.Profile)
        $InterfaceTool::highlightedPane.setProfile(GuiLargePanelContainerHighlight);
    else
        $InterfaceTool::highlightedPane.setProfile(GuiNarrowPanelContainerHighlight);
}

function It_GuiRewardPreview::onMouseDown(%this)
{
    if (isObject($InterfaceTool::highlightedPreview.highlight))
    {
        $InterfaceTool::highlightedPreview.remove($InterfaceTool::highlight);
        $InterfaceTool::highlightedPane.setProfile($InterfaceTool::oldPaneProfile);
    }
    if (!isObject($InterfaceTool::highlight))
    {
        $InterfaceTool::highlight = new GuiControl()
        {
            Profile = "GuiSelectedElementHighlight";
        };
    }

    $InterfaceTool::highlightedPreview = %this.parentPane;
    %this.parentPane.getParent().add($InterfaceTool::highlight);
    $InterfaceTool::highlight.setPosition(%this.parentPane.Position.x - 10, %this.parentPane.Position.y - 10);
    $InterfaceTool::highlight.Extent = %this.Extent.x + 20 SPC %this.Extent.y + 20;

    %pane = It_WinLoseLargeRewardBtnContainer;
    if ($InterfaceTool::highlightedPane !$= "")
        $InterfaceTool::highlightedPane.setProfile($InterfaceTool::oldPaneProfile);

    $InterfaceTool::oldPaneProfile = %pane.Profile;
    $InterfaceTool::highlightedPane = %pane;
    $InterfaceTool::highlightedPane.setProfile(GuiLargePanelContainerHighlight);
}

function It_LevelScreenSelectButton::onMouseUp(%this)
{
    %parent = %this.getParent();
    %parent.index = %this.index;
    %parent.imageName = %parent.backgroundEdit.getText();
    It_LevelListContentPane.selectPane(%parent);
}

function It_LevelScreenSelectButton::onMouseEnter(%this)
{
    %parent = %this.getParent();
    %parent.setProfile(%parent.highlightProfile);
}

function It_LevelScreenSelectButton::onMouseLeave(%this)
{
    %parent = %this.getParent();
    if (%parent.selected == false)
        %parent.setProfile(%parent.normalProfile);
}

function It_LevelSmallRewardBtnSelectBtn::onClick(%this)
{
    if (!%this.active)
        return;
    InterfaceTool.propertyPaneSelect(%this);
    // need to find a way to store or determine asset type before reaching this
    AssetPicker.open("ImageAsset", "", "gui", %this);
}

function It_LevelSmallRewardBtnSelectBtn::setSelectedAsset(%this, %assetID, %frame)
{
    %nameTag = strreplace(%this.getName(), "SelectBtn", "");
    %dropdown = %nameTag @ "StateDropdown";

    if (isObject(%dropdown))
        %index = %dropdown.getSelected();

    %edit = %nameTag @ "SelectEdit";

    eval(%edit.stateControl  @ "." @ %edit.stateField[%index] @ " = \"" @ %assetID @ "\";");
    if ( %index > 0 )
    {
        // only set the world select screen reward image to the "earned" image
        // state.
        StarDisplay.setImage(%assetID);
        StarDisplay.setImageFrame(%frame);
    }
    InterfaceTool.saveAllGuiObjects();
    if ( InterfaceTool.selectedPage == 1 )
        InterfaceTool.populateWorldPane();
    if ( InterfaceTool.selectedPage == 2 )
        InterfaceTool.populateLevelPane(It_LevelListWorldSelectDropdown.getSelected());
}
