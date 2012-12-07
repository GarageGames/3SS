//--------------------------------
// Tower Tool Help
//--------------------------------
/// <summary>
/// This function opens a browser and navigates to the Tower Defense Template 
/// Tower Tool help page.
/// </summary>
function TowerToolHelpButton::onClick(%this)
{
   gotoWebPage("http://docs.3stepstudio.com/towerdefense/tower/");
}

//--------------------------------
// Tower Globals
//--------------------------------

$TowerToolInitialized = false;

$TowerPreviewDefaultExtent = "256 256";

$SelectedTower = "";
$TowerToSelect = "";

$SelectedTowerRange = "";

$SelectedTowerRateOfFire = "";

$TowerCostMin = 1;
$TowerCostMax = 1000;

$TowerSellValueMin = 0;
$TowerSellValueMax = 200;

$TowerFireSoundHandle = "";

$TowerSelectedProjectile = "";

$TowerPreviewZoomed = false;

$TowerSpriteRadioSelection = "";

$TowerToolLaunchedProjectileTool = false;
$TowerToolDirtyBeforeEditingProjectile = false;


//--------------------------------
// Tower Tool
//--------------------------------

/// <summary>
/// This function initializes the tool on wake.
/// </summary>
function TowerTool::onWake(%this)
{
   if (!$TowerToolInitialized)
      %this.initialize();

   TowerProjectileDropdown.initialize();
   ReactToTowerSelection();

    if (TowerPreview.sprite.getClassName() !$= "t2dAnimatedSprite")
        TowerPreviewPlayButton.Visible = false;

   if (!TowerPreview.sprite.paused)
      TowerPreviewPlayButton.setBitmap("templates/commonInterface/gui/images/pauseButton.png");

}

/// <summary>
/// This function performs one-time-only initialization tasks.
/// </summary>
function TowerTool::initialize(%this)
{
   TowerFireSoundDisplay.setActive(false);
   TowerSpriteDisplay.setActive(false);
   TowerIconDisplay.setActive(false);  
   
   TowerSelectedDropdown.initialize();
   TowerSelectedDropdown.refresh();
   
   TowerSelectedDropdown.setSelected(TowerSelectedDropdown.findText(Tower1.getInternalName()));
   
   TowerRangeDropdown.initialize();
   TowerRateOfFireDropdown.initialize();
   
   
   $TowerToolInitialized = true;
}

/// <summary>
/// This function handles changing away from this tool window.
/// </summary>
function TowerTool::onDialogPop(%this)
{
   TemplateTool::onDialogPop();
   
   if ($TowerFireSoundHandle)
   {
      alxStop($TowerFireSoundHandle);
      $TowerFireSoundHandle = "";
   }
      
   SetTowerToolDirtyState(false);
}


//--------------------------------
// Tower Selection
//--------------------------------

/// <summary>
/// This function adds a disabled scheme to the TowerSelectedDropdown control.
/// </summary>
function TowerSelectedDropdown::initialize(%this)
{
   %this.addScheme(1, "127 127 127", "0 0 0", "0 0 0");
}

/// <summary>
/// This function refreshes the contents of the TowerSelectedDropdown.
/// </summary>
function TowerSelectedDropdown::refresh(%this)
{
   %this.clear();
   
   %count = $persistentObjectSet.getCount();
   %towerSlotButton = -1;

   for (%i = 0; %i < %count; %i++)
   {
      %object = $persistentObjectSet.getObject(%i);
      
      if (%object.Type !$= "Tower")
         continue;

      switch$ (%object.getName())
      {
         case "Tower1":
            %slotIndex = 0;
            %towerSlotButton = TowerSlot1Button;
      
         case "Tower2":
            %slotIndex = 1;
            %towerSlotButton = TowerSlot2Button;
            
         case "Tower3":
            %slotIndex = 2;
            %towerSlotButton = TowerSlot3Button;
            
         case "Tower4":
            %slotIndex = 3;
            %towerSlotButton = TowerSlot4Button;
            
         case "Tower5":
            %slotIndex = 4;
            %towerSlotButton = TowerSlot5Button;
            
         case "Tower1Upgrade1":
            %slotIndex = 5;
            %towerSlotButton = TowerSlot1Upgrade1Button;
            
         case "Tower2Upgrade1":
            %slotIndex = 6;
            %towerSlotButton = TowerSlot2Upgrade1Button;
            
         case "Tower3Upgrade1":
            %slotIndex = 7;
            %towerSlotButton = TowerSlot3Upgrade1Button;
            
         case "Tower4Upgrade1":
            %slotIndex = 8;
            %towerSlotButton = TowerSlot4Upgrade1Button;
            
         case "Tower5Upgrade1":
            %slotIndex = 9;
            %towerSlotButton = TowerSlot5Upgrade1Button;
            
         case "Tower1Upgrade2":
            %slotIndex = 10;
            %towerSlotButton = TowerSlot1Upgrade2Button;
            
         case "Tower2Upgrade2":
            %slotIndex = 11;
            %towerSlotButton = TowerSlot2Upgrade2Button;
            
         case "Tower3Upgrade2":
            %slotIndex = 12;
            %towerSlotButton = TowerSlot3Upgrade2Button;
            
         case "Tower4Upgrade2":
            %slotIndex = 13;
            %towerSlotButton = TowerSlot4Upgrade2Button;
            
         case "Tower5Upgrade2":
            %slotIndex = 14;
            %towerSlotButton = TowerSlot5Upgrade2Button;
      }
      
      if (!%object.includeInGame)
      {
         %this.add(%object.getInternalName(), %slotIndex, 1);
         %towerSlotButton.setBitmapNormal("templates/TowerDefense/interface/gui/images/towerSlotUndefined");
      }
      else
      {
         %this.add(%object.getInternalName(), %slotIndex);
         %towerSlotButton.setBitmapNormal("templates/TowerDefense/interface/gui/images/towerSlotDefined");
      }
   }
   
   %this.sort();
}

/// <summary>
/// This function prompts the user to save changes if needed and requests data on the selected tower.
/// </summary>
function TowerSelectedDropdown::onSelect(%this)
{
   if ($SelectedTower && ($SelectedTower.getInternalName() !$= %this.getText()))
      ValidateTowerInfo();
      
   if ($Tools::TemplateToolDirty && !ConfirmDialog.isAwake() && ($SelectedTower.getInternalName() !$= %this.getText()))
   {
      $TowerToSelect = $persistentObjectSet.findObjectByInternalName(TowerSelectedDropdown.getText(), true);
      
      ConfirmDialog.setupAndShow(TowerToolWindow.getGlobalCenter(), "Save Tower Changes?", 
         "Save", "if (SaveTowerData()) { TowerSelectedDropdown.setSelected(TowerSelectedDropdown.findText($TowerToSelect.getInternalName())); }", 
         "Don't Save", "SetTowerToolDirtyState(false); ReactToTowerSelection();", 
         "Cancel", "TowerSelectedDropdown.setSelected(TowerSelectedDropdown.findText($SelectedTower.getInternalName()));");
   }
   else
      ReactToTowerSelection();
}

/// <summary>
/// This function retrieves the data for the selected tower and updates the controls.
/// </summary>
function ReactToTowerSelection()
{
   $SelectedTower = $persistentObjectSet.findObjectByInternalName(TowerSelectedDropdown.getText(), true);
   
   if (!$Tools::TemplateToolDirty)
      RefreshTowerData();
   
   switch (TowerSelectedDropdown.getSelected())
   {
      case 0:
         TowerSlot1Button.setStateOn(true);
         TowerIconContainer.setVisible(true);
         
      case 1:
         TowerSlot2Button.setStateOn(true);
         TowerIconContainer.setVisible(true);
         
      case 2:
         TowerSlot3Button.setStateOn(true);
         TowerIconContainer.setVisible(true);
         
      case 3:
         TowerSlot4Button.setStateOn(true);
         TowerIconContainer.setVisible(true);
         
      case 4:
         TowerSlot5Button.setStateOn(true);
         TowerIconContainer.setVisible(true);
         
      case 5:
         TowerSlot1Upgrade1Button.setStateOn(true);
         TowerIconContainer.setVisible(false);
         
      case 6:
         TowerSlot2Upgrade1Button.setStateOn(true);
         TowerIconContainer.setVisible(false);
         
      case 7:
         TowerSlot3Upgrade1Button.setStateOn(true);
         TowerIconContainer.setVisible(false);
         
      case 8:
         TowerSlot4Upgrade1Button.setStateOn(true);
         TowerIconContainer.setVisible(false);
         
      case 9:
         TowerSlot5Upgrade1Button.setStateOn(true);
         TowerIconContainer.setVisible(false);
         
      case 10:
         TowerSlot1Upgrade2Button.setStateOn(true);
         TowerIconContainer.setVisible(false);
         
      case 11:
         TowerSlot2Upgrade2Button.setStateOn(true);
         TowerIconContainer.setVisible(false);
         
      case 12:
         TowerSlot3Upgrade2Button.setStateOn(true);
         TowerIconContainer.setVisible(false);
         
      case 13:
         TowerSlot4Upgrade2Button.setStateOn(true);
         TowerIconContainer.setVisible(false);
         
      case 14:
         TowerSlot5Upgrade2Button.setStateOn(true);
         TowerIconContainer.setVisible(false);
   }
   
   TowerPreview.refresh();
   TowerIconFrameEditBox.refresh();
   TowerIconPreview.refresh();
   
   if (TowerIconContainer.isVisible())
      SetTowerIconFrameAvailability((TowerIconDisplay.getText() !$= "") && (TowerIconDisplay.getText().getFrameCount() > 1));
}


//--------------------------------
// Tower Removal
//--------------------------------

//function TowerRemoveButton::onClick(%this)
//{
   //ConfirmDialog.setupAndShow(TowerToolWindow.getGlobalCenter(), "Remove Tower?", 
      //"Delete", "DeleteSelectedTower();", "", "", "Cancel", "");
//}


//--------------------------------
// Tower Include In Game
//--------------------------------

/// <summary>
/// This function enables or disables the use of the selected tower in the game.  If it is ahead of another
/// tower in the same upgrade chain it will disable those above it if it is disabled but will not enable them
/// when it is enabled.
/// </summary>
function TowerIncludeInGameCheckBox::refresh(%this)
{
    switch$ ($SelectedTower.getName())
    {
        case "Tower1Upgrade1":
            %this.includeInGame = Tower1.includeInGame;     
        case "Tower1Upgrade2":
            %this.includeInGame = Tower1Upgrade1.includeInGame;
        case "Tower2Upgrade1":
            %this.includeInGame = Tower2.includeInGame;
        case "Tower2Upgrade2":
            %this.includeInGame = Tower2Upgrade1.includeInGame;
        case "Tower3Upgrade1":
            %this.includeInGame = Tower3.includeInGame;
        case "Tower3Upgrade2":
            %this.includeInGame = Tower3Upgrade1.includeInGame;
        case "Tower4Upgrade1":
            %this.includeInGame = Tower4.includeInGame;
        case "Tower4Upgrade2":
            %this.includeInGame = Tower4Upgrade1.includeInGame;
        case "Tower5Upgrade1":
            %this.includeInGame = Tower5.includeInGame;
        case "Tower5Upgrade2":
            %this.includeInGame = Tower5Upgrade1.includeInGame;
        default:
            %this.includeInGame = true;
    }

    %this.setStateOn($SelectedTower.includeInGame);
}

/// <summary>
/// This function sets the dirty state when the TowerIncludeInGameCheckBox is clicked.
/// </summary>
function TowerIncludeInGameCheckBox::onClick(%this)
{
   SetTowerToolDirtyState(true);
}


//--------------------------------
// Tower Name
//--------------------------------

/// <summary>
/// This function refreshes the contents of the TowerNameEditBox.
/// </summary>
function TowerNameEditBox::refresh(%this)
{
   %this.text = $SelectedTower.getInternalName();
}

/// <summary>
/// This function validates the contents of the TowerNameEditBox.
/// </summary>
function TowerNameEditBox::onValidate(%this)
{
   if (%this.getText() !$= $SelectedTower.getInternalName())
      SetTowerToolDirtyState(true);
}

/// <summary>
/// This function clears the contents of the TowerNameEditBox.
/// </summary>
function TowerNameClearButton::onClick(%this)
{
   TowerNameEditBox.text = "";
   SetTowerToolDirtyState(true);
}


//--------------------------------
// Tower Range
//--------------------------------

/// <summary>
/// This function sets up the range selections in the TowerRangeDropdown control.
/// </summary>
function TowerRangeDropdown::initialize(%this)
{
   %this.add("Shortest", 0);
   %this.add("Short", 1);
   %this.add("Average", 2);
   %this.add("Long", 3);
   %this.add("Longest", 4);
   
   %this.setSelected(2);
}

/// <summary>
/// This function refreshes the selected range from the selected tower.
/// </summary>
function TowerRangeDropdown::refresh(%this)
{
   %range = $SelectedTower.callOnBehaviors(getRange);
   
   $SelectedTowerRange = "";
   
   %this.setSelected(%this.findText(%range));
}

/// <summary>
/// This function updates the selected tower with the selected range.
/// </summary>
function TowerRangeDropdown::onSelect(%this)
{
   if (($SelectedTowerRange !$= "") && ($SelectedTowerRange !$= %this.getText()))
      SetTowerToolDirtyState(true);
   
   $SelectedTowerRange = %this.getText();   
}


//--------------------------------
// Tower Rate of Fire
//--------------------------------

/// <summary>
/// This function sets up the contents of the TowerRateOfFireDropdown.
/// </summary>
function TowerRateOfFireDropdown::initialize(%this)
{
   %this.add("Slowest", 0);
   %this.add("Slow", 1);
   %this.add("Average", 2);
   %this.add("Fast", 3);
   %this.add("Fastest", 4);
   
   %this.setSelected(2);
}

/// <summary>
/// This function refreshes the selection of the TowerRateOfFireDropdown from the selected tower.
/// </summary>
function TowerRateOfFireDropdown::refresh(%this)
{
   %rateOfFire = $SelectedTower.callOnBehaviors(getRateOfFire);
   
   $SelectedTowerRateOfFire = "";
   
   %this.setSelected(%this.findText(%rateOfFire));
}

/// <summary>
/// This function sets the selected rate of fire on the selected tower.
/// </summary>
function TowerRateOfFireDropdown::onSelect(%this)
{
   if (($SelectedTowerRateOfFire !$= "") && ($SelectedTowerRateOfFire !$= %this.getText()))
      SetTowerToolDirtyState(true);
   
   $SelectedTowerRateOfFire = %this.getText();   
}


//--------------------------------
// Tower Cost
//--------------------------------

/// <summary>
/// This function refreshes the contents of the TowerCostEditBox from the selected tower.
/// </summary>
function TowerCostEditBox::refresh(%this)
{
   %this.text = $SelectedTower.callOnBehaviors(getCost);
}

/// <summary>
/// This function validates the contents of the TowerCostEditBox.
/// </summary>
function TowerCostEditBox::onValidate(%this)
{
   if (%this.getValue() < $TowerCostMin)
      %this.setValue($TowerCostMin);
   else if (%this.getValue() > $TowerCostMax)
      %this.setValue($TowerCostMax);
      
   if (%this.getText() !$= $SelectedTower.callOnBehaviors(getCost))
      SetTowerToolDirtyState(true);
}

/// <summary>
/// This function increments the value of the tower cost value.
/// </summary>
function TowerCostSpinUp::onClick(%this)
{
   if (TowerCostEditBox.getValue() < $TowerCostMax)
   {
      TowerCostEditBox.setValue(TowerCostEditBox.getValue() + 1);
      SetTowerToolDirtyState(true);
   }
}

/// <summary>
/// This function decrements the value of the tower cost value.
/// </summary>
function TowerCostSpinDown::onClick(%this)
{
   if (TowerCostEditBox.getValue() > $TowerCostMin)
   {
      TowerCostEditBox.setValue(TowerCostEditBox.getValue() - 1);
      SetTowerToolDirtyState(true);
   }
}


//--------------------------------
// Tower Sell Value
//--------------------------------

/// <summary>
/// This function refreshes the contents of the TowerSellValueEditBox from the selected tower.
/// </summary>
function TowerSellValueEditBox::refresh(%this)
{
   %this.text = $SelectedTower.callOnBehaviors(getSellValue);
}

/// <summary>
/// This function validates the contents of the TowerSellValueEditBox.
/// </summary>
function TowerSellValueEditBox::onValidate(%this)
{
   if (%this.getValue() < $TowerSellValueMin)
      %this.setValue($TowerSellValueMin);
   else if (%this.getValue() > $TowerSellValueMax)
      %this.setValue($TowerSellValueMax);
      
   if (%this.getText() !$= $SelectedTower.callOnBehaviors(getSellValue))
      SetTowerToolDirtyState(true);
}

/// <summary>
/// This function increments the tower sell value.
/// </summary>
function TowerSellValueSpinUp::onClick(%this)
{
   if (TowerSellValueEditBox.getValue() < $TowerSellValueMax)
   {
      TowerSellValueEditBox.setValue(TowerSellValueEditBox.getValue() + 1);
      SetTowerToolDirtyState(true);
   }
}

/// <summary>
/// This function decrements the tower sell value.
/// </summary>
function TowerSellValueSpinDown::onClick(%this)
{
   if (TowerSellValueEditBox.getValue() > $TowerSellValueMin)
   {
      TowerSellValueEditBox.setValue(TowerSellValueEditBox.getValue() - 1);
      SetTowerToolDirtyState(true);
   }
}

//--------------------------------
// Tower Fire Sound
//--------------------------------

/// <summary>
/// This function refreshes the contents of the TowerFireSoundDisplay from the selected tower.
/// </summary>
function TowerFireSoundDisplay::refresh(%this)
{
   %this.setText($SelectedTower.callOnBehaviors(getFireSound));
}

/// <summary>
/// This function opens the Asset Library to select an audio profile for the selected tower's fire sound.
/// </summary>
function TowerFireSoundSelectButton::onClick(%this)
{
   AssetLibrary.open(%this, $SoundsPage, "");
}

/// <summary>
/// This function assigns the selected audio profile to the selected tower's fire sound.
/// </summary>
/// <param name="asset">The name of the audio profile selected in the Asset Library.</param>
function TowerFireSoundSelectButton::setSelectedAsset(%this, %asset)
{
   TowerFireSoundDisplay.setText(%asset);
   
   SetTowerToolDirtyState(true);
}

/// <summary>
/// This function plays the tower's fire sound.
/// </summary>
function TowerFireSoundPlayButton::onClick(%this)
{
   if ($TowerFireSoundHandle)
   {
      alxStop($TowerFireSoundHandle);
      $TowerFireSoundHandle = "";
   }
      
   $TowerFireSoundHandle = alxPlay(TowerFireSoundDisplay.getText());
}

/// <summary>
/// This function stops playback of the tower's fire sound.
/// </summary>
function TowerFireSoundStopButton::onClick(%this)
{
   if ($TowerFireSoundHandle)
   {
      alxStop($TowerFireSoundHandle);
      $TowerFireSoundHandle = "";
   }
}


//--------------------------------
// Tower Projectile
//--------------------------------
/// <summary>
/// This function the contents of the TowerProjectileDropdown.
/// </summary>
function TowerProjectileDropdown::initialize(%this)
{
   %this.clear();
   
   %count = $persistentObjectSet.getCount();
   
   %index = 0;

   for (%i = 0; %i < %count; %i++)
   {
      %object = $persistentObjectSet.getObject(%i);
      
      if (%object.Type !$= "Projectile")
         continue;

      %this.add(%object.getInternalName(), %index++);
   }
   
   %this.sort();
}

/// <summary>
/// This function refreshes the selection of the TowerProjectileDropdown from the selected tower and 
/// requests a refresh of the selected projectile's data.
/// </summary>
function TowerProjectileDropdown::refresh(%this)
{
   %this.setSelected(%this.findText($SelectedTower.callOnBehaviors(getProjectile).getInternalName()), false);
   
   $TowerSelectedProjectile = $persistentObjectSet.findObjectByInternalName(%this.getText(), true);
   
   RefreshTowerProjectileData();
}

/// <summary>
/// This function selects the projectile for the selected tower and requests a refresh
/// of the selected projectile's data.
/// </summary>
function TowerProjectileDropdown::onSelect(%this)
{
   if ($TowerSelectedProjectile && ($TowerSelectedProjectile.getInternalName() !$= %this.getText()))
      SetTowerToolDirtyState(true);
      
   $TowerSelectedProjectile = $persistentObjectSet.findObjectByInternalName(%this.getText(), true);
   
   RefreshTowerProjectileData();
}

/// <summary>
/// This function launches the Projectile Tool to edit the selected tower's current projectile.
/// </summary>
function TowerProjectileEditButton::onClick(%this)
{
   $TowerToolDirtyBeforeEditingProjectile = $Tools::TemplateToolDirty;
   $Tools::TemplateToolDirty = false;
   ProjectileSelectedDropdown.setActive(false);
   ProjectileAddButton.setActive(false);
   ProjectileRemoveButton.setActive(false);
   Canvas.pushDialog(ProjectileTool);
   $TowerToolLaunchedProjectileTool = true;
   $SelectedProjectile = "";
   ProjectileSelectedDropdown.setSelected(ProjectileSelectedDropdown.findText($TowerSelectedProjectile.getInternalName()));
}

/// <summary>
/// This function refreshes the damage display from the selected tower's selected projectile.
/// </summary>
function TowerProjectileDamageDisplay::refresh(%this)
{
   if (isObject($TowerSelectedProjectile))
      %this.setText($TowerSelectedProjectile.callOnBehaviors(getAttack));
}

/// <summary>
/// This function refreshes the selected tower's projectile effect display.
/// </summary>
function TowerProjectileEffectDisplay::refresh(%this)
{
   if (!isObject($TowerSelectedProjectile)) 
      return;  
   
   %behaviorCount = $TowerSelectedProjectile.getBehaviorCount();
   
   for (%i = 0; %i < %behaviorCount; %i++)
   {
      %behaviorTemplate = $TowerSelectedProjectile.getBehaviorByIndex(%i).template;
      
      if (%behaviorTemplate.behaviorType $= "AttackEffect")
      {
         %this.setText(%behaviorTemplate.friendlyName);
         break;
      }
   }
   
   if (%i == %behaviorCount)
      %this.setText("None");
}


//--------------------------------
// Tower Preview
//--------------------------------

/// <summary>
/// This function refreshes tower preview display.
/// </summary>
function TowerPreview::refresh(%this)
{
   if (TowerSpriteDisplay.getText() $= "")
   {
      TowerPreview.display("");
      
      return;  
   }
   
   %flipX = false;
   %flipY = false;
   
   if ($TowerSpriteRadioSelection $= "Static")
      %asset = TowerSpriteDisplay.getText();
   else
   {
      switch (TowerPreviewDropdown.getSelected())
      {
         case 0:
            %asset = TowerSpriteDisplay.getText().GetAnimationDatablock("IdleNorthAnim");
            if (TowerSpriteDisplay.getText().IdleNorthMirror !$= "")
               %flipY = true;
         case 1:
            %asset = TowerSpriteDisplay.getText().GetAnimationDatablock("IdleNortheastAnim");
            if (TowerSpriteDisplay.getText().IdleNortheastMirror $= "Horizontal")
               %flipX = true;
            else if (TowerSpriteDisplay.getText().IdleNortheastMirror $= "Vertical")
               %flipY = true;
         case 2:
            %asset = TowerSpriteDisplay.getText().GetAnimationDatablock("IdleEastAnim");
            if (TowerSpriteDisplay.getText().IdleEastMirror !$= "")
               %flipX = true;
         case 3:
            %asset = TowerSpriteDisplay.getText().GetAnimationDatablock("IdleSoutheastAnim");
            if (TowerSpriteDisplay.getText().IdleSoutheastMirror $= "Horizontal")
               %flipX = true;
            else if (TowerSpriteDisplay.getText().IdleSoutheastMirror $= "Vertical")
               %flipY = true;
         case 4:
            %asset = TowerSpriteDisplay.getText().GetAnimationDatablock("IdleSouthAnim");
            if (TowerSpriteDisplay.getText().IdleSouthMirror !$= "")
               %flipY = true;
         case 5:
            %asset = TowerSpriteDisplay.getText().GetAnimationDatablock("IdleSouthwestAnim");
            if (TowerSpriteDisplay.getText().IdleSouthwestMirror $= "Horizontal")
               %flipX = true;
            else if (TowerSpriteDisplay.getText().IdleSouthwestMirror $= "Vertical")
               %flipY = true;
         case 6:
            %asset = TowerSpriteDisplay.getText().GetAnimationDatablock("IdleWestAnim");
            if (TowerSpriteDisplay.getText().IdleWestMirror !$= "")
               %flipX = true;
         case 7:
            %asset = TowerSpriteDisplay.getText().GetAnimationDatablock("IdleNorthwestAnim");
            if (TowerSpriteDisplay.getText().IdleNorthwestMirror $= "Horizontal")
               %flipX = true;
            else if (TowerSpriteDisplay.getText().IdleNorthwestMirror $= "Vertical")
               %flipY = true;
         case 8:
            %asset = TowerSpriteDisplay.getText().GetAnimationDatablock("FiringNorthAnim");
            if (TowerSpriteDisplay.getText().FiringNorthMirror !$= "")
               %flipY = true;
         case 9:
            %asset = TowerSpriteDisplay.getText().GetAnimationDatablock("FiringNortheastAnim");
            if (TowerSpriteDisplay.getText().FiringNortheastMirror $= "Horizontal")
               %flipX = true;
            else if (TowerSpriteDisplay.getText().FiringNortheastMirror $= "Vertical")
               %flipY = true;
         case 10:
            %asset = TowerSpriteDisplay.getText().GetAnimationDatablock("FiringEastAnim");
            if (TowerSpriteDisplay.getText().FiringEastMirror !$= "")
               %flipX = true;
         case 11:
            %asset = TowerSpriteDisplay.getText().GetAnimationDatablock("FiringSoutheastAnim");
            if (TowerSpriteDisplay.getText().FiringSoutheastMirror $= "Horizontal")
               %flipX = true;
            else if (TowerSpriteDisplay.getText().FiringSoutheastMirror $= "Vertical")
               %flipY = true;
         case 12:
            %asset = TowerSpriteDisplay.getText().GetAnimationDatablock("FiringSouthAnim");
            if (TowerSpriteDisplay.getText().FiringSouthMirror !$= "")
               %flipY = true;
         case 13:
            %asset = TowerSpriteDisplay.getText().GetAnimationDatablock("FiringSouthwestAnim");
            if (TowerSpriteDisplay.getText().FiringSouthwestMirror $= "Horizontal")
               %flipX = true;
            else if (TowerSpriteDisplay.getText().FiringSouthwestMirror $= "Vertical")
               %flipY = true;
         case 14:
            %asset = TowerSpriteDisplay.getText().GetAnimationDatablock("FiringWestAnim");
            if (TowerSpriteDisplay.getText().FiringWestMirror !$= "")
               %flipX = true;
         case 15:
            %asset = TowerSpriteDisplay.getText().GetAnimationDatablock("FiringNorthwestAnim");
            if (TowerSpriteDisplay.getText().FiringNorthwestMirror $= "Horizontal")
               %flipX = true;
            else if (TowerSpriteDisplay.getText().FiringNorthwestMirror $= "Vertical")
               %flipY = true;
      }
   }

   %frameSize = "";

   if ($TowerSpriteRadioSelection $= "Static")
   {
      TowerPreview.display(%asset, "t2dStaticSprite");
      
      TowerPreview.sprite.setFrame(TowerStaticSpriteFrameEditBox.getValue());
   }
   else
      TowerPreview.display(%asset, "t2dAnimatedSprite");
   
   %this.format();
   
   TowerPreview.sprite.setFlip(%flipX, %flipY);
}

/// <summary>
/// This function handles 'zooming' the tower preview.
/// </summary>
function TowerPreview::format(%this)
{
   %frameSize = "";
   
   if ($TowerSpriteRadioSelection $= "Static")
      %frameSize = TowerPreview.resource.getFrameSize(0);
   else
      %frameSize = TowerPreview.resource.imageMap.getFrameSize(0);
      
   %previewCenter = TowerPreview.getCenter();
   
   if ($TowerPreviewZoomed)
   {
      if (%frameSize.x < %frameSize.y)
         TowerPreview.setExtent($TowerPreviewDefaultExtent.x * (%frameSize.x / %frameSize.y), $TowerPreviewDefaultExtent.y);
      else if (%frameSize.x > %frameSize.y)
         TowerPreview.setExtent($TowerPreviewDefaultExtent.x, $TowerPreviewDefaultExtent.y * (%frameSize.x / %frameSize.y));
      else
         TowerPreview.setExtent($TowerPreviewDefaultExtent.x, $TowerPreviewDefaultExtent.y);
   }
   else
   {
      if ((%frameSize.x > $TowerPreviewDefaultExtent.x) || (%frameSize.y > $TowerPreviewDefaultExtent.y))
      {
         if (%frameSize.x >= %frameSize.y)
            TowerPreview.setExtent($TowerPreviewDefaultExtent.x, %frameSize.y * ($TowerPreviewDefaultExtent.x / %frameSize.x));
         else
            TowerPreview.setExtent(%frameSize.x * ($TowerPreviewDefaultExtent.y / %frameSize.y), $TowerPreviewDefaultExtent.y);
      }
      else
         TowerPreview.setExtent(%frameSize.x, %frameSize.y);      
   }
   
   TowerPreview.setPosition(%previewCenter.x - TowerPreview.getExtent().x / 2,
      %previewCenter.y - TowerPreview.getExtent().y / 2);
}

/// <summary>
/// This function toggles the tower preview display's "zoom" state.
/// </summary>
function TowerPreviewZoomButton::onClick(%this)
{
   $TowerPreviewZoomed = !$TowerPreviewZoomed;
   
   TowerPreview.format();
}

/// <summary>
/// This function refreshes the contents of the TowerPreviewDropdown.
/// </summary>
function TowerPreviewDropdown::refresh(%this)
{
   // If Tower is static, reset to that.  If animated, reset to IdleSouth Animation.     
   
   if ($TowerSpriteRadioSelection $= "Static")
   {
      %this.clear();
      
      %this.add("Static", 0);
   
      %this.setSelected(0);
   }
   else
   {
      %this.clear();
      
      %this.addCategory("Idle Animation");  
      %this.add("North", 0);
      %this.add("Northeast", 1);
      %this.add("East", 2);
      %this.add("Southeast", 3);
      %this.add("South", 4);
      %this.add("Southwest", 5);
      %this.add("West", 6);
      %this.add("Northwest", 7);
      
      %this.addCategory("Firing Animation");  
      %this.add("North", 8);
      %this.add("Northeast", 9);
      %this.add("East", 10);
      %this.add("Southeast", 11);
      %this.add("South", 12);
      %this.add("Southwest", 13);
      %this.add("West", 14);
      %this.add("Northwest", 15);
      
      %this.setSelected($SelectedTower.animationSet.defaultAnimation);
   }
   
   TowerSlot1Button.setStateOn(true);
}

/// <summary>
/// This function sets the contents of the preview display and plays the animation 
/// if it is a fire animation.
/// </summary>
function TowerPreviewDropdown::onSelect(%this)
{
    TowerPreview.refresh();
    TowerPreviewPlayButton.onClick();
    
    if (%this.getSelected() > 7)
        TowerPreviewPlayButton.playOnceClick();
    else
    {
        cancel(TowerPreviewPlayButton.resetSchedule);
        TowerPreviewPlayButton.loopClick();
    }
}


//--------------------------------
// Tower Slot Selection
//--------------------------------
/// <summary>
/// This function displays the tower for the selected tower slot.
/// </summary>
function TowerSlot1Button::onClick(%this)
{
   TowerSelectedDropdown.setSelected(0);
}

/// <summary>
/// This function displays the tower for the selected tower slot.
/// </summary>
function TowerSlot2Button::onClick(%this)
{
   TowerSelectedDropdown.setSelected(1);
}

/// <summary>
/// This function displays the tower for the selected tower slot.
/// </summary>
function TowerSlot3Button::onClick(%this)
{
   TowerSelectedDropdown.setSelected(2);
}

/// <summary>
/// This function displays the tower for the selected tower slot.
/// </summary>
function TowerSlot4Button::onClick(%this)
{
   TowerSelectedDropdown.setSelected(3);
}

/// <summary>
/// This function displays the tower for the selected tower slot.
/// </summary>
function TowerSlot5Button::onClick(%this)
{
   TowerSelectedDropdown.setSelected(4);
}

/// <summary>
/// This function displays the tower for the selected tower slot.
/// </summary>
function TowerSlot1Upgrade1Button::onClick(%this)
{
   TowerSelectedDropdown.setSelected(5);
}   

/// <summary>
/// This function displays the tower for the selected tower slot.
/// </summary>
function TowerSlot2Upgrade1Button::onClick(%this)
{
   TowerSelectedDropdown.setSelected(6);
}

/// <summary>
/// This function displays the tower for the selected tower slot.
/// </summary>
function TowerSlot3Upgrade1Button::onClick(%this)
{
   TowerSelectedDropdown.setSelected(7);
}

/// <summary>
/// This function displays the tower for the selected tower slot.
/// </summary>
function TowerSlot4Upgrade1Button::onClick(%this)
{
   TowerSelectedDropdown.setSelected(8);
}

/// <summary>
/// This function displays the tower for the selected tower slot.
/// </summary>
function TowerSlot5Upgrade1Button::onClick(%this)
{
   TowerSelectedDropdown.setSelected(9);
}   

/// <summary>
/// This function displays the tower for the selected tower slot.
/// </summary>
function TowerSlot1Upgrade2Button::onClick(%this)
{
   TowerSelectedDropdown.setSelected(10);
}

/// <summary>
/// This function displays the tower for the selected tower slot.
/// </summary>
function TowerSlot2Upgrade2Button::onClick(%this)
{
   TowerSelectedDropdown.setSelected(11);
}

/// <summary>
/// This function displays the tower for the selected tower slot.
/// </summary>
function TowerSlot3Upgrade2Button::onClick(%this)
{
   TowerSelectedDropdown.setSelected(12);
}

/// <summary>
/// This function displays the tower for the selected tower slot.
/// </summary>
function TowerSlot4Upgrade2Button::onClick(%this)
{
   TowerSelectedDropdown.setSelected(13);
}

/// <summary>
/// This function displays the tower for the selected tower slot.
/// </summary>
function TowerSlot5Upgrade2Button::onClick(%this)
{
   TowerSelectedDropdown.setSelected(14);
}


//--------------------------------
// Tower Sprite
//--------------------------------

/// <summary>
/// This function handles converting the selected tower to a static sprite.
/// </summary>
function TowerStaticRadioButton::onClick(%this)
{
   if ($TowerSpriteRadioSelection $= "Static")
      return; 
      
   $TowerSpriteRadioSelection = "Static";
   
   ToggleTowerStaticSpriteControls(true);
   
   TowerSpriteDisplay.setText($SelectedTower.getImageMap());
   
   TowerPreviewDropdown.refresh();

    TowerPreviewPlayButton.Visible = false;
   
   if ($SelectedTower.getClassName() $= "t2dAnimatedSprite")
      SetTowerToolDirtyState(true);
}

/// <summary>
/// This function handles converting the selected tower to an animated sprite.
/// </summary>
function TowerAnimatedRadioButton::onClick(%this)
{
   if ($TowerSpriteRadioSelection $= "Animated")
      return; 
      
   $TowerSpriteRadioSelection = "Animated";
      
   ToggleTowerStaticSpriteControls(false);
   
   TowerSpriteDisplay.setText($SelectedTower.AnimationSet);
   
   TowerPreviewDropdown.refresh();
   
   TowerPreviewPlayButton.Visible = true;
      
   if ($SelectedTower.getClassName() $= "t2dStaticSprite")
      SetTowerToolDirtyState(true);
}

/// <summary>
/// This function refreshes the tower name display with the sprite's name if it's a static sprite, or the
/// animation set name if it's an animated sprite.
/// </summary>
function TowerSpriteDisplay::refresh(%this)
{
   if ($SelectedTower.getClassName() $= "t2dStaticSprite")
      %this.setText($SelectedTower.getImageMap());
   else
      %this.setText($SelectedTower.animationSet);
}

/// <summary>
/// This function sets the visibliity of the static sprite controls.
/// </summary>
/// <param name="visible">Shows the controls if true, hides them if false.</param>
function ToggleTowerStaticSpriteControls(%visible)
{
   TowerStaticSpriteFrameLabel.setVisible(%visible);
   TowerStaticSpriteFrameSpinLeft.setVisible(%visible);
   TowerStaticSpriteFrameEditBox.setVisible(%visible);
   TowerStaticSpriteFrameSpinRight.setVisible(%visible);
   TowerSpriteSelectButton.setVisible(%visible);
   TowerAnimSetEditButton.setVisible(!%visible);
   
   if (%visible)
      SetTowerStaticSpriteFrameAvailability((TowerSpriteDisplay.getText() !$= "") && (TowerSpriteDisplay.getText().getFrameCount() > 1));
}

/// <summary>
/// This function opens the Asset Library to select the tower's sprite image.
/// </summary>
function TowerSpriteSelectButton::onClick(%this)
{
   AssetLibrary.open(%this, $SpriteSheetPage, "");
}

/// <summary>
/// This function assigns the asset from the Asset Library to the tower's sprite.
/// </summary>
/// <param name="asset">The image passed back from the Asset Library.</param>
function TowerSpriteSelectButton::setSelectedAsset(%this, %asset)
{
   if (%asset !$= TowerSpriteDisplay.getText())
      SetTowerToolDirtyState(true);
      
   TowerSpriteDisplay.setText(%asset);
   
   SetTowerStaticSpriteFrameAvailability(TowerSpriteDisplay.getText().getFrameCount() > 1);
   
   TowerPreview.refresh();
}

/// <summary>
/// This function opens the Tower Animation Set Tool to edit the current tower's animation set.
/// </summary>
function TowerAnimSetEditButton::onClick(%this)
{
   TowerAnimTool.animationSet = $SelectedTower.AnimationSet.getId();
   Canvas.pushDialog(TowerAnimTool);
}

/// <summary>
/// This function updates the current tower with the desired animation set.
/// </summary>
/// <param name="newAnimSetName">The animation set to assign to the selected tower.</param>
function TowerUpdateAnimSet(%newAnimSetName)
{
   if (%newAnimSetName !$= TowerSpriteDisplay.getText())
   {
      TowerSpriteDisplay.setText(%newAnimSetName);
      SaveTowerData();
   }
   
   TowerPreview.refresh();
}

/// <summary>
/// This function displays sprite frame for the static sprite assigned to the selected tower.
/// </summary>
function TowerStaticSpriteFrameEditBox::refresh(%this)
{
   if ($SelectedTower.getClassName() !$= "t2dStaticSprite")
      return;   
   
   if ($SelectedTower.getImageMap().getFrameCount())
      %this.text = $SelectedTower.getFrame();
   else
      %this.text = 0;
   
   SetTowerStaticSpriteFrameAvailability($SelectedTower.getImageMap().getFrameCount() > 1);
}

/// <summary>
/// This function handles enabling or disabling the frame select spin buttons.  If the 
/// sprite has multiple frames enable the buttons, else disable them.
/// </summary>
function SetTowerStaticSpriteFrameAvailability(%available)
{
   TowerStaticSpriteFrameEditBox.setActive(%available);
   TowerStaticSpriteFrameEditBox.setProfile(%available ? "GuiTextEditProfile" : "GuiTextEditInactiveProfile");

   if (!%available)
      TowerStaticSpriteFrameEditBox.setText(0);
   
   TowerStaticSpriteFrameSpinLeft.setActive(%available);
   TowerStaticSpriteFrameSpinRight.setActive(%available);
}

/// <summary>
/// This function validates the value of the sprite frame display.
/// </summary>
function TowerStaticSpriteFrameEditBox::onValidate(%this)
{
   if (%this.getValue() < 0)
      %this.setValue(0);
   else if ((TowerSpriteDisplay.getText() !$= "") && (%this.getValue() > (TowerSpriteDisplay.getText().getFrameCount() - 1)))
      %this.setValue(TowerSpriteDisplay.getText().getFrameCount() - 1);
      
   TowerPreview.refresh();
   
   if (%this.getText() !$= $SelectedTower.getFrame())
      SetTowerToolDirtyState(true);
}

/// <summary>
/// This function decrements the sprite frame value.
/// </summary>
function TowerStaticSpriteFrameSpinLeft::onClick(%this)
{
   if (TowerStaticSpriteFrameEditBox.getValue() > 0)
   {
      TowerStaticSpriteFrameEditBox.setValue(TowerStaticSpriteFrameEditBox.getValue() - 1);
      TowerPreview.refresh();
      SetTowerToolDirtyState(true);
   }
}

/// <summary>
/// This function increments the sprite frame value.
/// </summary>
function TowerStaticSpriteFrameSpinRight::onClick(%this)
{
   if (TowerStaticSpriteFrameEditBox.getValue() < (TowerSpriteDisplay.getText().getFrameCount() - 1))
   {
      TowerStaticSpriteFrameEditBox.setValue(TowerStaticSpriteFrameEditBox.getValue() + 1);
      TowerPreview.refresh();
      SetTowerToolDirtyState(true);
   }
}


//--------------------------------
// Tower Icon
//--------------------------------

/// <summary>
/// This function refreshes the display of the tower creation button icon image 
/// from the selected tower chain.
/// </summary>
function TowerIconDisplay::refresh(%this)
{
   if (!%this.isVisible())
      return;

   switch$ ($SelectedTower.getName())
   {
      case "Tower1":
         %this.setText(TowerSlot1Icon.getImageMap());
      
      case "Tower2":
         %this.setText(TowerSlot2Icon.getImageMap());
      
      case "Tower3":
         %this.setText(TowerSlot3Icon.getImageMap());
      
      case "Tower4":
         %this.setText(TowerSlot4Icon.getImageMap());
      
      case "Tower5":
         %this.setText(TowerSlot5Icon.getImageMap());
   }
}

/// <summary>
/// This function opens the Asset Library to select an image for the tower 
/// creation button icon.
/// </summary>
function TowerIconSelectButton::onClick(%this)
{
   AssetLibrary.open(%this, $SpriteSheetPage, "");
}

/// <summary>
/// This function assigns the selected image to the tower creation button.
/// </summary>
/// <param name="asset">The image returned from the Asset Library.</param>
function TowerIconSelectButton::setSelectedAsset(%this, %asset)
{
   if (%asset !$= TowerIconDisplay.getText())
      SetTowerToolDirtyState(true);
      
   TowerIconDisplay.setText(%asset);
   
   SetTowerIconFrameAvailability(TowerIconDisplay.getText().getFrameCount() > 1);
   
   TowerIconPreview.refresh();
}

/// <summary>
/// This function updates the tower creation button image display.
/// </summary>
function TowerIconPreview::refresh(%this)
{
   if (TowerIconDisplay.getText() $= "")
   {
      TowerIconPreview.display("");
      
      return;  
   }

   TowerIconPreview.display(TowerIconDisplay.getText(), "t2dStaticSprite");
      
   TowerIconPreview.sprite.setFrame(TowerIconFrameEditBox.getValue());
}

/// <summary>
/// This function displays the tower for the selected tower slot.
/// </summary>
function TowerIconFrameEditBox::refresh(%this)
{
   if (!%this.isVisible())
      return;
      
   %towerSlotIcon = "";
      
   switch$ ($SelectedTower.getName())
   {
      case "Tower1":
         %towerSlotIcon = TowerSlot1Icon;
      case "Tower2":
         %towerSlotIcon = TowerSlot2Icon;
      case "Tower3":
         %towerSlotIcon = TowerSlot3Icon;
      case "Tower4":
         %towerSlotIcon = TowerSlot4Icon;
      case "Tower5":
         %towerSlotIcon = TowerSlot5Icon;
   }
   
   if (%towerSlotIcon !$= "")
   {
      if (%towerSlotIcon.getImageMap().getFrameCount())
         %this.setText(%towerSlotIcon.getFrame());
      else
         %this.text = 0;

      SetTowerIconFrameAvailability(%towerSlotIcon.getImageMap().getFrameCount() > 1);
   }
}

/// <summary>
/// This function enables or disables the frame spinner buttons based on the number of frames
/// in the selected sprite.
/// </summary>
/// <param name="available">Enables the controls if there are more than one frame, disables them if not.</param>
function SetTowerIconFrameAvailability(%available)
{
   TowerIconFrameEditBox.setActive(%available);
   TowerIconFrameEditBox.setProfile(%available ? "GuiTextEditProfile" : "GuiTextEditInactiveProfile");

   if (!%available)
      TowerIconFrameEditBox.setText(0);
   
   TowerIconFrameSpinLeft.setActive(%available);
   TowerIconFrameSpinRight.setActive(%available);
}

/// <summary>
/// This function handles validation of the tower icon frame value.
/// </summary>
function TowerIconFrameEditBox::onValidate(%this)
{
   if (%this.getValue() < 0)
      %this.setValue(0);
   else if ((TowerIconDisplay.getText() !$= "") && (%this.getValue() > (TowerIconDisplay.getText().getFrameCount() - 1)))
      %this.setValue(TowerIconDisplay.getText().getFrameCount() - 1);
      
   TowerIconPreview.refresh();
   
   %oldFrame = "";
   
   switch$ ($SelectedTower.getName())
   {
      case "Tower1":
         %oldFrame = TowerSlot1Icon.getFrame();
      
      case "Tower2":
         %oldFrame = TowerSlot2Icon.getFrame();
      
      case "Tower3":
         %oldFrame = TowerSlot3Icon.getFrame();
      
      case "Tower4":
         %oldFrame = TowerSlot4Icon.getFrame();
      
      case "Tower5":
         %oldFrame = TowerSlot5Icon.getFrame();
   }
   
   if (%this.getText() !$= %oldFrame)
      SetTowerToolDirtyState(true);
}

/// <summary>
/// This function decrements the icon frame value.
/// </summary>
function TowerIconFrameSpinLeft::onClick(%this)
{
   if (TowerIconFrameEditBox.getValue() > 0)
   {
      TowerIconFrameEditBox.setValue(TowerIconFrameEditBox.getValue() - 1);
      TowerIconPreview.refresh();
      SetTowerToolDirtyState(true);
   }
}

/// <summary>
/// This function increments the icon frame value.
/// </summary>
function TowerIconFrameSpinRight::onClick(%this)
{
   if (TowerIconFrameEditBox.getValue() < (TowerIconDisplay.getText().getFrameCount() - 1))
   {
      TowerIconFrameEditBox.setValue(TowerIconFrameEditBox.getValue() + 1);
      TowerIconPreview.refresh();
      SetTowerToolDirtyState(true);
   }
}


//--------------------------------
// Tower Save
//--------------------------------

/// <summary>
/// This function refreshes the tower data if the save is successful.
/// </summary>
function TowerSaveButton::onClick(%this)
{
   if (SaveTowerData())
      TowerSelectedDropdown.setSelected(TowerSelectedDropdown.findText($SelectedTower.getInternalName()));
}

/// <summary>
/// This function handles saving the tower data to the persistent set.
/// </summary>
function SaveTowerData()
{ 
   if ($SelectedTower $= "")
      return false;
      
   if (TowerNameEditBox.getText() $= "")
   {
      WarningDialog.setupAndShow(TowerToolWindow.getGlobalCenter(), "Invalid Name", "You cannot Save a Tower without a Name", "", "", "OK");
            
      return false;
   }
   
   // Check for Duplicate Internal Names
   %scene = ToolManager.getLastWindow().getScene();
   
   %sceneObjectCount = %scene.getSceneObjectCount();
   
   for (%i = 0; %i < %sceneObjectCount; %i++)
   {
      %sceneObject = %scene.getSceneObject(%i);
      
      if (%sceneObject == $SelectedTower)
         continue;
      
      if (%sceneObject.getInternalName() $= TowerNameEditBox.getText())
      {
         WarningDialog.setupAndShow(TowerToolWindow.getGlobalCenter(), "Duplicate Name", "Another Object in your Game Already has this Name", "", "", "OK");      
      
         return false;
      }
   }
      
   $SelectedTower.includeInGame = TowerIncludeInGameCheckBox.getValue();
   $SelectedTower.setInternalName(TowerNameEditBox.getText());
   $SelectedTower.callOnBehaviors(setRange, TowerRangeDropdown.getText());
   $SelectedTower.callOnBehaviors(setRateOfFire, TowerRateOfFireDropdown.getText());
   
   TowerCostEditBox.onValidate();
   $SelectedTower.callOnBehaviors(setCost, TowerCostEditBox.getText());
   
   TowerSellValueEditBox.onValidate();
   $SelectedTower.callOnBehaviors(setSellValue, TowerSellValueEditBox.getText());
   
   $SelectedTower.callOnBehaviors(setFireSound, TowerFireSoundDisplay.getText());
   AddAssetToLevelDatablocks(TowerFireSoundDisplay.getText());
   $SelectedTower.callOnBehaviors(setProjectile, $persistentObjectSet.findObjectByInternalName(TowerProjectileDropdown.getText(), true).getName());
   
   if ($TowerSpriteRadioSelection $= "Static")
   {
      if ($SelectedTower.getClassName() $= "t2dAnimatedSprite")
      {
         %towerInternalName = $SelectedTower.getInternalName();
         
         ConvertSpriteToOtherType($SelectedTower);
         
         $SelectedTower = $persistentObjectSet.findObjectByInternalName(%towerInternalName);
      }
      
      $SelectedTower.setImageMap(TowerSpriteDisplay.getText());
      
      TowerStaticSpriteFrameEditBox.onValidate();
      $SelectedTower.setFrame(TowerStaticSpriteFrameEditBox.getValue());
      
      %size = $SelectedTower.getImageMap().getFrameSize(0);
      %size = Vector2Scale(%size, $TDMetersPerPixel);
      $SelectedTower.setSize(%size);
   }
   else
   {
      if ($SelectedTower.getClassName() $= "t2dStaticSprite")
      {
         %towerInternalName = $SelectedTower.getInternalName();
         
         ConvertSpriteToOtherType($SelectedTower);
         
         $SelectedTower = $persistentObjectSet.findObjectByInternalName(%towerInternalName);
      }
      
      $SelectedTower.animationSet = TowerSpriteDisplay.getText();
      $SelectedTower.animationName = $SelectedTower.animationSet.IdleSouthAnim;
      
      %size = $SelectedTower.getAnimation().imageMap.getFrameSize(0);
      %size = Vector2Scale(%size, $TDMetersPerPixel);
      $SelectedTower.setSize(%size);
   }
   
   if ($SelectedTower.includeInGame && (TowerSpriteDisplay.getText() $= ""))
   {
      $SelectedTower.includeInGame = false;
      TowerIncludeInGameCheckBox.setStateOn(false);
   }
   
   if (TowerIconContainer.isVisible())
   {
      if ($SelectedTower.includeInGame && (TowerIconDisplay.getText() $= ""))
      {
         $SelectedTower.includeInGame = false;
         TowerIncludeInGameCheckBox.setStateOn(false);
      }
      
      TowerIconFrameEditBox.onValidate();
   }
   
   switch$ ($SelectedTower.getName())
   {
      case "Tower1":
         TowerSlot1Icon.setImageMap(TowerIconDisplay.getText());
         AddAssetToLevelDatablocks(TowerIconDisplay.getText());
         TowerSlot1Icon.setFrame(TowerIconFrameEditBox.getValue());
      
      case "Tower2":
         TowerSlot2Icon.setImageMap(TowerIconDisplay.getText());
         AddAssetToLevelDatablocks(TowerIconDisplay.getText());
         TowerSlot2Icon.setFrame(TowerIconFrameEditBox.getValue());         
      
      case "Tower3":
         TowerSlot3Icon.setImageMap(TowerIconDisplay.getText());
         AddAssetToLevelDatablocks(TowerIconDisplay.getText());
         TowerSlot3Icon.setFrame(TowerIconFrameEditBox.getValue());         
      
      case "Tower4":
         TowerSlot4Icon.setImageMap(TowerIconDisplay.getText());
         AddAssetToLevelDatablocks(TowerIconDisplay.getText());
         TowerSlot4Icon.setFrame(TowerIconFrameEditBox.getValue());
      
      case "Tower5":
         TowerSlot5Icon.setImageMap(TowerIconDisplay.getText());
         AddAssetToLevelDatablocks(TowerIconDisplay.getText());
         TowerSlot5Icon.setFrame(TowerIconFrameEditBox.getValue());
   }
   
   if (!$SelectedTower.includeInGame)
   {
      switch$ ($SelectedTower.getName())
      {
         case "Tower1":
            Tower1Upgrade1.includeInGame = false;
            Tower1Upgrade2.includeInGame = false;
         case "Tower1Upgrade1":
            Tower1Upgrade2.includeInGame = false;
         case "Tower2":
            Tower2Upgrade1.includeInGame = false;
            Tower2Upgrade2.includeInGame = false;
         case "Tower2Upgrade1":
            Tower2Upgrade2.includeInGame = false;
         case "Tower3":
            Tower3Upgrade1.includeInGame = false;
            Tower3Upgrade2.includeInGame = false;
         case "Tower3Upgrade1":
            Tower3Upgrade2.includeInGame = false;
         case "Tower4":
            Tower4Upgrade1.includeInGame = false;
            Tower4Upgrade2.includeInGame = false;
         case "Tower4Upgrade1":
            Tower4Upgrade2.includeInGame = false;
         case "Tower5":
            Tower5Upgrade1.includeInGame = false;
            Tower5Upgrade2.includeInGame = false;
         case "Tower5Upgrade1":
            Tower5Upgrade2.includeInGame = false;
      }
   }
   
   if ($SelectedTower.includeInGame)
   {
      switch$ ($SelectedTower.getName())
      {
         case "Tower1Upgrade2":
            Tower1Upgrade1.includeInGame = true;
            Tower1.includeInGame = true;
         case "Tower1Upgrade1":
            Tower1.includeInGame = true;
         case "Tower2Upgrade2":
            Tower2Upgrade1.includeInGame = true;
            Tower2.includeInGame = true;
         case "Tower2Upgrade1":
            Tower2.includeInGame = true;
         case "Tower3Upgrade2":
            Tower3Upgrade1.includeInGame = true;
            Tower3.includeInGame = true;
         case "Tower3Upgrade1":
            Tower3.includeInGame = true;
         case "Tower4Upgrade2":
            Tower4Upgrade1.includeInGame = true;
            Tower4.includeInGame = true;
         case "Tower4Upgrade1":
            Tower4.includeInGame = true;
         case "Tower5Upgrade2":
            Tower5Upgrade1.includeInGame = true;
            Tower5.includeInGame = true;
         case "Tower5Upgrade1":
            Tower5.includeInGame = true;
      }
   }
   
   TowerSlot1Icon.setEnabled(Tower1.includeInGame);
   TowerSlot2Icon.setEnabled(Tower2.includeInGame);
   TowerSlot3Icon.setEnabled(Tower3.includeInGame);
   TowerSlot4Icon.setEnabled(Tower4.includeInGame);
   TowerSlot5Icon.setEnabled(Tower5.includeInGame);
   
   AddAssetToLevelDatablocks(TowerSpriteDisplay.getText());
   
   LDEonApply();
   SaveAllLevelDatablocks();
   LBProjectObj.saveLevel();
   
   SetTowerToolDirtyState(false);
   
   TowerSelectedDropdown.refresh();
   TowerSelectedDropdown.setSelected(TowerSelectedDropdown.findText($SelectedTower.getInternalName()), false);
   
   
   return true;
}

/// <summary>
/// This function handles the tool's dirty state to keep track of the need to save data.
/// </summary>
/// <param name="dirty">True if data has been changed, false if not.</param>
function SetTowerToolDirtyState(%dirty)
{
   $Tools::TemplateToolDirty = %dirty;
   
   if ($Tools::TemplateToolDirty)
      TowerToolWindow.setText("Tower Tool *");
   else
      TowerToolWindow.setText("Tower Tool");
}


//--------------------------------
// Tower Close
//--------------------------------

/// <summary>
/// This function prompts the user to save any changes if needed before closing the tool.
/// </summary>
function TowerCloseButton::onClick(%this)
{
   ValidateTowerInfo();
   
   if ($Tools::TemplateToolDirty)
   {
      ConfirmDialog.setupAndShow(TowerToolWindow.getGlobalCenter(), "Save Tower Changes?", 
            "Save", "if (SaveTowerData()) { Canvas.popDialog(TowerTool); }", 
            "Don't Save", "SetTowerToolDirtyState(false); Canvas.popDialog(TowerTool);", 
            "Cancel", "");
   }
   else
      Canvas.popDialog(TowerTool);
}


//--------------------------------
// Tower Miscellaneous
//--------------------------------

/// <summary>
/// This function handles retrieving the data for the currently selected tower.
/// </summary>
function RefreshTowerData()
{
   TowerIncludeInGameCheckBox.refresh();
   TowerNameEditBox.refresh();
   TowerRangeDropdown.refresh();
   TowerRateOfFireDropdown.refresh();
   TowerCostEditBox.refresh();
   TowerSellValueEditBox.refresh();
   TowerProjectileDropdown.refresh();
   TowerFireSoundDisplay.refresh();
   
   if ($SelectedTower.getClassName() $= "t2dStaticSprite")
   {
      $TowerSpriteRadioSelection = "Static";
      TowerStaticRadioButton.setStateOn(true);
      ToggleTowerStaticSpriteControls(true);
   }
   else
   {
      $TowerSpriteRadioSelection = "Animated";
      TowerAnimatedRadioButton.setStateOn(true);
      ToggleTowerStaticSpriteControls(false);
   }
      
   TowerSpriteDisplay.refresh();
   TowerIconDisplay.refresh();
   
   TowerPreviewDropdown.refresh();
   
   TowerStaticSpriteFrameEditBox.refresh();
}

//function DeleteSelectedTower()
//{
   //$SelectedTower.includeInGame = false;
   //$SelectedTower.setInternalName($SelectedTower.getName());
   //
   //LBProjectObj.saveLevel();
   //
   //SetTowerToolDirtyState(false);
   //
   //TowerSelectedDropdown.refresh();
//}

/// <summary>
/// This function handles retrieving the selected tower's projectile data.
/// </summary>
function RefreshTowerProjectileData()
{
   TowerProjectileDamageDisplay.refresh();
   TowerProjectileEffectDisplay.refresh();
}

/// <summary>
/// This function ensures that the data for the selected tower is valid.
/// </summary>
function ValidateTowerInfo()
{
   TowerNameEditBox.onValidate();
   TowerCostEditBox.onValidate();
   TowerSellValueEditBox.onValidate();
   
   if ($TowerSpriteRadioSelection $= "Static")
      TowerStaticSpriteFrameEditBox.onValidate();
      
   if (TowerIconContainer.isVisible())
      TowerIconFrameEditBox.onValidate();
}

/// <summary>
/// This function plays/pauses the preview animation.
/// </summary>
function TowerPreviewPlayButton::onClick(%this)
{
    %anim = TowerPreview.sprite.getAnimation();
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
/// This function handles playing/pausing looping animations.
/// </summary>
function TowerPreviewPlayButton::loopClick(%this)
{
    if (TowerPreview.sprite.paused)
    {
        %this.setBitmap("templates/commonInterface/gui/images/pauseButton.png");
        TowerPreview.play();
    }
    else
    {
        %this.setBitmap("templates/commonInterface/gui/images/playButton.png");
        TowerPreview.pause();
    }
}

/// <summary>
/// This function handles playing/pausing non-looping animations.
/// </summary>
function TowerPreviewPlayButton::playOnceClick(%this)
{
    if (TowerPreview.sprite.getIsAnimationFinished())
        TowerPreviewDropdown.onSelect();

    if (TowerPreview.sprite.paused)
    {
        %this.setBitmap("templates/commonInterface/gui/images/pauseButton.png");
        TowerPreview.play();
        %anim = TowerPreview.sprite.getAnimation();
        %frames = getWordCount(%anim.animationFrames);
        %currentFrame = TowerPreview.sprite.getAnimationFrame();
        %remainingFrames = %frames - %currentFrame;
        %frameTime = %anim.animationTime / %frames;
        %playTime = %remainingFrames * %frameTime * 1000;
        %this.resetSchedule = %this.schedule(%playTime, resetBitmap, "templates/commonInterface/gui/images/playButton.png");
    }
    else
    {
        %this.setBitmap("templates/commonInterface/gui/images/playButton.png");
        cancel(%this.resetSchedule);
        TowerPreview.pause();
    }
}

/// <summary>
/// This function sets the play/pause button's state image to the desired image.
/// </summary>
/// <param name="imageFile">The image to set the play/pause button to.</param>
function TowerPreviewPlayButton::resetBitmap(%this, %imageFile)
{
    %this.setBitmap(%imageFile);
    if (TowerPreview.sprite.getIsAnimationFinished())
        TowerPreviewDropdown.onSelect();
    TowerPreview.sprite.setAnimationFrame(0);
    TowerPreview.pause();
}
