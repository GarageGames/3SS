//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "io/resource/resourceManager.h"
#include "console/consoleTypes.h"
#include "ParticleEffect.h"

// Script bindings.
#include "ParticleEffect_ScriptBinding.h"

// Debug Profiling.
#ifdef TORQUE_ENABLE_PROFILER
#include "debug/profiler.h"
#endif

//------------------------------------------------------------------------------

IMPLEMENT_CONOBJECT(ParticleEffect);

//-----------------------------------------------------------------------------

static EnumTable::Enums effectLifeLookup[] =
{
   { ParticleEffect::INFINITE,  "INFINITE" },
   { ParticleEffect::CYCLE,     "CYCLE" },
   { ParticleEffect::KILL,      "KILL" },
   { ParticleEffect::STOP,      "STOP" },
};

//-----------------------------------------------------------------------------

static EnumTable gEffectMode(4, &effectLifeLookup[0]);

//-----------------------------------------------------------------------------

ParticleEffect::eEffectLifeMode getEffectMode(const char* label)
{
   // Search for Mnemonic.
   for(U32 i = 0; i < (sizeof(effectLifeLookup) / sizeof(EnumTable::Enums)); i++)
      if( dStricmp(effectLifeLookup[i].label, label) == 0)
         return((ParticleEffect::eEffectLifeMode)effectLifeLookup[i].index);

   // Invalid Effect Life-Mode!
   AssertFatal(false, "ParticleEffect::getEffectMode() - Invalid Effect-Life Mode!");
   // Bah!
   return ParticleEffect::INFINITE;
}

//-----------------------------------------------------------------------------

ParticleEffect::ParticleEffect() :    Stream_2D_HeaderID(makeFourCCTag('2','D','P','F')),
                                            mpCurrentGraph(NULL),
                                            mCurrentGraphName(StringTable->EmptyString),
                                            mEffectPaused(false),
                                            mEffectPlaying(false),
                                            mWaitingForParticles(false),
                                            mWaitingForDelete(false),
                                            mInitialised(false),
                                            mUseEffectCollisions(false),
                                            mEffectLifeMode(INFINITE),
                                            mEffectLifetime(0.0f),
                                            mCameraIdleDistance(0.0f),
                                            mCameraIdle(false),
                                            mEmitterSerial(1),
                                            mEffectFile(StringTable->EmptyString)
{
   // Set Vector Associations.
   VECTOR_SET_ASSOCIATION( mParticleEmitterList );

   mDisableParticleInterpolation = Con::getBoolVariable( "$pref::T2D::disableParticleInterpolation", false );

   // Set as batch isolated.
   setBatchIsolated( true );

   // Initialise Effect.
   if ( !mInitialised )
      initialise();
}

//-----------------------------------------------------------------------------

ParticleEffect::~ParticleEffect()
{
   // Clear Emitters.
   clearEmitters();

   // Clear Graph Selections.
   clearGraphSelections();
}

//-----------------------------------------------------------------------------

void ParticleEffect::safeDelete()
{
   // Are we already waiting for delete?
   if ( mWaitingForDelete )
      // Yes, nothing to do!
      return;

   // Is effect playing?
   if ( mEffectPlaying )
   {
      // Yes, so stop the effect and allow it to kill itself.
      stopEffect(true, true);
   }
   else
   {
       // Call parent which will deal with the deletion.
       Parent::safeDelete();
   }
}

//-----------------------------------------------------------------------------

bool ParticleEffect::onAdd()
{
   // Call Parent.
   if(!Parent::onAdd())
      return false;

   // Return Okay.
   return true;
}

//-----------------------------------------------------------------------------

void ParticleEffect::onRemove()
{
   // Call Parent.
   Parent::onRemove();
}

//-----------------------------------------------------------------------------

void ParticleEffect::initPersistFields()
{
   addProtectedField("effectFile", TypeFilename, Offset(mEffectFile, ParticleEffect), &setEffectFile, &defaultProtectedGetFn, &writeEffectFile, "");
   addProtectedField("useEffectCollisions", TypeBool, Offset(mUseEffectCollisions, ParticleEffect), &setUseEffectCollisions, &defaultProtectedGetFn, &writeUseEffectCollisions, "");
   addProtectedField("effectMode", TypeEnum, Offset(mEffectLifeMode, ParticleEffect), &setEffectMode, &defaultProtectedGetFn, &writeEffectMode, 1, &gEffectMode);
   addProtectedField("effectTime", TypeF32, Offset(mEffectLifetime, ParticleEffect), &setEffectTime, &defaultProtectedGetFn, &writeEffectTime, "" );
   addProtectedField("cameraIdleDistance", TypeF32, Offset(mCameraIdleDistance, ParticleEffect), &setCameraIdleDistance, &defaultProtectedGetFn, &writeCameraIdleDistance,"" );
   Parent::initPersistFields();
}

//-----------------------------------------------------------------------------

void ParticleEffect::copyTo(SimObject* object)
{
   Parent::copyTo(object);
    
   AssertFatal(dynamic_cast<ParticleEffect*>(object), "ParticleEffect::copyTo() - Object is not the correct type.");
   ParticleEffect* effect = static_cast<ParticleEffect*>(object);

   effect->setUseEffectCollision( mUseEffectCollisions );
   effect->setEffectLifeMode( mEffectLifeMode, mEffectLifetime );
   effect->setCameraIdleDistance( mCameraIdleDistance );

   for (S32 i = 0; i < getEmitterCount(); i++)
   {
      SimObject* emitter = mParticleEmitterList[i].mpSceneObject;
      ParticleEmitter* newEmitter = effect->addEmitter();
      emitter->copyTo(newEmitter);
   }

   mParticleLife.copyTo(effect->mParticleLife);
   mQuantity.copyTo(effect->mQuantity);
   mSizeX.copyTo(effect->mSizeX);
   mSizeY.copyTo(effect->mSizeY);
   mSpeed.copyTo(effect->mSpeed);
   mSpin.copyTo(effect->mSpin);
   mFixedForce.copyTo(effect->mFixedForce);
   mRandomMotion.copyTo(effect->mRandomMotion);
   mEmissionForce.copyTo(effect->mEmissionForce);
   mEmissionAngle.copyTo(effect->mEmissionAngle);
   mEmissionArc.copyTo(effect->mEmissionArc);
   mVisibility.copyTo(effect->mVisibility);

   if (getIsEffectPlaying() && effect->getScene())
      effect->playEffect(true);
}

//-----------------------------------------------------------------------------

void ParticleEffect::OnRegisterScene( Scene* pScene )
{
    // Call parent.
    Parent::OnRegisterScene( pScene );

    // Add always in scope.
    pScene->getWorldQuery()->addAlwaysInScope( this );
}

//-----------------------------------------------------------------------------

void ParticleEffect::OnUnregisterScene( Scene* pScene )
{
    // Remove always in scope.
    pScene->getWorldQuery()->removeAlwaysInScope( this );

    // Call parent.
    Parent::OnUnregisterScene( pScene );
}

//-----------------------------------------------------------------------------

void ParticleEffect::preIntegrate( const F32 totalTime, const F32 elapsedTime, DebugStats* pDebugStats )
{
    // Call Parent.
    Parent::preIntegrate( totalTime, elapsedTime, pDebugStats );

    // Fetch current position.
    const Vector2 position = getPosition();

    // Calculate camera idle distance squared.
    const F32 cameraIdleDistanceSqr = mCameraIdleDistance * mCameraIdleDistance;

    // Fetch scene windows.
    SimSet& sceneWindows = getScene()->getAttachedSceneWindows();

    // Find a scene window that stops the pause.
    for( SimSet::iterator itr = sceneWindows.begin(); itr != sceneWindows.end(); itr++ )
    {
        SceneWindow* pSceneWindow = static_cast<SceneWindow*>(*itr);

        // Are we within the camera distance?
        if ( (pSceneWindow->getCurrentCameraPosition() - position).LengthSquared() < cameraIdleDistanceSqr )
        {
            // Yes, so play effect.
            if ( !getIsEffectPlaying() )
                playEffect( true );

            // Not idle.
            mCameraIdle = false;

            return;
        }
    }

    // Stop effect.
    if ( getIsEffectPlaying() )
        stopEffect( false, false );

    // Now idle.
    mCameraIdle = true;
}

//-----------------------------------------------------------------------------

void ParticleEffect::integrateObject( const F32 totalTime, const F32 elapsedTime, DebugStats* pDebugStats )
{
   // Call Parent.
   Parent::integrateObject( totalTime, elapsedTime, pDebugStats );

#ifdef TORQUE_ENABLE_PROFILER
        PROFILE_START(ParticleEffect_IntegrateObject);
#endif

    // Reset particle counters.
    U32 allocatedParticles = 0;
    U32 activeParticles = 0;

   // Integrate particles?
   if ( mEffectPlaying && !mCameraIdle && !mEffectPaused )
   {
      // Yes, so update Effect Age.
      mEffectAge += elapsedTime;

      // Update all emitters.
      for ( U32 n = 0; n < (U32)mParticleEmitterList.size(); n++ )
      {
         // Integrate Emitter (if visible).
         ParticleEmitter *pEmitter = static_cast<ParticleEmitter*>( mParticleEmitterList[n].mpSceneObject );
         if ( pEmitter )
         {
             if ( pEmitter->getEmitterVisible() )
             {
                // Integrate Emitter.
                pEmitter->integrateObject( totalTime, elapsedTime, pDebugStats );
             }

            // Update particle counters.
            allocatedParticles += pEmitter->getAllocatedParticles();
            activeParticles += pEmitter->getActiveParticles();
         }
      }

      // Only handle modes if we're not waiting for particles.
      if ( !mWaitingForParticles )
      {
         // Handle Effect-Life Mode Appropriately.
         switch( mEffectLifeMode )
         {
            // Cycle-Life Mode.
         case ParticleEffect::CYCLE:
            {
               // Have we expired?
               if ( mEffectAge >= mEffectLifetime )
                  // Yes, so restart effect (reset existing particles).
                  playEffect(true);

            } break;

            // Kill-Life Mode.
         case ParticleEffect::KILL:
            {
               // Have we expired?
               if ( mEffectAge >= mEffectLifetime )
               {
                  if( getScene() && getScene()->getIsEditorScene() )
                     stopEffect( true, false );
                  else
                     // Yes, so stop Effect and Kill.
                     stopEffect( true, true );
               }

            } break;

            // Stop-Life Mode.
         case ParticleEffect::STOP:
            {
               // Have we expired?
               if ( mEffectAge >= mEffectLifetime )
                  // Yes, so stop Effect.
                  stopEffect( true, false );

            } break;
         }
      }

      // Are we waiting for particles and none are active?
      if ( mWaitingForParticles && activeParticles == 0 )
      {
         // Yes, so stop effect ( take note of 'killEffect' flag ).
         stopEffect( false, mWaitingForDelete );
      }
   }
   else
   {
      // Update all emitters.
      for ( U32 n = 0; n < (U32)mParticleEmitterList.size(); n++ )
      {
        // Integrate Emitter (if visible).
        ParticleEmitter *pEmitter = static_cast<ParticleEmitter*>( mParticleEmitterList[n].mpSceneObject );
        if ( pEmitter )
        {
            // Update particle counters.
            allocatedParticles += pEmitter->getAllocatedParticles();
            activeParticles += pEmitter->getActiveParticles();
        }
      }
   }

    // Update Debug Stats.
    pDebugStats->particlesAlloc += allocatedParticles;
    pDebugStats->particlesUsed += activeParticles;
    pDebugStats->particlesFree += allocatedParticles - activeParticles;

#ifdef TORQUE_ENABLE_PROFILER
        PROFILE_END();   // ParticleEffect_IntegrateObject
#endif
}

//-----------------------------------------------------------------------------

void ParticleEffect::interpolateObject( const F32 timeDelta )
{
    // Call Parent.
    Parent::interpolateObject( timeDelta );

    // Integrate particles?
    if ( !mDisableParticleInterpolation && mEffectPlaying && !mCameraIdle && !mEffectPaused )
    {
        // Yes, so interpolate all emitters.
        for ( U32 n = 0; n < (U32)mParticleEmitterList.size(); n++ )
        {
            // Integrate Emitter (if visible).
            ParticleEmitter *pEmitter = dynamic_cast<ParticleEmitter*>( mParticleEmitterList[n].mpSceneObject );
            if ( pEmitter && pEmitter->getEmitterVisible() )
            {
                // Interpolate Emitter.
                pEmitter->interpolateObject( timeDelta );
            }
        }
    }
}

//-----------------------------------------------------------------------------

void ParticleEffect::sceneRender( const SceneRenderState* pSceneRenderState, const SceneRenderRequest* pSceneRenderRequest, BatchRender* pBatchRenderer )
{
    // Render particles?
    if ( !mEffectPlaying || mCameraIdle )
        return;

    // Render all emitters.
    for ( S32 n = mParticleEmitterList.size()-1; n >= 0 ; n-- )
    {
        ParticleEmitter *pEmitter = dynamic_cast<ParticleEmitter*>( mParticleEmitterList[n].mpSceneObject );
        if ( pEmitter && pEmitter->getEmitterVisible() )
        {
            pEmitter->sceneRender( pSceneRenderState, pSceneRenderRequest, pBatchRenderer );
        }
    }
}

//-----------------------------------------------------------------------------

void ParticleEffect::sceneRenderOverlay( const SceneRenderState* sceneRenderState )
{
    // Call parent.
    Parent::sceneRenderOverlay( sceneRenderState );

    // Get Scene.
    Scene* pScene = getScene();

    // Cannot do anything without scene!
    if ( !pScene )
        return;

    // Finish if we shouldn't be drawing the debug overlay.
    if ( !pScene->getIsEditorScene() || mCameraIdleDistance <= 0.0f || !isEnabled() || !getVisible() )
        return;

    // Draw camera pause distance.
    pScene->mDebugDraw.DrawCircle( getRenderPosition(), mCameraIdleDistance, b2Color(1.0f, 1.0f, 0.0f ) );
}

//-----------------------------------------------------------------------------

void ParticleEffect::onTamlAddParent( SimObject* pParentObject )
{
    Parent::onTamlAddParent( pParentObject );

    playEffect(true);
}

//-----------------------------------------------------------------------------

bool ParticleEffect::setEffectFile(void *obj, const char *data)
{
   ParticleEffect* effect = static_cast<ParticleEffect*>(obj);
   if (effect->getScene())
   {
      effect->loadEffect(data);
      return false;
   }

   else if ( Scene::LoadingScene != NULL )
   {
      effect->mpTargetScene = Scene::LoadingScene;
      effect->loadEffect(data);
      return false;
   }

   return true;
}

//-----------------------------------------------------------------------------

bool ParticleEffect::setEffectMode(void* obj, const char* data)
{
    ParticleEffect *object = static_cast<ParticleEffect*>(obj);
    object->setEffectLifeMode( getEffectMode( data ), object->getEffectLifeTime() );
    return false;
}

//-----------------------------------------------------------------------------

void ParticleEffect::clearGraphSelections( void )
{
   // Destroy Graph Selections.
   for ( U32 n = 0; n < (U32)mGraphSelectionList.size(); n++ )
      delete mGraphSelectionList[n];

   // Clear List.
   mGraphSelectionList.clear();
}

//-----------------------------------------------------------------------------

void ParticleEffect::addGraphSelection( const char* graphName, GraphField* pGraphObject )
{
   // Generate new Graph Selection.
   tGraphSelection* pGraphSelection = new tGraphSelection;

   // Populate Graph Selection.
   pGraphSelection->mGraphName = StringTable->insert( graphName );
   pGraphSelection->mpGraphObject = pGraphObject;

   // Put into Graph Selection List.
   mGraphSelectionList.push_back( pGraphSelection );
}

//-----------------------------------------------------------------------------

GraphField* ParticleEffect::findGraphSelection( const char* graphName ) const
{
   // Search For Selected Graph and return if found.
   for ( U32 n = 0; n < (U32)mGraphSelectionList.size(); n++ )
      if ( mGraphSelectionList[n]->mGraphName == StringTable->insert(graphName) )
         return mGraphSelectionList[n]->mpGraphObject;

   // Return "Not Found".
   return NULL;
}

//-----------------------------------------------------------------------------

void ParticleEffect::initialise( void )
{
   // ****************************************************************************
   // Initialise Graph Selections.
   // ****************************************************************************
   addGraphSelection( "particlelife_scale", &mParticleLife.GraphField_Base );
   addGraphSelection( "quantity_scale", &mQuantity.GraphField_Base );
   addGraphSelection( "sizex_scale", &mSizeX.GraphField_Base );
   addGraphSelection( "sizey_scale", &mSizeY.GraphField_Base );
   addGraphSelection( "speed_scale", &mSpeed.GraphField_Base );
   addGraphSelection( "spin_scale", &mSpin.GraphField_Base );
   addGraphSelection( "fixedforce_scale", &mFixedForce.GraphField_Base );
   addGraphSelection( "randommotion_scale", &mRandomMotion.GraphField_Base );
   addGraphSelection( "visibility_scale", &mVisibility.GraphField_Base );
   addGraphSelection( "emissionforce_base", &mEmissionForce.GraphField_Base );
   addGraphSelection( "emissionforce_var", &mEmissionForce.GraphField_Variation );
   addGraphSelection( "emissionangle_base", &mEmissionAngle.GraphField_Base );
   addGraphSelection( "emissionangle_var", &mEmissionAngle.GraphField_Variation );
   addGraphSelection( "emissionarc_base", &mEmissionArc.GraphField_Base );
   addGraphSelection( "emissionarc_var", &mEmissionArc.GraphField_Variation );


   // ****************************************************************************
   // Initialise Graphs.
   // ****************************************************************************
   mParticleLife.GraphField_Base.setValueBounds( 1000.0f, 0.0f, 100.0f, 1.0f );
   mQuantity.GraphField_Base.setValueBounds( 1000.0f, 0.0f, 100.0f, 1.0f ); 
   mSizeX.GraphField_Base.setValueBounds( 1000.0f, 0.0f, 100.0f, 1.0f ); 
   mSizeY.GraphField_Base.setValueBounds( 1000.0f, 0.0f, 100.0f, 1.0f ); 
   mSpeed.GraphField_Base.setValueBounds( 1000.0f, 0.0f, 100.0f, 1.0f ); 
   mSpin.GraphField_Base.setValueBounds( 1000.0f, -100.0f, 100.0f, 1.0f ); 
   mFixedForce.GraphField_Base.setValueBounds( 1000.0f, -100.0f, 100.0f, 1.0f ); 
   mRandomMotion.GraphField_Base.setValueBounds( 1000.0f, 0.0f, 100.0f, 1.0f ); 
   mVisibility.GraphField_Base.setValueBounds( 1000.0f, 0.0f, 100.0f, 1.0f ); 
   mEmissionForce.GraphField_Base.setValueBounds( 1000.0f, -100.0f, 100.0f, 5.0f );
   mEmissionForce.GraphField_Variation.setValueBounds( 1000.0f, 0.0f, 200.0f, 0.0f );
   mEmissionAngle.GraphField_Base.setValueBounds( 1000.0f, -180.0f, 180.0f, 0.0f ); 
   mEmissionAngle.GraphField_Variation.setValueBounds( 1000.0f, 0.0f, 360.0f, 0.0f );
   mEmissionArc.GraphField_Base.setValueBounds( 1000.0f, 0.0f, 360.0f, 360.0f );
   mEmissionArc.GraphField_Variation.setValueBounds( 1000.0f, 0.0f, 720.0f, 0.0f );

   // Stop Effect.
   stopEffect(false, false);

   // Flag Initialised.
   mInitialised = true;
}

//-----------------------------------------------------------------------------

bool ParticleEffect::isEmitterValid( SimObjectId emitterID ) const
{
   // Search for emitter ID.
   for ( U32 n = 0; n < (U32)mParticleEmitterList.size(); n++ )
      if ( mParticleEmitterList[n].mObjectId == emitterID )
         // Return "Valid".
         return true;

   // Return "Not Valid".
   return false;
}

//-----------------------------------------------------------------------------

S32 ParticleEffect::findEmitterIndex( SimObjectId emitterID ) const
{
   // Search for emitter ID.
   for ( U32 n = 0; n < (U32)mParticleEmitterList.size(); n++ )
      if ( mParticleEmitterList[n].mObjectId == emitterID )
         // Return Index.
         return n;

   // Return "Not Found".
   return -1;
}

//-----------------------------------------------------------------------------

ParticleEmitter* ParticleEffect::addEmitter( ParticleEmitter* pEmitter )
{
   // Stop the Effect (if it's playing).
   if ( mEffectPlaying )
      stopEffect(false, false);

   if (!pEmitter)
   {
      // Create new Particle Emitter.
      pEmitter = new ParticleEmitter();

      // Register the Object.
      pEmitter->registerObject();

      // Initialise.
      pEmitter->initialise( this );

      // Format and Set Default Emitter Name.
      char emitterName[256];
      dSprintf( emitterName, 256, "Emitter_%d", mEmitterSerial++ );
      pEmitter->setEmitterName( emitterName );
   }

   // Add to Emitter List.
   tEmitterHandle emitterItem = { pEmitter->getId(), (SimObject*)pEmitter };
   mParticleEmitterList.push_back( emitterItem );

   // Return Emitter ID.
   return pEmitter;
}

//-----------------------------------------------------------------------------

void ParticleEffect::removeEmitter( SimObjectId emitterID, bool deleteEmitter )
{
   // Check Emitter Index.
   if ( !isEmitterValid(emitterID) )
   {
      // Warn.
      Con::warnf("ParticleEffect::removeEmitter() - Invalid Emitter Id! (%d)", emitterID);
      return;
   }

   // Stop the Effect (if it's playing).
   if ( mEffectPlaying )
      stopEffect(false, false);

   // Find Emitter Index.
   S32 index = findEmitterIndex(emitterID);

   // Stop if we can't find this emitter.
   if ( index == -1 )
      return;

   // Destroy Emitter.
   if (deleteEmitter)
   {
      ParticleEmitter *pEmitter = dynamic_cast<ParticleEmitter*>( Sim::findObject( mParticleEmitterList[index].mObjectId ) );
      if( pEmitter )
         pEmitter->deleteObject();
   }

   // Remove from Emitter List.
   mParticleEmitterList.erase( index );
}

//-----------------------------------------------------------------------------

void ParticleEffect::clearEmitters( void )
{
   // Stop Effect.
   stopEffect(false, false);

   // Destroy All Emitters.
   for ( U32 n = 0; n < (U32)mParticleEmitterList.size(); n++ )
   {
      ParticleEmitter *pEmitter = dynamic_cast<ParticleEmitter*>( Sim::findObject( mParticleEmitterList[n].mObjectId ) );
      if( pEmitter )
         pEmitter->deleteObject();
   }

   // Clear Emitter List.
   mParticleEmitterList.clear();
}

//-----------------------------------------------------------------------------

S32 ParticleEffect::getEmitterCount( void ) const
{
   // Get Emitter Count.
   return mParticleEmitterList.size();
}

//-----------------------------------------------------------------------------

SimObjectId ParticleEffect::findEmitterObject( const char* emitterName ) const
{
   // Any Emitters?
   if ( getEmitterCount() > 0 )
   {
      // Yes, so insert search name into String-Table.
      StringTableEntry searchName = StringTable->insert( emitterName );

      // Search for Emitter.
      for ( U32 n = 0; n < (U32)mParticleEmitterList.size(); n++ )
      {
         ParticleEmitter *pEmitter = dynamic_cast<ParticleEmitter*>( Sim::findObject( mParticleEmitterList[n].mObjectId ) );
         if( pEmitter && searchName == pEmitter->getEmitterName() )
            // Yes, so return Emitter ID.
            return mParticleEmitterList[n].mObjectId;
      }
   }

   // No Emitter Found.
   return 0;
}

//-----------------------------------------------------------------------------

SimObjectId ParticleEffect::getEmitterObject( S32 index ) const
{
   // Check Emitter Index.
   if ( index >= getEmitterCount() )
   {
      // Warn.
      Con::warnf("ParticleEffect::getEmitterId() - Invalid Emitter Index (%d)", index);
      return 0;
   }

   // Return Emitter ID.
   return mParticleEmitterList[index].mObjectId;
}

//-----------------------------------------------------------------------------

void ParticleEffect::moveEmitter( S32 fromIndex, S32 toIndex )
{
   // Check From Emitter Index.
   if ( fromIndex < 0 || fromIndex >= getEmitterCount() )
   {
      // Warn.
      Con::warnf("ParticleEffect::moveEmitter() - Invalid From-Emitter-Index (%d)", fromIndex);
      return;
   }

   // Check To Emitter Index.
   if ( toIndex < 0 || toIndex >= getEmitterCount() )
   {
      // Warn.
      Con::warnf("ParticleEffect::moveEmitter() - Invalid To-Emitter-Index (%d)", toIndex);
      return;
   }


   // We need to skip an object if we're inserting above the object.
   if ( toIndex > fromIndex )
      toIndex++;
   else
      fromIndex++;

   // Fetch Emitter to be moved.
   typeEmitterVector::iterator fromItr = (mParticleEmitterList.address()+fromIndex);

   // Fetch Emitter to be inserted at.
   typeEmitterVector::iterator toItr = (mParticleEmitterList.address()+toIndex);

   // Insert Object at new Position.
   mParticleEmitterList.insert( toItr, (*fromItr) );

   // Remove Original Reference.
   mParticleEmitterList.erase( fromItr );
}

//-----------------------------------------------------------------------------

void ParticleEffect::selectGraph( const char* graphName )
{
   // Find and Selected Graph.
   mpCurrentGraph = findGraphSelection( graphName );

   // Was it a registered graph?
   if ( mpCurrentGraph )
   {
      // Yes, so store name.
      mCurrentGraphName = StringTable->insert( graphName );
   }
   else
   {
      // No, so warn.
      Con::warnf( "ParticleEffect::selectGraph() - Invalid Graph Selected! (%s)", graphName );
      return;
   }
}

//-----------------------------------------------------------------------------

S32 ParticleEffect::addDataKey( F32 time, F32 value )
{
   // Have we got a valid Graph Selected?
   if ( !mpCurrentGraph )
   {
      // No, so warn.
      Con::warnf( "ParticleEffect::addDataKey() - No Graph Selected!" );
      return -1;
   }

   // Add Data Key.
   return mpCurrentGraph->addDataKey( time, value );
}

//-----------------------------------------------------------------------------

bool ParticleEffect::removeDataKey( S32 index )
{
   // Have we got a valid Graph Selected?
   if ( !mpCurrentGraph )
   {
      // No, so warn.
      Con::warnf( "ParticleEffect::removeDataKey() - No Graph Selected!" );
      return false;
   }

   // Remove Data Key.
   return mpCurrentGraph->removeDataKey( index );
}

//-----------------------------------------------------------------------------

bool ParticleEffect::clearDataKeys( void )
{
   // Have we got a valid Graph Selected?
   if ( !mpCurrentGraph )
   {
      // No, so warn.
      Con::warnf( "ParticleEffect::clearDataKeys() - No Graph Selected!" );
      return false;
   }

   // Clear Data Keys
   mpCurrentGraph->clearDataKeys();

   // Return Okay.
   return true;
}

//-----------------------------------------------------------------------------

bool ParticleEffect::setDataKey( S32 index, F32 value )
{
   // Have we got a valid Graph Selected?
   if ( !mpCurrentGraph )
   {
      // No, so warn.
      Con::warnf( "ParticleEffect::setDataKey() - No Graph Selected!" );
      return false;
   }

   // Set Data Key.
   return mpCurrentGraph->setDataKeyValue( index, value );
}

//-----------------------------------------------------------------------------

F32 ParticleEffect::getDataKeyValue( S32 index ) const
{
   // Have we got a valid Graph Selected?
   if ( !mpCurrentGraph )
   {
      // No, so warn.
      Con::warnf( "ParticleEffect::getDataKeyValue() - No Graph Selected!" );
      return false;
   }

   // Get Data Key Value.
   return mpCurrentGraph->getDataKeyValue( index );
}

//-----------------------------------------------------------------------------

F32 ParticleEffect::getDataKeyTime( S32 index ) const
{
   // Have we got a valid Graph Selected?
   if ( !mpCurrentGraph )
   {
      // No, so warn.
      Con::warnf( "ParticleEffect::getDataKeyTime() - No Graph Selected!" );
      return false;
   }

   // Get Data Key Time.
   return mpCurrentGraph->getDataKeyTime( index );
}

//-----------------------------------------------------------------------------

U32 ParticleEffect::getDataKeyCount( void ) const
{
   // Have we got a valid Graph Selected?
   if ( !mpCurrentGraph )
   {
      // No, so warn.
      Con::warnf( "ParticleEffect::getDataKeyCount() - No Graph Selected!" );
      return false;
   }

   // Get Data Key Count.
   return mpCurrentGraph->getDataKeyCount();
}

//-----------------------------------------------------------------------------

F32 ParticleEffect::getMinValue( void ) const
{
   // Have we got a valid Graph Selected?
   if ( !mpCurrentGraph )
   {
      // No, so warn.
      Con::warnf( "ParticleEffect::getMinValue() - No Graph Selected!" );
      return false;
   }

   // Get Min Value.
   return mpCurrentGraph->getMinValue();
}

//-----------------------------------------------------------------------------

F32 ParticleEffect::getMaxValue( void ) const
{
   // Have we got a valid Graph Selected?
   if ( !mpCurrentGraph )
   {
      // No, so warn.
      Con::warnf( "ParticleEffect::getMaxValue() - No Graph Selected!" );
      return false;
   }

   // Get Max Value.
   return mpCurrentGraph->getMaxValue();
}

//-----------------------------------------------------------------------------

F32 ParticleEffect::getMinTime( void ) const
{
   // Have we got a valid Graph Selected?
   if ( !mpCurrentGraph )
   {
      // No, so warn.
      Con::warnf( "ParticleEffect::getMinTime() - No Graph Selected!" );
      return false;
   }

   // Get Min Time.
   return mpCurrentGraph->getMinTime();
}

//-----------------------------------------------------------------------------

F32 ParticleEffect::getMaxTime( void ) const
{
   // Have we got a valid Graph Selected?
   if ( !mpCurrentGraph )
   {
      // No, so warn.
      Con::warnf( "ParticleEffect::getMaxTime() - No Graph Selected!" );
      return false;
   }

   // Get Max Time.
   return mpCurrentGraph->getMaxTime();
}

//-----------------------------------------------------------------------------

F32 ParticleEffect::getGraphValue( F32 time ) const
{
   // Have we got a valid Graph Selected?
   if ( !mpCurrentGraph )
   {
      // No, so warn.
      Con::warnf( "ParticleEffect::getGraphValue() - No Graph Selected!" );
      return false;
   }

   // Get Graph Value.
   return mpCurrentGraph->getGraphValue( time );
}

//-----------------------------------------------------------------------------

bool ParticleEffect::setTimeRepeat( const F32 timeRepeat )
{
   // Have we got a valid Graph Selected?
   if ( !mpCurrentGraph )
   {
      // No, so warn.
      Con::warnf( "ParticleEffect::setTimeRepeat() - No Graph Selected!" );
      return false;
   }

   // Set Time Repeat.
   return mpCurrentGraph->setTimeRepeat( timeRepeat );
}

//-----------------------------------------------------------------------------

F32 ParticleEffect::getTimeRepeat( void ) const
{
   // Have we got a valid Graph Selected?
   if ( !mpCurrentGraph )
   {
      // No, so warn.
      Con::warnf( "ParticleEffect::getTimeRepeat() - No Graph Selected!" );
      return false;
   }

   // Get Time Repeat.
   return mpCurrentGraph->getTimeRepeat();
}

//-----------------------------------------------------------------------------

bool ParticleEffect::setValueScale( const F32 valueScale )
{
   // Have we got a valid Graph Selected?
   if ( !mpCurrentGraph )
   {
      // No, so warn.
      Con::warnf( "ParticleEffect::setValueScale() - No Graph Selected!" );
      return false;
   }

   // Set Value Scale.
   return mpCurrentGraph->setValueScale( valueScale );
}

//-----------------------------------------------------------------------------

F32 ParticleEffect::getValueScale( void ) const
{
   // Have we got a valid Graph Selected?
   if ( !mpCurrentGraph )
   {
      // No, so warn.
      Con::warnf( "ParticleEffect::getValueScale() - No Graph Selected!" );
      return false;
   }

   // Get Value Scale.
   return mpCurrentGraph->getValueScale();
}

//-----------------------------------------------------------------------------

bool ParticleEffect::findParticlePeak( const F32 searchTime, const F32 timeStep, const U32 peakLimit, U32& peakCount, F32& peakTime )
{
   // Cannot do anything if we've not got any emitters!
   if ( mParticleEmitterList.size() == 0 )
   {
      // Warn.
      Con::warnf("ParticleEffect::findParticlePeak() - Cannot Play; no emitters!");
      return false;
   }

   // Are we in a Scene?
   if ( getScene() == NULL )
   {
      // No, so warn.
      Con::warnf("ParticleEffect::findParticlePeak() - Cannot Play; not in a scene!");
      return false;
   }

   // Check Search-time.
   if ( mLessThanZero( searchTime ) )
   {
      // Problem, so warn.
      Con::warnf("ParticleEffect::findParticlePeak() - Search-Time is invalid!");
      return false;
   }

   // Check Time-Step.
   if ( mIsZero( timeStep ) || mGreaterThan( timeStep, searchTime ) )
   {
      // Problem, so warn.
      Con::warnf("ParticleEffect::findParticlePeak() - Time-Step is invalid!");
      return false;
   }

   // Reset Effect Age.
   mEffectAge = 0.0f;

   // Play All Emitters.
   for ( U32 n = 0; n < (U32)mParticleEmitterList.size(); n++ )
   {
      ParticleEmitter *pEmitter = dynamic_cast<ParticleEmitter*>( Sim::findObject( mParticleEmitterList[n].mObjectId ) );
      if( pEmitter )
         pEmitter->playEmitter( true );
   }

   // Reset Waiting for Particles.
   mWaitingForParticles = false;
   // Reset Waiting for delete.
   mWaitingForDelete = false;

   // Flag as Playing.
   mEffectPlaying = true;

   // Set Unsafe Delete!
   setSafeDelete(false);

   // Now we'll move to the selected position using the selected intervals...

   // Calculate Number of intervals.
   const U32 intervalCount = U32(mFloor(searchTime / timeStep));
   // Calculate Final sub-interval.
   const F32 finalIntervalTime = searchTime - ( intervalCount * timeStep );

   // Debug Statistics.
   DebugStats debugStats;

   // Get Quicker Reference to active-particle count.
   U32& particlesActive = debugStats.particlesUsed;

   // Reset Scene Time.
   F32 totalTime = 0.0f;

   // Reset "Found" Flag.
   bool limitFound = false;

   // Reset Maximum Particle Tracking.
   peakCount = 0;
   peakTime = 0.0f;

   // Step the appropriate number of intervals.
   for ( U32 i = 0; i < intervalCount; ++i )
   {
      // Move Scene Time.
      totalTime += timeStep;

      // Reset Particles Active.
      particlesActive = 0;

      // Update All Emitters for the main intervals.
      for ( U32 n = 0; n < (U32)mParticleEmitterList.size(); ++n )
      {
         // Integrate Object.
         ParticleEmitter* pEmitter = dynamic_cast<ParticleEmitter*>( Sim::findObject( mParticleEmitterList[n].mObjectId ) );
         if( pEmitter )
            pEmitter->integrateObject( totalTime, timeStep, &debugStats );
      }

      // Check Peak Particle Count.
      if ( particlesActive > peakCount )
      {
         // Check we don't go over the limit.
         if ( particlesActive > peakLimit )
         {
            // We have so flag "found".
            limitFound = true;
            break;
         }

         // Set peak-count/time.
         peakCount = particlesActive;
         peakTime = totalTime;
      }
   }

   // Only continue searching if we've not found the limit.
   if ( !limitFound )
   {
      // Move Scene Time.
      totalTime += finalIntervalTime;

      // Reset Particles Active.
      particlesActive = 0;

      // Update All Emitters for the final sub-interval.
      for ( U32 n = 0; n < (U32)mParticleEmitterList.size(); ++n )
      {
         // Integrate Object.
         ParticleEmitter* pEmitter = dynamic_cast<ParticleEmitter*>( Sim::findObject( mParticleEmitterList[n].mObjectId ) );
         if( pEmitter )
            pEmitter->integrateObject( totalTime, finalIntervalTime, &debugStats );
      }

      // Check Peak Particle Count.
      if ( particlesActive > peakCount )
      {
         // Check we don't go over the limit.
         if ( particlesActive > peakLimit )
         {
            // We have so flag "found".
            limitFound = true;
         }
         else
         {
            // Set peak-count/time.
            peakCount = particlesActive;
            peakTime = totalTime;
         }
      }
   }

   // Stop All Emitters.
   stopEffect( false, false );

   // Return Okay.
   return true;
}

//-----------------------------------------------------------------------------

bool ParticleEffect::moveEffectTo( const F32 moveTime, const F32 timeStep, U32& peakCount, F32& peakTime )
{
   // Cannot do anything if we've not got any emitters!
   if ( mParticleEmitterList.size() == 0 )
   {
      // Warn.
      Con::warnf("ParticleEffect::MoveEffectTo() - Cannot Play; no emitters!");
      return false;
   }

   // Are we in a Scene?
   if ( getScene() == NULL )
   {
      // No, so warn.
      Con::warnf("ParticleEffect::MoveEffectTo() - Cannot Play; not in a scene!");
      return false;
   }

   // Check Move-time.
   if ( mLessThanZero( moveTime ) )
   {
      // Problem, so warn.
      Con::warnf("ParticleEffect::MoveEffectTo() - Move-Time is invalid!");
      return false;
   }

   // Check Time-Step.
   if ( mIsZero( timeStep ) || mGreaterThan( timeStep, moveTime ) )
   {
      // Problem, so warn.
      Con::warnf("ParticleEffect::MoveEffectTo() - Time-Step is invalid!");
      return false;
   }

   // Reset Effect Age.
   mEffectAge = 0.0f;

   // Play All Emitters.
   for ( U32 n = 0; n < (U32)mParticleEmitterList.size(); n++ )
   {
      ParticleEmitter* pEmitter = dynamic_cast<ParticleEmitter*>( Sim::findObject( mParticleEmitterList[n].mObjectId ) );
      if( pEmitter )
         pEmitter->playEmitter( true );
   }

   // Reset Waiting for Particles.
   mWaitingForParticles = false;
   // Reset Waiting for delete.
   mWaitingForDelete = false;

   // Flag as Playing.
   mEffectPlaying = true;

   // Set Unsafe Delete!
   setSafeDelete(false);

   // Now we'll move to the selected position using the selected intervals...

   // Calculate Number of intervals.
   const U32 intervalCount = U32(mFloor(moveTime / timeStep));
   // Calculate Final sub-interval.
   const F32 finalIntervalTime = moveTime - ( intervalCount * timeStep );

   // Debug Statistics.
   DebugStats debugStats;
   // Get Quicker Reference to active-particle count.
   U32& particlesActive = debugStats.particlesUsed;

   // Reset Scene Time.
   F32 totalTime = 0.0f;

   // Reset Maximum Particle Tracking.
   peakCount = 0;
   peakTime = 0.0f;

   // Step the appropriate number of intervals.
   for ( U32 i = 0; i < intervalCount; ++i )
   {
      // Move Scene Time.
      totalTime += timeStep;

      // Reset Particles Active.
      particlesActive = 0;

      // Update All Emitters for the main intervals.
      for ( U32 n = 0; n < (U32)mParticleEmitterList.size(); ++n )
      {
         // Integrate Object.
         ParticleEmitter* pEmitter = dynamic_cast<ParticleEmitter*>( Sim::findObject( mParticleEmitterList[n].mObjectId ) );
         if( pEmitter )
            pEmitter->integrateObject( totalTime, timeStep, &debugStats );
      }

      // Check Peak Particle Count.
      if ( particlesActive > peakCount )
      {
         peakCount = particlesActive;
         peakTime = totalTime;
      }
   }

   // Move Scene Time.
   totalTime += finalIntervalTime;

   // Reset Particles Active.
   particlesActive = 0;

   // Update All Emitters for the final sub-interval.
   for ( U32 n = 0; n < (U32)mParticleEmitterList.size(); ++n )
   {
      // Integrate Object.
      ParticleEmitter* pEmitter = dynamic_cast<ParticleEmitter*>( Sim::findObject( mParticleEmitterList[n].mObjectId ) );
      if( pEmitter )
         pEmitter->integrateObject( totalTime, finalIntervalTime, &debugStats );
   }

   // Check Peak Particle Count.
   if ( particlesActive > peakCount )
   {
      peakCount = particlesActive;
      peakTime = totalTime;
   }

   // Return Okay.
   return true;
}

//-----------------------------------------------------------------------------

bool ParticleEffect::playEffect( bool resetParticles )
{
   // Cannot do anything if we've not got any emitters!
   if ( mParticleEmitterList.size() == 0 )
   {
      // Warn.
      Con::warnf("ParticleEffect::playEffect() - Cannot Play; no emitters!");
      return false;
   }

   // Are we in a Scene?
   if ( getScene() == NULL )
   {
      // No, so warn.
      Con::warnf("ParticleEffect::playEffect() - Cannot Play; not in a scene!");
      return false;
   }

   // Reset Effect Age.
   mEffectAge = 0.0f;

   // Play All Emitters.
   for ( U32 n = 0; n < (U32)mParticleEmitterList.size(); n++ )
   {
      ParticleEmitter* pEmitter = dynamic_cast<ParticleEmitter*>( Sim::findObject( mParticleEmitterList[n].mObjectId ) );
      if( pEmitter )
         pEmitter->playEmitter( resetParticles );
   }

   // Reset Waiting for Particles.
   mWaitingForParticles = false;
   // Reset Waiting for delete.
   mWaitingForDelete = false;

   // Flag as Playing.
   mEffectPlaying = true;

   // Turn off effect pause.
   mEffectPaused = false;

   // Set Unsafe Delete!
   setSafeDelete(false);

   // Return Okay.
   return true;
}

//-----------------------------------------------------------------------------

void ParticleEffect::stopEffect( bool waitForParticles, bool killEffect )
{
   // Ignore if we're not playing and there's no kill command.
   if ( !mEffectPlaying && !killEffect )
      return;

   // Are we waiting for particles to end?
   if ( waitForParticles )
   {
      // Yes, so pause all emitters.
      for ( U32 n = 0; n < (U32)mParticleEmitterList.size(); n++ )
      {
         ParticleEmitter* pEmitter = dynamic_cast<ParticleEmitter*>( Sim::findObject( mParticleEmitterList[n].mObjectId ) );
         if( pEmitter )
            pEmitter->pauseEmitter();
      }

      // Flag Waiting for Particles.
      mWaitingForParticles = true;

      // Flag as waiting for deletion if killing effect.
      if ( killEffect )
         mWaitingForDelete = true;
   }
   else
   {
      // No, so stop All Emitters.
      for ( U32 n = 0; n < (U32)mParticleEmitterList.size(); n++ )
      {
         // Fetch Particle Emitter Pointer.
         ParticleEmitter* pEmitter = dynamic_cast<ParticleEmitter*>( Sim::findObject( mParticleEmitterList[n].mObjectId ) );
         if( pEmitter )
            pEmitter->stopEmitter();
      }

      // Reset Effect Age.
      mEffectAge = 0.0f;

      // Flag as Stopped and not waiting.
      mEffectPlaying = mWaitingForParticles = mWaitingForDelete = false;

       // Turn off effect pause.
       mEffectPaused = false;

      // Set Safe Delete.
      setSafeDelete(true);

      // Perform "OnStopEffect" Callback.
      if( isMethod( "onStopEffect" ) )
         Con::executef( this, 1, "onStopEffect" );

      // Flag for immediate Deletion if killing.
      if ( killEffect )
         safeDelete();
   }
}

//-----------------------------------------------------------------------------

void ParticleEffect::setEffectLifeMode( eEffectLifeMode lifeMode, F32 time )
{
   // Set Effect-Life Mode.
   mEffectLifeMode = lifeMode;

   setEffectLifeTime( time );
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEffect, getEffectLifeMode, const char*, 2, 2, "() \n @return Returns the Effect Life Mode (either INFINITE CYCLE KILL or STOP).")
{
   // Fetch Effect Life Mode.
   ParticleEffect::eEffectLifeMode lifeMode = object->getEffectLifeMode();

   // Search for Mnemonic.
   for(U32 i = 0; i < (sizeof(effectLifeLookup) / sizeof(effectLifeLookup[0])); i++)
      if( effectLifeLookup[i].index == lifeMode )
      {
         // Create Returnable Buffer.
         char* pBuffer = Con::getReturnBuffer(128);
         // Format Buffer.
         dSprintf(pBuffer, 128, "%s %f", effectLifeLookup[i].label, object->getEffectLifeTime() == ParticleEffect::INFINITE ? 0.0f : object->getEffectLifeTime());
         // Return buffer.
         return pBuffer;
      }

      // Bah!
      return NULL;
}

//-----------------------------------------------------------------------------

bool ParticleEffect::loadEffect( const char* effectFile )
{
   // Are we in a Scene?
   if ( !getScene() )
   {
      // No, so warn.
      Con::warnf("ParticleEffect::loadEffect() - Cannot Load-Effect; not in a scene!");
      // Return Error.
      return false;
   }

   // Expand relative paths.
   char buffer[1024];
   if ( effectFile )
      if ( Con::expandPath( buffer, sizeof( buffer ), effectFile ) )
         effectFile = buffer;

   // Open Effect File.
   Stream* pStream = ResourceManager->openStream( effectFile );
   // Check Stream.
   if ( !pStream )
   {
      // Warn.
      Con::warnf("ParticleEffect::loadEffect() - Could not Open File '%s' for Effect Load.", effectFile);
      // Return Error.
      return false;
   }

   // Scene Objects.
   Vector<SceneObject*> ObjReferenceList;

   // Set Vector Associations.
   VECTOR_SET_ASSOCIATION( ObjReferenceList );

   // Load Stream.
   if ( !loadStream( *pStream, getScene(), ObjReferenceList, true  ) )
   {
      // Warn.
      Con::warnf("ParticleEffect::loadEffect() - Error Loading Effect/Emitter(s)!");
      // Return Error.
      return false;
   }

   // Reset Any Lifetime Counter.
   setLifetime( 0.0f );

   // Close Stream.
   ResourceManager->closeStream( pStream );

   mEffectFile = StringTable->insert(effectFile);

   // Return Okay.
   return true;
}

//-----------------------------------------------------------------------------

bool ParticleEffect::saveEffect( const char* effectFile )
{
   // Are we in a Scene?
   if ( getScene() == NULL )
   {
      // No, so warn.
      Con::warnf("ParticleEffect::saveEffect() - Cannot Save-Effect; not in a scene!");
      // Return Error.
      return false;
   }

   // Expand relative paths.
   char buffer[1024];
   if ( effectFile )
      if ( Con::expandPath( buffer, sizeof( buffer ), effectFile ) )
         effectFile = buffer;

   // Open Effect File.
   FileStream fileStream;
   if ( !ResourceManager->openFileForWrite( fileStream, effectFile, FileStream::Write ) )
   {
      // Warn.
      Con::warnf("ParticleEffect::saveEffect() - Could not open File '%s' for Effect Save.", effectFile);
      // Return Error.
      return false;
   }

   // Stop Effect.
   // NOTE:-   We do this so that we don't save active emitter particles.
   stopEffect(false, false);

   // Save Stream.
   if ( !saveStream( fileStream, getScene()->getNextSerialiseID(), 1 ) )
   {
      // Warn.
      Con::warnf("ParticleEffect::saveEffect() - Error Saving Effect/Emitter(s)!");
      // Return Error.
      return false;
   }

   Con::executef( this, 1, "onEffectSaved" );

   // Return Okay.
   return true;
}

//-----------------------------------------------------------------------------

#if 0
bool ParticleEffect::checkCollisionSend( const F32 elapsedTime, t2dPhysics::cCollisionStatus& sendCollisionStatus, DebugStats* pDebugStats )
{
   // Use Effect Collisions?
   if ( mUseEffectCollisions )
   {
      // Yes, so use standard core-collision.
      return Parent::checkCollisionSend( elapsedTime, sendCollisionStatus, pDebugStats );
   }


   // Debug Profiling.
#ifdef TORQUE_ENABLE_PROFILER
   PROFILE_START(ParticleEffect_CheckCollisionSend);
#endif

   // Reset Initial Collision Status.
   bool collisionStatus = false;

   // Update all emitters.
   for ( U32 n = 0; n < mParticleEmitterList.size(); n++ )
   {
      // Fetch Particle Emitter Reference.
      ParticleEmitter* pEmitter = dynamic_cast<ParticleEmitter*>( mParticleEmitterList[n].mpSceneObject );

      // Integrate Emitter (if visible).
      if ( pEmitter->getVisible() && pEmitter->getEmitterCollisionStatus() )
      {
         // Check Particle Collision.
         if ( pEmitter->checkParticleCollisions( this, elapsedTime, sendCollisionStatus, pDebugStats ) )
         {
            // Flag Collision Occurred.
            collisionStatus = true;
         }
      }
   }

   // Did a collision occur.
   if ( collisionStatus )
   {
      // Yes, so perform callback.
      Con::executef( this, 1, "onParticleCollision" );
   }

   // Debug Profiling.
#ifdef TORQUE_ENABLE_PROFILER
   PROFILE_END();   // ParticleEffect_CheckCollisionSend
#endif

   // Return No Collision Always.
   // NOTE:-   This stops the particle-effect object from ever receiving collisions itself!
   return false;
}
#endif

//-----------------------------------------------------------------------------
// Serialisation.
//-----------------------------------------------------------------------------

// Register Handlers.
REGISTER_SERIALISE_START( ParticleEffect )
REGISTER_SERIALISE_VERSION( ParticleEffect, 1, false )
REGISTER_SERIALISE_VERSION( ParticleEffect, 2, false )
REGISTER_SERIALISE_VERSION( ParticleEffect, 3, false )
REGISTER_SERIALISE_END()

// Implement Base Serialisation.
IMPLEMENT_SERIALISE_PARENT( ParticleEffect, 3 )


//-----------------------------------------------------------------------------
// Load v1
//-----------------------------------------------------------------------------
IMPLEMENT_2D_LOAD_METHOD( ParticleEffect, 1 )
{
   // Clear Emitters.
   object->clearEmitters();

   // Load Graph Count.
   S32 graphCount;
   if ( !stream.read( &graphCount ) )
      return false;

   // Load Graphs.
   for ( U32 n = 0; n < (U32)graphCount; n++ )
   {
      // Load/Find Graph Name.
      GraphField* pGraphField = object->findGraphSelection( stream.readSTString() );
      // Check Graph Field.
      if ( !pGraphField )
         return false;

      // Load Graph Object.
      if ( !pGraphField->loadStream( SERIALISE_LOAD_ARGS_PASS ) )
         return false;
   }

   // Load Graph Selection Flag.
   bool graphSelection;
   if ( !stream.read( &graphSelection ) )
      return false;

   // Do we have a Graph Selection?
   if ( graphSelection )
      // Yes, so Read Graph Name and Select it.
      object->selectGraph( stream.readSTString() );

   // Load Emitter Count.
   S32 emitterCount;
   if ( !stream.read( &emitterCount ) )
      return false;

   // Load Emitters.
   for ( U32 n = 0; n < (U32)emitterCount; n++ )
   {
      // Add Emitter.
      object->addEmitter();

      // Find Emitter.
      ParticleEmitter *pEmitter = dynamic_cast<ParticleEmitter*>( Sim::findObject( object->mParticleEmitterList[n].mObjectId ) );

      // Error?
      if( !pEmitter )
      {
         Con::warnf("ParticleEmitter::loadStream - Unable to properly add emitter from disk" );
         continue;
      }

      // Load Emitter.
      pEmitter->loadStream( SERIALISE_LOAD_ARGS_PASS );

      // Is Emitter Name Empty?
      if ( dStrlen( pEmitter->getEmitterName() ) == 0 )
      {
         // Yes, so format and Set Default Emitter Name.
         char emitterName[256];
         dSprintf( emitterName, 256, "Emitter_%d", n );
         pEmitter->setEmitterName( emitterName );
      }
   }

   // Load Effect Playing Flag / Age.
   if (    !stream.read( &object->mEffectPlaying ) ||
      !stream.read( &object->mEffectAge ) ||
      !stream.read( &object->mWaitingForParticles ) ||
      !stream.read( &object->mWaitingForDelete ) ||
      !stream.read( (S32*)&object->mEffectLifeMode ) ||
      !stream.read( &object->mEffectLifetime ) )
      return false;

   // Return Okay.
   return true;
}

//-----------------------------------------------------------------------------
// Save v1
//-----------------------------------------------------------------------------
IMPLEMENT_2D_SAVE_METHOD( ParticleEffect, 1 )
{
   // Save Graph Count.
   if ( !stream.write( object->mGraphSelectionList.size() ) )
      return false;

   // Save Graphs.
   for ( U32 n = 0; n < (U32)object->mGraphSelectionList.size(); n++ )
   {
      // Write Graph Name.
      stream.writeString( object->mGraphSelectionList[n]->mGraphName );
      // Write Graph Object.
      if ( !object->mGraphSelectionList[n]->mpGraphObject->saveStream( SERIALISE_SAVE_ARGS_PASS ) )
         return false;
   }

   // Save Graph Selection Flag.
   if ( !stream.write( (object->mpCurrentGraph != NULL) ) )
      return false;

   // Do we have a Graph Selection?
   if ( object->mpCurrentGraph )
      // Yes, so save Graph Selection.
      stream.writeString( object->mCurrentGraphName );

   // Save Emitter Count.
   if ( !stream.write( object->mParticleEmitterList.size() ) )
      return false;

   // Save Emitters.
   for ( U32 n = 0; n < (U32)object->mParticleEmitterList.size(); n++ )
   {
      // Find Emitter.
      ParticleEmitter *pEmitter = dynamic_cast<ParticleEmitter*>( Sim::findObject( object->mParticleEmitterList[n].mObjectId ) );

      // Save Emitter.
      if( pEmitter )
         pEmitter->saveStream( SERIALISE_SAVE_ARGS_PASS );
   }

   // Save Effect Playing Flag / Age.
   if (    !stream.write( object->mEffectPlaying ) ||
      !stream.write( object->mEffectAge ) ||
      !stream.write( object->mWaitingForParticles ) ||
      !stream.write( object->mWaitingForDelete ) ||
      !stream.write( (S32)object->mEffectLifeMode ) ||
      !stream.write( object->mEffectLifetime ) )
      return false;

   // Return Okay.
   return true;
}


//-----------------------------------------------------------------------------
// Load v2
//-----------------------------------------------------------------------------
IMPLEMENT_2D_LOAD_METHOD( ParticleEffect, 2 )
{
   // Clear Emitters.
   object->clearEmitters();

   // Load Graph Count.
   S32 graphCount;
   if ( !stream.read( &graphCount ) )
      return false;

   // Load Graphs.
   for ( U32 n = 0; n < (U32)graphCount; n++ )
   {
      // Load/Find Graph Name.
      GraphField* pGraphField = object->findGraphSelection( stream.readSTString() );
      // Check Graph Field.
      if ( !pGraphField )
         return false;

      // Load Graph Object.
      if ( !pGraphField->loadStream( SERIALISE_LOAD_ARGS_PASS ) )
         return false;
   }

   // Load Graph Selection Flag.
   bool graphSelection;
   if ( !stream.read( &graphSelection ) )
      return false;

   // Do we have a Graph Selection?
   if ( graphSelection )
      // Yes, so Read Graph Name and Select it.
      object->selectGraph( stream.readSTString() );

   // Load Effect Collision Flag.
   bool useEffectCollision;
   if ( !stream.read( &useEffectCollision ) )
      return false;

   // Set use effect collision status.
   object->setUseEffectCollision( useEffectCollision );

   // Load Emitter Count.
   S32 emitterCount;
   if ( !stream.read( &emitterCount ) )
      return false;

   // Load Emitters.
   for ( U32 n = 0; n < (U32)emitterCount; n++ )
   {
      // Add Emitter.
      object->addEmitter();

      // Find Emitter.
      ParticleEmitter *pEmitter = dynamic_cast<ParticleEmitter*>( Sim::findObject( object->mParticleEmitterList[n].mObjectId ) );

      // Error?
      if( !pEmitter )
      {
         Con::warnf("ParticleEmitter::loadStream - Unable to properly add emitter from disk" );
         continue;
      }

      // Load Emitter.
      pEmitter->loadStream( SERIALISE_LOAD_ARGS_PASS );

      // Is Emitter Name Empty?
      if ( dStrlen( pEmitter->getEmitterName() ) == 0 )
      {
         // Yes, so format and Set Default Emitter Name.
         char emitterName[256];
         dSprintf( emitterName, 256, "Emitter_%d", n );
         pEmitter->setEmitterName( emitterName );
      }
   }

   // Load Effect Playing Flag / Age.
   if ( !stream.read( &object->mEffectPlaying ) ||
        !stream.read( &object->mEffectAge ) ||
        !stream.read( &object->mWaitingForParticles ) ||
        !stream.read( &object->mWaitingForDelete ) ||
        !stream.read( (S32*)&object->mEffectLifeMode ) ||
        !stream.read( &object->mEffectLifetime ) )
      return false;

   // Return Okay.
   return true;
}

//-----------------------------------------------------------------------------
// Save v2
//-----------------------------------------------------------------------------
IMPLEMENT_2D_SAVE_METHOD( ParticleEffect, 2 )
{
   // Save Graph Count.
   if ( !stream.write( object->mGraphSelectionList.size() ) )
      return false;

   // Save Graphs.
   for ( U32 n = 0; n < (U32)object->mGraphSelectionList.size(); n++ )
   {
      // Write Graph Name.
      stream.writeString( object->mGraphSelectionList[n]->mGraphName );
      // Write Graph Object.
      if ( !object->mGraphSelectionList[n]->mpGraphObject->saveStream( SERIALISE_SAVE_ARGS_PASS ) )
         return false;
   }

   // Save Graph Selection Flag.
   if ( !stream.write( (object->mpCurrentGraph != NULL) ) )
      return false;

   // Do we have a Graph Selection?
   if ( object->mpCurrentGraph )
      // Yes, so save Graph Selection.
      stream.writeString( object->mCurrentGraphName );

   // Save Effect Collision Status.
   if ( !stream.write( object->getUseEffectCollision() ) )
      return false;

   // Save Emitter Count.
   if ( !stream.write( object->mParticleEmitterList.size() ) )
      return false;

   // Save Emitters.
   for ( U32 n = 0; n < (U32)object->mParticleEmitterList.size(); n++ )
   {
      ParticleEmitter *pEmitter = dynamic_cast<ParticleEmitter*>( Sim::findObject( object->mParticleEmitterList[n].mObjectId ) );
      if( pEmitter )
         pEmitter->saveStream( SERIALISE_SAVE_ARGS_PASS );
   }

   // Save Effect Playing Flag / Age.
   if (    !stream.write( object->mEffectPlaying ) ||
      !stream.write( object->mEffectAge ) ||
      !stream.write( object->mWaitingForParticles ) ||
      !stream.write( object->mWaitingForDelete ) ||
      !stream.write( (S32)object->mEffectLifeMode ) ||
      !stream.write( object->mEffectLifetime ) )
      return false;

   // Return Okay.
   return true;
}

//-----------------------------------------------------------------------------
// Load v3
//-----------------------------------------------------------------------------
IMPLEMENT_2D_LOAD_METHOD( ParticleEffect, 3 )
{
   // Clear Emitters.
   object->clearEmitters();

   // Load Graph Count.
   S32 graphCount;
   if ( !stream.read( &graphCount ) )
      return false;

   // Load Graphs.
   for ( U32 n = 0; n < (U32)graphCount; n++ )
   {
      // Load/Find Graph Name.
      GraphField* pGraphField = object->findGraphSelection( stream.readSTString() );
      // Check Graph Field.
      if ( !pGraphField )
         return false;

      // Load Graph Object.
      if ( !pGraphField->loadStream( SERIALISE_LOAD_ARGS_PASS ) )
         return false;
   }

   // Load Graph Selection Flag.
   bool graphSelection;
   if ( !stream.read( &graphSelection ) )
      return false;

   // Do we have a Graph Selection?
   if ( graphSelection )
      // Yes, so Read Graph Name and Select it.
      object->selectGraph( stream.readSTString() );

   // Load Effect Collision Flag.
   bool useEffectCollision;
   if ( !stream.read( &useEffectCollision ) )
      return false;

   // Set use effect collision status.
   object->setUseEffectCollision( useEffectCollision );

   // Load Emitter Count.
   S32 emitterCount;
   if ( !stream.read( &emitterCount ) )
      return false;

   // Load Emitters.
   for ( U32 n = 0; n < (U32)emitterCount; n++ )
   {
      // Add Emitter.
      object->addEmitter();

      // Find Emitter.
      ParticleEmitter *pEmitter = dynamic_cast<ParticleEmitter*>( Sim::findObject( object->mParticleEmitterList[n].mObjectId ) );

      // Error?
      if( !pEmitter )
      {
         Con::warnf("ParticleEmitter::loadStream - Unable to properly add emitter from disk" );
         continue;
      }

      // Load Emitter.
      pEmitter->loadStream( SERIALISE_LOAD_ARGS_PASS );

      // Is Emitter Name Empty?
      if ( dStrlen( pEmitter->getEmitterName() ) == 0 )
      {
         // Yes, so format and Set Default Emitter Name.
         char emitterName[256];
         dSprintf( emitterName, 256, "Emitter_%d", n );
         pEmitter->setEmitterName( emitterName );
      }
   }

   // Load effect options.
   if ( !stream.read( &object->mEffectPlaying ) ||
        !stream.read( &object->mEffectAge ) ||
        !stream.read( &object->mWaitingForParticles ) ||
        !stream.read( &object->mWaitingForDelete ) ||
        !stream.read( (S32*)&object->mEffectLifeMode ) ||
        !stream.read( &object->mEffectLifetime ) ||
        !stream.read( &object->mCameraIdleDistance ) )
      return false;

   // Return Okay.
   return true;
}

//-----------------------------------------------------------------------------
// Save v3
//-----------------------------------------------------------------------------
IMPLEMENT_2D_SAVE_METHOD( ParticleEffect, 3 )
{
   // Save Graph Count.
   if ( !stream.write( object->mGraphSelectionList.size() ) )
      return false;

   // Save Graphs.
   for ( U32 n = 0; n < (U32)object->mGraphSelectionList.size(); n++ )
   {
      // Write Graph Name.
      stream.writeString( object->mGraphSelectionList[n]->mGraphName );
      // Write Graph Object.
      if ( !object->mGraphSelectionList[n]->mpGraphObject->saveStream( SERIALISE_SAVE_ARGS_PASS ) )
         return false;
   }

   // Save Graph Selection Flag.
   if ( !stream.write( (object->mpCurrentGraph != NULL) ) )
      return false;

   // Do we have a Graph Selection?
   if ( object->mpCurrentGraph )
      // Yes, so save Graph Selection.
      stream.writeString( object->mCurrentGraphName );

   // Save Effect Collision Status.
   if ( !stream.write( object->getUseEffectCollision() ) )
      return false;

   // Save Emitter Count.
   if ( !stream.write( object->mParticleEmitterList.size() ) )
      return false;

   // Save Emitters.
   for ( U32 n = 0; n < (U32)object->mParticleEmitterList.size(); n++ )
   {
      ParticleEmitter *pEmitter = dynamic_cast<ParticleEmitter*>( Sim::findObject( object->mParticleEmitterList[n].mObjectId ) );
      if( pEmitter )
         pEmitter->saveStream( SERIALISE_SAVE_ARGS_PASS );
   }

   // Save effect options.
   if (    !stream.write( object->mEffectPlaying ) ||
      !stream.write( object->mEffectAge ) ||
      !stream.write( object->mWaitingForParticles ) ||
      !stream.write( object->mWaitingForDelete ) ||
      !stream.write( (S32)object->mEffectLifeMode ) ||
      !stream.write( object->mEffectLifetime ) ||
      !stream.write( object->mCameraIdleDistance ) )
      return false;

   // Return Okay.
   return true;
}
