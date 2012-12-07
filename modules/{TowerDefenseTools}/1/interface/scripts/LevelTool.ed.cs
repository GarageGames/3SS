//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------



//--------------------------------
// Level Tool Help
//--------------------------------
/// <summary>
/// This function opens a browser and navigates to the Tower Defense Template 
/// Level Tool help page.
/// </summary>
function LevelToolHelpButton::onClick(%this)
{
   gotoWebPage("http://docs.3stepstudio.com/towerdefense/level/");
}


//-----------------------------------------------------------------------------
// Level Globals
//-----------------------------------------------------------------------------

$StartingLivesMin = 1;
$StartingLivesMax = 100;

$StartingFundsMin = 0;
$StartingFundsMax = 10000;

$DefendObj = "";

$LevelSaveDirectory = "^project/data/levels/";

/// <summary>
/// This function handles the tool's dirty state to keep track of the need to save data.
/// </summary>
/// <param name="dirty">True if data has been changed, false if not.</param>
function SetLevelToolDirtyState(%dirty)
{
   $Tools::TemplateToolDirty = %dirty;
   
   if ($Tools::TemplateToolDirty)
      LevelToolWindow.setText("Level Tool *");
   else
      LevelToolWindow.setText("Level Tool");
}

//-----------------------------------------------------------------------------
// Level Tool
//-----------------------------------------------------------------------------
/// <summary>
/// Initializes the tool
/// </summary>
function LevelTool::onWake(%this)
{
   // load up our audio profiles for access from the tool.
   if (!$GameDir)
      $GameDir = LBProjectObj.gamePath;
   
   exec($GameDir @ "/scripts/audioDescriptions.cs");
   exec($GameDir @ "/scripts/audioProfiles.cs");
   %levelFile = LBProjectObj.currentLevelFile;
   LevelToolLevelNameEditBox.text = fileBase(%levelFile);
   %this.levelName = LevelToolLevelNameEditBox.getText();
   
   LevelToolStartingLivesEditBox.text = SceneBehaviorObject.callOnBehaviors("getLives");
   %this.startingLives = LevelToolStartingLivesEditBox.getValue();

   LevelToolCurrencyEditBox.text = SceneBehaviorObject.callOnBehaviors("getStartFunds");
   %this.startingFunds = LevelToolCurrencyEditBox.getValue();

   %this.musicFile = SceneBehaviorObject.callOnBehaviors("getSongProfile");
   LevelToolBGMEditBox.text = %this.musicFile;
   LevelToolBGMEditBox.setActive(false);
   LevelToolBackgroundImage.setBitmap(backgroundImage.getImageMap().imageFile);
   LevelToolBackgroundEditBox.text = backgroundImage.getImageMap();
   LevelToolBackgroundEditBox.setActive(false);
   
   LevelToolLivesIcon.setBitmap(livesIcon.getImageMap().imageFile);
   LevelToolCurrencyIcon.setBitmap(fundsIcon.getImageMap().imageFile);
   SetLevelToolDirtyState(false);
}

/// <summary>
/// This function stops audio playback on exiting the tool.
/// </summary>
function LevelTool::onSleep(%this)
{
   alxStop($LevelMusicStreamHandle);
}

/// <summary>
/// This function handles closing the tool.
/// </summary>
function LevelTool::close(%this)
{
    LevelToolLevelNameEditBox.onValidate();
    LevelToolCurrencyEditBox.onValidate();
    LevelToolStartingLivesEditBox.onValidate();
    if ($Tools::TemplateToolDirty)
    {
        ConfirmDialog.setupAndShow(LevelToolWindow.getGlobalCenter(), "Save Level Changes?", 
            "Save", "if (LevelTool.saveLevelInfo()) { SetLevelToolDirtyState(false); Canvas.popDialog(LevelTool);}", 
            "Don't Save", "SetLevelToolDirtyState(false); Canvas.popDialog(LevelTool);", 
            "Cancel", "");
        }
    else
        Canvas.popDialog(LevelTool);
}

/// <summary>
/// This function saves level specific game settings.
/// </summary>
function LevelTool::saveLevelInfo(%this)
{
    $OriginalLevelName = fileBase(LBProjectObj.currentLevelFile);
    // Rename the level file if the name has changed
    %levelName = LevelToolLevelNameEditBox.getText();

    if (%levelName !$= fileBase(LBProjectObj.currentLevelFile))
    {
        if (%this.checkValidLevelFileName(%levelName) == true)
        {
            %this.updateLevelSelect(%levelName);
            renameCurrentLevelFile(%levelName);
        }
        else
            return false;
    }
    
    LevelToolCurrencyEditBox.onValidate();
    LevelToolStartingLivesEditBox.onValidate();

    SceneBehaviorObject.callOnBehaviors(setLives, LevelToolStartingLivesEditBox.getText());
    SceneBehaviorObject.callOnBehaviors(setStartFunds, LevelToolCurrencyEditBox.getText());
    SceneBehaviorObject.callOnBehaviors(setSongProfile, LevelToolBGMEditBox.getText());
    backgroundImage.setImageMap(LevelToolBackgroundEditBox.getText());
    
    %this.startingLives = LevelToolStartingLivesEditBox.getValue();
    %this.startingFunds = LevelToolCurrencyEditBox.getValue();

    LBProjectObj.saveLevel();

    SetLevelToolDirtyState(false);
    return true;
}

/// <summary>
/// This function updates the level name stored in the levelSelectGui file if 
/// the current level's name is changed.
/// </summary>
/// <param name="fileName">The new name for the current level file.</param>
function LevelTool::updateLevelSelect(%this, %fileName)
{
    if (!isObject(levelSelectGui))
        exec($GameDir @ "/gui/levelSelect.gui");
    %count = 0;
    %name = levelSelectGui.LevelList[%count];
    while (%name !$= $OriginalLevelName && %name !$= "")
    {
        %count++;
        %name = levelSelectGui.LevelList[%count];
    }
    if (%name $= $OriginalLevelName)
    {
        levelSelectGui.LevelList[%count] = %fileName;
        GuiToolSaveGui(levelSelectGui);
    }
    $OriginalLevelName = %fileName;
    %this.levelName = %fileName;
}

/// <summary>
/// This function ensures that the desired level file name is valid.
/// </summary>
/// <param name="fileName">The file name we want to validate.</param>
/// <return>Returns true if the file name is valid, or false if not.</return>
function LevelTool::checkValidLevelFileName(%this, %fileName)
{
    // Check for empty filename 
    if (strreplace(%fileName, " ", "") $= "")  
    {
        // Show message dialog
        WarningDialog.setupAndShow(LevelToolWindow.getGlobalCenter(), "Notice!", "Level name cannot be empty!", 
         "", "", "", "", "Okay", "return false;");         

        
        return false;   
    }

    // Check for spaces
    if (strreplace(%fileName, " ", "") !$= %fileName)
    {
        // Show message dialog
        WarningDialog.setupAndShow(LevelToolWindow.getGlobalCenter(), "Notice!", "Level name cannot contain spaces!", 
         "", "", "", "", "Okay", "return false;");         
        
        return false;
    }
    
    // Check for invalid characters
    %invalidCharacters = "-+*/%$&§=()[].?\"#,;!~<>|°^{}";
    %strippedName = stripChars(%fileName, %invalidCharacters);
    if (%strippedName !$= %fileName)
    {
        // Show message dialog
        WarningDialog.setupAndShow(LevelToolWindow.getGlobalCenter(), "Notice!", "Level name contains invalid symbols!", 
         "", "", "", "", "Okay", "return false;"); 
         
        return false;   
    }
    
    return true;
}

/// <summary>
/// This function opens the Asset Library to select an audio profile for the background music.
/// </summary>
function LevelToolBGMSelectButton::onClick(%this)
{
   AssetLibrary.open(%this, $SoundsPage);
}

/// <summary>
/// This function assigns the asset selected in the Asset Library to the
/// level background music.
/// </summary>
/// <param name="asset">The asset returned from the Asset Library.</param>
function LevelToolBGMSelectButton::setSelectedAsset(%this, %asset)
{
   //echo(" -- BGMSelect : " @ %asset);
   LevelTool.musicFile = %asset;
   LevelToolBGMEditBox.text = %asset;
   SetLevelToolDirtyState(true);
}

/// <summary>
/// This function plays the currently selected background music.
/// </summary>
function LevelToolBGMPlayButton::onClick(%this)
{
   // temped in a sound file to see if this works
   // have to exec("scripts/game/audio.cs"); from the console in the editor.
   alxStopAll();
   $LevelMusicStreamHandle = alxPlay(LevelTool.musicFile);
   //echo(" -- BGMPlay : " @ LevelTool.musicFile @ " : " @ $LevelMusicStreamHandle);
}

/// <summary>
/// This function stops the currently selected background music.
/// </summary>
function LevelToolBGMStopButton::onClick(%this)
{
   alxStop($LevelMusicStreamHandle);
}

/// <summary>
/// This function opens the Asset Library to select an image for the level
/// background.
/// </summary>
function LevelToolBackgroundSelectButton::onClick(%this)
{
   AssetLibrary.open(%this, $SpriteSheetPage);
}

/// <summary>
/// This function assigns the asset selected in the Asset Library to the
/// level background.
/// </summary>
/// <param name="asset">The asset returned from the Asset Library.</param>
function LevelToolBackgroundSelectButton::setSelectedAsset(%this, %asset)
{
   //echo(" -- BackgroundSelect : " @ %asset);
   LevelToolBackgroundImage.setBitmap(%asset.imageFile);
   LevelToolBackgroundEditBox.text = %asset;
   SetLevelToolDirtyState(true);
}

/// <summary>
/// This function validates the text in the LevelToolLevelNameEditBox when the contents change.
/// </summary>
function LevelToolLevelNameEditBox::onChanged(%this)
{
    %this.onValidate();
}

/// <summary>
/// This function handles validation of the contents of the LevelToolLevelNameEditBox.
/// </summary>
function LevelToolLevelNameEditBox::onValidate(%this)
{
    if (%this.getText() !$= LevelTool.levelName)
    {
        SetLevelToolDirtyState(true);
    }
}

/// <summary>
/// This function increments the value in the LevelToolCurrencyEditBox.
/// </summary>
function LevelToolCurrencySpinUp::onClick(%this)
{
    LevelToolCurrencyEditBox.setValue(LevelToolCurrencyEditBox.getValue() + 1);
    LevelToolCurrencyEditBox.onValidate();
}

/// <summary>
/// This function decrements the value in the LevelToolCurrencyEditBox.
/// </summary>
function LevelToolCurrencySpinDown::onClick(%this)
{
    LevelToolCurrencyEditBox.setValue(LevelToolCurrencyEditBox.getValue() - 1);
    LevelToolCurrencyEditBox.onValidate();
}

/// <summary>
/// This function requests validation of the contents in the LevelToolCurrencyEditBox.
/// </summary>
function LevelToolCurrencyEditBox::onChanged(%this)
{
    %this.onValidate();
}

/// <summary>
/// This function validates the contents of the LevelToolCurrencyEditBox.
/// </summary>
function LevelToolCurrencyEditBox::onValidate(%this)
{
    if (%this.getValue() > $StartingFundsMax)
    {
        %this.setValue($StartingFundsMax);
    }
    else if (%this.getValue() < $StartingFundsMin)
    {
        %this.setValue($StartingFundsMin);
    }
    
    if (%this.getValue() != LevelTool.startingFunds)
    {
        LevelTool.startingFunds = %this.getValue();
        SetLevelToolDirtyState(true);
    }
}

/// <summary>
/// This function increments the value in the LevelToolStartingLivesEditBox.
/// </summary>
function LevelToolLivesSpinUp::onClick(%this)
{
    LevelToolStartingLivesEditBox.setValue(LevelToolStartingLivesEditBox.getValue() + 1);
    LevelToolStartingLivesEditBox.onValidate();
}

/// <summary>
/// This function decrements the value in the LevelToolStartingLivesEditBox.
/// </summary>
function LevelToolLivesSpinDown::onClick(%this)
{
    LevelToolStartingLivesEditBox.setValue(LevelToolStartingLivesEditBox.getValue() - 1);
    LevelToolStartingLivesEditBox.onValidate();
}

/// <summary>
/// This function requests validation of the contents in the LevelToolStartingLivesEditBox.
/// </summary>
function LevelToolStartingLivesEditBox::onChanged(%this)
{
    %this.onValidate();
}

/// <summary>
/// This function validates the contents of the LevelToolStartingLivesEditBox.
/// </summary>
function LevelToolStartingLivesEditBox::onValidate(%this)
{
    if (%this.getValue() > $StartingLivesMax)
    {
        %this.setValue($StartingLivesMax);
    }
    else if (%this.getValue() < $StartingLivesMin)
    {
        %this.setValue($StartingLivesMin);
    }
    
    if (%this.getValue() != LevelTool.startingLives)
    {
        LevelTool.startingLives = %this.getValue();
        SetLevelToolDirtyState(true);
    }
}

/// <summary>
/// This function clears the contents of the LevelToolLevelNameEditBox.
/// </summary>
function LevelToolClearLevelNameButton::onClick(%this)
{
   LevelToolLevelNameEditBox.text = "";
   SetLevelToolDirtyState(true);
}