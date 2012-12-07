//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

$levelLoading = false;


/// <summary>
/// This function initializes the main menu GUI
/// </summary>
function mainMenuGui::onWake(%this)
{
   //echo(" -- mainMenuGui::onWake()");
   menuCreditsButton.visible = mainMenuGui.showCreditsButton;

   alxStopAll();

   if (isObject(%this.music))
      $TitleMusicHandle = alxPlay(%this.music);
   if (isObject(%this.buttonSound))
      $ButtonSound = %this.buttonSound;
   if (%this.fontSheet !$= "")
      $FontSheet = %this.fontSheet;
    if (isObject(%this.towerPlace))
        $TowerPlacementSound = %this.towerPlace;
    if (isObject(%this.towerUpgrade))
        $TowerUpgradeSound = %this.towerUpgrade;
    if (isObject(%this.towerSell))
        $TowerSellSound = %this.towerSell;
    if (isObject(%this.towerMisplace))
        $TowerMisplacementSound = %this.towerMisplace;

   GuiDefaultProfile.soundButtonDown = $ButtonSound;
}

/// <summary>
/// This function stops the title music when the user leaves the main menu.
/// </summary>
function mainMenuGui::onSleep(%this)
{
   alxStop($TitleMusicHandle);
}

/// <summary>
/// This function handles the start button by loading and launching the level select GUI.
/// </summary>
function menuStartButton::onClick(%this)
{
   Canvas.pushDialog(levelSelectGui);
   /*
   %level = "level.t2d";
   
   //echo("Setting level to " @ %level);
   $SelectedLevel = %level;
   
   if(!$levelLoading)
   {
      $levelLoading = true;
      
      $TowerBeingPlaced = "";
   
      moveMap.pop();
      
      schedule(100, 0, loadLevel);
   }*/
}

/// <summary>
/// This function shows the help screen GUI.
/// </summary>
function mainMenuHowToPlayButton::onClick(%this)
{
   Canvas.pushDialog(howToPlayGui);
}

/// <summary>
/// This function shows the credits GUI.
/// </summary>
function menuCreditsButton::onClick(%this)
{
   Canvas.pushDialog(creditsGui);
}

/// <summary>
/// This function is deprecated.
/// </summary>
function transitionlevel()
{  
   // May need to repopulate this once we have multiple levels
}

/// <summary>
/// This function loads and launches a level that is selected in the $SelectedLevel global.
/// </summary>
function loadLevel()
{
   // Load the .t2d file for this game
   sceneWindow2D.loadLevel("data/levels/" @ $SelectedLevel);
   $levelLoading = false;
   //$SelectedLevel = "multiTouch.t2d";
   
   if ($SelectedLevel $= "mainMenu.t2d")
      return;
    
   // Initialize Game Variables
   $BeginNextWaveCountdownSchedule = 0;
   $OnCancelTower = false;
   $SelectedTower = "";   
   $SellingTower = false;
   $UpgradingTower = false;
   $TowerBeingPlaced = "";
   
   $WaveTotal = 0;
   $TotalCreepsInLevel = 0;
   
   %waveControllerBehavior = WaveController.getBehavior("WaveControllerBehavior");

   for (%i = 0; %i < 10; %i++)
   {
      %wave = %waveControllerBehavior.waveArray[%i];

      if (!isObject(%wave))
         continue;
         
      %waveBehavior = %wave.getBehavior("WaveBehavior");
      
      if (!isObject(%waveBehavior))
         continue;
         
      $WaveTotal++;
      
      for (%j = 0; %j < 15; %j++)
      {
         if (isObject(%waveBehavior.creepArray[%j]) && 
             (isObject(%waveBehavior.spawnerArray[%j]) || isObject(%waveControllerBehavior.defaultSpawnLocation)))
         {
            $TotalCreepsInLevel++;
         }
      }
   }  
   
   $EnemiesGone = 0;
   
   UpdateWaveTotal($WaveTotal);  
   
   UpdateCurrentWave(0);
      
   UpdateFunds(0);
   UpdateScore(0);
   
   Canvas.popDialog(mainMenuGui);
   Canvas.popDialog(levelSelectGui);
}