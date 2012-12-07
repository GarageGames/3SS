//--------------------------------
// Enemy Tool Help
//--------------------------------
function EnemyToolHelpButton::onClick(%this)
{
   gotoWebPage("http://docs.3stepstudio.com/towerdefense/enemy/");
}


//--------------------------------
// Enemy Globals
//--------------------------------

$EnemyToolInitialized = false;

$EnemyPreviewDefaultExtent = "256 256";

$SelectedEnemy = "";
$EnemyToSelect = "";

$SelectedEnemySpeed = "";

$EnemyHealthMin = 1;
$EnemyHealthMax = 10000;

$EnemyAttackMin = 1;
$EnemyAttackMax = 100;

$EnemyFundsValueMin = 1;
$EnemyFundsValueMax = 500;

$EnemyScoreValueMin = 1;
$EnemyScoreValueMax = 500;

$EnemyDeathSoundHandle = "";

$EnemyPreviewZoomed = false;

$EnemySpriteRadioSelection = "";


//--------------------------------
// Enemy Tool
//--------------------------------

/// <summary>
/// This function initializes the Enemy Tool when it is opened.
/// </summary>
function EnemyTool::onWake(%this)
{
   if (!$EnemyToolInitialized)
      %this.initialize();

   if ($SelectedEnemy !$= "")
      RefreshEnemyData();

    if (EnemyPreview.sprite.getClassName() !$= "t2dAnimatedSprite")
        EnemyPreviewPlayButton.Visible = false;
        
   if (!EnemyPreview.sprite.paused)
      EnemyPreviewPlayButton.setBitmap("templates/commonInterface/gui/images/pauseButton.png");
}

/// <summary>
/// This function updates dropdowns and finalizes the tool initialization.
/// </summary>
function EnemyTool::initialize(%this)
{
   EnemyDeathSoundDisplay.setActive(false);
   EnemySpriteDisplay.setActive(false);   
   
   EnemySelectedDropdown.refresh();
   EnemySelectedDropdown.setFirstSelected();
   
   EnemySpeedDropdown.initialize();
   
      
   $EnemyToolInitialized = true;
}

/// <summary>
/// This function cleans up when the tool is hidden.
/// </summary>
function EnemyTool::onDialogPop(%this)
{
   TemplateTool::onDialogPop();
   
   if ($EnemyDeathSoundHandle)
   {
      alxStop($EnemyDeathSoundHandle);
      $EnemyDeathSoundHandle = "";
   }
      
   SetEnemyToolDirtyState(false);
}


//--------------------------------
// Enemy Selection
//--------------------------------

/// <summary>
/// This function refreshes the EnemySelectedDropdown contents.
/// </summary>
function EnemySelectedDropdown::refresh(%this)
{
   %this.clear();
   
   %count = $persistentObjectSet.getCount();
   
   %index = 0;

   for (%i = 0; %i < %count; %i++)
   {
      %object = $persistentObjectSet.getObject(%i);
      
      if (%object.Type !$= "Enemy")
         continue;

      %this.add(%object.getName().getInternalName(), %index++);
   }
   
   %this.sort();
}

/// <summary>
/// This function updates the Enemy Tool based on the EnemySelectedDropdown selection.
/// </summary>
function EnemySelectedDropdown::onSelect(%this)
{
   if ($SelectedEnemy && ($SelectedEnemy.getInternalName() !$= %this.getText()))
      ValidateEnemyInfo();
   
   if ($Tools::TemplateToolDirty && !ConfirmDialog.isAwake() && ($SelectedEnemy.getInternalName() !$= %this.getText()))
   {
      $EnemyToSelect = $persistentObjectSet.findObjectByInternalName(EnemySelectedDropdown.getText(), true);
      
      ConfirmDialog.setupAndShow(EnemyToolWindow.getGlobalCenter(), "Save Enemy Changes?", 
         "Save", "if (SaveEnemyData()) { EnemySelectedDropdown.setSelected(EnemySelectedDropdown.findText($EnemyToSelect.getInternalName())); }", 
         "Don't Save", "SetEnemyToolDirtyState(false); ReactToEnemySelection();", 
         "Cancel", "EnemySelectedDropdown.setSelected(EnemySelectedDropdown.findText($SelectedEnemy.getInternalName()));");
   }
   else
      ReactToEnemySelection();
}

/// <summary>
/// This function sets the selected enemy and calls to refresh the enemy data.
/// </summary>
function ReactToEnemySelection()
{
   $SelectedEnemy = $persistentObjectSet.findObjectByInternalName(EnemySelectedDropdown.getText(), true);
   
   if (!$Tools::TemplateToolDirty)
      RefreshEnemyData();
}


//--------------------------------
// Enemy Addition
//--------------------------------
/// <summary>
/// This function handles the Add Enemy button.
/// </summary>
function EnemyAddButton::onClick(%this)
{
   if ($Tools::TemplateToolDirty)
   {
      ConfirmDialog.setupAndShow(EnemyToolWindow.getGlobalCenter(), "Save Enemy Changes?", 
            "Save", "if (SaveEnemyData()) { AddEnemy(); }", 
            "Don't Save", "SetEnemyToolDirtyState(false); AddEnemy();", 
            "Cancel", "");
   }
   else
      AddEnemy();
}

/// <summary>
/// This function adds a new enemy type and handles base initialization for it.
/// </summary>
function AddEnemy()
{
   if (!EnemySelectedDropdown.size())
      ToggleEnemyControlsActiveState(true);   
   
   %name = "Enemy0";
   %number = 0;
   
   %count = $persistentObjectSet.getCount();
   
   %index = 0;

   for (%i = 0; %i < %count; %i++)
   {
      %object = $persistentObjectSet.getObject(%i);
      
      if (%object.Type !$= "Enemy")
         continue;

      if (%object.getName() $= %name)
      {
         %number++;
         %name = "Enemy" @ %number;
         %i = 0;
      }
   }
   
   %newEnemy = DefaultEnemy.clone();
   
   %newEnemy.setName(%name);
   
   %internalName = "NewEnemy" @ %number;
   
   %newEnemy.setInternalName(%internalName);
   
   %newEnemy.setPosition(DefaultEnemy.getPositionX() + 128, DefaultEnemy.getPositionY());
   
   %newEnemy.setFieldValue("Type", "Enemy");
   
   %animationSet = new ScriptObject(%internalName @ "AnimSet") {
      canSaveDynamicFields = "1";
      PlatformTarget = "UNIVERSAL";
      class = "animationSet";
         NameTags = "2";
   };
   
   LBProjectObj.addAnimationSet(%animationSet.getName(), true);
   
   %newEnemy.AnimationSet = %animationSet.getName();
   
   $persistentObjectSet.add(%newEnemy);
   
   LBProjectObj.saveLevel();
   
   EnemySelectedDropdown.refresh();
   
   $SelectedEnemy = "";
   
   EnemySelectedDropdown.setSelected(EnemySelectedDropdown.findText(%newEnemy.getInternalName()));
}


//--------------------------------
// Enemy Removal
//--------------------------------
/// <summary>
/// This function removes the selected enemy type.
/// </summary>
function EnemyRemoveButton::onClick(%this)
{
   ConfirmDialog.setupAndShow(EnemyToolWindow.getGlobalCenter(), "Remove Enemy?", 
      "Delete", "DeleteSelectedEnemy();", "", "", "Cancel", "");
}


//--------------------------------
// Enemy Name
//--------------------------------

/// <summary>
/// This function refreshes the EnemyNameEditBox contents.
/// </summary>
function EnemyNameEditBox::refresh(%this)
{
   %this.text = $SelectedEnemy.getInternalName();
}

/// <summary>
/// This function validates the contents of the EnemyNameEditBox.
/// </summary>
function EnemyNameEditBox::onValidate(%this)
{
   if (%this.getText() !$= $SelectedEnemy.getInternalName())
      SetEnemyToolDirtyState(true);
}

/// <summary>
/// This function clears the contents of the EnemyNameEditBox.
/// </summary>
function EnemyNameClearButton::onClick(%this)
{
   EnemyNameEditBox.text = "";
   SetEnemyToolDirtyState(true);
}


//--------------------------------
// Enemy Speed
//--------------------------------

/// <summary>
/// This function populates the contents of the EnemySpeedDropdown.
/// </summary>
function EnemySpeedDropdown::initialize(%this)
{
   %this.add("Slowest", 0);
   %this.add("Slow", 1);
   %this.add("Average", 2);
   %this.add("Fast", 3);
   %this.add("Fastest", 4);
   
   %this.setSelected(2);
}

/// <summary>
/// This function refreshes the EnemySpeedDropdown contents.
/// </summary>
function EnemySpeedDropdown::refresh(%this)
{
   %speed = $SelectedEnemy.callOnBehaviors(getSpeed);
   
   $SelectedEnemySpeed = "";
   
   %this.setSelected(%this.findText(%speed));
}

/// <summary>
/// This function sets the enemy's speed.
/// </summary>
function EnemySpeedDropdown::onSelect(%this)
{
   if (($SelectedEnemySpeed !$= "") && ($SelectedEnemySpeed !$= %this.getText()))
      SetEnemyToolDirtyState(true);
   
   $SelectedEnemySpeed = %this.getText();   
}


//--------------------------------
// Enemy Health
//--------------------------------

/// <summary>
/// This function refreshes the EnemyHealthEditBox contents.
/// </summary>
function EnemyHealthEditBox::refresh(%this)
{
   %this.text = $SelectedEnemy.callOnBehaviors(getHealth);
}

/// <summary>
/// This function validates the EnemyHealthEditBox contents.
/// </summary>
function EnemyHealthEditBox::onValidate(%this)
{
   if ($SelectedEnemy $= "")
      return;
      
   if (%this.getValue() < $EnemyHealthMin)
      %this.setValue($EnemyHealthMin);
   else if (%this.getValue() > $EnemyHealthMax)
      %this.setValue($EnemyHealthMax);
      
   if (%this.getText() !$= $SelectedEnemy.callOnBehaviors(getHealth))
      SetEnemyToolDirtyState(true);
}

/// <summary>
/// This function handles incrementing the EnemyHealthEditBox contents.
/// </summary>
function EnemyHealthSpinUp::onClick(%this)
{
   if (EnemyHealthEditBox.getValue() < $EnemyHealthMax)
   {
      EnemyHealthEditBox.setValue(EnemyHealthEditBox.getValue() + 1);
      SetEnemyToolDirtyState(true);
   }
}

/// <summary>
/// This function handles decrementing the EnemyHealthEditBox contents.
/// </summary>
function EnemyHealthSpinDown::onClick(%this)
{
   if (EnemyHealthEditBox.getValue() > $EnemyHealthMin)
   {
      EnemyHealthEditBox.setValue(EnemyHealthEditBox.getValue() - 1);
      SetEnemyToolDirtyState(true);
   }
}


//--------------------------------
// Enemy Attack
//--------------------------------

/// <summary>
/// This function refreshes the EnemyAttackEditBox contents.
/// </summary>
function EnemyAttackEditBox::refresh(%this)
{
   %this.text = $SelectedEnemy.callOnBehaviors(getAttack);
}

/// <summary>
/// This function validates the EnemyAttackEditBox contents.
/// </summary>
function EnemyAttackEditBox::onValidate(%this)
{
   if ($SelectedEnemy $= "")
      return;
      
   if (%this.getValue() < $EnemyAttackMin)
      %this.setValue($EnemyAttackMin);
   else if (%this.getValue() > $EnemyAttackMax)
      %this.setValue($EnemyAttackMax);
      
   if (%this.getText() !$= $SelectedEnemy.callOnBehaviors(getAttack))
      SetEnemyToolDirtyState(true);
}

/// <summary>
/// This function handles incrementing the EnemyAttackEditBox contents.
/// </summary>
function EnemyAttackSpinUp::onClick(%this)
{
   if (EnemyAttackEditBox.getValue() < $EnemyAttackMax)
   {
      EnemyAttackEditBox.setValue(EnemyAttackEditBox.getValue() + 1);
      SetEnemyToolDirtyState(true);
   }
}

/// <summary>
/// This function handles decrementing the EnemyAttackEditBox contents.
/// </summary>
function EnemyAttackSpinDown::onClick(%this)
{
   if (EnemyAttackEditBox.getValue() > $EnemyAttackMin)
   {
      EnemyAttackEditBox.setValue(EnemyAttackEditBox.getValue() - 1);
      SetEnemyToolDirtyState(true);
   }
}


//--------------------------------
// Enemy Funds Value
//--------------------------------

/// <summary>
/// This function refreshes the EnemyFundsValueEditBox contents.
/// </summary>
function EnemyFundsValueEditBox::refresh(%this)
{
   %this.text = $SelectedEnemy.callOnBehaviors(getFundsValue);
}

/// <summary>
/// This function validates the EnemyFundsValueEditBox contents.
/// </summary>
function EnemyFundsValueEditBox::onValidate(%this)
{
   if ($SelectedEnemy $= "")
      return;
      
   if (%this.getValue() < $EnemyFundsValueMin)
      %this.setValue($EnemyFundsValueMin);
   else if (%this.getValue() > $EnemyFundsValueMax)
      %this.setValue($EnemyFundsValueMax);
      
   if (%this.getText() !$= $SelectedEnemy.callOnBehaviors(getFundsValue))
      SetEnemyToolDirtyState(true);
}

/// <summary>
/// This function handles incrementing the EnemyFundsValueEditBox contents.
/// </summary>
function EnemyFundsValueSpinUp::onClick(%this)
{
   if (EnemyFundsValueEditBox.getValue() < $EnemyFundsValueMax)
   {
      EnemyFundsValueEditBox.setValue(EnemyFundsValueEditBox.getValue() + 1);
      SetEnemyToolDirtyState(true);
   }
}

/// <summary>
/// This function handles Decrementing the EnemyFundsValueEditBox contents.
/// </summary>
function EnemyFundsValueSpinDown::onClick(%this)
{
   if (EnemyFundsValueEditBox.getValue() > $EnemyFundsValueMin)
   {
      EnemyFundsValueEditBox.setValue(EnemyFundsValueEditBox.getValue() - 1);
      SetEnemyToolDirtyState(true);
   }
}


//--------------------------------
// Enemy Score Value
//--------------------------------

/// <summary>
/// This function refreshes the EnemyScoreValueEditBox contents.
/// </summary>
function EnemyScoreValueEditBox::refresh(%this)
{
   %this.text = $SelectedEnemy.callOnBehaviors(getScoreValue);
}

/// <summary>
/// This function validates the EnemyScoreValueEditBox contents.
/// </summary>
function EnemyScoreValueEditBox::onValidate(%this)
{
   if ($SelectedEnemy $= "")
      return;
      
   if (%this.getValue() < $EnemyScoreValueMin)
      %this.setValue($EnemyScoreValueMin);
   else if (%this.getValue() > $EnemyScoreValueMax)
      %this.setValue($EnemyScoreValueMax);
      
   if (%this.getText() !$= $SelectedEnemy.callOnBehaviors(getScoreValue))
      SetEnemyToolDirtyState(true);
}

/// <summary>
/// This function handles incrementing the EnemyScoreValueEditBox contents.
/// </summary>
function EnemyScoreValueSpinUp::onClick(%this)
{
   if (EnemyScoreValueEditBox.getValue() < $EnemyScoreValueMax)
   {
      EnemyScoreValueEditBox.setValue(EnemyScoreValueEditBox.getValue() + 1);
      SetEnemyToolDirtyState(true);
   }
}

/// <summary>
/// This function handles decrementing the EnemyScoreValueEditBox contents.
/// </summary>
function EnemyScoreValueSpinDown::onClick(%this)
{
   if (EnemyScoreValueEditBox.getValue() > $EnemyScoreValueMin)
   {
      EnemyScoreValueEditBox.setValue(EnemyScoreValueEditBox.getValue() - 1);
      SetEnemyToolDirtyState(true);
   }
}


//--------------------------------
// Enemy Death Sound
//--------------------------------

/// <summary>
/// This function refreshes the EnemyDeathSoundDisplay contents.
/// </summary>
function EnemyDeathSoundDisplay::refresh(%this)
{
   %this.setText($SelectedEnemy.callOnBehaviors(getDeathSound));
}

/// <summary>
/// This function opens the Asset Library to select a new sound.
/// </summary>
function EnemyDeathSoundSelectButton::onClick(%this)
{
   AssetLibrary.open(%this, $SoundsPage, "");
}

/// <summary>
/// This function assigns the selected sound to the enemy.
/// </summary>
/// <param name="asset">The name of the audio profile to assign to the enemy's death sound.</param>
function EnemyDeathSoundSelectButton::setSelectedAsset(%this, %asset)
{
   EnemyDeathSoundDisplay.setText(%asset);
   
   SetEnemyToolDirtyState(true);
}

/// <summary>
/// This function plays the currently selected enemy's death sound.
/// </summary>
function EnemyDeathSoundPlayButton::onClick(%this)
{
   if ($EnemyDeathSoundHandle)
   {
      alxStop($EnemyDeathSoundHandle);
      $EnemyDeathSoundHandle = "";
   }
      
   $EnemyDeathSoundHandle = alxPlay(EnemyDeathSoundDisplay.getText());
}

/// <summary>
/// This function stops playback of the enemy's death sound.
/// </summary>
function EnemyDeathSoundStopButton::onClick(%this)
{
   if ($EnemyDeathSoundHandle)
   {
      alxStop($EnemyDeathSoundHandle);
      $EnemyDeathSoundHandle = "";
}
}


//--------------------------------
// Enemy Preview
//--------------------------------

/// <summary>
/// This function refreshes the EnemyPreview and dislays the selected animation.
/// </summary>
function EnemyPreview::refresh(%this)
{ 
   if (EnemySpriteDisplay.getText() $= "")
   {
      EnemyPreview.display("");
      
      return;  
   }
   
   %flipX = false;
   %flipY = false;
   
   if ($EnemySpriteRadioSelection $= "Static")
      %asset = EnemySpriteDisplay.getText();
   else
   {
      switch (EnemyPreviewDropdown.getSelected())
      {
         case 0:
            %asset = EnemySpriteDisplay.getText().GetAnimationDatablock("MoveNorthAnim");
            if (EnemySpriteDisplay.getText().MoveNorthMirror !$= "")
               %flipY = true;
         case 1:
            %asset = EnemySpriteDisplay.getText().GetAnimationDatablock("MoveEastAnim");
            if (EnemySpriteDisplay.getText().MoveEastMirror !$= "")
               %flipX = true;
         case 2:
            %asset = EnemySpriteDisplay.getText().GetAnimationDatablock("MoveSouthAnim");
            if (EnemySpriteDisplay.getText().MoveSouthMirror !$= "")
               %flipY = true;
         case 3:
            %asset = EnemySpriteDisplay.getText().GetAnimationDatablock("MoveWestAnim");
            if (EnemySpriteDisplay.getText().MoveWestMirror !$= "")
               %flipX = true;
         case 4:
            %asset = EnemySpriteDisplay.getText().GetAnimationDatablock("DeathNorthAnim");
            if (EnemySpriteDisplay.getText().DeathNorthMirror !$= "")
               %flipY = true;
         case 5:
            %asset = EnemySpriteDisplay.getText().GetAnimationDatablock("DeathEastAnim");
            if (EnemySpriteDisplay.getText().DeathEastMirror !$= "")
               %flipX = true;
         case 6:
            %asset = EnemySpriteDisplay.getText().GetAnimationDatablock("DeathSouthAnim");
            if (EnemySpriteDisplay.getText().DeathSouthMirror !$= "")
               %flipY = true;
         case 7:
            %asset = EnemySpriteDisplay.getText().GetAnimationDatablock("DeathWestAnim");
            if (EnemySpriteDisplay.getText().DeathWestMirror !$= "")
               %flipX = true;
      }
   }
   
   if ($EnemySpriteRadioSelection $= "Static")
   {
      EnemyPreview.display(%asset, "t2dStaticSprite");
      
      EnemyPreview.sprite.setFrame(EnemyStaticSpriteFrameEditBox.getValue());
   }
   else
      EnemyPreview.display(%asset, "t2dAnimatedSprite");
   
   %this.format();
   
   EnemyPreview.sprite.setFlip(%flipX, %flipY);
}

/// <summary>
/// This function handles 'zooming' the enemy preview.
/// </summary>
function EnemyPreview::format(%this)
{
   %frameSize = "";
   
   if ($EnemySpriteRadioSelection $= "Static")
      %frameSize = EnemyPreview.resource.getFrameSize(0);
   else
      %frameSize = EnemyPreview.resource.imageMap.getFrameSize(0);
      
   %previewCenter = EnemyPreview.getCenter();
   
   if ($EnemyPreviewZoomed)
   {
      if (%frameSize.x < %frameSize.y)
         EnemyPreview.setExtent($EnemyPreviewDefaultExtent.x * (%frameSize.x / %frameSize.y), $EnemyPreviewDefaultExtent.y);
      else if (%frameSize.x > %frameSize.y)
         EnemyPreview.setExtent($EnemyPreviewDefaultExtent.x, $EnemyPreviewDefaultExtent.y * (%frameSize.x / %frameSize.y));
      else
         EnemyPreview.setExtent($EnemyPreviewDefaultExtent.x, $EnemyPreviewDefaultExtent.y);
   }
   else
   {
      if ((%frameSize.x > $EnemyPreviewDefaultExtent.x) || (%frameSize.y > $EnemyPreviewDefaultExtent.y))
      {
         if (%frameSize.x >= %frameSize.y)
            EnemyPreview.setExtent($EnemyPreviewDefaultExtent.x, %frameSize.y * ($EnemyPreviewDefaultExtent.x / %frameSize.x));
         else
            EnemyPreview.setExtent(%frameSize.x * ($EnemyPreviewDefaultExtent.y / %frameSize.y), $EnemyPreviewDefaultExtent.y);
      }
      else
         EnemyPreview.setExtent(%frameSize.x, %frameSize.y);      
   }
   
   EnemyPreview.setPosition(%previewCenter.x - EnemyPreview.getExtent().x / 2,
      %previewCenter.y - EnemyPreview.getExtent().y / 2);
}

/// <summary>
/// This function handles toggling the enemy preview zoom.
/// </summary>
function EnemyPreviewZoomButton::onClick(%this)
{
   $EnemyPreviewZoomed = !$EnemyPreviewZoomed;
   
   EnemyPreview.format();
}

/// <summary>
/// This function refreshes the EnemyPreviewDropdown contents based on 
/// whether the enemy is a static sprite or an animated sprite.
/// </summary>
function EnemyPreviewDropdown::refresh(%this)
{
   // If enemy is static, reset to that.  If animated, reset to Move South.   
   
   if ($EnemySpriteRadioSelection $= "Static")
   {
      %this.clear();
      
      %this.add("Static", 0);
   
      %this.setSelected(0);
   }
   else
   {
      %this.clear();
      
      %this.addCategory("Move Animation");  
      %this.add("North", 0);
      %this.add("East", 1);
      %this.add("South", 2);
      %this.add("West", 3);
      
      %this.addCategory("Death Animation");  
      %this.add("North", 4);
      %this.add("East", 5);
      %this.add("South", 6);
      %this.add("West", 7);
      
      %this.setSelected(2);
   }
}

/// <summary>
/// This function handles the EnemyPreviewDropdown selection, updates the preview and
/// kicks the preview play button if needed.
/// </summary>
function EnemyPreviewDropdown::onSelect(%this)
{
    EnemyPreview.refresh();
    EnemyPreviewPlayButton.onClick();

    if (%this.getSelected() > 3)
    {
        EnemyPreviewPlayButton.playOnceClick();
    }
    else
    {
        cancel(EnemyPreviewPlayButton.resetSchedule);
        EnemyPreviewPlayButton.loopClick();
    }
}


//--------------------------------
// Enemy Sprite
//--------------------------------

/// <summary>
/// This function toggles the enemy type to static sprite.
/// </summary>
function EnemyStaticRadioButton::onClick(%this)
{
   if ($EnemySpriteRadioSelection $= "Static")
      return; 
      
   $EnemySpriteRadioSelection = "Static";
   
   EnemySpriteDisplay.setText($SelectedEnemy.getImageMap());   
   
   ToggleEnemyStaticSpriteControls(true);
   
   EnemyPreviewDropdown.refresh();
   
   EnemyPreviewPlayButton.Visible = false;
   
   if ($SelectedEnemy.getClassName() $= "t2dAnimatedSprite")
      SetEnemyToolDirtyState(true);
}

/// <summary>
/// This function toggles the enemy type to animated sprite.
/// </summary>
function EnemyAnimatedRadioButton::onClick(%this)
{
   if ($EnemySpriteRadioSelection $= "Animated")
      return; 
      
   $EnemySpriteRadioSelection = "Animated";
      
   ToggleEnemyStaticSpriteControls(false);
   
   EnemySpriteDisplay.setText($SelectedEnemy.AnimationSet);
   
   EnemyPreviewDropdown.refresh();
   
   EnemyPreviewPlayButton.Visible = true;
      
   if ($SelectedEnemy.getClassName() $= "t2dStaticSprite")
      SetEnemyToolDirtyState(true);
}

/// <summary>
/// This function refreshes the EnemySpriteDisplay's contents.
/// </summary>
function EnemySpriteDisplay::refresh(%this)
{
   if ($SelectedEnemy.getClassName() $= "t2dStaticSprite")
      %this.setText($SelectedEnemy.getImageMap());
   else
      %this.setText($SelectedEnemy.animationSet);
}

/// <summary>
/// This function handles visibility of static sprite controls based on 
/// enemy type.
/// </summary>
function ToggleEnemyStaticSpriteControls(%visible)
{
   EnemyStaticSpriteAutoRotateCheckBox.setVisible(%visible);
   EnemyStaticSpriteFrameLabel.setVisible(%visible);
   EnemyStaticSpriteFrameSpinLeft.setVisible(%visible);
   EnemyStaticSpriteFrameEditBox.setVisible(%visible);
   EnemyStaticSpriteFrameSpinRight.setVisible(%visible);
   EnemySpriteSelectButton.setVisible(%visible);
   EnemyAnimSetEditButton.setVisible(!%visible);
   
   if (%visible)
      SetEnemyStaticSpriteFrameAvailability((EnemySpriteDisplay.getText() !$= "") && (EnemySpriteDisplay.getText().getFrameCount() > 1));

    EnemyPreviewPlayButton.Visible = !%visible;
    if (!%visible)
    {
        if (EnemyPreviewDropdown.getSelected() > 3)
        {
            EnemyPreviewPlayButton.setBitmap("templates/commonInterface/gui/images/pauseButton.png");
        }
        else
        {
            EnemyPreviewPlayButton.setBitmap("templates/commonInterface/gui/images/playButton.png");
        }
        EnemyPreview.refresh();
    }
}

/// <summary>
/// This function opens the Asset Library to select a static enemy sprite.
/// </summary>
function EnemySpriteSelectButton::onClick(%this)
{
      AssetLibrary.open(%this, $SpriteSheetPage, "");
}

/// <summary>
/// This function assigns the returned sprite.
/// </summary>
/// <param name="asset">The asset selected in the Asset Library.</param>
function EnemySpriteSelectButton::setSelectedAsset(%this, %asset)
{
   if (%asset !$= EnemySpriteDisplay.getText())
      SetEnemyToolDirtyState(true);   
   
   EnemySpriteDisplay.setText(%asset);
   
   SetEnemyStaticSpriteFrameAvailability(EnemySpriteDisplay.getText().getFrameCount() > 1);
   
   EnemyPreview.refresh();
}

/// <summary>
/// This function opens the Enemy Animation Set Tool to edit the currently selected 
/// enemy's animation set.
/// </summary>
function EnemyAnimSetEditButton::onClick(%this)
{
   EnemyAnimTool.animationSet = $SelectedEnemy.AnimationSet.getId();
   Canvas.pushDialog(EnemyAnimTool);
}

/// <summary>
/// This function sets the enemy's animation set to the new set and refreshes the display.
/// </summary>
/// <param name="newAnimSetName">The name of the animation set to assign to the enemy.</param>
function EnemyUpdateAnimSet(%newAnimSetName)
{
   if (%newAnimSetName !$= EnemySpriteDisplay.getText())
   {
      EnemySpriteDisplay.setText(%newAnimSetName);
      SaveEnemyData();
   }
   
   EnemyPreview.refresh();
}

/// <summary>
/// This function refreshes the EnemyStaticSpriteAutoRotateCheckBox contents based
/// on the selected enemy's state.
/// </summary>
function EnemyStaticSpriteAutoRotateCheckBox::refresh(%this)
{
   %this.setStateOn($SelectedEnemy.getBehavior(FaceDestinationBehavior));
}

/// <summary>
/// This function sets the enemy's auto-rotate property.  If this is set, the enemy will
/// rotate to face its travel direction.
/// </summary>
function EnemyStaticSpriteAutoRotateCheckBox::onClick(%this)
{
   SetEnemyToolDirtyState(true);
}

/// <summary>
/// This function selects the frame of a static sprite to use for the enemy's 
/// sprite image.
/// </summary>
function EnemyStaticSpriteFrameEditBox::refresh(%this)
{
   if ($SelectedEnemy.getClassName() !$= "t2dStaticSprite")
      return;
   
   if ($SelectedEnemy.getImageMap().getFrameCount())
      %this.text = $SelectedEnemy.getFrame();
   else
      %this.text = 0;
   
   SetEnemyStaticSpriteFrameAvailability($SelectedEnemy.getImageMap().getFrameCount() > 1);
}

/// <summary>
/// This function handles button and text profile switches based on the enemy's sprite type 
/// (animated or static).
/// </summary>
/// <param name="available">A flag to set static sprite option availability, true if static, false if animated.</param>
function SetEnemyStaticSpriteFrameAvailability(%available)
{
   EnemyStaticSpriteFrameEditBox.setActive(%available);
   EnemyStaticSpriteFrameEditBox.setProfile(%available ? "GuiTextEditProfile" : "GuiTextEditInactiveProfile");

   if (!%available)
      EnemyStaticSpriteFrameEditBox.setText(0);
   
   EnemyStaticSpriteFrameSpinLeft.setActive(%available);
   EnemyStaticSpriteFrameSpinRight.setActive(%available);
}

/// <summary>
/// This function validates the EnemyStaticSpriteFrameEditBox contents.
/// </summary>
function EnemyStaticSpriteFrameEditBox::onValidate(%this)
{
   if ($SelectedEnemy $= "")
      return;
      
   if (%this.getValue() < 0)
      %this.setValue(0);
   else if ((EnemySpriteDisplay.getText() !$= "") && (%this.getValue() > (EnemySpriteDisplay.getText().getFrameCount() - 1)))
      %this.setValue(EnemySpriteDisplay.getText().getFrameCount() - 1);
      
   EnemyPreview.refresh();
   
   if (%this.getText() !$= $SelectedEnemy.getFrame())
      SetEnemyToolDirtyState(true);
}

/// <summary>
/// This function decrements the EnemyStaticSpriteFrameEditBox contents.
/// </summary>
function EnemyStaticSpriteFrameSpinLeft::onClick(%this)
{
   if (EnemyStaticSpriteFrameEditBox.getValue() > 0)
   {
      EnemyStaticSpriteFrameEditBox.setValue(EnemyStaticSpriteFrameEditBox.getValue() - 1);
      EnemyPreview.refresh();
      SetEnemyToolDirtyState(true);
   }
}

/// <summary>
/// This function increments the EnemyStaticSpriteFrameEditBox contents.
/// </summary>
function EnemyStaticSpriteFrameSpinRight::onClick(%this)
{
   if (EnemyStaticSpriteFrameEditBox.getValue() < (EnemySpriteDisplay.getText().getFrameCount() - 1))
   {
      EnemyStaticSpriteFrameEditBox.setValue(EnemyStaticSpriteFrameEditBox.getValue() + 1);
      EnemyPreview.refresh();
      SetEnemyToolDirtyState(true);
   }
}


//--------------------------------
// Enemy Save
//--------------------------------

/// <summary>
/// This function handles the save button click.
/// </summary>
function EnemySaveButton::onClick(%this)
{
   if (SaveEnemyData())
      EnemySelectedDropdown.setSelected(EnemySelectedDropdown.findText($SelectedEnemy.getInternalName()));
}

/// <summary>
/// This function saves the selected enemy's data.
/// </summary>
function SaveEnemyData()
{ 
   if ($SelectedEnemy $= "")
      return false;
      
   if (EnemyNameEditBox.getText() $= "")
   {
      WarningDialog.setupAndShow(EnemyToolWindow.getGlobalCenter(), "Invalid Name", "You cannot Save an Enemy without a Name", "", "", "OK");
            
      return false;
   }
   
   // Check for Duplicate Internal Names
   %scene = ToolManager.getLastWindow().getScene();
   
   %sceneObjectCount = %scene.getSceneObjectCount();
   
   for (%i = 0; %i < %sceneObjectCount; %i++)
   {
      %sceneObject = %scene.getSceneObject(%i);
      
      if (%sceneObject == $SelectedEnemy)
         continue;
      
      if (%sceneObject.getInternalName() $= EnemyNameEditBox.getText())
      {
         WarningDialog.setupAndShow(EnemyToolWindow.getGlobalCenter(), "Duplicate Name", "Another Object in your Game Already has this Name", "", "", "OK");      
      
         return false;
      }
   }
      
   $SelectedEnemy.setInternalName(EnemyNameEditBox.getText());
   
   $SelectedEnemy.callOnBehaviors(setSpeed, EnemySpeedDropdown.getText());
   
   EnemyHealthEditBox.onValidate();
   $SelectedEnemy.callOnBehaviors(setHealth, EnemyHealthEditBox.getText());
   
   EnemyAttackEditBox.onValidate();
   $SelectedEnemy.callOnBehaviors(setAttack, EnemyAttackEditBox.getText());
   
   EnemyScoreValueEditBox.onValidate();
   $SelectedEnemy.callOnBehaviors(setScoreValue, EnemyScoreValueEditBox.getText());
   
   EnemyFundsValueEditBox.onValidate();
   $SelectedEnemy.callOnBehaviors(setFundsValue, EnemyFundsValueEditBox.getText());
   
   $SelectedEnemy.callOnBehaviors(setDeathSound, EnemyDeathSoundDisplay.getText());
   AddAssetToLevelDatablocks(EnemyDeathSoundDisplay.getText());
   
   if ($EnemySpriteRadioSelection $= "Static")
   {
      if ($SelectedEnemy.getClassName() $= "t2dAnimatedSprite")
      {
         %enemyInternalName = $SelectedEnemy.getInternalName();
         
         ConvertSpriteToOtherType($SelectedEnemy);
         
         $SelectedEnemy = $persistentObjectSet.findObjectByInternalName(%enemyInternalName);
      }
      
      $SelectedEnemy.setImageMap(EnemySpriteDisplay.getText());
      
      EnemyStaticSpriteFrameEditBox.onValidate();
      $SelectedEnemy.setFrame(EnemyStaticSpriteFrameEditBox.getValue());
      
      %size = $SelectedEnemy.getImageMap().getFrameSize(0);
      %size = Vector2Scale(%size, $TDMetersPerPixel);
      $SelectedEnemy.setSize(%size);
      
      if (EnemyStaticSpriteAutoRotateCheckBox.getValue())
      {
         if (!$SelectedEnemy.getBehavior(FaceDestinationBehavior))
            $SelectedEnemy.addBehavior(FaceDestinationBehavior.createInstance());
      }
      else
      {
         %autoRotateBehavior = $SelectedEnemy.getBehavior(FaceDestinationBehavior);
         
         if (%autoRotateBehavior)
            $SelectedEnemy.removeBehavior(%autoRotateBehavior, false);
      }
   }
   else
   {
      if ($SelectedEnemy.getClassName() $= "t2dStaticSprite")
      {
         %faceDesinationBehavior = $SelectedEnemy.getBehavior(FaceDestinationBehavior);
      
         if (%faceDesinationBehavior)
            $SelectedEnemy.removeBehavior(%faceDesinationBehavior, false);     
            
         %enemyInternalName = $SelectedEnemy.getInternalName();
         
         ConvertSpriteToOtherType($SelectedEnemy);
         
         $SelectedEnemy = $persistentObjectSet.findObjectByInternalName(%enemyInternalName);
      }
      
      $SelectedEnemy.animationSet = EnemySpriteDisplay.getText();
      $SelectedEnemy.animationName = $SelectedEnemy.animationSet.MoveSouthAnim;
      
      %size = $SelectedEnemy.getAnimation().imageMap.getFrameSize(0);
      %size = Vector2Scale(%size, $TDMetersPerPixel);
      $SelectedEnemy.setSize(%size);
   }
   
   AddAssetToLevelDatablocks(EnemySpriteDisplay.getText());

   LDEonApply();
   SaveAllLevelDatablocks();
   LBProjectObj.saveLevel();
   
   SetEnemyToolDirtyState(false);
   
   EnemySelectedDropdown.refresh();
   EnemySelectedDropdown.setSelected(EnemySelectedDropdown.findText($SelectedEnemy.getInternalName()), false);
   
   
   return true;
}

/// <summary>
/// This function handles setting the tool's dirty state to indicate that data needs 
/// to be saved.
/// </summary>
/// <param name="dirty">Flag to toggle the tool's dirty state, true if something needs to be saved, false if not.</param>
function SetEnemyToolDirtyState(%dirty)
{
   $Tools::TemplateToolDirty = %dirty;
   
   if ($Tools::TemplateToolDirty)
      EnemyToolWindow.setText("Enemy Tool *");
   else
      EnemyToolWindow.setText("Enemy Tool");
}


//--------------------------------
// Enemy Close
//--------------------------------

/// <summary>
/// This function handles the close button, showing a dialog if needed.
/// </summary>
function EnemyCloseButton::onClick(%this)
{
   if ($SelectedEnemy !$= "")
      ValidateEnemyInfo();
   
   if ($Tools::TemplateToolDirty)
   {
      ConfirmDialog.setupAndShow(EnemyToolWindow.getGlobalCenter(), "Save Enemy Changes?", 
            "Save", "if (SaveEnemyData()) { Canvas.popDialog(EnemyTool); }", 
            "Don't Save", "SetEnemyToolDirtyState(false); Canvas.popDialog(EnemyTool);", 
            "Cancel", "");
   }
   else
      Canvas.popDialog(EnemyTool);
}


//--------------------------------
// Enemy Miscellaneous
//--------------------------------

/// <summary>
/// This function refreshes the enemy data displayed based on the currently
/// selected enemy.
/// </summary>
function RefreshEnemyData()
{
   EnemyNameEditBox.refresh();
   EnemySpeedDropdown.refresh();
   EnemyHealthEditBox.refresh();
   EnemyAttackEditBox.refresh();
   EnemyScoreValueEditBox.refresh();
   EnemyFundsValueEditBox.refresh();
   EnemyDeathSoundDisplay.refresh();
   
   if ($SelectedEnemy.getClassName() $= "t2dStaticSprite")
   {
      $EnemySpriteRadioSelection = "Static";
      EnemyStaticRadioButton.setStateOn(true);
      ToggleEnemyStaticSpriteControls(true);
   }
   else
   {
      $EnemySpriteRadioSelection = "Animated";
      EnemyAnimatedRadioButton.setStateOn(true);
      ToggleEnemyStaticSpriteControls(false);
   }
      
   EnemySpriteDisplay.refresh();
   
   EnemyPreviewDropdown.refresh();
   
   EnemyStaticSpriteAutoRotateCheckBox.refresh();
   EnemyStaticSpriteFrameEditBox.refresh();
}

/// <summary>
/// This function handles deletion of the selected enemy type.
/// </summary>
function DeleteSelectedEnemy()
{
   // Remove deleted enemies that exist in current waves
   // This is the most appropriate place for this to occur, but after some investigation,
   // it is also much more messy/difficult.  For now, we will do a check and reaction 
   // upon launching the Wave Tool and do an object existence check in the Wave behaviors
   // in game.
   SetEnemyAnimToolDirtyState(false);
   
   $SelectedEnemy.safeDelete();
   
   $persistentObjectSet.remove($SelectedEnemy);
   
   LBProjectObj.saveLevel();
   
   EnemySelectedDropdown.refresh();
   
   $SelectedEnemy = "";
   
   EnemySelectedDropdown.setFirstSelected();

   if (!EnemySelectedDropdown.size())
   {
      ClearEnemyFields();
      ToggleEnemyControlsActiveState(false);
   }
   else
   {
      ToggleEnemyControlsActiveState(true);
   }
   
   SetEnemyToolDirtyState(false);
}

/// <summary>
/// This function clears all enemy data to baseline uninitialized values.
/// </summary>
function ClearEnemyFields()
{
   EnemyNameEditBox.setText("");
   EnemySpeedDropdown.setText("Average");
   EnemyHealthEditBox.setText("");
   EnemyAttackEditBox.setText("");
   EnemyScoreValueEditBox.setText("");
   EnemyFundsValueEditBox.setText("");
   EnemyDeathSoundDisplay.setText("");
   EnemyStaticRadioButton.onClick();
   EnemyStaticSpriteAutoRotateCheckBox.setStateOn(false);
   EnemyStaticSpriteFrameEditBox.setText("0");
}

/// <summary>
/// This function toggles all tool control availability.
/// </summary>
/// <param name="active">Flag to set for each control's active state, true for active, false for inactive.</param>
function ToggleEnemyControlsActiveState(%active)
{
   EnemySaveButton.setActive(%active);
   EnemySelectedDropdown.setActive(%active);
   EnemyRemoveButton.setActive(%active);
   EnemyNameEditBox.setActive(%active);
   EnemyNameClearButton.setActive(%active);
   EnemySpeedDropdown.setActive(%active);
   EnemyHealthEditBox.setActive(%active);
   EnemyHealthSpinUp.setActive(%active);
   EnemyHealthSpinDown.setActive(%active);
   EnemyAttackEditBox.setActive(%active);
   EnemyAttackSpinUp.setActive(%active);
   EnemyAttackSpinDown.setActive(%active);
   EnemyScoreValueEditBox.setActive(%active);
   EnemyScoreValueSpinUp.setActive(%active);
   EnemyScoreValueSpinDown.setActive(%active);
   EnemyFundsValueEditBox.setActive(%active);
   EnemyFundsValueSpinUp.setActive(%active);
   EnemyFundsValueSpinDown.setActive(%active);
   EnemyDeathSoundSelectButton.setActive(%active);
   EnemyDeathSoundPlayButton.setActive(%active);
   EnemyDeathSoundStopButton.setActive(%active);
   EnemyPreviewZoomButton.setActive(%active);
   EnemyPreviewDropdown.setActive(%active);
   EnemyStaticRadioButton.setActive(%active);
   EnemyAnimatedRadioButton.setActive(%active);
   EnemySpriteSelectButton.setActive(%active);
   EnemyAnimSetEditButton.setActive(%active);
   EnemyStaticSpriteAutoRotateCheckBox.setActive(%active);
   EnemyStaticSpriteFrameSpinLeft.setActive(%active);
   EnemyStaticSpriteFrameEditBox.setActive(%active);
   EnemyStaticSpriteFrameSpinRight.setActive(%active);
   
   EnemyNameEditBox.setProfile(%active ? "GuiTextEditProfile" : "GuiTextEditInactiveProfile");
   EnemyHealthEditBox.setProfile(%active ? "GuiTextEditProfile" : "GuiTextEditInactiveProfile");
   EnemyAttackEditBox.setProfile(%active ? "GuiTextEditProfile" : "GuiTextEditInactiveProfile");
   EnemyScoreValueEditBox.setProfile(%active ? "GuiTextEditProfile" : "GuiTextEditInactiveProfile");
   EnemyFundsValueEditBox.setProfile(%active ? "GuiTextEditProfile" : "GuiTextEditInactiveProfile");
   EnemyStaticSpriteFrameEditBox.setProfile(%active ? "GuiTextEditProfile" : "GuiTextEditInactiveProfile");
}

/// <summary>
/// This function verifies that all enemy data is valid.
/// </summary>
function ValidateEnemyInfo()
{
   EnemyNameEditBox.onValidate();
   EnemyHealthEditBox.onValidate();
   EnemyAttackEditBox.onValidate();
   EnemyScoreValueEditBox.onValidate();
   EnemyFundsValueEditBox.onValidate();
   
   if ($EnemySpriteRadioSelection $= "Static")
      EnemyStaticSpriteFrameEditBox.onValidate();
}

/// <summary>
/// This function handles playing/pausing the enemy preview if it is animated.
/// </summary>
function EnemyPreviewPlayButton::onClick(%this)
{
    %anim = EnemyPreview.sprite.getAnimation();
    if (%anim.animationCycle)
    {
        %this.loopClick();
    }
    else
    {
        %this.playOnceClick();
    }
}

/// <summary>
/// If the preview is a looping animation, this function handles playing/pausing it.
/// </summary>
function EnemyPreviewPlayButton::loopClick(%this)
{
    if (EnemyPreview.sprite.paused)
    {
        %this.setBitmap("templates/commonInterface/gui/images/pauseButton.png");
        EnemyPreview.play();
    }
    else
    {
        %this.setBitmap("templates/commonInterface/gui/images/playButton.png");
        EnemyPreview.pause();
    }
}

/// <summary>
/// If the preview is a non-looping animation, this function handles playing/pausing it.
/// </summary>
function EnemyPreviewPlayButton::playOnceClick(%this)
{
    if (EnemyPreview.sprite.getIsAnimationFinished())
        EnemyPreviewDropdown.onSelect();

    if (EnemyPreview.sprite.paused)
    {
        %this.setBitmap("templates/commonInterface/gui/images/pauseButton.png");
        EnemyPreview.play();
        %anim = EnemyPreview.sprite.getAnimation();
        %frames = getWordCount(%anim.animationFrames);
        %currentFrame = EnemyPreview.sprite.getAnimationFrame();
        %remainingFrames = %frames - %currentFrame;
        %frameTime = %anim.animationTime / %frames;
        %playTime = %remainingFrames * %frameTime * 1000;
        %this.resetSchedule = %this.schedule(%playTime, resetBitmap, "templates/commonInterface/gui/images/playButton.png");
    }
    else
    {
        %this.setBitmap("templates/commonInterface/gui/images/playButton.png");
        cancel(%this.resetSchedule);
        EnemyPreview.pause();
    }
}

/// <summary>
/// This function handles resetting the pause/play button image for non-looping animations.
/// </summary>
function EnemyPreviewPlayButton::resetBitmap(%this, %imageFile)
{
    %this.setBitmap(%imageFile);
    if (EnemyPreview.sprite.getIsAnimationFinished())
        EnemyPreviewDropdown.onSelect();
    EnemyPreview.sprite.setAnimationFrame(0);
    EnemyPreview.pause();
}