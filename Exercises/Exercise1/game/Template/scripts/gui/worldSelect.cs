//-----------------------------------------------------------------------------
// Three Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

$GameLevelFileSpec = "*.taml";
$levelLoading = false;

/// <summary>
/// This function initializes the GUI.
/// </summary>
function worldSelectGui::onWake(%this)
{
   %this.getWorldList();
   %this.currentWorldIndex = 0;
   %this.rewardCount = $WorldListData.rewardCount;
   %this.displayWorlds(%this.currentWorldIndex);
   
   if (%this.worldCount < 2)
   {
      WorldSelectBackBtn.Visible = false;
      WorldSelectNextBtn.Visible = false;
      WorldSelectButtonNext.Visible = false;
      WorldSelectButtonPrev.Visible = false;
   }
   else 
   {
      WorldSelectBackBtn.Visible = false;
      WorldSelectNextBtn.Visible = true;
      WorldSelectButtonNext.Visible = true;
      WorldSelectButtonPrev.Visible = false;
   }
   ScoreGroupContainer.setVisible(%this.showContainers);
}

/// <summary>
/// This function accesses the list of levels and their button images attached to 
/// the worldSelectGui object.
/// </summary>
function worldSelectGui::getWorldList(%this)
{
    if (!isObject($WorldListData) || $WorldDataChanged)
    {
        if (isFile($PhysicsLauncher::WorldListFile))
            $WorldListData = TamlRead($PhysicsLauncher::WorldListFile);

        else
        {
            $WorldListData = TamlRead("^PhysicsLauncherTemplate/managed/worldList.taml");
            createPath($PhysicsLauncher::WorldListFile);
            TamlWrite($WorldListData, $PhysicsLauncher::WorldListFile);
        }

        $WorldDataChanged = false;
    }
    // get and clean the level file list from worldSelectGui
    %count = 0;
    %worldCount = 0;
    while ( %count < $WorldListData.getCount() )
    {
        %world = $WorldListData.getObject(%count);
        if (%world.getInternalName() !$= "Unused Levels")
        {
            %this.currentWorldList[%worldCount] = %world;
            %worldCount++;
        }
        %count++;
    }
    %this.worldCount = %worldCount;
}

/// <summary>
/// This function displays one world at a time from a list of available worlds attached
/// to the worldSelectGui object.
/// </summary>
/// <param name="%index">The index of the world to display on this page.</param>
function worldSelectGui::displayWorlds(%this, %index)
{
    WorldSelectLabel.text = %this.currentWorldList[%index].getInternalName();
    WorldSelectButton.setNormalImage((%this.currentWorldList[%index].WorldLocked ? %this.currentWorldList[%index].WorldLockedImage : %this.currentWorldList[%index].WorldImage));
    WorldSelectButton.setHoverImage(WorldSelectButton.NormalImage);
    WorldSelectButton.setDownImage(WorldSelectButton.NormalImage);
    
    WorldSelectBackground.setImage(%this.currentWorldList[%index].WorldSelectBackground);
    
    WorldSelectLevelCount.text = %this.currentWorldList[%index].WorldProgress @ "/" @ %this.currentWorldList[%index].WorldLevelCount;
    WorldSelectLevelCount.Visible = !%this.currentWorldList[%index].WorldLocked;
    LevelLabel.Visible = !%this.currentWorldList[%index].WorldLocked;
    WorldScoreLabel.Visible = !%this.currentWorldList[%index].WorldLocked;
    StarDisplay.Visible = !%this.currentWorldList[%index].WorldLocked;
    WorldSelectScore.Visible = !%this.currentWorldList[%index].WorldLocked;
    WorldSelectScore.text = %this.getWorldScore(%index);
    WorldSelectStarCount.text = %this.getWorldStars(%index) @ "/" @ (%this.currentWorldList[%index].WorldLevelCount * %this.rewardCount);
    WorldSelectStarCount.Visible = !%this.currentWorldList[%index].WorldLocked;
    ScoreContainer1.setImage(%this.currentWorldList[%index].ContainerImage);
    ScoreContainer1.Visible = !%this.currentWorldList[%index].WorldLocked;
    ScoreContainer2.setImage(%this.currentWorldList[%index].ContainerImage);
    ScoreContainer2.Visible = !%this.currentWorldList[%index].WorldLocked;
    ScoreContainer3.setImage(%this.currentWorldList[%index].ContainerImage);
    ScoreContainer3.Visible = !%this.currentWorldList[%index].WorldLocked;
    
    if (%index > 0)
    {
        WorldSelectButtonPrev.setNormalImage((%this.currentWorldList[%index - 1].WorldLocked ? %this.currentWorldList[%index - 1].WorldLockedImage : %this.currentWorldList[%index - 1].WorldImage));
        WorldSelectButtonPrev.setHoverImage((%this.currentWorldList[%index - 1].WorldLocked ? %this.currentWorldList[%index - 1].WorldLockedImage : %this.currentWorldList[%index - 1].WorldImage));
        WorldSelectButtonPrev.setDownImage((%this.currentWorldList[%index - 1].WorldLocked ? %this.currentWorldList[%index - 1].WorldLockedImage : %this.currentWorldList[%index - 1].WorldImage));
    }
    
    if (%index < %this.worldCount - 1)
    {
        WorldSelectButtonNext.setNormalImage((%this.currentWorldList[%index + 1].WorldLocked ? %this.currentWorldList[%index + 1].WorldLockedImage : %this.currentWorldList[%index + 1].WorldImage));
        WorldSelectButtonNext.setHoverImage((%this.currentWorldList[%index + 1].WorldLocked ? %this.currentWorldList[%index + 1].WorldLockedImage : %this.currentWorldList[%index + 1].WorldImage));
        WorldSelectButtonNext.setDownImage((%this.currentWorldList[%index + 1].WorldLocked ? %this.currentWorldList[%index + 1].WorldLockedImage : %this.currentWorldList[%index + 1].WorldImage));
    }
}

/// <summary>
/// This function checks the total score for the selected world.
/// </summary>
/// <param name="%index">The index of the world to retrive the score from.</param>
/// <return>The total of all high scores for all levels in the selected world.</return>
function worldSelectGui::getWorldScore(%this, %index)
{
    %score = 0;
    for (%i = 0; %i < %this.currentWorldList[%index].WorldLevelCount; %i++)
    {
        %score += %this.currentWorldList[%index].LevelHighScore[%i];
    }
    return %score;
}

/// <summary>
/// This function checks the total "stars" for the selected world.
/// </summary>
/// <param name="%index">The index of the world to retrive the star count from.</param>
/// <return>The total of all stars earned for all levels in the selected world.</return>
function worldSelectGui::getWorldStars(%this, %index)
{
    %stars = 0;
    for (%i = 0; %i < %this.currentWorldList[%index].WorldLevelCount; %i++)
    {
        %stars += %this.currentWorldList[%index].LevelStars[%i];
    }
    return %stars;
}

/// <summary>
/// This function handles all world bitmapButton clicks.  It uses the index 
/// of the selected button to access the desired level file.
/// </summary>
function WorldSelectMouseEvt::onMouseDown(%this)
{
    if (!worldSelectGui.currentWorldList[worldSelectGui.currentWorldIndex].WorldLocked)
    {
        $SelectedWorld = worldSelectGui.currentWorldIndex;
        Canvas.pushDialog(levelSelectGui);
        Canvas.schedule(150, popDialog, worldSelectGui);
    }
}

/// <summary>
/// This function handles clicks on the level labels in the same fashion as  
/// LevelSelectButton::onClick().
/// </summary>
function WorldLabelButton::onClick(%this)
{
    $SelectedWorld = %this.currentWorldIndex;
    Canvas.pushDialog(levelSelectGui);
    Canvas.schedule(150, popDialog, worldSelectGui);
}

/// <summary>
/// This function handles the next button and updates the back button as needed.
/// </summary>
function WorldSelectNextBtn::onClick(%this)
{
   worldSelectGui.currentWorldIndex++;
      
   worldSelectGui.displayWorlds(worldSelectGui.currentWorldIndex);
   
   if (worldSelectGui.currentWorldIndex < 1 && worldSelectGui.worldCount > worldSelectGui.currentWorldIndex)
   {
      WorldSelectBackBtn.Visible = false;
      WorldSelectNextBtn.Visible = true;
      WorldSelectButtonPrev.Visible = false;
      WorldSelectButtonNext.Visible = true;
   }
   else if (worldSelectGui.currentWorldIndex > 0 && worldSelectGui.worldCount > worldSelectGui.currentWorldIndex + 1)
   {
      WorldSelectBackBtn.Visible = true;
      WorldSelectNextBtn.Visible = true;
      WorldSelectButtonPrev.Visible = true;
      WorldSelectButtonNext.Visible = true;
   }
   else if (worldSelectGui.currentWorldIndex > 0 && worldSelectGui.worldCount <= worldSelectGui.currentWorldIndex + 1)
   {
      WorldSelectBackBtn.Visible = true;
      WorldSelectNextBtn.Visible = false;
      WorldSelectButtonPrev.Visible = true;
      WorldSelectButtonNext.Visible = false;
   }
}

/// <summary>
/// This function returns the user to the first page of levels.
/// </summary>
function WorldSelectHomeBtn::onClick(%this)
{
    goToMainMenu(worldSelectGui);
}

/// <summary>
/// This function handles the back button and updates the next button if needed.
/// </summary>
function WorldSelectBackBtn::onClick(%this)
{
   worldSelectGui.currentWorldIndex--;
      
   worldSelectGui.displayWorlds(worldSelectGui.currentWorldIndex);
   
   if (worldSelectGui.currentWorldIndex < 1 && worldSelectGui.worldCount > worldSelectGui.currentWorldIndex)
   {
      WorldSelectBackBtn.Visible = false;
      WorldSelectNextBtn.Visible = true;
      WorldSelectButtonNext.Visible = true;
      WorldSelectButtonPrev.Visible = false;
   }
   else if (worldSelectGui.currentWorldIndex > 0 && worldSelectGui.worldCount > worldSelectGui.currentWorldIndex + 1)
   {
      WorldSelectBackBtn.Visible = true;
      WorldSelectNextBtn.Visible = true;
      WorldSelectButtonNext.Visible = true;
      WorldSelectButtonPrev.Visible = true;
   }
   else if (worldSelectGui.currentWorldIndex > 0 && worldSelectGui.worldCount < worldSelectGui.currentWorldIndex + 1)
   {
      WorldSelectBackBtn.Visible = true;
      WorldSelectNextBtn.Visible = false;
      WorldSelectButtonNext.Visible = false;
      WorldSelectButtonPrev.Visible = true;
   }
}
