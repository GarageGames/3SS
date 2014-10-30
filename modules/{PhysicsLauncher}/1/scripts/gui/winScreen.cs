//-----------------------------------------------------------------------------
// Three Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

/// <summary>
/// Displays the win screen with score, stars earned and the high score
/// </summary>
function winGui::onWake(%this)
{
    %highScore = HighScoreDisplay.text;
    %score = GameMaster.Score + GameMaster.Bonus;
    %this.rewardCount = $WorldListData.rewardCount;

    if (%highScore > %score)
        winLevelHighScoreLabel.setText(%highScore);
    else
    {
        HighScoreDisplay.setText(%score);
        winLevelHighScoreLabel.setText(%score);
        levelSelectGui.currentWorld.LevelHighScore[$LevelIndex] = %score;
    }
    
    %stars = levelSelectGui.currentWorld.LevelStars[$LevelIndex];
    %reward = %this.getRewardCount(%score);

    levelSelectGui.currentWorld.LevelStars[$LevelIndex] = (%reward > %stars ? %reward : %stars);

    %this.displayScore(%score);

    if (levelSelectGui.currentWorld.LevelList[$LevelIndex + 1] !$= "" || $SelectedWorld < (worldSelectGui.worldCount - 1))
        winNextLevelButton.Visible = true;
    else
        winNextLevelButton.Visible = false;

    if (%this.showRewards)
        %this.displayRewards(%reward);

    %this.unLockNextLevel();

    alxStopAll();

    if (AssetDatabase.isDeclaredAsset(%this.music))
        %this.playing = alxPlay(%this.music);
}

function winGui::clearRewardImages(%this)
{
    %count = %this.rewardContainer.getCount();
    for (%i = 0; %i < %count; %i++)
    {
        %obj = %this.rewardContainer.getObject(0);
        %this.rewardContainer.remove(%obj);
        %obj.delete();
    }
    %this.remove(%this.rewardContainer);
    %this.rewardContainer.delete();
}

function winGui::onSleep(%this)
{
    alxStop(%this.playing);
}

/// <summary>
/// Unlocks the next level in the current world, or unlocks the next world and its
/// first level, when the player successfully completes a level.
/// </summary>
function winGui::unLockNextLevel(%this)
{
    levelSelectGui.currentWorld.WorldProgress = %this.getWorldProgress();
    if(GameMaster.score > levelSelectGui.currentWorld.LevelHighScore[$LevelIndex])
         levelSelectGui.currentWorld.LevelHighScore[$LevelIndex] = GameMaster.score;

    %rewards = %this.getRewardCount(GameMaster.Score + GameMaster.Bonus);
    levelSelectGui.currentWorld.LevelStars[$LevelIndex] = (%rewards ? %rewards : "0");

    if (levelSelectGui.currentWorld.LevelList[$LevelIndex + 1] !$= "")
    {
        levelSelectGui.currentWorld.LevelLocked[$LevelIndex + 1] = false;
    }
    else
    {
        %world = worldSelectGui.currentWorldList[$SelectedWorld + 1];
        if (%world.WorldLocked)
            %world.WorldLocked = false;
        if (%world.LevelLocked[0])
            %world.LevelLocked[0] = false;
    }
    TamlWrite($WorldListData, $PhysicsLauncher::WorldListFile);
}

/// <summary>
/// This is used to get the current world progress.  It examines high scores of 
/// previous levels to determine which levels have been completed and returns the
/// total number of completed levels.
/// </summary>
function winGui::getWorldProgress(%this)
{
    %i = 0;
    for (; %i < worldSelectGui.currentWorldList[$SelectedWorld].WorldLevelCount; %i++)
    {
        %j = worldSelectGui.currentWorldList[$SelectedWorld].LevelHighScore[%i];
        if (!worldSelectGui.currentWorldList[$SelectedWorld].LevelHighScore[%i])
            break;
    }
    return %i;
}

function winGui::displayScore(%this, %score)
{
    %increment = mFloor(%score / (3000 / 32));
    %totalTime = 3000;
    %timeCount = 32;
    while (%totalTime > 32)
    {
        %tally += %increment;
        %this.schedule(%timeCount, "updateScoreDisplay", %tally);
        %timeCount += 32;
        %totalTime -= 32;
    }
    if (%tally < %score)
        %this.schedule(%timeCount, "updateScoreDisplay", %score);
}

function winGui::updateScoreDisplay(%this, %text)
{
    winLevelScoreLabel.setText(%text);
    alxPlay(%this.scoreSound);
}

function winGui::displayRewards(%this, %count)
{
    %this.displayRewardImages(0);
    if (%count > 0)
    {
        for (%i = 0; %i < %count; %i++)
        {
            %this.schedule((1000 * (%i + 1)), "displayRewardImages", (%i + 1));
        }
    }
}

/// <summary>
/// This function handles visibility and layout of the win screen reward images
/// </summary>
function winGui::displayRewardImages(%this, %rewardCount)
{
    // Create and add the win screen reward images.

    // First, create our reward image container if it does not exist.
    %container = %this.rewardContainer;
    %widthScale = %this.Extent.x / 1024;
    %heightScale = %this.Extent.y / 768;
    %rewardWidth = getWord(%this.rewardImageSize, 0) * %widthScale;
    %rewardHeight = getWord(%this.rewardImageSize, 1) * %heightScale;
    if (!isObject(%container))
    {
        %container = new GuiControl() {
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
        %container.setPosition((getWord(winGui.Extent, 0) / 2) - (%rewardWidth * %this.rewardCount / 2), (180 * %heightScale));
        %container.setExtent((%rewardWidth * %this.rewardCount), %rewardHeight);
        %this.add(%container);
        %this.rewardContainer = %container;
    }

    // Next, create bitmap control instances as needed
    %rewardImageWidth = %this.rewardImageSize.x * %widthScale;
    %rewardImageHeight = %this.rewardImageSize.y * %heightScale;
    for (%i = 0; %i < %this.rewardCount; %i++)
    {
        %rewardImage = %container.rewardImageCtrl[%i];
        if (!isObject(%rewardImage))
        {
            %rewardImage = new GuiSpriteCtrl() {
                canSaveDynamicFields = "1";
                isContainer = "0";
                Profile = "GuiTransparentProfile";
                HorizSizing = "relative";
                VertSizing = "relative";
                MinExtent = "8 2";
                canSave = "1";
                Visible = "1";
                hovertime = "1000";
                Image = %this.noRewardImage;
                    index = %i;
            };
            %rewardImage.setPosition(0, (8 * %heightScale));
            %rewardImage.setExtent(%rewardImageWidth, %rewardImageHeight);
            %container.add(%rewardImage);
            %container.rewardImageCtrl[%i] = %rewardImage;
        }
    }

    // Finally, set the images on the bitmap controls to correctly indicate the stars 
    // earned in the completed level.
    %containerWidth = getWord(%container.Extent, 0);
    %startX = (%containerWidth / 2) - (((%this.rewardCount * %this.rewardImageSize.x) / 2) * %widthScale);
    %earnedStars = %rewardCount;
    for (%i = 0; %i < %this.rewardCount; %i++)
    {
        %rewardImage = %container.rewardImageCtrl[%i];
        %rewardImage.setPosition((%startX + (%i * %rewardWidth)), 0);
        %rewardImage.setImage(%i < %earnedStars ? %this.rewardImage : %this.noRewardImage);
        %rewardImage.setVisible(true);
    }
    
    if (%rewardCount)
        alxPlay(%this.rewardSound);
}

function winGui::getRewardCount(%this, %score)
{
    %earnedReward = 0;
    for (%i = 0; %i < $WorldListData.rewardCount; %i++)
    {
        if (%score >= MainScene.RewardScore[%i] && %score != 0)
            %earnedReward += 1;
    }
    return %earnedReward;
}

/// <summary>
/// This function restarts the schedule manager, reloads the current level and
/// then cleans up the win GUI.
/// </summary>
function winRestartButton::onClick(%this)
{
    winGui.clearRewardImages();
    if (isObject(MainScene))
        MainScene.delete();

    ScheduleManager.initialize();
    loadLevel("^PhysicsLauncherTemplate/data/levels/" @ $CurrentLevel @ ".scene.taml");
    
    if (isObject(GameMaster))
        GameMaster.callOnBehaviors(initializeLevel);

    HudGui.setup();
    helpGui.clear();
    helpGui.setup();
    Canvas.popDialog(winGui);
}

/// <summary>
/// This function takes us back to the level select screen.
/// </summary>
function winLevelSelectButton::onClick(%this)
{
    winGui.clearRewardImages();
    Canvas.pushDialog(levelSelectGui);
}

/// <summary>
/// This function checks for a next level in the current world.  If there is no
/// level remaining in the current world, increment to the next world and go to
/// the level select screen.
/// </summary>
function winNextLevelButton::onClick(%this)
{
    winGui.clearRewardImages();
    if (levelSelectGui.currentWorld.LevelList[$LevelIndex + 1] !$= "")
    {
        $LevelIndex++;
        $SelectedLevel = "^PhysicsLauncherTemplate/data/levels/" @ levelSelectGui.currentWorld.LevelList[$LevelIndex] @ ".scene.taml";
        loadLevel($SelectedLevel);
    }
    else
    {
        $SelectedWorld++;
        Canvas.pushDialog(levelSelectGui);
    }
        
    HudGui.setup();
    helpGui.clear();
    helpGui.setup();
    Canvas.popDialog(winGui);
}