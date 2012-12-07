//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _SCENE_H_
#include "Scene.h"
#endif

#ifndef _DGL_H_
#include "graphics/dgl.h"
#endif

#ifndef _CONSOLETYPES_H_
#include "console/consoleTypes.h"
#endif

#ifndef _BITSTREAM_H_
#include "io/bitStream.h"
#endif

#ifndef _SCENE_OBJECT_H_
#include "2d/sceneobject/SceneObject.h"
#endif

#ifndef _CONTACT_FILTER_H_
#include "ContactFilter.h"
#endif

#ifndef _SCENE_RENDER_OBJECT_H_
#include "2d/SceneRenderObject.h"
#endif

// Script bindings.
#include "Scene_ScriptBinding.h"

// Debug Profiling.
#ifdef TORQUE_ENABLE_PROFILER
#include "debug/profiler.h"
#endif

//------------------------------------------------------------------------------

SimObjectPtr<Scene> Scene::LoadingScene = NULL;

//------------------------------------------------------------------------------

IMPLEMENT_CONOBJECT(Scene);

//------------------------------------------------------------------------------

static ContactFilter mContactFilter;

// Scene counter.
static U32 sSceneCount = 0;
static U32 sSceneMasterIndex = 0;

// Joint property names..
static bool jointPropertiesInitialized = false;

static StringTableEntry jointCollectionName;
static StringTableEntry jointCollideConnectedName;
static StringTableEntry jointObjectAName;
static StringTableEntry jointObjectBName;
static StringTableEntry jointLocalAnchorAName;
static StringTableEntry jointLocalAnchorBName;

static StringTableEntry jointDistanceTypeName;
static StringTableEntry jointDistanceLengthName;
static StringTableEntry jointDistanceFrequencyName;
static StringTableEntry jointDistanceDampingRatioName;

static StringTableEntry jointRopeTypeName;
static StringTableEntry jointRopeMaxLengthName;

static StringTableEntry jointRevoluteTypeName;
static StringTableEntry jointRevoluteLimitLowerAngleName;
static StringTableEntry jointRevoluteLimitUpperAngleName;
static StringTableEntry jointRevoluteMotorSpeedName;
static StringTableEntry jointRevoluteMotorMaxTorqueName;

static StringTableEntry jointWeldTypeName;
static StringTableEntry jointWeldFrequencyName;
static StringTableEntry jointWeldDampingRatioName;

static StringTableEntry jointWheelTypeName;
static StringTableEntry jointWheelWorldAxisName;
static StringTableEntry jointWheelMotorSpeedName;
static StringTableEntry jointWheelMotorMaxTorqueName;
static StringTableEntry jointWheelFrequencyName;
static StringTableEntry jointWheelDampingRatioName;

static StringTableEntry jointFrictionTypeName;
static StringTableEntry jointFrictionMaxForceName;
static StringTableEntry jointFrictionMaxTorqueName;

static StringTableEntry jointPrismaticTypeName;
static StringTableEntry jointPrismaticWorldAxisName;
static StringTableEntry jointPrismaticLimitLowerTransName;
static StringTableEntry jointPrismaticLimitUpperTransName;
static StringTableEntry jointPrismaticMotorSpeedName;
static StringTableEntry jointPrismaticMotorMaxForceName;

static StringTableEntry jointPulleyTypeName;
static StringTableEntry jointPulleyGroundAnchorAName;
static StringTableEntry jointPulleyGroundAnchorBName;
static StringTableEntry jointPulleyLengthAName;
static StringTableEntry jointPulleyLengthBName;
static StringTableEntry jointPulleyRatioName;

static StringTableEntry jointTargetTypeName;
static StringTableEntry jointTargetWorldTargetName;
static StringTableEntry jointTargetMaxForceName;
static StringTableEntry jointTargetFrequencyName;
static StringTableEntry jointTargetDampingRatioName;

static StringTableEntry jointMotorTypeName;
static StringTableEntry jointMotorLinearOffsetName;
static StringTableEntry jointMotorAngularOffsetName;
static StringTableEntry jointMotorMaxForceName;
static StringTableEntry jointMotorMaxTorqueName;
static StringTableEntry jointMotorCorrectionFactorName;

//-----------------------------------------------------------------------------

Scene::Scene() :
    /// World.
    mpWorld(NULL),
    mWorldGravity(0.0f, 0.0f),
    mVelocityIterations(8),
    mPositionIterations(3),

    /// Joint access.
    mJointMasterId(1),

    /// Scene time.
    mSceneTime(0.0f),
    mScenePause(false),

    /// Debug and metrics.
    mDebugMask(0X00000000),
    mpDebugSceneObject(NULL),

    /// Window rendering.
    mpCurrentRenderWindow(NULL),

    /// Miscellaneous.
    mIsEditorScene(0),
    mUpdateCallback(false),
    mSceneIndex(0)
{
    // Initialize joint property names.
    if ( !jointPropertiesInitialized )
    {
        jointCollectionName               = StringTable->insert( "Joints" );
        jointCollideConnectedName         = StringTable->insert( "CollideConnected" );
        jointObjectAName                  = StringTable->insert( "ObjectA" );
        jointObjectBName                  = StringTable->insert( "ObjectB" );
        jointLocalAnchorAName             = StringTable->insert( "LocalAnchorA" );
        jointLocalAnchorBName             = StringTable->insert( "LocalAnchorB" );

        jointDistanceTypeName             = StringTable->insert( "Distance" );
        jointDistanceLengthName           = StringTable->insert( "Length" );
        jointDistanceFrequencyName        = StringTable->insert( "Frequency" );
        jointDistanceDampingRatioName     = StringTable->insert( "DampingRatio" );

        jointRopeTypeName                 = StringTable->insert( "Rope" );
        jointRopeMaxLengthName            = StringTable->insert( "MaxLength" );

        jointRevoluteTypeName             = StringTable->insert( "Revolute" );
        jointRevoluteLimitLowerAngleName  = StringTable->insert( "LowerAngle" );
        jointRevoluteLimitUpperAngleName  = StringTable->insert( "UpperAngle" );
        jointRevoluteMotorSpeedName       = StringTable->insert( "MotorSpeed" );
        jointRevoluteMotorMaxTorqueName   = StringTable->insert( "MaxTorque" );

        jointWeldTypeName                 = StringTable->insert( "Weld" );
        jointWeldFrequencyName            = jointDistanceFrequencyName;
        jointWeldDampingRatioName         = jointDistanceDampingRatioName;

        jointWheelTypeName                = StringTable->insert( "Wheel" );
        jointWheelWorldAxisName           = StringTable->insert( "WorldAxis" );
        jointWheelMotorSpeedName          = StringTable->insert( "MotorSpeed" );
        jointWheelMotorMaxTorqueName      = jointRevoluteMotorMaxTorqueName;
        jointWheelFrequencyName           = jointDistanceFrequencyName;
        jointWheelDampingRatioName        = jointDistanceDampingRatioName;

        jointFrictionTypeName             = StringTable->insert( "Friction" );
        jointFrictionMaxForceName         = StringTable->insert( "MaxForce" );
        jointFrictionMaxTorqueName        = jointRevoluteMotorMaxTorqueName;

        jointPrismaticTypeName            = StringTable->insert( "Prismatic" );
        jointPrismaticWorldAxisName       = jointWheelWorldAxisName;
        jointPrismaticLimitLowerTransName = StringTable->insert( "LowerTranslation" );
        jointPrismaticLimitUpperTransName = StringTable->insert( "UpperTranslation" );
        jointPrismaticMotorSpeedName      = jointRevoluteMotorSpeedName;
        jointPrismaticMotorMaxForceName   = jointFrictionMaxForceName;

        jointPulleyTypeName               = StringTable->insert( "Pulley" );
        jointPulleyGroundAnchorAName      = StringTable->insert( "GroundAnchorA" );
        jointPulleyGroundAnchorBName      = StringTable->insert( "GroundAnchorB" );
        jointPulleyLengthAName            = StringTable->insert( "LengthA" );
        jointPulleyLengthBName            = StringTable->insert( "LengthB" );
        jointPulleyRatioName              = StringTable->insert( "Ratio" );

        jointTargetTypeName               = StringTable->insert( "Target" );
        jointTargetWorldTargetName        = StringTable->insert( "WorldTarget" );
        jointTargetMaxForceName           = StringTable->insert( jointFrictionMaxForceName );
        jointTargetFrequencyName          = jointDistanceFrequencyName;
        jointTargetDampingRatioName       = jointDistanceDampingRatioName;

        jointMotorTypeName                = StringTable->insert( "Motor" );
        jointMotorLinearOffsetName        = StringTable->insert( "LinearOffset" );
        jointMotorAngularOffsetName       = StringTable->insert( "AngularOffset" );
        jointMotorMaxForceName            = jointFrictionMaxForceName;
        jointMotorMaxTorqueName           = jointRevoluteMotorMaxTorqueName;
        jointMotorCorrectionFactorName    = StringTable->insert( "CorrectionFactor" );

        // Flag as initialized.
        jointPropertiesInitialized = true;
    }

    // Set Vector Associations.
    VECTOR_SET_ASSOCIATION( mSceneObjects );
    VECTOR_SET_ASSOCIATION( mDeleteRequests );
    VECTOR_SET_ASSOCIATION( mDeleteRequestsTemp );
    VECTOR_SET_ASSOCIATION( mEndContacts );
      
    // Initialize layer sort mode.
    for ( U32 n = 0; n < MAX_LAYERS_SUPPORTED; ++n )
       mLayerSortModes[n] = SceneRenderQueue::RENDER_SORT_NEWEST;

    // Set debug stats for batch renderer.
    mBatchRenderer.setDebugStats( &mDebugStats );

    // Turn-on tick processing.
    setProcessTicks( true );

    // Assign scene index.    
    mSceneIndex = ++sSceneMasterIndex;
    sSceneCount++;

    mNSLinkMask = LinkSuperClassName | LinkClassName;
}

//-----------------------------------------------------------------------------

Scene::~Scene()
{
    // Decrease scene count.
    --sSceneCount;
}

//-----------------------------------------------------------------------------

bool Scene::onAdd()
{
    // Call Parent.
    if(!Parent::onAdd())
        return false;

    // Synchronize Namespace's
    linkNamespaces();

    // Create physics world.
    mpWorld = new b2World( mWorldGravity );

    // Set contact filter.
    mpWorld->SetContactFilter( &mContactFilter );

    // Set contact listener.
    mpWorld->SetContactListener( this );

    // Set destruction listener.
    mpWorld->SetDestructionListener( this );

    // Create ground body.
    b2BodyDef groundBodyDef;
    groundBodyDef.userData = static_cast<PhysicsProxy*>(this);
    mpGroundBody = mpWorld->CreateBody(&groundBodyDef);
    mpGroundBody->SetAwake( false );

    // Create world query.
    mpWorldQuery = new WorldQuery(this);

    // Set loading scene.
    Scene::LoadingScene = this;

    // Tell the scripts
    Con::executef(this, 1, "onAdd");

    // Return Okay.
    return true;
}

//-----------------------------------------------------------------------------

void Scene::onRemove()
{
    // tell the scripts
    Con::executef(this, 1, "onRemove");

    // Clear Scene.
    clearScene();

    // Process Delete Requests.
    processDeleteRequests(true);

    // Delete ground body.
    mpWorld->DestroyBody( mpGroundBody );
    mpGroundBody = NULL;

    // Delete physics world and world query.
    delete mpWorldQuery;
    delete mpWorld;
    mpWorldQuery = NULL;
    mpWorld = NULL;

    // Detach All Scene Windows.
    detachAllSceneWindows();

    // Call Parent. Clear scene handles all the object removal, so we can skip
    // that part and just do the sim-object stuff.
    SimObject::onRemove();

    // Restore NameSpace's
    unlinkNamespaces();
}

//-----------------------------------------------------------------------------

void Scene::onDeleteNotify( SimObject* object )
{
    // Ignore if we're not monitoring a debug banner scene object.
    if ( mpDebugSceneObject == NULL )
        return;

    // Ignore if it's not the one we're monitoring.
    SceneObject* pSceneObject = dynamic_cast<SceneObject*>( object );
    if ( pSceneObject != mpDebugSceneObject )
        return;

    // Reset the monitored scene object.
    mpDebugSceneObject = NULL;
}

//-----------------------------------------------------------------------------

void Scene::initPersistFields()
{
    // Call Parent.
    Parent::initPersistFields();

    // Physics.
    addProtectedField("Gravity", TypeT2DVector, Offset(mWorldGravity, Scene), &setGravity, &getGravity, &writeGravity, "" );
    addField("VelocityIterations", TypeS32, Offset(mVelocityIterations, Scene), &writeVelocityIterations, "" );
    addField("PositionIterations", TypeS32, Offset(mPositionIterations, Scene), &writePositionIterations, "" );

    // Layer sort modes.
    char buffer[64];
    for ( U32 n = 0; n < MAX_LAYERS_SUPPORTED; n++ )
    {
       dSprintf( buffer, 64, "layerSortMode%d", n );
       addField( buffer, TypeEnum, OffsetNonConst(mLayerSortModes[n], Scene), &writeLayerSortMode, 1, &SceneRenderQueue::renderSortTable, "");
    }

    // Callbacks.
    addField("UpdateCallback", TypeBool, Offset(mUpdateCallback, Scene), &writeUpdateCallback, "");
}

//-----------------------------------------------------------------------------

void Scene::BeginContact( b2Contact* pContact )
{
    // Ignore contact if it's not a touching contact.
    if ( !pContact->IsTouching() )
        return;

#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_START(Scene_BeginContact);
#endif

    // Fetch fixtures.
    b2Fixture* pFixtureA = pContact->GetFixtureA();
    b2Fixture* pFixtureB = pContact->GetFixtureB();

    // Fetch physics proxies.
    PhysicsProxy* pPhysicsProxyA = static_cast<PhysicsProxy*>(pFixtureA->GetBody()->GetUserData());
    PhysicsProxy* pPhysicsProxyB = static_cast<PhysicsProxy*>(pFixtureB->GetBody()->GetUserData());

    // Ignore stuff that's not a scene object.
    if (    pPhysicsProxyA->getPhysicsProxyType() != PhysicsProxy::PHYSIC_PROXY_SCENEOBJECT ||
            pPhysicsProxyB->getPhysicsProxyType() != PhysicsProxy::PHYSIC_PROXY_SCENEOBJECT )
    {
#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_END();  //Scene_BeginContact
#endif
            return;
    }

    // Fetch scene objects.
    SceneObject* pSceneObjectA = static_cast<SceneObject*>(pPhysicsProxyA);
    SceneObject* pSceneObjectB = static_cast<SceneObject*>(pPhysicsProxyB);

    // Initialize the contact.
    TickContact tickContact;
    tickContact.initialize( pContact, pSceneObjectA, pSceneObjectB, pFixtureA, pFixtureB );

    // Add contact.
    mBeginContacts.insert( pContact, tickContact );

#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_END();  //Scene_BeginContact
#endif
}

//-----------------------------------------------------------------------------

void Scene::EndContact( b2Contact* pContact )
{
#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_START(Scene_EndContact);
#endif

    // Fetch fixtures.
    b2Fixture* pFixtureA = pContact->GetFixtureA();
    b2Fixture* pFixtureB = pContact->GetFixtureB();

    // Fetch physics proxies.
    PhysicsProxy* pPhysicsProxyA = static_cast<PhysicsProxy*>(pFixtureA->GetBody()->GetUserData());
    PhysicsProxy* pPhysicsProxyB = static_cast<PhysicsProxy*>(pFixtureB->GetBody()->GetUserData());

    // Ignore stuff that's not a scene object.
    if (    pPhysicsProxyA->getPhysicsProxyType() != PhysicsProxy::PHYSIC_PROXY_SCENEOBJECT ||
            pPhysicsProxyB->getPhysicsProxyType() != PhysicsProxy::PHYSIC_PROXY_SCENEOBJECT )
    {
#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_END();  //Scene_EndContact
#endif
            return;
    }

    // Fetch scene objects.
    SceneObject* pSceneObjectA = static_cast<SceneObject*>(pPhysicsProxyA);
    SceneObject* pSceneObjectB = static_cast<SceneObject*>(pPhysicsProxyB);

    // Initialize the contact.
    TickContact tickContact;
    tickContact.initialize( pContact, pSceneObjectA, pSceneObjectB, pFixtureA, pFixtureB );

    // Add contact.
    mEndContacts.push_back( tickContact );

#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_END();  //Scene_EndContact
#endif
}

//-----------------------------------------------------------------------------

void Scene::PostSolve( b2Contact* pContact, const b2ContactImpulse* pImpulse )
{
    // Find contact mapping.
    typeContactHash::iterator contactItr = mBeginContacts.find( pContact );

    // Finish if we didn't find the contact.
    if ( contactItr == mBeginContacts.end() )
        return;

    // Fetch contact.
    TickContact& tickContact = contactItr->value;

    // Add the impulse.
    for ( U32 index = 0; index < b2_maxManifoldPoints; ++index )
    {
        tickContact.mNormalImpulses[index] += pImpulse->normalImpulses[index];
        tickContact.mTangentImpulses[index] += pImpulse->tangentImpulses[index];
    }
}

//-----------------------------------------------------------------------------

void Scene::forwardContacts( void )
{
    // Iterate end contacts.
    for( typeContactVector::iterator contactItr = mEndContacts.begin(); contactItr != mEndContacts.end(); ++contactItr )
    {
        // Fetch tick contact.
        TickContact& tickContact = *contactItr;

        // Inform the scene objects.
        tickContact.mpSceneObjectA->onEndCollision( tickContact );
        tickContact.mpSceneObjectB->onEndCollision( tickContact );
    }

    // Iterate begin contacts.
    for( typeContactHash::iterator contactItr = mBeginContacts.begin(); contactItr != mBeginContacts.end(); ++contactItr )
    {
        // Fetch tick contact.
        TickContact& tickContact = contactItr->value;

        // Inform the scene objects.
        tickContact.mpSceneObjectA->onBeginCollision( tickContact );
        tickContact.mpSceneObjectB->onBeginCollision( tickContact );
    }
}

//-----------------------------------------------------------------------------

void Scene::dispatchBeginContactCallbacks( void )
{
    // Sanity!
    AssertFatal( b2_maxManifoldPoints == 2, "Scene::dispatchBeginContactCallbacks() - Invalid assumption about max manifold points." );

    // Fetch contact count.
    const U32 contactCount = mBeginContacts.size();

    // Finish if no contacts.
    if ( contactCount == 0 )
        return;

    // Finish if no collision method exists on scene.
    Namespace* pNamespace = getNamespace();
    if ( pNamespace != NULL && pNamespace->lookup( StringTable->insert( "onCollision" ) ) == NULL )
        return;

#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_START(Scene_DispatchBeginContactCallbacks);
#endif

    // Iterate all contacts.
    for ( typeContactHash::iterator contactItr = mBeginContacts.begin(); contactItr != mBeginContacts.end(); ++contactItr )
    {
        // Fetch contact.
        const TickContact& tickContact = contactItr->value;

        // Fetch scene objects.
        SceneObject* pSceneObjectA = tickContact.mpSceneObjectA;
        SceneObject* pSceneObjectB = tickContact.mpSceneObjectB;

        // Skip if either object is being deleted.
        if ( pSceneObjectA->isBeingDeleted() || pSceneObjectB->isBeingDeleted() )
            continue;

        // Skip if either object does not have collision callback active.
        if ( !pSceneObjectA->getCollisionCallback() || !pSceneObjectB->getCollisionCallback() )
            continue;

        // Fetch normal and contact points.
        const b2Vec2& normal = tickContact.mWorldManifold.normal;
        const b2Vec2& point1 = tickContact.mWorldManifold.points[0];
        const b2Vec2& point2 = tickContact.mWorldManifold.points[1];
        const U32& pointCount = tickContact.mPointCount;
        const S32 shapeIndexA = pSceneObjectA->getCollisionShapeIndex( tickContact.mpFixtureA );
        const S32 shapeIndexB = pSceneObjectB->getCollisionShapeIndex( tickContact.mpFixtureB );

        // Sanity!
        AssertFatal( shapeIndexA >= 0, "Scene::dispatchBeginContactCallbacks() - Cannot find shape index reported on physics proxy of a fixture." );
        AssertFatal( shapeIndexB >= 0, "Scene::dispatchBeginContactCallbacks() - Cannot find shape index reported on physics proxy of a fixture." );

        // Fetch collision impulse information
        const F32 normalImpulse1 = tickContact.mNormalImpulses[0];
        const F32 normalImpulse2 = tickContact.mNormalImpulses[1];
        const F32 tangentImpulse1 = tickContact.mTangentImpulses[0];
        const F32 tangentImpulse2 = tickContact.mTangentImpulses[1];

        // Format objects.
        char* pSceneObjectABuffer = Con::getArgBuffer( 8 );
        char* pSceneObjectBuffer = Con::getArgBuffer( 8 );
        dSprintf( pSceneObjectABuffer, 8, "%d", pSceneObjectA->getId() );
        dSprintf( pSceneObjectBuffer, 8, "%d", pSceneObjectB->getId() );

        // Format miscellaneous information.
        char* pMiscInfoBuffer = Con::getArgBuffer(128);
        if ( pointCount == 2 )
        {
            dSprintf(pMiscInfoBuffer, 128,
                "%d %d %0.4f %0.4f %0.4f %0.4f %0.4f %0.4f %0.4f %0.4f %0.4f %0.4f",
                shapeIndexA, shapeIndexB,
                normal.x, normal.y,
                point1.x, point1.y,
                normalImpulse1,
                tangentImpulse1,
                point2.x, point2.y,
                normalImpulse2,
                tangentImpulse2 );
        }
        else
        {
            dSprintf(pMiscInfoBuffer, 128,
                "%d %d %0.4f %0.4f %0.4f %0.4f %0.4f %0.4f",
                shapeIndexA, shapeIndexB,
                normal.x, normal.y,
                point1.x, point1.y,
                normalImpulse1,
                tangentImpulse1 );
        }

        // Script callback.
        Con::executef( this, 4, "onCollision",
            pSceneObjectABuffer,
            pSceneObjectBuffer,
            pMiscInfoBuffer );
    }

#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_END();  //Scene_DispatchBeginContactCallbacks
#endif
}

//-----------------------------------------------------------------------------

void Scene::dispatchEndContactCallbacks( void )
{
    // Sanity!
    AssertFatal( b2_maxManifoldPoints == 2, "Scene::dispatchEndContactCallbacks() - Invalid assumption about max manifold points." );

    // Fetch contact count.
    const U32 contactCount = mEndContacts.size();

    // Finish if no contacts.
    if ( contactCount == 0 )
        return;

    // Finish if no collision method exists on scene.
    Namespace* pNamespace = getNamespace();
    if ( pNamespace != NULL && pNamespace->lookup( StringTable->insert( "onEndCollision" ) ) == NULL )
        return;

#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_START(Scene_DispatchEndContactCallback);
#endif

    // Iterate all contacts.
    for ( typeContactVector::iterator contactItr = mEndContacts.begin(); contactItr != mEndContacts.end(); ++contactItr )
    {
        // Fetch contact.
        const TickContact& tickContact = *contactItr;

        // Fetch scene objects.
        SceneObject* pSceneObjectA = tickContact.mpSceneObjectA;
        SceneObject* pSceneObjectB = tickContact.mpSceneObjectB;

        // Skip if either object is being deleted.
        if ( pSceneObjectA->isBeingDeleted() || pSceneObjectB->isBeingDeleted() )
            continue;

        // Skip if either object does not have collision callback active.
        if ( !pSceneObjectA->getCollisionCallback() || !pSceneObjectB->getCollisionCallback() )
            continue;

        // Fetch shape index.
        const S32 shapeIndexA = pSceneObjectA->getCollisionShapeIndex( tickContact.mpFixtureA );
        const S32 shapeIndexB = pSceneObjectB->getCollisionShapeIndex( tickContact.mpFixtureB );

        // Sanity!
        AssertFatal( shapeIndexA >= 0, "Scene::dispatchEndContactCallbacks() - Cannot find shape index reported on physics proxy of a fixture." );
        AssertFatal( shapeIndexB >= 0, "Scene::dispatchEndContactCallbacks() - Cannot find shape index reported on physics proxy of a fixture." );

        // Format objects.
        char* pSceneObjectABuffer = Con::getArgBuffer( 8 );
        char* pSceneObjectBBuffer = Con::getArgBuffer( 8 );
        dSprintf( pSceneObjectABuffer, 8, "%d", pSceneObjectA->getId() );
        dSprintf( pSceneObjectBBuffer, 8, "%d", pSceneObjectB->getId() );

        // Format miscellaneous information.
        char* pMiscInfoBuffer = Con::getArgBuffer(32);
        dSprintf(pMiscInfoBuffer, 32, "%d %d", shapeIndexA, shapeIndexB );

        // Script callback.
        Con::executef( this, 4, "onEndCollision",
            pSceneObjectABuffer,
            pSceneObjectBBuffer,
            pMiscInfoBuffer );
    }

#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_END();  //Scene_DispatchEndContactCallback
#endif
}

//-----------------------------------------------------------------------------

void Scene::processTick( void )
{
#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_START(Scene_ProcessTick);
#endif

    // Process Delete Requests.
    processDeleteRequests(false);

    // Update debug stats.
    mDebugStats.fps           = Con::getFloatVariable("fps::real", 0.0f);
    mDebugStats.bodyCount     = mpWorld->GetBodyCount();
    mDebugStats.jointCount    = mpWorld->GetJointCount();
    mDebugStats.contactCount  = mpWorld->GetContactCount();
    mDebugStats.proxyCount    = mpWorld->GetProxyCount();
    mDebugStats.objectsCount  = mSceneObjects.size();
    mDebugStats.worldProfile  = mpWorld->GetProfile();

    // Reset particle stats.
    mDebugStats.particlesAlloc = 0;
    mDebugStats.particlesFree = 0;
    mDebugStats.particlesUsed = 0;

    // Finish if scene is paused.
    if ( !getScenePause() )
    {
        // Reset object stats.
        U32 objectsEnabled = 0;
        U32 objectsVisible = 0;
        U32 objectsAwake   = 0;

        // Fetch if a "normal" i.e. non-editor scene.
        const bool isNormalScene = !getIsEditorScene();

        // Update scene time.
        mSceneTime += Tickable::smTickSec;

        // Clear ticked scene objects.
        mTickedSceneObjects.clear();

        // Iterate scene objects.
        for( S32 n = 0; n < mSceneObjects.size(); ++n )
        {
            // Fetch scene object.
            SceneObject* pSceneObject = mSceneObjects[n];

            // Update awake/asleep counts.
            if ( pSceneObject->getAwake() )
                objectsAwake++;

            // Update visible.
            if ( pSceneObject->getVisible() )
                objectsVisible++;

            // Push scene object if it's eligible for ticking.
            if ( pSceneObject->isEnabled() )
            {
                // Update enabled.
                objectsEnabled++;

                // Add to ticked objects if object is not being deleted and this is a "normal" scene or
                // the object is marked as allowing editor ticks.
                if ( !pSceneObject->isBeingDeleted() && (isNormalScene || pSceneObject->getIsEditorTickAllowed() )  )
                    mTickedSceneObjects.push_back( pSceneObject );
            }
        }

        // Update object stats.
        mDebugStats.objectsEnabled = objectsEnabled;
        mDebugStats.objectsVisible = objectsVisible;
        mDebugStats.objectsAwake   = objectsAwake;

        // Debug Status Reference.
        DebugStats* pDebugStats = &mDebugStats;

#ifdef TORQUE_ENABLE_PROFILER
        PROFILE_START(Scene_PreIntegrate);
#endif

        // ****************************************************
        // Pre-integrate objects.
        // ****************************************************

        // Iterate ticked scene objects.
        for ( S32 n = 0; n < mTickedSceneObjects.size(); ++n )
        {
            // Pre-integrate.
            mTickedSceneObjects[n]->preIntegrate( mSceneTime, Tickable::smTickSec, pDebugStats );
        }

#ifdef TORQUE_ENABLE_PROFILER
        PROFILE_END();   // Scene_PreIntegrate
#endif

#ifdef TORQUE_ENABLE_PROFILER
PROFILE_START(Scene_IntegratePhysics);
#endif

        // Reset contacts.
        mBeginContacts.clear();
        mEndContacts.clear();

        // Only step the physics if a "normal" scene.
        if ( isNormalScene )
        {
            // Step the physics.
            mpWorld->Step( Tickable::smTickSec, mVelocityIterations, mPositionIterations );
        }

        // Forward the contacts.
        forwardContacts();

#ifdef TORQUE_ENABLE_PROFILER
        PROFILE_END();   // Scene_IntegratePhysics
#endif

#ifdef TORQUE_ENABLE_PROFILER
        PROFILE_START(Scene_IntegrateObject);
#endif
        // ****************************************************
        // Integrate objects.
        // ****************************************************

        // Iterate ticked scene objects.
        for ( S32 n = 0; n < mTickedSceneObjects.size(); ++n )
        {
            // Integrate.
            mTickedSceneObjects[n]->integrateObject( mSceneTime, Tickable::smTickSec, pDebugStats );
        }

#ifdef TORQUE_ENABLE_PROFILER
        PROFILE_END();   // Scene_IntegrateObject
#endif

#ifdef TORQUE_ENABLE_PROFILER
        PROFILE_START(Scene_PostIntegrate);
#endif
        // ****************************************************
        // Post-Integrate Stage.
        // ****************************************************

        // Iterate ticked scene objects.
        for ( S32 n = 0; n < mTickedSceneObjects.size(); ++n )
        {
            // Post-integrate.
            mTickedSceneObjects[n]->postIntegrate( mSceneTime, Tickable::smTickSec, pDebugStats );
        }

#ifdef TORQUE_ENABLE_PROFILER
        PROFILE_END();   // Scene_PostIntegrate
#endif

#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_START(Scene_OnSceneUpdatetCallback);
#endif

        // Scene update callback.
        if( mUpdateCallback )
            Con::executef( this, 1, "onSceneUpdate" );

#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_END();  //Scene_OnSceneUpdatetCallback
#endif

        // Only dispatch contacts if a "normal" scene.
        if ( isNormalScene )
        {
            // Dispatch contacts callbacks.
            dispatchEndContactCallbacks();
            dispatchBeginContactCallbacks();
        }

        // Clear ticked scene objects.
        mTickedSceneObjects.clear();
    }

    // Update debug stat ranges.
    mDebugStats.updateRanges();

#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_END();   // Scene_ProcessTick
#endif
}

//-----------------------------------------------------------------------------

void Scene::interpolateTick( F32 timeDelta )
{
    // Finish if scene is paused.
    if ( getScenePause() ) return;

#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_START(Scene_InterpolateTick);
#endif

    // Iterate scene objects.
    for( S32 n = 0; n < mSceneObjects.size(); ++n )
    {
        // Fetch scene object.
        SceneObject* pSceneObject = mSceneObjects[n];

        // Skip interpolation of scene object if it's not eligible.
        if ( !pSceneObject->isEnabled() || pSceneObject->isBeingDeleted() )
            continue;

        pSceneObject->interpolateObject( timeDelta );
    }

#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_END();   // Scene_InterpolateTick
#endif
}

//-----------------------------------------------------------------------------

void Scene::sceneRender( const SceneRenderState* pSceneRenderState )
{
#ifdef TORQUE_ENABLE_PROFILER
        PROFILE_START(Scene_RenderView);
#endif

    // Fetch debug stats.
    DebugStats* pDebugStats = pSceneRenderState->mpDebugStats;

    // Reset Render Stats.
    pDebugStats->renderPicked                   = 0;
    pDebugStats->renderRequests                 = 0;
    pDebugStats->renderFallbacks                = 0;
    pDebugStats->batchTrianglesSubmitted        = 0;
    pDebugStats->batchDrawCallsStrictSingle     = 0;
    pDebugStats->batchDrawCallsStrictMultiple   = 0;
    pDebugStats->batchDrawCallsSorted           = 0;
    pDebugStats->batchFlushes                   = 0;
    pDebugStats->batchBlendStateFlush           = 0;
    pDebugStats->batchColorStateFlush           = 0;
    pDebugStats->batchAlphaStateFlush           = 0;
    pDebugStats->batchTextureChangeFlush        = 0;
    pDebugStats->batchBufferFullFlush           = 0;
    pDebugStats->batchIsolatedFlush             = 0;
    pDebugStats->batchLayerFlush                = 0;
    pDebugStats->batchNoBatchFlush              = 0;
    pDebugStats->batchAnonymousFlush            = 0;

    // Clear world query.
    mpWorldQuery->clearQuery();

    // Set filter.
    WorldQueryFilter queryFilter( pSceneRenderState->mRenderLayerMask, pSceneRenderState->mRenderGroupMask, true, true, false, false );
    mpWorldQuery->setQueryFilter( queryFilter );

    // Query render AABB.
    mpWorldQuery->renderQueryArea( pSceneRenderState->mRenderAABB );

    // Are there any query results?
    if ( mpWorldQuery->getQueryResultsCount() > 0 )
    {
        // Fetch the primary scene render queue.
        SceneRenderQueue* pSceneRenderQueue = SceneRenderQueueFactory.createObject();      

        // Yes so step through layers.
        for ( S32 layer = MAX_LAYERS_SUPPORTED-1; layer >= 0 ; layer-- )
        {
            // Fetch layer.
            typeWorldQueryResultVector& layerResults = mpWorldQuery->getLayeredQueryResults( layer );

            // Fetch layer object count.
            const U32 layerObjectCount = layerResults.size();

            // Are there any objects to render in this layer?
            if ( layerObjectCount > 0 )
            {
                // Yes, so increase render picked.
                pDebugStats->renderPicked += layerObjectCount;

                // Iterate query results.
                for( typeWorldQueryResultVector::iterator worldQueryItr = layerResults.begin(); worldQueryItr != layerResults.end(); ++worldQueryItr )
                {
                    // Fetch scene object.
                    SceneObject* pSceneObject = worldQueryItr->mpSceneObject;

                    // Can the scene object prepare a render?
                    if ( pSceneObject->canPrepareRender() )
                    {
                        // Yes. so is it batch isolated.
                        if ( pSceneObject->getBatchIsolated() )
                        {
                            // Yes, so create an isolated render request on the primary queue.
                            SceneRenderRequest* pIsolatedSceneRenderRequest = pSceneRenderQueue->createRenderRequest()->set(
                                pSceneObject,
                                pSceneObject->getRenderPosition(),
                                pSceneObject->getSceneLayerDepth(),
                                pSceneObject->getSortPoint(),
                                pSceneObject->getSerialId() );

                            // Create a new isolated render queue.
                            pIsolatedSceneRenderRequest->mpIsolatedRenderQueue = SceneRenderQueueFactory.createObject();

                            // Prepare in the isolated queue.
                            pSceneObject->scenePrepareRender( pSceneRenderState, pIsolatedSceneRenderRequest->mpIsolatedRenderQueue );

                            // Increase render request count.
                            pDebugStats->renderRequests += (U32)pIsolatedSceneRenderRequest->mpIsolatedRenderQueue->getRenderRequests().size();

                            // Adjust for the extra private render request.
                            pDebugStats->renderRequests -= 1;
                        }
                        else
                        {
                            // No, so prepare in primary queue.
                            pSceneObject->scenePrepareRender( pSceneRenderState, pSceneRenderQueue );
                        }
                    }
                    else
                    {
                        // No, so perform the render preparation for it.
                        pSceneRenderQueue->createRenderRequest()->set(
                            pSceneObject,
                            pSceneObject->getRenderPosition(),
                            pSceneObject->getSceneLayerDepth(),
                            pSceneObject->getSortPoint(),
                            pSceneObject->getSerialId() );
                    }
                }

                // Fetch render requests.
                SceneRenderQueue::typeRenderRequestVector& sceneRenderRequests = pSceneRenderQueue->getRenderRequests();

                // Fetch render request count.
                const U32 renderRequestCount = (U32)sceneRenderRequests.size();

                // Increase render request count.
                pDebugStats->renderRequests += renderRequestCount;

                // Do we have more than a single render request?
                if ( renderRequestCount > 1 )
                {
                    // Yes, so fetch layer sort mode.
                    SceneRenderQueue::RenderSort& mode = mLayerSortModes[layer];

                    // Temporarily switch to normal sort if batch sort but batcher disabled.
                    if ( !mBatchRenderer.getBatchEnabled() && mode == SceneRenderQueue::RENDER_SORT_BATCH )
                        mode = SceneRenderQueue::RENDER_SORT_NEWEST;

#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_START(Scene_LayerSorting);
#endif
                    // Set render queue mode.
                    pSceneRenderQueue->setSortMode( mode );

                    // Sort the render requests.
                    pSceneRenderQueue->sort();

#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_END(); // Scene_LayerSorting
#endif
                }

                // Iterate render requests.
                for( SceneRenderQueue::typeRenderRequestVector::iterator renderRequestItr = sceneRenderRequests.begin(); renderRequestItr != sceneRenderRequests.end(); ++renderRequestItr )
                {
                    // Fetch render request.
                    SceneRenderRequest* pSceneRenderRequest = *renderRequestItr;

                    // Fetch scene render object.
                    SceneRenderObject* pSceneRenderObject = pSceneRenderRequest->mpSceneRenderObject;

#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_START(Scene_RenderObjects);
#endif                   
                    // Flush if the object is not render batched and we're in strict order mode.
                    if ( !pSceneRenderObject->isBatchRendered() && mBatchRenderer.getStrictOrderMode() )
                    {
                        mBatchRenderer.flush( pDebugStats->batchNoBatchFlush );
                    }
                    // Flush if the object is batch isolated.
                    else if ( pSceneRenderObject->getBatchIsolated() )
                    {
                        mBatchRenderer.flush( pDebugStats->batchIsolatedFlush );
                    }

                    // Yes, so is the object batch rendered?
                    if ( pSceneRenderObject->isBatchRendered() )
                    {
                        // Yes, so are we blending?
                        if ( pSceneRenderRequest->mBlendMode )
                        {
                            // Yes, so set blending to standard alpha-blending.
                            mBatchRenderer.setBlendMode(
                                pSceneRenderRequest->mSrcBlendFactor,
                                pSceneRenderRequest->mDstBlendFactor,
                                pSceneRenderRequest->mBlendColor );                            
                        }
                        else
                        {
                            // No, so turn-off blending.
                            mBatchRenderer.setBlendOff();
                        }

                        // Set alpha-test mode.
                        mBatchRenderer.setAlphaTestMode( pSceneRenderRequest->mAlphaTest );
                    }

                    // Set batch strict order mode.
                    // NOTE:    We keep reasserting this because an object is free to change it during rendering.
                    mBatchRenderer.setStrictOrderMode( pSceneRenderQueue->getStrictOrderMode() );

                    // Is the object batch isolated?
                    if ( pSceneRenderObject->getBatchIsolated() )
                    {
                        // Yes, so fetch isolated render queue.
                        SceneRenderQueue* pIsolatedRenderQueue = pSceneRenderRequest->mpIsolatedRenderQueue;

                        // Sanity!
                        AssertFatal( pIsolatedRenderQueue != NULL, "Cannot render batch isolated with an isolated render queue." );

                        // Sort the isolated render requests.
                        pIsolatedRenderQueue->sort();

                        // Fetch isolated render requests.
                        SceneRenderQueue::typeRenderRequestVector& isolatedRenderRequests = pIsolatedRenderQueue->getRenderRequests();

                        // Can the object render?
                        if ( pSceneRenderObject->canRender() )
                        {
                            // Yes, so iterate isolated render requests.
                            for( SceneRenderQueue::typeRenderRequestVector::iterator isolatedRenderRequestItr = isolatedRenderRequests.begin(); isolatedRenderRequestItr != isolatedRenderRequests.end(); ++isolatedRenderRequestItr )
                            {
                                pSceneRenderObject->sceneRender( pSceneRenderState, *isolatedRenderRequestItr, &mBatchRenderer );
                            }
                        }
                        else
                        {
                            // No, so iterate isolated render requests.
                            for( SceneRenderQueue::typeRenderRequestVector::iterator isolatedRenderRequestItr = isolatedRenderRequests.begin(); isolatedRenderRequestItr != isolatedRenderRequests.end(); ++isolatedRenderRequestItr )
                            {
                                pSceneRenderObject->sceneRenderFallback( pSceneRenderState, *isolatedRenderRequestItr, &mBatchRenderer );
                            }

                            // Increase render fallbacks.
                            pDebugStats->renderFallbacks++;
                        }

                        // Flush isolated batch.
                        mBatchRenderer.flush( pDebugStats->batchIsolatedFlush );
                    }
                    else
                    {
                        // No, so can the object render?
                        if ( pSceneRenderObject->canRender() )
                        {
                            // Yes, so render object.
                            pSceneRenderObject->sceneRender( pSceneRenderState, pSceneRenderRequest, &mBatchRenderer );
                        }
                        else
                        {
                            // No, so render using fallback.
                            pSceneRenderObject->sceneRenderFallback( pSceneRenderState, pSceneRenderRequest, &mBatchRenderer );

                            // Increase render fallbacks.
                            pDebugStats->renderFallbacks++;
                        }
                    }

#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_END();   // Scene_RenderObjects
#endif
                }

                // Flush.
                // NOTE:    We cannot batch between layers as we adhere to a strict layer render order.
                mBatchRenderer.flush( pDebugStats->batchLayerFlush );

#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_START(Scene_RenderObjectOverlays);
#endif
                // Iterate query results.
                for( typeWorldQueryResultVector::iterator worldQueryItr = layerResults.begin(); worldQueryItr != layerResults.end(); ++worldQueryItr )
                {
                    // Fetch scene object.
                    SceneObject* pSceneObject = worldQueryItr->mpSceneObject;

                    // Render object overlay.
                    pSceneObject->sceneRenderOverlay( pSceneRenderState );
                }
#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_END();   // Scene_RenderObjectOverlays
#endif
            }

            // Reset render queue.
            pSceneRenderQueue->resetState();
        }

        // Cache render queue..
        SceneRenderQueueFactory.cacheObject( pSceneRenderQueue );

    }

#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_START(Scene_RenderJointOverlays);
#endif
    // Draw Joints.
    if ( getDebugMask() & Scene::SCENE_DEBUG_JOINTS )
    {
        mDebugDraw.DrawJoints( mpWorld );
    }
#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_END();   // Scene_RenderJointOverlays
#endif

    // Update debug stat ranges.
    mDebugStats.updateRanges();

#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_START(Scene_OnSceneRendertCallback);
#endif

    if( mUpdateCallback )
        Con::executef( this, 1, "onSceneRender" );

#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_END();  //Scene_OnSceneRendertCallback
#endif

#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_END();   // Scene_RenderView
#endif
}

//-----------------------------------------------------------------------------

void Scene::clearScene( bool deleteObjects )
{
    while( mSceneObjects.size() > 0 )
    {
        // Fetch first scene object.
        SceneObject* pSceneObject = mSceneObjects[0];

        // Remove Object from Scene.
        removeFromScene( pSceneObject );

        // Queue Object for deletion.
        if ( deleteObjects )
            pSceneObject->safeDelete();
    }
}

//-----------------------------------------------------------------------------

void Scene::addToScene( SceneObject* pSceneObject )
{
    if ( pSceneObject == NULL )
        return;

    // Is the scene-object an asset?
    if ( pSceneObject->getAssetId() != StringTable->EmptyString )
    {
        // Yes, so warn.
        Con::warnf( "Scene::addToScene() - Cannot add a scene object asset to the scene." );
        return;
    }

    // Fetch current scene.
    Scene* pCurrentScene = pSceneObject->getScene();

    // Ignore if already in the scene.
    if ( pCurrentScene == this )
        return;

#if defined(TORQUE_DEBUG)
    // Sanity!
    for ( S32 n = 0; n < mSceneObjects.size(); ++n )
    {
        AssertFatal( mSceneObjects[n] != pSceneObject, "A scene object has become corrupt." );
    }
#endif

    // Check that object is not already in a scene.
    if ( pCurrentScene )
    {
        // Remove from scene.
        pCurrentScene->removeFromScene( pSceneObject );
    }

    // Add scene object.
    mSceneObjects.push_back( pSceneObject );

    // Register with the scene.
    pSceneObject->OnRegisterScene( this );

    // Add the object to our set if it's not already there.
    if(!findChildObject(pSceneObject))
       Parent::addObject(pSceneObject);

    // Perform callback only if properly added to the simulation.
    if ( pSceneObject->isProperlyAdded() )
    {
        Con::executef(pSceneObject, 2, "onAddToScene", getIdString());
    }
    else
    {
        // Warning.
        Con::warnf("Scene object added to scene but the object is not registered with the simulation.  No 'onAddToScene' can be performed!  Use Target scene.");
    }
}

//-----------------------------------------------------------------------------

void Scene::removeFromScene( SceneObject* pSceneObject )
{
    if ( pSceneObject == NULL )
        return;

    // Check if object is actually in a scene.
    if ( !pSceneObject->getScene() )
    {
        Con::warnf("Scene::removeFromScene() - Object '%s' is not in a scene!.", pSceneObject->getIdString());
        return;
    }

    // Remove as debug-object if set.
    if ( pSceneObject == getDebugSceneObject() )
        setDebugSceneObject( NULL );

    // Process Destroy Notifications.
    pSceneObject->processDestroyNotifications();

    // Dismount Any Camera.
    pSceneObject->dismountCamera();

    // Remove from the SceneWindow last pickers
    for( U32 i = 0; i < (U32)mAttachedSceneWindows.size(); ++i )
    {
        (dynamic_cast<SceneWindow*>(mAttachedSceneWindows[i]))->removeFromInputEventPick(pSceneObject);
    }

    // Unregister from scene.
    pSceneObject->OnUnregisterScene( this );

    // Find scene object and remove it quickly.
    for ( S32 n = 0; n < mSceneObjects.size(); ++n )
    {
        if ( mSceneObjects[n] == pSceneObject )
        {
            mSceneObjects.erase_fast( n );
            break;
        }
    }

    // Remove from Set, When Appropriate.
    SimSet::iterator pObjectLookup = find(begin(), end(), pSceneObject);
    if (pObjectLookup != end())
       Parent::removeObject(pSceneObject);

    else if (pSceneObject->getSceneObjectGroup())
    {
       if (pSceneObject->getSceneObjectGroup()->getScene() == this)
          pSceneObject->getSceneObjectGroup()->removeObject(pSceneObject);
    }

    // Perform callback.
    Con::executef( pSceneObject, 2, "onRemoveFromScene", getIdString() );
}

//-----------------------------------------------------------------------------

void Scene::addToScene(SceneObjectGroup* pSceneObjectGroup)
{
    if ( pSceneObjectGroup == NULL )
        return;

   // Check that object is not already in a scene.
   if ( pSceneObjectGroup->getScene() )
   {
      // Remove from scene.
      pSceneObjectGroup->getScene()->removeFromScene( pSceneObjectGroup );
   }

   pSceneObjectGroup->mScene = this;

   if (!findChildObject(pSceneObjectGroup))
      Parent::addObject(pSceneObjectGroup);

   for (S32 i = 0; i < pSceneObjectGroup->size(); i++)
      addToScene(pSceneObjectGroup->at(i));
}

//-----------------------------------------------------------------------------

void Scene::removeFromScene(SceneObjectGroup* pSceneObjectGroup)
{
    if ( pSceneObjectGroup == NULL )
        return;

   // Check if object is actually in a scene.
   if ( !pSceneObjectGroup->getScene() )
   {
      Con::warnf("Scene::removeFromScene() - Object '%s' is not in a scene!.", pSceneObjectGroup->getIdString());
      return;
   }

   pSceneObjectGroup->mScene = NULL;
   
   for (S32 i = 0; i < pSceneObjectGroup->size(); i++)
      removeFromScene(pSceneObjectGroup->at(i));

    SimSet::iterator pObjectLookup = find(begin(), end(), pSceneObjectGroup);
    if (pObjectLookup != end())
       Parent::removeObject(pSceneObjectGroup);

    else if (pSceneObjectGroup->getSceneObjectGroup())
    {
       if (pSceneObjectGroup->getSceneObjectGroup()->getScene() == this)
          pSceneObjectGroup->getSceneObjectGroup()->removeObject(pSceneObjectGroup);
    }
}

//-----------------------------------------------------------------------------

void Scene::addToScene( SimObject* pObject )
{
    if ( pObject == NULL )
        return;

   SceneObject* pSceneObject = dynamic_cast<SceneObject*>(pObject);
   if (pSceneObject)
      addToScene(pSceneObject);

   else
   {
      SceneObjectGroup* pGroup = dynamic_cast<SceneObjectGroup*>(pObject);
      if (pGroup)
         addToScene(pGroup);
   }
}

//-----------------------------------------------------------------------------

void Scene::removeFromScene( SimObject* pObject )
{
    if ( pObject == NULL )
        return;

   SceneObject* pSceneObject = dynamic_cast<SceneObject*>(pObject);
   if (pSceneObject)
      removeFromScene(pSceneObject);

   else
   {
      SceneObjectGroup* pGroup = dynamic_cast<SceneObjectGroup*>(pObject);
      if (pGroup)
         removeFromScene(pGroup);
   }
}

//-----------------------------------------------------------------------------

void Scene::addObject( SimObject* pObject )
{
    if ( pObject == NULL )
        return;

   SceneObjectGroup* parentGroup = SceneObjectGroup::getSceneObjectGroup(pObject);
   Scene* parentGraph = SceneObjectGroup::getScene(pObject);

   if (parentGraph == this)
   {
      if (parentGroup)
         parentGroup->removeObject(pObject);

      Parent::addObject(pObject);
   }

   else
   {
      if (parentGraph)
         parentGraph->removeFromScene(pObject);

      else if (parentGroup)
         parentGroup->removeObject(pObject);

      addToScene(pObject);
   }
}

//-----------------------------------------------------------------------------

bool Scene::findChildObject(SimObject* pObject)
{
    if ( pObject == NULL )
        return false;

   for (S32 i = 0; i < size(); i++)
   {
      SimObject* pSimObject = at(i);

      if (pSimObject == pObject)
         return true;

      SceneObjectGroup* pGroup = dynamic_cast<SceneObjectGroup*>(pSimObject);
      if (pGroup)
      {
         if (pGroup->findChildObject(pObject))
            return true;
      }
   }

   // If we make it here, no.
   return false;
}

//-----------------------------------------------------------------------------

SceneObject* Scene::getSceneObject( const U32 objectIndex )
{
    // Sanity!
    AssertFatal( objectIndex < getSceneObjectCount(), "Scene::getSceneObject() - Invalid object index." );

    return mSceneObjects[objectIndex];
}

//-----------------------------------------------------------------------------

U32 Scene::getSceneObjects( typeSceneObjectVector& objects )
{
    // No objects if scene is empty!
    if ( getSceneObjectCount() == 0 )
        return 0;

    // Merge with objects.
    objects.merge( mSceneObjects );

    return getSceneObjectCount();
}

//-----------------------------------------------------------------------------

U32 Scene::getSceneObjects( typeSceneObjectVector& objects, const U32 sceneLayer )
{
    // No objects if scene is empty!
    if ( getSceneObjectCount() == 0 )
        return 0;

    // Reset object count.
    U32 count = 0;

    // Iterate scene objects.
    for( S32 n = 0; n < mSceneObjects.size(); ++n )
    {
        // Fetch scene object.
        SceneObject* pSceneObject = mSceneObjects[n];

        // Skip if not the correct layer.
        if ( pSceneObject->getSceneLayer() != sceneLayer )
            continue;

        // Add to objects.
        objects.push_back( pSceneObject );

        // Increase count.
        count++;
    }

    return count;
}

//-----------------------------------------------------------------------------

U32 Scene::getChildCount( void )
{
    // No children if scene is empty!
    if ( getSceneObjectCount() == 0 )
        return 0;

    // Reset Child Count.
    U32 childCount = 0;

    // Iterate scene objects.
    for( S32 n = 0; n < mSceneObjects.size(); ++n )
    {
        // Fetch scene object.
        SceneObject* pSceneObject = mSceneObjects[n];

        if ( pSceneObject->getIsChild() )
            childCount++;
    }

    // Return Child Count.
    return childCount;
}

//-----------------------------------------------------------------------------

void Scene::synchronizePrefabs( void )
{
    for( typeSceneObjectVector::iterator sceneObjectItr = mSceneObjects.begin(); sceneObjectItr != mSceneObjects.end(); ++sceneObjectItr )
    {
        // Fetch scene Object.
        SceneObject* pSceneObject = *sceneObjectItr;

        // Skip if it's a child object.
        if ( pSceneObject->getIsChild() )
            continue;

        // Synchronize the prefab.
        pSceneObject->synchronizePrefab();
    }
}

//-----------------------------------------------------------------------------

b2Joint* Scene::getJoint( const U32 jointId )
{
    // Find joint.
    typeJointHash::iterator itr = mJoints.find( jointId );

    if ( itr == mJoints.end() )
    {
        Con::warnf("The joint Id of %d is invalid.", jointId);
        return NULL;
    }

    return itr->value;
}

//-----------------------------------------------------------------------------

b2JointType Scene::getJointType( const U32 jointId )
{
    // Sanity!
    if ( jointId >= mJointMasterId )
    {
        Con::warnf("The joint Id of %d is invalid.", jointId);
        return e_unknownJoint;
    }

    return getJoint( jointId )->GetType();
}

//-----------------------------------------------------------------------------

U32 Scene::findJointId( b2Joint* pJoint )
{
    // Sanity!
    AssertFatal( pJoint != NULL, "Joint cannot be NULL." );

    // Find joint.
    typeReverseJointHash::iterator itr = mReverseJoints.find( (U32)pJoint );

    if ( itr == mReverseJoints.end() )
    {
        Con::warnf("The joint Id could not be found via a joint reference of %x", (U32)pJoint);
        return 0;
    }

    return itr->value;
}

//-----------------------------------------------------------------------------

U32 Scene::createJoint( b2JointDef* pJointDef )
{
    // Sanity!
    AssertFatal( pJointDef != NULL, "Joint definition cannot be NULL." );

    // Create Joint.
    b2Joint* pJoint = mpWorld->CreateJoint( pJointDef );

    // Allocate joint Id.
    const U32 jointId = mJointMasterId++;

    // Insert joint.
    typeJointHash::iterator itr = mJoints.insert( jointId, pJoint );

    // Sanity!
    AssertFatal( itr != mJoints.end(), "Joint already in hash table." );

    // Insert reverse joint.
    mReverseJoints.insert( (U32)pJoint, jointId );

    return jointId;
}

//-----------------------------------------------------------------------------

bool Scene::deleteJoint( const U32 jointId )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Finish if no joint.
    if ( pJoint == NULL )
        return false;

    // Destroy joint.
    // This should result in the joint references also being destroyed
    // as the scene is a destruction listener.
    mpWorld->DestroyJoint( pJoint );

    return true;
}

//-----------------------------------------------------------------------------

bool Scene::hasJoints( SceneObject* pSceneObject )
{
    // Sanity!
    AssertFatal( pSceneObject != NULL, "Scene object cannot be NULL!" );
    AssertFatal( pSceneObject->getScene() != this, "Scene object is not in this scene" );

    // Fetch body.
    b2Body* pBody = pSceneObject->getBody();

    // Fetch joint edge.
    b2JointEdge* pJointEdge = pBody->GetJointList();

    if ( pJointEdge == NULL || pJointEdge->joint == NULL )
        return false;

    // Found at least one joint.
    return true;
}

//-----------------------------------------------------------------------------

U32 Scene::createDistanceJoint(
    const SceneObject* pSceneObjectA, const SceneObject* pSceneObjectB,
    const b2Vec2& localAnchorA, const b2Vec2& localAnchorB,
    const F32 length,
    const F32 frequency,
    const F32 dampingRatio,
    const bool collideConnected )
{
    // Fetch bodies.
    b2Body* pBodyA = pSceneObjectA != NULL ? pSceneObjectA->getBody() : getGroundBody();
    b2Body* pBodyB = pSceneObjectB != NULL ? pSceneObjectB->getBody() : getGroundBody();
    
    // Populate definition.
    b2DistanceJointDef jointDef;
    jointDef.userData         = static_cast<PhysicsProxy*>(this);
    jointDef.collideConnected = collideConnected;
    jointDef.bodyA            = pBodyA;
    jointDef.bodyB            = pBodyB;
    jointDef.localAnchorA     = localAnchorA;
    jointDef.localAnchorB     = localAnchorB;
    jointDef.length           = length < 0.0f ? (pBodyB->GetWorldPoint( localAnchorB ) - pBodyA->GetWorldPoint( localAnchorA )).Length() : length;
    jointDef.frequencyHz      = frequency;
    jointDef.dampingRatio     = dampingRatio;
    
    // Create joint.
    return createJoint( &jointDef );
}

//-----------------------------------------------------------------------------

void Scene::setDistanceJointLength(
        const U32 jointId,
        const F32 length )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_distanceJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return;
    }

    // Cast joint.
    b2DistanceJoint* pRealJoint = static_cast<b2DistanceJoint*>( pJoint );

    // Access joint.
    pRealJoint->SetLength( length );
}

//-----------------------------------------------------------------------------

F32 Scene::getDistanceJointLength( const U32 jointId )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return -1.0f;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_distanceJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return -1.0f;
    }

    // Cast joint.
    b2DistanceJoint* pRealJoint = static_cast<b2DistanceJoint*>( pJoint );

    // Access joint.
    return pRealJoint->GetLength();
}

//-----------------------------------------------------------------------------

void Scene::setDistanceJointFrequency(
        const U32 jointId,
        const F32 frequency )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_distanceJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return;
    }

    // Cast joint.
    b2DistanceJoint* pRealJoint = static_cast<b2DistanceJoint*>( pJoint );

    // Access joint.
    pRealJoint->SetFrequency( frequency );
}

//-----------------------------------------------------------------------------

F32 Scene::getDistanceJointFrequency( const U32 jointId )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return -1.0f;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_distanceJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return -1.0f;
    }

    // Cast joint.
    b2DistanceJoint* pRealJoint = static_cast<b2DistanceJoint*>( pJoint );

    // Access joint.
    return pRealJoint->GetFrequency();
}

//-----------------------------------------------------------------------------

void Scene::setDistanceJointDampingRatio(
        const U32 jointId,
        const F32 dampingRatio )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_distanceJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return;
    }

    // Cast joint.
    b2DistanceJoint* pRealJoint = static_cast<b2DistanceJoint*>( pJoint );

    // Access joint.
    pRealJoint->SetDampingRatio( dampingRatio );
}

//-----------------------------------------------------------------------------

F32 Scene::getDistanceJointDampingRatio( const U32 jointId )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return -1.0f;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_distanceJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return -1.0f;
    }

    // Cast joint.
    b2DistanceJoint* pRealJoint = static_cast<b2DistanceJoint*>( pJoint );

    // Access joint.
    return pRealJoint->GetDampingRatio();
}

//-----------------------------------------------------------------------------

U32 Scene::createRopeJoint(
        const SceneObject* pSceneObjectA, const SceneObject* pSceneObjectB,
        const b2Vec2& localAnchorA, const b2Vec2& localAnchorB,
        const F32 maxLength,
        const bool collideConnected )
{
    // Fetch bodies.
    b2Body* pBodyA = pSceneObjectA != NULL ? pSceneObjectA->getBody() : getGroundBody();
    b2Body* pBodyB = pSceneObjectB != NULL ? pSceneObjectB->getBody() : getGroundBody();
    
    // Populate definition.
    b2RopeJointDef jointDef;
    jointDef.userData         = static_cast<PhysicsProxy*>(this);
    jointDef.collideConnected = collideConnected;
    jointDef.bodyA            = pBodyA;
    jointDef.bodyB            = pBodyB;
    jointDef.localAnchorA     = localAnchorA;
    jointDef.localAnchorB     = localAnchorB;
    jointDef.maxLength        = maxLength < 0.0f ? (pBodyB->GetWorldPoint( localAnchorB ) - pBodyA->GetWorldPoint( localAnchorA )).Length() : maxLength;
    
    // Create joint.
    return createJoint( &jointDef );
}

//-----------------------------------------------------------------------------

void Scene::setRopeJointMaxLength(
        const U32 jointId,
        const F32 maxLength )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_ropeJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return;
    }

    // Cast joint.
    b2RopeJoint* pRealJoint = static_cast<b2RopeJoint*>( pJoint );

    // Access joint.
    pRealJoint->SetMaxLength( maxLength );
}

//-----------------------------------------------------------------------------

F32 Scene::getRopeJointMaxLength( const U32 jointId )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return -1.0f;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_ropeJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return -1.0f;
    }

    // Cast joint.
    b2RopeJoint* pRealJoint = static_cast<b2RopeJoint*>( pJoint );

    // Access joint.
    return pRealJoint->GetMaxLength();
}

//-----------------------------------------------------------------------------

U32 Scene::createRevoluteJoint(
        const SceneObject* pSceneObjectA, const SceneObject* pSceneObjectB,
        const b2Vec2& localAnchorA, const b2Vec2& localAnchorB,
        const bool collideConnected )
{
    // Fetch bodies.
    b2Body* pBodyA = pSceneObjectA != NULL ? pSceneObjectA->getBody() : getGroundBody();
    b2Body* pBodyB = pSceneObjectB != NULL ? pSceneObjectB->getBody() : getGroundBody();
    
    // Populate definition.
    b2RevoluteJointDef jointDef;
    jointDef.userData         = static_cast<PhysicsProxy*>(this);
    jointDef.collideConnected = collideConnected;
    jointDef.bodyA            = pBodyA;
    jointDef.bodyB            = pBodyB;
    jointDef.referenceAngle   = pBodyB->GetAngle() - pBodyA->GetAngle();
    jointDef.localAnchorA     = localAnchorA;
    jointDef.localAnchorB     = localAnchorB;
    
    // Create joint.
    return createJoint( &jointDef );
}

//-----------------------------------------------------------------------------

void Scene::setRevoluteJointLimit(
        const U32 jointId,
        const bool enableLimit,
        const F32 lowerAngle, const F32 upperAngle )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_revoluteJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return;
    }

    // Cast joint.
    b2RevoluteJoint* pRealJoint = static_cast<b2RevoluteJoint*>( pJoint );

    // Access joint.
    pRealJoint->SetLimits( lowerAngle, upperAngle );
    pRealJoint->EnableLimit( enableLimit );
}

//-----------------------------------------------------------------------------

bool Scene::getRevoluteJointLimit(
        const U32 jointId,
        bool& enableLimit,
        F32& lowerAngle, F32& upperAngle )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return false;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_revoluteJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return false;
    }

    // Cast joint.
    b2RevoluteJoint* pRealJoint = static_cast<b2RevoluteJoint*>( pJoint );

    // Access joint.
    enableLimit = pRealJoint->IsLimitEnabled();
    lowerAngle = pRealJoint->GetLowerLimit();
    upperAngle = pRealJoint->GetUpperLimit();

    return true;
}

//-----------------------------------------------------------------------------

void Scene::setRevoluteJointMotor(
        const U32 jointId,
        const bool enableMotor,
        const F32 motorSpeed,
        const F32 maxMotorTorque )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_revoluteJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return;
    }

    // Cast joint.
    b2RevoluteJoint* pRealJoint = static_cast<b2RevoluteJoint*>( pJoint );

    // Access joint.
    pRealJoint->SetMotorSpeed( motorSpeed );
    pRealJoint->SetMaxMotorTorque( maxMotorTorque );
    pRealJoint->EnableMotor( enableMotor );    
}

//-----------------------------------------------------------------------------

bool Scene::getRevoluteJointMotor(
        const U32 jointId,
        bool& enableMotor,
        F32& motorSpeed,
        F32& maxMotorTorque )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return false;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_revoluteJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return false;
    }

    // Cast joint.
    b2RevoluteJoint* pRealJoint = static_cast<b2RevoluteJoint*>( pJoint );

    // Access joint.
    enableMotor = pRealJoint->IsMotorEnabled();
    motorSpeed = pRealJoint->GetMotorSpeed();
    maxMotorTorque = pRealJoint->GetMaxMotorTorque();

    return true;
}

//-----------------------------------------------------------------------------

U32 Scene::createWeldJoint(
        const SceneObject* pSceneObjectA, const SceneObject* pSceneObjectB,
        const b2Vec2& localAnchorA, const b2Vec2& localAnchorB,
        const F32 frequency,
        const F32 dampingRatio,
        const bool collideConnected )
{
    // Fetch bodies.
    b2Body* pBodyA = pSceneObjectA != NULL ? pSceneObjectA->getBody() : getGroundBody();
    b2Body* pBodyB = pSceneObjectB != NULL ? pSceneObjectB->getBody() : getGroundBody();
    
    // Populate definition.
    b2WeldJointDef jointDef;
    jointDef.userData         = static_cast<PhysicsProxy*>(this);
    jointDef.collideConnected = collideConnected;
    jointDef.bodyA            = pBodyA;
    jointDef.bodyB            = pBodyB;
    jointDef.referenceAngle   = pBodyB->GetAngle() - pBodyA->GetAngle();
    jointDef.localAnchorA     = localAnchorA;
    jointDef.localAnchorB     = localAnchorB;
    jointDef.frequencyHz      = frequency;
    jointDef.dampingRatio     = dampingRatio;
    
    // Create joint.
    return createJoint( &jointDef );
}

//-----------------------------------------------------------------------------

void Scene::setWeldJointFrequency(
        const U32 jointId,
        const F32 frequency )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_weldJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return;
    }

    // Cast joint.
    b2WeldJoint* pRealJoint = static_cast<b2WeldJoint*>( pJoint );

    // Access joint.
    pRealJoint->SetFrequency( frequency );
}


//-----------------------------------------------------------------------------

F32 Scene::getWeldJointFrequency( const U32 jointId  )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return -1.0f;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_weldJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return -1.0f;
    }

    // Cast joint.
    b2WeldJoint* pRealJoint = static_cast<b2WeldJoint*>( pJoint );

    // Access joint.
    return pRealJoint->GetFrequency();
}

//-----------------------------------------------------------------------------

void Scene::setWeldJointDampingRatio(
        const U32 jointId,
        const F32 dampingRatio )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_weldJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return;
    }

    // Cast joint.
    b2WeldJoint* pRealJoint = static_cast<b2WeldJoint*>( pJoint );

    // Access joint.
    pRealJoint->SetDampingRatio( dampingRatio );
}

//-----------------------------------------------------------------------------

F32 Scene::getWeldJointDampingRatio( const U32 jointId )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return -1.0f;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_weldJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return -1.0f;
    }

    // Cast joint.
    b2WeldJoint* pRealJoint = static_cast<b2WeldJoint*>( pJoint );

    // Access joint.
    return pRealJoint->GetDampingRatio();
}

//-----------------------------------------------------------------------------

U32 Scene::createWheelJoint(
        const SceneObject* pSceneObjectA, const SceneObject* pSceneObjectB,
        const b2Vec2& localAnchorA, const b2Vec2& localAnchorB,
        const b2Vec2& worldAxis,
        const bool collideConnected )
{
    // Fetch bodies.
    b2Body* pBodyA = pSceneObjectA != NULL ? pSceneObjectA->getBody() : getGroundBody();
    b2Body* pBodyB = pSceneObjectB != NULL ? pSceneObjectB->getBody() : getGroundBody();
    
    // Populate definition.
    b2WheelJointDef jointDef;
    jointDef.userData         = static_cast<PhysicsProxy*>(this);
    jointDef.collideConnected = collideConnected;
    jointDef.bodyA            = pBodyA;
    jointDef.bodyB            = pBodyB;
    jointDef.localAnchorA     = localAnchorA;
    jointDef.localAnchorB     = localAnchorB;
    jointDef.localAxisA       = pBodyA->GetLocalVector( worldAxis );
    
    // Create joint.
    return createJoint( &jointDef );
}

//-----------------------------------------------------------------------------

void Scene::setWheelJointMotor(
        const U32 jointId,
        const bool enableMotor,
        const F32 motorSpeed,
        const F32 maxMotorTorque )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_wheelJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return;
    }

    // Cast joint.
    b2WheelJoint* pRealJoint = static_cast<b2WheelJoint*>( pJoint );

    // Access joint.
    pRealJoint->SetMotorSpeed( motorSpeed );
    pRealJoint->SetMaxMotorTorque( maxMotorTorque );
    pRealJoint->EnableMotor( enableMotor ); 
}

//-----------------------------------------------------------------------------

bool Scene::getWheelJointMotor(
        const U32 jointId,
        bool& enableMotor,
        F32& motorSpeed,
        F32& maxMotorTorque )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return false;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_wheelJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return false;
    }

    // Cast joint.
    b2WheelJoint* pRealJoint = static_cast<b2WheelJoint*>( pJoint );

    // Access joint.
    enableMotor = pRealJoint->IsMotorEnabled();
    motorSpeed = pRealJoint->GetMotorSpeed();
    maxMotorTorque = pRealJoint->GetMaxMotorTorque();

    return true;
}

//-----------------------------------------------------------------------------

void Scene::setWheelJointFrequency(
        const U32 jointId,
        const F32 frequency )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_wheelJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return;
    }

    // Cast joint.
    b2WheelJoint* pRealJoint = static_cast<b2WheelJoint*>( pJoint );

    // Access joint.
    pRealJoint->SetSpringFrequencyHz( frequency );
}

//-----------------------------------------------------------------------------

F32 Scene::getWheelJointFrequency( const U32 jointId )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return -1.0f;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_wheelJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return -1.0f;
    }

    // Cast joint.
    b2WheelJoint* pRealJoint = static_cast<b2WheelJoint*>( pJoint );

    // Access joint.
    return pRealJoint->GetSpringFrequencyHz();
}

//-----------------------------------------------------------------------------

void Scene::setWheelJointDampingRatio(
        const U32 jointId,
        const F32 dampingRatio )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_wheelJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return;
    }

    // Cast joint.
    b2WheelJoint* pRealJoint = static_cast<b2WheelJoint*>( pJoint );

    // Access joint.
    pRealJoint->SetSpringDampingRatio( dampingRatio );
}

//-----------------------------------------------------------------------------

F32 Scene::getWheelJointDampingRatio( const U32 jointId )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return -1.0f;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_wheelJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return -1.0f;
    }

    // Cast joint.
    b2WheelJoint* pRealJoint = static_cast<b2WheelJoint*>( pJoint );

    // Access joint.
    return pRealJoint->GetSpringDampingRatio();
}

//-----------------------------------------------------------------------------

U32 Scene::createFrictionJoint(
        const SceneObject* pSceneObjectA, const SceneObject* pSceneObjectB,
        const b2Vec2& localAnchorA, const b2Vec2& localAnchorB,
        const F32 maxForce,
        const F32 maxTorque,
        const bool collideConnected )
{
    // Fetch bodies.
    b2Body* pBodyA = pSceneObjectA != NULL ? pSceneObjectA->getBody() : getGroundBody();
    b2Body* pBodyB = pSceneObjectB != NULL ? pSceneObjectB->getBody() : getGroundBody();
    
    // Populate definition.
    b2FrictionJointDef jointDef;
    jointDef.userData         = static_cast<PhysicsProxy*>(this);
    jointDef.collideConnected = collideConnected;
    jointDef.bodyA            = pBodyA;
    jointDef.bodyB            = pBodyB;
    jointDef.localAnchorA     = localAnchorA;
    jointDef.localAnchorB     = localAnchorB;
    jointDef.maxForce         = maxForce;
    jointDef.maxTorque        = maxTorque;
    
    // Create joint.
    return createJoint( &jointDef );
}

//-----------------------------------------------------------------------------

void Scene::setFrictionJointMaxForce(
        const U32 jointId,
        const F32 maxForce )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_frictionJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return;
    }

    // Cast joint.
    b2FrictionJoint* pRealJoint = static_cast<b2FrictionJoint*>( pJoint );

    // Access joint.
    pRealJoint->SetMaxForce( maxForce );
}

//-----------------------------------------------------------------------------

F32 Scene::getFrictionJointMaxForce( const U32 jointId )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return -1.0f;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_frictionJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return -1.0f;
    }

    // Cast joint.
    b2FrictionJoint* pRealJoint = static_cast<b2FrictionJoint*>( pJoint );

    // Access joint.
    return pRealJoint->GetMaxForce();
}

//-----------------------------------------------------------------------------

void Scene::setFrictionJointMaxTorque(
        const U32 jointId,
        const F32 maxTorque )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_frictionJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return;
    }

    // Cast joint.
    b2FrictionJoint* pRealJoint = static_cast<b2FrictionJoint*>( pJoint );

    // Access joint.
    pRealJoint->SetMaxTorque( maxTorque );
}


//-----------------------------------------------------------------------------

F32 Scene::getFrictionJointMaxTorque( const U32 jointId )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return -1.0f;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_frictionJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return -1.0f;
    }

    // Cast joint.
    b2FrictionJoint* pRealJoint = static_cast<b2FrictionJoint*>( pJoint );

    // Access joint.
    return pRealJoint->GetMaxTorque();
}

//-----------------------------------------------------------------------------

U32 Scene::createPrismaticJoint(
        const SceneObject* pSceneObjectA, const SceneObject* pSceneObjectB,
        const b2Vec2& localAnchorA, const b2Vec2& localAnchorB,
        const b2Vec2& worldAxis,
        const bool collideConnected )
{
    // Fetch bodies.
    b2Body* pBodyA = pSceneObjectA != NULL ? pSceneObjectA->getBody() : getGroundBody();
    b2Body* pBodyB = pSceneObjectB != NULL ? pSceneObjectB->getBody() : getGroundBody();
    
    // Populate definition.
    b2PrismaticJointDef jointDef;
    jointDef.userData         = static_cast<PhysicsProxy*>(this);
    jointDef.collideConnected = collideConnected;
    jointDef.bodyA            = pBodyA;
    jointDef.bodyB            = pBodyB;
    jointDef.referenceAngle   = pBodyB->GetAngle() - pBodyA->GetAngle();
    jointDef.localAnchorA     = localAnchorA;
    jointDef.localAnchorB     = localAnchorB;
    jointDef.localAxisA       = pBodyA->GetLocalVector( worldAxis );
    
    // Create joint.
    return createJoint( &jointDef );
}

//-----------------------------------------------------------------------------

void Scene::setPrismaticJointLimit(
        const U32 jointId,
        const bool enableLimit,
        const F32 lowerTranslation, const F32 upperTranslation )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_prismaticJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return;
    }

    // Cast joint.
    b2PrismaticJoint* pRealJoint = static_cast<b2PrismaticJoint*>( pJoint );

    // Access joint.
    pRealJoint->SetLimits( lowerTranslation, upperTranslation );
    pRealJoint->EnableLimit( enableLimit );
}

//-----------------------------------------------------------------------------

bool Scene::getPrismaticJointLimit(
        const U32 jointId,
        bool& enableLimit,
        F32& lowerTranslation, F32& upperTranslation )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return false;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_prismaticJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return false;
    }

    // Cast joint.
    b2PrismaticJoint* pRealJoint = static_cast<b2PrismaticJoint*>( pJoint );

    // Access joint.
    enableLimit = pRealJoint->IsLimitEnabled();
    lowerTranslation = pRealJoint->GetLowerLimit();
    upperTranslation = pRealJoint->GetUpperLimit();

    return true;
}

//-----------------------------------------------------------------------------

void Scene::setPrismaticJointMotor(
        const U32 jointId,
        const bool enableMotor,
        const F32 motorSpeed,
        const F32 maxMotorForce )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_prismaticJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return;
    }

    // Cast joint.
    b2PrismaticJoint* pRealJoint = static_cast<b2PrismaticJoint*>( pJoint );

    // Access joint.
    pRealJoint->SetMotorSpeed( motorSpeed );
    pRealJoint->SetMaxMotorForce( maxMotorForce );
    pRealJoint->EnableMotor( enableMotor ); 
}

//-----------------------------------------------------------------------------

bool Scene::getPrismaticJointMotor(
        const U32 jointId,
        bool& enableMotor,
        F32& motorSpeed,
        F32& maxMotorForce )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return false;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_prismaticJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return false;
    }

    // Cast joint.
    b2PrismaticJoint* pRealJoint = static_cast<b2PrismaticJoint*>( pJoint );

    // Access joint.
    enableMotor = pRealJoint->IsMotorEnabled();
    motorSpeed = pRealJoint->GetMotorSpeed();
    maxMotorForce = pRealJoint->GetMaxMotorForce();

    return true;
}

//-----------------------------------------------------------------------------

U32 Scene::createPulleyJoint(
        const SceneObject* pSceneObjectA, const SceneObject* pSceneObjectB,
        const b2Vec2& localAnchorA, const b2Vec2& localAnchorB,
        const b2Vec2& worldGroundAnchorA, const b2Vec2& worldGroundAnchorB,
        const F32 ratio,
        const F32 lengthA, const F32 lengthB,
        const bool collideConnected )
{
    // Fetch bodies.
    b2Body* pBodyA = pSceneObjectA != NULL ? pSceneObjectA->getBody() : getGroundBody();
    b2Body* pBodyB = pSceneObjectB != NULL ? pSceneObjectB->getBody() : getGroundBody();
    
    // Populate definition.
    b2PulleyJointDef jointDef;
    jointDef.userData         = static_cast<PhysicsProxy*>(this);
    jointDef.collideConnected = collideConnected;
    jointDef.bodyA            = pBodyA;
    jointDef.bodyB            = pBodyB;
    jointDef.groundAnchorA    = worldGroundAnchorA;
    jointDef.groundAnchorB    = worldGroundAnchorB;
    jointDef.localAnchorA     = localAnchorA;
    jointDef.localAnchorB     = localAnchorB;
    jointDef.lengthA          = lengthA < 0.0f ? (pBodyA->GetWorldPoint( localAnchorA ) - worldGroundAnchorA).Length() : lengthA;
    jointDef.lengthB          = lengthB < 0.0f ? (pBodyA->GetWorldPoint( localAnchorB ) - worldGroundAnchorB).Length() : lengthB;
    jointDef.ratio            = ratio > b2_epsilon ? ratio : b2_epsilon + b2_epsilon;
    
    // Create joint.
    return createJoint( &jointDef );
}

//-----------------------------------------------------------------------------

U32 Scene::createTargetJoint(
        const SceneObject* pSceneObject,
        const b2Vec2& worldTarget,
        const F32 maxForce,
        const F32 frequency,
        const F32 dampingRatio,
        const bool collideConnected )
{
    // Sanity!
    AssertFatal( pSceneObject != NULL, "Invalid scene object." );

    // Fetch bodies.
    b2Body* pBody = pSceneObject->getBody();
    
    // Populate definition.
    b2MouseJointDef jointDef;
    jointDef.userData         = static_cast<PhysicsProxy*>(this);
    jointDef.collideConnected = collideConnected;
    jointDef.bodyA            = getGroundBody();
    jointDef.bodyB            = pBody;
    jointDef.target           = pBody->GetPosition();
    jointDef.maxForce         = maxForce;
    jointDef.frequencyHz      = frequency;
    jointDef.dampingRatio     = dampingRatio;

    // Create joint.
    const U32 jointId = createJoint( &jointDef );

    // Cast joint.
    b2MouseJoint* pRealJoint = static_cast<b2MouseJoint*>( getJoint( jointId ) );

    // Access joint.
    // NOTE:-   This is done because initially the target (mouse) joint assumes the target 
    //          coincides with the body anchor.
    pRealJoint->SetTarget( worldTarget );

    return jointId;
}

//-----------------------------------------------------------------------------

void Scene::setTargetJointTarget(
        const U32 jointId,
        const b2Vec2& worldTarget )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_mouseJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return;
    }

    // Cast joint.
    b2MouseJoint* pRealJoint = static_cast<b2MouseJoint*>( pJoint );

    // Access joint.
    pRealJoint->SetTarget( worldTarget );
}

//-----------------------------------------------------------------------------
b2Vec2 Scene::getTargetJointTarget( const U32 jointId )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return b2Vec2_zero;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_mouseJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return b2Vec2_zero;
    }

    // Cast joint.
    b2MouseJoint* pRealJoint = static_cast<b2MouseJoint*>( pJoint );

    // Access joint.
    return pRealJoint->GetTarget();
}

//-----------------------------------------------------------------------------

void Scene::setTargetJointMaxForce(
        const U32 jointId,
        const F32 maxForce )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_mouseJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return;
    }

    // Cast joint.
    b2MouseJoint* pRealJoint = static_cast<b2MouseJoint*>( pJoint );

    // Access joint.
    pRealJoint->SetMaxForce( maxForce );
}

//-----------------------------------------------------------------------------

F32 Scene::getTargetJointMaxForce( const U32 jointId )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return -1.0f;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_mouseJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return -1.0f;
    }

    // Cast joint.
    b2MouseJoint* pRealJoint = static_cast<b2MouseJoint*>( pJoint );

    // Access joint.
    return pRealJoint->GetMaxForce();
}

//-----------------------------------------------------------------------------

void Scene::setTargetJointFrequency(
        const U32 jointId,
        const F32 frequency )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_mouseJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return;
    }

    // Cast joint.
    b2MouseJoint* pRealJoint = static_cast<b2MouseJoint*>( pJoint );

    // Access joint.
    pRealJoint->SetFrequency( frequency );
}

//-----------------------------------------------------------------------------

F32 Scene::getTargetJointFrequency( const U32 jointId )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return -1.0f;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_mouseJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return -1.0f;
    }

    // Cast joint.
    b2MouseJoint* pRealJoint = static_cast<b2MouseJoint*>( pJoint );

    // Access joint.
    return pRealJoint->GetFrequency();
}

//-----------------------------------------------------------------------------

void Scene::setTargetJointDampingRatio(
        const U32 jointId,
        const F32 dampingRatio )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_mouseJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return;
    }

    // Cast joint.
    b2MouseJoint* pRealJoint = static_cast<b2MouseJoint*>( pJoint );

    // Access joint.
    pRealJoint->SetDampingRatio( dampingRatio );
}

//-----------------------------------------------------------------------------

F32 Scene::getTargetJointDampingRatio( const U32 jointId )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return -1.0f;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_mouseJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return -1.0f;
    }

    // Cast joint.
    b2MouseJoint* pRealJoint = static_cast<b2MouseJoint*>( pJoint );

    // Access joint.
    return pRealJoint->GetDampingRatio();
}

//-----------------------------------------------------------------------------

U32 Scene::createMotorJoint(
            const SceneObject* pSceneObjectA, const SceneObject* pSceneObjectB,
            const b2Vec2 linearOffset,
            const F32 angularOffset,
            const F32 maxForce,
            const F32 maxTorque,
            const F32 correctionFactor,
            const bool collideConnected )
{
    // Fetch bodies.
    b2Body* pBodyA = pSceneObjectA != NULL ? pSceneObjectA->getBody() : getGroundBody();
    b2Body* pBodyB = pSceneObjectB != NULL ? pSceneObjectB->getBody() : getGroundBody();
    
    // Populate definition.
    b2MotorJointDef jointDef;
    jointDef.userData           = static_cast<PhysicsProxy*>(this);
    jointDef.collideConnected   = collideConnected;
    jointDef.bodyA              = pBodyA;
    jointDef.bodyB              = pBodyB;
    jointDef.linearOffset       = linearOffset;
    jointDef.angularOffset      = angularOffset;
    jointDef.correctionFactor   = correctionFactor;
    jointDef.maxForce           = maxForce;
    jointDef.maxTorque          = maxTorque;
    
    // Create joint.
    return createJoint( &jointDef );
}

//-----------------------------------------------------------------------------

void Scene::setMotorJointLinearOffset(
        const U32 jointId,
        const b2Vec2& linearOffset )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_motorJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return;
    }

    // Cast joint.
    b2MotorJoint* pRealJoint = static_cast<b2MotorJoint*>( pJoint );

    // Access joint.
    pRealJoint->SetLinearOffset( linearOffset );
}

//-----------------------------------------------------------------------------

b2Vec2 Scene::getMotorJointLinearOffset( const U32 jointId )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return b2Vec2_zero;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_motorJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return b2Vec2_zero;
    }

    // Cast joint.
    b2MotorJoint* pRealJoint = static_cast<b2MotorJoint*>( pJoint );

    // Access joint.
    return pRealJoint->GetLinearOffset();
}

//-----------------------------------------------------------------------------

void Scene::setMotorJointAngularOffset(
        const U32 jointId,
        const F32 angularOffset )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_motorJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return;
    }

    // Cast joint.
    b2MotorJoint* pRealJoint = static_cast<b2MotorJoint*>( pJoint );

    // Access joint.
    pRealJoint->SetAngularOffset( angularOffset );
}

//-----------------------------------------------------------------------------

F32 Scene::getMotorJointAngularOffset( const U32 jointId )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return -1.0f;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_motorJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return -1.0f;
    }

    // Cast joint.
    b2MotorJoint* pRealJoint = static_cast<b2MotorJoint*>( pJoint );

    // Access joint.
    return pRealJoint->GetAngularOffset();
}

//-----------------------------------------------------------------------------

void Scene::setMotorJointMaxForce(
        const U32 jointId,
        const F32 maxForce )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_motorJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return;
    }

    // Cast joint.
    b2MotorJoint* pRealJoint = static_cast<b2MotorJoint*>( pJoint );

    // Access joint.
    pRealJoint->SetMaxForce( maxForce );
}

//-----------------------------------------------------------------------------

F32 Scene::getMotorJointMaxForce( const U32 jointId )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return -1.0f;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_motorJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return -1.0f;
    }

    // Cast joint.
    b2MotorJoint* pRealJoint = static_cast<b2MotorJoint*>( pJoint );

    // Access joint.
    return pRealJoint->GetMaxForce();
}

//-----------------------------------------------------------------------------

void Scene::setMotorJointMaxTorque(
        const U32 jointId,
        const F32 maxTorque )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_motorJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return;
    }

    // Cast joint.
    b2MotorJoint* pRealJoint = static_cast<b2MotorJoint*>( pJoint );

    // Access joint.
    pRealJoint->SetMaxTorque( maxTorque );
}


//-----------------------------------------------------------------------------

F32 Scene::getMotorJointMaxTorque( const U32 jointId )
{
    // Fetch joint.
    b2Joint* pJoint = getJoint( jointId );

    // Ignore invalid joint.
    if ( !pJoint )
        return -1.0f;

    // Fetch joint type.
    const b2JointType jointType = pJoint->GetType();

    if ( jointType != e_motorJoint )
    {
        Con::warnf( "Invalid joint type of %s.", getJointTypeDescription(jointType) );
        return -1.0f;
    }

    // Cast joint.
    b2MotorJoint* pRealJoint = static_cast<b2MotorJoint*>( pJoint );

    // Access joint.
    return pRealJoint->GetMaxTorque();
}

//-----------------------------------------------------------------------------

void Scene::setDebugSceneObject( SceneObject* pSceneObject )
{
    // Ignore no change.
    if ( mpDebugSceneObject == pSceneObject )
        return;

    // Remove delete notification for existing monitored object.
    if ( mpDebugSceneObject != NULL )
        clearNotify( mpDebugSceneObject );

    // Set monitored scene object.
    mpDebugSceneObject = pSceneObject;

    // Finish if resetting monitored object.
    if ( pSceneObject == NULL )
        return;

    // Add delete notification for new monitored object/
    deleteNotify( pSceneObject );
}

//-----------------------------------------------------------------------------

void Scene::attachSceneWindow( SceneWindow* pSceneWindow2D )
{
    // Ignore if already attached.
    if ( isSceneWindowAttached( pSceneWindow2D ) )
        return;

    // Add to Attached List.
    mAttachedSceneWindows.addObject( pSceneWindow2D );
}

//-----------------------------------------------------------------------------

void Scene::detachSceneWindow( SceneWindow* pSceneWindow2D )
{
    // Ignore if not attached.
    if ( !isSceneWindowAttached( pSceneWindow2D ) )
        return;

    // Add to Attached List.
    mAttachedSceneWindows.removeObject( pSceneWindow2D );
}

//-----------------------------------------------------------------------------

void Scene::detachAllSceneWindows( void )
{
    // Detach All Scene Windows.
    while( mAttachedSceneWindows.size() > 0 )
        dynamic_cast<SceneWindow*>(mAttachedSceneWindows[mAttachedSceneWindows.size()-1])->resetScene();
}

//-----------------------------------------------------------------------------

bool Scene::isSceneWindowAttached( SceneWindow* pSceneWindow2D )
{
    for( SimSet::iterator itr = mAttachedSceneWindows.begin(); itr != mAttachedSceneWindows.end(); itr++ )
        if ( pSceneWindow2D == dynamic_cast<SceneWindow*>(*itr) )
            // Found.
            return true;

    // Not Found.
    return false;
}

//-----------------------------------------------------------------------------

void Scene::addDeleteRequest( SceneObject* pSceneObject )
{
    // Ignore if it's already being safe-deleted.
    if ( pSceneObject->isBeingDeleted() )
        return;

    // Populate Delete Request.
    tDeleteRequest deleteRequest = { pSceneObject->getId(), NULL, false };

    // Push Delete Request.
    mDeleteRequests.push_back( deleteRequest );

    // Flag Delete in Progress.
    pSceneObject->mBeingSafeDeleted = true;
}


//-----------------------------------------------------------------------------

void Scene::processDeleteRequests( const bool forceImmediate )
{
    // Ignore if there's no delete requests!
    if ( mDeleteRequests.size() == 0 )
        return;

    // Validate All Delete Requests.
    U32 safeDeleteReadyCount = 0;
    for ( U32 requestIndex = 0; requestIndex < (U32)mDeleteRequests.size(); )
    {
        // Fetch Reference to Delete Request.
        tDeleteRequest& deleteRequest = mDeleteRequests[requestIndex];

        // Fetch Object.
        // NOTE:- Let's be safer and check that it's definitely a scene-object.
        SceneObject* pSceneObject = dynamic_cast<SceneObject*>( Sim::findObject( deleteRequest.mObjectId ) );

        // Does this object exist?
        if ( pSceneObject )
        {
            // Yes, so write object.
            deleteRequest.mpSceneObject = pSceneObject;

            // Calculate Safe-Ready Flag.
            deleteRequest.mSafeDeleteReady = forceImmediate || pSceneObject->getSafeDelete();

            // Is it ready to safe-delete?
            if ( deleteRequest.mSafeDeleteReady )
            {
                // Yes, so increase safe-ready count.
                ++safeDeleteReadyCount;
            }         
        }
        else
        {
            // No, so it looks like the object got deleted prematurely; let's just remove
            // the request instead.
            mDeleteRequests.erase_fast( requestIndex );
            
            // Repeat this item.
            continue;
        }

        // Skip to next request index.
        ++requestIndex;
    }

    // Stop if there's no delete requests!
    if ( mDeleteRequests.size() == 0 )
        return;

    // Transfer Delete-Requests to Temporary version.
    // NOTE:-   We do this because we may delete objects which have dependencies.  This would
    //          cause objects to be added to the safe-delete list.  We don't want to work on
    //          the list whilst this is happening so we'll transfer it to a temporary list.
    mDeleteRequestsTemp = mDeleteRequests;

    // Can we process all remaining delete-requests?
    if ( safeDeleteReadyCount == (U32)mDeleteRequestsTemp.size() )
    {
        // Yes, so process ALL safe-ready delete-requests.
        for ( U32 requestIndex = 0; requestIndex < (U32)mDeleteRequestsTemp.size(); ++requestIndex )
        {
            // Yes, so fetch object.
            SceneObject* pSceneObject = mDeleteRequestsTemp[requestIndex].mpSceneObject;

            // Do script callback.
            Con::executef(this, 2, "onSafeDelete", pSceneObject->getIdString() );

            // Destroy the object.
            pSceneObject->deleteObject();
        }

        // Remove All delete-requests.
        mDeleteRequestsTemp.clear();
    }
    else
    {
        // No, so process only safe-ready delete-requests.
        for ( U32 requestIndex = 0; requestIndex <(U32) mDeleteRequestsTemp.size(); )
        {
            // Fetch Reference to Delete Request.
            tDeleteRequest& deleteRequest = mDeleteRequestsTemp[requestIndex];

            // Is the Object Safe-Ready?
            if ( deleteRequest.mSafeDeleteReady )
            {
                // Yes, so fetch object.
                SceneObject* pSceneObject = deleteRequest.mpSceneObject;

                // Do script callback.
                Con::executef(this, 2, "onSafeDelete", pSceneObject->getIdString() );

                // Destroy the object.
                pSceneObject->deleteObject();

                // Quickly remove delete-request.
                mDeleteRequestsTemp.erase_fast( requestIndex );

                // Repeat this item.
                continue;
            }

            // Skip to next request index.
            ++requestIndex;
        }
    }
}

//-----------------------------------------------------------------------------

void Scene::SayGoodbye( b2Joint* pJoint )
{
    // Find the joint id.
    const U32 jointId = findJointId( pJoint );

    // Ignore a bad joint.
    if ( jointId == 0 )
        return;

    // Remove joint references.
    mJoints.erase( jointId );
    mReverseJoints.erase( (U32)pJoint );
}

//-----------------------------------------------------------------------------

void Scene::SayGoodbye( b2Fixture* pFixture )
{
    // The scene is not currently interested in tracking fixtures
    // so we do nothing here for now.
}

//-----------------------------------------------------------------------------

void Scene::onTamlPreWrite( void )
{
    // Call parent.
    Parent::onTamlPreWrite();
}

//-----------------------------------------------------------------------------

void Scene::onTamlPostWrite( void )
{
    // Call parent.
    Parent::onTamlPostWrite();
}

//-----------------------------------------------------------------------------

void Scene::onTamlPreRead( void )
{
    // Call parent.
    Parent::onTamlPreRead();
}

//-----------------------------------------------------------------------------

void Scene::onTamlPostRead( const TamlCollection& customCollection )
{
    // Call parent.
    Parent::onTamlPostRead( customCollection );

    // Reset the loading scene.
    Scene::LoadingScene = NULL;

    // Find joint collection.
    const TamlCollectionProperty* pJointProperty = customCollection.findProperty( jointCollectionName );

    // Finish if no joints.
    if ( pJointProperty == NULL )
        return;

    // Iterate joints.
    for( TamlCollectionProperty::const_iterator propertyTypeAliasItr = pJointProperty->begin(); propertyTypeAliasItr != pJointProperty->end(); ++propertyTypeAliasItr )
    {
        // Fetch property type alias.
        TamlPropertyTypeAlias* pPropertyTypeAlias = *propertyTypeAliasItr;

        // Fetch alias name.
        StringTableEntry aliasName = pPropertyTypeAlias->mAliasName;

        // Is this a distance joint?
        if ( aliasName == jointDistanceTypeName )
        {
            SceneObject* pSceneObjectA = NULL;
            SceneObject* pSceneObjectB = NULL;
            b2Vec2 localAnchorA = b2Vec2_zero;
            b2Vec2 localAnchorB = b2Vec2_zero;
            bool collideConnected = false;

            F32 length = -1.0f;
            F32 frequency = 0.0f;
            F32 dampingRatio = 0.0f;

            // Iterate property fields.
            for ( TamlPropertyTypeAlias::const_iterator propertyFieldItr = pPropertyTypeAlias->begin(); propertyFieldItr != pPropertyTypeAlias->end(); ++propertyFieldItr )
            {
                // Fetch property field.
                TamlPropertyField* pPropertyField = *propertyFieldItr;

                // Fetch property field name.
                StringTableEntry fieldName = pPropertyField->getFieldName();

                // Fetch fields.
                if ( fieldName == jointObjectAName )
                {
                    pSceneObjectA = dynamic_cast<SceneObject*>( pPropertyField->getFieldObject() );
                }
                else if ( fieldName == jointObjectBName )
                {
                    pSceneObjectB = dynamic_cast<SceneObject*>( pPropertyField->getFieldObject() );
                }
                else if ( fieldName == jointLocalAnchorAName )
                {
                    pPropertyField->getFieldValue( localAnchorA );
                }
                else if ( fieldName == jointLocalAnchorBName )
                {
                    pPropertyField->getFieldValue( localAnchorB );
                }
                else if ( fieldName == jointCollideConnectedName )
                {
                    pPropertyField->getFieldValue( collideConnected );
                }
                else if ( fieldName == jointDistanceLengthName )
                {
                    pPropertyField->getFieldValue( length );
                }
                else if ( fieldName == jointDistanceFrequencyName )
                {
                    pPropertyField->getFieldValue( frequency );
                }
                else if ( fieldName == jointDistanceDampingRatioName )
                {
                    pPropertyField->getFieldValue( dampingRatio );
                }
            }

            // Create joint.
            createDistanceJoint( pSceneObjectA, pSceneObjectB, localAnchorA, localAnchorB, length, frequency, dampingRatio, collideConnected );
        }
        // is this a rope joint?
        else if ( aliasName == jointRopeTypeName )
        {
            SceneObject* pSceneObjectA = NULL;
            SceneObject* pSceneObjectB = NULL;
            b2Vec2 localAnchorA = b2Vec2_zero;
            b2Vec2 localAnchorB = b2Vec2_zero;
            bool collideConnected = false;

            F32 maxLength = -1.0f;

            // Iterate property fields.
            for ( TamlPropertyTypeAlias::const_iterator propertyFieldItr = pPropertyTypeAlias->begin(); propertyFieldItr != pPropertyTypeAlias->end(); ++propertyFieldItr )
            {
                // Fetch property field.
                TamlPropertyField* pPropertyField = *propertyFieldItr;

                // Fetch property field name.
                StringTableEntry fieldName = pPropertyField->getFieldName();

                // Fetch fields.
                if ( fieldName == jointObjectAName )
                {
                    pSceneObjectA = dynamic_cast<SceneObject*>( pPropertyField->getFieldObject() );
                }
                else if ( fieldName == jointObjectBName )
                {
                    pSceneObjectB = dynamic_cast<SceneObject*>( pPropertyField->getFieldObject() );
                }
                else if ( fieldName == jointLocalAnchorAName )
                {
                    pPropertyField->getFieldValue( localAnchorA );
                }
                else if ( fieldName == jointLocalAnchorBName )
                {
                    pPropertyField->getFieldValue( localAnchorB );
                }
                else if ( fieldName == jointCollideConnectedName )
                {
                    pPropertyField->getFieldValue( collideConnected );
                }
                else if ( fieldName == jointRopeMaxLengthName )
                {
                    pPropertyField->getFieldValue( maxLength );
                }
            }

            // Create joint.
            createRopeJoint( pSceneObjectA, pSceneObjectB, localAnchorA, localAnchorB, maxLength, collideConnected );
        }
        // Is this a revolute joint?
        else if ( aliasName == jointRevoluteTypeName )
        {
            SceneObject* pSceneObjectA = NULL;
            SceneObject* pSceneObjectB = NULL;
            b2Vec2 localAnchorA = b2Vec2_zero;
            b2Vec2 localAnchorB = b2Vec2_zero;
            bool collideConnected = false;

            bool enableLimit = false;
            F32 lowerAngle = 0.0f;
            F32 upperAngle = 0.0f;

            bool enableMotor = false;
            F32 motorSpeed = b2_pi;
            F32 maxMotorTorque = 0.0f;

            // Iterate property fields.
            for ( TamlPropertyTypeAlias::const_iterator propertyFieldItr = pPropertyTypeAlias->begin(); propertyFieldItr != pPropertyTypeAlias->end(); ++propertyFieldItr )
            {
                // Fetch property field.
                TamlPropertyField* pPropertyField = *propertyFieldItr;

                // Fetch property field name.
                StringTableEntry fieldName = pPropertyField->getFieldName();

                // Fetch fields.
                if ( fieldName == jointObjectAName )
                {
                    pSceneObjectA = dynamic_cast<SceneObject*>( pPropertyField->getFieldObject() );
                }
                else if ( fieldName == jointObjectBName )
                {
                    pSceneObjectB = dynamic_cast<SceneObject*>( pPropertyField->getFieldObject() );
                }
                else if ( fieldName == jointLocalAnchorAName )
                {
                    pPropertyField->getFieldValue( localAnchorA );
                }
                else if ( fieldName == jointLocalAnchorBName )
                {
                    pPropertyField->getFieldValue( localAnchorB );
                }
                else if ( fieldName == jointCollideConnectedName )
                {
                    pPropertyField->getFieldValue( collideConnected );
                }
                else if ( fieldName == jointRevoluteLimitLowerAngleName )
                {
                    pPropertyField->getFieldValue( lowerAngle );
                    lowerAngle = mDegToRad( lowerAngle );
                    enableLimit = true;
                }
                else if ( fieldName == jointRevoluteLimitUpperAngleName )
                {
                    pPropertyField->getFieldValue( upperAngle );
                    upperAngle = mDegToRad( upperAngle );
                    enableLimit = true;
                }
                else if ( fieldName == jointRevoluteMotorSpeedName )
                {
                    pPropertyField->getFieldValue( motorSpeed );
                    motorSpeed = mDegToRad( motorSpeed );
                    enableMotor = true;
                }
                else if ( fieldName == jointRevoluteMotorMaxTorqueName )
                {
                    pPropertyField->getFieldValue( maxMotorTorque );
                    enableMotor = true;
                }
            }

            // Create joint.
            const U32 jointId = createRevoluteJoint( pSceneObjectA, pSceneObjectB, localAnchorA, localAnchorB, collideConnected );
            
            if ( enableLimit )
                setRevoluteJointLimit( jointId, true, lowerAngle, upperAngle );

            if ( enableMotor )
                setRevoluteJointMotor( jointId, true, motorSpeed, maxMotorTorque );
        }
        // is this a weld joint?
        else if ( aliasName == jointWeldTypeName )
        {
            SceneObject* pSceneObjectA = NULL;
            SceneObject* pSceneObjectB = NULL;
            b2Vec2 localAnchorA = b2Vec2_zero;
            b2Vec2 localAnchorB = b2Vec2_zero;
            bool collideConnected = false;

            F32 frequency = 0.0f;
            F32 dampingRatio = 0.0f;

            // Iterate property fields.
            for ( TamlPropertyTypeAlias::const_iterator propertyFieldItr = pPropertyTypeAlias->begin(); propertyFieldItr != pPropertyTypeAlias->end(); ++propertyFieldItr )
            {
                // Fetch property field.
                TamlPropertyField* pPropertyField = *propertyFieldItr;

                // Fetch property field name.
                StringTableEntry fieldName = pPropertyField->getFieldName();

                // Fetch fields.
                if ( fieldName == jointObjectAName )
                {
                    pSceneObjectA = dynamic_cast<SceneObject*>( pPropertyField->getFieldObject() );
                }
                else if ( fieldName == jointObjectBName )
                {
                    pSceneObjectB = dynamic_cast<SceneObject*>( pPropertyField->getFieldObject() );
                }
                else if ( fieldName == jointLocalAnchorAName )
                {
                    pPropertyField->getFieldValue( localAnchorA );
                }
                else if ( fieldName == jointLocalAnchorBName )
                {
                    pPropertyField->getFieldValue( localAnchorB );
                }
                else if ( fieldName == jointCollideConnectedName )
                {
                    pPropertyField->getFieldValue( collideConnected );
                }
                else if ( fieldName == jointWeldFrequencyName )
                {
                    pPropertyField->getFieldValue( frequency );
                }
                else if ( fieldName == jointWeldDampingRatioName )
                {
                    pPropertyField->getFieldValue( dampingRatio );
                }
            }

            // Create joint.
            createWeldJoint( pSceneObjectA, pSceneObjectB, localAnchorA, localAnchorB, frequency, dampingRatio, collideConnected );
        }
        // Is this a wheel joint?
        else if ( aliasName == jointWheelTypeName )
        {
            SceneObject* pSceneObjectA = NULL;
            SceneObject* pSceneObjectB = NULL;
            b2Vec2 localAnchorA = b2Vec2_zero;
            b2Vec2 localAnchorB = b2Vec2_zero;
            bool collideConnected = false;

            bool enableMotor = false;
            F32 motorSpeed = b2_pi;
            F32 maxMotorTorque = 0.0f;

            F32 frequency = 0.0f;
            F32 dampingRatio = 0.0f;
            b2Vec2 worldAxis( 0.0f, 1.0f );

            // Iterate property fields.
            for ( TamlPropertyTypeAlias::const_iterator propertyFieldItr = pPropertyTypeAlias->begin(); propertyFieldItr != pPropertyTypeAlias->end(); ++propertyFieldItr )
            {
                // Fetch property field.
                TamlPropertyField* pPropertyField = *propertyFieldItr;

                // Fetch property field name.
                StringTableEntry fieldName = pPropertyField->getFieldName();

                // Fetch fields.
                if ( fieldName == jointObjectAName )
                {
                    pSceneObjectA = dynamic_cast<SceneObject*>( pPropertyField->getFieldObject() );
                }
                else if ( fieldName == jointObjectBName )
                {
                    pSceneObjectB = dynamic_cast<SceneObject*>( pPropertyField->getFieldObject() );
                }
                else if ( fieldName == jointLocalAnchorAName )
                {
                    pPropertyField->getFieldValue( localAnchorA );
                }
                else if ( fieldName == jointLocalAnchorBName )
                {
                    pPropertyField->getFieldValue( localAnchorB );
                }
                else if ( fieldName == jointCollideConnectedName )
                {
                    pPropertyField->getFieldValue( collideConnected );
                }
                else if ( fieldName == jointWheelMotorSpeedName )
                {
                    pPropertyField->getFieldValue( motorSpeed );
                    motorSpeed = mDegToRad( motorSpeed );
                    enableMotor = true;
                }
                else if ( fieldName == jointWheelMotorMaxTorqueName )
                {
                    pPropertyField->getFieldValue( maxMotorTorque );
                    enableMotor = true;
                }
                else if ( fieldName == jointWheelFrequencyName )
                {
                    pPropertyField->getFieldValue( frequency );
                }
                else if ( fieldName == jointWheelDampingRatioName )
                {
                    pPropertyField->getFieldValue( dampingRatio );
                }
                else if ( fieldName == jointWheelWorldAxisName )
                {
                    pPropertyField->getFieldValue( worldAxis );                    
                }
            }

            // Create joint.
            const U32 jointId = createWheelJoint( pSceneObjectA, pSceneObjectB, localAnchorA, localAnchorB, worldAxis, collideConnected );

            if ( enableMotor )
                setWheelJointMotor( jointId, true, motorSpeed, maxMotorTorque );

            setWheelJointFrequency( jointId, frequency );
            setWheelJointDampingRatio( jointId, dampingRatio );
        }
        // Is this a friction joint?
        else if ( aliasName == jointFrictionTypeName )
        {
            SceneObject* pSceneObjectA = NULL;
            SceneObject* pSceneObjectB = NULL;
            b2Vec2 localAnchorA = b2Vec2_zero;
            b2Vec2 localAnchorB = b2Vec2_zero;
            bool collideConnected = false;

            F32 maxForce = 0.0f;
            F32 maxTorque = 0.0f;

            // Iterate property fields.
            for ( TamlPropertyTypeAlias::const_iterator propertyFieldItr = pPropertyTypeAlias->begin(); propertyFieldItr != pPropertyTypeAlias->end(); ++propertyFieldItr )
            {
                // Fetch property field.
                TamlPropertyField* pPropertyField = *propertyFieldItr;

                // Fetch property field name.
                StringTableEntry fieldName = pPropertyField->getFieldName();

                // Fetch fields.
                if ( fieldName == jointObjectAName )
                {
                    pSceneObjectA = dynamic_cast<SceneObject*>( pPropertyField->getFieldObject() );
                }
                else if ( fieldName == jointObjectBName )
                {
                    pSceneObjectB = dynamic_cast<SceneObject*>( pPropertyField->getFieldObject() );
                }
                else if ( fieldName == jointLocalAnchorAName )
                {
                    pPropertyField->getFieldValue( localAnchorA );
                }
                else if ( fieldName == jointLocalAnchorBName )
                {
                    pPropertyField->getFieldValue( localAnchorB );
                }
                else if ( fieldName == jointCollideConnectedName )
                {
                    pPropertyField->getFieldValue( collideConnected );
                }
                else if ( fieldName == jointFrictionMaxForceName )
                {
                    pPropertyField->getFieldValue( maxForce );
                }
                else if ( fieldName == jointFrictionMaxTorqueName )
                {
                    pPropertyField->getFieldValue( maxTorque );
                }
            }

            // Create joint.
            createFrictionJoint( pSceneObjectA, pSceneObjectB, localAnchorA, localAnchorB, maxForce, maxTorque, collideConnected );
        }
        // Is this a prismatic joint?
        else if ( aliasName == jointPrismaticTypeName )
        {
            SceneObject* pSceneObjectA = NULL;
            SceneObject* pSceneObjectB = NULL;
            b2Vec2 localAnchorA = b2Vec2_zero;
            b2Vec2 localAnchorB = b2Vec2_zero;
            bool collideConnected = false;

            bool enableLimit;
            F32 lowerTransLimit = 0.0f;
            F32 upperTransLimit = 1.0f;

            bool enableMotor = false;
            F32 motorSpeed = b2_pi;
            F32 maxMotorForce = 0.0f;

            b2Vec2 worldAxis( 0.0f, 1.0f );

            // Iterate property fields.
            for ( TamlPropertyTypeAlias::const_iterator propertyFieldItr = pPropertyTypeAlias->begin(); propertyFieldItr != pPropertyTypeAlias->end(); ++propertyFieldItr )
            {
                // Fetch property field.
                TamlPropertyField* pPropertyField = *propertyFieldItr;

                // Fetch property field name.
                StringTableEntry fieldName = pPropertyField->getFieldName();

                // Fetch fields.
                if ( fieldName == jointObjectAName )
                {
                    pSceneObjectA = dynamic_cast<SceneObject*>( pPropertyField->getFieldObject() );
                }
                else if ( fieldName == jointObjectBName )
                {
                    pSceneObjectB = dynamic_cast<SceneObject*>( pPropertyField->getFieldObject() );
                }
                else if ( fieldName == jointLocalAnchorAName )
                {
                    pPropertyField->getFieldValue( localAnchorA );
                }
                else if ( fieldName == jointLocalAnchorBName )
                {
                    pPropertyField->getFieldValue( localAnchorB );
                }
                else if ( fieldName == jointCollideConnectedName )
                {
                    pPropertyField->getFieldValue( collideConnected );
                }
                else if ( fieldName == jointPrismaticLimitLowerTransName )
                {
                    pPropertyField->getFieldValue( lowerTransLimit );
                    enableLimit = true;
                }
                else if ( fieldName == jointPrismaticLimitUpperTransName )
                {
                    pPropertyField->getFieldValue( upperTransLimit );
                    enableLimit = true;
                }
                else if ( fieldName == jointPrismaticMotorSpeedName )
                {
                    pPropertyField->getFieldValue( motorSpeed );
                    motorSpeed = mDegToRad( motorSpeed );
                    enableMotor = true;
                }
                else if ( fieldName == jointPrismaticMotorMaxForceName )
                {
                    pPropertyField->getFieldValue( maxMotorForce );
                    enableMotor = true;
                }
                else if ( fieldName == jointPrismaticWorldAxisName )
                {
                    pPropertyField->getFieldValue( worldAxis );
                }
            }

            // Create joint.
            const U32 jointId = createPrismaticJoint( pSceneObjectA, pSceneObjectB, localAnchorA, localAnchorB, worldAxis, collideConnected );

            if ( enableLimit )
                setPrismaticJointLimit( jointId, true, lowerTransLimit, upperTransLimit );

            if ( enableMotor )
                setPrismaticJointMotor( jointId, true, motorSpeed, maxMotorForce );
        }
        // Is this a pulley joint?
        else if ( aliasName == jointPulleyTypeName )
        {
            SceneObject* pSceneObjectA = NULL;
            SceneObject* pSceneObjectB = NULL;
            b2Vec2 localAnchorA = b2Vec2_zero;
            b2Vec2 localAnchorB = b2Vec2_zero;
            bool collideConnected = false;

            F32 lengthA = -1.0f;
            F32 lengthB = -1.0f;
            F32 ratio = 0.5f;
            b2Vec2 worldGroundAnchorA = b2Vec2_zero;
            b2Vec2 worldGroundAnchorB = b2Vec2_zero;

            // Iterate property fields.
            for ( TamlPropertyTypeAlias::const_iterator propertyFieldItr = pPropertyTypeAlias->begin(); propertyFieldItr != pPropertyTypeAlias->end(); ++propertyFieldItr )
            {
                // Fetch property field.
                TamlPropertyField* pPropertyField = *propertyFieldItr;

                // Fetch property field name.
                StringTableEntry fieldName = pPropertyField->getFieldName();

                // Fetch fields.
                if ( fieldName == jointObjectAName )
                {
                    pSceneObjectA = dynamic_cast<SceneObject*>( pPropertyField->getFieldObject() );
                }
                else if ( fieldName == jointObjectBName )
                {
                    pSceneObjectB = dynamic_cast<SceneObject*>( pPropertyField->getFieldObject() );
                }
                else if ( fieldName == jointLocalAnchorAName )
                {
                    pPropertyField->getFieldValue( localAnchorA );
                }
                else if ( fieldName == jointLocalAnchorBName )
                {
                    pPropertyField->getFieldValue( localAnchorB );
                }
                else if ( fieldName == jointCollideConnectedName )
                {
                    pPropertyField->getFieldValue( collideConnected );
                }
                else if ( fieldName == jointPulleyLengthAName )
                {
                    pPropertyField->getFieldValue( lengthA );
                }
                else if ( fieldName == jointPulleyLengthBName )
                {
                    pPropertyField->getFieldValue( lengthB );
                }
                else if ( fieldName == jointPulleyRatioName )
                {
                    pPropertyField->getFieldValue( ratio );
                }
                else if ( fieldName == jointPulleyGroundAnchorAName )
                {
                    pPropertyField->getFieldValue( worldGroundAnchorA );
                }
                else if ( fieldName == jointPulleyGroundAnchorBName )
                {
                    pPropertyField->getFieldValue( worldGroundAnchorB );
                }
            }

            // Create joint.
            createPulleyJoint( pSceneObjectA, pSceneObjectB, localAnchorA, localAnchorB, worldGroundAnchorA, worldGroundAnchorB, ratio, lengthA, lengthB, collideConnected );

        }
        // Is this a target joint?
        else if ( aliasName == jointTargetTypeName )
        {
            SceneObject* pSceneObject = NULL;
            bool collideConnected = false;
            b2Vec2 worldTarget = b2Vec2_zero;
            F32 maxForce = 1.0f;
            F32 frequency = 5.0f;
            F32 dampingRatio = 0.7f;

            // Iterate property fields.
            for ( TamlPropertyTypeAlias::const_iterator propertyFieldItr = pPropertyTypeAlias->begin(); propertyFieldItr != pPropertyTypeAlias->end(); ++propertyFieldItr )
            {
                // Fetch property field.
                TamlPropertyField* pPropertyField = *propertyFieldItr;

                // Fetch property field name.
                StringTableEntry fieldName = pPropertyField->getFieldName();

                // Fetch fields.
                if ( fieldName == jointObjectAName )
                {
                    pSceneObject = dynamic_cast<SceneObject*>( pPropertyField->getFieldObject() );
                }
                else if ( fieldName == jointCollideConnectedName )
                {
                    pPropertyField->getFieldValue( collideConnected );
                }
                else if ( fieldName == jointTargetWorldTargetName )
                {
                    pPropertyField->getFieldValue( worldTarget );
                }
                else if ( fieldName == jointTargetMaxForceName )
                {
                    pPropertyField->getFieldValue( maxForce );
                }
                else if ( fieldName == jointTargetFrequencyName )
                {
                    pPropertyField->getFieldValue( frequency );
                }
                else if ( fieldName == jointTargetDampingRatioName )
                {
                    pPropertyField->getFieldValue( dampingRatio );
                }
            }

            // Create joint.
            createTargetJoint( pSceneObject, worldTarget, maxForce, frequency, dampingRatio, collideConnected );
        }
        // Is this a motor joint?
        else if ( aliasName == jointMotorTypeName )
        {
            SceneObject* pSceneObjectA = NULL;
            SceneObject* pSceneObjectB = NULL;
            bool collideConnected = false;

            b2Vec2 linearOffset = b2Vec2_zero;
            F32 angularOffset = 0.0f;
            F32 maxForce = 1.0f;
            F32 maxTorque = 1.0f;
            F32 correctionFactor = 0.3f;

            // Iterate property fields.
            for ( TamlPropertyTypeAlias::const_iterator propertyFieldItr = pPropertyTypeAlias->begin(); propertyFieldItr != pPropertyTypeAlias->end(); ++propertyFieldItr )
            {
                // Fetch property field.
                TamlPropertyField* pPropertyField = *propertyFieldItr;

                // Fetch property field name.
                StringTableEntry fieldName = pPropertyField->getFieldName();

                // Fetch fields.
                if ( fieldName == jointObjectAName )
                {
                    pSceneObjectA = dynamic_cast<SceneObject*>( pPropertyField->getFieldObject() );
                }
                else if ( fieldName == jointObjectBName )
                {
                    pSceneObjectB = dynamic_cast<SceneObject*>( pPropertyField->getFieldObject() );
                }
                else if ( fieldName == jointCollideConnectedName )
                {
                    pPropertyField->getFieldValue( collideConnected );
                }
                else if ( fieldName == jointMotorLinearOffsetName )
                {
                    pPropertyField->getFieldValue( linearOffset );
                }
                else if ( fieldName == jointMotorAngularOffsetName )
                {
                    pPropertyField->getFieldValue( angularOffset );
                    angularOffset = mDegToRad( angularOffset );
                }
                else if ( fieldName == jointMotorMaxForceName )
                {
                    pPropertyField->getFieldValue( maxForce );
                }
                else if ( fieldName == jointMotorMaxTorqueName )
                {
                    pPropertyField->getFieldValue( maxTorque );
                }
                else if ( fieldName == jointMotorCorrectionFactorName )
                {
                    pPropertyField->getFieldValue( correctionFactor );
                }
            }

            // Create joint.
            createMotorJoint( pSceneObjectA, pSceneObjectB, linearOffset, angularOffset, maxForce, maxTorque, correctionFactor, collideConnected );

        }
        // Unknown joint type!
        else
        {
            // Warn.
            Con::warnf( "Unknown joint type of '%s' encountered.", aliasName );

            // Sanity!
            AssertFatal( false, "Scene::onTamlCustomRead() - Unknown joint type detected." );

            continue;
        }
    }
}

//-----------------------------------------------------------------------------

void Scene::onTamlCustomWrite( TamlCollection& customCollection )
{
    // Call parent.
    Parent::onTamlCustomWrite( customCollection );

    // Fetch joint count.
    const U32 jointCount = getJointCount();

    // Finish if no joints.
    if ( jointCount == 0 )
        return;

    // Add joint property.
    TamlCollectionProperty* pJointProperty = customCollection.addCollectionProperty( jointCollectionName );

    // Iterate joints.
    for( typeJointHash::iterator jointItr = mJoints.begin(); jointItr != mJoints.end(); ++jointItr )
    {
        // Fetch base joint.
        b2Joint* pBaseJoint = jointItr->value;

        // Add joint type alias.
        // NOTE:    The name of the type-alias will get updated shortly.
        TamlPropertyTypeAlias* pJointTypeAlias = pJointProperty->addTypeAlias( StringTable->EmptyString );

        // Fetch common details.
        b2Body* pBodyA = pBaseJoint->GetBodyA();
        b2Body* pBodyB = pBaseJoint->GetBodyB();

        // Fetch physics proxies.
        PhysicsProxy* pPhysicsProxyA = static_cast<PhysicsProxy*>(pBodyA->GetUserData());
        PhysicsProxy* pPhysicsProxyB = static_cast<PhysicsProxy*>(pBodyB->GetUserData());

        // Fetch physics proxy type.
        PhysicsProxy::ePhysicsProxyType proxyTypeA = static_cast<PhysicsProxy*>(pBodyA->GetUserData())->getPhysicsProxyType();
        PhysicsProxy::ePhysicsProxyType proxyTypeB = static_cast<PhysicsProxy*>(pBodyB->GetUserData())->getPhysicsProxyType();

        // Fetch scene objects.
        SceneObject* pSceneObjectA = proxyTypeA == PhysicsProxy::PHYSIC_PROXY_SCENEOBJECT ? static_cast<SceneObject*>(pPhysicsProxyA) : NULL;
        SceneObject* pSceneObjectB = proxyTypeB == PhysicsProxy::PHYSIC_PROXY_SCENEOBJECT ? static_cast<SceneObject*>(pPhysicsProxyB) : NULL;

        // Populate joint appropriately.
        switch( pBaseJoint->GetType() )
        {
            case e_distanceJoint:
                {
                    // Set type alias name.
                    pJointTypeAlias->mAliasName = StringTable->insert( jointDistanceTypeName );

                    // Fetch joint.
                    const b2DistanceJoint* pJoint = dynamic_cast<const b2DistanceJoint*>( pBaseJoint );

                    // Sanity!
                    AssertFatal( pJoint != NULL, "Scene::onTamlCustomWrite() - Invalid distance joint type returned." );

                    // Add length.
                    pJointTypeAlias->addPropertyField( jointDistanceLengthName, pJoint->GetLength() );

                    // Add frequency.
                    if ( mNotZero( pJoint->GetFrequency() ) )
                        pJointTypeAlias->addPropertyField( jointDistanceFrequencyName, pJoint->GetFrequency() );

                    // Add damping ratio.
                    if ( mNotZero( pJoint->GetDampingRatio() ) )
                        pJointTypeAlias->addPropertyField( jointDistanceDampingRatioName, pJoint->GetDampingRatio() );

                    // Add local anchors.
                    if ( mNotZero( pJoint->GetLocalAnchorA().LengthSquared() ) )
                        pJointTypeAlias->addPropertyField( jointLocalAnchorAName, pJoint->GetLocalAnchorA() );
                    if ( mNotZero( pJoint->GetLocalAnchorB().LengthSquared() ) )
                        pJointTypeAlias->addPropertyField( jointLocalAnchorBName, pJoint->GetLocalAnchorB() );

                    // Add bodies.
                    if ( pSceneObjectA != NULL )
                        pJointTypeAlias->addPropertyField( jointObjectAName, pSceneObjectA );
                    if ( pSceneObjectB != NULL )
                        pJointTypeAlias->addPropertyField( jointObjectBName, pSceneObjectB );
                }
                break;

            case e_ropeJoint:
                {
                    // Set type alias name.
                    pJointTypeAlias->mAliasName = StringTable->insert( jointRopeTypeName );

                    // Fetch joint.
                    const b2RopeJoint* pJoint = dynamic_cast<const b2RopeJoint*>( pBaseJoint );

                    // Sanity!
                    AssertFatal( pJoint != NULL, "Scene::onTamlCustomWrite() - Invalid rope joint type returned." );

                    // Add max length.
                    if ( mNotZero( pJoint->GetMaxLength() ) )
                        pJointTypeAlias->addPropertyField( jointRopeMaxLengthName, pJoint->GetMaxLength() );

                    // Add local anchors.
                    if ( mNotZero( pJoint->GetLocalAnchorA().LengthSquared() ) )
                        pJointTypeAlias->addPropertyField( jointLocalAnchorAName, pJoint->GetLocalAnchorA() );
                    if ( mNotZero( pJoint->GetLocalAnchorB().LengthSquared() ) )
                        pJointTypeAlias->addPropertyField( jointLocalAnchorBName, pJoint->GetLocalAnchorB() );

                    // Add bodies.
                    if ( pSceneObjectA != NULL )
                        pJointTypeAlias->addPropertyField( jointObjectAName, pSceneObjectA );
                    if ( pSceneObjectB != NULL )
                        pJointTypeAlias->addPropertyField( jointObjectBName, pSceneObjectB );
                }
                break;

            case e_revoluteJoint:
                {
                    // Set type alias name.
                    pJointTypeAlias->mAliasName = StringTable->insert( jointRevoluteTypeName );

                    // Fetch joint.
                    const b2RevoluteJoint* pJoint = dynamic_cast<const b2RevoluteJoint*>( pBaseJoint );

                    // Sanity!
                    AssertFatal( pJoint != NULL, "Scene::onTamlCustomWrite() - Invalid revolute joint type returned." );

                    // Add limit.
                    if ( pJoint->IsLimitEnabled() )
                    {
                        // Add limits.
                        pJointTypeAlias->addPropertyField( jointRevoluteLimitLowerAngleName, mRadToDeg(pJoint->GetLowerLimit()) );
                        pJointTypeAlias->addPropertyField( jointRevoluteLimitUpperAngleName, mRadToDeg(pJoint->GetUpperLimit()) );
                    }

                    // Add motor.
                    if ( pJoint->IsMotorEnabled() )
                    {
                        // Add motor.
                        pJointTypeAlias->addPropertyField( jointRevoluteMotorSpeedName, mRadToDeg(pJoint->GetMotorSpeed()) );
                        pJointTypeAlias->addPropertyField( jointRevoluteMotorMaxTorqueName, pJoint->GetMaxMotorTorque() );
                    }

                    // Add local anchors.
                    if ( mNotZero( pJoint->GetLocalAnchorA().LengthSquared() ) )
                        pJointTypeAlias->addPropertyField( jointLocalAnchorAName, pJoint->GetLocalAnchorA() );
                    if ( mNotZero( pJoint->GetLocalAnchorB().LengthSquared() ) )
                        pJointTypeAlias->addPropertyField( jointLocalAnchorBName, pJoint->GetLocalAnchorB() );

                    // Add bodies.
                    if ( pSceneObjectA != NULL )
                        pJointTypeAlias->addPropertyField( jointObjectAName, pSceneObjectA );
                    if ( pSceneObjectB != NULL )
                        pJointTypeAlias->addPropertyField( jointObjectBName, pSceneObjectB );
                }
                break;

            case e_weldJoint:
                {
                    // Set type alias name.
                    pJointTypeAlias->mAliasName = StringTable->insert( jointWeldTypeName );

                    // Fetch joint.
                    const b2WeldJoint* pJoint = dynamic_cast<const b2WeldJoint*>( pBaseJoint );

                    // Sanity!
                    AssertFatal( pJoint != NULL, "Scene::onTamlCustomWrite() - Invalid weld joint type returned." );

                    // Add frequency.
                    if ( mNotZero( pJoint->GetFrequency() ) )
                        pJointTypeAlias->addPropertyField( jointWeldFrequencyName, pJoint->GetFrequency() );

                    // Add damping ratio.
                    if ( mNotZero( pJoint->GetDampingRatio() ) )
                        pJointTypeAlias->addPropertyField( jointWeldDampingRatioName, pJoint->GetDampingRatio() );

                    // Add local anchors.
                    if ( mNotZero( pJoint->GetLocalAnchorA().LengthSquared() ) )
                        pJointTypeAlias->addPropertyField( jointLocalAnchorAName, pJoint->GetLocalAnchorA() );
                    if ( mNotZero( pJoint->GetLocalAnchorB().LengthSquared() ) )
                        pJointTypeAlias->addPropertyField( jointLocalAnchorBName, pJoint->GetLocalAnchorB() );

                    // Add bodies.
                    if ( pSceneObjectA != NULL )
                        pJointTypeAlias->addPropertyField( jointObjectAName, pSceneObjectA );
                    if ( pSceneObjectB != NULL )
                        pJointTypeAlias->addPropertyField( jointObjectBName, pSceneObjectB );
                }
                break;

            case e_wheelJoint:
                {
                    // Set type alias name.
                    pJointTypeAlias->mAliasName = StringTable->insert( jointWheelTypeName );

                    // Fetch joint.
                    b2WheelJoint* pJoint = dynamic_cast<b2WheelJoint*>( pBaseJoint );

                    // Sanity!
                    AssertFatal( pJoint != NULL, "Scene::onTamlCustomWrite() - Invalid wheel joint type returned." );

                    // Add motor.
                    if ( pJoint->IsMotorEnabled() )
                    {
                        // Add motor.
                        pJointTypeAlias->addPropertyField( jointWheelMotorSpeedName, mRadToDeg(pJoint->GetMotorSpeed()) );
                        pJointTypeAlias->addPropertyField( jointWheelMotorMaxTorqueName, pJoint->GetMaxMotorTorque() );
                    }

                    // Add frequency.
                    if ( mNotZero( pJoint->GetSpringFrequencyHz() ) )
                        pJointTypeAlias->addPropertyField( jointWheelFrequencyName, pJoint->GetSpringFrequencyHz() );

                    // Add damping ratio.
                    if ( mNotZero( pJoint->GetSpringDampingRatio() ) )
                        pJointTypeAlias->addPropertyField( jointWheelDampingRatioName, pJoint->GetSpringDampingRatio() );

                    // Add world axis.
                    pJointTypeAlias->addPropertyField( jointWheelWorldAxisName, pJoint->GetBodyA()->GetWorldVector( pJoint->GetLocalAxisA() ) );

                    // Add local anchors.
                    pJointTypeAlias->addPropertyField( jointLocalAnchorAName, pJoint->GetLocalAnchorA() );
                    pJointTypeAlias->addPropertyField( jointLocalAnchorBName, pJoint->GetLocalAnchorB() );

                    // Add bodies.
                    if ( pSceneObjectA != NULL )
                        pJointTypeAlias->addPropertyField( jointObjectAName, pSceneObjectA );
                    if ( pSceneObjectB != NULL )
                        pJointTypeAlias->addPropertyField( jointObjectBName, pSceneObjectB );
                }
                break;

            case e_frictionJoint:
                {
                    // Set type alias name.
                    pJointTypeAlias->mAliasName = StringTable->insert( jointFrictionTypeName );

                    // Fetch joint.
                    const b2FrictionJoint* pJoint = dynamic_cast<const b2FrictionJoint*>( pBaseJoint );

                    // Add max force.
                    if ( mNotZero( pJoint->GetMaxForce() ) )
                        pJointTypeAlias->addPropertyField( jointFrictionMaxForceName, pJoint->GetMaxForce() );

                    // Add max torque.
                    if ( mNotZero( pJoint->GetMaxTorque() ) )
                        pJointTypeAlias->addPropertyField( jointFrictionMaxTorqueName, pJoint->GetMaxTorque() );

                    // Sanity!
                    AssertFatal( pJoint != NULL, "Scene::onTamlCustomWrite() - Invalid friction joint type returned." );

                    // Add local anchors.
                    if ( mNotZero( pJoint->GetLocalAnchorA().LengthSquared() ) )
                        pJointTypeAlias->addPropertyField( jointLocalAnchorAName, pJoint->GetLocalAnchorA() );
                    if ( mNotZero( pJoint->GetLocalAnchorB().LengthSquared() ) )
                        pJointTypeAlias->addPropertyField( jointLocalAnchorBName, pJoint->GetLocalAnchorB() );

                    // Add bodies.
                    if ( pSceneObjectA != NULL )
                        pJointTypeAlias->addPropertyField( jointObjectAName, pSceneObjectA );
                    if ( pSceneObjectB != NULL )
                        pJointTypeAlias->addPropertyField( jointObjectBName, pSceneObjectB );
                }
                break;

            case e_prismaticJoint:
                {
                    // Set type alias name.
                    pJointTypeAlias->mAliasName = StringTable->insert( jointPrismaticTypeName );

                    // Fetch joint.
                    b2PrismaticJoint* pJoint = dynamic_cast<b2PrismaticJoint*>( pBaseJoint );

                    // Sanity!
                    AssertFatal( pJoint != NULL, "Scene::onTamlCustomWrite() - Invalid prismatic joint type returned." );

                    // Add limit.
                    if ( pJoint->IsLimitEnabled() )
                    {
                        // Add limits.
                        pJointTypeAlias->addPropertyField( jointPrismaticLimitLowerTransName, pJoint->GetLowerLimit() );
                        pJointTypeAlias->addPropertyField( jointPrismaticLimitUpperTransName, pJoint->GetUpperLimit() );
                    }

                    // Add motor.
                    if ( pJoint->IsMotorEnabled() )
                    {
                        // Add motor.
                        pJointTypeAlias->addPropertyField( jointPrismaticMotorSpeedName, mRadToDeg(pJoint->GetMotorSpeed()) );
                        pJointTypeAlias->addPropertyField( jointPrismaticMotorMaxForceName, pJoint->GetMaxMotorForce() );
                    }

                    // Add world axis.
                    pJointTypeAlias->addPropertyField( jointPrismaticWorldAxisName, pJoint->GetBodyA()->GetWorldVector( pJoint->GetLocalAxisA() ) );

                    // Add local anchors.
                    pJointTypeAlias->addPropertyField( jointLocalAnchorAName, pJoint->GetLocalAnchorA() );
                    pJointTypeAlias->addPropertyField( jointLocalAnchorBName, pJoint->GetLocalAnchorB() );

                    // Add bodies.
                    if ( pSceneObjectA != NULL )
                        pJointTypeAlias->addPropertyField( jointObjectAName, pSceneObjectA );
                    if ( pSceneObjectB != NULL )
                        pJointTypeAlias->addPropertyField( jointObjectBName, pSceneObjectB );
                }
                break;

            case e_pulleyJoint:
                {
                    // Set type alias name.
                    pJointTypeAlias->mAliasName = StringTable->insert( jointPulleyTypeName );

                    // Fetch joint.
                    b2PulleyJoint* pJoint = dynamic_cast<b2PulleyJoint*>( pBaseJoint );

                    // Sanity!
                    AssertFatal( pJoint != NULL, "Scene::onTamlCustomWrite() - Invalid pulley joint type returned." );

                    // Add lengths.
                    pJointTypeAlias->addPropertyField( jointPulleyLengthAName, pJoint->GetLengthA() );
                    pJointTypeAlias->addPropertyField( jointPulleyLengthBName, pJoint->GetLengthB() );

                    // Add ratio,
                    pJointTypeAlias->addPropertyField( jointPulleyRatioName, pJoint->GetRatio() );

                    // Add ground anchors.
                    pJointTypeAlias->addPropertyField( jointPulleyGroundAnchorAName, pJoint->GetGroundAnchorA() );
                    pJointTypeAlias->addPropertyField( jointPulleyGroundAnchorBName, pJoint->GetGroundAnchorB() );

                    // Add local anchors.
                    pJointTypeAlias->addPropertyField( jointLocalAnchorAName, pJoint->GetBodyA()->GetLocalPoint( pJoint->GetAnchorA() ) );
                    pJointTypeAlias->addPropertyField( jointLocalAnchorBName, pJoint->GetBodyB()->GetLocalPoint( pJoint->GetAnchorB() ) );

                    // Add bodies.
                    if ( pSceneObjectA != NULL )
                        pJointTypeAlias->addPropertyField( jointObjectAName, pSceneObjectA );
                    if ( pSceneObjectB != NULL )
                        pJointTypeAlias->addPropertyField( jointObjectBName, pSceneObjectB );
                }
                break;

            case e_mouseJoint:
                {
                    // Set type alias name.
                    pJointTypeAlias->mAliasName = StringTable->insert( jointTargetTypeName );

                    // Fetch joint.
                    const b2MouseJoint* pJoint = dynamic_cast<const b2MouseJoint*>( pBaseJoint );

                    // Sanity!
                    AssertFatal( pJoint != NULL, "Scene::onTamlCustomWrite() - Invalid target joint type returned." );

                    // Add target.
                    pJointTypeAlias->addPropertyField( jointTargetWorldTargetName, pJoint->GetTarget() );

                    // Add max force.
                    pJointTypeAlias->addPropertyField( jointTargetMaxForceName, pJoint->GetMaxForce() );

                    // Add frequency
                    pJointTypeAlias->addPropertyField( jointTargetFrequencyName, pJoint->GetFrequency() );

                    // Add damping ratio.
                    pJointTypeAlias->addPropertyField( jointTargetDampingRatioName, pJoint->GetDampingRatio() );

                    // Add body.
                    // NOTE: This joint uses BODYB as the object, BODYA is the ground-body however for easy of use
                    // we'll refer to this as OBJECTA in the persisted format.
                    if ( pSceneObjectB != NULL )
                        pJointTypeAlias->addPropertyField( jointObjectAName, pSceneObjectB );
                }
                break;

            case e_motorJoint:
                {
                    // Set type alias name.
                    pJointTypeAlias->mAliasName = StringTable->insert( jointMotorTypeName );

                    // Fetch joint.
                    const b2MotorJoint* pJoint = dynamic_cast<const b2MotorJoint*>( pBaseJoint );

                    // Sanity!
                    AssertFatal( pJoint != NULL, "Scene::onTamlCustomWrite() - Invalid motor joint type returned." );

                    // Add linear offset.
                    if ( mNotZero( pJoint->GetLinearOffset().LengthSquared() ) )
                        pJointTypeAlias->addPropertyField( jointMotorLinearOffsetName, pJoint->GetLinearOffset() );

                    // Add angular offset.
                    if ( mNotZero( pJoint->GetAngularOffset() ) )
                        pJointTypeAlias->addPropertyField( jointMotorAngularOffsetName, mRadToDeg( pJoint->GetAngularOffset() ) );

                    // Add max force.
                    pJointTypeAlias->addPropertyField( jointMotorMaxForceName, pJoint->GetMaxForce() );

                    // Add max torque.
                    pJointTypeAlias->addPropertyField( jointMotorMaxTorqueName, pJoint->GetMaxTorque() );

                    // Add correction factor.
                    pJointTypeAlias->addPropertyField( jointMotorCorrectionFactorName, pJoint->GetCorrectionFactor() );
                }
                break;

        default:
            // Sanity!
            AssertFatal( false, "Scene::onTamlCustomWrite() - Unknown joint type detected." );
        }

        // Add collide connected flag.
        if ( pBaseJoint->GetCollideConnected() )
            pJointTypeAlias->addPropertyField( jointCollideConnectedName, pBaseJoint->GetCollideConnected() );
    }
}

//-----------------------------------------------------------------------------

void Scene::onTamlCustomRead( const TamlCollection& customCollection )
{
    // Call parent.
    Parent::onTamlCustomRead( customCollection );
}

//-----------------------------------------------------------------------------

U32 Scene::getGlobalSceneCount( void )
{
    return sSceneCount;
}

//-----------------------------------------------------------------------------

static EnumTable::Enums jointTypeLookup[] =
                {
                { e_distanceJoint,  "distance"  },
                { e_ropeJoint,      "rope"      },
                { e_revoluteJoint,  "revolute"  },
                { e_weldJoint,      "weld"      },
                { e_wheelJoint,     "wheel"     },
                { e_frictionJoint,  "friction"  },
                { e_prismaticJoint, "prismatic" },
                { e_pulleyJoint,    "pulley"    },
                { e_mouseJoint,     "target"    },
                { e_motorJoint,     "motor"     },
                };

EnumTable jointTypeTable(sizeof(jointTypeLookup) / sizeof(EnumTable::Enums), &jointTypeLookup[0]);

//-----------------------------------------------------------------------------

const char* getJointTypeDescription( b2JointType jointType )
{
    // Search for Mnemonic.
    for (U32 i = 0; i < (sizeof(jointTypeLookup) / sizeof(EnumTable::Enums)); i++)
    {
        if( jointTypeLookup[i].index == jointType )
            return jointTypeLookup[i].label;
    }

    // Error.
    return StringTable->EmptyString;
}

//-----------------------------------------------------------------------------

b2JointType getJointTypeEnum(const char* label)
{
    // Search for Mnemonic.
    for (U32 i = 0; i < (sizeof(jointTypeLookup) / sizeof(EnumTable::Enums)); i++)
    {
        if( dStricmp(jointTypeLookup[i].label, label) == 0)
            return (b2JointType)jointTypeLookup[i].index;
    }

    // Warn!
    Con::warnf("SceneObject::getJointTypeEnum() - Invalid joint of '%s'", label );

    // Bah!
    return e_unknownJoint;
}

//-----------------------------------------------------------------------------

static EnumTable::Enums pickModeLookup[] =
                {
                { Scene::PICK_ANY,          "ANY" },
                { Scene::PICK_SIZE,         "SIZE" },
                { Scene::PICK_COLLISION,    "COLLISION" },
                };

//-----------------------------------------------------------------------------

Scene::PickMode getPickMode(const char* label)
{
    // Search for Mnemonic.
    for(U32 i = 0; i < (sizeof(pickModeLookup) / sizeof(EnumTable::Enums)); i++)
        if( dStricmp(pickModeLookup[i].label, label) == 0)
            return((Scene::PickMode)pickModeLookup[i].index);

    // Error.
    return Scene::PICK_INVALID;
}

//-----------------------------------------------------------------------------

const char* getPickModeDescription( Scene::PickMode pickMode )
{
    // Search for Mnemonic.
    for (U32 i = 0; i < (sizeof(pickModeLookup) / sizeof(EnumTable::Enums)); i++)
    {
        if( pickModeLookup[i].index == pickMode )
            return pickModeLookup[i].label;
    }

    // Fatal!
    AssertFatal(false, "SceneObject::getPickModeDescription() - Invalid pick mode.");

    // Error.
    return StringTable->EmptyString;
}
