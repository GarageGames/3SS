//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _PARTICLE_EMITTER_H_
#define _PARTICLE_EMITTER_H_

#ifndef _SCENE_OBJECT_H_
#include "2d/sceneobject/SceneObject.h"
#endif

#ifndef _GRAPH_FIELD_H_
#include "GraphField.h"
#endif

#ifndef _IMAGE_ASSET_H_
#include "2d/assets/ImageAsset.h"
#endif

#ifndef _ANIMATION_CONTROLLER_H_
#include "2d/assets/AnimationController.h"
#endif


///-----------------------------------------------------------------------------
/// Forward Declarations.
///-----------------------------------------------------------------------------
class ParticleEffect;


///-----------------------------------------------------------------------------
/// Particle Emitter Object 2D.
///-----------------------------------------------------------------------------
class ParticleEmitter : public SimObject
{
    typedef SimObject       Parent;

public:
    enum tEmitterOrientationMode { ALIGNED = 0, FIXED, RANDOM };
    enum tEmitterType { POINT = 0, LINEX, LINEY, AREA };

private:
    ParticleEffect*      pParentEffectObject;
    StringTableEntry        mEmitterName;
    Vector2               mGlobalClipBoundary[4];
    F32                     mTimeSinceLastGeneration;
    bool                    mPauseEmitter;
    bool                    mEmitterVisible;
    F32						mParticlePref;

    /// Graph Selection.
    struct tGraphSelection
    {
        StringTableEntry    mGraphName;
        GraphField*      mpGraphObject;
    };

    GraphField*              mpCurrentGraph;
    StringTableEntry            mCurrentGraphName;
    Vector<tGraphSelection*>    mGraphSelectionList;


    /// Particle Node.
    struct tParticleNode
    {
        /// Particle Node Linkages.
        tParticleNode*  mPreviousNode;
        tParticleNode*  mNextNode;

        /// Suppress Movement.
        bool                    mSuppressMovement;

        /// Particle Components.
        F32                     mParticleLifetime;
        F32                     mParticleAge;
        Vector2               mPosition;
        Vector2               mVelocity;
        F32                     mOrientationAngle;
        Vector2               mOOBB[4];
        b2Transform             mRotationTransform;
        AnimationController  mAnimationController;

        /// Render Properties.
        Vector2               mLastRenderSize;
        Vector2               mRenderSize;
        F32                     mRenderSpeed;
        F32                     mRenderSpin;
        F32                     mRenderFixedForce;
        F32                     mRenderRandomMotion;

        /// Base Properties.
        Vector2               mSize;
        F32                     mSpeed;
        F32                     mSpin;
        F32                     mFixedForce;
        F32                     mRandomMotion;
        ColorF                  mColour;    

        /// Interpolated Tick Position.
        Vector2               mPreTickPosition;
        Vector2               mPostTickPosition;
        Vector2               mRenderTickPosition;
    };


    /// Particle Data.
    const U32               mParticlePoolBlockSize;
    Vector<tParticleNode*>  mParticlePool;
    tParticleNode*          mpFreeParticleNodes;
    tParticleNode           mParticleNodeHead;
    U32                     mActiveParticles;


    // Graph Properties.
    GraphField_BV        mParticleLife;
    GraphField_BV        mQuantity;
    GraphField_BVL       mSizeX;
    GraphField_BVL       mSizeY;
    GraphField_BVL       mSpeed;
    GraphField_BVL       mSpin;
    GraphField_BVL       mFixedForce;
    GraphField_BVL       mRandomMotion;
    GraphField_BV        mEmissionForce;
    GraphField_BV        mEmissionAngle;
    GraphField_BV        mEmissionArc;
    GraphField_L         mColourRed;
    GraphField_L         mColourGreen;
    GraphField_L         mColourBlue;
    GraphField_L         mVisibility;

    /// Other Properties.
    bool                    mFixedAspect;
    Vector2               mFixedForceDirection;
    F32                     mFixedForceAngle;
    tEmitterOrientationMode mParticleOrientationMode;
    F32                     mAlign_AngleOffset;
    bool                    mAlign_KeepAligned;
    F32                     mRandom_AngleOffset;
    F32                     mRandom_Arc;
    F32                     mFixed_AngleOffset;
    tEmitterType            mEmitterType;

    AssetPtr<ImageAsset>  mImageAsset;
    U32                     mImageMapFrame;
    AssetPtr<AnimationAsset> mAnimationAsset;
    AnimationController  mAnimationControllerProxy;
    bool                    mStaticMode;

    Vector2               mPivotPoint;
    bool                    mUseEffectEmission;
    bool                    mLinkEmissionRotation;
    bool                    mIntenseParticles;
    bool                    mSingleParticle;
    bool                    mAttachPositionToEmitter;
    bool                    mAttachRotationToEmitter;
    bool                    mOrderedParticles;
    bool                    mFirstInFrontOrder;

    /// Render Options.
    bool                    mBlendMode;
    S32                     mSrcBlendFactor;
    S32                     mDstBlendFactor;

    /// Collisions.
    bool                    mUseEmitterCollisions;


    void clearGraphSelections( void );
    void addGraphSelection( const char* graphName, GraphField* pGraphObject );
    GraphField* findGraphSelection( const char* graphName ) const;

public:
    ParticleEmitter();
    ~ParticleEmitter();

    /// Initialise.
    void initialise( ParticleEffect* pParentEffect );

    // Get particle counters.
    inline U32 getActiveParticles( void ) const { return mActiveParticles; };
    inline U32 getAllocatedParticles( void ) const { return mParticlePool.size() * mParticlePoolBlockSize; }

    void setParentEffect( ParticleEffect* parent ) { pParentEffectObject = parent; };
    ParticleEffect* getParentEffect() { return pParentEffectObject; };
   
    /// Graph Editing.
    void selectGraph( const char* graphName );
    S32 addDataKey( F32 time, F32 value );
    bool removeDataKey( S32 index );
    bool clearDataKeys( void );
    bool setDataKeyValue( S32 index, F32 value );
    F32 getDataKeyValue( S32 index ) const;
    F32 getDataKeyTime( S32 index ) const;
    U32 getDataKeyCount( void ) const;
    F32 getMinValue( void ) const;
    F32 getMaxValue( void ) const;
    F32 getMinTime( void ) const;
    F32 getMaxTime( void ) const;
    F32 getGraphValue( F32 time ) const;
    bool setTimeRepeat( F32 timeRepeat );
    F32 getTimeRepeat( void ) const;
    bool setValueScale( const F32 valueScale );
    F32 getValueScale( void ) const;

    /// Set Properties.
    void setEmitterVisible( bool status );
    void setEmitterName( const char* emitterName );
    void setEmitterType( tEmitterType emitterType );
    void setParticleOrientationMode( tEmitterOrientationMode particleOrientationMode );
    void setAlignAngleOffset( F32 angleOffset );
    void setAlignKeepAligned( bool keepAligned );
    void setFixedAngleOffset( F32 angleOffset );
    void setRandomAngleOffset( F32 angleOffset );
    void setRandomArc( F32 arc );
    void setPivotPoint( Vector2 pivotPoint );
    void setFixedForceAngle( F32 fixedForceAngle );
    bool setImageMap( const char* imageMapName, U32 frame );
    bool setAnimation( const char* animationName );
    void setFixedAspect( bool aspect );
    void setIntenseParticles( bool intenseParticles );
    void setSingleParticle( bool singleParticle );
    void setAttachPositionToEmitter( bool attachPositionToEmitter );
    void setAttachRotationToEmitter( bool attachRotationToEmitter );
    void setUseEffectEmission( bool useEffectEmission );
    void setLinkEmissionRotation( bool linkEmissionRotation );
    inline void setOrderedParticles( bool ordered ) { mOrderedParticles = ordered; }
    void setFirstInFrontOrder( bool firstInFrontOrder );
    void setBlending( bool status, S32 srcBlendFactor, S32 dstBlendFactor );


    /// Get Properties.
    bool getEmitterVisible( void ) const;
    const char* getEmitterName( void ) const;
    F32 getFixedForceAngle( void ) const;
    const char* getParticleOrientation( void ) const;
    F32 getAlignAngleOffset( void ) const;
    bool getAlignKeepAligned( void ) const;
    F32 getFixedAngleOffset( void ) const;
    F32 getRandomAngleOffset( void ) const;
    F32 getRandomArc( void ) const;
    const char* getEmitterType( void ) const;
    const char* getImageMapNameFrame( void ) const;
    const char* getAnimation( void ) const;
    const char* getPivotPoint( void ) const;
    bool getFixedAspect( void ) const;
    bool getIntenseParticles( void ) const;
    bool getSingleParticle( void ) const;
    bool getAttachPositionToEmitter( void ) const;
    bool getAttachRotationToEmitter( void ) const;
    bool getUseEffectEmission( void ) const;
    bool getLinkEmissionRotation( void ) const;
    bool getOrderedParticles( void ) const { return mOrderedParticles; }
    bool getFirstInFrontOrder( void ) const;
    bool getUsingAnimation( void ) const;
    bool getBlendingStatus( void ) const { return mBlendMode; };
    S32 getSrcBlendFactor( void ) const { return mSrcBlendFactor; };
    S32 getDstBlendFactor( void ) const { return mDstBlendFactor; };

    /// Load/Save Emitter.
    bool loadEmitter( const char* emitterFile );
    bool saveEmitter( const char* emitterFile );

    /// Emitter Control.
    void playEmitter( bool resetParticles );
    void stopEmitter( void );
    void pauseEmitter( void );

    /// Remove All Particles.
    void freeAllParticles( void );

    /// Particle Block Control.
    tParticleNode* createParticle( void );
    void freeParticle( tParticleNode* pParticleNode );
    void createParticlePoolBlock( void );
    void destroyParticlePool( void );

    /// Particle Creation/Integration.
    inline void configureParticle( tParticleNode* pParticleNode );
    inline void integrateParticle( tParticleNode* pParticleNode, F32 particleAge, F32 elapsedTime );
    
    /// Collisions.
    void setEmitterCollisionStatus( const bool status );
    bool getEmitterCollisionStatus( void );

    /// Integration.
    void integrateObject( const F32 totalTime, const F32 elapsedTime, DebugStats* pDebugStats );
    void interpolateObject( const F32 timeDelta );

    /// Object Rendering.
    virtual void sceneRender( const SceneRenderState* pSceneRenderState, const SceneRenderRequest* pSceneRenderRequest, BatchRender* pBatchRenderer );

    virtual bool canRender( void ) const
    {
        return  mActiveParticles != 0 &&
            !(mStaticMode && mImageAsset.isNull()) &&
            !(!mStaticMode && mAnimationAsset.isNull() );
    }

    virtual void copyTo(SimObject* object);

    /// Declare Serialise Object.
    DECLARE_T2D_SERIALISE( ParticleEmitter );

    /// Declare Serialise Objects.
    DECLARE_2D_LOADSAVE_METHOD( ParticleEmitter, 3);
    DECLARE_2D_LOADSAVE_METHOD( ParticleEmitter, 4);

    /// Declare Console Object.
    DECLARE_CONOBJECT(ParticleEmitter);
};

//------------------------------------------------------------------------------

extern ParticleEmitter::tEmitterOrientationMode getParticleOrientationMode(const char* label);
extern ParticleEmitter::tEmitterType getEmitterType(const char* label);

#endif // _PARTICLE_EMITTER_H_
