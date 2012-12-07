//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

%waveStartAudioProfiles = "None" TAB "WaveAnnouncementSound";

/// <summary>
/// The Wave Behavior handles managing the contents of a wave of enemies.
/// </summary>
if (!isObject(WaveBehavior))
{
   %template = new BehaviorTemplate(WaveBehavior);
   
   %template.friendlyName = "Wave Behavior";
   %template.behaviorType = "Global";
   %template.description  = "Controls the enemys that will spawn in a wave.";

   %template.addBehaviorField(waveSound, "Sound profile played when a wave starts", enum, "", %waveStartAudioProfiles);

   %template.addBehaviorField(manualStart, "This wave must be started manually.  All waves after this will wait for this wave.", bool, 0);
   %template.addBehaviorField(startDelay, "The delay period before spawn start(seconds)", int, 60);
   %template.addBehaviorField(defaultSpawnLocation, "The default path for enemies in this wave.", object, "", Grid);
   %template.addBehaviorField(defaultSpawnInterval, "The period between spawning each enemy(seconds)", float, 1.0);

    // These behavior fields are not used in the current implementation of this behavior.  They were 
    // replaced with direct storage of the wave contents in dynamic fields on the wave object in order
    // to allow greater flexibility in wave contents.
   %template.addBehaviorField(enemy1, "The type of enemy for this enemy group", object, "", SceneObject);
   %template.addBehaviorField(spawner1, "The path for this enemy group", object, "", Grid);
   %template.addBehaviorField(spawnDelay1, "The delay period before this enemy group spawns", float, 1.0);
   %template.addBehaviorField(enemyDelay1, "The delay period between each enemy in this group", float, 1.0);
   %template.addBehaviorField(enemyCount1, "The number of this type of enemy to spawn", int, 1);

   %template.addBehaviorField(enemy2, "The type of enemy for this enemy group", object, "", SceneObject);
   %template.addBehaviorField(spawner2, "The path for this enemy group", object, "", Grid);
   %template.addBehaviorField(spawnDelay2, "The delay period before this enemy group spawns", float, 1.0);
   %template.addBehaviorField(enemyDelay2, "The delay period between each enemy in this group", float, 1.0);
   %template.addBehaviorField(enemyCount2, "The number of this type of enemy to spawn", int, 1);

   %template.addBehaviorField(enemy3, "The type of enemy for this enemy group", object, "", SceneObject);
   %template.addBehaviorField(spawner3, "The path for this enemy group", object, "", Grid);
   %template.addBehaviorField(spawnDelay3, "The delay period before this enemy group spawns", float, 1.0);
   %template.addBehaviorField(enemyDelay3, "The delay period between each enemy in this group", float, 1.0);
   %template.addBehaviorField(enemyCount3, "The number of this type of enemy to spawn", int, 1);

   %template.addBehaviorField(enemy4, "The type of enemy for this enemy group", object, "", SceneObject);
   %template.addBehaviorField(spawner4, "The path for this enemy group", object, "", Grid);
   %template.addBehaviorField(spawnDelay4, "The delay period before this enemy group spawns", float, 1.0);
   %template.addBehaviorField(enemyDelay4, "The delay period between each enemy in this group", float, 1.0);
   %template.addBehaviorField(enemyCount4, "The number of this type of enemy to spawn", int, 1);

   %template.addBehaviorField(enemy5, "The type of enemy for this enemy group", object, "", SceneObject);
   %template.addBehaviorField(spawner5, "The path for this enemy group", object, "", Grid);
   %template.addBehaviorField(spawnDelay5, "The delay period before this enemy group spawns", float, 1.0);
   %template.addBehaviorField(enemyDelay5, "The delay period between each enemy in this group", float, 1.0);
   %template.addBehaviorField(enemyCount5, "The number of this type of enemy to spawn", int, 1);

   %template.addBehaviorField(enemy6, "The type of enemy for this enemy group", object, "", SceneObject);
   %template.addBehaviorField(spawner6, "The path for this enemy group", object, "", Grid);
   %template.addBehaviorField(spawnDelay6, "The delay period before this enemy group spawns", float, 1.0);
   %template.addBehaviorField(enemyDelay6, "The delay period between each enemy in this group", float, 1.0);
   %template.addBehaviorField(enemyCount6, "The number of this type of enemy to spawn", int, 1);

   %template.addBehaviorField(enemy7, "The type of enemy for this enemy group", object, "", SceneObject);
   %template.addBehaviorField(spawner7, "The path for this enemy group", object, "", Grid);
   %template.addBehaviorField(spawnDelay7, "The delay period before this enemy group spawns", float, 1.0);
   %template.addBehaviorField(enemyDelay7, "The delay period between each enemy in this group", float, 1.0);
   %template.addBehaviorField(enemyCount8, "The number of this type of enemy to spawn", int, 1);

   %template.addBehaviorField(enemy8, "The type of enemy for this enemy group", object, "", SceneObject);
   %template.addBehaviorField(spawner8, "The path for this enemy group", object, "", Grid);
   %template.addBehaviorField(spawnDelay8, "The delay period before this enemy group spawns", float, 1.0);
   %template.addBehaviorField(enemyDelay8, "The delay period between each enemy in this group", float, 1.0);
   %template.addBehaviorField(enemyCount8, "The number of this type of enemy to spawn", int, 1);

   %template.addBehaviorField(enemy9, "The type of enemy for this enemy group", object, "", SceneObject);
   %template.addBehaviorField(spawner9, "The path for this enemy group", object, "", Grid);
   %template.addBehaviorField(spawnDelay9, "The delay period before this enemy group spawns", float, 1.0);
   %template.addBehaviorField(enemyDelay9, "The delay period between each enemy in this group", float, 1.0);
   %template.addBehaviorField(enemyCount9, "The number of this type of enemy to spawn", int, 1);

   %template.addBehaviorField(enemy10, "The type of enemy for this enemy group", object, "", SceneObject);
   %template.addBehaviorField(spawner10, "The path for this enemy group", object, "", Grid);
   %template.addBehaviorField(spawnDelay10, "The delay period before this enemy group spawns", float, 1.0);
   %template.addBehaviorField(enemyDelay10, "The delay period between each enemy in this group", float, 1.0);
   %template.addBehaviorField(enemyCount10, "The number of this type of enemy to spawn", int, 1);

   %template.addBehaviorField(enemy11, "The type of enemy for this enemy group", object, "", SceneObject);
   %template.addBehaviorField(spawner11, "The path for this enemy group", object, "", Grid);
   %template.addBehaviorField(spawnDelay11, "The delay period before this enemy group spawns", float, 1.0);
   %template.addBehaviorField(enemyDelay11, "The delay period between each enemy in this group", float, 1.0);
   %template.addBehaviorField(enemyCount11, "The number of this type of enemy to spawn", int, 1);

   %template.addBehaviorField(enemy12, "The type of enemy for this enemy group", object, "", SceneObject);
   %template.addBehaviorField(spawner12, "The path for this enemy group", object, "", Grid);
   %template.addBehaviorField(spawnDelay12, "The delay period before this enemy group spawns", float, 1.0);
   %template.addBehaviorField(enemyDelay12, "The delay period between each enemy in this group", float, 1.0);
   %template.addBehaviorField(enemyCount12, "The number of this type of enemy to spawn", int, 1);

   %template.addBehaviorField(enemy13, "The type of enemy for this enemy group", object, "", SceneObject);
   %template.addBehaviorField(spawner13, "The path for this enemy group", object, "", Grid);
   %template.addBehaviorField(spawnDelay13, "The delay period before this enemy group spawns", float, 1.0);
   %template.addBehaviorField(enemyDelay13, "The delay period between each enemy in this group", float, 1.0);
   %template.addBehaviorField(enemyCount13, "The number of this type of enemy to spawn", int, 1);

   %template.addBehaviorField(enemy14, "The type of enemy for this enemy group", object, "", SceneObject);
   %template.addBehaviorField(spawner14, "The path for this enemy group", object, "", Grid);
   %template.addBehaviorField(spawnDelay14, "The delay period before this enemy group spawns", float, 1.0);
   %template.addBehaviorField(enemyDelay14, "The delay period between each enemy in this group", float, 1.0);
   %template.addBehaviorField(enemyCount14, "The number of this type of enemy to spawn", int, 1);

   %template.addBehaviorField(enemy15, "The type of enemy for this enemy group", object, "", SceneObject);
   %template.addBehaviorField(spawner15, "The path for this enemy group", object, "", Grid);
   %template.addBehaviorField(spawnDelay15, "The delay period before this enemy group spawns", float, 1.0);
   %template.addBehaviorField(enemyDelay15, "The delay period between each enemy in this group", float, 1.0);
   %template.addBehaviorField(enemyCount15, "The number of this type of enemy to spawn", int, 1);

   %template.addBehaviorField(enemy16, "The type of enemy for this enemy group", object, "", SceneObject);
   %template.addBehaviorField(spawner16, "The path for this enemy group", object, "", Grid);
   %template.addBehaviorField(spawnDelay16, "The delay period before this enemy group spawns", float, 1.0);
   %template.addBehaviorField(enemyDelay16, "The delay period between each enemy in this group", float, 1.0);
   %template.addBehaviorField(enemyCount16, "The number of this type of enemy to spawn", int, 1);

   %template.addBehaviorField(enemy17, "The type of enemy for this enemy group", object, "", SceneObject);
   %template.addBehaviorField(spawner17, "The path for this enemy group", object, "", Grid);
   %template.addBehaviorField(spawnDelay17, "The delay period before this enemy group spawns", float, 1.0);
   %template.addBehaviorField(enemyDelay17, "The delay period between each enemy in this group", float, 1.0);
   %template.addBehaviorField(enemyCount17, "The number of this type of enemy to spawn", int, 1);

   %template.addBehaviorField(enemy18, "The type of enemy for this enemy group", object, "", SceneObject);
   %template.addBehaviorField(spawner18, "The path for this enemy group", object, "", Grid);
   %template.addBehaviorField(spawnDelay18, "The delay period before this enemy group spawns", float, 1.0);
   %template.addBehaviorField(enemyDelay18, "The delay period between each enemy in this group", float, 1.0);
   %template.addBehaviorField(enemyCount18, "The number of this type of enemy to spawn", int, 1);

   %template.addBehaviorField(enemy19, "The type of enemy for this enemy group", object, "", SceneObject);
   %template.addBehaviorField(spawner19, "The path for this enemy group", object, "", Grid);
   %template.addBehaviorField(spawnDelay19, "The delay period before this enemy group spawns", float, 1.0);
   %template.addBehaviorField(enemyDelay19, "The delay period between each enemy in this group", float, 1.0);
   %template.addBehaviorField(enemyCount19, "The number of this type of enemy to spawn", int, 1);

   %template.addBehaviorField(enemy20, "The type of enemy for this enemy group", object, "", SceneObject);
   %template.addBehaviorField(spawner20, "The path for this enemy group", object, "", Grid);
   %template.addBehaviorField(spawnDelay20, "The delay period before this enemy group spawns", float, 1.0);
   %template.addBehaviorField(enemyDelay20, "The delay period between each enemy in this group", float, 1.0);
   %template.addBehaviorField(enemyCount20, "The number of this type of enemy to spawn", int, 1);
}

/// <summary>
/// Handles some basic setup tasks
/// </summary>
/// <param name="scene">The scene object to which the behavior instance is being added</param>
function WaveBehavior::onAdd(%this, %scene)
{
   %this.spawnCount = 0;
   %this.waveEnemyCount = 0;
   %this.createGroupList();
   %this.initialize();
}

/// <summary>
/// This handles some more advanced setup tasks, including setting up the enemy cache.
/// </summary>
/// <param name="scene">The scene object to which the behavior instance is being added.</param>
function WaveBehavior::onAddToScene(%this, %scene)
{
   if (!$LevelEditorActive)
   {
      // Let the enemy factory cache know about our enemy types
      MainScene.startEnemyTypeCounting();
      for (%i = 0; %i < %this.groupCount; %i++)
      {
         if (%this.groupList[%i].enemy !$= "" && %this.groupList[%i].enemy !$= "None")
         {
            MainScene.addWaveEnemyType(%this.groupList[%i].enemy, %this.groupList[%i].enemyCount);
         }
      }
      MainScene.endEnemyTypeCounting();
   }
}

/// <summary>
/// This function handles setting up total delay times for internal use in the wave.
/// It is not used with the current implementation
/// </summary>
function WaveBehavior::initialize(%this)
{
   //echo(" -- " @ %this.owner.friendlyName @ " has " @ %this.waveEnemyCount @ " enemies");
   
   if (%this.waveEnemyCount = 0)
   {
      %this.manualStart = false;
      %this.startDelay = 0;
      return;
   }
   
   if (%this.manualStart)
      %this.startDelay = "";
   else
   {
      if (%this.startDelay < 0)
         %this.startDelay = 0;
   }
   
   if (%this.startDelay !$= "")
      %this.totalDelayTime = %this.totalWaveTime + %this.startDelay;
   else
      %this.totalDelayTime = %this.totalWaveTime;
   
   //echo(" -- totalDelayTime : " @ %this.totalWaveTime @ " + " @ %this.startDelay @ " = " @ %this.totalDelayTime);
   //echo(" -- waveEnemyCount : " @ %this.waveEnemyCount);
}

/// <summary>
/// This function is used by the Wave Tool to clear the contents of the wave object.
/// </summary>
function WaveBehavior::clear(%this)
{
    for (%i = 0; %i < %this.groupCount; %i++)
   {
        %this.groupList[%i].spawnDelay = "";
        %this.groupList[%i].enemyCount = "";
        %this.groupList[%i].enemyDelay = "";
        %this.groupList[%i].spawner = "";
        %this.groupList[%i].enemy = "";
        
        %this.owner.extraEnemy[%i] = "";
        %this.owner.extraEnemyCount[%i] = "";
        %this.owner.extraEnemyDelay[%i] = "";
        %this.owner.extraSpawnDelay[%i] = "";
        %this.owner.extraSpawner[%i] = "";
   }
   %this.groupCount = 0;
   %this.groupObjectCount = 0;
   %this.onAdd(MainScene);
}

/// <summary>
/// This function is used by the Wave Tool to clear the 'hot' list of the current
/// wave contents.
/// </summary>
function WaveBehavior::clearObjectList(%this)
{
    for (%i = 0; %i < %this.groupObjectCount; %i++)
    {
        %this.groupObjectList[%i].spawnDelay = "";
        %this.groupObjectList[%i].enemyCount = "";
        %this.groupObjectList[%i].enemyDelay = "";
        %this.groupObjectList[%i].spawner = "";
        %this.groupObjectList[%i].enemy = "";
        
        %this.owner.extraEnemy[%i] = "";
        %this.owner.extraEnemyCount[%i] = "";
        %this.owner.extraEnemyDelay[%i] = "";
        %this.owner.extraSpawnDelay[%i] = "";
        %this.owner.extraSpawner[%i] = "";
    }
    %this.groupObjectCount = 0;
}

/// <summary>
/// This is used by the Wave Tool to determine if the wave is started manually.
/// </summary>
/// <return>Returns whether the wave is manually started or not.</return>
function WaveBehavior::getManualStart(%this)
{
   return %this.manualStart;
}

/// <summary>
/// This is used by the Wave Tool to set the wave's manual start state.
/// </summary>
/// <param name="flag">The value to set for manual start (true to start manually, false to start on a timer).</param>
function WaveBehavior::setManualStart(%this, %flag)
{
   %this.manualStart = %flag;
   WaveController.callOnBehaviors("initialize");
}

/// <summary>
/// Accesses the total time that the wave will take to complete.
/// </summary>
/// <return>Returns the total time in seconds from wave start to spawn of last enemy, including initial spawn delay and enemy group delays.</return>
function WaveBehavior::getTotalWaveTime(%this)
{
   return %this.totalWaveTime;
}

/// <summary>
/// Used to access the wave's start delay time.
/// </summary>
/// <return>Returns the wave start delay in seconds</return>
function WaveBehavior::getStartDelay(%this)
{
   return %this.startDelay;
}

/// <summary>
/// Used by the Wave Tool to set the wave's start delay time.
/// </summary>
/// <param name="delay">The wave's start delay time in seconds.</param>
function WaveBehavior::setStartDelay(%this, %delay)
{
   %this.startDelay = %delay;
}

/// <summary>
/// Used by the Wave Tool to populate the wave contents pane.
/// </summary>
/// <param name="index">The index of the desired enemy group.</param>
/// <return>Returns a script object that holds the contents of the enemy group at %index.</return>
function WaveBehavior::getEnemyGroup(%this, %index)
{
    if (!isObject(%this.groupObjectList))
        %this.createGroupObjectList();
    if (%index < 0 || %index > %this.groupObjectCount)
      return -1;

    //echo(" -- WaveBehavior::getEnemyGroup(): " @ %this.groupObjectList[%index].enemy @ ", count: " @ %this.groupObjectList[%index].enemyCount @ ", enemy delay: " @ %this.groupObjectList[%index].enemyDelay @ ", delay: " @ %this.groupObjectList[%index].spawnDelay @ ", path: " @ %this.groupObjectList[%index].spawner);
    return %this.groupObjectList[%index];
}

/// <summary>
/// Access the wave's default spawn grid.  Enemies will be spawned on this grid if their individual 
/// spawn grid is undefined.
/// </summary>
/// <return>Returns the wave's default path grid.</return>
function WaveBehavior::getDefaultSpawn(%this)
{
   return %this.defaultSpawnLocation;
}

/// <summary>
/// Sets the path grid to use for spawning enemies if their spawn grid is undefined.
/// </summary>
/// <param name="spawnPoint">The name of the grid to assign for default spawns.</param>
function WaveBehavior::setDefaultSpawn(%this, %spawnPoint)
{
   %this.defaultSpawnLocation = %spawnPoint;
}

/// <summary>
/// Used by the Wave Tool to set a user-friendly name for the wave.  This is stored in the 
/// wave object's internalName field.
/// </summary>
/// <param name="name">The name to assign to the wave object.</param>
function WaveBehavior::setName(%this, %name)
{
   %this.owner.setInternalName(%name);
}

/// <summary>
/// Used by the Wave Tool to access the name of the wave's spawn sound profile.
/// </summary>
/// <return>Returns the name of the audio profile for the sound to play when this wave spawns</return>
function WaveBehavior::getSpawnSound(%this)
{
   return %this.waveSound;
}

/// <summary>
/// Used by the Wave Tool to assign an audio profile containing a sound to play when starting to 
/// spawn enemies.
/// </summary>
/// <param name="sound">The name of the audio profile to assign to this wave.</param>
function WaveBehavior::setSpawnSound(%this, %sound)
{
   %this.waveSound = %sound;
}

/// <summary>
/// Used by the Wave Tool to assign enemy group data to a spawn group.
/// </summary>
/// <param name="group">The script object containing enemy group data.</param>
function WaveBehavior::addEnemyGroup(%this, %group)
{
    if (isObject(%group))
    {
        //echo(" -- WaveBehavior::addEnemyGroup() - " @ %group.getName() @ " enemy: " @ %group.enemy @ " enemyCount: " @ %group.enemyCount @ " enemyDelay: " @ %group.enemyDelay @ " spawnDelay: " @ %group.spawnDelay @ " spawner: " @ %group.spawner);
        %this.owner.extraEnemy[%this.groupObjectCount] = %group.enemy;
        %this.owner.extraEnemyCount[%this.groupObjectCount] = %group.enemyCount;
        %this.owner.extraEnemyDelay[%this.groupObjectCount] = %group.enemyDelay;
        %this.owner.extraSpawnDelay[%this.groupObjectCount] = %group.spawnDelay;
        %this.owner.extraSpawner[%this.groupObjectCount] = %group.spawner;
        %this.createGroupObjectList();
        return true;
    }
    else
        return false;
}

/// <summary>
/// ! Deprecated !
/// Used by the Wave Tool to assign an enemy to a wave spawn slot.
/// </summary>
/// <param name="enemy">The SimObject name of the enemy object to spawn in this slot.</param>
/// <param name="spawn">The path grid object that contains the path this enemy will follow.</param>
/// <param name="delay">The delay in seconds from the spawn of the previous enemy to delay the spawn of this enemy.</param>
function WaveBehavior::writeEnemy(%this, %enemy, %spawn, %delay)
{
   // This function and the next are designed to be used together.  This allows
   // the addition of multiple enemies before calling the onAdd() method
   // to populate the behavior's arrays.
   if (isObject(%enemy))
   {
      for (%i = 0; %i < 20; %i++)
      {
         eval("%slot = %this.enemy" @ %i + 1 @ ";");
         if (%slot $= "")
         {
            eval("%this.enemy" @ %i + 1 @ " = " @ %enemy @ ";");
            eval("%this.spawner" @ %i + 1 @ " = " @ %spawn @ ";");
            eval("%this.spawnDelay" @ %i + 1 @ " = " @ %delay @ ";");
            //echo(" -- WaveBehavior::writeEnemy() - " @ %enemy @ ", " @ %spawn @ ", "@ %delay @ " at " @ %i);

            return true;
         }
      }
   }
   return false;
}

/// <summary>
/// ! Deprectated !
/// Used by the Wave Tool to force a refresh of the wave's internal contents lists.
/// </summary>
function WaveBehavior::commitEnemyWrite(%this)
{
   // Call this method to manually refresh the behavior's contents arrays.
   %this.onAdd(MainScene);
}

/// <summary>
/// ! Deprecated !
/// Used by the wave tool to clear the contents of an enemy slot.  All slots are then 
/// refreshed to leave no blank slots in the middle of the lists.
/// </summary>
/// <param name="index">The index of the enemy slot to clear.</param>
function WaveBehavior::removeEnemy(%this, %index)
{
    if (%index < 0 || %index > %this.groupObjectCount)
      return -1;
      
    eval("%this.spawnDelay" @ (%index + 1) @ " = " @ %groupDelay @ ";");
    eval("%this.enemy" @ (%index + 1) @ " = " @ %enemy @ ";");
    eval("%this.enemyCount" @ (%index + 1) @ " = " @ %count @ ";");
    eval("%this.spawner" @ (%index + 1) @ " = " @ %path @ ";");
    eval("%this.enemyDelay" @ (%index + 1) @ " = " @ %delayEach @ ";");

    %this.owner.extraSpawnDelay[%index] = %groupDelay;
    %this.owner.extraEnemy[%index] = %enemy;
    %this.owner.extraEnemyCount[%index] = %count;
    %this.owner.extraSpawner[%index] = %path;
    %this.owner.extraEnemyDelay[%index] = %delayEach;

   %this.onAdd(MainScene);
   
   return true;
}

/// <summary>
/// This function returns the total time from spawn of the first enemy to the spawn of 
/// the last enemy - but does not include the initial wave spawn delay.
/// </summary>
/// <return>Returns the total time in seconds spent in spawning enemies</return>
function WaveBehavior::getTotalDelayTime(%this)
{
   return %this.totalDelayTime;
}

/// <summary>
/// This function gets the wave's total number of enemies.
/// </summary>
/// <return>Returns the number of enemies in this wave.</return>
function WaveBehavior::getWaveEnemyCount(%this)
{
   return %this.enemyCount;
         }

/// <summary>
/// This function is used to start spawning the wave's enemies.
/// </summary>
/// <param name="delay">The wave's initial spawn delay.</param>
function WaveBehavior::startSpawns(%this, %delay)
{      
   %this.spawnCount = 0;   
   if (%delay)
   {
      %delay *= 1000;
   }
   else
      %delay = %this.startDelay * 1000;
   if (%this.count)
      %this.spawnEvent = ScheduleManager.scheduleEvent(%delay, %this, "WaveBehavior::spawn", %this);
}

/// <summary>
/// This function schedules the enemy group spawn start times.
/// </summary>
function WaveBehavior::spawn(%this)
{
   // Play wave sound
   if(%this.waveSound !$= "None" && %this.waveSound !$= "")
   {
      //echo(" -- playing : " @ %this.waveSound);
      alxPlay(%this.waveSound);
   }

   UpdateCurrentWave($CurrentWave + 1);   

   %time = 0;   
   for (%i = 0; %i < %this.groupCount; %i++)
   {
      if (!isObject(%this.groupList[%i].enemy))
      {
         //echo(" -- WaveBehavior::spawn() - enemy " @ %i @ " not assigned");
         // try to spawn next enemy in the list.
         continue;
      }

      %position = "";
      
      if (isObject(%this.groupList[%i].spawner))
      {
         %position = getGridStartPoint(%this.groupList[%i].spawner);
      }
      else if (isObject(%this.defaultSpawnLocation))
      {
         %position = getGridStartPoint(%this.defaultSpawnLocation);
      }
      else
      {
         //echo(" -- WaveBehavior::spawn() - spawner " @ %i @ " not assigned");
         // no spawn location, skip this enemy
         continue;
      }
      // Path grid is defined but has no path assigned
      if (%position $= "")
        continue;
        
      %time = (1000 * %this.groupList[%i].spawnDelay);
      //echo(" -- " @ %this.owner.getInternalName() @ " spawning " @ %this.groupList[%i].getName() @ " in " @ %time @ " milliseconds");
      ScheduleManager.scheduleEvent(%time, %this, "WaveBehavior::spawnGroup", %this, %i);
   }
}

/// <summary>
/// This function schedules the spawn of each enemy within a group.
/// </summary>
/// <param name="index">The index of the group to spawn.</param>
function WaveBehavior::spawnGroup(%this, %index)
{
    // this will handle getting the spawn groups started.
    %group = %this.groupList[%index];
    %spawnTime = 0;
    if (isObject(%group))
    {
        //echo(" -- Wave : " @ %this.owner.getInternalName() @ " groupCount : " @ %this.groupCount @ " index : " @ %index);
        for (%i = 0; %i < %group.enemyCount; %i++)
        {
            %spawnTime = (%i + 1) * (1000 * %group.enemyDelay);
            %position = getGridStartPoint(%group.spawner);
            //echo(" -- " @ %this.owner.getInternalName() @ " spawning " @ %group.getName() @ " : " @ %group.enemy @ "#" @ %i @ " Delay " @ %group.enemyDelay @ " in " @ %spawnTime @ " milliseconds");
            ScheduleManager.scheduleEvent(%spawnTime, %this, "WaveBehavior::spawnEnemy", %this, %group.enemy, %index, %position);
        }

        if ((%index + 1) == %this.groupCount)
        {
            //echo(" -- unlocking wave advance in " @ %spawnTime @ " milliseconds");
            %this.schedule(%spawnTime, updateAdvanceAvailability);
        }
    }
}

/// <summary>
/// This function tells the WaveControllerBehavior object that this wave has finished
/// spawning enemies.
/// </summary>
function WaveBehavior::updateAdvanceAvailability(%this)
{
    waveController.callOnBehaviors(updateAdvanceWaveAvailability);
}

/// <summary>
/// This function handles the creation of an enemy.
/// </summary>
/// <param name="enemy">The SimObject name of the enemy object to create.</param>
/// <param name="index">The index of the enemy within the group.</param>
/// <param name="position>The world position at which to place the new enemy.</param>
function WaveBehavior::spawnEnemy(%this, %enemy, %index, %position)
{
   %clone = MainScene.getEnemyFromCache(%enemy);
   
   
   // Offset the enemy based on their path following
   %offset = %clone.callOnBehaviors(getPathFollowingOffset);
   %spawnPoint = Vector2Add(%position, %offset);
   //echo(" -- WaveBehavior::spawnEnemy() - spawn point : " @ %spawnPoint);
   %clone.setPosition(%spawnPoint);
   %clone.active = true;
   
   %scorekeeper = %clone.callOnBehaviors(getScorekeeper);
   %scorekeeper.callOnBehaviors(reportEnemySpawn, %clone);

   //%clone.schedule(0, "PathFollowingBehavior::goToDestination"); // does not work
   //%clone.schedule(0, "goToDestination"); // does not work
   //%clone.schedule(0, "callMethod", "goToDestination"); // does not work
   
   //schedule(0, 0, "PathFollowingBehavior::goToDestination", %clone); // works but is unmanaged
   //schedule(0, 0, "callMethod", %clone, "goToDestination"); // does not work
   //schedule(0, %clone, "callMethod", "goToDestination"); // does not work
   //schedule(0, %clone, "callMethod", %clone, "goToDestination"); // does not work
   
   //ScheduleManager.objectEvent(0, %clone, "callMethod", "goToDestination"); // does not work
   //ScheduleManager.objectEvent(0, %clone, "PathFollowingBehavior::goToDestination"); // does not work
   //ScheduleManager.objectEvent(0, %clone, "goToDestination"); // does not work

   //ScheduleManager.scheduleEvent(0, %this, callMethod, %clone, "goToDestination"); // does not work
   //ScheduleManager.scheduleEvent(0, %clone, callMethod, %clone, "goToDestination"); // does not work
   //ScheduleManager.scheduleEvent(0, %this, "goToDestination", %clone); // does not work
   //ScheduleManager.scheduleEvent(0, %clone, "goToDestination", %clone); // does not work

   //ScheduleManager.scheduleEvent(0, %this, "PathFollowingBehavior::goToDestination", %clone); // works
   %clone.callOnBehaviors("goToDestination", %this.groupList[%index].spawner);
                  
   %this.spawnCount++;
}

/// <summary>
/// This function cancels the spawning of enemies in this wave.
/// </summary>
function WaveBehavior::stopSpawns(%this)
{
   cancel(%this.spawnEvent);
}

/// <summary>
/// This function is used to generate the enemy list to spawn from the data attached 
/// to the wave object.
/// </summary>
function WaveBehavior::createGroupList(%this)
{
    // This function creates a list of spawn groups from the arrays attached 
    // to the behavior's owner.
    %this.groupCount = 0;
    %count = 0;
    %totalDelay = 0;
    %test = %this.owner.extraEnemy[%count];
    while (%test !$= "")
    {
        %name = %this.owner.getName() @ "group" @ %count;
        if (!isObject(%name))
            %group = new ScriptObject(%name);
        else
            %group = %name;
        %group.spawnDelay = %this.owner.extraSpawnDelay[%count] + %totalDelay;
        %group.enemyCount = %this.owner.extraEnemyCount[%count];
        %group.enemyDelay = %this.owner.extraEnemyDelay[%count];
        %group.spawner = %this.owner.extraSpawner[%count];
        %group.enemy = %this.owner.extraEnemy[%count];
        
        %totalDelay += %this.owner.extraSpawnDelay[%count] + ((%group.enemyCount - 1) * %group.enemyDelay);

        %this.groupList[%count] = %group;
        
        %this.enemyCount += %group.enemyCount;
        %this.totalWaveTime += (%this.owner.extraSpawnDelay[%count] + ((%this.owner.extraEnemyDelay[%count]) * %this.owner.extraEnemyCount[%count]));
        
        %count++;
        %test = %this.owner.extraEnemy[%count];
        //echo(" -- createGroupList() - " @ %group.getName() @ " delay time : " @ %group.spawnDelay);
    }
    %this.groupCount = %count;
}

/// <summary>
/// This function is used by the Wave Tool to generate a group list for populating the wave preview pane
/// that doesn't accumulate group spawn delay time - this is to simplify entering delay time data.
/// </summary>
function WaveBehavior::createGroupObjectList(%this)
{
    // This function creates a list of spawn groups from the arrays attached 
    // to the behavior's owner.
    %this.groupObjectCount = 0;
    %count = 0;
    %test = %this.owner.extraEnemy[%count];
    while (%test !$= "")
    {
        %name = %this.owner.getName() @ "groupObject" @ %count;
        if (!isObject(%name))
            %group = new ScriptObject(%name);
        else
            %group = %name;
        %group.spawnDelay = %this.owner.extraSpawnDelay[%count];
        %group.enemyCount = %this.owner.extraEnemyCount[%count];
        %group.enemyDelay = %this.owner.extraEnemyDelay[%count];
        %group.spawner = %this.owner.extraSpawner[%count];
        %group.enemy = %this.owner.extraEnemy[%count];

        %this.groupObjectList[%count] = %group;
        %count++;
        %test = %this.owner.extraEnemy[%count];
    }
    %this.groupObjectCount = %count;
}

/// <summary>
/// This function is used by the Wave Tool to save the spawn group data to the wave
/// object as dynamic field data.
/// </summary>
function WaveBehavior::saveGroupList(%this)
{
    if (!isObject(%this.groupObjectList))
        %this.createGroupObjectList();
    %count = 0;
    %group = %this.groupObjectList[%count];
    while (isObject(%group))
    {
        %this.owner.extraSpawnDelay[%count] = %group.spawnDelay;
        %this.owner.extraEnemyCount[%count] = %group.enemyCount;
        %this.owner.extraEnemyDelay[%count] = %group.enemyDelay;
        %this.owner.extraSpawner[%count] = %group.spawner;
        %this.owner.extraEnemy[%count] = %group.enemy;
        %count++;
        %group = %this.groupObjectList[%count];
    }
}

/// <summary>
/// This function is used by the Wave Tool to help to populate the wave preview pane.
/// </summary>
/// <return>Returns the number of enemy groups contained in this wave.</return>
function WaveBehavior::getGroupCount(%this)
{
    return %this.groupCount;
}

/// <summary>
/// This function is used by the Wave Tool to remove an enemy group from the wave.  All 
/// groups will be accumulated to the front of the list to avoid having gaps in the list.
/// </summary>
/// <param name="groupIndex">The index of the group to remove.</param>
function WaveBehavior::removeGroup(%this, %groupIndex)
{
    // remove group from wave's list.
    %this.groupList[%groupIndex] = "";
    
    %k = 0;
    for (%i = 0; %i < %this.groupCount; %i++)
    {
        if (%this.groupList[%i] !$= "")
        {
            %cleanList[%k] = %this.groupList[%i];
            %k++;
        }
        else
        {
            %this.owner.extraSpawnDelay[%i] = "";
            %this.owner.extraEnemyCount[%i] = "";
            %this.owner.extraEnemyDelay[%i] = "";
            %this.owner.extraSpawner[%i] = "";
            %this.owner.extraEnemy[%i] = "";
        }
    }
    for (%i = 0; %i < %k; %i++)
    {
        %this.groupList[%i] = %cleanList[%i];
    }
    %this.groupCount = %k;
    %this.saveGroupList();
}