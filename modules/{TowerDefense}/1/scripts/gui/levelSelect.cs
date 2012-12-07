//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

$GameLevelFileSpec = "*.t2d";
$levelLoading = false;

/// <summary>
/// This function initializes the GUI.
/// </summary>
function levelSelectGui::onWake(%this)
{
   %this.getLevelList();
   %this.currentLevelIndex = 0;
   %this.displayLevels(%this.currentLevelIndex);
   
   if (%this.levelCount < 8)
   {
      LevelSelectBackBtn.Visible = false;
      LevelSelectNextBtn.Visible = false;
   }
   else 
   {
      LevelSelectBackBtn.Visible = false;
      LevelSelectNextBtn.Visible = true;
   }
      
   //echo(" -- Level Select onWake : " @ %this.levelCount);
}

/// <summary>
/// This function accesses the list of levels and their button images attached to 
/// the levelSelectGui object.
/// </summary>
function levelSelectGui::getLevelList(%this)
{
    // get and clean the level file list from levelSelectGui
    %count = 0;
    while ( levelSelectGui.LevelList[%count] !$= "" )
    {
        // if the file exists, add it and its associated image to the clean list
        %file = "^TowerDefenseTemplate/data/levels/" @ %this.LevelList[%count] @ ".scene.taml";

        if (isFile(%file))
        {
            %levelList[%count] = %this.LevelList[%count];
            %levelImageList[%count] = %this.LevelImageList[%count];
        }
        else
        {
            // otherwise add a blank
            %levelList[%count] = "";
            %levelImageList[%count] = "";
        }
        %count++;
    }
    %k = 0;
    for (%i = 0; %i < %count; %i++)
    {
        // loop through and drop the blanks.
        if (%levelList[%i] !$= "")
        {
            %this.LevelList[%k] = %levelList[%i];
            %this.LevelImageList[%k] = %levelImageList[%i];
            %k++;
        }
    }
    %this.levelCount = %k;
}

/// <summary>
/// This function displays a page of available levels.  It paginates by using the
/// passed startIndex to find its place in the level list.
/// </summary>
/// <param name="startIndex">The index of the first level to display on this page.</param>
function levelSelectGui::displayLevels(%this, %startIndex)
{
   //echo(" -- Level Select Start Index : " @ %startIndex);
   for (%i = 1; %i < 9; %i++)
   {
      %button = "LevelSelectButton" @ %i;
      %button.Visible = false;
      %text = "LevelSelectLabel" @ %i;
      %text.Profile = "GuiTextWhite14Profile";
      %text.Visible = false;
   }
   
   %last = %startIndex + 8;
   if (%last > %this.levelCount)
      %last = %this.levelCount;
   %j = 1;
   for (%i = %startIndex; %i < %last; %i++)
   {
      %button = "LevelSelectButton" @ %j;
      %text = "LevelSelectLabel" @ %j;
      %button.index = %i;
      %text.index = %i;
      %text.text = %this.LevelList[%i];
      %text.Visible = true;
      %text.level = %this.LevelList[%i];
      %button.level = %this.LevelList[%i];
      %button.Image = %this.LevelImageList[%i];
      %button.Visible = true;
      %j++;
   }
}

/// <summary>
/// This function handles all level bitmapButton clicks.  It uses the index 
/// of the selected button to access the desired level file.
/// </summary>
function LevelSelectButton::onClick(%this)
{
   alxStopAll();   
   
   //echo("Setting level to " @ %level);
   $SelectedLevel = %this.level @ ".t2d";
   $LevelIndex = %this.index;

   if(!$levelLoading)
   {
      $levelLoading = true;
      
      $TowerBeingPlaced = "";
   
      moveMap.pop();
      
      schedule(100, 0, loadLevel);
   }
   Canvas.schedule(150, popDialog, levelSelectGui);
}

/// <summary>
/// This function handles clicks on the level labels in the same fashion as  
/// LevelSelectButton::onClick().
/// </summary>
function LevelLabelButton::onClick(%this)
{
   alxStopAll();
   
   //echo("Setting level to " @ %level);
   $SelectedLevel = %this.level @ ".t2d";
   $LevelIndex = %this.index;

   if(!$levelLoading)
   {
      $levelLoading = true;
      
      $TowerBeingPlaced = "";
   
      moveMap.pop();
      
      schedule(100, 0, loadLevel);
   }
   Canvas.schedule(150, popDialog, levelSelectGui);
}

/// <summary>
/// This function handles the next button and updates the back button as needed.
/// </summary>
function LevelSelectNextBtn::onClick(%this)
{
   levelSelectGui.currentLevelIndex += 8;
   if (levelSelectGui.currentLevelIndex > levelSelectGui.levelCount)
      levelSelectGui.currentLevelIndex = mFloor(levelSelectGui.levelCount / 8);
      
   levelSelectGui.displayLevels(levelSelectGui.currentLevelIndex);
   
   %page = levelSelectGui.currentLevelIndex / 8;
   //echo(" -- Level Select Page : " @ %page);
   if (%page < 1 && levelSelectGui.levelCount > levelSelectGui.currentLevelIndex)
   {
      LevelSelectBackBtn.Visible = false;
      LevelSelectNextBtn.Visible = true;
   }
   else if (%page > 0 && levelSelectGui.levelCount > levelSelectGui.currentLevelIndex + 8)
   {
      LevelSelectBackBtn.Visible = true;
      LevelSelectNextBtn.Visible = true;
   }
   else if (%page > 0 && levelSelectGui.levelCount < levelSelectGui.currentLevelIndex + 8)
   {
      LevelSelectBackBtn.Visible = true;
      LevelSelectNextBtn.Visible = false;
   }
}

/// <summary>
/// This function returns the user to the first page of levels.
/// </summary>
function LevelSelectHomeBtn::onClick(%this)
{
    goToMainMenu(levelSelectGui);
}

/// <summary>
/// This function handles the back button and updates the next button if needed.
/// </summary>
function LevelSelectBackBtn::onClick(%this)
{
   levelSelectGui.currentLevelIndex -= 8;
   if (levelSelectGui.currentLevelIndex < 0)
      levelSelectGui.currentLevelIndex = 0;
      
   levelSelectGui.displayLevels(levelSelectGui.currentLevelIndex);
   
   %page = levelSelectGui.currentLevelIndex / 8;
   //echo(" -- Level Select Page : " @ %page);
   if (%page < 1 && levelSelectGui.levelCount > levelSelectGui.currentLevelIndex)
   {
      LevelSelectBackBtn.Visible = false;
      LevelSelectNextBtn.Visible = true;
   }
   else if (%page > 0 && levelSelectGui.levelCount > levelSelectGui.currentLevelIndex + 8)
   {
      LevelSelectBackBtn.Visible = true;
      LevelSelectNextBtn.Visible = true;
   }
   else if (%page > 0 && levelSelectGui.levelCount < levelSelectGui.currentLevelIndex + 8)
   {
      LevelSelectBackBtn.Visible = true;
      LevelSelectNextBtn.Visible = false;
   }
}
