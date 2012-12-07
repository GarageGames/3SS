//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------



//--------------------------------
// Wave Tool Help
//--------------------------------
/// <summary>
/// This function opens a browser and navigates to the Tower Defense Template 
/// Wave Tool help page.
/// </summary>
function WToolHelpButton::onClick(%this)
{
   gotoWebPage("http://docs.3stepstudio.com/towerdefense/wave/");
}


//-----------------------------------------------------------------------------
// Wave Tool Globals
//-----------------------------------------------------------------------------
$WaveTool::firstEmpty = false;
$WaveTool::waveCap = 99;

/// <summary>
/// This function handles the tool's dirty state to keep track of the need to save data.
/// </summary>
/// <param name="dirty">True if data has been changed, false if not.</param>
function SetWaveToolDirtyState(%dirty)
{
   $Tools::TemplateToolDirty = %dirty;
   
   if ($Tools::TemplateToolDirty)
      WaveToolWindow.setText("Wave Tool *");
   else
      WaveToolWindow.setText("Wave Tool");
}

/// <summary>
/// This function handles updating the wave data on wave selection.
/// </summary>
/// <param name="dropdown">The dropdown requesting the wave selection.</param>
function HandleWaveSelect(%dropdown)
{
    WaveManager.getWaveContents(%dropdown.getValue());

    WToolWaveNameEdit.text = WaveManager.curSelectedWave.internalName;

    WToolWaveStartSoundEdit.text = WaveManager.spawnSound;
    %manualStart = WaveManager.manualStart;
    if (%manualStart)
    {
        WToolTimedRButton.setValue(false);
        WToolManualRButton.setValue(%manualStart);
        WToolManualRButton.onClick();
    }
    else
    {
        WToolTimedRButton.setValue(true);
        WToolManualRButton.setValue(%manualStart);
        WToolTimedRButton.onClick();
    }
    WToolWaveDelaySpinner.text = WaveManager.startDelay;
    WaveManager.populateWaveDisplay(%dropdown.getText());
    WaveManager.lastSelected = %dropdown.getText();
    $SelectedWave = %dropdown.getText();
    $RefreshingWaveDisplay = false;
}

/// <summary>
/// This function handles confirmation of saving a wave if invalid enemies were detected on wave data load.
/// It will then save the cleaned wave data.
/// </summary>
function HandleInvalidEnemies()
{
    // force save after preview fill method cleans out invalid enemies.
    ConfirmDialog.setupAndShow(WaveToolWindow.getGlobalCenter(), "Invalid entries have been removed from the wave.  Save changes?", 
        "OK", "WaveTool.saveWaveset(); SetWaveToolDirtyState(false);", 
        "", "", 
        "", "");
        
    WaveManager.missingEnemies = false;
}

/// <summary>
/// This function handles adding a new empty wave to the wave set.
/// </summary>
function HandleAddWaveClick()
{
    if ($CreatingWave == true)
        return;

    $CreatingWave = true;

    // Create a new scene object
    %newWave = WaveManager.createNewWave();
    // Add the new object to the scene
    MainScene.addToScene(%newWave);
    WaveManager.addWave(%newWave);

    WaveManager.populateWaveDropdown(WToolSelectWaveDropdown);
    WaveManager.populateWaveDropdown(WToolCopyWaveSelectDropdown);

    %text = WToolSelectWaveDropdown.findText(%newWave.internalName);
    WToolSelectWaveDropdown.setSelected(%text);
    
    if (!WToolRemoveWaveButton.isActive())
        WToolRemoveWaveButton.setActive(true);

    SetWaveToolDirtyState(true);    
    EnableControls();
    WaveTool.refreshDisplay();
    $CreatingWave = false;
}

/// <summary>
/// This function is called to disable all relevent controls when wave data is unavailable.
/// </summary>
function DisableControls()
{
    WToolSelectWaveDropdown.setActive(false);
    WToolWaveNameEdit.setActive(false);
    WToolWaveNameEdit.Profile = GuiTextEditInactiveProfile;
    WToolWaveNameClearBtn.setActive(false);
    WToolCopyWaveSelectDropdown.setActive(false);
    WToolEnemyPreviewDropdown.setActive(false);
    WToolEnemyDelaySpinDown.setActive(false);
    WToolEnemyDelaySpinner.setActive(false);
    WToolEnemyDelaySpinner.Profile = GuiTextEditInactiveProfile;
    WToolEnemyDelaySpinUp.setActive(false);
    WToolEnemyPathDropdown.setActive(false);
    WToolTimedRButton.setActive(false);
    WToolManualRButton.setActive(false);
    WToolWaveDelaySpinDown.setActive(false);
    WToolWaveDelaySpinner.setActive(false);
    WToolWaveDelaySpinner.Profile = GuiTextEditInactiveProfile;
    WToolWaveDelaySpinUp.setActive(false);
    WToolWaveStartSoundEdit.setActive(false);
    WToolWaveStartSoundEdit.Profile = GuiTextEditInactiveProfile;
    WToolWaveSoundSelectButton.setActive(false);
}

/// <summary>
/// This function is called to enable all relevant controls when wave data becomes available.
/// </summary>
function EnableControls()
{
    WToolSelectWaveDropdown.setActive(true);
    WToolWaveNameEdit.setActive(true);
    WToolWaveNameEdit.Profile = GuiTextEditProfile;
    WToolWaveNameClearBtn.setActive(true);
    WToolCopyWaveSelectDropdown.setActive(true);
    WToolEnemyPreviewDropdown.setActive(true);
    WToolEnemyDelaySpinDown.setActive(true);
    WToolEnemyDelaySpinner.setActive(true);
    WToolEnemyDelaySpinner.Profile = GuiTextEditProfile;
    WToolEnemyDelaySpinUp.setActive(true);
    WToolEnemyPathDropdown.setActive(true);
    WToolTimedRButton.setActive(true);
    WToolManualRButton.setActive(true);
    WToolWaveDelaySpinDown.setActive(true);
    WToolWaveDelaySpinner.setActive(true);
    WToolWaveDelaySpinner.Profile = GuiTextEditProfile;
    WToolWaveDelaySpinUp.setActive(true);
    WToolWaveStartSoundEdit.setActive(true);
    WToolWaveStartSoundEdit.Profile = GuiTextEditProfile;
    WToolWaveSoundSelectButton.setActive(true);
}
//-----------------------------------------------------------------------------
// Wave Tool
//-----------------------------------------------------------------------------

/// <summary>
/// This function handles basic initialization for the tool.
/// </summary>
function WaveTool::onWake(%this)
{
    $WaveTool::firstEmpty = false;
    $RefreshingWaveDisplay = true;

    WaveManager.initialize();

    WaveManager.populateWaveDropdown(WToolCopyWaveSelectDropdown);
    WaveManager.populateWaveDropdown(WToolSelectWaveDropdown);
    WaveManager.populatePathDropdown(WToolEnemyPathDropdown);

    WToolWaveStartSoundEdit.setActive(false);
    WToolWaveStartSoundEdit.Profile = "GuiTextEditInactiveProfile";
    WToolWaveMaxCountEdit.setActive(false);
    WToolWaveCurrentCountEdit.setActive(false);

    WToolSelectWaveDropdown.setFirstSelected();
    WToolCopyWaveSelectDropdown.setFirstSelected();
    WToolEnemyPreviewDropdown.setFirstSelected();
    WToolEnemyPathDropdown.setFirstSelected();
    
    WToolWaveNameEdit.text = WToolSelectWaveDropdown.getText();

    %this.setTimedVisibility();
    $RefreshingWaveDisplay = false;
    SetWaveToolDirtyState(false);
}

/// <summary>
/// This function handles some basic cleanup when the tool sleeps.
/// </summary>
function WaveTool::onSleep(%this)
{
   WToolWaveContentsPane.clear();
   $SelectedWave = "";
}

/// <summary>
/// This function refreshes the current wave display from the currently selected wave's cached data.
/// </summary>
function WaveTool::refreshDisplay(%this)
{
    WaveManager.populateEnemyList(WToolEnemyPreviewDropdown);
    WaveManager.populateWaveDropdown(WToolCopyWaveSelectDropdown);
    WaveManager.populateWaveDropdown(WToolSelectWaveDropdown);
    WaveManager.populatePathDropdown(WToolEnemyPathDropdown);
    WaveManager.populateWaveDisplay(WToolSelectWaveDropdown.getSelected());

    WToolWaveStartSoundEdit.text = WaveManager.spawnSound;
    %manualStart = WaveManager.manualStart;
    if (%manualStart)
    {
        WToolTimedRButton.setValue(false);
        WToolManualRButton.setValue(%manualStart);
        WToolManualRButton.onClick();
    }
    else
    {
        WToolTimedRButton.setValue(true);
        WToolManualRButton.setValue(%manualStart);
        WToolTimedRButton.onClick();
    }
    WToolWaveDelaySpinner.text = WaveManager.startDelay;
    WToolWaveNameEdit.text = WToolSelectWaveDropdown.getText();

    WToolWaveStartSoundEdit.setActive(false);

    %this.setTimedVisibility();
}

/// <summary>
/// This function confirms saving any changes before changing the selected wave.
/// </summary>
function WaveTool::confirmChanges(%this)
{
   if ($Tools::TemplateToolDirty)
   {
      ConfirmDialog.setupAndShow(WaveToolWindow.getGlobalCenter(), "Save Wave Changes?", 
            "Save", "WaveTool.saveWaveset(); SetWaveToolDirtyState(false);", 
            "Don't Save", "WaveManager.revert(); HandleWaveSelect(WToolSelectWaveDropdown); SetWaveToolDirtyState(false);", 
            "Cancel", "");// need to reset dropdown selection if canceled.
   }
}

/// <summary>
/// This function sets the Wave Tool's wave delay controls based on the selection of the
/// timed or manual radio button set.
/// </summary>
function WaveTool::setTimedVisibility(%this)
{
    WToolWaveDelaySpinner.setVisible(WToolTimedRButton.getValue());
    WToolWaveDelaySpinDown.setVisible(WToolTimedRButton.getValue());
    WToolWaveDelaySpinUp.setVisible(WToolTimedRButton.getValue());
    WToolDelayText.setVisible(WToolTimedRButton.getValue());
}

/// <summary>
/// This function validates all of the Wave Tool's text fields.
/// </summary>
function WaveTool::validateTextFields(%this)
{
    WToolWaveDelaySpinner.onValidate();
    WToolWaveNameEdit.onValidate();
    
    for (%i = 0; %i < 20; %i++)
    {
        if (isObject(%this.delayEdit[%i]))
            %this.delayEdit[%i].onValidate();
    }
}

/// <summary>
/// This function presents a dialog to inform the user that the maximum number of waves has been reached.
/// </summary>
function WaveTool::warnWaveMax(%this)
{
    WarningDialog.setupAndShow(WaveToolWindow.getGlobalCenter(), "Notice!", "You can not create more than " @ $WaveTool::waveCap @ " waves in a single level.", 
        "", "", 
        "OK", "", 
        "", "");
}

/// <summary>
/// This function handles closing the tool, prompting the user to save changes if needed.
/// </summary>
function WaveTool::close(%this)
{
    %this.validateTextFields();
    if ($Tools::TemplateToolDirty)
    {
        ConfirmDialog.setupAndShow(WaveToolWindow.getGlobalCenter(), "Save Wave Changes?", 
            "Save", "if (WaveTool.saveWaveset()) {SetWaveToolDirtyState(false); Canvas.popDialog(WaveTool);}", 
            "Don't Save", "WaveManager.revert(); SetWaveToolDirtyState(false); Canvas.popDialog(WaveTool);", 
            "Cancel", "");
    }
    else
        Canvas.popDialog(WaveTool);
}

/// <summary>
/// This function cycles through all waves in the current level and revalidates them, 
/// ensures that they are all added to the level's Wave Controller and saves all of them.
/// </summary>
function WaveTool::saveWaveset(%this)
{
    if (WaveManager.WaveSet.getCount() < 1 && !%this.removingWave)
        return true;
    
    %name = WToolWaveNameEdit.getText();

    // Check for Duplicate Internal Names
    %scene = ToolManager.getLastWindow().getScene();

    %sceneObjectCount = %scene.getSceneObjectCount();

    for (%i = 0; %i < %sceneObjectCount; %i++)
    {
        %sceneObject = %scene.getSceneObject(%i);

        if (%sceneObject == WaveManager.curSelectedWave)
            continue;

        if (%sceneObject.getInternalName() $= %name)
        {
            WarningDialog.setupAndShow(WaveToolWindow.getGlobalCenter(), "Duplicate Name", "Another Object in your Game Already has this Name", "", "", "OK");      

            return false;
        }
    }

    if (%name $= "")
    {
        $RefreshingWaveDisplay = true;

        WarningDialog.setupAndShow(WaveToolWindow.getGlobalCenter(), "Invalid Name",
            "You cannot Save a Wave without a Name", 
            "", "", 
            "OK", "WaveTool.refreshDisplay(); SetWaveToolDirtyState(false);", 
            "", "");

        $RefreshingWaveDisplay = false;
        return false;
    }
    
    if (isObject(WaveManager.lastAdded))
        WaveManager.lastAdded = "";
    
    if (isObject(WaveManager.lastRemoved))
        WaveManager.lastRemoved.delete();

    WaveManager.saveWave();
    WaveManager.saveWaveset();
    LBProjectObj.saveLevel();
    %this.refreshDisplay();
    return true;
}

/// <summary>
/// This function presents a dialog to confirm or cancel the deletion of a wave.
/// </summary>
function WaveTool::confirmRemoveWave(%this)
{
    ConfirmDialog.setupAndShow(WaveToolWindow.getGlobalCenter(), "Remove Selected Wave?", 
        "Remove", "WaveTool.removeWave(); WaveTool.saveWaveset(); WaveTool.removingWave = false; HandleWaveSelect(WToolSelectWaveDropdown); SetWaveToolDirtyState(false);", 
        "", "", 
        "Cancel", "");// need to reset dropdown selection if canceled.
}

/// <summary>
/// This function presents a dialog to confirm or cancel the creation of a new wave.
/// </summary>
function WaveTool::confirmAddWave(%this)
{
    ConfirmDialog.setupAndShow(WaveToolWindow.getGlobalCenter(), "Add a New Wave?", 
        "Add", "schedule(100, 0, HandleAddWaveClick);", 
        "", "", 
        "Cancel", "");// need to reset dropdown selection if canceled.
}

/// <summary>
/// This function deletes the currently selected wave from the wave set and the level.  If this is the
/// last wave in the level, it also disables the remove button.
/// </summary>
function WaveTool::removeWave(%this)
{
    // Remove the current wave.
    %remove = WToolSelectWaveDropdown.getValue();
    %wave = WaveManager.WaveSet.findObjectByInternalName(%remove);
    %this.removingWave = true;
    WaveManager.removeWave(%wave);
    if (WaveManager.WaveSet.getCount() < 1)
        DisableControls();
}

/// <summary>
/// This function handles validation of the WToolWaveDelaySpinner.  This displays the 
/// initial wave start time for the selected wave.
/// </summary>
function WToolWaveDelaySpinner::onValidate(%this)
{
    if (%this.getValue() != WaveManager.startDelay)
        SetWaveToolDirtyState(true);
}

/// <summary>
/// This function validates the WToolWaveNameEdit to ensure that the selected name is valid.
/// </summary>
function WToolWaveNameEdit::onValidate(%this)
{
    %text = %this.getText();
    if (%text $= "")
    {
        SetWaveToolDirtyState(true);
    }
    if (%text !$= WToolSelectWaveDropdown.getText())
    {
        SetWaveToolDirtyState(true);
    }
}

/// <summary>
/// This function handles selection of a wave from the list of available waves.  If there are
/// changes to the current wave it prompts the user to save them.
/// </summary>
function WToolSelectWaveDropdown::onSelect(%this)
{
   //echo(" -- WToolSelectWaveDropdown::onSelect()");
   $RefreshingWaveDisplay = true;
   if ($Tools::TemplateToolDirty && !ConfirmDialog.isAwake() && ($SelectedWave !$= WToolSelectWaveDropdown.getText()))
   {
      // prompt before saving
      //echo(" -- $Tools::TemplateToolDirty - saving");
      WaveTool.confirmChanges();
   }
   else if ($SelectedWave !$= WToolSelectWaveDropdown.getText() && !$Tools::TemplateToolDirty)
      HandleWaveSelect(%this);
}

/// <summary>
/// This function requests a new wave if we still have not reached the current wave limit.
/// It also confirms saving any changes to the current wave before proceeding.
/// </summary>
function WToolAddWaveButton::onClick(%this)
{
    if (WaveManager.WaveSet.getCount() >= $WaveTool::waveCap)
    {
        WaveTool.warnWaveMax();
        return;
    }
    if ($Tools::TemplateToolDirty)
    {
        WaveTool.confirmChanges();
        return;
    }
    else
        WaveTool.confirmAddWave();
}

/// <summary>
/// This function requests the removal of the currently selected wave.
/// </summary>
function WToolRemoveWaveButton::onClick(%this)
{
    WaveTool.confirmRemoveWave();
}

/// <summary>
/// This function selects the wave to copy from when the WToolCopyWaveButton is clicked.
/// </summary>
function WToolCopyWaveSelectDropdown::onSelect(%this)
{
   //echo(" -- WToolCopyWaveSelectDropdown::onSelect()");
}

/// <summary>
/// This function requests a copy of the wave selected in the WToolCopyWaveSelectDropdown to
/// the currently selected wave.
/// </summary>
function WToolCopyWaveButton::onClick(%this)
{
    //echo(" -- WToolCopyWaveButton::onClick()");
    %this.sourceWave = WToolCopyWaveSelectDropdown.getText();
    %destWave = WToolSelectWaveDropdown.getText();
    
    if (%sourceWave $= %destWave)
    {
        WarningDialog.setupAndShow(WaveToolWindow.getGlobalCenter(),
            "Notice!", 
            "Target and destination waves are the same.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return;
    }

    WaveTool.validateTextFields();
    
    if ($Tools::TemplateToolDirty)
        WaveTool.confirmChanges();
    
    %this.handleCopy();
}

/// <summary>
/// This function handles copying the contents of the wave selected in the WToolCopyWaveSelectDropdown
/// to the currenly selected wave.
/// </summary>
function WToolCopyWaveButton::handleCopy(%this)
{
   // Copy wave selected in the Copy from Wave dropdown to the currently selected
   // wave.
    //echo(" -- WToolCopyWaveButton::handleCopy()");
   WaveManager.copyWave(%this.sourceWave);
   WaveTool.refreshDisplay();
   SetWaveToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an audio profile for the selected
/// wave's spawn sound.
/// </summary>
function WToolWaveSoundSelectButton::onClick(%this)
{
   //echo(" -- WToolWaveSoundSelectButton::onClick()");
   AssetLibrary.open(%this, $SoundsPage);
}

/// <summary>
/// This function assigns the selected audio profile to the selected wave's spawn sound.
/// </summary>
/// <param name="asset">The name of the selected audio profile.</param>
function WToolWaveSoundSelectButton::setSelectedAsset(%this, %asset)
{
   WaveManager.waveSound = %asset;
   WToolWaveStartSoundEdit.text = %asset;
   SetWaveToolDirtyState(true);
}

/// <summary>
/// This function decrements the selected wave's delay time.
/// </summary>
function WToolWaveDelaySpinDown::onClick(%this)
{
   SetWaveToolDirtyState(true);

   if (!WToolTimedRButton.getValue())
      return;
   
   %value = WToolWaveDelaySpinner.getText();
   %value--;
   if (%value < 0)
      %value = 0;
   WToolWaveDelaySpinner.text = %value;
}

/// <summary>
/// This function increments the selected wave's delay time.
/// </summary>
function WToolWaveDelaySpinUp::onClick(%this)
{
   SetWaveToolDirtyState(true);

   if (!WToolTimedRButton.getValue())
      return;
   
   %value = WToolWaveDelaySpinner.getText();
   %value++;
   if (%value > 600)
      %value = 600;
   WToolWaveDelaySpinner.text = %value;
}

/// <summary>
/// This function updates the wave's delay time when the wave delay value changes.
/// </summary>
function WToolWaveDelaySpinner::onChanged(%this)
{
    WaveManager.startDelay = %this.getValue();
}

/// <summary>
/// This function decrements the stamp enemy's individual spawn delay.
/// </summary>
function WToolEnemyDelaySpinDown::onClick(%this)
{
   SetWaveToolDirtyState(true);

   %value = WToolEnemyDelaySpinner.getText();
   %value--;
   if (%value < 0)
      %value = 0;
   WToolEnemyDelaySpinner.text = %value;
}

/// <summary>
/// This function increments the stamp enemy's individial spawn delay.
/// </summary>
function WToolEnemyDelaySpinUp::onClick(%this)
{
   SetWaveToolDirtyState(true);

   %value = WToolEnemyDelaySpinner.getText();
   %value++;
   if (%value > 600)
      %value = 600;
   WToolEnemyDelaySpinner.text = %value;
}

/// <summary>
/// This function sets the wave to start on a timer as specified in the WToolWaveDelaySpinner control.
/// </summary>
function WToolTimedRButton::onClick(%this)
{
    if (!$RefreshingWaveDisplay)
        SetWaveToolDirtyState(true);

    if (%this.value == true)
        WToolWaveDelaySpinner.text = "manual";
    else
    {
        %value = WToolWaveDelaySpinner.getText();
        if (%value < 0)
            %value = 0;
        WToolWaveDelaySpinner.text = WaveManager.startDelay;
    }
    WaveManager.manualStart = WToolManualRButton.getValue();
    WToolWaveDelaySpinner.text = WaveManager.startDelay;
    if (WToolWaveDelaySpinner.getText() $= "")
        WToolWaveDelaySpinner.text = "0";
        
    WaveTool.setTimedVisibility();
}

/// <summary>
/// This function sets the wave to start only when the Start Wave button is clicked.
/// </summary>
function WToolManualRButton::onClick(%this)
{
    if (!$RefreshingWaveDisplay)
        SetWaveToolDirtyState(true);

    if (%this.value == false)
        WToolWaveDelaySpinner.text = "manual";
    else
    {
        %value = WToolWaveDelaySpinner.getText();
        if (%value < 0)
            %value = 0;
        WToolWaveDelaySpinner.text = WaveManager.startDelay;
        if (WToolWaveDelaySpinner.getText() $= "")
            WToolWaveDelaySpinner.text = "0";
    }
    WaveManager.manualStart = %this.getValue();
    WaveTool.setTimedVisibility();
}

/// <summary>
/// This function selects the stamp enemy to add to the current wave.
/// </summary>
function WToolEnemyPreviewDropdown::onSelect(%this)
{
    %enemy = $persistentObjectSet.findObjectByInternalName(WToolEnemyPreviewDropdown.getValue(), true);
    if (%enemy.getClassName() $= "t2dAnimatedSprite")
    {
        WToolEnemyPreview.display(%enemy.animationSet.MoveSouthAnim, "t2dAnimatedSprite");

        if (%enemy.animationSet.MoveNorthMirror)
            WToolEnemyPreview.sprite.setFlipY();
    }
    else
        WToolEnemyPreview.display(%enemy.getImageMap(), "t2dStaticSprite");

    WToolNameLabel.text = %enemy.getInternalName();
    WToolHealthLabel.text = %enemy.callOnBehaviors("getHealth");
    WToolSpeedLabel.text = %enemy.callOnBehaviors("getSpeed");
    WToolDamageLabel.text = %enemy.callOnBehaviors("getAttack");
    WToolScoreLabel.text = %enemy.callOnBehaviors("getScoreValue");
    WToolFundsLabel.text = %enemy.callOnBehaviors("getFundsValue");
    WToolEnemyDelaySpinner.text = "1";

    WaveManager.populatePathDropdown(WToolEnemyPathDropdown);
    WToolEnemyPathDropdown.setFirstSelected();
}

/// <summary>
/// This function is a placeholder for the WToolEnemyPathDropdown::onSelect() callback.
/// </summary>
function WToolEnemyPathDropdown::onSelect(%this)
{
}

//-----------------------------------------------------------------------------
// Wave Management
//-----------------------------------------------------------------------------
new ScriptObject(WaveManager);
new ScriptObject(HoldWave);

/// <remarks>
/// The wave manager handles all wave information for the current level.  It adds and 
/// removes waves from the wave controller, it adds and removes enemies and enemy groups
/// from the individual waves and it handles copy operations.  It also keeps track of 
/// available enemies and paths in the current level.
/// </remarks>

/// <summary>
/// This function initializes the wave manager.  It finds the wave controller, gets a list of 
/// waves from the controller, gets lists of available enemies and paths and sets up
/// internal lists based on the data.  It then handles the control states based on wave count.
/// </summary>
function WaveManager::initialize(%this)
{
   if (!isObject(WaveSet) )
   {
      %this.WaveSet = new SimSet(WaveSet);
   }
   %this.WaveSet.clear();
   
   if (!isObject(WaveEnemySet) )
   {
      %this.WaveEnemySet = new SimSet(WaveEnemySet);
   }
   %this.WaveEnemySet.clear();
   
   if (!isObject(PathSet) )
   {
       %this.PathSet = new SimSet(PathSet);
   }
   %this.PathSet.clear();
   
   if (!isObject(GroupSet))
   {
       %this.GroupSet = new SimSet(GroupSet);
   }
   %this.GroupSet.clear();
   
   %this.PathSet.add(EnemyPathGrid01);
   %this.PathSet.add(EnemyPathGrid02);
   %this.PathSet.add(EnemyPathGrid03);
   %this.PathSet.add(EnemyPathGrid04);
       
   %this.Paths[0] = EnemyPathGrid01;
   %this.Paths[1] = EnemyPathGrid02;
   %this.Paths[2] = EnemyPathGrid03;
   %this.Paths[3] = EnemyPathGrid04;
   
   for (%c = 0; %c < MainScene.getSceneObjectCount(); %c++)
   {
      %object = MainScene.getSceneObject(%c);
      %behavior = %object.getBehavior("WaveControllerBehavior");
      if (isObject(%behavior))
      {
         %this.waveController = %object;
         //echo(" -- found wave controller " @ %object.getName());
      }
   }

   %this.manualStart = WToolManualRButton.getValue();
   
   %this.refreshWaveList();

   %this.populateEnemyList(WToolEnemyPreviewDropdown);

    if (%this.WaveSet.getCount() < 1)
    {
        WToolRemoveWaveButton.setActive(false);
        DisableControls();
    }
    else
    {
        WToolRemoveWaveButton.setActive(true);
        EnableControls();
    }
}

/// <summary>
/// This function cleans the current wave's group set.
/// </summary>
function WaveManager::cleanSets(%this)
{
    %this.GroupSet.clear();
}

/// <summary>
/// This function adds the selected wave to the current wave set.
/// </summary>
/// <param name="wave">The wave to add to the wave set.</param>
function WaveManager::addWave(%this, %wave)
{
   %this.WaveSet.add(%wave);
   %this.WaveList[%this.WaveListSize] = %wave;
   %this.WaveListSize = %this.WaveSet.getCount();
   %this.lastAdded = %wave;
}

/// <summary>
/// This function tells the wave controller to write the desired wave to the indicated slot.
/// </summary>
/// <param name="wave">The wave we need to save to the list.</param>
/// <param name="index">The spot in the wave list to insert the wave.</param>
function WaveManager::writeWave(%this, %wave, %index)
{
   %this.waveController.callOnBehaviors("writeWave", %wave, %index);
}

/// <summary>
/// This function adds the wave to the end of the wave controller's list.
/// </summary>
/// <param name="wave">The wave to add to the list.</param>
function WaveManager::commitWave(%this, %wave)
{
   %this.waveController.callOnBehaviors("addWave", %wave);
}

/// <summary>
/// This function saves the entire wave set to the wave controller.
/// </summary>
function WaveManager::saveWaveset(%this)
{
   for (%i = 0; %i < %this.WaveSet.getCount(); %i++)
   {
      %this.writeWave(%this.WaveSet.getObject(%i).getName(), %i);
   }
}

/// <summary>
/// This adds a path to the list of available paths in the level.  It won't add a path 
/// if it doesn't have a start point, since we can't spawn enemies on it.
/// </summary>
/// <param name="pathName">The name of the path to add to the list.</param>
function WaveManager::addPath(%this, %pathName)
{
    for (%i = 0; %i < 4; %i++)
    {
        if (%this.Paths[%i] $= "")
        {
            if (%pathName.getGridStartPoint() !$= "")
            {
                %this.Paths[%i] = %pathName;
                %this.PathSet.add(%pathName);
                return true;
            }
        }
    }
    // path list is full
    return false;
}

/// <summary>
/// This function clears the stored path list.
/// </summary>
function WaveManager::clearPaths(%this)
{
    for (%i = 0; %i < 4; %i++)
        %this.Paths[%i] = "";
    
    %this.PathSet.clear();
}

/// <summary>
/// This function sets the selected path to the selected slot in the path list.
/// </summary>
/// <param name="pathName">The name of the path to insert in the list.</param>
/// <param name="index">The index to insert the path to.</param>
function WaveManager::setPath(%this, %pathName, %index)
{
   if (%index >= 0 && %index < 4)
   {
      %this.Paths[%index] = %pathName;
      return true;
   }
   // index out of bounds
   return false;
}

/// <summary>
/// This function adds the contents of the current path list to the desired dropdown control.
/// It then ensures that the previously selected path is set back to the currently selected path.
/// </summary>
/// <param name="dropdown">The dropdown to add the path list to.</param>
function WaveManager::populatePathDropdown(%this, %dropdown)
{
    %selected = %dropdown.getSelected();
    
    %dropdown.clear();
    %j = 0;
    for (%i = 0; %i < 4; %i++)
    {
        %path = %this.Paths[%i];
        if (isObject(%path))
        {
            if (getGridStartPoint(%path) !$= "")
            {
                %dropdown.add(%path.getInternalName(), %j);
                %j++;
            }
        }
    }
    %dropdown.setSelected((%selected > 0 ? %selected : 0));
}

/// <summary>
/// This function removes an enemy from the set of available enemies.
/// </summary>
/// <param name="index">The slot containing the enemy to remove.</param>
function WaveManager::removeEnemy(%this, %index)
{
   %temp = %this.GroupSet.getObject(%index);
   %this.GroupSet.remove(%temp);
   //%this.groupCount = %this.GroupSet.getCount();
}

/// <summary>
/// This function populates the selected dropdown with a full list of the currently available waves.
/// </summary>
/// <param name="dropDown">The dropdown to populate with the current wave list.</param>
function WaveManager::populateWaveDropdown(%this, %dropDown)
{
    //echo(" -- WaveManager::populateWaveDropdown(" @ %dropDown @ ")");
    %selected = %dropDown.getSelected();
    if (%selected $= "")
        %selected = 0;
        
    %dropDown.clear();
    for (%c = 0; %c < %this.WaveSet.getCount(); %c++)
    {
        %dropDown.add(%this.WaveSet.getObject(%c).internalName, %c);
    }

    %dropDown.setSelected((%selected > 0 && %selected < %dropDown.size() ? %selected : 0));
}

/// <summary>
/// This function searches for all waves in the current level and requests a prefetch, which
/// checks to ensure that any enemies that have been deleted are removed from all existing
/// waves.
/// </summary>
function WaveManager::refreshWaveList(%this)
{
    %count = 0;
    %this.WaveSet.clear();
    for (%c = 0; %c < MainScene.getSceneObjectCount(); %c++)
    {
        %object = MainScene.getSceneObject(%c);
        %behavior = %object.getBehavior("WaveBehavior");
        if (isObject(%behavior))
        {
            %this.WaveList[%count] = %object;
            %this.WaveSet.add(%this.WaveList[%count]);
            %count++;
            //echo(" -- found wave " @ %object.internalName);
        }
    }
    %this.WaveListSize = %count;
    %this.prefetchWaves();
}

/// <summary>
/// This function cycles through all waves in the level and ensures that all 
/// enemies in the waves actually exist in the game.  If any of them have been deleted,
/// it cleans the wave enemy lists and updates them.
/// </summary>
function WaveManager::prefetchWaves(%this)
{
    //echo(" -- entering WaveManager::prefetchWaves()...");
    %waveContents[0] = "";
    %cleanList[0] = "";
    %updateFiles = false;
    for(%i = 0; %i < %this.WaveListSize; %i++)
    {
        %tempWave = %this.WaveList[%i];
        %count = 0;
        %groupCount = %tempWave.callOnBehaviors("getGroupCount");
        //echo(" -- prefetching " @ %tempWave.getInternalName() @ "; checking " @ %groupCount @ " groups in wave");
        if (%groupCount $= "ERR_CALL_NOT_HANDLED")
            continue;
        if (%groupCount < 1 || %groupCount > 100)
            continue;
        for (%j = 0; %j < %groupCount; %j++)
        {
            // For each wave, clean the arrays
            %waveContents[%j] = "";
            %cleanList[%j] = "";
        }
        // No blank entries found, no need to update for each wave.
        %blankFound = false;
        %needUpdate = false;
        for (%a = 0; %a < %groupCount; %a++)
        {
            // Get the enemy for each group in the wave
            %group = %tempWave.callOnBehaviors("getEnemyGroup", %a);
            // See if the enemy is in the persistent object set
            %found = $persistentObjectSet.findObjectByInternalName(%group.enemy.internalName);
            if (isObject(%found))
            {
                // Our enemy is still in the persistent set, so it's still in the game.
                // If a blank has already been found, then we have to remove entries and will need 
                // to do the full update.
                if (%blankFound)
                    %needUpdate = true;
                // Add the enemy to the list for processing.
                %waveContents[%count] = %group;
                %count++;
                //echo(" -- Prefetch " @ %tempWave.getInternalName() @ " phase 1- " @ %a @ " : " @ %waveContents[%a].getName() @ " : " @ %waveContents[%a].enemy @ " : " @ %waveContents[%a].spawner);
            }
            else
            {
                // The enemy is not in the persistent set.  Either the entry is blank in the wave
                // or the enemy has been removed.  Set %blankFound to true and check again if we find
                // another valid enemy in the wave.  If %blankFound is true and we find another valid
                // enemy this means we have deleted an enemy that exists in this wave and need to 
                // update this wave.
                %blankFound = true;
                // This should catch the case where the last enemy in a wave has been deleted.
                if (%group.enemy !$= "")
                    %needUpdate = true;
                %waveContents[%a] = "";
            }
        }
        // If we found a valid enemy after an invalid one we will need to continue the process and 
        // save the files when finished.
        if (%needUpdate)
        {
            // We're going to have to save
            %updateFiles = true;
            %k = 0;
            for (%m = 0; %m < %count; %m++)
            {
                if (%waveContents[%m] !$= "")
                {
                    // Add valid entries from the processing list to the clean list, packing them to the
                    // front of the array.
                    %cleanList[%k] = %waveContents[%m];
                    //echo(" -- Prefetch " @ %tempWave.getInternalName() @ " phase 2- " @ %k @ " : " @ %cleanList[%k].getName() @ " : " @ %cleanList[%k].enemy @ " : " @ %cleanList[%k].spawner);
                    %k++;
                }
            }
            // Tell the current wave behavior to clear its enemy list
            %tempWave.callOnBehaviors("clear");
            for (%c = 0; %c < %count; %c++)
            {
                // Write the new clean enemy list to the wave object
                //echo(" -- Prefetch " @ %tempWave @ " phase 3- " @ %c @ " : " @ %cleanList[%c] @ " : " @ %cleanSpawnList[%c] @ " : " @ %cleanDelayList[%c]);
                %tempWave.callOnBehaviors("addEnemyGroup", %cleanList[%c]);
            }
            // Tell the current wave to commit the new list and update its internal arrays
            %tempWave.callOnBehaviors("createGroupObjectList");
        }
    }
    // At this point we've processed all waves and if any of them had an invalid enemy we have
    // cleaned it.  If we needed to clean any wave we need to save the changes, but if we didn't
    // need to change anything we don't need to waste time on disk access.
    if (%updateFiles)
        LBProjectObj.saveLevel();
}

/// <summary>
/// This function updates the display with the selected wave's data.
/// </summary>
/// <param name="wave">The wave to display.</param>
function WaveManager::populateWaveDisplay(%this, %wave)
{
    $RefreshingWaveDisplay = true;
    //echo(" -- WaveManager::populateWaveDisplay(" @ %wave @ ")");
    %this.enemyCount = 0;
    WToolWaveContentsPane.clear();
    $WaveTool::firstEmpty = false;
    $WaveTool::emptyIndex = 0;
    %enemyPane = "";
    %waveObj = %this.WaveSet.findObjectByInternalName(%wave);
    %count = %this.GroupSet.getCount();

    for (%i = 0; %i < %count; %i++)
    {
        WaveTool.delayEdit[%i] = "";
        %group = %this.GroupSet.getObject(%i);
        if (isObject(%group) && isObject(%group.enemy))
        {
            //echo(" -- Adding " @ %enemy @ " of type " @ %enemy.getClassName() @ " to preview pane");
            %enemyPane = %this.createPreviewPane(%i);
            %enemyPane.WTEPreviewEnemyName.text = %group.enemy.getInternalName();

            if (%group.enemy.getClassName() $= "t2dAnimatedSprite")
            {
                %enemyPane.WTEPreviewDisplay.display(%group.enemy.animationSet.MoveSouthAnim, "t2dAnimatedSprite");

                if (%group.enemy.animationSet.MoveNorthMirror)
                    WTEPreviewDisplay.sprite.setFlipY();
            }
            else
                %enemyPane.WTEPreviewDisplay.display(%group.enemy.getImageMap(), "t2dStaticSprite");

            %enemyPane.WTEPreviewGroupSpawnDelay.text = (%group.spawnDelay $= "Manual" ? 0 : %group.spawnDelay);

            %enemyPane.WTEPreviewGroupCount.text = %group.enemyCount;
            %enemyPane.WTEPreviewGroupCount.delaySpinUp = %enemyPane.WTEPreviewDelaySpinUp;
            %enemyPane.WTEPreviewGroupCount.delaySpinDown = %enemyPane.WTEPreviewDelaySpinDown;

            if ( %enemyPane.WTEPreviewGroupCount.getText() < 2 )
            {
                %enemyPane.WTEPreviewSpawnDelay.setActive(false);
                %enemyPane.WTEPreviewDelaySpinUp.setActive(false);
                %enemyPane.WTEPreviewDelaySpinDown.setActive(false);
            }
            %enemyPane.WTEPreviewSpawnDelay.text = %group.enemyDelay;
            %enemyPane.WTEPreviewSpawnDelay.Profile = (%group.enemyCount > 1 ? "GuiTextEditNumericProfile" : "GuiTextEditInactiveProfile");
            WaveTool.delayEdit[%i] = %group.WTEPreviewSpawnDelay;
            WaveTool.groupDelayEdit[%i] = %enemyPane.WTEPreviewGroupSpawnDelay;

            %this.populatePathDropdown(%enemyPane.WTEPreviewPathDropdown);
            
            %path = %group.spawner;
            %pathName = %path.getInternalName();
            %entry = %enemyPane.WTEPreviewPathDropdown.findText(%pathName);
            %enemyPane.WTEPreviewPathDropdown.setSelected(%entry);
            WaveTool.onSelectPathPlaceholder[%enemyPane.WTEPreviewPathDropdown.getId()] = %enemyPane.WTEPreviewPathDropdown.getValue();
            
            %this.enemyCount += %enemyPane.WTEPreviewGroupCount.getText();
            
            %this.spawnDelayControl[%i] = %enemyPane.WTEPreviewGroupSpawnDelay;
            %this.enemyCountControl[%i] = %enemyPane.WTEPreviewGroupCount;
            %this.enemyDelayControl[%i] = %enemyPane.WTEPreviewSpawnDelay;
            %this.spawnerControl[%i] = %enemyPane.WTEPreviewPathDropdown;
        }
            
        if (isObject(%enemyPane))
            WToolWaveContentsPane.add(%enemyPane);
        }

    %enemyPane = %this.createPreviewPane();
        WToolWaveContentsPane.add(%enemyPane);

    $RefreshingWaveDisplay = false;

    if (%this.WaveSet.getCount() < 1)
        WToolRemoveWaveButton.setActive(false);
    else
        WToolRemoveWaveButton.setActive(true);

   %position = WToolWaveContentsPane.Position;
   %width = WToolWaveContentsPane.colCount * 248;
   %rowCount = mFloor(((WToolWaveContentsPane.getCount() - 1) / WToolWaveContentsPane.colCount)) + 1;
   WToolWaveContentsPane.resize(%position.x, %position.y, %width, %rowCount * 210);
   WToolWaveContentsPane.refresh();

    %this.updateEnemyTotal();
    %this.setEnemyCount();
}

/// <summary>
/// This function gets the wave's data from the selected wave.
/// </summary>
/// <param name="waveName">The name of the wave to fetch data from.</param>
function WaveManager::getWaveContents(%this, %waveName)
{
    %this.cleanSets();

    %this.curSelectedWave = %this.WaveSet.findObjectByInternalName(%waveName);

    %this.defaultSpawn = %this.curSelectedWave.callOnBehaviors("getDefaultSpawn");
    %this.startDelay = %this.curSelectedWave.callOnBehaviors("getStartDelay");
    %this.spawnSound = %this.curSelectedWave.callOnBehaviors("getSpawnSound");
    %this.manualStart = %this.curSelectedWave.callOnBehaviors("getManualStart");

    %this.curSelectedWave.callOnBehaviors("createGroupList");
    %count = %this.curSelectedWave.callOnBehaviors("getGroupCount");
    //echo(" -- " @ %count @ " groups in " @ %waveName);
    for (%i = 0; %i < %count; %i++)
    {
        %group = %this.curSelectedWave.callOnBehaviors("getEnemyGroup", %i).getName();
        %this.GroupSet.add(%group);
        //echo(" -- Stored " @ %this.GroupSet.getObject(%i).getName() @ " : " @ %this.GroupSet.getObject(%i).enemy);
    }
}

/// <summary>
/// This function copies the data from the desired wave to the currently selected wave.
/// </summary>
/// <param name="waveName">The source wave for the copy operation.</param>
function WaveManager::copyWave(%this, %waveName)
{
   //echo(" -- WaveManager::copyWave(" @ %waveName @ ")");
   %this.createHoldWave();
   
   for (%c = 0; %c < MainScene.getSceneObjectCount(); %c++)
   {
      %object = MainScene.getSceneObject(%c);
      if (%object.internalName $= %waveName)
      {
         //echo(" -- found " @ %object @ " : friendly name - " @ %object.internalName);
         break;
      }
   }
   
   %this.defaultSpawn = %object.callOnBehaviors("getDefaultSpawn");
   %this.startDelay = %object.callOnBehaviors("getStartDelay");
   %this.spawnSound = %object.callOnBehaviors("getSpawnSound");
   %this.manualStart = %object.callOnBehaviors("getManualStart");

   %this.curSelectedWave.callOnBehaviors("setDefaultSpawn", %this.defaultSpawn);
   %this.curSelectedWave.callOnBehaviors("setStartDelay", %this.startDelay);
   %this.curSelectedWave.callOnBehaviors("setSpawnSound", %this.spawnSound);
   %this.curSelectedWave.callOnBehaviors("setManualStart", %this.manualStart);
   
   %count = %object.callOnBehaviors("getGroupCount");
   
   for (%i = 0; %i < %count; %i++)
   {
      %group = %object.callOnBehaviors("getEnemyGroup", %i);
      //echo(" -- got enemy " @ %enemyName @ " from " @ %object.internalName @ " : " @ %object.getName());
      if (isObject(%group))
      {
         %this.GroupSet.add(%group);
         %this.curSelectedWave.callOnBehaviors("addEnemyGroup", %group);
      }
   }
   %this.curSelectedWave.callOnBehaviors("saveGroupList");
}

/// <summary>
/// This function creates a temporary wave object and stores the current wave data in it.
/// </summary>
function WaveManager::createHoldWave(%this)
{
   //echo(" -- WaveManager::createHoldWave()");
   if (!isObject(%this.HoldWave))
   {
      %this.HoldWave = new ScriptObject(HoldWave);
   }
   
   %this.HoldWave.defaultSpawn = %this.defaultSpawn;
   %this.HoldWave.startDelay = %this.startDelay;
   %this.HoldWave.spawnSound = %this.spawnSound;
   %this.HoldWave.manualStart = %this.manualStart;
   //%this.HoldWave.groupCount = %this.groupCount;
   
   for (%i = 0; %i < %this.GroupSet.getCount(); %i++)
   {
      %this.HoldWave.GroupSet.add(%this.GroupSet.getObject(%i));
      //echo(" -- HoldWave - " @ %enemy @ ", " @ %spawn @ ", " @ %delay);
   }
}

/// <summary>
/// This function requests the return of the data stored in the Hold Wave object to the currently selected
/// wave and discards the Hold Wave object.  This is a primitive 'undo' feature.
/// </summary>
function WaveManager::revert(%this)
{
   if (%this.lastRemoved !$= "")
   {
      // restore last removed wave
      
      // clear this out.
      %this.lastRemoved = "";
   }
   
   %this.revertWave();
   
   if (isObject(%this.lastAdded))
   {
      %this.removeWave(%this.lastAdded.internalName);
      %this.lastAdded.delete();
      %this.lastRemoved = ""; // we don't want to dirty again
   }
}

/// <summary>
/// This function handles reverting the currently selected wave to its original state.
/// </summary>
function WaveManager::revertWave(%this)
{
   //echo(" -- WaveManager::revertWave()");
   if (isObject(%this.HoldWave)) 
   {
      %this.defaultSpawn = %this.HoldWave.defaultSpawn;
      %this.startDelay = %this.HoldWave.startDelay;
      %this.spawnSound = %this.HoldWave.spawnSound;
      %this.manualStart = %this.HoldWave.manualStart;
      //%this.groupCount = %this.HoldWave.groupCount;
      
      for (%i = 0; %i < %this.HoldWave.GroupSet.getCount(); %i++)
      {
         %this.GroupSet.add(%this.HoldWave.GroupSet.getObject(%i));
      }

      %this.curSelectedWave.callOnBehaviors("setDefaultSpawn", %this.defaultSpawn);
      %this.curSelectedWave.callOnBehaviors("setStartDelay", %this.startDelay);
      %this.curSelectedWave.callOnBehaviors("setSpawnSound", %this.spawnSound);
      %this.curSelectedWave.callOnBehaviors("setManualStart", %this.manualStart);
      
      %this.curSelectedWave.callOnBehaviors("clearObjectList");
      for (%i = 0; %i < %this.HoldWave.GroupSet.getCount(); %i++)
      {
         %group = %this.GroupSet.getObject(%i);
         if (isObject(%group))
            %this.curSelectedWave.callOnBehaviors("addEnemyGroup", %group);
         //echo(" -- HoldWave - " @ %enemy @ ", " @ %spawn @ ", " @ %delay);
      }
      %this.curSelectedWave.callOnBehaviors("saveGroupList");
      
      %this.HoldWave.delete();

      return true;
   }
   return false;
}

/// <summary>
/// This function creates a new scene object and sets it up to act as a wave object.
/// </summary>
function WaveManager::createNewWave(%this)
{
    // Create a new wave object from scratch
    %wave = new SceneObject() {
        canSaveDynamicFields = "1";
        Position = "549.000 -1039.000";
        BodyType = "static";
        size = "2.000 2.000";
    };
    %behavior = WaveBehavior.createInstance();
    %wave.addBehavior(%behavior);
    
    %number = 1;
    %name = "Wave" @ %number;

    while (isObject(%name))
    {
        %number++;
        %name = "Wave" @ %number;
    }

    %wave.setName(%name);
    %name = "Wave " @ %number;
    %wave.setInternalName(%name);
    
    return %wave.getName();
}

/// <summary>
/// This function deletes a wave object from the scene.
/// </summary>
/// <param name="wave">The name of the wave to delete.</param>
function WaveManager::removeWave(%this, %wave)
{
    //echo(" -- WaveManager::removeWave("@%wave.internalName@")");
    %this.WaveSet.remove(%wave.getName());
    MainScene.remove(%wave.getName());
/*
    for (%i = 0; %i < %this.WaveSet.getCount(); %i++)
    {
        echo(" -- WaveManager::removeWave("@%wave.getName()@") verify set removal - " @ %this.WaveSet.getObject(%i).getName());
    }*/

    %check = %this.waveController.callOnBehaviors(waveExists, %wave.getName());
    if (%check == true)
        this.waveController.callOnBehaviors(removeWave, %wave.getName());

    %this.refreshWaveList();
    %this.WaveListSize = %this.WaveSet.getCount();
    if (%this.WaveSet.getCount() < 1)
        WToolRemoveWaveButton.setActive(false);
    %wave.delete();
}

/// <summary>
/// This function completely resets all waves on the controller and commits the current 
/// wave list to it.
/// </summary>
function WaveManager::commitWaveset(%this)
{
    %this.waveController.callOnBehaviors("clear");
    for (%i = 0; %i < %this.WaveSet.getCount(); %i++)
    {
        %wave = %this.WaveSet.getObject(%i).getName();
        //echo(" -- commitWaveset("@%wave@")");
        %this.waveController.callOnBehaviors("addWave", %wave);
    }
}

/// <summary>
/// This function saves the currently selected wave to its object.
/// </summary>
function WaveManager::saveWave(%this)
{
    %waveName = %this.curSelectedWave.getInternalName();
    %found = false;
    for (%c = 0; %c < MainScene.getSceneObjectCount(); %c++)
    {
        %object = MainScene.getSceneObject(%c);
        if (%object.getInternalName() $= %waveName)
        {
            //echo(" -- found " @ %object @ " : friendly name - " @ %object.internalName);
            %found = true;
            break;
        }
    }
    if (%found == false)
    {
        echo(" -- WaveManager::saveWave() - Cannot find the requested wave");
        return;
    }

    // Because the GroupSet contains the same script objects as the wave behavior we
    // need to hold temp copies before we clear the wave's object list or we'll have
    // a list of blank groups.
    for (%c = 0; %c < %this.GroupSet.getCount(); %c++)
    {
        %group = %this.GroupSet.getObject(%c);
        %tempName = %group.getName() @ "temp";
        %tempObjectList[%c] = new ScriptObject(%tempName);
        %tempObjectList[%c].enemy = %group.enemy;
        %tempObjectList[%c].enemyCount = %group.enemyCount;
        %tempObjectList[%c].enemyDelay = %group.enemyDelay;
        %tempObjectList[%c].spawnDelay = %group.spawnDelay;
        %tempObjectList[%c].spawner = %group.spawner;
    }

    // This clears the current wave's groups
    %object.callOnBehaviors("clearObjectList");
    
    // And this restores our temp copies' values to the correct spots in the wave's 
    // GroupSet.
    for (%c = 0; %c < %this.GroupSet.getCount(); %c++)
    {
        %group = %this.GroupSet.getObject(%c);
        %group.enemy = %tempObjectList[%c].enemy;
        %group.enemyDelay = %tempObjectList[%c].enemyDelay;
        %group.enemyCount = %tempObjectList[%c].enemyCount;
        %group.spawnDelay = %tempObjectList[%c].enemyDelay;
        %group.spawner = %tempObjectList[%c].spawner;
    }

    %object.callOnBehaviors("setName", WToolWaveNameEdit.getText());
    %object.callOnBehaviors("setDefaultSpawn", %this.defaultSpawn);

    if (%this.startDelay != WToolWaveDelaySpinner.getValue())
        %this.startDelay = WToolWaveDelaySpinner.getValue();
    
    %this.manualStart = WToolManualRButton.getValue();
    %this.spawnSound = WToolWaveStartSoundEdit.getText();

    %object.callOnBehaviors("setStartDelay", %this.startDelay);
    %object.callOnBehaviors("setSpawnSound", %this.spawnSound);
    %object.callOnBehaviors("setManualStart", %this.manualStart);

    for (%i = 0; %i < %this.GroupSet.getCount(); %i++)
    {
        %group = %this.GroupSet.getObject(%i);
        //echo(" -- Accessing - " @ %group.getName() @ " enemy: " @ %group.enemy @ " enemyCount: " @ %group.enemyCount @ " enemyDelay: " @ %group.enemyDelay @ " spawnDelay: " @ %group.spawnDelay @ " spawner: " @ %group.spawner);
        if (isObject(%group))
        {
            //echo(" -- Saving - " @ %group.getName() @ " enemy: " @ %group.enemy @ " enemyCount: " @ %group.enemyCount @ " enemyDelay: " @ %group.enemyDelay @ " spawnDelay: " @ %group.spawnDelay @ " spawner: " @ %group.spawner);
            %object.callOnBehaviors("addEnemyGroup", %group);
        }
    }
        
    %object.callOnBehaviors(saveGroupList);

    if (isObject(%this.HoldWave))
        %this.HoldWave.delete();

    %this.commitWaveset();

    SetWaveToolDirtyState(false);
}

/// <summary>
/// This function sets the enemy count display.
/// </summary>
function WaveManager::setEnemyCount(%this)
{
    WToolWaveCurrentCountEdit.text = %this.enemyCount;
}

/// <summary>
/// This function adds the current list of available enemies to the desired dropdown control.
/// </summary>
/// <param name="dropdown">The dropdown to add the enemy list to.</param>
function WaveManager::populateEnemyList(%this, %dropdown)
{
    %selected = %dropdown.getSelected();
    if (%selected $= "")
        %selected = 0;
        
    %dropdown.clear();

    %count = $persistentObjectSet.getCount();

    %index = 0;

    for (%i = 0; %i < %count; %i++)
    {
        %sceneObject = $persistentObjectSet.getObject(%i);

        if (%sceneObject.Type !$= "Enemy")
            continue;

        %dropdown.add(%sceneObject.getName().getInternalName(), %index++);
        %this.WaveEnemySet.add(%sceneObject);
    }

    %dropdown.sort();
    %dropdown.setSelected((%selected > 0 ? %selected : 0));
}

/// <summary>
/// This function creates a GUI control that contains information on an enemy or a
/// group of enemies contained in the currently selected wave.  It appends an 'add enemy' 
/// button pane to the end of the list.
/// </summary>
/// <param name="index">An index number that is used to determine which pane the control callback came from.</param>
function WaveManager::createPreviewPane(%this, %index)
{
    %this.WaveGroupCountSet[%index] = 0;
   %pane = new GuiControl() {
      canSaveDynamicFields = "0";
         isContainer = "1";
      Profile = ((%index $= "" && $WaveTool::firstEmpty == true) ? "EditorPanelDark" : "EditorPanelMedium");
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "0 0";
      Extent = "248 210";
      MinExtent = "248 210";
      canSave = "0";
      Visible = "1";
      hovertime = "1000";
         index = %index;
   };
   
   if (%index !$= "")
   {
       %pane.WTEPreviewBox = new GuiControl() {
         canSaveDynamicFields = "0";
          isContainer = "1";
         Profile = "EditorPanelDark";
         HorizSizing = "right";
         VertSizing = "bottom";
         Position = "14 63";
         Extent = "72 72";
         MinExtent = "8 2";
         canSave = "1";
         Visible = "1";
         hovertime = "1000";
             index = %index;
       };
       %pane.addGuiControl(%pane.WTEPreviewBox);

      %pane.WTEPreviewDisplay = new SceneWindow() {
         canSaveDynamicFields = "0";
          superclass = "GenericPreviewWindow";
         isContainer = "0";
         Profile = "GuiDefaultProfile";
         HorizSizing = "right";
         VertSizing = "bottom";
         Position = "14 63";
         Extent = "72 72";
         MinExtent = "8 2";
         canSave = "0";
         Visible = "1";
         hovertime = "1000";
         lockMouse = "0";
         UseWindowInputEvents = "0";
         UseObjectInputEvents = "0";
      };
      %pane.addGuiControl(%pane.WTEPreviewDisplay);

      %pane.WTEPreviewDisplaySelector = new GuiMouseEventCtrl() {
         canSaveDynamicFields = "0";
          class = "EnemyPreviewDisplaySelector";
         isContainer = "0";
         Profile = "GuiTransparentProfile";
         HorizSizing = "right";
         VertSizing = "bottom";
         Position = "14 63";
         Extent = "72 72";
         MinExtent = "8 2";
         canSave = "0";
         Visible = "1";
         hovertime = "1000";
         lockMouse = "0";
            index = %index;
      };
      %pane.addGuiControl(%pane.WTEPreviewDisplaySelector);
   
       %pane.WTEControlBox = new GuiControl() {
         canSaveDynamicFields = "0";
          isContainer = "1";
         Profile = "EditorPanelDark";
         HorizSizing = "right";
         VertSizing = "bottom";
         Position = "101 12";
         Extent = "127 187";
         MinExtent = "8 2";
         canSave = "1";
         Visible = "1";
         hovertime = "1000";
             index = %index;
       };
       %pane.addGuiControl(%pane.WTEControlBox);

      %pane.WTEPreviewPathDropdown = new GuiPopUpMenuCtrl() {
         canSaveDynamicFields = "0";
          class = "EnemyPreviewPathDropdown";
         isContainer = "0";
         Profile = "GuiPopUpMenuProfile";
         HorizSizing = "right";
         VertSizing = "bottom";
            Position = "9 127";
         Extent = "110 24";
         MinExtent = "8 2";
         canSave = "0";
         Visible = "1";
         hovertime = "1000";
         maxLength = "1024";
         maxPopupHeight = "200";
         sbUsesNAColor = "0";
         reverseTextList = "0";
         bitmapBounds = "16 16";
            index = %index;
      };
      %pane.WTEControlBox.addGuiControl(%pane.WTEPreviewPathDropdown);   
      
      %pane.WTEPreviewEnemyName = new GuiTextCtrl() {
         canSaveDynamicFields = "0";
          isContainer = "0";
         Profile = "GuiTextCenterProfile";
         HorizSizing = "right";
         VertSizing = "bottom";
         Position = "5 41";
         Extent = "92 19";
         MinExtent = "8 2";
         canSave = "1";
         Visible = (%index $= "" ? "0" : "1");
         hovertime = "1000";
         text = "enemyName";
         maxLength = "1024";
            enemyText = %pane.WTEPreviewEnemyName;
            delayEdit = %pane.WTEPreviewSpawnDelay;
      };
      %pane.addGuiControl(%pane.WTEPreviewEnemyName);
      
      %pane.WTEPreviewSpawnDelay = new GuiTextEditCtrl() {
         canSaveDynamicFields = "0";
          class = "EnemyPreviewPaneDelayEdit";
         isContainer = "0";
         Profile = "GuiTextEditNumericProfile";
         HorizSizing = "right";
         VertSizing = "bottom";
            Position = "74 84";
            Extent = "29 24";
         MinExtent = "8 2";
         canSave = "0";
         Visible = "1";
         hovertime = "1000";
         maxLength = "1024";
         historySize = "0";
         password = "0";
         tabComplete = "0";
         sinkAllKeyEvents = "0";
         password = "0";
         passwordMask = "*";
            index = %index;
      };
      %pane.WTEControlBox.addGuiControl(%pane.WTEPreviewSpawnDelay);
      
      %pane.WTEPreviewDelaySpinUp = new GuiBitmapButtonCtrl() {
         canSaveDynamicFields = "0";
          class = "EnemyPreviewEnemyDelaySpinUp";
         isContainer = "0";
         Profile = "GuiDefaultProfile";
         HorizSizing = "right";
         VertSizing = "bottom";
            Position = "106 84";
         Extent = "12 12";
         MinExtent = "8 2";
         canSave = "0";
         Visible = "1";
         hovertime = "1000";
         groupNum = "-1";
         buttonType = "PushButton";
         useMouseEvents = "1";
         isLegacyVersion = "0";
         bitmap = "templates/commoninterface/gui/images/spinUpButton_normal.png";
         bitmapNormal = "templates/commoninterface/gui/images/spinUpButton_normal.png";
         bitmapDepressed = "templates/commoninterface/gui/images/spinUpButton_depressed.png";
            index = %index;
            editControl = %pane.WTEPreviewSpawnDelay;
      };
      %pane.WTEControlBox.addGuiControl(%pane.WTEPreviewDelaySpinUp);
      
      %pane.WTEPreviewDelaySpinDown = new GuiBitmapButtonCtrl() {
         canSaveDynamicFields = "0";
          class = "EnemyPreviewEnemyDelaySpinDown";
         isContainer = "0";
         Profile = "GuiDefaultProfile";
         HorizSizing = "right";
         VertSizing = "bottom";
            Position = "106 96";
         Extent = "12 12";
         MinExtent = "8 2";
         canSave = "0";
         Visible = "1";
         hovertime = "1000";
         groupNum = "-1";
         buttonType = "PushButton";
         useMouseEvents = "1";
         isLegacyVersion = "0";
         bitmap = "templates/commoninterface/gui/images/spinDownButton_normal.png";
         bitmapNormal = "templates/commoninterface/gui/images/spinDownButton_normal.png";
         bitmapDepressed = "templates/commoninterface/gui/images/spinDownButton_depressed.png";
            index = %index;
            editControl = %pane.WTEPreviewSpawnDelay;
      };
      %pane.WTEControlBox.addGuiControl(%pane.WTEPreviewDelaySpinDown);
      
      %pane.WTEPreviewReplaceButton = new GuiBitmapButtonCtrl() {
         canSaveDynamicFields = "0";
          class = "EnemyPreviewReplaceButton";
         isContainer = "0";
         Profile = "GuiDefaultProfile";
         HorizSizing = "right";
         VertSizing = "bottom";
         Position = "35 158";
         Extent = "24 24";
         MinExtent = "8 2";
         canSave = "0";
         Visible = "1";
         hovertime = "1000";
         groupNum = "-1";
         buttonType = "PushButton";
         useMouseEvents = "1";
         isLegacyVersion = "0";
         bitmap = "templates/commoninterface/gui/images/buttonReplace_up";
         bitmapNormal = "templates/commoninterface/gui/images/buttonReplace_up";
         bitmapHilight = "templates/commoninterface/gui/images/buttonReplace_over";
         bitmapDepressed = "templates/commoninterface/gui/images/buttonReplace_down";
         bitmapInactive = "templates/commoninterface/gui/images/buttonReplace_inactive";
            index = %index;
      };
      %pane.WTEControlBox.addGuiControl(%pane.WTEPreviewReplaceButton);
      
      %pane.WTEPreviewRemoveButton = new GuiBitmapButtonCtrl() {
         canSaveDynamicFields = "0";
          class = "EnemyPreviewRemoveButton";
         isContainer = "0";
         Profile = "GuiDefaultProfile";
         HorizSizing = "right";
         VertSizing = "bottom";
         Position = "67 158";
         Extent = "24 24";
         MinExtent = "8 2";
         canSave = "0";
         Visible = "1";
         hovertime = "1000";
         groupNum = "-1";
         buttonType = "PushButton";
         useMouseEvents = "1";
         isLegacyVersion = "0";
         bitmap = "templates/commoninterface/gui/images/removeButton_normal.png";
         bitmapNormal = "templates/commoninterface/gui/images/removeButton_normal.png";
         bitmapHilight = "templates/commoninterface/gui/images/removeButton_highlight.png";
         bitmapDepressed = "templates/commoninterface/gui/images/removeButton_depressed.png";
         bitmapInactive = "templates/commoninterface/gui/images/removeButton_inactive.png";
            index = %index;
      };
      %pane.WTEControlBox.addGuiControl(%pane.WTEPreviewRemoveButton);

      %pane.WTEGroupSpawnText = new GuiMLTextCtrl() {
         canSaveDynamicFields = "0";
          isContainer = "0";
            isContainer = "1";
            Profile = "GuiTextCenterProfile";
         HorizSizing = "right";
         VertSizing = "bottom";
            Position = "6 5";
            Extent = "69 30";
         MinExtent = "8 2";
            canSave = "1";
         Visible = (%index $= "" ? "0" : "1");
         hovertime = "1000";
            lineSpacing = "2";
            allowColorChars = "0";
            maxChars = "-1";
            text = "Initial Spawn Delay";
                index = %index;
      };
      %pane.WTEControlBox.addGuiControl(%pane.WTEGroupSpawnText);

         %pane.WTEHowManyTxt = new GuiTextCtrl() {
            canSaveDynamicFields = "0";
            PlatformTarget = "UNIVERSAL";
            isContainer = "1";
            Profile = "GuiTextProfile";
            HorizSizing = "right";
            VertSizing = "bottom";
            Position = "6 45";
            Extent = "69 35";
            MinExtent = "8 2";
            canSave = "1";
            Visible = "1";
            hovertime = "1000";
            text = "How Many";
         maxLength = "1024";
                index = %index;
      };
         %pane.WTEControlBox.addGuiControl(%pane.WTEHowManyTxt);

         %pane.WTEPreviewGroupCount = new GuiTextEditCtrl() {
            canSaveDynamicFields = "0";
            PlatformTarget = "UNIVERSAL";
            class = "EnemyPreviewGroupCountEdit";
            isContainer = "0";
            Profile = "GuiTextEditNumericProfile";
            HorizSizing = "right";
            VertSizing = "bottom";
            Position = "74 52";
            Extent = "29 24";
            MinExtent = "8 2";
            canSave = "1";
            Visible = "1";
            hovertime = "1000";
            maxLength = "1024";
            historySize = "0";
            password = "0";
            tabComplete = "0";
            sinkAllKeyEvents = "0";
            password = "0";
            passwordMask = "*";
                index = %index;
         };
         %pane.WTEControlBox.addGuiControl(%pane.WTEPreviewGroupCount);
         
         %pane.WTEPreviewGroupCountSpinUp = new GuiBitmapButtonCtrl() {
            canSaveDynamicFields = "0";
            PlatformTarget = "UNIVERSAL";
            class = "EnemyPreviewGroupCountSpinUp";
            isContainer = "0";
            Profile = "GuiDefaultProfile";
            HorizSizing = "right";
            VertSizing = "bottom";
            Position = "106 52";
            Extent = "12 12";
            MinExtent = "8 2";
            canSave = "1";
            Visible = "1";
            hovertime = "1000";
            groupNum = "-1";
            buttonType = "PushButton";
            useMouseEvents = "0";
             isLegacyVersion = "0";
             bitmap = "templates/commoninterface/gui/images/spinUpButton_normal.png";
             bitmapNormal = "templates/commoninterface/gui/images/spinUpButton_normal.png";
             bitmapDepressed = "templates/commoninterface/gui/images/spinUpButton_depressed.png";
                index = %index;
                editControl = %pane.WTEPreviewGroupCount;
                delaySpinUp =  %pane.WTEPreviewDelaySpinUp;
                delaySpinDown = %pane.WTEPreviewDelaySpinDown;
         };
         %pane.WTEControlBox.addGuiControl(%pane.WTEPreviewGroupCountSpinUp);
         
         %pane.WTEPreviewGroupCountSpinDown = new GuiBitmapButtonCtrl() {
            canSaveDynamicFields = "0";
            PlatformTarget = "UNIVERSAL";
            class = "EnemyPreviewGroupCountSpinDown";
            isContainer = "0";
            Profile = "GuiDefaultProfile";
            HorizSizing = "right";
            VertSizing = "bottom";
            Position = "106 64";
            Extent = "12 12";
            MinExtent = "8 2";
            canSave = "1";
            Visible = "1";
            hovertime = "1000";
            groupNum = "-1";
            buttonType = "PushButton";
            useMouseEvents = "0";
             isLegacyVersion = "0";
             bitmap = "templates/commoninterface/gui/images/spinDownButton_normal.png";
             bitmapNormal = "templates/commoninterface/gui/images/spinDownButton_normal.png";
             bitmapDepressed = "templates/commoninterface/gui/images/spinDownButton_depressed.png";
                index = %index;
                editControl = %pane.WTEPreviewGroupCount;
                delaySpinUp =  %pane.WTEPreviewDelaySpinUp;
                delaySpinDown = %pane.WTEPreviewDelaySpinDown;
         };
         %pane.WTEControlBox.addGuiControl(%pane.WTEPreviewGroupCountSpinDown);
         
         %pane.WTESpawnDelayEachTxt = new GuiMLTextCtrl() {
            canSaveDynamicFields = "0";
            PlatformTarget = "UNIVERSAL";
            isContainer = "1";
            Profile = "GuiTextCenterProfile";
            HorizSizing = "right";
            VertSizing = "bottom";
            Position = "6 81";
            Extent = "69 30";
            MinExtent = "8 2";
            canSave = "1";
            Visible = "1";
            hovertime = "1000";
            lineSpacing = "2";
            allowColorChars = "0";
            maxChars = "-1";
            text = "Spawn Delay Each by";
                index = %index;
         };
         %pane.WTEControlBox.addGuiControl(%pane.WTESpawnDelayEachTxt);

         %pane.WTESeparator1 = new GuiSeparatorCtrl() {
            canSaveDynamicFields = "0";
            PlatformTarget = "UNIVERSAL";
            isContainer = "0";
            Profile = "GuiSeparatorProfile";
            HorizSizing = "right";
            VertSizing = "bottom";
            Position = "2 40";
            Extent = "123 2";
            MinExtent = "8 2";
            canSave = "1";
            Visible = "1";
            hovertime = "1000";
            Type = "Horizontal";
            BorderMargin = "2";
            Invisible = "0";
            LeftMargin = "0";
                index = %index;
         };
         %pane.WTEControlBox.addGuiControl(%pane.WTESeparator1);
         
         %pane.WTESeparator2 = new GuiSeparatorCtrl() {
            canSaveDynamicFields = "0";
            PlatformTarget = "UNIVERSAL";
            isContainer = "0";
            Profile = "GuiSeparatorProfile";
            HorizSizing = "right";
            VertSizing = "bottom";
            Position = "2 116";
            Extent = "123 2";
            MinExtent = "8 2";
            canSave = "1";
            Visible = "1";
            hovertime = "1000";
            Type = "Horizontal";
            BorderMargin = "2";
            Invisible = "0";
            LeftMargin = "0";
                index = %index;
         };
         %pane.WTEControlBox.addGuiControl(%pane.WTESeparator2);

         %pane.WTEPreviewGroupSpawnDelay = new GuiTextEditCtrl() {
            canSaveDynamicFields = "0";
            PlatformTarget = "UNIVERSAL";
            class = "EnemyPreviewPaneGroupDelayEdit";
            isContainer = "0";
            Profile = "GuiTextEditNumericProfile";
            HorizSizing = "right";
            VertSizing = "bottom";
            Position = "74 8";
            Extent = "29 24";
            MinExtent = "8 2";
            canSave = "1";
            Visible = "1";
            hovertime = "1000";
            maxLength = "1024";
            historySize = "0";
            password = "0";
            tabComplete = "0";
            sinkAllKeyEvents = "0";
            password = "0";
            passwordMask = "*";
                index = %index;
         };
         %pane.WTEControlBox.addGuiControl(%pane.WTEPreviewGroupSpawnDelay);
         
         %pane.WTEPreviewGroupDelaySpinUp = new GuiBitmapButtonCtrl() {
            canSaveDynamicFields = "0";
            PlatformTarget = "UNIVERSAL";
            class = "EnemyPreviewGroupDelaySpinUp";
            isContainer = "0";
            Profile = "GuiDefaultProfile";
            HorizSizing = "right";
            VertSizing = "bottom";
            Position = "106 8";
            Extent = "12 12";
            MinExtent = "8 2";
            canSave = "1";
            Visible = "1";
            hovertime = "1000";
            groupNum = "-1";
            buttonType = "PushButton";
            useMouseEvents = "0";
             isLegacyVersion = "0";
             bitmap = "templates/commoninterface/gui/images/spinUpButton_normal.png";
             bitmapNormal = "templates/commoninterface/gui/images/spinUpButton_normal.png";
             bitmapDepressed = "templates/commoninterface/gui/images/spinUpButton_depressed.png";
                index = %index;
                editControl = %pane.WTEPreviewGroupSpawnDelay;
         };
         %pane.WTEControlBox.addGuiControl(%pane.WTEPreviewGroupDelaySpinUp);
         
         %pane.WTEPreviewGroupDelaySpinDown = new GuiBitmapButtonCtrl() {
            canSaveDynamicFields = "0";
            PlatformTarget = "UNIVERSAL";
            class = "EnemyPreviewGroupDelaySpinDown";
            isContainer = "0";
            Profile = "GuiDefaultProfile";
            HorizSizing = "right";
            VertSizing = "bottom";
            Position = "106 20";
            Extent = "12 12";
            MinExtent = "8 2";
            canSave = "1";
            Visible = "1";
            hovertime = "1000";
            groupNum = "-1";
            buttonType = "PushButton";
            useMouseEvents = "0";
             isLegacyVersion = "0";
             bitmap = "templates/commoninterface/gui/images/spinDownButton_normal.png";
             bitmapNormal = "templates/commoninterface/gui/images/spinDownButton_normal.png";
             bitmapDepressed = "templates/commoninterface/gui/images/spinDownButton_depressed.png";
                index = %index;
                editControl = %pane.WTEPreviewGroupSpawnDelay;
         };
         %pane.WTEControlBox.addGuiControl(%pane.WTEPreviewGroupDelaySpinDown);

      $WaveTool::emptyIndex = %index + 1;

      if(isObject(%pane.WTEPreviewDisplaySelector))
      {
         %pane.WTEPreviewDisplaySelector.enemyText = %pane.WTEPreviewEnemyName;
         %pane.WTEPreviewDisplaySelector.delayEdit = %pane.WTEPreviewSpawnDelay;
         %pane.WTEPreviewDisplaySelector.spawnDropdown = %pane.WTEPreviewPathDropdown;
      }
      else
      {
         //echo(" -- " @ %pane @ ".WTEPreviewDisplaySelector not created!");
      }
   }
   else if (%index $= "" && $WaveTool::firstEmpty == false)
   {
      %pane.WTEPreviewDisplay = new GuiBitmapButtonCtrl() {
         canSaveDynamicFields = "0";
          class = "EnemyPreviewAddButton";
         isContainer = "0";
         Profile = "GuiDefaultProfile";
         HorizSizing = "right";
         VertSizing = "bottom";
         Position = "14 63";
         Extent = "72 72";
         MinExtent = "8 2";
         canSave = "0";
         Visible = "1";
         hovertime = "1000";
         groupNum = "-1";
         buttonType = "PushButton";
         useMouseEvents = "1";
         isLegacyVersion = "0";
         bitmap = "templates/TowerDefense/interface/gui/images/addEnemy";
         bitmapNormal = "templates/TowerDefense/interface/gui/images/addEnemy";
         bitmapHilight = "templates/TowerDefense/interface/gui/images/addEnemy";
         bitmapDepressed = "templates/TowerDefense/interface/gui/images/addEnemy";
         bitmapInactive = "templates/TowerDefense/interface/gui/images/addEnemy";
      };
      %pane.addGuiControl(%pane.WTEPreviewDisplay);

    %pane.WTEControlBox = new GuiControl() {
         canSaveDynamicFields = "0";
         PlatformTarget = "UNIVERSAL";
         isContainer = "1";
         Profile = "EditorPanelDark";
         HorizSizing = "right";
         VertSizing = "bottom";
         Position = "101 12";
         Extent = "127 187";
         MinExtent = "8 2";
         canSave = "1";
         Visible = "1";
         hovertime = "1000";
             index = %index;
       };
       %pane.addGuiControl(%pane.WTEControlBox);
      $WaveTool::firstEmpty = true;
   }
   else
   {
      %pane.WTEPreviewDisplay = new GuiBitmapCtrl() {
         canSaveDynamicFields = "0";
         isContainer = "0";
         Profile = "EditorPanelMedium";
         HorizSizing = "right";
         VertSizing = "bottom";
         Position = "14 63";
         Extent = "72 72";
         MinExtent = "8 2";
         canSave = "0";
         Visible = "1";
         hovertime = "1000";
         groupNum = "-1";
         buttonType = "PushButton";
         useMouseEvents = "0";
         bitmap = "templates/commoninterface/gui/images/emptyEnemySlotinWave.png";
      };
      %pane.addGuiControl(%pane.WTEPreviewDisplay);
   }

   return %pane;
}

/// <summary>
/// This function iterates through all enemy preview panes and aggregates the total
/// number of enemies in the current wave.  It also attaches the current number of 
/// enemies in each group to the group pane to allow the validation to easily 
/// revert to the previous value if needed.
/// </summary>
function WaveManager::updateEnemyTotal(%this)
{
    %paneCount = WToolWaveContentsPane.getCount();
    %enemyCount = 0;
    for (%i = 0; %i < %paneCount; %i++)
    {
        %pane = WToolWaveContentsPane.getObject(%i);
        if (isObject(%pane.WTEPreviewGroupCount))
        {
            %pane.WTEPreviewGroupCount.tempValue = %pane.WTEPreviewGroupCount.getText();
            %enemyCount += %pane.WTEPreviewGroupCount.getText();
        }
    }
    %this.enemyCount = %enemyCount;
    %this.setEnemyCount();
}

/// <summary>
/// This function sets the currently selected stamp enemy to the enemy displayed in the
/// selected enemy preview.
/// </summary>
function EnemyPreviewDisplaySelector::onMouseDown(%this)
{
   //echo(" -- EnemyPreviewDisplaySelector::onMouseDown("@%this@") - position " @ %this.index);
   %enemy = %this.enemyText.text;
   %delay = %this.delayEdit.getText();
   %spawn = %this.spawnDropdown.getValue();
   WToolEnemyPreviewDropdown.setSelected(WToolEnemyPreviewDropdown.findText(%enemy));
   WToolEnemyDelaySpinner.text = %delay;
   WToolEnemyPathDropdown.setSelected(%spawn);
}

/// <summary>
/// This function sets the spawn path for the enemies in the selected group pane.
/// </summary>
function EnemyPreviewPathDropdown::onSelect(%this)
{
    //echo(" -- EnemyPreviewPathDropdown::onSelect("@%this@") - position " @ %this.index);
    if (!$RefreshingWaveDisplay)
    {
        %path = %this.getValue();
        
        %group = WaveManager.GroupSet.getObject(%this.index);
        %group.spawner = WaveManager.PathSet.findObjectByInternalName(%path).getName();
        
        if (WaveTool.onSelectPathPlaceholder[%this] $= "")
            WaveTool.onSelectPathPlaceholder[%this] = %this.getValue();

        if (WaveTool.onSelectPathPlaceholder[%this] !$= %this.getValue())
            SetWaveToolDirtyState(true);

        WaveTool.onSelectPathPlaceholder[%this] = %this.getValue();
    }
}

/// <summary>
/// This function validates the selected group's delay time.
/// </summary>
function EnemyPreviewPaneDelayEdit::onValidate(%this)
{
    %group = WaveManager.GroupSet.getObject(%this.index);
    if (%this.getValue() != %group.enemyDelay)
        SetWaveToolDirtyState(true);
        
    if (%this.getValue() > 600)
        %this.setText(600);
    
    if (%this.getValue() < 0)
        %this.setText(0);
}

/// <summary>
/// This function increments the delay between enemies within the group.
/// </summary>
function EnemyPreviewEnemyDelaySpinUp::onClick(%this)
{
   //echo(" -- EnemyPreviewSpinUp::onClick("@%this@") - position " @ %this.index);
   SetWaveToolDirtyState(true);

   %value = %this.editControl.getText();
   %value++;
   if (%value > 600)
      %value = 600;
   %this.editControl.text = %value;

    %group = WaveManager.GroupSet.getObject(%this.index);
    %group.enemyDelay = %value;

    %group = WaveManager.GroupSet.getObject(%this.index);
    //echo(" -- EnemyPreviewSpinUp::onClick() - " @ %group.getName() @ " enemy: " @ %group.enemy @ " enemyCount: " @ %group.enemyCount @ " enemyDelay: " @ %group.enemyDelay @ " spawnDelay: " @ %group.spawnDelay @ " spawner: " @ %group.spawner);
}

/// <summary>
/// This function decrements the delay between enemies within the group.
/// </summary>
function EnemyPreviewEnemyDelaySpinDown::onClick(%this)
{
   //echo(" -- EnemyPreviewSpinDown::onClick("@%this@") - position " @ %this.index);
   SetWaveToolDirtyState(true);

   %value = %this.editControl.getText();
   %value--;
   if (%value < 0)
      %value = 0;
   %this.editControl.text = %value;

    %group = WaveManager.GroupSet.getObject(%this.index);
    %group.enemyDelay = %value;

    %group = WaveManager.GroupSet.getObject(%this.index);
    //echo(" -- EnemyPreviewSpinDown::onClick() - " @ %group.getName() @ " enemy: " @ %group.enemy @ " enemyCount: " @ %group.enemyCount @ " enemyDelay: " @ %group.enemyDelay @ " spawnDelay: " @ %group.spawnDelay @ " spawner: " @ %group.spawner);
}

/// <summary>
/// This function validates the selected group's delay time.
/// </summary>
function EnemyPreviewPaneGroupDelayEdit::onValidate(%this)
{
    %group = WaveManager.GroupSet.getObject(%this.index);
    if (%this.getValue() != %group.spawnDelay)
        SetWaveToolDirtyState(true);
        
    if (%this.getValue() > 600)
        %this.setText(600);
    
    if (%this.getValue() < 0)
        %this.setText(0);
}

/// <summary>
/// This function increments the delay between enemy groups.
/// </summary>
function EnemyPreviewGroupDelaySpinUp::onClick(%this)
{
    //echo(" -- EnemyPreviewSpinUp::onClick("@%this@") - position " @ %this.index);
    SetWaveToolDirtyState(true);

    %value = %this.editControl.getText();
    %value++;
    if (%value > 600)
        %value = 600;
    %this.editControl.text = %value;

    %group = WaveManager.GroupSet.getObject(%this.index);
    %group.spawnDelay = %value;

    %group = WaveManager.GroupSet.getObject(%this.index);
    //echo(" -- EnemyPreviewGroupDelaySpinUp::onClick() - " @ %group.getName() @ " enemy: " @ %group.enemy @ " enemyCount: " @ %group.enemyCount @ " enemyDelay: " @ %group.enemyDelay @ " spawnDelay: " @ %group.spawnDelay @ " spawner: " @ %group.spawner);
}

/// <summary>
/// This function decrements the delay between enemy groups.
/// </summary>
function EnemyPreviewGroupDelaySpinDown::onClick(%this)
{
    //echo(" -- EnemyPreviewSpinDown::onClick("@%this@") - position " @ %this.index);
    SetWaveToolDirtyState(true);

    %value = %this.editControl.getText();
    %value--;
    if (%value < 0)
        %value = 0;
    %this.editControl.text = %value;

    %group = WaveManager.GroupSet.getObject(%this.index);
    %group.spawnDelay = %value;

    %group = WaveManager.GroupSet.getObject(%this.index);
    //echo(" -- EnemyPreviewGroupDelaySpinDown::onClick() - " @ %group.getName() @ " enemy: " @ %group.enemy @ " enemyCount: " @ %group.enemyCount @ " enemyDelay: " @ %group.enemyDelay @ " spawnDelay: " @ %group.spawnDelay @ " spawner: " @ %group.spawner);
}

/// <summary>
/// This function validates the number of enemies in the selected group.
/// </summary>
function EnemyPreviewGroupCountEdit::onValidate(%this)
{
    %original = %this.tempValue;
    %value = %this.getText();

    if ((WaveManager.enemyCount + (%value - %original)) > 100)
    {
        WarningDialog.setupAndShow(WaveToolWindow.getGlobalCenter(), "Wave is Full", "You cannot have more than 100 enemies in a wave.", "", "", "OK");
        %this.text = %original;
        return;
    }
    else
    {
        SetWaveToolDirtyState(true);

        if (%value > 100)
            %value = 100;
        %this.text = %value;
        
        if (%value > 1)
        {
            WaveTool.zeroBounce = false;
            %this.delaySpinUp.setActive(true);
            %this.delaySpinDown.setActive(true);
            WaveManager.enemyDelayControl[%this.index].Profile = "GuiTextEditNumericProfile";
        }

        if (%value < 1)
        {
            WaveTool.zeroBounce = true;
            %value = 1;
            %this.text = %value;
        }
        %this.editControl.text = %value;

        if (%value < 2)
        {
            %this.delaySpinUp.setActive(false);
            %this.delaySpinDown.setActive(false);
            WaveManager.enemyDelayControl[%this.index].Profile = "GuiTextEditInactiveProfile";
        }

        %group = WaveManager.GroupSet.getObject(%this.index);
        %group.enemyCount = %value;

        WaveManager.updateEnemyTotal();
        WaveManager.setEnemyCount();
        SetWaveToolDirtyState(true);
    }
}

/// <summary>
/// This function increments the number of enemies in the selected group.
/// </summary>
function EnemyPreviewGroupCountSpinUp::onClick(%this)
{
   //echo(" -- EnemyPreviewSpinUp::onClick("@%this@") - position " @ %this.index);
    if (WaveManager.enemyCount >= 100)
    {
        WarningDialog.setupAndShow(WaveToolWindow.getGlobalCenter(), "Wave is Full", "You cannot have more than 100 enemies in a wave.", "", "", "OK");      
        return;
    }
    else
    {
        SetWaveToolDirtyState(true);

        %value = %this.editControl.getText();
        %value++;
        if (%value > 100)
            %value = 100;
        %this.editControl.text = %value;
        
        if (%value > 1)
        {
            WaveTool.zeroBounce = false;
            %this.delaySpinUp.setActive(true);
            %this.delaySpinDown.setActive(true);
            WaveManager.enemyDelayControl[%this.index].Profile = "GuiTextEditNumericProfile";
        }

        %group = WaveManager.GroupSet.getObject(%this.index);
        %group.enemyCount = %value;
        
        %group = WaveManager.GroupSet.getObject(%this.index);
        //echo(" -- EnemyPreviewCountSpinUp::onClick() - " @ %group.getName() @ " enemy: " @ %group.enemy @ " enemyCount: " @ %group.enemyCount @ " enemyDelay: " @ %group.enemyDelay @ " spawnDelay: " @ %group.spawnDelay @ " spawner: " @ %group.spawner);
        
        WaveManager.updateEnemyTotal();
        WaveManager.setEnemyCount();
    }
}

/// <summary>
/// This function decrements the number of enemies in the selected group.
/// </summary>
function EnemyPreviewGroupCountSpinDown::onClick(%this)
{
    //echo(" -- EnemyPreviewSpinDown::onClick("@%this@") - position " @ %this.index);
    %value = %this.editControl.getText();
    if (%value > 0)
    {
        %value--;
        if (%value < 1)
        {
            WaveTool.zeroBounce = true;
            %value = 1;
        }
        %this.editControl.text = %value;

        if (%value < 2)
        {
            %this.delaySpinUp.setActive(false);
            %this.delaySpinDown.setActive(false);
            WaveManager.enemyDelayControl[%this.index].Profile = "GuiTextEditInactiveProfile";
        }

        if (!WaveTool.zeroBounce)
        {
            %group = WaveManager.GroupSet.getObject(%this.index);
            %group.enemyCount = %value;
        }

        %group = WaveManager.GroupSet.getObject(%this.index);
        //echo(" -- EnemyPreviewCountSpinDown::onClick() - " @ %group.getName() @ " enemy: " @ %group.enemy @ " enemyCount: " @ %group.enemyCount @ " enemyDelay: " @ %group.enemyDelay @ " spawnDelay: " @ %group.spawnDelay @ " spawner: " @ %group.spawner);
        
        WaveManager.updateEnemyTotal();
        WaveManager.setEnemyCount();
        SetWaveToolDirtyState(true);
    }
}

/// <summary>
/// This function exchanges the current enemy pane information with the currently selected stamp enemy.
/// </summary>
function EnemyPreviewReplaceButton::onClick(%this)
{
   //echo(" -- EnemyPreviewReplaceButton::onClick("@%this@") - position " @ %this.index);
   SetWaveToolDirtyState(true);

   // WaveBehavior::addEnemy(%enemy, %spawn, %delay, %index);
    %name = WToolEnemyPreviewDropdown.getValue();
    %enemy = $persistentObjectSet.findObjectByInternalName(%name, true);
    if (%enemy !$= "NotFound")
    {
        %type = %enemy.getClassName();
        if (%type $= "t2dAnimatedSprite" || %type $= "t2dStaticSprite")
        {
            %group = WaveManager.GroupSet.getObject(%this.index);
            %group.enemy = %enemy.getName();
            %group.spawner = WaveManager.PathSet.findObjectByInternalName(WToolEnemyPathDropdown.getValue()).getName();
            %group.enemyDelay = WToolEnemyDelaySpinner.text;
            %group.spawnDelay = WaveManager.spawnDelayControl[%this.index].getText();
            %group.enemyCount = WaveManager.enemyCountControl[%this.index].getText();
        }
        else
        {
            //echo(" !!! Wave Tool: Enemy " @ %name @ " is not a sprite! : " @ %type);
            return;
        }
    }
    else
    {
        //echo(" !!! Wave Tool: No enemy named " @ %name @ " found!");
        return;
    }

   WaveManager.populateWaveDisplay(WToolSelectWaveDropdown.getValue());
}

/// <summary>
/// This function removes the selected enemy or group from the wave.
/// </summary>
function EnemyPreviewRemoveButton::onClick(%this)
{
   //echo(" -- EnemyPreviewRemoveButton::onClick("@%this@") - position " @ %this.index);
   SetWaveToolDirtyState(true);

   WaveManager.removeEnemy(%this.index);
   WaveManager.populateWaveDisplay(WToolSelectWaveDropdown.getValue());
}

/// <summary>
/// This function adds the currently selected stamp enemy to the wave in a new group.
/// </summary>
function EnemyPreviewAddButton::onClick(%this)
{
    // This function adds a new enemy (enemy group) to the current wave

    if (WaveManager.enemyCount >= 100)
    {
        WarningDialog.setupAndShow(WaveToolWindow.getGlobalCenter(), "Wave is Full", "You cannot have more than 100 enemies in a wave.", "", "", "OK");      
        return;
    }
    //echo(" -- EnemyPreviewAddButton::onClick("@%this@") - position " @ %this.index);
    // WaveBehavior::addEnemy(%this, %enemy, %spawn, %delay);
    %name = WToolEnemyPreviewDropdown.getValue();
    %enemy = $persistentObjectSet.findObjectByInternalName(%name, true);
    if (%enemy !$= "NotFound")
    {
        %type = %enemy.getClassName();
        if (%type $= "t2dAnimatedSprite" || %type $= "t2dStaticSprite")
        {
            WaveManager.groupCount++;
            %name = WaveManager.curSelectedWave.getName() @ "group" @ $WaveTool::emptyIndex;
            %group = new ScriptObject(%name);
            %group.enemy = %enemy.getName();
            %group.spawner = WaveManager.PathSet.findObjectByInternalName(WToolEnemyPathDropdown.getValue()).getName();
            %group.enemyDelay = WToolEnemyDelaySpinner.getText();
            %group.spawnDelay = WToolEnemyDelaySpinner.getText();
            %group.enemyCount = 1;
            WaveManager.GroupSet.add(%group);
            //echo(" -- adding " @ %enemy.getInternalName() @ " to group " @ %group.getName() @ " at index " @ $WaveTool::emptyIndex);
        }
        else
        {
            //echo(" !!! Wave Tool: Enemy " @ %name @ " is not a sprite! : " @ %type);
            return;
        }
    }
    else
    {
        echo(" !!! Wave Tool: No enemy named " @ %name @ " found!");
        return;
    }
    WaveManager.populateWaveDisplay(WToolSelectWaveDropdown.getValue());

    SetWaveToolDirtyState(true);
}

/// <summary>
/// This function clears the WToolWaveNameEdit control.
/// </summary>
function WToolWaveNameClearBtn::onClick()
{
    WToolWaveNameEdit.text = "";
    SetWaveToolDirtyState(true);
}