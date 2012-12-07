//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

$NextWaveCountDownStart = 0;
$waveEnemyCountTemp = 0;
$waveStartDelay = "";
$waveTotalWaveTime = "";
$waveTotalDelayTime = "";

/// <summary>
/// The WaveControllerBehavior manages lists of WaveBehaviors.
/// </summary>
if(!isObject(WaveControllerBehavior))
{
   %template = new BehaviorTemplate(WaveControllerBehavior);

   %template.friendlyName = "Wave Controller Behavior";
   %template.behaviorType = "Global";
   %template.description = "Starts a list of spawn areas at a specified time";

   %template.addBehaviorField(levelLoadDelay, "The start spawning (seconds) after the level starts.", int, 1);

    // in the current implementation these fields are not used.  In the interest of greater flexibility
    // this data was moved to dynamic fields stored on the wave controller object.
   %template.addBehaviorField(wave1, "The first wave in the wave sequence.", object, "", SceneObject);
   %template.addBehaviorField(wave2, "The second wave in the wave sequence.", object, "", SceneObject);
   %template.addBehaviorField(wave3, "The third wave in the wave sequence.", object, "", SceneObject);
   %template.addBehaviorField(wave4, "The fourth wave in the wave sequence.", object, "", SceneObject);
   %template.addBehaviorField(wave5, "The fifth wave in the wave sequence.", object, "", SceneObject);
   %template.addBehaviorField(wave6, "The sixth wave in the wave sequence.", object, "", SceneObject);
   %template.addBehaviorField(wave7, "The seventh wave in the wave sequence.", object, "", SceneObject);
   %template.addBehaviorField(wave8, "The eighth wave in the wave sequence.", object, "", SceneObject);
   %template.addBehaviorField(wave9, "The ninth wave in the wave sequence.", object, "", SceneObject);
   %template.addBehaviorField(wave10, "The tenth wave in the wave sequence.", object, "", SceneObject);
   %template.addBehaviorField(ScorekeeperObject, "The scene object that has the scorekeeping behavior on it.", object, "GlobalBehaviorObject", SceneObject);
   %template.addBehaviorField(AdvanceBonus, "Bonus points for advancing a wave early", int , 500);
}

/// <summary>
/// This function handles basic setup tasks.
/// </summary>
/// <param name="scene">The scene that this behavior instance is attached to.</param>
function WaveControllerBehavior::onAdd(%this, %scene)
{
    %this.createWaveList();
}

/// <summary>
/// This function handles basic setup tasks.
/// </summary>
function WaveControllerBehavior::createWaveList(%this)
{
    for (%i = 0; %i < %this.owner.waveCount; %i++)
        %this.waveArray[%i] = "";
        
   %this.owner.waveCount = 0;
   %this.waveArray[0] = "";   
   
   %i = 0;
   while ((%wave = %this.getWaveField(%i)) !$= "")
   {
      %this.waveArray[%i] = %wave;
      //echo(" -- WaveControllerBehavior::onAdd() - " @ %this.waveArray[%i] @ ", " @ %this.owner.waveCount);
      %i++;
      %this.owner.waveCount++;
   }
}

/// <summary>
/// This function retrieves the data in the indicated field on the wave controller
/// object.
/// </summary>
/// <param name="index">The index of the desired wave to access.</param>
function WaveControllerBehavior::getWaveField(%this, %index)
{
   if (%index < 10)
   {
      eval("return %this.wave" @ %index + 1 @ ";");
   }
   else
   {
      eval("return %this.owner.extraWave[" @ %index @ "];");
   }
}

/// <summary>
/// This function recounts the waves attached to the wave controller object.
/// </summary>
function WaveControllerBehavior::adjustCount(%this)
{
   %i = 0;
   while (%this.waveArray[%i] !$= "")
   {
      %this.owner.waveCount++;
   }

   //echo(" -- WaveControllerBehavior::adjustCount() - " @ %this.owner.waveCount);
}

/// <summary>
/// This function handles some initialization tasks
/// </summary>
/// <param name="scene">The scene to which this behavior instance is attached.</param>
function WaveControllerBehavior::onAddToScene(%this, %scene)
{
   %this.currentWaveCount = 0;

   %this.advance = false;
   %this.manual = false;

   %this.controllerEnemyCount = 0;

   if (!$LevelEditorActive)
      ScheduleManager.scheduleEvent(1000, %this, "WaveControllerBehavior::initialize", %this);
}

/// <summary>
/// This function is used by the Wave Tool to set the contents of a specific wave slot on the
/// wave controller object.
/// </summary>
/// <param name="wave">The SimObject name of the wave object to use.</param>
/// <param name="index">The spawn order position to assign the wave object to.</param>
function WaveControllerBehavior::setWave(%this, %wave, %index)
{
   //echo(" -- WaveControllerBehavior::setWave() - " @ %wave);
   %wavebehavior = %wave.getBehavior("WaveBehavior");
   if (!isObject(%wavebehavior))
      return false;

   if (%index >= 0 && %index < %this.owner.waveCount)
   {
      eval("%this.wave" @ %index + 1 @ " = " @ %wave.getName() @ ";");
      %this.createWaveList();
      return true;
   }
   // index out of bounds
   return false;
}

/// <summary>
/// This function is used by the Wave Tool to add a wave to the end of the wave controller 
/// spawn list.
/// </summary>
/// <param name="wave">The SimObject name of the wave object to add to the list.</param>
function WaveControllerBehavior::addWave(%this, %wave)
{
    //echo(" -- WaveControllerBehavior::addWave() - " @ %wave);
    %wavebehavior = %wave.getBehavior("WaveBehavior");
    if (!isObject(%wavebehavior))
        return false;

    if (%this.owner.waveCount < 10)
    {
        eval("%this.wave" @ %this.owner.waveCount + 1 @ " = " @ %wave.getName() @ ";");
        %this.owner.extraWave[%this.owner.waveCount] = %wave.getName();
    }
    else
    {
        %this.owner.extraWave[%this.owner.waveCount] = %wave.getName();
    }
    %this.owner.waveCount++;
    %this.createWaveList();
}

/// <summary>
/// This function is used by the Wave Tool to remove a specific wave from the wave controller's 
/// wave list.  It searches for the wave in the controller's list, then removes it and compacts 
/// the list to eliminate blanks.
/// </summary>
/// <param name="wave">The SimObject name of the wave to remove from the list.</param>
function WaveControllerBehavior::removeWave(%this, %wave)
{
    // remove wave from controller's list.
    %count = 0;
    %temp = %this.owner.extraWave[%count];
    while (%temp !$= %wave)
    {
        %count++;
        %temp = %this.owner.extraWave[%count];
    }
    %this.owner.extraWave[%count] = "";
    
    %k = 0;
    %cleanList[%k] = "";
    for (%i = 0; %i < %this.owner.waveCount; %i++)
    {
        %temp = %this.owner.extraWave[%i];
        if (%temp !$= "")
        {
            %cleanList[%k] = %temp;
            %k++;
        }
    }
    %wavecount = 0;
    for (%i = 0; %i < %this.owner.waveCount; %i++)
    {
        %tempfield = %this.wave @ (%i + 1);
        if (%cleanList[%i] !$= "")
        {
            %this.owner.extraWave[%i] = %cleanList[%i];
            %tempfield = %cleanList[%i];
            %wavecount++;
        }
        else
        {
            %this.owner.extraWave[%i] = "";
            eval("%this.wave"@(%i+1)@" = \"\";");
        }
    }
    %this.owner.waveCount = %wavecount;
}

/// <summary>
/// Used to get the total number of waves in the level.
/// </summary>
/// <return>Returns the number of waves attached to the controller.</return>
function WaveControllerBehavior::getWaveCount(%this)
{
   return %this.owner.waveCount;
}

/// <summary>
/// Gets the SimObject name of the wave at the specified index.
/// </summary>
/// <param name="index">The index of the desired wave.</param>
/// <return>Returns the SimObject name of the wave at the specified index.</return>
function WaveControllerBehavior::getWave(%this, %index)
{
    return %this.owner.extraWave[%index];   
}

/// <summary>
/// Checks to see if the wave exists within the wave controller's list.
/// </summary>
/// <param name="wave">The SimObject name of the desired wave.</param>
/// <return>Returns true if the wave is in the list, false otherwise.</return>
function WaveControllerBehavior::waveExists(%this, %wave)
{
    %found = false;
    for (%i = 0; %i < %this.owner.waveCount; %i++)
    {
        if (%this.owner.extraWave[%i] $= %wave)
            %found = true;
    }
    return %found;
}

/// <summary>
/// This function is used by the Wave Tool to set the wave at a specified index in the 
/// wave controller's list.
/// </summary>
/// <param name="wave">The SimObject name of the wave to set.</param>
/// <param name="index">The index to set the wave to.</param>
function WaveControllerBehavior::writeWave(%this, %wave, %index)
{
   //echo(" -- WaveControllerBehavior::writeWave() - " @ %wave);
   if (%index < 10)
      eval("%this.wave[" @ %index + 1 @ "] = " @ %wave.getName() @ ";");
   else
   {
      %test = %this.waveArray[%index];
      if (%test !$= "")
      {
         %this.waveArray[%index] = %wave.getName();
         eval("%this.wave[" @ %index + 1 @ "] = " @ %wave.getName() @ ";");
         eval("%this.owner.extraWave[" @ %index @ "] = " @ %wave.getName() @ ";");
      }
      else
      {
         %this.addWave(%wave);
      }
   }
}

/// <summary>
/// Initialize handles setting up the wave schedule and reporting the total number of 
/// enemies in all waves to the scorekeeper.
/// </summary>
function WaveControllerBehavior::initialize(%this)
{
    %this.createWaveList();
    %this.controllerEnemyCount = 0;

    for (%i = 0; %i < %this.owner.waveCount; %i++)
    {
        if (%this.waveArray[%i].getName() !$= "" && %this.waveArray[%i] !$= "None")
        {
            %numEnemies = %this.waveArray[%i].callOnBehaviors("getWaveEnemyCount");
            %this.controllerEnemyCount += %numEnemies;

            %totalDelay = %this.waveArray[%i].callOnBehaviors("getTotalDelayTime");
            %spawnDelay = %this.waveArray[%i].callOnBehaviors("getTotalWaveTime");
            
            %this.lockoutArray[%i] = (%totalDelay > %spawnDelay ? %totalDelay : %spawnDelay);
        }
    }
    %this.reportEnemyTotal();

    UpdateWaveTotal(%this.owner.waveCount);
    $WaveTotal = %this.owner.waveCount;

    %this.nextScheduledWave = ScheduleManager.scheduleEvent((%this.levelLoadDelay - 1) * 1000, %this, "WaveControllerBehavior::startWave", %this);
}

/// <summary>
/// This function is used by the Wave Tool to clear the entire contents of the wave 
/// controller.
/// </summary>
function WaveControllerBehavior::clear(%this)
{
    for (%i = 0; %i < %this.owner.waveCount; %i++)
    {
        %this.waveArray[%i] = "";
        eval("%this.wave[" @ %i + 1 @ "] = \"\";");
        %this.owner.extraWave[%i] = "";
    }
    %this.createWaveList();
}

/// <summary>
/// This function reports the total number of enemies assigned to the waves in the 
/// controller's list to the scorekeeper.
/// </summary>
function WaveControllerBehavior::reportEnemyTotal(%this)
{
   if (isObject(%this.ScorekeeperObject))
      %this.ScorekeeperObject.callOnBehaviors(setEnemyTotal, %this.controllerEnemyCount);
}

/// <summary>
/// This function schedules the start of the first wave in the list.  Once it starts, it also
/// increments the current wave and schedules another call to itself.  It also sets the manual 
/// start button lockout.
/// </summary>
function WaveControllerBehavior::startWave(%this)
{
   %paused = MainScene.getScenePause();
   if (%paused)
      return;

   if (%this.currentWaveCount > %this.owner.waveCount)
   {
      echo(" ** Wave cycle is empty");
      return;
   }

   %currentWaveTime = 0;
   %waveDelay = 0;
   if (%this.waveArray[%this.currentWaveCount] !$= "" && %this.waveArray[%this.currentWaveCount] !$= "None")
   {
      %waveDelay = %this.waveArray[%this.currentWaveCount].callOnBehaviors(getStartDelay);
      
      if (!%waveDelay && !%this.advance)
      {
         echo(" ** Wave " @ %this.currentWaveCount @ " must be manually started to continue spawn sequence.");
         %this.manual = true;
         BeginNextWaveCountdown(-1);
         
         // Flash the Start Next Wave Button when a Manual Start is Required
         startNextWaveButton.beginFlash();
         
         return;
      }

      %currentWaveTime = %this.waveArray[%this.currentWaveCount].callOnBehaviors(getTotalWaveTime);

      %this.waveArray[%this.currentWaveCount].callOnBehaviors(spawn);

      //echo(" -- WaveControllerBehavior::startWave() spawning wave " @ %this.currentWaveCount);
   }
   else
   {
      //echo(" ** Wave " @ %this.currentWaveCount @ " is empty - scheduling next wave");
   }

   // Schedule the next wave
   %nextWave = "";
   %next = %this.currentWaveCount + 1;
   if (%next < %this.owner.waveCount)
   {
      if (%this.waveArray[%next] !$= "" && %this.waveArray[%next] !$= "None")
      {
         %nextWave = %this.waveArray[%next].callOnBehaviors(getStartDelay);
      }
      
      if (%nextWave $= "")
      {
         %this.advance = false;
         %this.manual = true;
         %delay = -1;
      }
      else
      {
         %delay = %this.waveArray[%next].callOnBehaviors(getTotalDelayTime);
         %this.manual = false;
      }
      
      %this.advanceTimeout = %this.lockoutArray[%this.currentWaveCount] * 1000;
      //echo(" -- WaveControllerBehavior::startWave() scheduling wave " @ %next @ " in " @ %delay @ " seconds.");
      //echo(" -- WaveControllerBehavior::startWave() advanceTimeout " @ %this.advanceTimeout @ " milliseconds.");
      %this.nextScheduledWave = ScheduleManager.scheduleEvent((%delay < 0 ? 0 : %delay) * 1000, %this, "WaveControllerBehavior::startWave", %this);
      
      if (!%this.manual)
         %this.setupLockout(%this.advanceTimeout);
         //%this.updateAdvanceWaveAvailabilityEvent = ScheduleManager.scheduleEvent(100, %this, "WaveControllerBehavior::updateAdvanceWaveAvailability", %this);
      
      $NextWaveCountDownStart = %delay;
      BeginNextWaveCountdown(%delay);
   }
   else
   {
      echo(" ** No waves remaining in cycle");
      $NextWaveCountDownStart = -1;
      BeginNextWaveCountdown(-1);
      startNextWaveButton.setAvailability(false);
   }

   // Increment the wave count
   %this.currentWaveCount++;
}

/// <summary>
/// This function cancels the next scheduled wave.  Used to cancel the schedule when 
/// manually starting a wave.
/// </summary>
function WaveControllerBehavior::cancelWave(%this)
{
   ScheduleManager.cancelEvent(%this.nextScheduledWave);
}

/// <summary>
/// This was used to set the delay time for the lockout, but now simply sets up the 
/// next wave button's enabled state.
/// </summary>
/// <param name="time">The duration of the lockout timer.</param>
function WaveControllerBehavior::setupLockout(%this, %time)
{
    if (%time < 0)
        %time = 0;
    startNextWaveButton.setAvailability(false);
}

/// <summary>
/// This function is called from the WaveBehavior to enable the manual wave advance button.  
/// If this is the final wave the button won't be enabled.
/// </summary>
function WaveControllerBehavior::updateAdvanceWaveAvailability(%this)
{
    ScheduleManager.cancelEvent(%this.updateAdvanceWaveAvailabilityEvent);
    if (%this.currentWaveCount < %this.owner.waveCount)
        startNextWaveButton.setAvailability(true);
}

/// <summary>
/// This function handles manually advancing the wave progression.
/// </summary>
function WaveControllerBehavior::advanceWave(%this)
{
   if (%this.currentWaveCount > %this.owner.waveCount)
   {
      echo(" ** Wave cycle is empty");
      return;
   }

   if (!startNextWaveButton.Available)
   {
      //echo(" -- Advance Wave locked out until current wave ("@%this.currentWaveCount@") has finished spawning : " @ (%timeRemaining - %this.advanceTimeout));
      return;
   }
   
   //echo(" -- WaveControllerBehavior::advanceWave() spawning wave " @ %this.currentWaveCount @ " now.  Manual = " @ %this.manual);
   %this.cancelWave();
   if (%this.manual == false)
   {
      %this.ScorekeeperObject.callOnBehaviors(updateScore, %this.AdvanceBonus, 0);
      %this.lockoutArray[%this.currentWaveCount] = %this.waveArray[%this.currentWaveCount].callOnBehaviors("getTotalWaveTime");
      //echo(" ** Scorekeeper called with " @ %this.AdvanceBonus @ " points");
   }
   
   %this.advance = true;
   %this.manual = false;
   
   ScheduleManager.scheduleEvent(0, %this, "WaveControllerBehavior::startWave", %this);
}