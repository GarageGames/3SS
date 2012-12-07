//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEffect, addEmitter, S32, 2, 3, "(emitter) - Adds an Emitter to the Effect creating a new one if necessary.\n"
              "@return On success it returns the ID of the emitter, or 0 if failed.")
{
   ParticleEmitter* emitter = NULL;
   if (argc > 2)
      emitter = dynamic_cast<ParticleEmitter*>(Sim::findObject(argv[2]));

   // Add Emitter.
   ParticleEmitter* newEmitter = object->addEmitter(emitter);
   return newEmitter ? newEmitter->getId() : 0;
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEffect, removeEmitter, void, 3, 4, "(emitterObject, [bool deleteEmitter]) - Removes an Emitter from the Effect.\n"
              "@return No return value.")
{
   bool deleteEmitter = true;
   if (argc > 3)
      deleteEmitter = dAtob(argv[3]);

   // Remove Emitter.
   object->removeEmitter( dAtoi(argv[2]), deleteEmitter );
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEffect, clearEmitters, void, 2, 2, "() Clear all Emitters from the Effect.\n"
              "@return No return Value.")
{
   // Clear Emitters.
   object->clearEmitters();
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEffect, getEmitterCount, S32, 2, 2, "() Gets Emitter Count for Effect.\n"
              "@return Returns the number of emitters as an integer.")
{
   // Get Emitter Count.
   return object->getEmitterCount();
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEffect, findEmitterObject, S32, 3, 3, "(emitterName$) Finds Emitter Object by name from Effect.\n"
              "@param emitterName The name of the desired effect emitter\n"
              "@return The emitter's name as a string.")
{
   // Find Emitter Object.
   return object->findEmitterObject( argv[2] );
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEffect, getEmitterObject, S32, 3, 3, "(emitterIndex) Gets Emitter Object from Effect.\n"
              "@param emitterIndex The index value for the desired emitter\n"
              "@return The object ID")
{
   // Get Emitter Object.
   return object->getEmitterObject( dAtoi(argv[2]) );
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEffect, moveEmitter, void, 4, 4, "(fromEmitterIndex, toEmitterIndex) Moves the Emitter Object.\n"
              "@param fromEmitterIndex Original index of desired emitter\n"
              "@param toEmitterIndex Desired destination index.\n"
              "@return No return value.")
{
   // Move Emitter Object.
   object->moveEmitter( dAtoi(argv[2]), dAtoi(argv[3]) );
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEffect, selectGraph, void, 3, 3, "(graphName) Select Graph by Name.\n"
              "@param graphName Name of desired graph\n"
              "@return No return value.")
{
   // Select Graph.
   object->selectGraph( argv[2] );
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEffect, addDataKey, S32, 4, 4, "(time, value) Add Data-Key to Graph.\n"
              "@param time The time key\n"
              "@param value The value at given time\n"
              "@return Returns the index of the Data-Key or -1 on failure.")
{
   // Add Data Key.
   return object->addDataKey( dAtof(argv[2]), dAtof(argv[3]) );
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEffect, removeDataKey, bool, 3, 3, "(keyIndex) Remove Data-Key from Graph.\n"
              "@param keyIndex The index of the data-key you want to remove\n"
              "@return Returns true on success and false otherwise.")
{
   // Remove Data Key.
   return object->removeDataKey( dAtoi(argv[2]) );
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEffect, clearDataKeys, bool, 2, 2, "() Clears all Data-Key(s) from Graph.\n"
              "@return Returns true on success (false means you do not have a graph selected).")
{
   // Clear Data Keys
   return object->clearDataKeys();
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEffect, setDataKeyValue, bool, 4, 4, "(keyIndex, value) Set Data-Key Value in Graph.\n"
              "@param keyIndex The index of the key you wish to modify.\n"
              "@param value The value you wish to reset the given data-key to.\n"
              "@return Returns true on success and false otherwise.")
{
   // Set Data Key.
   return object->setDataKey( dAtoi(argv[2]), dAtof(argv[3]) );
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEffect, getDataKey, const char*, 3, 3, "(keyIndex) Get Data-Key Time/Value from Graph.\n"
              "@param keyIndex The index of the desired data-key\n"
              "@return Returns the data-key as a string formatted as \"time value\" or false if failed")
{
   // Fetch Key Index.
   S32 keyIndex = dAtoi(argv[2]);

   // Create Returnable Buffer.
   char* pBuffer = Con::getReturnBuffer(32);
   // Format Buffer.
   dSprintf(pBuffer, 32, "%f %f", object->getDataKeyTime( keyIndex ), object->getDataKeyValue( keyIndex ) );
   // Return buffer.
   return pBuffer;
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEffect, getDataKeyCount, S32, 2, 2, "() \n @return Returns the current Data-Key count from selected graph (or false if failed).")
{
   // Get Data Key Count.
   return object->getDataKeyCount();
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEffect, getMinValue, F32, 2, 2, "() \n @return Returns Min-Value from Graph (or false if failed).")
{
   // Get Min Value.
   return object->getMinValue();
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEffect, getMaxValue, F32, 2, 2, "() \n @return Max-Value from Graph (or false if failed).")
{
   // Get Max Value.
   return object->getMaxValue();
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEffect, getMinTime, F32, 2, 2, "() \n @return Returns Min-Time from Graph (or false if failed).")
{
   // Get Min Time.
   return object->getMinTime();
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEffect, getMaxTime, F32, 2, 2, "() \n @return Returns Max-Value from Graph (or false if failed).")
{
   // Get Max Time.
   return object->getMaxTime();
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEffect, getGraphValue, F32, 3, 3, "(time) Get value at given time from selected graph.\n"
              "@param time The desired graph time step\n"
              "@return Returns the value set at given time (or false if failed).")
{
   // Get Graph Value.
   return object->getGraphValue( dAtof(argv[2]) );
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEffect, setTimeRepeat, bool, 3, 3, "(timeRepeat) Set Time-Repeat For Graph.\n"
              "@return Returns true on success (or false if failed).")
{
   // Set Time Repeat.
   return object->setTimeRepeat( dAtof(argv[2]) );
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEffect, getTimeRepeat, F32, 2, 2, "() \n @return Returns Time-Repeat for Graph (or false if failed).")
{
   // Get Time Repeat.
   return object->getTimeRepeat();
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEffect, setValueScale, bool, 3, 3, "(valueScale) - Set Value-Scale For Graph.\n"
              "@return Returns true on success (or false if failed).")
{
   // Set Value Scale.
   return object->setValueScale( dAtof(argv[2]) );
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEffect, getValueScale, F32, 2, 2, "() \n @return Returns Value-Scale for Graph (or false if failed).")
{
   // Get Value Scale.
   return object->getValueScale();
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEffect, findParticlePeak, const char*, 5, 5, "(searchTime, timeStep, peakLimit) Finds the time of a particle effect at which there are a peak number of particles emitted.\n"
              "@param searchTime The amount of time into the effect to search.\n"
              "@param timeStep The time step at which the effect will be checked. Smaller time steps will give more accurate results, but take more time.\n"
              "@param peakLimit The maximum number of particles to find. The search will stop when this number of particles is found.\n"
              "@return On success it returns a string formatted as \"true peakCount peakTime\". On fail it returns \"false\"")
{
   // Fetch Move-Time.
   const F32 moveTime = dAtof(argv[2]);
   // Fetch Time-Step.
   const F32 timeStep = dAtof(argv[3]);
   // Fetch Peak-Limit.
   const U32 peakLimit = dAtoi(argv[4]);

   // Reset Maximum Particle Tracking.
   U32 peakCount;
   F32 peakTime;

   // Create Returnable Buffer.
   char* pBuffer = Con::getReturnBuffer(32);

   // Find Particle Peak.
   if ( object->findParticlePeak( moveTime, timeStep, peakLimit, peakCount, peakTime ) )
   {
      // Okay, so format Buffer.
      dSprintf(pBuffer, 32, "%s %d %f", "true", peakCount, peakTime);
   }
   else
   {
      // Problem, so format Buffer.
      dSprintf(pBuffer, 32, "false");
   }

   // Return Buffer.
   return pBuffer;
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEffect, moveEffectTo, const char*, 4, 4, "(moveTime, timeStep) Moves an effect's playback to a particular time.\n"
              "@param moveTime The time to move the effect to.\n"
              "@param timeStep The time steps at which the effect will be processed in moving to the move time.\n"
              "@return On success it returns a string formatted as \"true peakCount peakTime\". On fail it returns \"false\"")
{
   // Fetch Move-Time.
   const F32 moveTime = dAtof(argv[2]);
   // Fetch Time-Step.
   const F32 timeStep = dAtof(argv[3]);

   U32 peakCount;
   F32 peakTime;

   // Create Returnable Buffer.
   char* pBuffer = Con::getReturnBuffer(32);

   // Move Effect To.
   if ( object->moveEffectTo( moveTime, timeStep, peakCount, peakTime ) )
   {
      // Okay, so format Buffer.
      dSprintf(pBuffer, 32, "%s %d %f", "true", peakCount, peakTime);
   }
   else
   {
      // Problem, so format Buffer.
      dSprintf(pBuffer, 32, "false");
   }

   // Return Buffer.
   return pBuffer;
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEffect, setEffectPaused, void, 3, 3,   "(effectPaused?) Sets whether the effect is paused or not.\n"
                                                                "@param effectPaused Whether the effect is paused or not.\n"
                                                                "@return No return value.")
{
    object->setEffectPaused( dAtob(argv[2]) );
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEffect, getEffectPaused, bool, 2, 2,   "() Gets whether the effect is paused or not.\n"
                                                                "@return Whether the effect is paused or not.")
{
    return object->getEffectPaused();
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEffect, setCameraIdleDistance, void, 3, 3,    "(idleDistance]) Sets the distance from any camera when the effect will become idle i.e. stop integrating and rendering.\n"
                                                                        "@param pauseDistance The distance from any camera when the effect will become idle i.e. stop integrating and rendering.\n"
                                                                        "@return No return value.")
{
    object->setCameraIdleDistance( dAtof(argv[2]) );
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEffect, getCameraIdleDistance, F32, 2, 2,   "() Gets the distance from any camera when the effect will become idle i.e. stop integrating and rendering.\n"
                                                                        "@return The distance from any camera when the effect will become idle i.e. stop integrating and rendering.")
{
    return object->getCameraIdleDistance();
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEffect, playEffect, bool, 2, 3, "([resetParticles]) Plays the Particle Effect.\n"
              "@param resetParticles Flag for whether to reset particles before playback (default true)\n"
              "@return Returns true on success and false otherwise")
{
   // Calculate Reset-Particles Flag.
   bool resetParticles = argc >= 3 ? dAtob(argv[2]) : true;

   // Play Effect.
   return object->playEffect( resetParticles );
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEffect, stopEffect, void, 2, 4, "([waitForParticles?, killEffect?]) - Stops the Particle Effect.\n"
              "@param waitForParticles Whether or not the effect should wait until all of its particles have run their course, or just stop immediately and delete the particles (default true).\n"
              "@param killEffect Whether or not the effect should be deleted after it has stopped (default false).\n"
              "@return No return value.")
{
   // Calculate Options.
   bool waitForParticles   = argc >= 3 ? dAtob(argv[2]) : true;
   bool killEffect         = argc >= 4 ? dAtob(argv[3]) : false;

   // Stop Effect.
   object->stopEffect( waitForParticles, killEffect );
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEffect, getIsEffectPlaying, bool, 2, 2, "() \n @return Returns true if effect is playing")
{
   return object->getIsEffectPlaying();
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEffect, setEffectLifeMode, void, 3, 4, "(lifeMode, [time]) - Sets the Effect Life Mode/Time.\n"
              "@param lifeMode The life mode of the effect (either INFINITE CYCLE KILL or STOP)\n"
              "@param time The lifetime of the effect. This has different effects depending on the life mode (required for CYCLE or KILL)."
              "@return No return value.")
{
   // Fetch Effect-Life Mode.
   ParticleEffect::eEffectLifeMode lifeMode = getEffectMode( argv[2] );

   // Check for Invalid Arguments.
   if (    (lifeMode == ParticleEffect::CYCLE || lifeMode == ParticleEffect::KILL) &&
      (argc < 4) )
   {
      // Missing Parameter so Warn.
      Con::warnf( "ParticleEffect::setEffectLifeMode() - Missing 'time' parameter for selected mode!" );
      return;
   }

   // Check for Invalid Life Argument.
   if (    (lifeMode == ParticleEffect::CYCLE || lifeMode == ParticleEffect::KILL) &&
      dAtof(argv[3]) <= 0.0f )
   {
      // Missing Parameter so Warn.
      Con::warnf( "ParticleEffect::setEffectLifeMode() - 'time' parameter has to be greater than zero!" );
      return;
   }

   // Handle Effect-Life Mode Appropriately.
   switch( lifeMode )
   {
      // Infinite-Life Mode.
   case ParticleEffect::INFINITE:
      {
         // Set Effect Life Mode.
         object->setEffectLifeMode( ParticleEffect::INFINITE, 0.0f );

      } break;

      // Cycle-Life Mode.
   case ParticleEffect::CYCLE:
      {
         // Set Effect Life Mode.
         object->setEffectLifeMode( ParticleEffect::CYCLE, dAtof(argv[3]) );

      } break;

      // Kill-Life Mode.
   case ParticleEffect::KILL:
      {
         // Set Effect Life Mode.
         object->setEffectLifeMode( ParticleEffect::KILL, dAtof(argv[3]) );

      } break;

      // Stop-Life Mode.
   case ParticleEffect::STOP:
      {
         // Set Effect Life Mode.
         object->setEffectLifeMode( ParticleEffect::STOP, dAtof(argv[3]) );

      } break;
   }
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEffect, setUseEffectCollision, void, 3, 3, "(status) Set whether to use effect collisions or not.\n"
              "@param status True if the effect as a whole is processing collisions, false if the individual particles are processing collisions.\n"
              "@return No return value.")
{
   // Set whether to use effect collisions or not.
   object->setUseEffectCollision( dAtob(argv[2]) );
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEffect, getUseEffectCollision, bool, 2, 2, "() Get whether to use effect collisions or not.\n @return Returns the effect collision status (true if effect processes collisions, or false if individual particles are).")
{
   // Get whether to use effect collisions or not.
   return object->getUseEffectCollision();
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEffect, loadEffect, bool, 3, 3, "(effectFile$) Loads a Particle Effect.\n"
              "@param effectFile File name from which to load desired effect\n"
              "@return Returns true on success.")
{
   // Load Effect.
   return object->loadEffect( argv[2] );
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEffect, saveEffect, bool, 3, 3, "(effectFile$) Saves a Particle Effect to file.\n"
              "@param effectFile The name of the file to save the effect to.\n"
              "@return Returns true on success and false otherwise.")
{
   // Save Effect.
   return object->saveEffect( argv[2] );
}
