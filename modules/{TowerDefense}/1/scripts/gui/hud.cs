//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

$BeginNextWaveCountdownSchedule = 0;
$OnCancelTower = false;
$SelectedTower = "";
$SellingTower = false;
$UpgradingTower = false;
$TowerBeingPlaced = "";
$CellWidth = 1;
$CellHeight = 1;

/// <summary>
/// Handle updating the player lives display.  Also, serves as a backup to the 
/// scorekeeper behavior to ensure that the lose screen is shown.
/// </summary>
/// <param name="lives">The current number of lives to display.</param>
function UpdateLives(%lives)
{
   $Lives = %lives;
   
   if ($Lives < 0)
      $Lives = 0;
   
   livesBitmapFont.setText($Lives);
   
   // You Lose
   if ($Lives == 0)
   {
      MainScene.gameOver = true;
      Canvas.pushDialog(loseGui);
   }
}

/// <summary>
/// Handle updating the wave total display.
/// </summary>
/// <param name="waveTotal">The total number of waves in the current level.</param>
function UpdateWaveTotal(%waveTotal)
{
   $WaveTotal = %waveTotal;
   
   if (%waveTotal < 10)
      %waveTotal = 0 @ %waveTotal;
   
   waveTotalBitmapFont.setText(%waveTotal);
}

/// <summary>
/// Handle updating the current wave display
/// </summary>
/// <param name="currentWave">The number of the current wave being spawned.</param>
function UpdateCurrentWave(%currentWave)
{
   $CurrentWave = %currentWave;

   currentWaveBitmapFont.setText(%currentWave);
}

/// <summary>
/// Starts the countdown to the next wave start time.
/// </summary>
/// <param name="waveDelay">The time until the next wave starts to spawn enemies.</param>
function BeginNextWaveCountdown(%waveDelay)
{
   if (%waveDelay == -1)
   {
      waveCountdownBitmapFont.setText("-:--");
      
      if ($BeginNextWaveCountdownSchedule)
         cancel($BeginNextWaveCountdownSchedule);
      
      return;
   }
   
   %waveDelayMinutes = mFloor(%waveDelay / 60);
    
   %waveDelaySeconds = %waveDelay % 60;
   if (%waveDelaySeconds < 10)
      %waveDelaySeconds = 0 @ %waveDelaySeconds;
   
   waveCountdownBitmapFont.setText(%waveDelayMinutes @ ":" @ %waveDelaySeconds);
   
   if (%waveDelay > 0)
   {
      if ($BeginNextWaveCountdownSchedule)
         cancel($BeginNextWaveCountdownSchedule);
      
      $BeginNextWaveCountdownSchedule = ScheduleManager.scheduleEvent(1000, 0, "BeginNextWaveCountdown", %waveDelay - 1);
      //$BeginNextWaveCountdownSchedule = schedule(1000, 0, beginNextWaveCountdown, %waveDelay - 1);
   }
}

/// <summary>
/// Handle updating the funds display.
/// </summary>
/// <param name="funds">The amount of money the player currently has.</param>
function UpdateFunds(%funds)
{
   $Funds = %funds;

   fundsBitmapFont.setText(%funds);
   
   Tower::UpdatePurchaseAvailability();
   Tower::UpdateUpgradeAvailability();
}

/// <summary>
/// Handle updating the score display.
/// </summary>
/// <param name="score">The player's current score.</param>
function UpdateScore(%score)
{
   $Score = %score;

   scoreBitmapFont.setText(%score);
}

/// <summary>
/// Handle tower purchase availability indication.  Based on player money, will enable or disable towers
/// depending on whether or not the player can afford them.
/// </summary>
function Tower::UpdatePurchaseAvailability(%this)
{
   %scene = sceneWindow2D.getScene();
   
   if (Tower1.includeInGame)
      TowerSlot1Icon.setBlendColor(1.0, 1.0, 1.0, (Tower1.callOnBehaviors(getCost) > $Funds) ? 0.25 : 1.0);
      
   if (Tower2.includeInGame)
      TowerSlot2Icon.setBlendColor(1.0, 1.0, 1.0, (Tower2.callOnBehaviors(getCost) > $Funds) ? 0.25 : 1.0);
      
   if (Tower3.includeInGame)
      TowerSlot3Icon.setBlendColor(1.0, 1.0, 1.0, (Tower3.callOnBehaviors(getCost) > $Funds) ? 0.25 : 1.0);
      
   if (Tower4.includeInGame)
      TowerSlot4Icon.setBlendColor(1.0, 1.0, 1.0, (Tower4.callOnBehaviors(getCost) > $Funds) ? 0.25 : 1.0);
      
   if (Tower5.includeInGame)
      TowerSlot5Icon.setBlendColor(1.0, 1.0, 1.0, (Tower5.callOnBehaviors(getCost) > $Funds) ? 0.25 : 1.0);
}

/// <summary>
/// Handle the onButtonDown callback.  This function creates a new tower to be placed on 
/// the playing field.
/// </summary>
/// <param name="worldPos">The world position of the button down event.</param>
/// <param name="touchID">The touch ID associated with the button down event.</param>
function TowerButton::onButtonDown(%this, %worldPos, %touchID)
{
   if (isObject($TowerBeingPlaced))
      return;
      
   %desiredTower = -1;
   
   switch$ (%this.getName())
   {
      case TowerSlot1Icon:
         %desiredTower = Tower1;
         
      case TowerSlot2Icon:
         %desiredTower = Tower2;
         
      case TowerSlot3Icon:
         %desiredTower = Tower3;
         
      case TowerSlot4Icon:
         %desiredTower = Tower4;
         
      case TowerSlot5Icon:
         %desiredTower = Tower5;
   }
   
   if (%desiredTower.callOnBehaviors(getCost) > $Funds)
      return;
      
   // Clear tower selection
   ClearRangesBehavior.onTouchDown();
      
   CreateTower(%desiredTower, %this, %touchID);
   
   cancelTower.Visible = true;
}

/// <summary>
/// Handle indicating if a tower upgrade is available based on player funds.
/// </summary>
function Tower::UpdateUpgradeAvailability(%this)
{
   if (!$SelectedTower)
      return;

   upgradeButton.setBlendColor(1.0, 1.0, 1.0, ($SelectedTower.callOnBehaviors(getUpgradeCost) > $Funds) ? 0.25 : 1.0);
}

/// <summary>
/// This function displays the selected tower's available option buttons
/// </summary>
/// <param name="selectedTower">The tower that the player selected.</param>
function ShowTowerOptions(%selectedTower)
{
   $SelectedTower = %selectedTower;
    
   %x = ($SelectedTower.getPositionX() + ($CellWidth / 2)) + (sellButton.getWidth() / 2);
   %y = ($SelectedTower.getPositionY() + ($CellHeight / 2)) + (sellButton.getHeight() / 2);
   sellButton.setPosition(%x SPC %y);
      
   sellButton.setVisible(true);
      
   %upgrade = $SelectedTower.callOnBehaviors("getUpgrade");
   
   if (!isObject(%upgrade) || !%upgrade.includeInGame)
      return;

   %x = ($SelectedTower.getPositionX() - ($CellWidth / 2)) - (sellButton.getWidth() / 2);
   %y = ($SelectedTower.getPositionY() + ($CellHeight / 2)) + (sellButton.getHeight() / 2);
   upgradeButton.setPosition(%x SPC %y);

   Tower::UpdateUpgradeAvailability();

   upgradeButton.setVisible(true);
} 

/// <summary>
/// This function hides all of the tower option buttons.
/// </summary>
function HideTowerOptions()
{
   if ($SelectedTower)
   {
      if (sellButton.getVisible())      
         sellButton.setVisible(false);

      if (upgradeButton.getVisible())
         upgradeButton.setVisible(false);
         
      if (confirmButton.getVisible())      
         confirmButton.setVisible(false);
         
      if (towerCancelButton.getVisible())      
         towerCancelButton.setVisible(false);
         
      $SellingTower = false;
      $UpgradingTower = false;
      
      $SelectedTower = "";
   }
}

/// <summary>
/// Handles the cancel button onTouchEnter callback to prepare to delete the tower 
/// that is currently being placed.
/// </summary>
function cancelTower::onTouchEnter(%this)
{
   $OnCancelTower = true;
}

/// <summary>
/// Handles the cancel button onTouchLeave callback to prepare the tower that is being
/// placed to actually be placed.
/// </summary>
function cancelTower::onTouchLeave(%this)
{
   $OnCancelTower = false;
}

/// <summary>
/// Handles cleanup from the tower drag behavior and range behavior
/// </summary>
function cancelTower::onButtonUp(%this)
{
   if (!isObject($TowerBeingPlaced))
      return;
   
   %rangeBeh = $TowerBeingPlaced.getBehavior("toggleShowRangeOnTouchDownBehavior");
   $RangeBehaviorCollection.remove(%rangeBeh);
   %rangeBeh.delete();
   
   $TowerBeingPlaced.safeDelete();
   //TowerPlaceOverlay.visible = false;
   
   cancelTower.Visible = false;
   
   $TowerBeingPlaced = "";   
   
   $OnCancelTower = false;
}

/// <summary>
/// This function prepares to sell a tower from the tower option buttons
/// </summary>
function sellButton::onButtonDown(%this)
{
   %position = sellButton.getPosition();
   %x = getWord(%position, 0);
   %y = getWord(%position, 1);
   confirmButton.setPosition(%x SPC %y);

   %x = ($SelectedTower.getPositionX() - ($CellWidth / 2)) - (sellButton.getWidth() / 2);
   %y = ($SelectedTower.getPositionY() + ($CellHeight / 2)) + (sellButton.getHeight() / 2);
   towerCancelButton.setPosition(%x, %y);
      
   upgradeButton.setVisible(false);
   sellButton.setVisible(false);
   
   confirmButton.setVisible(true);
   towerCancelButton.setVisible(true);
   
   $SellingTower = true;
   
   $TouchHandled = true;
}

/// <summary>
/// This function prepares to upgrade a tower from the tower option buttons
/// </summary>
function upgradeButton::onButtonDown(%this)
{
   $TouchHandled = true;
   
   if (%this.getBlendAlpha() != 1)
      return;

   %position = upgradeButton.getPosition();
   %x = getWord(%position, 0);
   %y = getWord(%position, 1);
   confirmButton.setPosition(%x SPC %y);
      
   %position = sellButton.getPosition();
   %x = getWord(%position, 0);
   %y = getWord(%position, 1);
   towerCancelButton.setPosition(%x, %y);
      
   sellButton.setVisible(false);
   upgradeButton.setVisible(false);
   
   confirmButton.setVisible(true);
   towerCancelButton.setVisible(true);   
   
   $UpgradingTower = true;
}

/// <summary>
/// This function handles completing the sale or upgrade of a tower from the tower
/// option buttons.
/// </summary>
function confirmButton::onButtonDown(%this)
{
   if ($SellingTower)
   {
      $SelectedTower.callOnBehaviors(sell);
      
      %tile = $TowerPlaceGrid.getCellFromWorldPosition($SelectedTower.centerPoint);
      %tx = getWord(%tile, 0);
      %ty = getWord(%tile, 1);
      $TowerPlaceGrid.setCellWeight(%tx, %ty, 1);

      DeleteSelectedTower();
      
      HideTowerOptions();
      rangeCircle.Visible = false;
      $SellSound = alxPlay($TowerSellSound);
      
      $SellingTower = false;
   }
   else
   {
      %newTower = $SelectedTower.callOnBehaviors("getUpgrade");
      
      %upgradeSound = $SelectedTower.callOnBehaviors("getUpgradeSound");

      %towerUpgrade = %newTower.clone();
      %towerUpgrade.slot = %newTower;

      %tile = $TowerPlaceGrid.getCellFromWorldPosition($SelectedTower.centerPoint);
      %tx = getWord(%tile, 0);
      %ty = getWord(%tile, 1);
      %pos = $TowerPlaceGrid.getCellWorldPosition(%tx, %ty);
      
      %numTiles = $TowerPlaceGrid.getCellCount();
      %numTilesY = getWord(%numTiles, 1);

      %towerUpgrade.yOffset = calculateYOffset(%towerUpgrade, $TowerPlaceGrid.getHeight() / %numTilesY);
      %towerUpgrade.centerPoint = %pos;
      %position = Vector2Add(%pos, %towerUpgrade.yOffset);
      %x = getWord(%position, 0);
      %y = getWord(%position, 1);
      %towerUpgrade.setPosition(%x SPC %y);
      %towerUpgrade.BlendColor = $Towers::PlacedColor;
      
      %towerUpgrade.inValidPos = true;
        
      %towerUpgrade.callOnBehaviors(buy);
      
      // Play upgrade sound
      if (%upgradeSound !$= "None" && %upgradeSound !$= "")
      {
         //echo(" -- upgradeButton::onButtonUp() - play sound : " @ %towerUpgradeBehavior.upgradeSound);
         alxPlay(%upgradeSound);
      }
      else if (isObject($TowerUpgradeSound))
          alxPlay($TowerUpgradeSound);

      %towerUpgrade.canFire = true;

      // Activate the tower
      %towerUpgrade.callOnBehaviors(toggle, true);

      DeleteSelectedTower();
      HideTowerOptions();

      $UpgradingTower = false;
   }
   
   towerCancelButton.setVisible(false);
   confirmButton.setVisible(false);
   
   $TouchHandled = true;
}

/// <summary>
/// This function cancels a pending sell or upgrade operation from the tower option buttons.
/// </summary>
function towerCancelButton::onButtonDown(%this)
{
   if ($UpgradingTower)
      $UpgradingTower = false;
   else
      $SellingTower = false;
   
   sellButton.setVisible(true);
   
   confirmButton.setVisible(false);
   towerCancelButton.setVisible(false);
   
   $TouchHandled = true;

   %upgrade = $SelectedTower.callOnBehaviors("getUpgrade");
   
   if (!isObject(%upgrade) || !%upgrade.includeInGame)
      return;
   
   upgradeButton.setVisible(true);
}

/// <summary>
/// Handle setup tasks onLevelLoaded
/// </summary>
/// <param name="sg">The scene that this behavior instance is assigned to.</param>
function DeleteSelectedTower()
{
   %rangeBeh = $SelectedTower.getBehavior("toggleShowRangeOnTouchDownBehavior");
   $RangeBehaviorCollection.remove(%rangeBeh);

   $SelectedTower.safeDelete();
}

/// <summary>
/// This function starts the flashing effect on the advance wave button when it becomes
/// available.
/// </summary>
function startNextWaveButton::beginFlash(%this)
{
   startNextWaveButton.Available = true;
   startNextWaveButton.IsFlashing = true;
   %this.setBlendColor(1.0, 1.0, 1.0, 0.5);
   
   %this.flashEvent = ScheduleManager.scheduleEvent(500, %this, "startNextWaveButton::flash", %this);
}

/// <summary>
/// This function handles "flashing" the advance wave button.
/// </summary>
function startNextWaveButton::flash(%this)
{
   ScheduleManager.cancelEvent(%this.flashEvent);
   
   %this.setBlendColor(1.0, 1.0, 1.0, (getWord(%this.getBlendColor(), 3) == 1.0) ? 0.5 : 1.0);
   
   %this.flashEvent = ScheduleManager.scheduleEvent(500, %this, "startNextWaveButton::flash", %this);
}

/// <summary>
/// This function enables or disables the onButtonUp callback on the advance wave button
/// </summary>
/// <param name="available">True to allow clicking, false to disallow.</param>
function startNextWaveButton::setAvailability(%this, %available)
{
   %this.Available = %available;
   
   %this.setBlendColor(1.0, 1.0, 1.0, %this.Available ? 1.0 : 0.5);
}

/// <summary>
/// This function tells the wave controller to begin spawning the next wave
/// </summary>
function startNextWaveButton::onButtonUp(%this)
{
   if (!startNextWaveButton.Available)
      return;
      
   if (startNextWaveButton.IsFlashing)
   {
      ScheduleManager.cancelEvent(startNextWaveButton.flashEvent);
      %this.setBlendColor(1.0, 1.0, 1.0, 1.0);
      startNextWaveButton.IsFlashing = false;
   }
   
   // Start Wave Early Code Here
   WaveController.callOnBehaviors(advanceWave);
}

/// <summary>
/// This function pauses the game and displays the pause screen.
/// </summary>
function pauseButton::onButtonUp(%this)
{
   // Pause Code Here
   ScheduleManager.setPause(true);
   MainScene.setScenePause(true);
   Canvas.pushDialog(pauseGui);
}

/// <summary>
/// This function resumes the game and hides the pause screen.
/// </summary>
function resumeButton::onButtonUp(%this)
{
   // Resume Code Here
   ScheduleManager.setPause(false);
   MainScene.setScenePause(false);
   Canvas.popDialog(pauseGui);
}

/// <summary>
/// This function creates a tower of the requested type and assigns it to a global.
/// </summary>
/// <param name="type">The SimObject name of the new tower type.</param>
/// <param name="button">The ID of the button that requested the tower.</param>
/// <param name="touchID">The touch ID associated with this input event.</param>
function CreateTower(%type, %button, %touchID)
{
   %newTower = %type.clone();
   MainScene.addToScene(%newTower);
   %newTower.slot = %type;
   
   %newTower.position = sceneWindow2D.getMousePosition();
   
   %newTower.callOnBehaviors("createNewTower", %button, %touchID, %newTower.position);
   
   $TowerBeingPlaced = %newTower;
}