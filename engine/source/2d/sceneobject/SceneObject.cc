//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _SCENE_OBJECT_H_
#include "2d/sceneobject/SceneObject.h"
#endif

#ifndef _DGL_H_
#include "graphics/dgl.h"
#endif

#ifndef _COLOR_H_
#include "graphics/color.h"
#endif

#ifndef _BITSTREAM_H_
#include "io/bitStream.h"
#endif

#ifndef _MMATHFN_H_
#include "math/mMathFn.h"
#endif

#ifndef _CONSOLETYPES_H_
#include "console/consoleTypes.h"
#endif

#ifndef _SIMBASE_H_
#include "sim/simBase.h"
#endif

#ifndef _BEHAVIORTEMPLATE_H_
#include "component/behaviors/behaviorTemplate.h"
#endif

#ifndef _SCENE_OBJECT_TIMER_EVENT_H_
#include "2d/sceneobject/SceneObjectTimerEvent.h"
#endif

#ifndef _SCENE_OBJECT_MOVE_TO_EVENT_H_
#include "2d/sceneobject/SceneObjectMoveToEvent.h"
#endif

#ifndef _SCENE_OBJECT_ROTATE_TO_EVENT_H_
#include "2d/sceneobject/SceneObjectRotateToEvent.h"
#endif

#ifndef _RENDER_PROXY_H_
#include "2d/core/RenderProxy.h"
#endif

#ifndef _STRINGUNIT_H_
#include "string/stringUnit.h"
#endif

// Script bindings.
#include "SceneObject_ScriptBinding.h"

// Debug Profiling.
#ifdef TORQUE_ENABLE_PROFILER
#include "debug/profiler.h"
#endif

//-----------------------------------------------------------------------------

IMPLEMENT_CONOBJECT(SceneObject);

//-----------------------------------------------------------------------------

// Scene-Object counter.
static U32 sGlobalSceneObjectCount = 0;
static U32 sSceneObjectMasterSerialId = 0;

// Collision shape property names.
static bool collisionShapePropertiesInitialized = false;

static StringTableEntry shapeCollectionName;

static StringTableEntry shapeDensityName;
static StringTableEntry shapeFrictionName;
static StringTableEntry shapeRestitutionName;
static StringTableEntry shapeSensorName;

static StringTableEntry circleTypeName;
static StringTableEntry circleRadiusName;
static StringTableEntry circleOffsetName;

static StringTableEntry polygonTypeName;
static StringTableEntry polygonPointName;

static StringTableEntry chainTypeName;
static StringTableEntry chainPointName;
static StringTableEntry chainAdjacentStartName;
static StringTableEntry chainAdjacentEndName;

static StringTableEntry edgeTypeName;
static StringTableEntry edgeStartName;
static StringTableEntry edgeEndName;
static StringTableEntry edgeAdjacentStartName;
static StringTableEntry edgeAdjacentEndName;

//------------------------------------------------------------------------------

// Important: If these defaults are changed then modify the associated "write" field protected methods to ensure
// that the associated field is persisted if not the default.
SceneObject::SceneObject() :
    /// Scene.
    mpScene(NULL),
    mpTargetScene(NULL),

    /// Lifetime.
    mLifetime(0.0f),
    mLifetimeActive(false),

    /// Layers.
    mSceneLayer(0),
    mSceneLayerMask(BIT(mSceneLayer)),
    mSceneLayerDepth(0.0f),

    /// Scene groups.
    mSceneGroup(0),
    mSceneGroupMask(BIT(mSceneGroup)),

    /// Area.
    mWorldProxyId(-1),

    /// Position / Angle.
    mPreTickPosition( 0.0f, 0.0f ),
    mPreTickAngle( 0.0f ),
    mRenderPosition( 0.0f, 0.0f ),
    mRenderAngle( 0.0f ),
    mSpatialDirty( true ),

    /// Body.
    mpBody(NULL),
    mWorldQueryKey(0),

    /// Collision control.
    mCollisionLayerMask(MASK_ALL),
    mCollisionGroupMask(MASK_ALL),
    mCollisionSuppress(false),
    mGatherContacts(false),
    mpCurrentContacts(NULL),

    /// Render visibility.                                        
    mVisible(true),

    /// Render flipping.
    mFlipX(false),
    mFlipY(false),

    /// Render blending.
    mBlendMode(true),
    mSrcBlendFactor(GL_SRC_ALPHA),
    mDstBlendFactor(GL_ONE_MINUS_SRC_ALPHA),
    mBlendColor(ColorF(1.0f,1.0f,1.0f,1.0f)),
    mAlphaTest(-1.0f),

    /// Render sorting.
    mSortPoint(0.0f,0.0f),

    /// Input events.
    mUseInputEvents(false),

    /// Script callbacks.
    mUpdateCallback(false),
    mCollisionCallback(false),
    mSleepingCallback(false),

    /// Debug mode.
    mDebugMask(0X00000000),

    /// Camera mounting.
    mpAttachedCamera(NULL),

    /// GUI attachment.
    mAttachedGuiSizeControl(false),
    mpAttachedGui(NULL),
    mpAttachedGuiSceneWindow(NULL),

    /// Pathing.
    mAttachedToPath(NULL),

    /// Safe deletion.
    mBeingSafeDeleted(false),
    mSafeDeleteReady(true),

    /// Miscellaneous.
    mBatchIsolated(false),
    mEnableChild(false),
    mSerialiseKey(0),
    mpSceneObjectGroup(NULL),
    mEditorTickAllowed(true),
    mPickingAllowed(true),
    mAlwaysInScope(false),
    mPeriodicTimerID(0),
    mMoveToEventId(0),
    mRotateToEventId(0),
    mSerialId(0)

{
    // Field names.
    SCENEOBJECT_COLLISIONSHAPE_FIELDNAME      = StringTable->insert("CollisionShape");

    // Initialize collision shape field names.
    if ( !collisionShapePropertiesInitialized )
    {
        shapeCollectionName     = StringTable->insert( "CollisionShapes" );

        shapeDensityName        = StringTable->insert( "Density" );
        shapeFrictionName       = StringTable->insert( "Friction" );
        shapeRestitutionName    = StringTable->insert( "Restitution" );
        shapeSensorName         = StringTable->insert( "Sensor" );

        circleTypeName          = StringTable->insert( "Circle" );
        circleRadiusName        = StringTable->insert( "Radius" );
        circleOffsetName        = StringTable->insert( "Offset" );

        polygonTypeName         = StringTable->insert( "Polygon" );
        polygonPointName        = StringTable->insert( "Point" );

        chainTypeName           = StringTable->insert( "Chain" );
        chainPointName          = polygonPointName;
        chainAdjacentStartName  = StringTable->insert( "AdjacentStartPoint" );
        chainAdjacentEndName    = StringTable->insert( "AdjacentEndPoint" );

        edgeTypeName            = StringTable->insert( "Edge" );
        edgeStartName           = StringTable->insert( "Point0" );
        edgeEndName             = StringTable->insert( "Point1" );
        edgeAdjacentStartName   = chainAdjacentStartName;
        edgeAdjacentEndName     = chainAdjacentEndName;

        // Flag as initialized.
        collisionShapePropertiesInitialized = true;
    }

    // Set Vector Associations.
    VECTOR_SET_ASSOCIATION( mDestroyNotifyList );
    VECTOR_SET_ASSOCIATION( mCollisionFixtureDefs );
    VECTOR_SET_ASSOCIATION( mCollisionFixtures );

    // Assign scene-object index.
    mSerialId = ++sSceneObjectMasterSerialId;
    sGlobalSceneObjectCount++;

    // Initialize the body definition.
    // Important: If these defaults are changed then modify the associated "write" field protected methods to ensure
    // that the associated field is persisted if not the default.
    mBodyDefinition.userData        = static_cast<PhysicsProxy*>(this);
    mBodyDefinition.position.Set(0.0f, 0.0f);
    mBodyDefinition.angle           = 0.0f;
    mBodyDefinition.linearVelocity.Set(0.0f, 0.0f);
    mBodyDefinition.angularVelocity = 0.0f;
    mBodyDefinition.linearDamping   = 0.0f;
    mBodyDefinition.angularDamping  = 0.0f;
    mBodyDefinition.allowSleep      = true;
    mBodyDefinition.awake           = true;
    mBodyDefinition.fixedRotation   = false;
    mBodyDefinition.bullet          = false;
    mBodyDefinition.type            = b2_dynamicBody;
    mBodyDefinition.active          = true;
    mBodyDefinition.gravityScale    = 1.0f;

    // Initial the default fixture definition.
    // Important: If these defaults are changed then modify the associated "write" field protected methods to ensure
    // that the associated field is persisted if not the default.
    mDefaultFixture.userData = static_cast<PhysicsProxy*>(this);
    mDefaultFixture.density     = 1.0f;
    mDefaultFixture.friction    = 0.2f;
    mDefaultFixture.restitution = 0.0f;
    mDefaultFixture.isSensor    = false;
    mDefaultFixture.shape       = NULL;

    // Set last awake state.
    mLastAwakeState = !mBodyDefinition.allowSleep || mBodyDefinition.awake;

    // Turn-off auto-sizing.
    mAutoSizing = false;

    // Set size.
    setSize( Vector2::getOne() );
}

//-----------------------------------------------------------------------------

SceneObject::~SceneObject()
{
    // Are we in a Scene?
    if ( mpScene )
    {
        // Yes, so remove from Scene.
        mpScene->removeFromScene( this );
    }

    // Decrease scene-object count.
    --sGlobalSceneObjectCount;
}

//-----------------------------------------------------------------------------

void SceneObject::initPersistFields()
{
    // Call parent.
    Parent::initPersistFields();

    /// Lifetime.
    addProtectedField("Lifetime", TypeF32, Offset(mLifetime, SceneObject), &setLifetime, &defaultProtectedGetFn, &writeLifetime, "");

    /// Scene Layers.
    addProtectedField("SceneLayer", TypeS32, Offset(mSceneLayer, SceneObject), &setSceneLayer, &defaultProtectedGetFn, &writeSceneLayer, "");
    addProtectedField("SceneLayerDepth", TypeF32, Offset(mSceneLayerDepth, SceneObject), &setSceneLayerDepth, &defaultProtectedGetFn, &writeSceneLayerDepth, "" );
    
    // Scene Groups.
    addProtectedField("SceneGroup", TypeS32, Offset(mSceneGroup, SceneObject), &setSceneGroup, &defaultProtectedGetFn, &writeSceneGroup, "");

    /// Area.
    addProtectedField("Size", TypeVector2, Offset( mSize, SceneObject), &setSize, &defaultProtectedGetFn, &writeSize, "");

    /// Position / Angle.
    addProtectedField("Position", TypeVector2, NULL, &setPosition, &getPosition, &writePosition, "");
    addProtectedField("Angle", TypeF32, NULL, &setAngle, &getAngle, &writeAngle, "");
    addProtectedField("FixedAngle", TypeBool, NULL, &setFixedAngle, &getFixedAngle, &writeFixedAngle, "");

    /// Body.
    addProtectedField("BodyType", TypeEnum, NULL, &setBodyType, &getBodyType, &writeBodyType, 1, &bodyTypeTable, "" );
    addProtectedField("Active", TypeBool, NULL, &setActive, &getActive, &writeActive, "" );
    addProtectedField("Awake", TypeBool, NULL, &setAwake, &getAwake, &writeAwake, "" );
    addProtectedField("Bullet", TypeBool, NULL, &setBullet, &getBullet, &writeBullet, "" );
    addProtectedField("SleepingAllowed", TypeBool, NULL, &setSleepingAllowed, &getSleepingAllowed, &writeSleepingAllowed, "" );

    /// Collision control.
    addProtectedField("CollisionGroups", TypeS32, Offset(mCollisionGroupMask, SceneObject), &setCollisionGroups, &defaultProtectedGetFn, &writeCollisionGroups, "");
    addProtectedField("CollisionLayers", TypeS32, Offset(mCollisionLayerMask, SceneObject), &setCollisionLayers, &defaultProtectedGetFn, &writeCollisionLayers, "");
    addField("CollisionSuppress", TypeBool, Offset(mCollisionSuppress, SceneObject), &writeCollisionSuppress, "");
    addProtectedField("GatherContacts", TypeBool, NULL, &setGatherContacts, &defaultProtectedGetFn, &writeGatherContacts, "");
    addProtectedField("DefaultDensity", TypeF32, Offset( mDefaultFixture.density, SceneObject), &setDefaultDensity, &defaultProtectedGetFn, &writeDefaultDensity, "");
    addProtectedField("DefaultFriction", TypeF32, Offset( mDefaultFixture.friction, SceneObject), &setDefaultFriction, &defaultProtectedGetFn, &writeDefaultFriction, "");
    addProtectedField("DefaultRestitution", TypeF32, Offset( mDefaultFixture.restitution, SceneObject), &setDefaultRestitution, &defaultProtectedGetFn, &writeDefaultRestitution, "");

    /// Velocities.
    addProtectedField("LinearVelocity", TypeVector2, NULL, &setLinearVelocity, &getLinearVelocity, &writeLinearVelocity, "");
    addProtectedField("AngularVelocity", TypeF32, NULL, &setAngularVelocity, &getAngularVelocity, &writeAngularVelocity, "");
    addProtectedField("LinearDamping", TypeF32, NULL, &setLinearDamping, &getLinearDamping, &writeLinearDamping, "");
    addProtectedField("AngularDamping", TypeF32, NULL, &setAngularDamping, &getAngularDamping, &writeAngularDamping, "");

    /// Gravity scaling.
    addProtectedField("GravityScale", TypeF32, NULL, &setGravityScale, &getGravityScale, &writeGravityScale, "");

    /// Render visibility.
    addField("Visible", TypeBool, Offset(mVisible, SceneObject), &writeVisible, "");

    /// Render flipping.
    addField("FlipX", TypeBool, Offset(mFlipX, SceneObject), &writeFlipX, "");
    addField("FlipY", TypeBool, Offset(mFlipY, SceneObject), &writeFlipY, "");

    /// Render blending.
    addField("BlendMode", TypeBool, Offset(mBlendMode, SceneObject), &writeBlendMode, "");
    addField("SrcBlendFactor", TypeEnum, Offset(mSrcBlendFactor, SceneObject), &writeSrcBlendFactor, 1, &srcBlendFactorTable);
    addField("DstBlendFactor", TypeEnum, Offset(mDstBlendFactor, SceneObject), &writeDstBlendFactor, 1, &dstBlendFactorTable);
    addField("BlendColor", TypeColorF, Offset(mBlendColor, SceneObject), &writeBlendColor, "");
    addField("AlphaTest", TypeF32, Offset(mAlphaTest, SceneObject), &writeAlphaTest, "");

    /// Render sorting.
    addField("SortPoint", TypeVector2, Offset(mSortPoint, SceneObject), &writeSortPoint, "");

    /// Input events.
    addField("UseInputEvents", TypeBool, Offset(mUseInputEvents, SceneObject), &writeUseInputEvents, "");

    // Script callbacks.
    addField("UpdateCallback", TypeBool, Offset(mUpdateCallback, SceneObject), &writeUpdateCallback, "");
    addField("CollisionCallback", TypeBool, Offset(mCollisionCallback, SceneObject), &writeCollisionCallback, "");
    addField("SleepingCallback", TypeBool, Offset(mSleepingCallback, SceneObject), &writeSleepingCallback, "");

    /// Scene.
    addProtectedField("scene", TypeSimObjectPtr, Offset(mpScene, SceneObject), &setScene, &defaultProtectedGetFn, &writeScene, "");
}

//-----------------------------------------------------------------------------

bool SceneObject::onAdd()
{
    // Call Parent.
    if(!Parent::onAdd())
        return false;

    // Add to any target scene.
    if ( mpTargetScene )
    {
        // Add to the target scene.
        mpTargetScene->addToScene(this);

        mpTargetScene = NULL;
    }

    // Perform the callback.
    Con::executef(this, 1, "onAdd");
   
    // Return Okay.
    return true;
}

//-----------------------------------------------------------------------------

void SceneObject::onRemove()
{
    // Perform the callback.
    Con::executef(this, 1, "onRemove");

    // Detach Any GUI Control.
    detachGui();

    // Remove from Scene.
    if ( getScene() )
        getScene()->removeFromScene( this );

    // Call Parent.
    Parent::onRemove();
}

//-----------------------------------------------------------------------------

void SceneObject::onDestroyNotify( SceneObject* pSceneObject )
{
}

//-----------------------------------------------------------------------------

void SceneObject::OnRegisterScene( Scene* pScene )
{
    // Sanity!
    AssertFatal( mpScene == NULL, "Cannot register to a scene if already registered." );
    AssertFatal( mpBody == NULL, "Cannot create a physics body if one already exists." );

    // Initialize contact gathering.
    initializeContactGathering();

    // Set scene.
    mpScene = pScene;

    // Create the physics body.
    mpBody = pScene->getWorld()->CreateBody( &mBodyDefinition );

    // Set active status.
    if ( !isEnabled() ) mpBody->SetActive( false );

    // Create fixtures.
    for( typeCollisionFixtureDefVector::iterator itr = mCollisionFixtureDefs.begin(); itr != mCollisionFixtureDefs.end(); itr++ )
    {
        // Fetch fixture definition.
        b2FixtureDef* pFixtureDef = (*itr);

        // Create fixture.
        b2Fixture* pFixture = mpBody->CreateFixture( pFixtureDef );

        // Push fixture.
        mCollisionFixtures.push_back( pFixture );

        // Destroy fixture shape.
        delete pFixtureDef->shape;

        // Destroy fixture definition.
        delete pFixtureDef;          
    }
    // Clear offline fixture definitions.
    mCollisionFixtureDefs.clear();

    // Calculate current AABB.
    CoreMath::mCalculateAABB( mLocalSizeVertices, mpBody->GetTransform(), &mCurrentAABB );

    // Create world proxy Id.
    mWorldProxyId = pScene->getWorldQuery()->add( this );

    // Reset the spatials.
    resetTickSpatials();

    // Notify components.
    notifyComponentsAddToScene();
}

//-----------------------------------------------------------------------------

void SceneObject::OnUnregisterScene( Scene* pScene )
{
    // Sanity!
    AssertFatal( mpScene == pScene, "Cannot unregister from a scene that is not registered." );
    AssertFatal( mpBody != NULL, "Cannot unregister physics body as it does not exist." );

    // Notify components.
    notifyComponentsRemoveFromScene();

    // Copy fixtures to fixture definitions.
    for( typeCollisionFixtureVector::iterator itr = mCollisionFixtures.begin(); itr != mCollisionFixtures.end(); itr++ )
    {
        // Fetch fixture.
        b2Fixture* pFixture = (*itr);

        // Create fixture definition.
        b2FixtureDef* pFixtureDef = new b2FixtureDef();
        pFixtureDef->density      = pFixture->GetDensity();
        pFixtureDef->friction     = pFixture->GetFriction();
        pFixtureDef->restitution  = pFixture->GetRestitution();
        pFixtureDef->isSensor     = pFixture->IsSensor();        
        pFixtureDef->userData     = this;
        pFixtureDef->shape        = pFixture->GetShape()->Clone( pScene->getBlockAllocator() );

        // Push fixture definition.
        mCollisionFixtureDefs.push_back( pFixtureDef );
    }
    // Clear our fixture references.
    // The actual fixtures will get destroyed when the body is destroyed so no need to destroy them here.
    mCollisionFixtures.clear();

    // Transfer physics body configuration back to definition.
    mBodyDefinition.type            = getBodyType();
    mBodyDefinition.position        = getPosition();
    mBodyDefinition.angle           = getAngle();
    mBodyDefinition.linearVelocity  = getLinearVelocity();
    mBodyDefinition.angularVelocity = getAngularVelocity();
    mBodyDefinition.linearDamping   = getLinearDamping();
    mBodyDefinition.angularDamping  = getAngularDamping();
    mBodyDefinition.gravityScale    = getGravityScale();
    mBodyDefinition.allowSleep      = getSleepingAllowed();
    mBodyDefinition.awake           = getAwake();
    mBodyDefinition.fixedRotation   = getFixedAngle();
    mBodyDefinition.bullet          = getBullet();
    mBodyDefinition.active          = getActive();

    // Destroy current contacts.
    delete mpCurrentContacts;
    mpCurrentContacts = NULL;

    // Destroy the physics body.
    mpScene->getWorld()->DestroyBody( mpBody );
    mpBody = NULL;

    // Destroy world proxy Id.
    if ( mWorldProxyId != -1 )
    {

        mpScene->getWorldQuery()->remove( this );
        mWorldProxyId = -1;
    }

    // Reset scene.
    mpScene = NULL;
}

//-----------------------------------------------------------------------------

void SceneObject::resetTickSpatials( const bool resize )
{
    // Set coincident pre-tick, current & render.
    mPreTickPosition = mRenderPosition = getPosition();
    mPreTickAngle = mRenderAngle = getAngle();

    // Fetch body transform.
    b2Transform bodyXform = getTransform();

    // Calculate current AABB.
    CoreMath::mCalculateAABB( mLocalSizeVertices, bodyXform, &mCurrentAABB );

    // Set coincident AABBs.
    mPreTickAABB = mCurrentAABB;

    // Calculate render OOBB.
    CoreMath::mCalculateOOBB( mLocalSizeVertices, bodyXform, mRenderOOBB );

    // Update world proxy (if in scene).
    if ( mpScene )
    {
        // Fetch world query.
        WorldQuery* pWorldQuery = mpScene->getWorldQuery();

        // Are we resizing the object?

        // MM:  There is a problem with moving the proxy currently.  I cannot isolate why but it has
        //      manifested itself as a "flickering" bug in the "Release" branch.
        //      For now I'll just enforce recreation of the world proxy which is a fool-proof way
        //      of dealing with the issue.  I will investigate why the "MoveProxy" is not always
        //      working correctly.  It's possible that the displacement of "{0,0}" is triggering the
        //      problem although there is nothing I can find to suggest than such a displacement
        //      is invalid in any way even if it doesn't entirely make sense to move a proxy with
        //      zero displacement.

        if ( resize )
        {
            // Yes, so we need to recreate a proxy.
            pWorldQuery->remove( this );
            mWorldProxyId = pWorldQuery->add( this );

        }
        else
        {
            // No, so we can simply update the proxy.
            pWorldQuery->update( this, mCurrentAABB, b2Vec2( 0.0f, 0.0f ) );
        }
    }

    // Flag spatial changed.
    mSpatialDirty = true;
}

//-----------------------------------------------------------------------------

void SceneObject::preIntegrate( const F32 totalTime, const F32 elapsedTime, DebugStats* pDebugStats )
{
   // Finish if nothing is dirty.
    if ( !mSpatialDirty )
        return;

    // Reset spatial changed.
    mSpatialDirty = false;

    mPreTickPosition = mRenderPosition = getPosition();
    mPreTickAngle    = mRenderAngle = getAngle();
    mPreTickAABB     = mCurrentAABB;

    // Calculate render OOBB.
    CoreMath::mCalculateOOBB( mLocalSizeVertices,getTransform(), mRenderOOBB );
}

//-----------------------------------------------------------------------------

void SceneObject::integrateObject( const F32 totalTime, const F32 elapsedTime, DebugStats* pDebugStats )
{
    // Fetch position.
    const b2Vec2 position = getPosition();

    // Has the angle or position changed?
    if (    mPreTickAngle != getAngle() ||
            mPreTickPosition.x != position.x ||
            mPreTickPosition.y != position.y )
    {
        // Yes, so flag spatial dirty.
        mSpatialDirty = true;

        // Calculate current AABB.
        CoreMath::mCalculateAABB( mLocalSizeVertices, getTransform(), &mCurrentAABB );

        // Calculate tick AABB.
        b2AABB tickAABB;
        tickAABB.Combine( mPreTickAABB, mCurrentAABB );

        // Calculate tick displacement.
        b2Vec2 tickDisplacement = position - mPreTickPosition;
            
        // Update world proxy.
        mpScene->getWorldQuery()->update( this, tickAABB, tickDisplacement );
    }

    // Update Lifetime.
    if ( mLifetimeActive && !getScene()->getIsEditorScene() )
    {
        updateLifetime( elapsedTime );
    }

    // Update Any Attached GUI.
    if ( mpAttachedGui && mpAttachedGuiSceneWindow )
    {
        updateAttachedGui();
    }

    // Are we attached to a camera?
    if ( mpAttachedCamera )
    {
        // Yes, so calculate camera mount.
        mpAttachedCamera->calculateCameraMount( elapsedTime );
    }
}

//-----------------------------------------------------------------------------

void SceneObject::postIntegrate(const F32 totalTime, const F32 elapsedTime, DebugStats *pDebugStats)
{
   if( getScene() )
   {
#ifdef TORQUE_ENABLE_PROFILER
        PROFILE_START(PHYSIC_PROXY_SCENEOBJECT_onUpdate);
#endif

        // Notify components.
        notifyComponentsUpdate();

        // Script "onUpdate".
        if ( mUpdateCallback )
            Con::executef(this, 1, "onUpdate");

        // Are we using the sleeping callback?
        if ( mSleepingCallback )
        {
            // Yes, so fetch the current awake state.
            const bool currentAwakeState = getAwake();

            // Did we change awake state?
            if ( currentAwakeState != mLastAwakeState )
            {
                // Yes, so update last awake state.
                mLastAwakeState = currentAwakeState;

                // Perform the appropriate callback.
                if ( currentAwakeState )
                    Con::executef(this, 1, "onWake");
                else
                    Con::executef(this, 1, "onSleep");
            }
        }

#ifdef TORQUE_ENABLE_PROFILER
        PROFILE_END();   // PHYSIC_PROXY_SCENEOBJECT_onUpdate
#endif
   }
}

//-----------------------------------------------------------------------------

void SceneObject::interpolateObject( const F32 timeDelta )
{
    if ( mSpatialDirty )
    {
        if ( timeDelta < 1.0f )
        {
            // Calculate render position.
            b2Vec2 position    = getPosition();
            b2Vec2 renderDelta = position - mPreTickPosition;
            renderDelta       *= timeDelta;
            mRenderPosition    = position - renderDelta;
        }
        else
        {
            mRenderPosition = mPreTickPosition;
        }

        // NOTE: We should really be interpolating the rotation here as well.

        // Calculate render transform.
        b2Transform renderXF( mRenderPosition, b2Rot(getAngle()) );

        // Calculate render OOBB.
        CoreMath::mCalculateOOBB( mLocalSizeVertices, renderXF, mRenderOOBB );
    }

    // Update Any Attached GUI.
    if ( mpAttachedGui && mpAttachedGuiSceneWindow )
    {
        updateAttachedGui();
    }

    // Are we attached to a camera?
    if ( mpAttachedCamera )
    {
        // Yes, so interpolate camera mount.
        mpAttachedCamera->interpolateCameraMount( timeDelta );
    }
};

//-----------------------------------------------------------------------------

void SceneObject::sceneRenderFallback( const SceneRenderState* pSceneRenderState, const SceneRenderRequest* pSceneRenderRequest, BatchRender* pBatchRenderer )
{
    // Fetch the default "NoImageRenderProxy".
    RenderProxy* pNoImageRenderProxy = Sim::findObject<RenderProxy>( Con::getVariable( NO_IMAGE_RENDER_PROXY_NAME) );

    // Finish if no render proxy available.
    if ( pNoImageRenderProxy == NULL )
        return;

    // Fetch render AABB.
    const b2Vec2* pRenderOOBB = getRenderOOBB();

    // Render using render proxy.
    pNoImageRenderProxy->render(
        false, false,
        pRenderOOBB[0], pRenderOOBB[1], pRenderOOBB[2], pRenderOOBB[3],
        pBatchRenderer );
}

//-----------------------------------------------------------------------------

void SceneObject::sceneRenderOverlay( const SceneRenderState* sceneRenderState )
{
    // Get Scene.
    Scene* pScene = getScene();

    // Cannot do anything without scene!
    if ( !pScene )
        return;

    // Don't draw debug if not enabled.
    if ( !isEnabled() )
        return;

    // Get merged Local/Scene Debug Mask.
    U32 debugMask = getDebugMask() | pScene->getDebugMask();

    // Finish here if we're not rendering any debug info or only have scene-rendered debug options.
    if ( !debugMask || (debugMask & ~(Scene::SCENE_DEBUG_STATISTICS | Scene::SCENE_DEBUG_JOINTS)) == 0 )
        return;

    // Clear blending.
    glDisable( GL_BLEND );
    glDisable( GL_TEXTURE_2D );

    // AABB debug draw.
    if ( debugMask & Scene::SCENE_DEBUG_AABB )
    {
        pScene->mDebugDraw.DrawAABB( mCurrentAABB );
    }

    // OOBB debug draw.
    if ( debugMask & Scene::SCENE_DEBUG_OOBB )
    {
        pScene->mDebugDraw.DrawOOBB( mRenderOOBB );
    }

    // Asleep debug draw.
    if ( !getAwake() && debugMask & Scene::SCENE_DEBUG_ASLEEP )
    {
        pScene->mDebugDraw.DrawAsleep( mRenderOOBB );
    }

    // Collision Shapes.
    if ( debugMask & Scene::SCENE_DEBUG_COLLISION_SHAPES )
    {
        pScene->mDebugDraw.DrawCollisionShapes( getBody() );
    }

    // Position and local center of mass.
    if ( debugMask & Scene::SCENE_DEBUG_POSITION_AND_COM )
    {
        const b2Vec2 renderPosition = getRenderPosition();

        pScene->mDebugDraw.DrawPoint( renderPosition + getLocalCenter(), 6, b2Color( 0.0f, 1.0f, 0.4f ) );
        pScene->mDebugDraw.DrawPoint( renderPosition, 4, b2Color( 0.0f, 0.4f, 1.0f ) );
    }

    // Sort Points.
    if ( debugMask & Scene::SCENE_DEBUG_SORT_POINTS )
    {
        pScene->mDebugDraw.DrawSortPoint( getRenderPosition(), getSize(), mSortPoint );
    }
}

//-----------------------------------------------------------------------------

U32 SceneObject::packUpdate(NetConnection * conn, U32 mask, BitStream *stream)
{
    return 0;
}

//-----------------------------------------------------------------------------

void SceneObject::unpackUpdate(NetConnection * conn, BitStream *stream)
{
}

//-----------------------------------------------------------------------------

void SceneObject::setEnabled( const bool enabled )
{
    // Call parent.
    Parent::setEnabled( enabled );

    // If we have a scene, modify active.
    if ( mpScene )
    {
        mpBody->SetActive( enabled );
    }
}

//-----------------------------------------------------------------------------

bool SceneObject::synchronizePrefab( void )
{
    // Snapshot the current objects state that we don't want to change.
    const Vector2 position   = getPosition();
    const Vector2 size       = getSize();
    const F32 angle          = getAngle();

    // Call parent.
    if ( !Parent::synchronizePrefab() )
        return false;

    // Restore the state we don't want to change.
    setPosition( position );
    setAngle( angle );

    // Set size if not auto-sizing.
    if ( !getAutoSizing() )
        setSize( size );

    return true;
}

//-----------------------------------------------------------------------------

void SceneObject::setLifetime( const F32 lifetime )
{
// Debug Profiling.
#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_START(PHYSIC_PROXY_SCENEOBJECT_setLifetime);
#endif

    // Usage Flag.
    mLifetimeActive = mGreaterThanZero( lifetime );

    // Is life active?
    if ( mLifetimeActive )
    {
        // Yes, so set to incoming lifetime.
        mLifetime = lifetime;
    }
    else
    {
        // No, so reset it to be safe.
        mLifetime = 0.0f;
    }

// Debug Profiling.
#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_END();   // PHYSIC_PROXY_SCENEOBJECT_setLifetime
#endif
}

//-----------------------------------------------------------------------------

void SceneObject::updateLifetime( const F32 elapsedTime )
{
    // Update Lifetime.
    if ( mLifetimeActive )
    {
        // Reduce Lifetime.
        setLifetime( getLifetime() - elapsedTime );

        // Are we now dead?
        if ( mLessThanOrEqual( getLifetime(), 0.0f) )
        {
            // Yes, so reset lifetime.
            setLifetime( 0.0f );

            // Initiate Death!
            safeDelete();
        }
    }
}

//-----------------------------------------------------------------------------

void SceneObject::setSceneLayer( const U32 sceneLayer )
{
    // Check Layer.
    if ( sceneLayer > (MAX_LAYERS_SUPPORTED-1) )
    {
        Con::warnf("SceneObject::setSceneLayer() - Invalid scene layer '%d' (0-31).", sceneLayer);
        return;
    }

    // Set Layer.
    mSceneLayer = sceneLayer;

    // Set Layer Mask.
    mSceneLayerMask = BIT( mSceneLayer );
}

//-----------------------------------------------------------------------------

bool SceneObject::setSceneLayerDepthFront( void )
{
    // Fetch the scene.
    Scene* pScene = getScene();

    // Finish if no scene or only a single object.
    if ( pScene == NULL || pScene->getSceneObjectCount() == 1 )
        return false;

    // Fetch scene objects.
    typeSceneObjectVector layerList;
    pScene->getSceneObjects( layerList, getSceneLayer() );

    // Fetch layer object count.
    const U32 layerObjectCount = (U32)layerList.size();

    // Sort the layer.
    dQsort(layerList.address(), layerObjectCount, sizeof(SceneObject*), sceneObjectLayerDepthSort);

    // Reset object index.
    U32 objectIndex = 0;

    // Find object index.
    for ( ; objectIndex < layerObjectCount; ++objectIndex )
    {
        // Stop if this is the current object index.
        if ( layerList[objectIndex] == this )
            break;
    }

    // Finish if already at front of layer.
    if ( objectIndex == layerObjectCount-1 )
        return false;

    // Fetch furthest front depth.
    const F32 frontDepth = layerList.last()->getSceneLayerDepth();

    // Adjust depth to be in front.
    setSceneLayerDepth( frontDepth - 1.0f );

    return true;
}

//-----------------------------------------------------------------------------

bool SceneObject::setSceneLayerDepthBack( void )
{
    // Fetch the scene.
    Scene* pScene = getScene();

    // Finish if no scene or only a single object.
    if ( pScene == NULL || pScene->getSceneObjectCount() == 1 )
        return false;

    // Fetch scene objects.
    typeSceneObjectVector layerList;
    pScene->getSceneObjects( layerList, getSceneLayer() );

    // Fetch layer object count.
    const U32 layerObjectCount = (U32)layerList.size();

    // Sort the layer.
    dQsort(layerList.address(), layerObjectCount, sizeof(SceneObject*), sceneObjectLayerDepthSort);

    // Reset object index.
    U32 objectIndex = 0;

    // Find object index.
    for ( ; objectIndex < layerObjectCount; ++objectIndex )
    {
        // Stop if this is the current object index.
        if ( layerList[objectIndex] == this )
            break;
    }

    // Finish if already at back of layer.
    if ( objectIndex == 0 )
        return false;

    // Fetch furthest back depth.
    const F32 backDepth = layerList.first()->getSceneLayerDepth();

    // Adjust depth to be in back.
    setSceneLayerDepth( backDepth + 1.0f );

    return true;
}

//-----------------------------------------------------------------------------

bool SceneObject::setSceneLayerDepthForward( void )
{
    // Fetch the scene.
    Scene* pScene = getScene();

    // Finish if no scene or only a single object.
    if ( pScene == NULL || pScene->getSceneObjectCount() == 1 )
        return false;

    // Fetch scene objects.
    typeSceneObjectVector layerList;
    pScene->getSceneObjects( layerList, getSceneLayer() );

    // Fetch layer object count.
    const U32 layerObjectCount = (U32)layerList.size();

    // Sort the layer.
    dQsort(layerList.address(), layerObjectCount, sizeof(SceneObject*), sceneObjectLayerDepthSort);

    // Reset object index.
    U32 objectIndex = 0;

    // Find object index.
    for ( ; objectIndex < layerObjectCount; ++objectIndex )
    {
        // Stop if this is the current object index.
        if ( layerList[objectIndex] == this )
            break;
    }

    // Finish if already at the front of the layer.
    if ( objectIndex == layerObjectCount-1 )
        return false;

    // Fetch forwards depth.
    const F32 forwardDepth = layerList[objectIndex+1]->getSceneLayerDepth();

    // Adjust depth to be forward.
    setSceneLayerDepth( forwardDepth - 0.5f );

    // Finish if we were almost at the front anyway.
    if ( objectIndex == layerObjectCount-2 )
        return true;

    // Adjust next objects forwards.
    for( U32 forwardIndex = objectIndex+2; forwardIndex < layerObjectCount; ++forwardIndex )
    {
        // Fetch scene object.
        SceneObject* forwardSceneObject = layerList[forwardIndex];

        // Adjust depth to be forwards.
        forwardSceneObject->setSceneLayerDepth( forwardSceneObject->getSceneLayerDepth() - 1.0f );
    }

    return true;
}

//-----------------------------------------------------------------------------

bool SceneObject::setSceneLayerDepthBackward( void )
{
    // Fetch the scene.
    Scene* pScene = getScene();

    // Finish if no scene or only a single object.
    if ( pScene == NULL || pScene->getSceneObjectCount() == 1 )
        return false;

    // Fetch scene objects.
    typeSceneObjectVector layerList;
    pScene->getSceneObjects( layerList, getSceneLayer() );

    // Fetch layer object count.
    const U32 layerObjectCount = (U32)layerList.size();

    // Sort the layer.
    dQsort(layerList.address(), layerObjectCount, sizeof(SceneObject*), sceneObjectLayerDepthSort);

    // Reset object index.
    U32 objectIndex = 0;

    // Find object index.
    for ( ; objectIndex < layerObjectCount; ++objectIndex )
    {
        // Stop if this is the current object index.
        if ( layerList[objectIndex] == this )
            break;
    }

    // Finish if already at the back of the layer.
    if ( objectIndex == 0 )
        return false;

    // Fetch backwards depth.
    const F32 backDepth = layerList[objectIndex-1]->getSceneLayerDepth();

    // Adjust depth to be backwards.
    setSceneLayerDepth( backDepth + 0.5f );

    // Finish if we were almost at the back anyway.
    if ( objectIndex == 1 )
        return true;

    // Adjust previous objects backwards.
    for( U32 backIndex = 0; backIndex < (objectIndex-1); ++backIndex )
    {
        // Fetch scene object.
        SceneObject* backSceneObject = layerList[backIndex];

        // Adjust depth to be backwards.
        backSceneObject->setSceneLayerDepth( backSceneObject->getSceneLayerDepth() + 1.0f );
    }

    return true;
}

//-----------------------------------------------------------------------------

void SceneObject::setSceneGroup( const U32 sceneGroup )
{
    // Check Group.
    if ( sceneGroup > 31 )
    {
        Con::warnf("SceneObject::setSceneGroup() - Invalid scene group '%d' (0-31).", sceneGroup);
        return;
    }

    // Set Group.
    mSceneGroup = sceneGroup;

    // Set Group Mask.
    mSceneGroupMask = BIT( mSceneGroup );
}

//-----------------------------------------------------------------------------

void SceneObject::setArea( const Vector2& corner1, const Vector2& corner2 )
{
   // Calculate Normalized region.
   const Vector2 topLeft((corner1.x <= corner2.x) ? corner1.x : corner2.x, (corner1.y <= corner2.y) ? corner1.y : corner2.y);
   const Vector2 bottomRight((corner1.x > corner2.x) ? corner1.x : corner2.x, (corner1.y > corner2.y) ? corner1.y : corner2.y);

   // Calculate New Size.
   const Vector2 size = bottomRight - topLeft;

   // Calculate New Position.
   const Vector2 position = topLeft + (size * 0.5f);

   // Reset angle.
   setAngle( 0.0f );

   // Set Position/Size.
   setPosition( position );
   setSize( size );
}

//-----------------------------------------------------------------------------

void SceneObject::setSize( const Vector2& size )
{
    mSize = size;

    // Calculate half size.
    const F32 halfWidth = size.x * 0.5f;
    const F32 halfHeight = size.y * 0.5f;

    // Set local size vertices.
    mLocalSizeVertices[0].Set( -halfWidth, -halfHeight );
    mLocalSizeVertices[1].Set( +halfWidth, -halfHeight );
    mLocalSizeVertices[2].Set( +halfWidth, +halfHeight );
    mLocalSizeVertices[3].Set( -halfWidth, +halfHeight );

    if ( mpScene )
    {
        // Reset tick spatials.
        resetTickSpatials( true );
    }
}

//-----------------------------------------------------------------------------
  
void SceneObject::setPosition( const Vector2& position )
{
    if ( mpScene )
    {
        mpBody->SetTransform( position, mpBody->GetAngle() );

        // Reset tick spatials.
        resetTickSpatials();
    }
    else
    {
        mBodyDefinition.position = position;
    }
}

//-----------------------------------------------------------------------------

void SceneObject::setAngle( const F32 radians )
{
    if ( mpScene )
    {
        mpBody->SetTransform( mpBody->GetPosition(), radians );

        // Reset tick spatials.
        resetTickSpatials();
    }
    else
    {
        mBodyDefinition.angle = radians;
    }
}

//-----------------------------------------------------------------------------

Vector2 SceneObject::getLocalPoint( const Vector2 &worldPoint )
{
    if ( mpScene )
    {
        return mpBody->GetLocalPoint( worldPoint );
    }

    // Calculate body definition transform.
    const b2Transform transform( mBodyDefinition.position, b2Rot(mBodyDefinition.angle) );

    return b2MulT(transform, worldPoint);
}

//-----------------------------------------------------------------------------

Vector2 SceneObject::getWorldPoint( const Vector2 &localPoint )
{
    if ( mpScene )
    {
        return mpBody->GetWorldPoint( localPoint );
    }

    // Calculate body definition transform.
    const b2Transform transform( mBodyDefinition.position, b2Rot(mBodyDefinition.angle) );

    return b2Mul(transform, localPoint);
}

//-----------------------------------------------------------------------------

Vector2 SceneObject::getLocalVector( const Vector2& worldVector )
{
    if ( mpScene )
    {
        return mpBody->GetLocalVector( worldVector );
    }

    // Calculate body definition transform.
    const b2Transform transform( mBodyDefinition.position, b2Rot(mBodyDefinition.angle) );

    return b2MulT(transform.q, worldVector);
}

//-----------------------------------------------------------------------------

Vector2 SceneObject::getWorldVector( const Vector2& localVector )
{
    if ( mpScene )
    {
        return mpBody->GetWorldVector( localVector );
    }

    // Calculate body definition transform.
    const b2Transform transform( mBodyDefinition.position, b2Rot(mBodyDefinition.angle) );

    return b2Mul(transform.q, localVector);
}

//-----------------------------------------------------------------------------

bool SceneObject::getIsPointInOOBB( const Vector2& worldPoint )
{
    // Fetch local point.
    Vector2 localPoint = getLocalPoint( worldPoint );

    // Turn the local point into an absolute component distance.
    localPoint.absolute();

    const Vector2 halfSize = getHalfSize();

    // Check distance.
    return !( localPoint.x > halfSize.x || localPoint.y > halfSize.y );
}

//-----------------------------------------------------------------------------

bool SceneObject::getIsPointInCollisionShape( const U32 shapeIndex, const Vector2& worldPoint )
{
    // Sanity!
    AssertFatal( shapeIndex < getCollisionShapeCount(), "SceneObject::getIsPointInCollisionShape() - Invalid shape index." );

    // Sanity!
    if ( !mpScene )
    {
        Con::warnf( "SceneObject::getIsPointInCollisionShape() - Cannot test for point, object not in scene." );
        return false;
    }

    // Fetch fixture.
    b2Fixture* pFixture = mCollisionFixtures[shapeIndex];

    // Fetch local point.
    const Vector2 localPoint = getLocalPoint( worldPoint );

    // Test point.
    return pFixture->TestPoint( localPoint );
}

//-----------------------------------------------------------------------------

void SceneObject::setBodyType( const b2BodyType type )
{
    // Sanity!
    AssertFatal( type == b2_staticBody || type == b2_kinematicBody || type == b2_dynamicBody, "Invalid body type." );

    if ( mpScene )
    {
        mpBody->SetType( type );
        return;
    }
    else
    {
        mBodyDefinition.type = type;
    }
}


//-----------------------------------------------------------------------------

void SceneObject::applyForce( const Vector2& worldForce, const bool wake )
{
    // Ignore if not in scene.
    if ( !mpScene )
        return;

    applyForce( worldForce, getPosition() + getLocalCenter(), wake );
}

//-----------------------------------------------------------------------------

void SceneObject::applyForce( const Vector2& worldForce, const Vector2& worldPoint, const bool wake )
{
    // Ignore if not in scene.
    if ( !mpScene )
        return;

    getBody()->ApplyForce( worldForce, worldPoint, wake );
}

//-----------------------------------------------------------------------------

void SceneObject::applyTorque( const F32 torque, const bool wake )
{
    // Ignore if not in scene.
    if ( !mpScene )
        return;

    getBody()->ApplyTorque( torque, wake );
}

//-----------------------------------------------------------------------------

void SceneObject::applyLinearImpulse( const Vector2& worldImpulse, const bool wake )
{
    // Ignore if not in scene.
    if ( !mpScene )
        return;

    applyLinearImpulse( worldImpulse, getPosition() + getLocalCenter(), wake );
}

//-----------------------------------------------------------------------------

void SceneObject::applyLinearImpulse( const Vector2& worldImpulse, const Vector2& worldPoint, const bool wake )
{
    // Ignore if not in scene.
    if ( !mpScene )
        return;

    getBody()->ApplyLinearImpulse( worldImpulse, worldPoint, wake );
}

//-----------------------------------------------------------------------------

void SceneObject::applyAngularImpulse( const F32 impulse, const bool wake )
{
    // Ignore if not in scene.
    if ( !mpScene )
        return;

    getBody()->ApplyAngularImpulse( impulse, wake );
}

//-----------------------------------------------------------------------------

void SceneObject::setCollisionMasks( const U32 groupMask, const U32 layerMask )
{
    // Set Group/Layer Collision Masks.
    mCollisionGroupMask = groupMask;
    mCollisionLayerMask = layerMask;
}

//-----------------------------------------------------------------------------

void SceneObject::setCollisionAgainst( const SceneObject* pSceneObject, const bool clearMasks )
{
    // Do we need to clear existing masks?
    if ( clearMasks )
    {
        // Yes, so just set the masks to the referenced-objects' masks.
        setCollisionMasks( pSceneObject->getSceneGroupMask(), pSceneObject->getSceneLayerMask() );
    }
    else
    {
        // No, so merge with existing masks.
        setCollisionMasks( getCollisionGroupMask() | pSceneObject->getSceneGroupMask(), getCollisionLayerMask() | pSceneObject->getSceneLayerMask() );
    }
}

//-----------------------------------------------------------------------------

void SceneObject::setDefaultDensity( const F32 density, const bool updateShapes )
{
    mDefaultFixture.density = density;

    // Early-out if not updating shapes.
    if ( !updateShapes )
        return;

    if ( mpScene )
    {
        // Update live fixtures.
        for( U32 index = 0; index < (U32)mCollisionFixtures.size(); ++index )
        {
            mCollisionFixtures[index]->SetDensity( density );
        }

        // Update the body mass data.
        mpBody->ResetMassData();

        return;
    }

    // Update offline fixture definitions.
    for( U32 index = 0; index < (U32)mCollisionFixtureDefs.size(); ++index )
    {
        mCollisionFixtureDefs[index]->density = density;
    }
}

//-----------------------------------------------------------------------------

void SceneObject::setDefaultFriction( const F32 friction, const bool updateShapes )
{
    mDefaultFixture.friction = friction;

    // Early-out if not updating shapes.
    if ( !updateShapes )
        return;

    if ( mpScene )
    {
        // Update live fixtures.
        for( U32 index = 0; index < (U32)mCollisionFixtures.size(); ++index )
        {
            mCollisionFixtures[index]->SetFriction( friction );
        }
        return;
    }

    // Update offline fixture definitions.
    for( U32 index = 0; index < (U32)mCollisionFixtureDefs.size(); ++index )
    {
        mCollisionFixtureDefs[index]->friction = friction;
    }
}

//-----------------------------------------------------------------------------

void SceneObject::setDefaultRestitution( const F32 restitution, const bool updateShapes )
{
    mDefaultFixture.restitution = restitution;

    // Early-out if not updating shapes.
    if ( !updateShapes )
        return;

    if ( mpScene )
    {
        // Update live fixtures.
        for( U32 index = 0; index < (U32)mCollisionFixtures.size(); ++index )
        {
            mCollisionFixtures[index]->SetRestitution( restitution );
        }
        return;
    }

    // Update offline fixture definitions.
    for( U32 index = 0; index < (U32)mCollisionFixtureDefs.size(); ++index )
    {
        mCollisionFixtureDefs[index]->restitution = restitution;
    }
}

//-----------------------------------------------------------------------------

void SceneObject::initializeContactGathering( void )
{
    // Are we gathering contacts?
    if ( !mGatherContacts )
    {
        // No, so do we have any current contacts?
        if ( mpCurrentContacts )
        {
            // Yes, so destroy them.
            delete mpCurrentContacts;
            mpCurrentContacts = NULL;
        }
        return;
    }

    // Clear current contacts if already present.
    if ( mpCurrentContacts != NULL )
    {
        mpCurrentContacts->clear();
        return;
    }

    // Generate current contacts.
    mpCurrentContacts = new typeContactVector();
}

//-----------------------------------------------------------------------------

void SceneObject::onBeginCollision( const TickContact& tickContact )
{
    // Finish if we're not gathering contacts.
    if ( !mGatherContacts )
        return;

    // Sanity!
    AssertFatal( mpCurrentContacts != NULL, "SceneObject::onBeginCollision() - Contacts not initialized correctly." );
    AssertFatal( tickContact.mpSceneObjectA == this || tickContact.mpSceneObjectB == this, "SceneObject::onBeginCollision() - Contact does not involve this scene object." );

    // Keep contact.
    mpCurrentContacts->push_back( tickContact );
}

//-----------------------------------------------------------------------------

void SceneObject::onEndCollision( const TickContact& tickContact )
{
    // Finish if we're not gathering contacts.
    if ( !mGatherContacts )
        return;

    // Sanity!
    AssertFatal( mpCurrentContacts != NULL, "SceneObject::onBeginCollision() - Contacts not initialized correctly." );
    AssertFatal( tickContact.mpSceneObjectA == this || tickContact.mpSceneObjectB == this, "SceneObject::onEndCollision() - Contact does not involve this scene object." );

    // Remove contact.
    for( typeContactVector::iterator contactItr = mpCurrentContacts->begin(); contactItr != mpCurrentContacts->end(); ++contactItr )
    {
        // Is this is the correct contact?
        if ( contactItr->mpContact == tickContact.mpContact )
        {
            // Yes, so remove it.
            mpCurrentContacts->erase_fast( contactItr );
            return;
        }
    }
}

//-----------------------------------------------------------------------------

bool SceneObject::moveTo( const Vector2& targetWorldPoint, const U32 time, const bool autoStop )
{
    // Check in a scene.
    if ( !getScene() )
    {
        Con::warnf("Cannot move object (%d) to a point as it is not in a scene.", getId() );
        return false;
    }

    // Check not a static body.
    if ( getBodyType() == b2_staticBody )
    {
        Con::warnf("Cannot move object (%d) to a point as it is a static body.", getId() );
        return false;
    }

    // Cancel any previous event.
    if ( mMoveToEventId != 0 )
    {
        Sim::cancelEvent( mMoveToEventId );
        mMoveToEventId = 0;
    }

    // Calculate relative position.
    const Vector2 relativePosition = targetWorldPoint - getPosition();

    // Calculate linear velocity to use over time.
    const Vector2 linearVelocity = relativePosition / (time/1000.0f);

    // Set the linear velocity.
    setLinearVelocity( linearVelocity );

    // Create and post event.
    SceneObjectMoveToEvent* pEvent = new SceneObjectMoveToEvent( targetWorldPoint, autoStop );
    mMoveToEventId = Sim::postEvent(this, pEvent, Sim::getCurrentTime() + time );

    return true;
}

//-----------------------------------------------------------------------------

bool SceneObject::rotateTo( const F32 targetAngle, const U32 time, const bool autoStop )
{
    // Check in a scene.
    if ( !getScene() )
    {
        Con::warnf("Cannot rotate object (%d) to an angle as it is not in a scene.", getId() );
        return false;
    }

    // Check not a static body.
    if ( getBodyType() == b2_staticBody )
    {
        Con::warnf("Cannot move object (%d) to an angle as it is a static body.", getId() );
        return false;
    }

    // Cancel any previous event.
    if ( mRotateToEventId != 0 )
    {
        Sim::cancelEvent( mRotateToEventId );
        mRotateToEventId = 0;
    }

    // Calculate relative angle.
    const F32 relativeAngle = targetAngle - getAngle();

    // Calculate delta angle.
    const F32 deltaAngle = mAtan( mSin( relativeAngle ), mCos( relativeAngle ) );

    // Calculate angular velocity over time.
    const F32 angularVelocity = deltaAngle / (time/1000.0f);

    // Set angular velocity.
    setAngularVelocity( angularVelocity );

    // Create and post event.
    SceneObjectRotateToEvent* pEvent = new SceneObjectRotateToEvent( targetAngle, autoStop );
    mRotateToEventId = Sim::postEvent(this, pEvent, Sim::getCurrentTime() + time );

    return true;
}

//-----------------------------------------------------------------------------

void SceneObject::cancelMoveTo( const bool autoStop )
{
    // Only cancel an active moveTo event
    if ( mMoveToEventId != 0 )
    {
        Sim::cancelEvent( mMoveToEventId );
        mMoveToEventId = 0;

        // Should we auto stop?
        if ( autoStop )
        {
            // Yes, so remove linear velocity
            setLinearVelocity( Vector2::getZero() );
        }
    }
}

//-----------------------------------------------------------------------------

void SceneObject::cancelRotateTo( const bool autoStop )
{
    // Only cancel an active rotateTo event
    if ( mRotateToEventId != 0 )
    {
        Sim::cancelEvent( mRotateToEventId );
        mRotateToEventId = 0;

        // Should we auto stop?
        if ( autoStop )
        {
            // Yes, so remove angular velocity
            setAngularVelocity( 0.0f );
        }
    }
}

//-----------------------------------------------------------------------------

b2Shape::Type SceneObject::getCollisionShapeType( U32 shapeIndex ) const
{
    // Sanity!
    AssertFatal( shapeIndex < getCollisionShapeCount(), "SceneObject::getCollisionShapeType() - Invalid shape index." );

    if ( mpScene )
    {
        return mCollisionFixtures[shapeIndex]->GetType();
    }

    return mCollisionFixtureDefs[shapeIndex]->shape->GetType();
}

//-----------------------------------------------------------------------------

S32 SceneObject::getCollisionShapeIndex( const b2Fixture* pFixture ) const
{
    // Iterate collision shapes.
    S32 collisionShapeIndex = 0;
    for( typeCollisionFixtureVector::const_iterator collisionShapeItr = mCollisionFixtures.begin(); collisionShapeItr != mCollisionFixtures.end(); ++collisionShapeItr, ++collisionShapeIndex )
    {
        // Return index if this is the collision shape we are searching for.
        if ( pFixture == *collisionShapeItr )
            return collisionShapeIndex;
    }

    // Not found.
    return -1;
}

//-----------------------------------------------------------------------------

void SceneObject::setCollisionShapeDefinition( const U32 shapeIndex, const b2FixtureDef& fixtureDef )
{
    // We only set specific features of a fixture definition here.
    setCollisionShapeDensity( shapeIndex, fixtureDef.density );
    setCollisionShapeFriction( shapeIndex, fixtureDef.friction );
    setCollisionShapeRestitution( shapeIndex, fixtureDef.restitution );
    setCollisionShapeIsSensor( shapeIndex, fixtureDef.isSensor );
}

//-----------------------------------------------------------------------------

b2FixtureDef SceneObject::getCollisionShapeDefinition( const U32 shapeIndex ) const
{
    // Sanity!
    AssertFatal( shapeIndex < getCollisionShapeCount(), "SceneObject::getCollisionShapeDefinition() - Invalid shape index." );

    if ( mpScene )
    {       
        // Fetch fixture.
        const b2Fixture* pFixture = mCollisionFixtures[shapeIndex];

        b2FixtureDef fixtureDef;
        fixtureDef.density     = pFixture->GetDensity();
        fixtureDef.friction    = pFixture->GetFriction();
        fixtureDef.restitution = pFixture->GetRestitution();
        fixtureDef.isSensor    = pFixture->IsSensor();
        fixtureDef.filter      = pFixture->GetFilterData();
        fixtureDef.shape       = pFixture->GetShape();

        return fixtureDef;
    }

    return *mCollisionFixtureDefs[shapeIndex];
}

//-----------------------------------------------------------------------------

const b2CircleShape* SceneObject::getCollisionCircleShape( const U32 shapeIndex ) const
{
    // Sanity!
    AssertFatal( shapeIndex < getCollisionShapeCount(), "SceneObject::getCollisionCircleShape() - Invalid shape index." );

    // Fetch shape definition.
    const b2FixtureDef fixtureDef = getCollisionShapeDefinition( shapeIndex );

    // Sanity!
    AssertFatal( fixtureDef.shape->GetType() == b2Shape::e_circle, "SceneObject::getCollisionCircleShape() - Shape is not a circle shape." );

    // Fetch circle shape.
    const b2CircleShape* pShape = dynamic_cast<const b2CircleShape*>( fixtureDef.shape );

    // Sanity!
    AssertFatal( pShape != NULL, "SceneObject::getCollisionCircleShape() - Invalid circle shape." );

    return pShape;
}

//-----------------------------------------------------------------------------

const b2PolygonShape* SceneObject::getCollisionPolygonShape( const U32 shapeIndex ) const
{
    // Sanity!
    AssertFatal( shapeIndex < getCollisionShapeCount(), "SceneObject::getCollisionPolygonShape() - Invalid shape index." );

    // Fetch shape definition.
    const b2FixtureDef fixtureDef = getCollisionShapeDefinition( shapeIndex );

    // Sanity!
    AssertFatal( fixtureDef.shape->GetType() == b2Shape::e_polygon, "SceneObject::getCollisionPolygonShape() - Shape is not a polygon shape." );

    // Fetch shape.
    const b2PolygonShape* pShape = dynamic_cast<const b2PolygonShape*>( fixtureDef.shape );

    // Sanity!
    AssertFatal( pShape != NULL, "SceneObject::getCollisionPolygonShape() - Invalid polygon shape." );

    return pShape;
}

//-----------------------------------------------------------------------------

const b2ChainShape* SceneObject::getCollisionChainShape( const U32 shapeIndex ) const
{
    // Sanity!
    AssertFatal( shapeIndex < getCollisionShapeCount(), "SceneObject::getCollisionChainShape() - Invalid shape index." );

    // Fetch shape definition.
    const b2FixtureDef fixtureDef = getCollisionShapeDefinition( shapeIndex );

    // Sanity!
    AssertFatal( fixtureDef.shape->GetType() == b2Shape::e_chain, "SceneObject::getCollisionChainShape() - Shape is not a chain shape." );

    // Fetch shape.
    const b2ChainShape* pShape = dynamic_cast<const b2ChainShape*>( fixtureDef.shape );

    // Sanity!
    AssertFatal( pShape != NULL, "SceneObject::getCollisionChainShape() - Invalid chain shape." );

    return pShape;
}

//-----------------------------------------------------------------------------

const b2EdgeShape* SceneObject::getCollisionEdgeShape( const U32 shapeIndex ) const
{
    // Sanity!
    AssertFatal( shapeIndex < getCollisionShapeCount(), "SceneObject::getCollisionEdgeShape() - Invalid shape index." );

    // Fetch shape definition.
    const b2FixtureDef fixtureDef = getCollisionShapeDefinition( shapeIndex );

    // Sanity!
    AssertFatal( fixtureDef.shape->GetType() == b2Shape::e_edge, "SceneObject::getCollisionEdgeShape() - Shape is not a edge shape." );

    // Fetch shape.
    const b2EdgeShape* pShape = dynamic_cast<const b2EdgeShape*>( fixtureDef.shape );

    // Sanity!
    AssertFatal( pShape != NULL, "SceneObject::getCollisionEdgeShape() - Invalid edge shape." );

    return pShape;
}

//-----------------------------------------------------------------------------

void SceneObject::setCollisionShapeDensity( const U32 shapeIndex, const F32 density )
{
    // Sanity!
    AssertFatal( shapeIndex < getCollisionShapeCount(), "SceneObject::setCollisionShapeDensity() - Invalid shape index." );

    if ( mpScene )
    {
        // Update live fixture.
        mCollisionFixtures[shapeIndex]->SetDensity( density );

        // Update the body mass data.
        mpBody->ResetMassData();

        return;
    }

    // Update offline fixture definition.
    mCollisionFixtureDefs[shapeIndex]->density = density;
}

//-----------------------------------------------------------------------------

F32 SceneObject::getCollisionShapeDensity( const U32 shapeIndex ) const
{
    // Sanity!
    AssertFatal( shapeIndex < getCollisionShapeCount(), "SceneObject::getCollisionShapeDensity() - Invalid shape index." );

    if ( mpScene )
    {
        // Use live fixture.
        return mCollisionFixtures[shapeIndex]->GetDensity();
    }

    // Use offline fixture definition.
    return mCollisionFixtureDefs[shapeIndex]->density;
}

//-----------------------------------------------------------------------------

void SceneObject::setCollisionShapeFriction( const U32 shapeIndex, const F32 friction )
{
    // Sanity!
    AssertFatal( shapeIndex < getCollisionShapeCount(), "SceneObject::setCollisionShapeFriction() - Invalid shape index." );

    if ( mpScene )
    {
        // Update live fixture.
        mCollisionFixtures[shapeIndex]->SetFriction( friction );
        return;
    }

    // Update offline fixture definition.
    mCollisionFixtureDefs[shapeIndex]->friction = friction;
}

//-----------------------------------------------------------------------------

F32 SceneObject::getCollisionShapeFriction( const U32 shapeIndex ) const
{
    // Sanity!
    AssertFatal( shapeIndex < getCollisionShapeCount(), "SceneObject::getCollisionShapeFriction() - Invalid shape index." );

    if ( mpScene )
    {
        // Use live fixture.
        return mCollisionFixtures[shapeIndex]->GetFriction();
    }

    // Use offline fixture definition.
    return mCollisionFixtureDefs[shapeIndex]->friction;
}

//-----------------------------------------------------------------------------

void SceneObject::setCollisionShapeRestitution( const U32 shapeIndex, const F32 restitution )
{
    // Sanity!
    AssertFatal( shapeIndex < getCollisionShapeCount(), "SceneObject::setCollisionShapeRestitution() - Invalid shape index." );

    if ( mpScene )
    {
        // Update live fixture.
        mCollisionFixtures[shapeIndex]->SetRestitution( restitution );
        return;
    }

    // Update offline fixture definition.
    mCollisionFixtureDefs[shapeIndex]->restitution = restitution;
}

//-----------------------------------------------------------------------------

F32 SceneObject::getCollisionShapeRestitution( const U32 shapeIndex ) const
{
    // Sanity!
    AssertFatal( shapeIndex < getCollisionShapeCount(), "SceneObject::getCollisionShapeRestitution() - Invalid shape index." );

    if ( mpScene )
    {
        // Use live fixture.
        return mCollisionFixtures[shapeIndex]->GetRestitution();
    }

    // Use offline fixture definition.
    return mCollisionFixtureDefs[shapeIndex]->restitution;
}

//-----------------------------------------------------------------------------

void SceneObject::setCollisionShapeIsSensor( const U32 shapeIndex, const bool isSensor )
{
    // Sanity!
    AssertFatal( shapeIndex < getCollisionShapeCount(), "SceneObject::setCollisionShapeIsSensor() - Invalid shape index." );

    if ( mpScene )
    {
        // Update live fixture.
        mCollisionFixtures[shapeIndex]->SetSensor( isSensor );
        return;
    }

    // Update offline fixture definition.
    mCollisionFixtureDefs[shapeIndex]->isSensor = isSensor;
}

//-----------------------------------------------------------------------------

bool SceneObject::getCollisionShapeIsSensor( const U32 shapeIndex ) const
{
    // Sanity!
    AssertFatal( shapeIndex < getCollisionShapeCount(), "SceneObject::getCollisionShapeIsSensor() - Invalid shape index." );

    if ( mpScene )
    {
        // Use live fixture.
        return mCollisionFixtures[shapeIndex]->IsSensor();
    }

    // Use offline fixture definition.
    return mCollisionFixtureDefs[shapeIndex]->isSensor;
}

//-----------------------------------------------------------------------------

void SceneObject::deleteCollisionShape( const U32 shapeIndex )
{
    // Sanity!
    AssertFatal( shapeIndex < getCollisionShapeCount(), "SceneObject::deleteCollisionShape() - Invalid shape index." );

    if ( mpScene )
    {
        mpBody->DestroyFixture( mCollisionFixtures[ shapeIndex ] );
        mCollisionFixtures.erase_fast( shapeIndex );
        return;
    }

    mCollisionFixtureDefs.erase_fast( shapeIndex );
}

//-----------------------------------------------------------------------------

void SceneObject::clearCollisionShapes( void )
{
    // Delete all collision shapes.
    while ( getCollisionShapeCount() > 0 )
    {
        deleteCollisionShape( 0 );
    }
}

//-----------------------------------------------------------------------------

S32 SceneObject::createCircleCollisionShape( const F32 radius )
{
    return createCircleCollisionShape( radius, b2Vec2(0.0f, 0.0f) );
}

//-----------------------------------------------------------------------------

S32 SceneObject::createCircleCollisionShape( const F32 radius, const b2Vec2& localPosition )
{
    // Sanity!
    if ( radius <= 0.0f )
    {
        Con::errorf("SceneObject::createCircleCollisionShape() - Invalid radius of %g.", radius);
        return -1;
    }

    // Configure fixture definition.
    b2FixtureDef* pFixtureDef = new b2FixtureDef( mDefaultFixture );
    b2CircleShape* pShape = new b2CircleShape();
    pShape->m_p = localPosition;
    pShape->m_radius = radius;
    pFixtureDef->shape = pShape;

    if ( mpScene )
    {
        // Create and push fixture.
        mCollisionFixtures.push_back( mpBody->CreateFixture( pFixtureDef ) );

        // Destroy shape and fixture.
        delete pShape;
        delete pFixtureDef;

        return mCollisionFixtures.size()-1;
    }

    // Push fixture definition.
    mCollisionFixtureDefs.push_back( pFixtureDef );

    return mCollisionFixtureDefs.size()-1;
}

//-----------------------------------------------------------------------------

F32 SceneObject::getCircleCollisionShapeRadius( const U32 shapeIndex ) const
{
    // Fetch shape.
    const b2CircleShape* pShape = getCollisionCircleShape( shapeIndex );

    return pShape->m_radius;
}

//-----------------------------------------------------------------------------

Vector2 SceneObject::getCircleCollisionShapeLocalPosition( const U32 shapeIndex ) const
{
    // Fetch shape.
    const b2CircleShape* pShape = getCollisionCircleShape( shapeIndex );

    return pShape->m_p;
}

//-----------------------------------------------------------------------------

S32 SceneObject::createPolygonCollisionShape( const U32 pointCount, const b2Vec2* localPoints )
{
    // Sanity!
    if ( pointCount < 3 || pointCount > b2_maxPolygonVertices )
    {
        Con::errorf("SceneObject::createPolygonCollisionShape() - Invalid point count of %d.", pointCount);
        return -1;
    }

    // Configure fixture definition.
    b2FixtureDef* pFixtureDef = new b2FixtureDef( mDefaultFixture );
    b2PolygonShape* pShape    = new b2PolygonShape();
    pShape->Set( localPoints, pointCount );
    pFixtureDef->shape = pShape;

    if ( mpScene )
    {
        // Create and push fixture.
        mCollisionFixtures.push_back( mpBody->CreateFixture( pFixtureDef ) );

        // Destroy shape and fixture.
        delete pShape;
        delete pFixtureDef;

        return mCollisionFixtures.size()-1;
    }

    // Push fixture definition.
    mCollisionFixtureDefs.push_back( pFixtureDef );

    return mCollisionFixtureDefs.size()-1;
}

//-----------------------------------------------------------------------------

S32 SceneObject::createPolygonBoxCollisionShape( const F32 width, const F32 height )
{
    // Sanity!
    if ( width <= 0.0f )
    {
        Con::errorf("SceneObject::createPolygonBoxCollisionShape() - Invalid width of %g.", width);
        return -1;
    }
    if ( height <= 0.0f )
    {
        Con::errorf("SceneObject::createPolygonBoxCollisionShape() - Invalid height of %g.", height);
        return -1;
    }

    // Configure fixture definition.
    b2FixtureDef* pFixtureDef = new b2FixtureDef( mDefaultFixture );
    b2PolygonShape* pShape    = new b2PolygonShape();
    pShape->SetAsBox( width * 0.5f, height * 0.5f );
    pFixtureDef->shape = pShape;

    if ( mpScene )
    {
        // Create and push fixture.
        mCollisionFixtures.push_back( mpBody->CreateFixture( pFixtureDef ) );

        // Destroy shape and fixture.
        delete pShape;
        delete pFixtureDef;

        return mCollisionFixtures.size()-1;
    }

    // Push fixture definition.
    mCollisionFixtureDefs.push_back( pFixtureDef );

    return mCollisionFixtureDefs.size()-1;
}

//-----------------------------------------------------------------------------

S32 SceneObject::createPolygonBoxCollisionShape( const F32 width, const F32 height, const b2Vec2& localCentroid )
{
    // Sanity!
    if ( width <= 0.0f )
    {
        Con::errorf("SceneObject::createPolygonBoxCollisionShape() - Invalid width of %g.", width);
        return -1;
    }
    if ( height <= 0.0f )
    {
        Con::errorf("SceneObject::createPolygonBoxCollisionShape() - Invalid height of %g.", height);
        return -1;
    }

    // Configure fixture definition.
    b2FixtureDef* pFixtureDef = new b2FixtureDef( mDefaultFixture );
    b2PolygonShape* pShape    = new b2PolygonShape();
    pShape->SetAsBox( width * 0.5f, height * 0.5f, localCentroid, 0.0f );
    pFixtureDef->shape = pShape;

    if ( mpScene )
    {
        // Create and push fixture.
        mCollisionFixtures.push_back( mpBody->CreateFixture( pFixtureDef ) );

        // Destroy shape and fixture.
        delete pShape;
        delete pFixtureDef;

        return mCollisionFixtures.size()-1;
    }

    // Push fixture definition.
    mCollisionFixtureDefs.push_back( pFixtureDef );

    return mCollisionFixtureDefs.size()-1;
}

//-----------------------------------------------------------------------------

S32 SceneObject::createPolygonBoxCollisionShape( const F32 width, const F32 height, const b2Vec2& localCentroid, const F32 rotation )
{
    // Sanity!
    if ( width <= 0.0f )
    {
        Con::errorf("SceneObject::createPolygonBoxCollisionShape() - Invalid width of %g.", width);
        return -1;
    }
    if ( height <= 0.0f )
    {
        Con::errorf("SceneObject::createPolygonBoxCollisionShape() - Invalid height of %g.", height);
        return -1;
    }

    // Configure fixture definition.
    b2FixtureDef* pFixtureDef = new b2FixtureDef( mDefaultFixture );
    b2PolygonShape* pShape    = new b2PolygonShape();
    pShape->SetAsBox( width * 0.5f, height * 0.5f, localCentroid, rotation );
    pFixtureDef->shape = pShape;

    if ( mpScene )
    {
        // Create and push fixture.
        mCollisionFixtures.push_back( mpBody->CreateFixture( pFixtureDef ) );

        // Destroy shape and fixture.
        delete pShape;
        delete pFixtureDef;

        return mCollisionFixtures.size()-1;
    }

    // Push fixture definition.
    mCollisionFixtureDefs.push_back( pFixtureDef );

    return mCollisionFixtureDefs.size()-1;
}

//-----------------------------------------------------------------------------

U32 SceneObject::getPolygonCollisionShapePointCount( const U32 shapeIndex ) const
{
    // Fetch shape.
    const b2PolygonShape* pShape = getCollisionPolygonShape( shapeIndex );

    return pShape->GetVertexCount();
}

//-----------------------------------------------------------------------------

Vector2 SceneObject::getPolygonCollisionShapeLocalPoint( const U32 shapeIndex, const U32 pointIndex ) const
{
    // Fetch shape.
    const b2PolygonShape* pShape = getCollisionPolygonShape( shapeIndex );

    // Sanity!
    AssertFatal( pointIndex < (U32)pShape->GetVertexCount(), "SceneObject::getPolygonCollisionShapeLocalPoint() - Invalid local point index." );

    return pShape->GetVertex( pointIndex );
}

//-----------------------------------------------------------------------------

S32 SceneObject::createChainCollisionShape( const U32 pointCount, const b2Vec2* localPoints )
{
    // Sanity!
    if ( pointCount < 2 )
    {
        Con::errorf("SceneObject::createChainCollisionShape() - Invalid point count of %d.", pointCount);
        return -1;
    }

    // Configure fixture definition.
    b2FixtureDef* pFixtureDef = new b2FixtureDef( mDefaultFixture );
    b2ChainShape* pShape      = new b2ChainShape();
    pShape->CreateChain( localPoints, pointCount );
    pFixtureDef->shape = pShape;

    if ( mpScene )
    {
        // Create and push fixture.
        mCollisionFixtures.push_back( mpBody->CreateFixture( pFixtureDef ) );

        // Destroy shape and fixture.
        delete pShape;
        delete pFixtureDef;

        return mCollisionFixtures.size()-1;
    }

    // Push fixture definition.
    mCollisionFixtureDefs.push_back( pFixtureDef );

    return mCollisionFixtureDefs.size()-1;
}

//-----------------------------------------------------------------------------

S32 SceneObject::createChainCollisionShape(  const U32 pointCount, const b2Vec2* localPoints,
                                                const bool hasAdjacentLocalPositionStart, const bool hasAdjacentLocalPositionEnd,
                                                const b2Vec2& adjacentLocalPositionStart, const b2Vec2& adjacentLocalPositionEnd )
{
    // Sanity!
    if ( pointCount < 2 )
    {
        Con::errorf("SceneObject::createChainCollisionShape() - Invalid point count of %d.", pointCount);
        return -1;
    }

    // Configure fixture definition.
    b2FixtureDef* pFixtureDef = new b2FixtureDef( mDefaultFixture );
    b2ChainShape* pShape      = new b2ChainShape();
    pShape->CreateChain( localPoints, pointCount );
    
    if ( hasAdjacentLocalPositionStart )
        pShape->SetPrevVertex( adjacentLocalPositionStart );

    if ( hasAdjacentLocalPositionEnd )
        pShape->SetNextVertex( adjacentLocalPositionEnd );

    pFixtureDef->shape = pShape;

    if ( mpScene )
    {
        // Create and push fixture.
        mCollisionFixtures.push_back( mpBody->CreateFixture( pFixtureDef ) );

        // Destroy shape and fixture.
        delete pShape;
        delete pFixtureDef;

        return mCollisionFixtures.size()-1;
    }

    // Push fixture definition.
    mCollisionFixtureDefs.push_back( pFixtureDef );

    return mCollisionFixtureDefs.size()-1;
}

//-----------------------------------------------------------------------------

U32 SceneObject::getChainCollisionShapePointCount( const U32 shapeIndex ) const
{
    // Fetch shape.
    const b2ChainShape* pShape = getCollisionChainShape( shapeIndex );

    return pShape->m_count;
}

//-----------------------------------------------------------------------------

Vector2 SceneObject::getChainCollisionShapeLocalPoint( const U32 shapeIndex, const U32 pointIndex ) const
{
    // Fetch shape.
    const b2ChainShape* pShape = getCollisionChainShape( shapeIndex );

    // Sanity!
    AssertFatal( pointIndex < (U32)pShape->m_count, "SceneObject::getChainCollisionShapeLocalPoint() - Invalid local point index." );

    return pShape->m_vertices[ pointIndex ];
}

//-----------------------------------------------------------------------------

bool SceneObject::getChainCollisionShapeHasAdjacentStart( const U32 shapeIndex ) const
{
    // Fetch shape.
    const b2ChainShape* pShape = getCollisionChainShape( shapeIndex );

    return pShape->m_hasPrevVertex;
}

//-----------------------------------------------------------------------------

bool SceneObject::getChainCollisionShapeHasAdjacentEnd( const U32 shapeIndex ) const
{
    // Fetch shape.
    const b2ChainShape* pShape = getCollisionChainShape( shapeIndex );

    return pShape->m_hasNextVertex;
}

//-----------------------------------------------------------------------------

Vector2 SceneObject::getChainCollisionShapeAdjacentStart( const U32 shapeIndex ) const
{
    // Fetch shape.
    const b2ChainShape* pShape = getCollisionChainShape( shapeIndex );

    return pShape->m_prevVertex;
}

//-----------------------------------------------------------------------------

Vector2 SceneObject::getChainCollisionShapeAdjacentEnd( const U32 shapeIndex ) const
{
    // Fetch shape.
    const b2ChainShape* pShape = getCollisionChainShape( shapeIndex );

    return pShape->m_nextVertex;
}

//-----------------------------------------------------------------------------

S32 SceneObject::createEdgeCollisionShape( const b2Vec2& localPositionStart, const b2Vec2& localPositionEnd )
{
    // Configure fixture definition.
    b2FixtureDef* pFixtureDef = new b2FixtureDef( mDefaultFixture );
    b2EdgeShape* pShape       = new b2EdgeShape();
    pShape->Set( localPositionStart, localPositionEnd );
    pFixtureDef->shape = pShape;

    if ( mpScene )
    {
        // Create and push fixture.
        mCollisionFixtures.push_back( mpBody->CreateFixture( pFixtureDef ) );

        // Destroy shape and fixture.
        delete pShape;
        delete pFixtureDef;

        return mCollisionFixtures.size()-1;
    }

    // Push fixture definition.
    mCollisionFixtureDefs.push_back( pFixtureDef );

    return mCollisionFixtureDefs.size()-1;
}

//-----------------------------------------------------------------------------

S32 SceneObject::createEdgeCollisionShape(   const b2Vec2& localPositionStart, const b2Vec2& localPositionEnd,
                                                const bool hasAdjacentLocalPositionStart, const bool hasAdjacentLocalPositionEnd,
                                                const b2Vec2& adjacentLocalPositionStart, const b2Vec2& adjacentLocalPositionEnd )
{
    // Configure fixture definition.
    b2FixtureDef* pFixtureDef = new b2FixtureDef( mDefaultFixture );
    b2EdgeShape* pShape       = new b2EdgeShape();
    pShape->Set( localPositionStart, localPositionEnd );
    pShape->m_hasVertex0      = hasAdjacentLocalPositionStart;
    pShape->m_hasVertex3      = hasAdjacentLocalPositionEnd;
    pShape->m_vertex0         = adjacentLocalPositionStart;
    pShape->m_vertex3         = adjacentLocalPositionEnd;
    pFixtureDef->shape        = pShape;

    if ( mpScene )
    {
        // Create and push fixture.
        mCollisionFixtures.push_back( mpBody->CreateFixture( pFixtureDef ) );

        // Destroy shape and fixture.
        delete pShape;
        delete pFixtureDef;

        return mCollisionFixtures.size()-1;
    }

    // Push fixture definition.
    mCollisionFixtureDefs.push_back( pFixtureDef );

    return mCollisionFixtureDefs.size()-1;
}

//-----------------------------------------------------------------------------

Vector2 SceneObject::getEdgeCollisionShapeLocalPositionStart( const U32 shapeIndex ) const
{
    // Fetch shape.
    const b2EdgeShape* pShape = getCollisionEdgeShape( shapeIndex );

    return pShape->m_vertex1;
}

//-----------------------------------------------------------------------------

Vector2 SceneObject::getEdgeCollisionShapeLocalPositionEnd( const U32 shapeIndex ) const
{
    // Fetch shape.
    const b2EdgeShape* pShape = getCollisionEdgeShape( shapeIndex );

    return pShape->m_vertex2;
}

//-----------------------------------------------------------------------------

bool SceneObject::getEdgeCollisionShapeHasAdjacentStart( const U32 shapeIndex ) const
{
    // Fetch shape.
    const b2EdgeShape* pShape = getCollisionEdgeShape( shapeIndex );

    return pShape->m_hasVertex0;
}

//-----------------------------------------------------------------------------

bool SceneObject::getEdgeCollisionShapeHasAdjacentEnd( const U32 shapeIndex ) const
{
    // Fetch shape.
    const b2EdgeShape* pShape = getCollisionEdgeShape( shapeIndex );

    return pShape->m_hasVertex3;
}

//-----------------------------------------------------------------------------

Vector2 SceneObject::getEdgeCollisionShapeAdjacentStart( const U32 shapeIndex ) const
{
    // Fetch shape.
    const b2EdgeShape* pShape = getCollisionEdgeShape( shapeIndex );

    return pShape->m_vertex0;
}

//-----------------------------------------------------------------------------

Vector2 SceneObject::getEdgeCollisionShapeAdjacentEnd( const U32 shapeIndex ) const
{
    // Fetch shape.
    const b2EdgeShape* pShape = getCollisionEdgeShape( shapeIndex );

    return pShape->m_vertex3;
}

//-----------------------------------------------------------------------------

S32 SceneObject::formatCollisionShape( const U32 shapeIndex, char* pBuffer, U32 bufferSize ) const
{
    // Sanity!
    AssertFatal( shapeIndex < getCollisionShapeCount(), "SceneObject::formatCollisionShape() - Invalid shape index." );

    F32 density;
    F32 friction;
    F32 restitution;
    bool isSensor;

    if ( mpScene )
    {
        // Fetch fixture.
        b2Fixture* pFixture = mCollisionFixtures[shapeIndex];

        // Fetch common details.
        density     = pFixture->GetDensity();
        friction    = pFixture->GetFriction();
        restitution = pFixture->GetRestitution();
        isSensor    = pFixture->IsSensor();
    }
    else
    {
        // Fetch fixture def.
        b2FixtureDef* pFixtureDef = mCollisionFixtureDefs[shapeIndex];

        // Fetch common details.
        density     = pFixtureDef->density;
        friction    = pFixtureDef->friction;
        restitution = pFixtureDef->restitution;
        isSensor    = pFixtureDef->isSensor;
    }

    // Fetch shape type.
    const b2Shape::Type shapeType = getCollisionShapeType( shapeIndex );

    // Fetch shape type description.
    const char* pShapeTypeDescription = getCollisionShapeTypeDescription( shapeType );

    // Format prefix.
    S32 offset = dSprintf( pBuffer, bufferSize, "%s %g %g %g %d ", pShapeTypeDescription, density, friction, restitution, isSensor );
    pBuffer += offset;
    bufferSize -= offset;

    // Format appropriate shape.
    switch( shapeType )
    {
        case b2Shape::e_circle:
            offset += formatCircleCollisionShape( shapeIndex, pBuffer, bufferSize );
            break;

        case b2Shape::e_polygon:
            offset += formatPolygonCollisionShape( shapeIndex, pBuffer, bufferSize );
            break;

        case b2Shape::e_chain:
            offset += formatChainCollisionShape( shapeIndex, pBuffer, bufferSize );
            break;

        case b2Shape::e_edge:
            offset += formatEdgeCollisionShape( shapeIndex, pBuffer, bufferSize );
            break;

        default:
            AssertFatal( false, "SceneObject::formatCollisionShape() - Unsupported collision shape type encountered." );
    }

    return offset;
}

//-----------------------------------------------------------------------------

S32 SceneObject::formatCircleCollisionShape( const U32 shapeIndex, char* pBuffer, U32 bufferSize ) const
{
    // Sanity!
    AssertFatal( shapeIndex < getCollisionShapeCount(), "SceneObject::formatCircleCollisionShape() - Invalid shape index." );

    const b2CircleShape* pShape;

    if ( mpScene )
    {
        // Fetch fixture.
        b2Fixture* pFixture = mCollisionFixtures[shapeIndex];

        // Fetch shape.
        pShape = dynamic_cast<b2CircleShape*>( pFixture->GetShape() );
    }
    else
    {
        // Fetch fixture def.
        b2FixtureDef* pFixtureDef = mCollisionFixtureDefs[shapeIndex];

        // Fetch shape.
        pShape = dynamic_cast<b2CircleShape*>( const_cast<b2Shape*>(pFixtureDef->shape) );
    }

    // Check shape.
    if ( !pShape )
    {
        Con::errorf("SceneObject::formatCircleCollisionShape() - Invalid shape.");
        return 0;
    }

    // Fetch shape details.
    const F32 radius           = pShape->m_radius;
    const b2Vec2 localPosition = pShape->m_p;

    // Format output.
    return dSprintf( pBuffer, bufferSize, "%g %g %g ", radius, localPosition.x, localPosition.y );
}

//-----------------------------------------------------------------------------

S32 SceneObject::formatPolygonCollisionShape( const U32 shapeIndex, char* pBuffer, U32 bufferSize ) const
{
    // Sanity!
    AssertFatal( shapeIndex < getCollisionShapeCount(), "SceneObject::formatPolygonCollisionShape() - Invalid shape index." );

    const b2PolygonShape* pShape;

    if ( mpScene )
    {
        // Fetch fixture.
        b2Fixture* pFixture = mCollisionFixtures[shapeIndex];

        // Fetch shape.
        pShape = dynamic_cast<b2PolygonShape*>( pFixture->GetShape() );
    }
    else
    {
        // Fetch fixture def.
        b2FixtureDef* pFixtureDef = mCollisionFixtureDefs[shapeIndex];

        // Fetch shape.
        pShape = dynamic_cast<b2PolygonShape*>( const_cast<b2Shape*>(pFixtureDef->shape) );
    }

    // Check shape.
    if ( !pShape )
    {
        Con::errorf("SceneObject::formatPolygonCollisionShape() - Invalid shape.");
        return 0;
    }

    // Fetch point count.
    const U32 pointCount = pShape->GetVertexCount();

    // Format point count.
    S32 mainOffset = dSprintf( pBuffer, bufferSize, "%d ", pointCount );
    pBuffer       += mainOffset;
    bufferSize    -= mainOffset;

    // Format points.
    for ( S32 index = 0; index < (S32)pointCount; ++index )
    {
        // Fetch point.
        const b2Vec2& point = pShape->GetVertex( index );

        // Format point count.
        const S32 offset = dSprintf( pBuffer, bufferSize, "%g %g ", point.x, point.y );
        mainOffset      += offset;
        pBuffer         += offset;
        bufferSize      -= offset;
    }

    return mainOffset;
}

//-----------------------------------------------------------------------------

S32 SceneObject::formatChainCollisionShape( const U32 shapeIndex, char* pBuffer, U32 bufferSize ) const
{
    // Sanity!
    AssertFatal( shapeIndex < getCollisionShapeCount(), "SceneObject::formatChainCollisionShape() - Invalid shape index." );

    const b2ChainShape* pShape;

    if ( mpScene )
    {
        // Fetch fixture.
        b2Fixture* pFixture = mCollisionFixtures[shapeIndex];

        // Fetch shape.
        pShape = dynamic_cast<b2ChainShape*>( pFixture->GetShape() );
    }
    else
    {
        // Fetch fixture def.
        b2FixtureDef* pFixtureDef = mCollisionFixtureDefs[shapeIndex];

        // Fetch shape.
        pShape = dynamic_cast<b2ChainShape*>( const_cast<b2Shape*>(pFixtureDef->shape) );
    }

    // Check shape.
    if ( !pShape )
    {
        Con::errorf("SceneObject::formatChainCollisionShape() - Invalid shape.");
        return 0;
    }

    // Fetch point count.
    const U32 pointCount = pShape->m_count;

    // Format point count.
    S32 mainOffset = dSprintf( pBuffer, bufferSize, "%d ", pointCount );
    pBuffer       += mainOffset;
    bufferSize    -= mainOffset;

    // Format points.
    b2Vec2* pVertices = pShape->m_vertices;
    for ( U32 index = 0; index < pointCount; ++index )
    {
        // Fetch point.
        const b2Vec2* pVertex = pVertices + index;

        // Format point count.
        const S32 offset = dSprintf( pBuffer, bufferSize, "%g %g ", pVertex->x, pVertex->y );
        mainOffset      += offset;
        pBuffer         += offset;
        bufferSize      -= offset;
    }

    // Fetch adjacent positions.
    const bool hasAdjacentLocalPositionStart = pShape->m_hasPrevVertex;
    const bool hasAdjacentLocalPositionEnd   = pShape->m_hasNextVertex;
    const b2Vec2 adjacentLocalPositionStart  = pShape->m_prevVertex;
    const b2Vec2 adjacentLocalPositionEnd    = pShape->m_nextVertex;

    // Adjacent positions?
    if ( hasAdjacentLocalPositionStart || hasAdjacentLocalPositionEnd )
    {
        // Format with adjacent positions.
        const S32 offset = dSprintf(
            pBuffer, bufferSize,
            "%d %d %g %g %g %g ",
            hasAdjacentLocalPositionStart, hasAdjacentLocalPositionEnd,
            adjacentLocalPositionStart.x, adjacentLocalPositionStart.y,
            adjacentLocalPositionEnd.x, adjacentLocalPositionEnd.y );

        mainOffset += offset;
        pBuffer    += offset;
        bufferSize -= offset;
    }

    return mainOffset;
}

//-----------------------------------------------------------------------------

S32 SceneObject::formatEdgeCollisionShape( const U32 shapeIndex, char* pBuffer, U32 bufferSize ) const
{
    // Sanity!
    AssertFatal( shapeIndex < getCollisionShapeCount(), "SceneObject::formatEdgeCollisionShape() - Invalid shape index." );

    const b2EdgeShape* pShape;

    if ( mpScene )
    {
        // Fetch fixture.
        b2Fixture* pFixture = mCollisionFixtures[shapeIndex];

        // Fetch shape.
        pShape = dynamic_cast<b2EdgeShape*>( pFixture->GetShape() );
    }
    else
    {
        // Fetch fixture def.
        b2FixtureDef* pFixtureDef = mCollisionFixtureDefs[shapeIndex];

        // Fetch shape.
        pShape = dynamic_cast<b2EdgeShape*>( const_cast<b2Shape*>(pFixtureDef->shape) );
    }

    // Check shape.
    if ( !pShape )
    {
        Con::errorf("SceneObject::formatEdgeCollisionShape() - Invalid shape.");
        return 0;
    }

    // Fetch positions.
    const b2Vec2 localPosition1          = pShape->m_vertex1;
    const b2Vec2 localPosition2          = pShape->m_vertex2;
    const bool hasAdjacentLocalPosition1 = pShape->m_hasVertex0;
    const bool hasAdjacentLocalPosition2 = pShape->m_hasVertex3;
    const b2Vec2 adjacentLocalPosition1  = pShape->m_vertex0;
    const b2Vec2 adjacentLocalPosition2  = pShape->m_vertex3;

    // Adjacent positions?
    if ( hasAdjacentLocalPosition1 || hasAdjacentLocalPosition2 )
    {
        // Format with adjacent positions.
        return dSprintf(
            pBuffer, bufferSize,
            "%g %g %g %g %d %d %g %g %g %g ",
            localPosition1.x, localPosition1.y,
            localPosition2.x, localPosition2.y,
            hasAdjacentLocalPosition1, hasAdjacentLocalPosition2,
            adjacentLocalPosition1.x, adjacentLocalPosition1.y,
            adjacentLocalPosition2.x, adjacentLocalPosition2.y );
    }

    // Format without adjacent positions.
    return dSprintf(
        pBuffer, bufferSize,
        "%g %g %g %g ",
        localPosition1.x, localPosition1.y,
        localPosition2.x, localPosition2.y );
}

//-----------------------------------------------------------------------------

S32 SceneObject::parseCollisionShape( const char* pBuffer )
{
    // Fetch element count.
    const U32 elementCount =Utility::mGetStringElementCount( pBuffer );

    // Sanity!
    if ( elementCount < 6 )
    {
        Con::errorf("SceneObject::parseCollisionShape() - Found %d elements which is incorrect for parsing the shape.", elementCount );
        return -1;
    }

    // Fetch common arguments.
    b2FixtureDef fixtureDef;

    const b2Shape::Type shapeType = getCollisionShapeTypeEnum( Utility::mGetStringElement(pBuffer, 0) );
    fixtureDef.density            = dAtof( Utility::mGetStringElement(pBuffer, 1) );
    fixtureDef.friction           = dAtof( Utility::mGetStringElement(pBuffer, 2) );
    fixtureDef.restitution        = dAtof( Utility::mGetStringElement(pBuffer, 3) );
    fixtureDef.isSensor           = dAtob( Utility::mGetStringElement(pBuffer, 4) );

    // Fetch position of next argument.
    pBuffer = Utility::mGetStringElement(pBuffer, 5, false );

    // Parse appropriate shape.
    switch( shapeType )
    {
        case b2Shape::e_circle:
            return parseCircleCollisionShape( pBuffer, fixtureDef );

        case b2Shape::e_polygon:
            return parsePolygonCollisionShape( pBuffer, fixtureDef );

        case b2Shape::e_chain:
            return parseChainCollisionShape( pBuffer, fixtureDef );

        case b2Shape::e_edge:
            return parseEdgeCollisionShape( pBuffer, fixtureDef );

        default:
            Con::errorf("SceneObject::parseCollisionShape() - Unsupported collision shape type encountered." );
    }

    return -1;
}

//-----------------------------------------------------------------------------

S32 SceneObject::parseCircleCollisionShape( const char *pBuffer, b2FixtureDef& fixtureDef )
{
    // Fetch element count.
    const U32 elementCount =Utility::mGetStringElementCount( pBuffer );

    // Sanity!
    if ( elementCount != 3 )
    {
        Con::errorf("SceneObject::parseCircleCollisionShape() - Found %d elements which is incorrect for parsing the shape.", elementCount );
        return -1;
    }

    // Parse arguments.
    const F32 radius           = dAtof(Utility::mGetStringElement(pBuffer, 0));
    const b2Vec2 localPosition = Utility::mGetStringElementVector(pBuffer, 1);

    // Create shape.
    const S32 shapeIndex = createCircleCollisionShape( radius, localPosition );

    // Was shape created.
    if ( shapeIndex != -1 )
    {
        // Yes, so configure shape.
        setCollisionShapeDefinition( shapeIndex, fixtureDef );
    }

    return shapeIndex;
}

//-----------------------------------------------------------------------------

S32 SceneObject::parsePolygonCollisionShape( const char *pBuffer, b2FixtureDef& fixtureDef )
{
    // Fetch element count.
    const U32 elementCount =Utility::mGetStringElementCount( pBuffer );

    // Sanity!
    // NOTE:-   Must contain at least the vertex count plus three vertices.
    if ( elementCount < (1 + (3 * 2)) )
    {
        Con::errorf("SceneObject::parsePolygonCollisionShape() - Found %d elements which is incorrect for parsing the shape.", elementCount );
        return -1;
    }

    // Parse arguments.
    U32 pointCount = dAtoi(Utility::mGetStringElement(pBuffer, 0));

    // Clamp vertex count.
    if ( pointCount > b2_maxPolygonVertices )
    {
        Con::warnf("Polygon vertex count of %d exceeds the maximum vertex count of %d.  Ignoring excess vertices.", pointCount, b2_maxPolygonVertices);
        pointCount = b2_maxPolygonVertices;
    }

    // Sanity!
    // NOTE:-   Must contain the vertex count plus all the specified vertex pairs.
    if ( elementCount != (1 + (pointCount * 2)) )
    {
        Con::errorf("SceneObject::parsePolygonCollisionShape() - Found %d elements which is incorrect for parsing the shape.", elementCount );
        return -1;
    }

    // Fetch position of vertex pairs.
    pBuffer = Utility::mGetStringElement(pBuffer, 1, false);

    b2Vec2 localPoints[b2_maxPolygonVertices];
    for ( U32 vertexIndex = 0, elementIndex = 0; vertexIndex < pointCount; ++vertexIndex, elementIndex += 2 )
    {
        localPoints[vertexIndex] = Utility::mGetStringElementVector(pBuffer, elementIndex);
    }

    // Create shape.
    const S32 shapeIndex = createPolygonCollisionShape( pointCount, localPoints );

    // Was shape created.
    if ( shapeIndex != -1 )
    {
        // Yes, so configure shape.
        setCollisionShapeDefinition( shapeIndex, fixtureDef );
    }

    return shapeIndex;
}

//-----------------------------------------------------------------------------

S32 SceneObject::parseChainCollisionShape( const char *pBuffer, b2FixtureDef& fixtureDef )
{
    // Fetch element count.
    const U32 elementCount =Utility::mGetStringElementCount( pBuffer );

    // Sanity!
    // NOTE:-   Must contain at least the vertex count plus three vertices.
    if ( elementCount < (1 + (3 * 2)) )
    {
        Con::errorf("SceneObject::parseChainCollisionShape() - Found %d elements which is incorrect for parsing the shape.", elementCount );
        return -1;
    }

    // Parse arguments.
    U32 pointCount = dAtoi(Utility::mGetStringElement(pBuffer, 0));

    // Clamp vertex count.
    if ( pointCount > b2_maxPolygonVertices )
    {
        Con::warnf("Polygon vertex count of %d exceeds the maximum vertex count of %d.  Ignoring excess vertices.", pointCount, b2_maxPolygonVertices);
        pointCount = b2_maxPolygonVertices;
    }

    // Sanity!
    // NOTE:-   Must contain the vertex count plus all the specified vertex pairs.
    if ( elementCount != (1 + (pointCount * 2)) )
    {
        Con::errorf("SceneObject::parseChainCollisionShape() - Found %d elements which is incorrect for parsing the shape.", elementCount );
        return -1;
    }

    b2Vec2 localPoints[b2_maxPolygonVertices];
    for ( U32 vertexIndex = 0, elementIndex = 1; vertexIndex < pointCount; ++vertexIndex, elementIndex += 2 )
    {
        localPoints[vertexIndex] = Utility::mGetStringElementVector(pBuffer, elementIndex);
    }

    bool hasAdjacentLocalPositionStart = false;
    bool hasAdjacentLocalPositionEnd   = false;
    b2Vec2 adjacentLocalPositionStart( 0.0f, 0.0f );
    b2Vec2 adjacentLocalPositionEnd( 0.0f, 0.0f );

    // Adjacent arguments?    
    if ( elementCount > (1 + (pointCount * 2)) )
    {
        // Yes, so sanity!
        if ( elementCount != ((1 + (pointCount * 2)) + 6 ) )
        {
            Con::errorf("SceneObject::parseChainCollisionShape() - Found %d elements which is incorrect for parsing the shape.", elementCount );
            return -1;
        }

        // Fetch position of adjacent arguments.
        pBuffer = Utility::mGetStringElement(pBuffer, 1 + (pointCount * 2), false );

        hasAdjacentLocalPositionStart = dAtob(Utility::mGetStringElement( pBuffer, 0 ));
        hasAdjacentLocalPositionEnd   = dAtob(Utility::mGetStringElement( pBuffer, 1 ));
        adjacentLocalPositionStart    = Utility::mGetStringElementVector( pBuffer, 2 );
        adjacentLocalPositionEnd      = Utility::mGetStringElementVector( pBuffer, 4 );
    }

    // Create shape.
    const S32 shapeIndex = createChainCollisionShape(
        pointCount, localPoints,
        hasAdjacentLocalPositionStart, hasAdjacentLocalPositionEnd,
        adjacentLocalPositionStart, adjacentLocalPositionEnd);

    // Was shape created.
    if ( shapeIndex != -1 )
    {
        // Yes, so configure shape.
        setCollisionShapeDefinition( shapeIndex, fixtureDef );
    }

    return shapeIndex;
}

//-----------------------------------------------------------------------------

S32 SceneObject::parseEdgeCollisionShape( const char *pBuffer, b2FixtureDef& fixtureDef )
{
    // Fetch element count.
    const U32 elementCount =Utility::mGetStringElementCount( pBuffer );

    // Sanity!
    if ( elementCount < 4 )
    {
        Con::errorf("SceneObject::parseEdgeCollisionShape() - Found %d elements which is incorrect for parsing the shape.", elementCount );
        return -1;
    }

    // Parse arguments.
    const b2Vec2 localPosition1 = Utility::mGetStringElementVector(pBuffer, 0);
    const b2Vec2 localPosition2 = Utility::mGetStringElementVector(pBuffer, 2);

    bool hasAdjacentLocalPosition1 = false;
    bool hasAdjacentLocalPosition2 = false;
    b2Vec2 adjacentLocalPosition1( 0.0f, 0.0f );
    b2Vec2 adjacentLocalPosition2( 0.0f, 0.0f );

    // Adjacent arguments?    
    if ( elementCount > 4 )
    {
        // Yes, so sanity!
        if ( elementCount != 10 )
        {
            Con::errorf("SceneObject::parseEdgeCollisionShape() - Found %d elements which is incorrect for parsing the shape.", elementCount );
            return -1;
        }

        // Fetch position of adjacent arguments.
        pBuffer = Utility::mGetStringElement(pBuffer, 4, false );

        hasAdjacentLocalPosition1 = dAtob(Utility::mGetStringElement( pBuffer, 0 ));
        hasAdjacentLocalPosition2 = dAtob(Utility::mGetStringElement( pBuffer, 1 ));
        adjacentLocalPosition1    = Utility::mGetStringElementVector( pBuffer, 2 );
        adjacentLocalPosition2    = Utility::mGetStringElementVector( pBuffer, 4 );
    }

    // Create shape.
    const S32 shapeIndex = createEdgeCollisionShape(
        localPosition1, localPosition2,
        hasAdjacentLocalPosition1, hasAdjacentLocalPosition2,
        adjacentLocalPosition1, adjacentLocalPosition2 );

    // Was shape created.
    if ( shapeIndex != -1 )
    {
        // Yes, so configure shape.
        setCollisionShapeDefinition( shapeIndex, fixtureDef );
    }

    return shapeIndex;
}

//-----------------------------------------------------------------------------

void SceneObject::setFlip( const bool flipX, const bool flipY )
{
   // If nothing's changed, we don't update anything. (JDD)
   if( flipX == mFlipX && flipY == mFlipY )
      return;

   // Set Flip.
   mFlipX = flipX;
   mFlipY = flipY;
}

//-----------------------------------------------------------------------------

void SceneObject::setBlendColorString(const char* color)
{
   U32 elementCount =Utility::mGetStringElementCount(color);
   
   // ("R G B [A]")
   if ((elementCount == 3) || (elementCount == 4))
   {
      // Extract the color.
      F32 red   = dAtof(Utility::mGetStringElement(color, 0));
      F32 green = dAtof(Utility::mGetStringElement(color, 1));
      F32 blue  = dAtof(Utility::mGetStringElement(color, 2));
      F32 alpha = 1.0f;

      // Grab the alpha if it's there.
      if (elementCount > 3)
         alpha = dAtof(Utility::mGetStringElement(color, 3));
      

      setBlendColor(ColorF(red, green, blue, alpha));
   }
}

//-----------------------------------------------------------------------------

void SceneObject::setBlendOptions( void )
{
    // Set Blend Status.
    if ( mBlendMode )
    {
        // Enable Blending.
        glEnable( GL_BLEND );
        // Set Blend Function.
        glBlendFunc( mSrcBlendFactor, mDstBlendFactor );

        // Set Colour.
        glColor4f(mBlendColor.red,mBlendColor.green,mBlendColor.blue,mBlendColor.alpha );
    }
    else
    {
        // Disable Blending.
        glDisable( GL_BLEND );
        // Reset Colour.
        glColor4f(1,1,1,1);
    }

    // Set Alpha Test.
    if ( mAlphaTest >= 0.0f )
    {
        // Enable Test.
        glEnable( GL_ALPHA_TEST );
        glAlphaFunc( GL_GREATER, mAlphaTest );
    }
    else
    {
        // Disable Test.
        glDisable( GL_ALPHA_TEST );
    }
}

//-----------------------------------------------------------------------------

void SceneObject::resetBlendOptions( void )
{
    // Disable Blending.
    glDisable( GL_BLEND );

    glDisable( GL_ALPHA_TEST);

    // Reset Colour.
    glColor4f(1,1,1,1);
}

//---------------------------------------------------------------------------------------------

void SceneObject::onInputEvent( StringTableEntry name, const GuiEvent& event, const Vector2& worldMousePosition )
{
    // Argument Buffers.
    char argBuffer[3][32];

    // ID
    dSprintf(argBuffer[0], 32, "%d", event.eventID);
    
    // Format Mouse-Position Buffer.
    dSprintf(argBuffer[1], 32, "%g %g", worldMousePosition.x, worldMousePosition.y);

    // Optional double click
    dSprintf(argBuffer[2], 32, "%d", event.mouseClickCount);

    // Call Scripts.
    Con::executef(this, 4, name, argBuffer[0], argBuffer[1], argBuffer[2]);
}

//-----------------------------------------------------------------------------

void SceneObject::attachGui( GuiControl* pGuiControl, SceneWindow* pSceneWindow, const bool sizeControl )
{
    // Attach Gui Control.
    mpAttachedGui = pGuiControl;

    // Attach SceneWindow.
    mpAttachedGuiSceneWindow = pSceneWindow;

    // Set Size Gui Flag.
    mAttachedGuiSizeControl = sizeControl;

    // Register Gui Control/Window References.
    mpAttachedGui->registerReference( (SimObject**)&mpAttachedGui );
    mpAttachedGuiSceneWindow->registerReference( (SimObject**)&mpAttachedGuiSceneWindow );

    // Check/Adjust Parentage.
    if ( mpAttachedGui->getParent() != mpAttachedGuiSceneWindow )
    {
        // Warn.
        //Con::warnf("SceneObject::attachGui() - GuiControl is not a direct-child of Scene; adjusting!");
        // Remove GuiControl from existing parent (if it has one).
        if ( mpAttachedGui->getParent() )
        {
            mpAttachedGui->getParent()->removeObject( mpAttachedGui );
        }

        // Add it to the scene-window.
        mpAttachedGuiSceneWindow->addObject( mpAttachedGui );
    }
    
}

//-----------------------------------------------------------------------------

void SceneObject::detachGui( void )
{
    // Unregister Gui Control Reference.
    if ( mpAttachedGui )
    {
       // [neo, 5/7/2007 - #2997]
       // Changed to UNregisterReference was registerReference which would crash later
       mpAttachedGui->unregisterReference( (SimObject**)&mpAttachedGui );
        mpAttachedGui = NULL;
    }

    // Unregister Gui Control Reference.
    if ( mpAttachedGuiSceneWindow )
    {
        mpAttachedGuiSceneWindow->registerReference( (SimObject**)&mpAttachedGuiSceneWindow );
        mpAttachedGuiSceneWindow = NULL;
    }
}

//-----------------------------------------------------------------------------

void SceneObject::updateAttachedGui( void )
{
    // Finish if either Gui Control or Window is invalid.
    if ( !mpAttachedGui || !mpAttachedGuiSceneWindow )
        return;

    // Ignore if we're not in the scene that the scene-window is attached to.
    if ( getScene() != mpAttachedGuiSceneWindow->getScene() )
    {
        // Warn.
        Con::warnf("SceneObject::updateAttachedGui() - SceneWindow is not attached to my Scene!");
        // Detach from GUI Control.
        detachGui();
        // Finish Here.
        return;
    }

// Debug Profiling.
#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_START(PHYSIC_PROXY_SCENEOBJECT_updateAttachedGui);
#endif

    // Calculate the GUI Controls' dimensions.
    Point2I topLeftI, extentI;

    // Size Control?
    if ( mAttachedGuiSizeControl )
    {
        // Yes, so fetch Clip Rectangle; this forms the area we want to fix the Gui-Control to.
        const RectF objAABB = getAABBRectangle();
        // Fetch Top-Left.
        Vector2 upperLeft = Vector2( objAABB.point.x, objAABB.point.y + objAABB.extent.y );
        Vector2 lowerRight = Vector2( objAABB.point.x + objAABB.extent.x, objAABB.point.y );

        // Convert Scene to Window Coordinates.
        mpAttachedGuiSceneWindow->sceneToWindowPoint( upperLeft, upperLeft );
        mpAttachedGuiSceneWindow->sceneToWindowPoint( lowerRight, lowerRight );
        // Convert Control Dimensions.
        topLeftI.set( S32(upperLeft.x), S32(upperLeft.y) );
        extentI.set( S32(lowerRight.x-upperLeft.x), S32(lowerRight.y-upperLeft.y) );
    }
    else
    {
        // No, so center GUI-Control on objects position but don't resize it.

        // Calculate Position from World Clip.
        const RectF clipRectangle = getAABBRectangle();
        // Calculate center position.
        const Vector2 centerPosition = clipRectangle.point + Vector2(clipRectangle.len_x()*0.5f, clipRectangle.len_y()*0.5f);

        // Convert Scene to Window Coordinates.
        Vector2 positionI;
        mpAttachedGuiSceneWindow->sceneToWindowPoint( centerPosition, positionI );
        // Fetch Control Extents (which don't change here).
        extentI = mpAttachedGui->getExtent();
        // Calculate new top-left.
        topLeftI.set( S32(positionI.x-extentI.x/2), S32(positionI.y-extentI.y/2) );
    }

    // Set Control Dimensions.
    mpAttachedGui->resize( topLeftI, extentI );

// Debug Profiling.
#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_END();   // PHYSIC_PROXY_SCENEOBJECT_updateAttachedGui
#endif
}

//-----------------------------------------------------------------------------

void SceneObject::copyFrom( SceneObject* pSceneObject, const bool copyDynamicFields )
{
    // Copy the specified object.
    pSceneObject->copyTo( this );

    // Copy over dynamic fields if requested.
    if ( copyDynamicFields )
        pSceneObject->assignDynamicFieldsFrom( this );
}

//-----------------------------------------------------------------------------

void SceneObject::copyTo( SimObject* obj )
{
    // Call parent.
    Parent::copyTo(obj);

    // Fetch object.
    SceneObject* object = dynamic_cast<SceneObject*>(obj);

    // Sanity!
    AssertFatal(object != NULL, "SceneObject::copyTo() - Object is not the correct type.");

    /// Lifetime.
    object->setLifetime( getLifetime() );

    /// Scene Layers.
    object->setSceneLayer( getSceneLayer() );

    /// Scene groups.
    object->setSceneGroup( getSceneGroup() );

    /// Area.
    object->setSize( getSize() );

    /// Position / Angle.
    object->setPosition( getPosition() );
    object->setAngle( getAngle() );
    object->setFixedAngle( getFixedAngle() );

    /// Body.
    object->setBodyType( getBodyType() );
    object->setActive( getActive() );
    object->setAwake( getAwake() );
    object->setBullet( getBullet() );
    object->setSleepingAllowed( getSleepingAllowed() );

    /// Collision control.
    object->setCollisionGroups( getCollisionGroupMask() );
    object->setCollisionLayers( getCollisionLayerMask() );
    object->setCollisionSuppress( getCollisionSuppress() );
    object->setGatherContacts( getGatherContacts() );
    object->setDefaultDensity( getDefaultDensity() );
    object->setDefaultFriction( getDefaultFriction() );
    object->setDefaultRestitution( getDefaultRestitution() );

    /// Velocities.
    object->setLinearVelocity( getLinearVelocity() );
    object->setAngularVelocity( getAngularVelocity() );
    object->setLinearDamping( getLinearDamping() );
    object->setAngularDamping( getAngularDamping() );

    /// Gravity scaling.
    object->setGravityScale( getGravityScale() );

    /// Collision shapes.
    object->clearCollisionShapes();
    const U32 collisionShapeCount = getCollisionShapeCount();
    if ( collisionShapeCount > 0 )
    {
        // Write collision shapes using string formatting for now.
        char collisionShapeBuffer[256];
        for ( U32 index = 0; index < collisionShapeCount; ++index )
        {
            formatCollisionShape( index, collisionShapeBuffer, 256 );
            object->parseCollisionShape( collisionShapeBuffer );
        }
    }

    /// Render visibility.
    object->setVisible( getVisible() );

    /// Render flipping.
    object->setFlip( getFlipX(), getFlipY() );

    /// Render blending.
    object->setBlendMode( getBlendMode() );
    object->setSrcBlendFactor( getSrcBlendFactor() );
    object->setDstBlendFactor( getDstBlendFactor() );
    object->setBlendColor( getBlendColor() );
    object->setAlphaTest( getAlphaTest() );

    /// Render sorting.
    object->setSortPoint( getSortPoint() );

    /// Input events.
    object->setUseInputEvents( getUseInputEvents() );

    // Script callbacks.
    object->setUpdateCallback( getUpdateCallback() );   
    object->setCollisionCallback( getCollisionCallback() );
    object->setSleepingCallback( getSleepingCallback() );

    /// Misc.
    object->setBatchIsolated( getBatchIsolated() );
   
    /// Debug mode.
    setDebugOn( getDebugMask() );
}

//-----------------------------------------------------------------------------

void SceneObject::safeDelete()
{
    // We cannot delete child objects here.
    if ( getIsChild() )
        return;

    // Are we in a scene?
    if ( getScene() )
    {
        // Yes, so add a delete-request to the scene.
        getScene()->addDeleteRequest( this );
    }
    else
    {
        // No, so use standard SimObject helper.
        deleteObject();
    }
}

//-----------------------------------------------------------------------------

void SceneObject::addDestroyNotification( SceneObject* pSceneObject )
{
    // Search list to see if we're already in it (finish if we are).
    for ( U32 n = 0; n < (U32)mDestroyNotifyList.size(); n++ )
    {
        // In the list already?
        if ( mDestroyNotifyList[n].mpSceneObject == pSceneObject )
        {
            // Yes, so just bump-up the reference count.
            mDestroyNotifyList[n].mRefCount++;

            // Finish here.
            return;
        }
    }

    // Add Destroy Notification.
    tDestroyNotification notification;
    notification.mpSceneObject = pSceneObject;
    notification.mRefCount = 1;

    // Add Notification.
    mDestroyNotifyList.push_back( notification );
}

//-----------------------------------------------------------------------------

void SceneObject::removeDestroyNotification( SceneObject* pSceneObject )
{
    // Find object in notification list.
    for ( U32 n = 0; n < (U32)mDestroyNotifyList.size(); n++ )
    {
        // Our object?
        if ( mDestroyNotifyList[n].mpSceneObject == pSceneObject )
        {
            // Yes, so reduce reference count.
            mDestroyNotifyList[n].mRefCount--;
            // Finish Here.
            return;
        }
    }
}

//-----------------------------------------------------------------------------

void SceneObject::processDestroyNotifications( void )
{
    // Find object in notification list.
    while( mDestroyNotifyList.size() )
    {
        // Fetch Notification Item.
        tDestroyNotification notification = mDestroyNotifyList.first();
        // Only action if we've got a reference active.
        if ( notification.mRefCount > 0 )
            // Call Destroy Notification.
            notification.mpSceneObject->onDestroyNotify( this );

        // Remove it.
        mDestroyNotifyList.pop_front();
    }

    // Sanity!
    AssertFatal( mDestroyNotifyList.size() == 0, "SceneObject::processDestroyNotifications() - Notifications still pending!" );
}

//-----------------------------------------------------------------------------

void SceneObject::notifyComponentsAddToScene( void )
{
    // Notify components.
    VectorPtr<SimComponent*>& componentList = lockComponentList();
    for( SimComponentIterator itr = componentList.begin(); itr != componentList.end(); ++itr )
    {
        SimComponent *pComponent = *itr;
        if( pComponent != NULL )
            pComponent->onAddToScene();
    }
    unlockComponentList();
}

//-----------------------------------------------------------------------------

void SceneObject::notifyComponentsRemoveFromScene( void )
{
    // Notify components.
    VectorPtr<SimComponent*>& componentList = lockComponentList();
    for( SimComponentIterator itr = componentList.begin(); itr != componentList.end(); ++itr )
    {
        SimComponent *pComponent = *itr;
        if( pComponent != NULL )
            pComponent->onRemoveFromScene();
    }
    unlockComponentList();
}

//-----------------------------------------------------------------------------

void SceneObject::notifyComponentsUpdate( void )
{
    // Notify components.
    VectorPtr<SimComponent*>& componentList = lockComponentList();
    for( SimComponentIterator itr = componentList.begin(); itr != componentList.end(); ++itr )
    {
        SimComponent *pComponent = *itr;
        if( pComponent != NULL )
            pComponent->onUpdate();
    }
    unlockComponentList();
}

//-----------------------------------------------------------------------------

BehaviorInstance * SceneObject::behavior(const char *name)
{
   StringTableEntry stName = StringTable->insert(name);
   VectorPtr<SimComponent *>&componentList = lockComponentList();

   for( SimComponentIterator nItr = componentList.begin(); nItr != componentList.end(); nItr++ )
   {
      BehaviorInstance *pComponent = dynamic_cast<BehaviorInstance*>(*nItr);
      if( pComponent && StringTable->insert(pComponent->getTemplateName()) == stName )
      {
         unlockComponentList();
         return pComponent;
      }
   }

   unlockComponentList();

   return NULL;
}

//-----------------------------------------------------------------------------

U32 SceneObject::getGlobalSceneObjectCount( void )
{
    return sGlobalSceneObjectCount;
}

//-----------------------------------------------------------------------------

void SceneObject::onTamlPreWrite( void )
{
    // Call parent.
    Parent::onTamlPreWrite();

    // Finish if a prefab is assigned.
    if ( hasPrefab() )
        return;

#if 0
    // Fetch collision shape count.
    const U32 collisionShapeCount = getCollisionShapeCount();

    // Format collision shapes if we have any.
    if ( collisionShapeCount > 0 )
    {
        char indexBuffer[16];
        char collisionShapeBuffer[256];
        for ( U32 index = 0; index < collisionShapeCount; ++index )
        {
            dSprintf( indexBuffer, 16, "%d", index );
            formatCollisionShape( index, collisionShapeBuffer, 256 );
            setDataField( SCENEOBJECT_COLLISIONSHAPE_FIELDNAME, indexBuffer, collisionShapeBuffer ); 
        }
    }
#endif
}

//-----------------------------------------------------------------------------

void SceneObject::onTamlPostWrite( void )
{
    // Call parent.
    Parent::onTamlPostWrite();

    // Finish if a prefab is assigned.
    if ( hasPrefab() )
        return;

    // Fetch collision shape count.
    const U32 collisionShapeCount = getCollisionShapeCount();

    // Format collision shapes if we have any.
    if ( collisionShapeCount > 0 )
    {
        char indexBuffer[16];

        // Now clear fields as these are processed when the object is added to a scene.
        for ( U32 index = 0; index < collisionShapeCount; ++index )
        {
            dSprintf( indexBuffer, 16, "%d", index );
            setDataField( SCENEOBJECT_COLLISIONSHAPE_FIELDNAME, indexBuffer, "" ); 
        }
    }
}

//-----------------------------------------------------------------------------

void SceneObject::onTamlPreRead( void )
{
    // Call parent.
    Parent::onTamlPreRead();
}

//-----------------------------------------------------------------------------

void SceneObject::onTamlPostRead( const TamlCollection& customCollection )
{
    // Call parent.
    Parent::onTamlPostRead( customCollection );

    // Create collision shapes from dynamic fields.
    char indexBuffer[16];

    // Iterate all collision shapes.
    for ( U32 collisionShapeIndex = 0;; ++collisionShapeIndex )
    {
        // Format index buffer.
        dSprintf( indexBuffer, 16, "%d", collisionShapeIndex );

        // Fetch data field.
        const char* pData = getDataField(SCENEOBJECT_COLLISIONSHAPE_FIELDNAME, indexBuffer);

        // Do we have any useful data for this collision shape?
        if ( pData != NULL && dStrlen( pData ) > 0 )
        {
            // Yes, so parse it.
            parseCollisionShape( pData );

            // Reset data field.
            setDataField(SCENEOBJECT_COLLISIONSHAPE_FIELDNAME, indexBuffer, "");

            // Next collision shape.
            continue;
        }

        // Stop parsing collision shapes.
        break;
    };
}

//-----------------------------------------------------------------------------

void SceneObject::onTamlCustomWrite( TamlCollection& customCollection )
{
    // Call parent.
    Parent::onTamlCustomWrite( customCollection );

    // Finish if we have a prefab assigned.
    if ( hasPrefab() )
        return;

    // Fetch collision shape count.
    const U32 collisionShapeCount = getCollisionShapeCount();

    // Finish if no collision shapes.
    if ( collisionShapeCount == 0 )
        return;

    // Add collision shape property.
    TamlCollectionProperty* pCollisionShapeProperty = customCollection.addCollectionProperty( shapeCollectionName );

    // Iterate collision shapes.
    for ( U32 shapeIndex = 0; shapeIndex < collisionShapeCount; ++shapeIndex )
    {
        // Fetch collision shape definition.
        b2FixtureDef fixtureDef = getCollisionShapeDefinition( shapeIndex );

        // Add collision shape type alias.
        // NOTE:    The name of the type-alias will get updated shortly.
        TamlPropertyTypeAlias* pCollisionShapeTypeAlias = pCollisionShapeProperty->addTypeAlias( StringTable->EmptyString );

        // Add common collision shape fields.
        if ( mNotEqual( getDefaultDensity(), fixtureDef.density ) )
            pCollisionShapeTypeAlias->addPropertyField( shapeDensityName, fixtureDef.density );

        if ( mNotEqual( getDefaultFriction(), fixtureDef.friction ) )
            pCollisionShapeTypeAlias->addPropertyField( shapeFrictionName, fixtureDef.friction );

        if ( mNotEqual( getDefaultRestitution(), fixtureDef.restitution ) )
            pCollisionShapeTypeAlias->addPropertyField( shapeRestitutionName, fixtureDef.restitution );

        if ( fixtureDef.isSensor == true )
            pCollisionShapeTypeAlias->addPropertyField( shapeSensorName, fixtureDef.isSensor );

        // Populate collision shape appropriately.
        switch( fixtureDef.shape->GetType() )
        {
        case b2Shape::e_circle:
            {
                // Set type alias name.
                pCollisionShapeTypeAlias->mAliasName = StringTable->insert( circleTypeName );

                // Fetch shape.
                const b2CircleShape* pShape = dynamic_cast<const b2CircleShape*>( fixtureDef.shape );

                // Sanity!
                AssertFatal( pShape != NULL, "SceneObject::onTamlCustomWrite() - Invalid circle shape type returned." );

                // Add radius property.
                pCollisionShapeTypeAlias->addPropertyField( circleRadiusName, pShape->m_radius );

                // Add offset property (if not zero).
                if ( !Vector2(pShape->m_p).isZero() )
                    pCollisionShapeTypeAlias->addPropertyField( circleOffsetName, pShape->m_p );
            }
            break;

        case b2Shape::e_polygon:
            {
                // Set type alias name.
                pCollisionShapeTypeAlias->mAliasName = StringTable->insert( polygonTypeName );

                // Fetch shape.
                const b2PolygonShape* pShape = dynamic_cast<const b2PolygonShape*>( fixtureDef.shape );

                // Sanity!
                AssertFatal( pShape != NULL, "SceneObject::onTamlCustomWrite() - Invalid polygon shape type returned." );

                // Fetch point count.
                const U32 pointCount = pShape->GetVertexCount();

                // Add shape properties.
                for ( U32 pointIndex = 0; pointIndex < pointCount; ++pointIndex )
                {
                    // Format point index name.
                    char pointIndexBuffer[16];
                    dSprintf( pointIndexBuffer, sizeof(pointIndexBuffer), "%s%d", polygonPointName, pointIndex );
                    
                    // Add point property.
                    pCollisionShapeTypeAlias->addPropertyField( pointIndexBuffer, pShape->GetVertex( pointIndex ) );
                }
            }
            break;

        case b2Shape::e_chain:
            {
                // Set type alias name.
                pCollisionShapeTypeAlias->mAliasName = StringTable->insert( chainTypeName );

                // Fetch shape.
                const b2ChainShape* pShape = dynamic_cast<const b2ChainShape*>( fixtureDef.shape );

                // Sanity!
                AssertFatal( pShape != NULL, "SceneObject::onTamlCustomWrite() - Invalid chain shape type returned." );

                // Fetch point count.
                const U32 pointCount = pShape->m_count;

                // Add shape properties.
                for ( U32 pointIndex = 0; pointIndex < pointCount; ++pointIndex )
                {
                    // Format point index name.
                    char pointIndexBuffer[16];
                    dSprintf( pointIndexBuffer, sizeof(pointIndexBuffer), "%s%d", chainPointName, pointIndex );
                    
                    // Add point property.
                    pCollisionShapeTypeAlias->addPropertyField( pointIndexBuffer, pShape->m_vertices[pointIndex] );
                }

                // Add adjacent start point (if specified).
                if ( pShape->m_hasPrevVertex )
                    pCollisionShapeTypeAlias->addPropertyField( chainAdjacentStartName, pShape->m_prevVertex );

                // Add adjacent end point (if specified).
                if ( pShape->m_hasNextVertex )
                    pCollisionShapeTypeAlias->addPropertyField( chainAdjacentEndName, pShape->m_nextVertex );
            }
            break;

        case b2Shape::e_edge:
            {
                // Set type alias name.
                pCollisionShapeTypeAlias->mAliasName = StringTable->insert( edgeTypeName );

                // Fetch shape.
                const b2EdgeShape* pShape = dynamic_cast<const b2EdgeShape*>( fixtureDef.shape );

                // Sanity!
                AssertFatal( pShape != NULL, "SceneObject::onTamlCustomWrite() - Invalid edge shape type returned." );

                // Add start/end points.
                pCollisionShapeTypeAlias->addPropertyField( edgeStartName, pShape->m_vertex1 );
                pCollisionShapeTypeAlias->addPropertyField( edgeEndName, pShape->m_vertex2 );

                // Add adjacent start point (if specified).
                if ( pShape->m_hasVertex0 )
                    pCollisionShapeTypeAlias->addPropertyField( edgeAdjacentStartName, pShape->m_vertex0 );

                // Add adjacent end point (if specified).
                if ( pShape->m_hasVertex3 )
                    pCollisionShapeTypeAlias->addPropertyField( edgeAdjacentEndName, pShape->m_vertex3 );
            }
            break;

        default:
            // Sanity!
            AssertFatal( false, "SceneObject::onTamlCustomWrite() - Unknown shape type detected." );
        }
    }
}

//-----------------------------------------------------------------------------

void SceneObject::onTamlCustomRead( const TamlCollection& customCollection )
{
    // Call parent.
    Parent::onTamlCustomRead( customCollection );

    // Finish if we have a prefab assigned.
    if ( hasPrefab() )
        return;

    // Find collision shape collection.
    const TamlCollectionProperty* pCollisionShapeProperty = customCollection.findProperty( shapeCollectionName );

    // Finish if we don't have collision shapes.
    if ( pCollisionShapeProperty == NULL )
        return;

    // Iterate collision shapes.
    for( TamlCollectionProperty::const_iterator propertyTypeAliasItr = pCollisionShapeProperty->begin(); propertyTypeAliasItr != pCollisionShapeProperty->end(); ++propertyTypeAliasItr )
    {
        // Fetch property type alias.
        TamlPropertyTypeAlias* pPropertyTypeAlias = *propertyTypeAliasItr;

        // Fetch alias name.
        StringTableEntry aliasName = pPropertyTypeAlias->mAliasName;

        // Ready common fields.
        F32 shapeDensity     = getDefaultDensity();
        F32 shapeFriction    = getDefaultFriction();
        F32 shapeRestitution = getDefaultRestitution();
        bool shapeSensor     = false;

        S32 shapeIndex;

        // Is this a circle shape?
        if ( aliasName == circleTypeName )
        {
            // Yes, so ready fields.
            F32 radius = 0.0f;
            b2Vec2 offset( 0.0f, 0.0f );

            // Iterate property fields.
            for ( TamlPropertyTypeAlias::const_iterator propertyFieldItr = pPropertyTypeAlias->begin(); propertyFieldItr != pPropertyTypeAlias->end(); ++propertyFieldItr )
            {
                // Fetch property field.
                TamlPropertyField* pPropertyField = *propertyFieldItr;

                // Fetch property field name.
                StringTableEntry fieldName = pPropertyField->getFieldName();

                // Check common fields.
                if ( fieldName == shapeDensityName )
                {
                    pPropertyField->getFieldValue( shapeDensity );
                }
                else if ( fieldName == shapeFrictionName )
                {
                    pPropertyField->getFieldValue( shapeFriction );
                }
                else if ( fieldName == shapeRestitutionName )
                {
                    pPropertyField->getFieldValue( shapeRestitution );
                }
                else if ( fieldName == shapeSensorName )
                {
                    pPropertyField->getFieldValue( shapeSensor );
                }
                // Check circle fields.
                else if ( fieldName == circleRadiusName )
                {
                    pPropertyField->getFieldValue( radius );
                }
                else if ( fieldName == circleOffsetName )
                {
                    pPropertyField->getFieldValue( offset );
                }                   
            }

            // Is radius valid?
            if ( radius <= 0.0f )
            {
                // No, so warn.
                Con::warnf( "SceneObject::onTamlCustomRead() - Invalid radius on circle collision shape '%g'.  Using default.", radius );

                // Set default.
                radius = 1.0f;
            }

            // Create shape.
            shapeIndex = createCircleCollisionShape( radius, offset );
        }
        // Is this a polygon shape?
        else if ( aliasName == polygonTypeName )
        {
            // Yes, so ready fields.
            b2Vec2 points[b2_maxPolygonVertices];
            U32 pointCount = 0;

            // Iterate property fields.
            for ( TamlPropertyTypeAlias::const_iterator propertyFieldItr = pPropertyTypeAlias->begin(); propertyFieldItr != pPropertyTypeAlias->end(); ++propertyFieldItr )
            {
                // Fetch property field.
                TamlPropertyField* pPropertyField = *propertyFieldItr;

                // Fetch property field name.
                StringTableEntry fieldName = pPropertyField->getFieldName();

                // Check common fields.
                if ( fieldName == shapeDensityName )
                {
                    pPropertyField->getFieldValue( shapeDensity );
                }
                else if ( fieldName == shapeFrictionName )
                {
                    pPropertyField->getFieldValue( shapeFriction );
                }
                else if ( fieldName == shapeRestitutionName )
                {
                    pPropertyField->getFieldValue( shapeRestitution );
                }
                else if ( fieldName == shapeSensorName )
                {
                    pPropertyField->getFieldValue( shapeSensor );
                }
                // Check polygon fields.
                else if ( pPropertyField->fieldNameBeginsWith( polygonPointName ) )
                {
                    // Is the point count at maximum?
                    if ( pointCount == b2_maxPolygonVertices )
                    {
                        // Yes, so warn.
                        Con::warnf( "SceneObject::onTamlCustomRead() - Polygon point count exceed the maximum points '%d'.", b2_maxPolygonVertices );
                        continue;
                    }

                    b2Vec2 point;
                    pPropertyField->getFieldValue( point );
                    points[pointCount++] = point;
                }
            }

            // Is point count valid?
            if ( pointCount == 0 )
            {
                // No, so warn.
                Con::warnf( "SceneObject::onTamlCustomRead() - No points on polygon collision shape." );

                continue;
            }

            // Create shape.
            shapeIndex = createPolygonCollisionShape( pointCount, points );
        }
        // Is this a chain shape?
        else if ( aliasName == chainTypeName )
        {
            // Yes, so ready fields.
            Vector<b2Vec2> points;
            bool hasAdjacentStartPoint;
            bool hasAdjacentEndPoint;
            b2Vec2 adjacentStartPoint;
            b2Vec2 adjacentEndPoint;

            // Iterate property fields.
            for ( TamlPropertyTypeAlias::const_iterator propertyFieldItr = pPropertyTypeAlias->begin(); propertyFieldItr != pPropertyTypeAlias->end(); ++propertyFieldItr )
            {
                // Fetch property field.
                TamlPropertyField* pPropertyField = *propertyFieldItr;

                // Fetch property field name.
                StringTableEntry fieldName = pPropertyField->getFieldName();

                // Check common fields.
                if ( fieldName == shapeDensityName )
                {
                    pPropertyField->getFieldValue( shapeDensity );
                }
                else if ( fieldName == shapeFrictionName )
                {
                    pPropertyField->getFieldValue( shapeFriction );
                }
                else if ( fieldName == shapeRestitutionName )
                {
                    pPropertyField->getFieldValue( shapeRestitution );
                }
                else if ( fieldName == shapeSensorName )
                {
                    pPropertyField->getFieldValue( shapeSensor );
                }
                // Check chain fields.
                else if ( pPropertyField->fieldNameBeginsWith( chainPointName ) )
                {
                    b2Vec2 point;
                    pPropertyField->getFieldValue( point );
                    points.push_back( point );
                }
                else if ( fieldName == chainAdjacentStartName )
                {
                    pPropertyField->getFieldValue( adjacentStartPoint );
                    hasAdjacentStartPoint = true;
                }
                else if ( fieldName == chainAdjacentEndName )
                {
                    pPropertyField->getFieldValue( adjacentEndPoint );
                    hasAdjacentEndPoint = true;
                }
            }

            // Is point count valid?
            if ( points.size() == 0 )
            {
                // No, so warn.
                Con::warnf( "SceneObject::onTamlCustomRead() - No points on chain collision shape." );

                continue;
            }

            // Create shape.
            shapeIndex = createChainCollisionShape( points.size(), points.address(), hasAdjacentStartPoint, hasAdjacentEndPoint, adjacentStartPoint, adjacentEndPoint );
        }
        // Is this an edge shape?
        else if ( aliasName == edgeTypeName )
        {
            // Yes, so ready fields.
            b2Vec2 point0;
            b2Vec2 point1;
            bool hasAdjacentStartPoint;
            bool hasAdjacentEndPoint;
            b2Vec2 adjacentStartPoint;
            b2Vec2 adjacentEndPoint;

            // Iterate property fields.
            for ( TamlPropertyTypeAlias::const_iterator propertyFieldItr = pPropertyTypeAlias->begin(); propertyFieldItr != pPropertyTypeAlias->end(); ++propertyFieldItr )
            {
                // Fetch property field.
                TamlPropertyField* pPropertyField = *propertyFieldItr;

                // Fetch property field name.
                StringTableEntry fieldName = pPropertyField->getFieldName();

                // Check common fields.
                if ( fieldName == shapeDensityName )
                {
                    pPropertyField->getFieldValue( shapeDensity );
                }
                else if ( fieldName == shapeFrictionName )
                {
                    pPropertyField->getFieldValue( shapeFriction );
                }
                else if ( fieldName == shapeRestitutionName )
                {
                    pPropertyField->getFieldValue( shapeRestitution );
                }
                else if ( fieldName == shapeSensorName )
                {
                    pPropertyField->getFieldValue( shapeSensor );
                }
                // Check edge fields.
                else if ( fieldName == edgeStartName )
                {
                    pPropertyField->getFieldValue( point0 );
                }
                else if ( fieldName == edgeEndName )
                {
                    pPropertyField->getFieldValue( point1 );
                }
                else if ( fieldName == edgeAdjacentStartName )
                {
                    pPropertyField->getFieldValue( adjacentStartPoint );
                    hasAdjacentStartPoint = true;
                }
                else if ( fieldName == edgeAdjacentEndName )
                {
                    pPropertyField->getFieldValue( adjacentEndPoint );
                    hasAdjacentEndPoint = true;
                }
            }

            // Create shape.
            shapeIndex = createEdgeCollisionShape( point0, point1, hasAdjacentStartPoint, hasAdjacentEndPoint, adjacentStartPoint, adjacentEndPoint );
        }
        // Unknown shape type!
        else
        {
            // Warn.
            Con::warnf( "Unknown shape type of '%s' encountered.", aliasName );

            // Sanity!
            AssertFatal( false, "SceneObject::onTamlCustomRead() - Unknown shape type detected." );

            continue;
        }

        // Set common properties.
        setCollisionShapeDensity( shapeIndex, shapeDensity );
        setCollisionShapeFriction( shapeIndex, shapeFriction );
        setCollisionShapeRestitution( shapeIndex, shapeRestitution );
        setCollisionShapeIsSensor( shapeIndex, shapeSensor );
    }        
}

//-----------------------------------------------------------------------------

bool SceneObject::writeField(StringTableEntry fieldname, const char* value)
{
   if (!Parent::writeField(fieldname, value))
      return false;

   // Never save the scene field.
   if (dStricmp(fieldname, "scene") == 0)
      return false;

   return true;
}

//-----------------------------------------------------------------------------

void SceneObject::writeFields(Stream& stream, U32 tabStop)
{
    // Fetch collision shape count.
    const U32 collisionShapeCount = getCollisionShapeCount();

    // Format collision shapes if we have any.
    if ( collisionShapeCount > 0 )
    {
        // The work we want to perform here is in the Taml callback.
        onTamlPreWrite();

        // Call parent.
        Parent::writeFields(stream, tabStop);

        // The work we want to perform here is in the Taml callback.
        onTamlPostWrite();
    }
    else
    {
        // Call parent.
        Parent::writeFields(stream, tabStop);
    }
}

//------------------------------------------------------------------------------

S32 QSORT_CALLBACK SceneObject::sceneObjectLayerDepthSort(const void* a, const void* b)
{
    // Fetch scene objects.
    SceneObject* pSceneObjectA  = *((SceneObject**)a);
    SceneObject* pSceneObjectB  = *((SceneObject**)b);

    // Fetch layers.
    const U32 layerA = pSceneObjectA->getSceneLayer();
    const U32 layerB = pSceneObjectB->getSceneLayer();

    if ( layerA < layerB )
        return -1;

    if ( layerA > layerB )
        return 1;

    // Fetch layer depths.
    const F32 depthA = pSceneObjectA->getSceneLayerDepth();
    const F32 depthB = pSceneObjectB->getSceneLayerDepth();

    return depthA < depthB ? 1 : depthA > depthB ? -1 : pSceneObjectA->getSerialId() - pSceneObjectB->getSerialId();
}

//-----------------------------------------------------------------------------

static EnumTable::Enums bodyTypeLookup[] =
                {
                { b2_staticBody,    "static"    },
                { b2_kinematicBody, "kinematic" },
                { b2_dynamicBody,   "dynamic"   },
                };

EnumTable bodyTypeTable(sizeof(bodyTypeLookup) / sizeof(EnumTable::Enums), &bodyTypeLookup[0]);

//-----------------------------------------------------------------------------

static EnumTable::Enums collisionShapeTypeLookup[] =
                {
                { b2Shape::e_circle,             "circle"   },
                { b2Shape::e_edge,               "edge"     },
                { b2Shape::e_polygon,            "polygon"  },
                { b2Shape::e_chain,              "chain"    },
                };

EnumTable collisionShapeTypeTable(sizeof(collisionShapeTypeLookup) / sizeof(EnumTable::Enums), &collisionShapeTypeLookup[0]);

//-----------------------------------------------------------------------------

static EnumTable::Enums srcBlendFactorLookup[] =
                {
                { GL_ZERO,                  "ZERO"                  },
                { GL_ONE,                   "ONE"                   },
                { GL_DST_COLOR,             "DST_COLOR"             },
                { GL_ONE_MINUS_DST_COLOR,   "ONE_MINUS_DST_COLOR"   },
                { GL_SRC_ALPHA,             "SRC_ALPHA"             },
                { GL_ONE_MINUS_SRC_ALPHA,   "ONE_MINUS_SRC_ALPHA"   },
                { GL_DST_ALPHA,             "DST_ALPHA"             },
                { GL_ONE_MINUS_DST_ALPHA,   "ONE_MINUS_DST_ALPHA"   },
                { GL_SRC_ALPHA_SATURATE,    "SRC_ALPHA_SATURATE"    },
                };

EnumTable srcBlendFactorTable(sizeof(srcBlendFactorLookup) / sizeof(EnumTable::Enums), &srcBlendFactorLookup[0]);

//-----------------------------------------------------------------------------

static EnumTable::Enums dstBlendFactorLookup[] =
                {
                { GL_ZERO,                  "ZERO" },
                { GL_ONE,                   "ONE" },
                { GL_SRC_COLOR,             "SRC_COLOR" },
                { GL_ONE_MINUS_SRC_COLOR,   "ONE_MINUS_SRC_COLOR" },
                { GL_SRC_ALPHA,             "SRC_ALPHA" },
                { GL_ONE_MINUS_SRC_ALPHA,   "ONE_MINUS_SRC_ALPHA" },
                { GL_DST_ALPHA,             "DST_ALPHA" },
                { GL_ONE_MINUS_DST_ALPHA,   "ONE_MINUS_DST_ALPHA" },
                };

EnumTable dstBlendFactorTable(sizeof(dstBlendFactorLookup) / sizeof(EnumTable::Enums), &dstBlendFactorLookup[0]);

//-----------------------------------------------------------------------------

b2BodyType getBodyTypeEnum(const char* label)
{
    // Search for Mnemonic.
    for (U32 i = 0; i < (sizeof(bodyTypeLookup) / sizeof(EnumTable::Enums)); i++)
    {
        if( dStricmp(bodyTypeLookup[i].label, label) == 0)
            return (b2BodyType)bodyTypeLookup[i].index;
    }

    // Warn!
    Con::warnf("SceneObject::getBodyTypeDescription() - Invalid body type of '%s'", label );

    // Bah!
    return (b2BodyType)-1;
}

//-----------------------------------------------------------------------------

const char* getBodyTypeDescription(const b2BodyType bodyType)
{
    // Search for Mnemonic.
    for (U32 i = 0; i < (sizeof(bodyTypeLookup) / sizeof(EnumTable::Enums)); i++)
    {
        if( bodyTypeLookup[i].index == bodyType )
            return bodyTypeLookup[i].label;
    }

    // Fatal!
    AssertFatal(false, "SceneObject::getBodyTypeDescription() - Invalid body type.");

    // Bah!
    return StringTable->EmptyString;
}

//-----------------------------------------------------------------------------

b2Shape::Type getCollisionShapeTypeEnum(const char* label)
{
    // Search for Mnemonic.
    for (U32 i = 0; i < (sizeof(collisionShapeTypeLookup) / sizeof(EnumTable::Enums)); i++)
    {
        if( dStricmp(collisionShapeTypeLookup[i].label, label) == 0)
            return (b2Shape::Type)collisionShapeTypeLookup[i].index;
    }

    // Warn!
    Con::warnf("SceneObject::getCollisionShapeTypeEnum() - Invalid collision shape type of '%s'", label );

    // Bah!
    return b2Shape::e_typeCount;
}

//-----------------------------------------------------------------------------

const char* getCollisionShapeTypeDescription(const b2Shape::Type collisionShapeType)
{
    // Search for Mnemonic.
    for (U32 i = 0; i < (sizeof(collisionShapeTypeLookup) / sizeof(EnumTable::Enums)); i++)
    {
        if( collisionShapeTypeLookup[i].index == collisionShapeType )
            return collisionShapeTypeLookup[i].label;
    }

    // Fatal!
    AssertFatal(false, "SceneObject::getCollisionShapeTypeDescription() - Invalid collision shape type.");

    // Bah!
    return StringTable->EmptyString;
}

//-----------------------------------------------------------------------------

S32 getSrcBlendFactorEnum(const char* label)
{
    // Search for Mnemonic.
    for (U32 i = 0; i < (sizeof(srcBlendFactorLookup) / sizeof(EnumTable::Enums)); i++)
    {
        if( dStricmp(srcBlendFactorLookup[i].label, label) == 0)
            return(srcBlendFactorLookup[i].index);
    }

    // Warn!
    Con::warnf("SceneObject::getSrcBlendFactorEnum() - Invalid source blend factor of '%s'", label );

    // Bah!
    return GL_INVALID_BLEND_FACTOR;
}

//-----------------------------------------------------------------------------

const char* getSrcBlendFactorDescription(const GLenum factor)
{
    // Search for Mnemonic.
    for (U32 i = 0; i < (sizeof(srcBlendFactorLookup) / sizeof(EnumTable::Enums)); i++)
    {
        if( srcBlendFactorLookup[i].index == (S32)factor )
            return srcBlendFactorLookup[i].label;
    }

    // Fatal!
    AssertFatal(false, "SceneObject::getSrcBlendFactorDescription() - Invalid source blend factor.");

    // Bah!
    return StringTable->EmptyString;
}

//-----------------------------------------------------------------------------

S32 getDstBlendFactorEnum(const char* label)
{
    // Search for Mnemonic.
    for (U32 i = 0; i < (sizeof(dstBlendFactorLookup) / sizeof(EnumTable::Enums)); i++)
    {
        if( dStricmp(dstBlendFactorLookup[i].label, label) == 0)
            return(dstBlendFactorLookup[i].index);
    }

    // Warn!
    Con::warnf("SceneObject::getSrcBlendFactorEnum() - Invalid destination blend factor of '%s'", label );

    // Bah!
    return GL_INVALID_BLEND_FACTOR;
}

//-----------------------------------------------------------------------------

const char* getDstBlendFactorDescription(const GLenum factor)
{
    // Search for Mnemonic.
    for(U32 i = 0; i < (sizeof(dstBlendFactorLookup) / sizeof(EnumTable::Enums)); i++)
    {
        if( dstBlendFactorLookup[i].index == (S32)factor )
            return dstBlendFactorLookup[i].label;
    }

    // Fatal!
    AssertFatal(false, "SceneObject::getDstBlendFactorDescription() - Invalid destination blend factor.");

    // Bah!
    return StringTable->EmptyString;
}
