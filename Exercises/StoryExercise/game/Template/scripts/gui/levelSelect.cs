//-----------------------------------------------------------------------------
// Three Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

$GameLevelFileSpec = "*.taml";
$levelLoading = false;

/// <summary>
/// This function initializes the GUI.
/// </summary>
function levelSelectGui::onWake(%this)
{
    %this.getLevelList();
    %this.currentLevelIndex = 0;
    %this.selectedPage = 0;
    %this.rewardCount = $WorldListData.rewardCount;
    %this.displayLevels(%this.currentLevelIndex);
    if (%this.pages > 1)
    {
        %this.displayPageButtons();
    }

    if (%this.levelCount < 15)
    {
        LevelSelectBackBtn.Visible = false;
        LevelSelectNextBtn.Visible = false;
    }
    else 
    {
        LevelSelectBackBtn.Visible = false;
        LevelSelectNextBtn.Visible = true;
    }
}

/// <summary>
/// This function cleans up the page indicator/selector button display.
/// </summary>
function levelSelectGui::onDialogPop(%this)
{
    if (isObject(PageButtonContainer))
    {
        %object = PageButtonContainer.getObject(0);
        while (isObject(%object))
        {
            PageButtonContainer.remove(%object);
            %object.delete();
            %object = PageButtonContainer.getObject(0);
        }
        levelSelectGui.remove(PageButtonContainer);
        PageButtonContainer.delete();
    }
    Canvas.popDialog(pauseGui);
    Canvas.popDialog(winGui);
    Canvas.popDialog(loseGui);
}

/// <summary>
/// This function ensures that the selected page is reset to 0.
/// </summary>
function levelSelectGui::onDialogPush(%this)
{
    %this.onWake();
}

/// <summary>
/// This function accesses the list of levels and their button images attached to 
/// the levelSelectGui object.
/// </summary>
function levelSelectGui::getLevelList(%this)
{
    // clean the temp list.
    for (%i = 0; %i < %this.levelCount; %i++)
    {
        %this.CurrentLevelList[%i] = "";
    }
    // get and clean the level file list from levelSelectGui
    %this.currentWorld = $WorldListData.getObject($SelectedWorld + 1);
    %count = 0;
    %k = 0;
    for ( %i = 0; %i < %this.currentWorld.WorldLevelCount; %i++ )
    {
        if (%this.currentWorld.LevelList[%i] !$= "")
        {
            // if the file exists, add it and its associated image to the list
            %tamlFile = expandPath("^PhysicsLauncherTemplate/data/levels/" @ %this.currentWorld.LevelList[%count] @ ".scene.taml");
            %dsoFile = expandPath("^PhysicsLauncherTemplate/data/levels/" @ %this.currentWorld.LevelList[%count] @ ".scene.baml");
            if (isFile(%tamlFile) || isFile(%dsoFile))
            {
                %this.CurrentLevelList[%k] = %this.currentWorld.LevelList[%i];
                %this.CurrentLevelImageList[%k] = $WorldListData.LevelImage;
                %this.CurrentLevelLockedImage[%k] = $WorldListData.LevelLockedImage;
                %this.CurrentLevelLocked[%k] = %this.currentWorld.LevelLocked[%i];
                %this.CurrentLevelHighScore[%k] = %this.currentWorld.LevelHighScore[%i];
                %k++;
            }
        }
    }
    %this.levelCount = %k;

    %this.pages = %this.levelCount / 15;
}

/// <summary>
/// This function displays a page of available levels.  It paginates by using the
/// passed startIndex to find its place in the level list.
/// </summary>
/// <param name="startIndex">The index of the first level to display on this page.</param>
function levelSelectGui::displayLevels(%this, %startIndex)
{
    // LevelCompleted
    // LevelList
    // LevelImageList
    // LevelWorldAssigned
    // WorldBackground
    %this.hideRewardImages();
    
    for (%i = 0; %i < 15; %i++)
    {
        %mouseEvent = "MouseEvent" @ %i+1;
        %mouseEvent.Visible = false;
        %button = "LevelSelectButton" @ %i+1;
        %button.Visible = false;
        %text = "LevelSelectLabel" @ %i+1;
        %text.Visible = false;
        %score = "LevelSelectScore" @ %i+1;
        %score.Visible = false;
    }

    %last = %startIndex + 15;
    if (%last > %this.levelCount)
        %last = %this.levelCount;

    %j = 1;
    for (%k = %startIndex; %k < %last; %k++)
    {
        %mouseEvent = "MouseEvent" @ %j;
        %button = "LevelSelectButton" @ %j;
        %text = "LevelSelectLabel" @ %j;
        %score = "LevelSelectScore" @ %j;
        %score.text = %this.CurrentLevelHighScore[%k];
        if (%this.currentLevelLocked[%k])
            %score.Visible = false;
        else 
            %score.Visible = %this.showScore;
        %mouseEvent.Visible = true;
        %mouseEvent.index = %j;
        %mouseEvent.level = %this.CurrentLevelList[%k];
        %mouseEvent.locked = %this.CurrentLevelLocked[%k];
        %button.index = %k;
        %text.index = %k;
        %text.text = %this.CurrentLevelList[%k];
        //%text.Visible = true;
        %text.level = %this.CurrentLevelList[%k];
        %text.locked = %this.CurrentLevelLocked[%k];
        %button.level = %this.CurrentLevelList[%k];
        %button.setNormalImage(%this.CurrentLevelLocked[%k] ? %this.CurrentLevelLockedImage[%k] : %this.CurrentLevelImageList[%k]);
        %button.setHoverImage(%button.NormalImage);
        %button.setDownImage(%button.NormalImage);
        %button.locked = %this.CurrentLevelLocked[%k];
        %button.Visible = true;

        if (!%this.CurrentLevelLocked[%k])
            %this.displayRewardImages(%j);

        %j++;
    }
    LevelSelectBackground.setImage(%this.currentWorld.WorldBackground[%this.selectedPage]);
}

function levelSelectGui::hideRewardImages(%this)
{
    for (%i = 1; %i < 16; %i++)
    {
        for (%j = 0; %j < %this.rewardCount; %j++)
        {
            %image = "RewardImage_" @ %i @ "_" @ %j;
            if (isObject(%image))
                %image.setVisible(false);
        }
    }
}

/// <summary>
/// This function handles visibility and layout of the level reward images
/// </summary>
/// <param name="index">The index of the level selection button to display images on.</param>
function levelSelectGui::displayRewardImages(%this, %index)
{
    // Create and add the page indicator/selector buttons.
    // Take the number of pages, figure a total width, spread the buttons out
    // so that they're centered on the page.

    // First, create our reward image container if it does not exist.
    %container = "RewardContainer" @ %index;
    if (!isObject(%container))
    {
        %currentButton = "LevelSelectButton" @ %index;
        %ExtX = getWord(%currentButton.Extent, 0) - 5;
        %posX = getWord(%currentButton.Position, 0);
        %posX += 5;
        new GuiControl(%container) {
            canSaveDynamicFields = "0";
            isContainer = "1";
            Profile = "GuiTransparentProfile";
            HorizSizing = "relative";
            VertSizing = "relative";
            Position = %posX SPC (getWord(%currentButton.Position, 1) + (getWord(%currentButton.Extent, 1) - 42));
            Extent = %ExtX SPC "32";
            MinExtent = "8 2";
            canSave = "1";
            Visible = "1";
            hovertime = "1000";
        };
        %this.add(%container);
    }

    // Next, create reward image instances as needed
    for (%i = 0; %i < %this.rewardCount; %i++)
    {
        %rewardImage = "RewardImage_" @ %index @ "_" @ %i;
        if (!isObject(%rewardImage))
        {
            %container[%i] = new GuiSpriteCtrl(%rewardImage) {
                canSaveDynamicFields = "1";
                isContainer = "0";
                Profile = "GuiTransparentProfile";
                HorizSizing = "relative";
                VertSizing = "relative";
                Position = "0 8";
                Extent = "24 24";
                MinExtent = "8 2";
                canSave = "1";
                Visible = "1";
                hovertime = "1000";
                bitmap = %this.noRewardImage;
                    index = %i;
            };
            %container.add(%rewardImage);
        }
    }

    // Finally, set the images on the bitmap controls to correctly indicate the number of 
    // earned stars
    %containerWidth = getWord(%container.Extent, 0);
    %startX = (%containerWidth / 2) - (((%this.rewardCount * 12) + ((%this.rewardCount - 1) * 10)) / 2) - 13;
    %levelIndex = %index + (%this.selectedPage * 15) - 1;
    %earnedStars = %this.currentWorld.LevelStars[%levelIndex];
    for (%i = 0; %i < %this.rewardCount; %i++)
    {
        %rewardImage = "RewardImage_" @ %index @ "_" @ %i;
        %rewardImage.Position = (%startX + (%i * 24)) SPC "8";
        %rewardImage.setImage(%i < %earnedStars ? %this.rewardImage : %this.noRewardImage);
        %rewardImage.setVisible(true);
    }
    // Ugly way to pull the mouse event control over the whole level selection button 
    // arrangement so that clicking over the reward container will still trigger the
    // level selection.
    %mouseEvent = "MouseEvent" @ %index;
    %this.remove(%mouseEvent);
    %this.add(%mouseEvent);
}

/// <summary>
/// This function handles visibility and layout of the level page buttons.
/// </summary>
function levelSelectGui::displayPageButtons(%this)
{
    // Create and add the page indicator/selector buttons.
    // Take the number of pages, figure a total width, spread the buttons out
    // so that they're centered on the page.

    // First, create our page button container if it does not exist.
    if (!isObject(PageButtonContainer))
    {
        %PosX = LevelSelectBackBtn.Position.x + LevelSelectBackBtn.Extent.x + 10;
        %PosY = LevelSelectBackBtn.Position.y + (LevelSelectBackBtn.Extent.y / 2) - 16;
        %ExtX = (LevelSelectNextBtn.Position.x - 10) - %PosX;
        new GuiControl(PageButtonContainer) {
            canSaveDynamicFields = "0";
            isContainer = "1";
            Profile = "GuiTransparentProfile";
            HorizSizing = "relative";
            VertSizing = "relative";
            Position = %PosX SPC %PosY;
            Extent = %ExtX SPC "32";
            MinExtent = "8 2";
            canSave = "1";
            Visible = "1";
            hovertime = "1000";
        };
        levelSelectGui.add(PageButtonContainer);
    }

    // Next, create button instances as needed
    for (%i = 0; %i < %this.pages; %i++)
    {
        %buttonName = "PageButton" @ %i;
        if (!isObject(%buttonName))
        {
            %this.pageButtons[%i] = new GuiImageButtonCtrl(%buttonName) {
                canSaveDynamicFields = "0";
                class = "LevelPageButton";
                isContainer = "0";
                Profile = "GuiDefaultProfile";
                HorizSizing = "relative";
                VertSizing = "relative";
                Position = "0 8";
                Extent = "16 16";
                MinExtent = "8 2";
                canSave = "1";
                Visible = "1";
                hovertime = "1000";
                groupNum = "-1";
                buttonType = "PushButton";
                useMouseEvents = "0";
                isLegacyVersion = "0";
                NormalImage = %this.pageIndicatorOff;
                HoverImage = %this.pageIndicatorOff;
                DownImage = %this.pageIndicatorOff;
                    index = %i;
            };
            PageButtonContainer.add(%buttonName);
        }
    }

    // Finally, set the images on the buttons to correctly indicate which page is selected 
    // and place the buttons centered horizontally in the container.
    %containerWidth = PageButtonContainer.Extent.x;
    %spacing = 24;
    %buttonWidth = 16;
    %pad = %this.pages % 2 * %spacing;
    %startX = (%containerWidth / 2) - (((%this.pages * %buttonWidth) + %pad + ((%this.pages - 1) * %spacing)) / 2);
    for (%i = 0; %i < %this.pages; %i++)
    {
        %buttonName = "PageButton" @ %i;
        %buttonName.Position = %startX + (%i * (%buttonWidth + %spacing)) SPC "8";
        %buttonName.setNormalImage(%i == %this.selectedPage ? %this.pageIndicatorOn : %this.pageIndicatorOff);
        %buttonName.setHoverImage(%i == %this.selectedPage ? %this.pageIndicatorOn : %this.pageIndicatorOff);
        %buttonName.setDownImage(%i == %this.selectedPage ? %this.pageIndicatorOn : %this.pageIndicatorOff);
    }
}

/// <summary>
/// This function handles all level LevelPageButton clicks.  It uses the index 
/// of the selected button to access the desired level page.
/// </summary>
function LevelPageButton::onClick(%this)
{
    levelSelectGui.currentLevelIndex = %this.index * 15;
    if (levelSelectGui.currentLevelIndex > levelSelectGui.levelCount)
        levelSelectGui.currentLevelIndex = mFloor(levelSelectGui.levelCount / 15);

    levelSelectGui.selectedPage = levelSelectGui.currentLevelIndex / 15;

    levelSelectGui.displayLevels(levelSelectGui.currentLevelIndex);
    levelSelectGui.displayPageButtons();

    if (levelSelectGui.selectedPage < 1 && levelSelectGui.levelCount > levelSelectGui.currentLevelIndex)
    {
        LevelSelectBackBtn.Visible = false;
        LevelSelectNextBtn.Visible = true;
    }
    else if (levelSelectGui.selectedPage > 0 && levelSelectGui.levelCount > levelSelectGui.currentLevelIndex + 15)
    {
        LevelSelectBackBtn.Visible = true;
        LevelSelectNextBtn.Visible = true;
    }
    else if (levelSelectGui.selectedPage > 0 && levelSelectGui.levelCount <= levelSelectGui.currentLevelIndex + 15)
    {
        LevelSelectBackBtn.Visible = true;
        LevelSelectNextBtn.Visible = false;
    }
    LevelSelectBackground.setImage(levelSelectGui.currentWorld.WorldBackground[levelSelectGui.selectedPage]);
}

/// <summary>
/// Selects and triggers the loading of a level (if unlocked) associated with
/// the index of the selected button/label.
/// </summary>
function LevelSelectEvent::onMouseDown(%this)
{
    if (!%this.locked)
    {
        alxStopAll();
        if (isObject(MainScene))
            MainScene.delete();

        $LevelIndex = %this.index + (levelSelectGui.selectedPage * 15) - 1;
        $SelectedLevel = "^PhysicsLauncherTemplate/data/levels/" @ levelSelectGui.CurrentLevelList[$LevelIndex] @ ".scene.taml";

        if (!$levelLoading)
        {
            $levelLoading = true;
            loadLevel($SelectedLevel);
        }

        ScheduleManager.initialize();
        Canvas.schedule(500, popDialog, levelSelectGui);
        Canvas.schedule(500, pushDialog, HudGui);
        HudGui.setup();
        helpGui.setup();
    }
}

function panCameraToStart()
{
    %launcherGroup = "LauncherSceneGroup";
    
    if (isObject(%launcherGroup))
    {
        %launcherPosition = %launcherGroup.getObject(0).position;
        sceneWindow2D.setTargetCameraPosition(%launcherPosition);
        sceneWindow2D.startCameraMove(1.0);
    }
    
    sceneWindow2D.canMoveCamera = true;
}

/// <summary>
/// This function handles the next button and updates the back button as needed.
/// </summary>
function LevelSelectNextBtn::onClick(%this)
{
    levelSelectGui.currentLevelIndex += 15;
    if (levelSelectGui.currentLevelIndex > levelSelectGui.levelCount)
        levelSelectGui.currentLevelIndex = mFloor(levelSelectGui.levelCount / 15);

    levelSelectGui.selectedPage = levelSelectGui.currentLevelIndex / 15;

    levelSelectGui.displayLevels(levelSelectGui.currentLevelIndex);
    levelSelectGui.displayPageButtons();

    if (levelSelectGui.selectedPage < 1 && levelSelectGui.levelCount > levelSelectGui.currentLevelIndex)
    {
        LevelSelectBackBtn.Visible = false;
        LevelSelectNextBtn.Visible = true;
    }
    else if (levelSelectGui.selectedPage > 0 && levelSelectGui.levelCount > levelSelectGui.currentLevelIndex + 15)
    {
        LevelSelectBackBtn.Visible = true;
        LevelSelectNextBtn.Visible = true;
    }
    else if (levelSelectGui.selectedPage > 0 && levelSelectGui.levelCount <= levelSelectGui.currentLevelIndex + 15)
    {
        LevelSelectBackBtn.Visible = true;
        LevelSelectNextBtn.Visible = false;
    }
    LevelSelectBackground.setImage(levelSelectGui.currentWorld.WorldBackground[levelSelectGui.selectedPage]);
}

/// <summary>
/// This function returns the user to the first page of levels.
/// </summary>
function LevelSelectHomeBtn::onClick(%this)
{
    Canvas.pushDialog(worldSelectGui);
    Canvas.schedule(150, popDialog, levelSelectGui);
}

/// <summary>
/// This function handles the back button and updates the next button if needed.
/// </summary>
function LevelSelectBackBtn::onClick(%this)
{
    levelSelectGui.currentLevelIndex -= 15;
    if (levelSelectGui.currentLevelIndex < 0)
        levelSelectGui.currentLevelIndex = 0;

    levelSelectGui.selectedPage = levelSelectGui.currentLevelIndex / 15;

    levelSelectGui.displayLevels(levelSelectGui.currentLevelIndex);
    levelSelectGui.displayPageButtons();

    if (levelSelectGui.selectedPage < 1 && levelSelectGui.levelCount > levelSelectGui.currentLevelIndex)
    {
        LevelSelectBackBtn.Visible = false;
        LevelSelectNextBtn.Visible = true;
    }
    else if (levelSelectGui.selectedPage > 0 && levelSelectGui.levelCount > levelSelectGui.currentLevelIndex + 15)
    {
        LevelSelectBackBtn.Visible = true;
        LevelSelectNextBtn.Visible = true;
    }
    else if (levelSelectGui.selectedPage > 0 && levelSelectGui.levelCount <= levelSelectGui.currentLevelIndex + 15)
    {
        LevelSelectBackBtn.Visible = true;
        LevelSelectNextBtn.Visible = false;
    }
    LevelSelectBackground.setImage(levelSelectGui.currentWorld.WorldBackground[levelSelectGui.selectedPage]);
}