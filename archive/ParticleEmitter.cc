//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "graphics/dgl.h"
#include "ParticleEffect.h"
#include "ParticleEmitter.h"

// Script bindings.
#include "ParticleEmitter_ScriptBinding.h"

//-----------------------------------------------------------------------------

IMPLEMENT_CONOBJECT(ParticleEmitter);

//------------------------------------------------------------------------------

static EnumTable::Enums particleOrientationLookup[] =
                {
                { ParticleEmitter::ALIGNED,  "ALIGNED" },
                { ParticleEmitter::FIXED,    "FIXED" },
                { ParticleEmitter::RANDOM,   "RANDOM" },
                };

//------------------------------------------------------------------------------

ParticleEmitter::tEmitterOrientationMode getParticleOrientationMode(const char* label)
{
    // Search for Mnemonic.
    for(U32 i = 0; i < (sizeof(particleOrientationLookup) / sizeof(EnumTable::Enums)); i++)
        if( dStricmp(particleOrientationLookup[i].label, label) == 0)
            return((ParticleEmitter::tEmitterOrientationMode)particleOrientationLookup[i].index);

    // Invalid Orientation!
    AssertFatal(false, "ParticleEmitter::getParticleOrientationMode() - Invalid Orientation Mode!");
    // Bah!
    return ParticleEmitter::FIXED;
}

//------------------------------------------------------------------------------

static EnumTable::Enums emitterTypeLookup[] =
                {
                { ParticleEmitter::POINT,    "POINT" },
                { ParticleEmitter::LINEX,    "LINEX" },
                { ParticleEmitter::LINEY,    "LINEY" },
                { ParticleEmitter::AREA,     "AREA" },
                };

//------------------------------------------------------------------------------

ParticleEmitter::tEmitterType getEmitterType(const char* label)
{
    // Search for Mnemonic.
    for(U32 i = 0; i < (sizeof(emitterTypeLookup) / sizeof(EnumTable::Enums)); i++)
        if( dStricmp(emitterTypeLookup[i].label, label) == 0)
            return((ParticleEmitter::tEmitterType)emitterTypeLookup[i].index);

    // Invalid Emitter-Type!
    AssertFatal(false, "ParticleEmitter::getEmitterType() - Invalid Emitter-Type!");
    // Bah!
    return ParticleEmitter::POINT;
}

//------------------------------------------------------------------------------

ParticleEmitter::ParticleEmitter( ) : Stream_2D_HeaderID(makeFourCCTag('2','D','P','E')),
                                            mLocalSerialiseID(1),
                                            mStaticMode(true),
                                            mEmitterName(StringTable->EmptyString),
                                            mpFreeParticleNodes(NULL),
                                            mpCurrentGraph(NULL),
                                            mCurrentGraphName(StringTable->EmptyString),
                                            mParticlePoolBlockSize(64),
                                            mActiveParticles(0),
                                            mPauseEmitter(false),
                                            mEmitterVisible(true),
                                            mUseEmitterCollisions(true)
{
    // Set Vector Associations.
    VECTOR_SET_ASSOCIATION( mParticlePool );

    // Reset Particle Node Head.
    mParticleNodeHead.mNextNode = mParticleNodeHead.mPreviousNode = &mParticleNodeHead;
    mParticlePref = Con::getFloatVariable("$pref::T2D::particleEngineQuantityScale", 1.0f);
}

//------------------------------------------------------------------------------

ParticleEmitter::~ParticleEmitter()
{
    // Destroy Particle Pool.
    destroyParticlePool();

    // Clear Graph Selections.
    clearGraphSelections();
}

//------------------------------------------------------------------------------

void ParticleEmitter::copyTo(SimObject* object)
{
   AssertFatal(dynamic_cast<ParticleEmitter*>(object), "ParticleEmitter::copyTo() - Object is not the correct type.");
   ParticleEmitter* emitter = static_cast<ParticleEmitter*>(object);

   emitter->mFixedAspect = mFixedAspect;
   emitter->mFixedForceDirection = mFixedForceDirection;
   emitter->mFixedForceAngle = mFixedForceAngle;
   emitter->mParticleOrientationMode = mParticleOrientationMode;
   emitter->mAlign_AngleOffset = mAlign_AngleOffset;
   emitter->mAlign_KeepAligned = mAlign_KeepAligned;
   emitter->mRandom_AngleOffset = mRandom_AngleOffset;
   emitter->mRandom_Arc = mRandom_Arc;
   emitter->mFixed_AngleOffset = mFixed_AngleOffset;
   emitter->mEmitterType = mEmitterType;

   if ( mImageAsset.notNull() )
       emitter->setImageMap( mImageAsset.getAssetId(), mImageMapFrame );

   if ( mAnimationAsset.notNull() )
       emitter->setAnimation( mAnimationAsset.getAssetId() );

   emitter->mPivotPoint = mPivotPoint;
   emitter->mUseEffectEmission = mUseEffectEmission;
   emitter->mLinkEmissionRotation = mLinkEmissionRotation;
   emitter->mIntenseParticles = mIntenseParticles;
   emitter->mSingleParticle = mSingleParticle;
   emitter->mAttachPositionToEmitter = mAttachPositionToEmitter;
   emitter->mAttachRotationToEmitter = mAttachRotationToEmitter;
   emitter->mOrderedParticles = mOrderedParticles;
   emitter->mFirstInFrontOrder = mFirstInFrontOrder;

   /// Render Options.
   emitter->mBlendMode = mBlendMode;
   emitter->mSrcBlendFactor = mSrcBlendFactor;
   emitter->mDstBlendFactor = mDstBlendFactor;

   /// Collisions.
   emitter->mUseEmitterCollisions = mUseEmitterCollisions;

   mParticleLife.copyTo(emitter->mParticleLife);
   mQuantity.copyTo(emitter->mQuantity);
   mSizeX.copyTo(emitter->mSizeX);
   mSizeY.copyTo(emitter->mSizeY);
   mSpeed.copyTo(emitter->mSpeed);
   mSpin.copyTo(emitter->mSpin);
   mFixedForce.copyTo(emitter->mFixedForce);
   mRandomMotion.copyTo(emitter->mRandomMotion);
   mEmissionForce.copyTo(emitter->mEmissionForce);
   mEmissionAngle.copyTo(emitter->mEmissionAngle);
   mEmissionArc.copyTo(emitter->mEmissionArc);
   mColourRed.copyTo(emitter->mColourRed);
   mColourGreen.copyTo(emitter->mColourGreen);
   mColourBlue.copyTo(emitter->mColourBlue);
   mVisibility.copyTo(emitter->mVisibility);
}

//------------------------------------------------------------------------------

void ParticleEmitter::integrateParticle( tParticleNode* pParticleNode, F32 particleAge, F32 elapsedTime )
{
    // **********************************************************************************************************************
    // Copy Old Tick Position.
    // **********************************************************************************************************************
    pParticleNode->mRenderTickPosition = pParticleNode->mPreTickPosition = pParticleNode->mPostTickPosition;


    // **********************************************************************************************************************
    // Scale Size.
    // **********************************************************************************************************************

    // Scale Size-X.
    pParticleNode->mRenderSize.x = mClampF(    pParticleNode->mSize.x * mSizeX.GraphField_OverLife.getGraphValue( particleAge ),
                                                mSizeX.GraphField_Base.getMinValue(),
                                                mSizeX.GraphField_Base.getMaxValue() );

    // Is the Particle Aspect-Locked?
    if ( mFixedAspect )
    {
        // Yes, so simply copy Size-X.
        pParticleNode->mRenderSize.y = pParticleNode->mRenderSize.x;
    }
    else
    {
        // No, so Scale Size-Y.
        pParticleNode->mRenderSize.y = mClampF(    pParticleNode->mSize.y * mSizeY.GraphField_OverLife.getGraphValue( particleAge ),
                                                    mSizeY.GraphField_Base.getMinValue(),
                                                    mSizeY.GraphField_Base.getMaxValue() );
    }


    // **********************************************************************************************************************
    // Scale Speed.
    // **********************************************************************************************************************
    pParticleNode->mRenderSpeed = mClampF(  pParticleNode->mSpeed * mSpeed.GraphField_OverLife.getGraphValue( particleAge ),
                                            mSpeed.GraphField_Base.getMinValue(),
                                            mSpeed.GraphField_Base.getMaxValue() );


    // **********************************************************************************************************************
    // Scale Spin (if Keep Aligned is not selected)
    // **********************************************************************************************************************
    if ( !(mParticleOrientationMode == ALIGNED && mAlign_KeepAligned) )
        pParticleNode->mRenderSpin = pParticleNode->mSpin * mSpin.GraphField_OverLife.getGraphValue( particleAge );



    // **********************************************************************************************************************
    // Scale Fixed-Force.
    // **********************************************************************************************************************
    pParticleNode->mRenderFixedForce = mClampF( pParticleNode->mFixedForce * mFixedForce.GraphField_OverLife.getGraphValue( particleAge ),
                                                mFixedForce.GraphField_Base.getMinValue(),
                                                mFixedForce.GraphField_Base.getMaxValue() );


    // **********************************************************************************************************************
    // Scale Random-Motion.
    // **********************************************************************************************************************
    pParticleNode->mRenderRandomMotion = mClampF(   pParticleNode->mRandomMotion * mRandomMotion.GraphField_OverLife.getGraphValue( particleAge ),
                                                    mRandomMotion.GraphField_Base.getMinValue(),
                                                    mRandomMotion.GraphField_Base.getMaxValue() );


    // **********************************************************************************************************************
    // Scale Colour.
    // **********************************************************************************************************************

    // Red.
    pParticleNode->mColour.red = mClampF(   mColourRed.GraphField_OverLife.getGraphValue( particleAge ),
                                            mColourRed.GraphField_OverLife.getMinValue(),
                                            mColourRed.GraphField_OverLife.getMaxValue() );

    // Green.
    pParticleNode->mColour.green = mClampF( mColourGreen.GraphField_OverLife.getGraphValue( particleAge ),
                                            mColourGreen.GraphField_OverLife.getMinValue(),
                                            mColourGreen.GraphField_OverLife.getMaxValue() );

    // Blue.
    pParticleNode->mColour.blue = mClampF(  mColourBlue.GraphField_OverLife.getGraphValue( particleAge ),
                                            mColourBlue.GraphField_OverLife.getMinValue(),
                                            mColourBlue.GraphField_OverLife.getMaxValue() );

    // Alpha.
    pParticleNode->mColour.alpha = mClampF( mVisibility.GraphField_OverLife.getGraphValue( particleAge ) * pParentEffectObject->mVisibility.GraphField_Base.getGraphValue( particleAge ),
                                            mVisibility.GraphField_OverLife.getMinValue(),
                                            mVisibility.GraphField_OverLife.getMaxValue() );




    // **********************************************************************************************************************
    // Integrate Particle.
    // **********************************************************************************************************************


    // Update Animation Controller (if used).
    if ( !mStaticMode )
        pParticleNode->mAnimationController.updateAnimation( elapsedTime );


    // **********************************************************************************************************************
    // Calculate New Velocity...
    // **********************************************************************************************************************

    // Only Calculate Velocity if not a Single Particle.
    if ( !mSingleParticle )
    {
        // Calculate Random Motion (if we've got any).
        if ( mNotZero( pParticleNode->mRenderRandomMotion ) )
        {
            // Fetch Random Motion.
            F32 randomMotion = pParticleNode->mRenderRandomMotion * 0.5f;
            // Add Time-Integrated Random-Motion into Velocity.
            pParticleNode->mVelocity += elapsedTime * Vector2( CoreMath::mGetRandomF(-randomMotion, randomMotion), CoreMath::mGetRandomF(-randomMotion, randomMotion) );
        }

        // Add Time-Integrated Fixed-Force into Velocity ( if we've got any ).
        if ( mNotZero( pParticleNode->mRenderFixedForce ) )
            pParticleNode->mVelocity += (mFixedForceDirection * pParticleNode->mRenderFixedForce * elapsedTime);

        // Suppress Movement?
        if ( !pParticleNode->mSuppressMovement )
        {
            // No, so adjust Particle Position.
            pParticleNode->mPosition += (pParticleNode->mVelocity * pParticleNode->mRenderSpeed * elapsedTime);
        }

    }


    // **********************************************************************************************************************
    // Are we Aligning to motion?
    // **********************************************************************************************************************
    if ( mParticleOrientationMode == ALIGNED && mAlign_KeepAligned )
    {
        // Yes, so calculate last movement direction.
        F32 movementAngle = mRadToDeg( mAtan( pParticleNode->mVelocity.x, -pParticleNode->mVelocity.y ) );
        // Adjust for Negative ArcTan-Quadrants.
        if ( movementAngle < 0.0f )
            movementAngle += 360.0f;

        // Set new Orientation Angle.
        pParticleNode->mOrientationAngle = -movementAngle - mAlign_AngleOffset;


        // **********************************************************************************************************************
        // Calculate Local Clip-Boundary.
        // **********************************************************************************************************************

        // Calculate Rotation Matrix.
        pParticleNode->mRotationTransform.Set( pParticleNode->mPosition, mDegToRad(pParticleNode->mOrientationAngle) );
    }
    else
    {
        // Have we got some Spin?
        if ( mNotZero(pParticleNode->mRenderSpin) )
        {
            // Yes, so add into Orientation.
            pParticleNode->mOrientationAngle += pParticleNode->mRenderSpin * elapsedTime;
            // Keep within range.
            pParticleNode->mOrientationAngle = mFmod( pParticleNode->mOrientationAngle, 360.0f );
        }


        // If the size has changed or we have some Spin then we need to recalculate the Local Clip-Boundary.
        if ( mNotZero(pParticleNode->mRenderSpin) || pParticleNode->mRenderSize != pParticleNode->mLastRenderSize )
        {
            // **********************************************************************************************************************
            // Calculate Local Clip-Boundary.
            // **********************************************************************************************************************

            // Calculate Rotation Matrix.
            pParticleNode->mRotationTransform.Set( pParticleNode->mPosition, mDegToRad(pParticleNode->mOrientationAngle) );
        }

        // We've dealt with a potential Size change so store current size for next time.
        pParticleNode->mLastRenderSize = pParticleNode->mRenderSize;
    }

    // Calculate Clip Boundary.
    Vector2 renderSize = pParticleNode->mRenderSize;
    Vector2 clipBoundary[4];
    clipBoundary[0] = mGlobalClipBoundary[0] * renderSize;
    clipBoundary[1] = mGlobalClipBoundary[1] * renderSize;
    clipBoundary[2] = mGlobalClipBoundary[2] * renderSize;
    clipBoundary[3] = mGlobalClipBoundary[3] * renderSize;
    CoreMath::mCalculateOOBB( clipBoundary, pParticleNode->mRotationTransform, pParticleNode->mOOBB );


    // **********************************************************************************************************************
    // Set Post Tick Position.
    // **********************************************************************************************************************
    pParticleNode->mPostTickPosition = pParticleNode->mPosition;
}

//------------------------------------------------------------------------------

void ParticleEmitter::integrateObject( const F32 totalTime, const F32 elapsedTime, DebugStats* pDebugStats )
{
    // **********************************************************************************************************************
    //
    // Integrate Particles.
    //
    // **********************************************************************************************************************

    // Fetch First Particle Node.
    tParticleNode* pParticleNode = mParticleNodeHead.mNextNode;

    // Process All particle nodes.
    while ( pParticleNode != &mParticleNodeHead )
    {
        // Update Particle Life.
        pParticleNode->mParticleAge += elapsedTime;

        // Has the particle expired?
        // NOTE:-   If we're in Single-Particle mode then the particle
        //          lives as long as the emitter does!
        if ( (!mSingleParticle && pParticleNode->mParticleAge > pParticleNode->mParticleLifetime) || (pParticleNode->mParticleLifetime == 0.0f) )
        {
            // Yes, so fetch next particle before we kill it.
            pParticleNode = pParticleNode->mNextNode;

            // Kill Particle.
            // NOTE:-   Because we move to the next particle,
            //          the particle to kill is now the previous!
            freeParticle( pParticleNode->mPreviousNode );
        }
        else
        {
            // Integrate Particle.
            integrateParticle( pParticleNode, pParticleNode->mParticleAge / pParticleNode->mParticleLifetime, elapsedTime );

            // **********************************************************************************************************************
            // Move to next Particle Node.
            // **********************************************************************************************************************
            pParticleNode = pParticleNode->mNextNode;
        }
    };


    // **********************************************************************************************************************
    //
    // Generate New Particles.
    //
    // **********************************************************************************************************************

    // Only Generate particles if we're not pause.
    if ( !mPauseEmitter )
    {
        // Are we in Single-Particle Mode?
        if ( mSingleParticle )
        {
            // Yes, so do we have a single particle yet?
            if ( mParticleNodeHead.mNextNode == &mParticleNodeHead )
            {
                // No, so generate Single Particle.
                createParticle();
            }
        }
        else
        {
            // No, so fetch Effect Age.
            F32 effectAge = pParentEffectObject->mEffectAge;

            // Accumulate Last Generation Time as we need to handle very small time-integrations correctly.
            //
            // NOTE:-   We need to do this if there's an emission target but the
            //          time-integration is so small that rounding results in
            //          no emission.  Downside to good FPS!
            mTimeSinceLastGeneration += elapsedTime;

            // Calculate Local Emission Quantity.
            const F32 baseEmission = mQuantity.GraphField_Base.getGraphValue( effectAge );
            const F32 varEmission = mQuantity.GraphField_Variation.getGraphValue( effectAge ) * 0.5f;
            const F32 effectEmission = pParentEffectObject->mQuantity.GraphField_Base.getGraphValue( effectAge ) * mParticlePref;

            const F32 localEmission = mClampF(    (baseEmission + CoreMath::mGetRandomF(-varEmission, varEmission)) * effectEmission,
                                                  mQuantity.GraphField_Base.getMinValue(),
                                                  mQuantity.GraphField_Base.getMaxValue() );
            const U32 emission = U32(mFloor(localEmission * mTimeSinceLastGeneration));

            // Do we have an emission?
            if ( emission > 0 )
            {
                // Yes, so remove this emission from accumulated time.
                mTimeSinceLastGeneration = getMax(0.0f, mTimeSinceLastGeneration - (emission / localEmission));

                // Suppress Precision Errors.
                if ( mIsZero( mTimeSinceLastGeneration ) )
                    mTimeSinceLastGeneration = 0.0f;

                // Generate Emission.
                for ( U32 n = 0; n < emission; n++ )
                    createParticle();
            }
            // No, so was there a calculated emission?
            else if ( localEmission == 0 )
            {
                // No, so reset accumulated time.
                //mTimeSinceLastGeneration = 0.0f;
            }
        }
    }
}

//------------------------------------------------------------------------------

void ParticleEmitter::interpolateObject( const F32 timeDelta )
{
    // **********************************************************************************************************************
    //
    // Interpolate Particles.
    //
    // **********************************************************************************************************************

    // Fetch First Particle Node.
    tParticleNode* pParticleNode = mParticleNodeHead.mNextNode;

    // Process All particle nodes.
    while ( pParticleNode != &mParticleNodeHead )
    {
        // Interpolate Particle.
        pParticleNode->mRenderTickPosition = (timeDelta * pParticleNode->mPreTickPosition) + ((1.0f-timeDelta) * pParticleNode->mPostTickPosition);

        pParticleNode->mRotationTransform.p = pParticleNode->mRenderTickPosition;

        Vector2 renderSize = pParticleNode->mRenderSize;
        Vector2 clipBoundary[4];
        clipBoundary[0] = mGlobalClipBoundary[0] * renderSize;
        clipBoundary[1] = mGlobalClipBoundary[1] * renderSize;
        clipBoundary[2] = mGlobalClipBoundary[2] * renderSize;
        clipBoundary[3] = mGlobalClipBoundary[3] * renderSize;

        // Calculate Clip Boundary.
        CoreMath::mCalculateOOBB( clipBoundary, pParticleNode->mRotationTransform, pParticleNode->mOOBB );

        // Move to next Particle Node.
        pParticleNode = pParticleNode->mNextNode;
    }
}

//------------------------------------------------------------------------------

void ParticleEmitter::sceneRender( const SceneRenderState* pSceneRenderState, const SceneRenderRequest* pSceneRenderRequest, BatchRender* pBatchRenderer )
{
    // No point in going further if there's no particles active!
    if ( mActiveParticles == 0 )
        return;

    // Cannot Render without Animation/ImageMap.
    if ( mStaticMode )
    {
        if ( mImageAsset.isNull() )
            return;
    }
    else
    {
        if ( mAnimationAsset.isNull() )
            return;
    }

    // Flush.
    pBatchRenderer->flush( getParentEffect()->getScene()->getDebugStats().batchIsolatedFlush );

    // Intense Particles?
    if ( mIntenseParticles )
    {
        pBatchRenderer->setBlendMode( GL_SRC_ALPHA, GL_ONE );
    }
    else
    {
        // No, so set standard blend options.
        if ( mBlendMode )
        {
            pBatchRenderer->setBlendMode( mSrcBlendFactor, mDstBlendFactor );
        }
        else
        {
            pBatchRenderer->setBlendOff();
        }
    }

    // Alpha-testing is off.
    pBatchRenderer->setAlphaTestMode( -1.0f );

    // Is the Position attached to the emitter?
    if ( mAttachPositionToEmitter )
    {
        // Yes, so get Effect Position.
        const Vector2 effectPos = pParentEffectObject->getPosition();
        // Move into Emitter-Space.
        glTranslatef( effectPos.x, effectPos.y, 0.0f );

        // Is the Rotation attached to the emitter?
        if ( mAttachRotationToEmitter )
        {
            // Yes, so rotate into Emitter-Space.
            // NOTE:- We need clockwise rotation here.
            glRotatef( pParentEffectObject->getRenderAngle(), 0.0f, 0.0f, 1.0f );
        }
    }

    // Frame texture.
    TextureHandle frameTexture;

    // Frame area.
    ImageAsset::FrameArea::TexelArea texelFrameArea;

    // Are we using an ImageMap?
    if ( mStaticMode )
    {
        // Yes, so fetch frame area.
        texelFrameArea = mImageAsset->getImageFrameArea( mImageMapFrame ).mTexelArea;
        frameTexture = mImageAsset->getImageTexture();
    }

    // Fetch First Particle ( using appropriate sort-order ).
    tParticleNode* pParticleNode = mOrderedParticles | mFirstInFrontOrder ? mParticleNodeHead.mNextNode : mParticleNodeHead.mPreviousNode;

    // Process All particle nodes.
    while ( pParticleNode != &mParticleNodeHead )
    {
        // Are we using an Animation?
        if ( !mStaticMode )
        {
            // Yes, so fetch current frame area.
            texelFrameArea = pParticleNode->mAnimationController.getCurrentImageFrameArea().mTexelArea;
            frameTexture = pParticleNode->mAnimationController.getImageTexture();
        }

        Vector2* renderOOBB = pParticleNode->mOOBB;

        // Fetch lower/upper texture coordinates.
        const Vector2& texLower = texelFrameArea.mTexelLower;
        const Vector2& texUpper = texelFrameArea.mTexelUpper;

        // Submit batched quad.
        pBatchRenderer->SubmitQuad(
            renderOOBB[0],
            renderOOBB[1],
            renderOOBB[2],
            renderOOBB[3],
            Vector2( texLower.x, texUpper.y ),
            Vector2( texUpper.x, texUpper.y ),
            Vector2( texUpper.x, texLower.y ),
            Vector2( texLower.x, texLower.y ),
            frameTexture,
            pParticleNode->mColour );

        // Move to next Particle ( using appropriate sort-order ).
        pParticleNode = mOrderedParticles | mFirstInFrontOrder ? pParticleNode->mNextNode : pParticleNode->mPreviousNode;
    };
}

//------------------------------------------------------------------------------

void ParticleEmitter::clearGraphSelections( void )
{
    // Destroy Graph Selections.
    for ( U32 n = 0; n < (U32)mGraphSelectionList.size(); n++ )
        delete mGraphSelectionList[n];

    // Clear List.
    mGraphSelectionList.clear();
}

//------------------------------------------------------------------------------

void ParticleEmitter::addGraphSelection( const char* graphName, GraphField* pGraphObject )
{
    // Generate new Graph Selection.
    tGraphSelection* pGraphSelection = new tGraphSelection;

    // Populate Graph Selection.
    pGraphSelection->mGraphName = StringTable->insert( graphName );
    pGraphSelection->mpGraphObject = pGraphObject;

    // Put into Graph Selection List.
    mGraphSelectionList.push_back( pGraphSelection );
}

//------------------------------------------------------------------------------

GraphField* ParticleEmitter::findGraphSelection( const char* graphName ) const
{
    // Search For Selected Graph and return if found.
    for ( U32 n = 0; n < (U32)mGraphSelectionList.size(); n++ )
        if ( mGraphSelectionList[n]->mGraphName == StringTable->insert(graphName) )
            return mGraphSelectionList[n]->mpGraphObject;

    // Return "Not Found".
    return NULL;
}

//------------------------------------------------------------------------------

void ParticleEmitter::initialise( ParticleEffect* pParentEffect )
{
    // Set Parent Effect.
    pParentEffectObject = pParentEffect;

    // ****************************************************************************
    // Initialise Graph Selections.
    // ****************************************************************************
    addGraphSelection( "particlelife_base", &mParticleLife.GraphField_Base );
    addGraphSelection( "particlelife_var", &mParticleLife.GraphField_Variation );

    addGraphSelection( "quantity_base", &mQuantity.GraphField_Base );
    addGraphSelection( "quantity_var", &mQuantity.GraphField_Variation );

    addGraphSelection( "sizex_base", &mSizeX.GraphField_Base );
    addGraphSelection( "sizex_var", &mSizeX.GraphField_Variation );
    addGraphSelection( "sizex_life", &mSizeX.GraphField_OverLife );

    addGraphSelection( "sizey_base", &mSizeY.GraphField_Base );
    addGraphSelection( "sizey_var", &mSizeY.GraphField_Variation );
    addGraphSelection( "sizey_life", &mSizeY.GraphField_OverLife );

    addGraphSelection( "speed_base", &mSpeed.GraphField_Base );
    addGraphSelection( "speed_var", &mSpeed.GraphField_Variation );
    addGraphSelection( "speed_life", &mSpeed.GraphField_OverLife );

    addGraphSelection( "spin_base", &mSpin.GraphField_Base );
    addGraphSelection( "spin_var", &mSpin.GraphField_Variation );
    addGraphSelection( "spin_life", &mSpin.GraphField_OverLife );

    addGraphSelection( "fixedforce_base", &mFixedForce.GraphField_Base );
    addGraphSelection( "fixedforce_var", &mFixedForce.GraphField_Variation );
    addGraphSelection( "fixedforce_life", &mFixedForce.GraphField_OverLife );

    addGraphSelection( "randommotion_base", &mRandomMotion.GraphField_Base );
    addGraphSelection( "randommotion_var", &mRandomMotion.GraphField_Variation );
    addGraphSelection( "randommotion_life", &mRandomMotion.GraphField_OverLife );

    addGraphSelection( "emissionforce_base", &mEmissionForce.GraphField_Base );
    addGraphSelection( "emissionforce_var", &mEmissionForce.GraphField_Variation );

    addGraphSelection( "emissionangle_base", &mEmissionAngle.GraphField_Base );
    addGraphSelection( "emissionangle_var", &mEmissionAngle.GraphField_Variation );

    addGraphSelection( "emissionarc_base", &mEmissionArc.GraphField_Base );
    addGraphSelection( "emissionarc_var", &mEmissionArc.GraphField_Variation );

    addGraphSelection( "red_life", &mColourRed.GraphField_OverLife );
    addGraphSelection( "green_life", &mColourGreen.GraphField_OverLife );
    addGraphSelection( "blue_life", &mColourBlue.GraphField_OverLife );
    addGraphSelection( "visibility_life", &mVisibility.GraphField_OverLife );



    // ****************************************************************************
    // Initialise Graphs.
    // ****************************************************************************
    mParticleLife.GraphField_Base.setValueBounds( 1000.0f, 0.0f, 1000.0f, 2.0f );
    mParticleLife.GraphField_Variation.setValueBounds( 1000.0f, 0.0f, 2000.0f, 0.0f );

    mQuantity.GraphField_Base.setValueBounds( 1000.0f, 0.0f, 1000.0f, 10.0f );
    mQuantity.GraphField_Variation.setValueBounds( 1000.0f, 0.0f, 1000.0f, 0.0f );

    mSizeX.GraphField_Base.setValueBounds( 1000.0f, 0.0f, 100.0f, 2.0f );
    mSizeX.GraphField_Variation.setValueBounds( 1000.0f, 0.0f, 200.0f, 0.0f );
    mSizeX.GraphField_OverLife.setValueBounds( 1.0f, -100.0f, 100.0f, 1.0f );

    mSizeY.GraphField_Base.setValueBounds( 1000.0f, 0.0f, 100.0f, 2.0f );
    mSizeY.GraphField_Variation.setValueBounds( 1000.0f, 0.0f, 200.0f, 0.0f );
    mSizeY.GraphField_OverLife.setValueBounds( 1.0f, -100.0f, 100.0f, 1.0f );

    mSpeed.GraphField_Base.setValueBounds( 1000.0f, 0.0f, 100.0f, 10.0f );
    mSpeed.GraphField_Variation.setValueBounds( 1000.0f, 0.0f, 200.0f, 0.0f );
    mSpeed.GraphField_OverLife.setValueBounds( 1.0f, -100.0f, 100.0f, 1.0f );

    mSpin.GraphField_Base.setValueBounds( 1000.0f, -1000.0f, 1000.0f, 0.0f );
    mSpin.GraphField_Variation.setValueBounds( 1000.0f, 0.0f, 2000.0f, 0.0f );
    mSpin.GraphField_OverLife.setValueBounds( 1.0f, -1000.0f, 1000.0f, 1.0f );

    mFixedForce.GraphField_Base.setValueBounds( 1000.0f, -100.0f, 100.0f, 0.0f );
    mFixedForce.GraphField_Variation.setValueBounds( 1000.0f, 0.0f, 200.0f, 0.0f );
    mFixedForce.GraphField_OverLife.setValueBounds( 1.0f, -100.0f, 100.0f, 1.0f );

    mRandomMotion.GraphField_Base.setValueBounds( 1000.0f, 0.0f, 100.0f, 0.0f );
    mRandomMotion.GraphField_Variation.setValueBounds( 1000.0f, 0.0f, 200.0f, 0.0f );
    mRandomMotion.GraphField_OverLife.setValueBounds( 1.0f, -100.0f, 100.0f, 1.0f );

    mEmissionForce.GraphField_Base.setValueBounds( 1000.0f, -100.0f, 100.0f, 5.0f );
    mEmissionForce.GraphField_Variation.setValueBounds( 1000.0f, 0.0f, 200.0f, 0.0f );

    mEmissionAngle.GraphField_Base.setValueBounds( 1000.0f, -180.0f, 180.0f, 0.0f );
    mEmissionAngle.GraphField_Variation.setValueBounds( 1000.0f, 0.0f, 360.0f, 0.0f );

    mEmissionArc.GraphField_Base.setValueBounds( 1000.0f, 0.0f, 360.0f, 360.0f );
    mEmissionArc.GraphField_Variation.setValueBounds( 1000.0f, 0.0f, 720.0f, 0.0f );

    mColourRed.GraphField_OverLife.setValueBounds( 1.0f, 0.0f, 1.0f, 1.0f );
    mColourGreen.GraphField_OverLife.setValueBounds( 1.0f, 0.0f, 1.0f, 1.0f );
    mColourBlue.GraphField_OverLife.setValueBounds( 1.0f, 0.0f, 1.0f, 1.0f );
    mVisibility.GraphField_OverLife.setValueBounds( 1.0f, 0.0f, 1.0f, 1.0f );


    // Set Other Properties.
    mFixedAspect = true;
    setFixedForceAngle(0.0f);
    mParticleOrientationMode = FIXED;
    mFixed_AngleOffset = 0.0f;
    mAlign_AngleOffset = 0.0f;
    mAlign_KeepAligned = false;
    mRandom_AngleOffset = 0.0f;
    mRandom_Arc = 360.0f;
    mEmitterType = POINT;

    // Other Properties.
    setPivotPoint( Vector2(0.5f, 0.5f) );
    mStaticMode = true;
    mImageAsset.clear();
    mAnimationAsset.clear();
    mUseEffectEmission = true;
    mLinkEmissionRotation = false;
    mIntenseParticles = false;
    mSingleParticle = false;
    mAttachPositionToEmitter = false;
    mAttachRotationToEmitter = false;
    mOrderedParticles = false;
    mFirstInFrontOrder = false;

    // Rendering Options.
    mBlendMode = true;
    mSrcBlendFactor = GL_SRC_ALPHA;
    mDstBlendFactor = GL_ONE_MINUS_SRC_ALPHA;
}

//------------------------------------------------------------------------------

void ParticleEmitter::selectGraph( const char* graphName )
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
        Con::warnf( "ParticleEmitter::selectGraph() - Invalid Graph Selected! (%s)", graphName );
        return;
    }
}

//------------------------------------------------------------------------------

S32 ParticleEmitter::addDataKey( F32 time, F32 value )
{
    // Have we got a valid Graph Selected?
    if ( !mpCurrentGraph )
    {
        // No, so warn.
        Con::warnf( "ParticleEmitter::addDataKey() - No Graph Selected!" );
        return -1;
    }

    // Add Data Key.
    return mpCurrentGraph->addDataKey( time, value );
}

//------------------------------------------------------------------------------

bool ParticleEmitter::removeDataKey( S32 index )
{
    // Have we got a valid Graph Selected?
    if ( !mpCurrentGraph )
    {
        // No, so warn.
        Con::warnf( "ParticleEmitter::removeDataKey() - No Graph Selected!" );
        return false;
    }

    // Remove Data Key.
    return mpCurrentGraph->removeDataKey( index );
}

//------------------------------------------------------------------------------

bool ParticleEmitter::clearDataKeys( void )
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

//------------------------------------------------------------------------------

bool ParticleEmitter::setDataKeyValue( S32 index, F32 value )
{
    // Have we got a valid Graph Selected?
    if ( !mpCurrentGraph )
    {
        // No, so warn.
        Con::warnf( "ParticleEmitter::setDataKeyValue() - No Graph Selected!" );
        return false;
    }

    // Set Data Key Value.
    return mpCurrentGraph->setDataKeyValue( index, value );
}

//------------------------------------------------------------------------------

F32 ParticleEmitter::getDataKeyValue( S32 index ) const
{
    // Have we got a valid Graph Selected?
    if ( !mpCurrentGraph )
    {
        // No, so warn.
        Con::warnf( "ParticleEmitter::getDataKeyValue() - No Graph Selected!" );
        return false;
    }

    // Get Data Key Value.
    return mpCurrentGraph->getDataKeyValue( index );
}

//------------------------------------------------------------------------------

F32 ParticleEmitter::getDataKeyTime( S32 index ) const
{
    // Have we got a valid Graph Selected?
    if ( !mpCurrentGraph )
    {
        // No, so warn.
        Con::warnf( "ParticleEmitter::getDataKeyTime() - No Graph Selected!" );
        return false;
    }

    // Get Data Key Time.
    return mpCurrentGraph->getDataKeyTime( index );
}

//------------------------------------------------------------------------------

U32 ParticleEmitter::getDataKeyCount( void ) const
{
    // Have we got a valid Graph Selected?
    if ( !mpCurrentGraph )
    {
        // No, so warn.
        Con::warnf( "ParticleEmitter::getDataKeyCount() - No Graph Selected!" );
        return false;
    }

    // Get Data Key Count.
    return mpCurrentGraph->getDataKeyCount();
}

//------------------------------------------------------------------------------

F32 ParticleEmitter::getMinValue( void ) const
{
    // Have we got a valid Graph Selected?
    if ( !mpCurrentGraph )
    {
        // No, so warn.
        Con::warnf( "ParticleEmitter::getMinValue() - No Graph Selected!" );
        return false;
    }

    // Get Min Value.
    return mpCurrentGraph->getMinValue();
}

//------------------------------------------------------------------------------

F32 ParticleEmitter::getMaxValue( void ) const
{
    // Have we got a valid Graph Selected?
    if ( !mpCurrentGraph )
    {
        // No, so warn.
        Con::warnf( "ParticleEmitter::getMaxValue() - No Graph Selected!" );
        return false;
    }

    // Get Max Value.
    return mpCurrentGraph->getMaxValue();
}

//------------------------------------------------------------------------------

F32 ParticleEmitter::getMinTime( void ) const
{
    // Have we got a valid Graph Selected?
    if ( !mpCurrentGraph )
    {
        // No, so warn.
        Con::warnf( "ParticleEmitter::getMinTime() - No Graph Selected!" );
        return false;
    }

    // Get Min Time.
    return mpCurrentGraph->getMinTime();
}

//------------------------------------------------------------------------------

F32 ParticleEmitter::getMaxTime( void ) const
{
    // Have we got a valid Graph Selected?
    if ( !mpCurrentGraph )
    {
        // No, so warn.
        Con::warnf( "ParticleEmitter::getMaxTime() - No Graph Selected!" );
        return false;
    }

    // Get Max Time.
    return mpCurrentGraph->getMaxTime();
}

//------------------------------------------------------------------------------

F32 ParticleEmitter::getGraphValue( F32 time ) const
{
    // Have we got a valid Graph Selected?
    if ( !mpCurrentGraph )
    {
        // No, so warn.
        Con::warnf( "ParticleEmitter::getGraphValue() - No Graph Selected!" );
        return false;
    }

    // Get Graph Value.
    return mpCurrentGraph->getGraphValue( time );
}

//------------------------------------------------------------------------------

bool ParticleEmitter::setTimeRepeat( F32 timeRepeat )
{
    // Have we got a valid Graph Selected?
    if ( !mpCurrentGraph )
    {
        // No, so warn.
        Con::warnf( "ParticleEmitter::setTimeRepeat() - No Graph Selected!" );
        return false;
    }

    // Set Time Repeat.
    return mpCurrentGraph->setTimeRepeat( timeRepeat );
}

//------------------------------------------------------------------------------

F32 ParticleEmitter::getTimeRepeat( void ) const
{
    // Have we got a valid Graph Selected?
    if ( !mpCurrentGraph )
    {
        // No, so warn.
        Con::warnf( "ParticleEmitter::getTimeRepeat() - No Graph Selected!" );
        return false;
    }

    // Get Time Repeat.
    return mpCurrentGraph->getTimeRepeat();
}

//------------------------------------------------------------------------------

bool ParticleEmitter::setValueScale( const F32 valueScale )
{
    // Have we got a valid Graph Selected?
    if ( !mpCurrentGraph )
    {
        // No, so warn.
        Con::warnf( "ParticleEmitter::setValueScale() - No Graph Selected!" );
        return false;
    }

    // Set Value Scale.
    return mpCurrentGraph->setValueScale( valueScale );
}

//------------------------------------------------------------------------------

F32 ParticleEmitter::getValueScale( void ) const
{
    // Have we got a valid Graph Selected?
    if ( !mpCurrentGraph )
    {
        // No, so warn.
        Con::warnf( "ParticleEmitter::getValueScale() - No Graph Selected!" );
        return false;
    }

    // Get Value Scale.
    return mpCurrentGraph->getValueScale();
}

//------------------------------------------------------------------------------

void ParticleEmitter::setEmitterVisible( bool status )
{
    // Set Emitter Visibility.
    mEmitterVisible = status;

    // Free All Particles ( if making invisible ).
    if ( !mEmitterVisible )
        freeAllParticles();
}

//------------------------------------------------------------------------------

void ParticleEmitter::setEmitterName( const char* emitterName )
{
    // Set Emitter Name.
    mEmitterName = StringTable->insert(emitterName);
}

//------------------------------------------------------------------------------

void ParticleEmitter::setFixedAspect( bool aspect )
{
    // Sets Fixed Aspect.
    mFixedAspect = aspect;
}

//------------------------------------------------------------------------------

void ParticleEmitter::setFixedForceAngle( F32 fixedForceAngle )
{
    // Set Fixed-Force Angle.
    mFixedForceAngle = fixedForceAngle;

    // Set Fixed-Force Direction.
    mFixedForceDirection.Set( mSin(mDegToRad(mFixedForceAngle)), mCos(mDegToRad(mFixedForceAngle)) );
}

//------------------------------------------------------------------------------

void ParticleEmitter::setParticleOrientationMode( tEmitterOrientationMode particleOrientationMode )
{
    // Set Particle Orientation Mode.
    mParticleOrientationMode = particleOrientationMode;
}

//------------------------------------------------------------------------------

void ParticleEmitter::setAlignAngleOffset( F32 angleOffset )
{
    // Set Align Angle Offset.
    mAlign_AngleOffset = angleOffset;
}

//------------------------------------------------------------------------------

void ParticleEmitter::setAlignKeepAligned( bool keepAligned )
{
    // Set Align Keep Aligned.
    mAlign_KeepAligned = keepAligned;
}

//------------------------------------------------------------------------------

void ParticleEmitter::setRandomAngleOffset( F32 angleOffset )
{
    // Set Random Angle.
    mRandom_AngleOffset = angleOffset;
}

//------------------------------------------------------------------------------

void ParticleEmitter::setRandomArc( F32 arc )
{
    // Set Random Arc.
    mRandom_Arc = arc;
}

//------------------------------------------------------------------------------

void ParticleEmitter::setFixedAngleOffset( F32 angleOffset )
{
    // Set Fixed Angle Offset.
    mFixed_AngleOffset = angleOffset;
}

//------------------------------------------------------------------------------

void ParticleEmitter::setEmitterType( tEmitterType emitterType )
{
    // Set Emitter Type.
    mEmitterType = emitterType;
}

//------------------------------------------------------------------------------

bool ParticleEmitter::setImageMap( const char* pImageMapAssetId, U32 frame )
{
    // Sanity!
    AssertFatal( pImageMapAssetId != NULL, "Cannot use a NULL asset Id." );

    // Set static mode.
    mStaticMode = true;

    // Clear animation asset.
    mAnimationAsset.clear();
    mAnimationControllerProxy.clearAssets();

    // Set asset Id.
    mImageAsset = pImageMapAssetId;

    // Finish if no asset.
    if ( mImageAsset.isNull() )
        return false;

    // Check Frame Validity.
    if ( frame >= mImageAsset->getFrameCount() )
    {
        // Warn.
        Con::warnf("ParticleEmitter::setImageMap() - Invalid Frame #%d for ImageAsset Datablock! (%s)", frame, mImageAsset.getAssetId() );
        // Return Here.
        return false;
    }

    // Set Frame.
    mImageMapFrame = frame;

    // Free All Particles.
    freeAllParticles();

    // Return Okay.
    return true;
}

//------------------------------------------------------------------------------

bool ParticleEmitter::setAnimation( const char* pAnimationAssetId )
{
    // Sanity!
    AssertFatal( pAnimationAssetId != NULL, "Cannot use NULL asset Id." );

    // Set animated mode.
    mStaticMode = false;

    // Clear static asset.
    mImageAsset.clear();

    // Set animation asset.
    mAnimationAsset = pAnimationAssetId;

    // Attempt to Play Animation.
    // NOTE:-   This animation controller is used as a proxy simply to test
    //          validity of the animationName that's passed.  Other than that,
    //          it's not used for rendering particles.
    if ( !mAnimationControllerProxy.playAnimation( mAnimationAsset, false ) )
    {
        mAnimationAsset.clear();
        mAnimationControllerProxy.clearAssets();
        return false;
    }

    return true;
}

//------------------------------------------------------------------------------

void ParticleEmitter::setPivotPoint( Vector2 pivotPoint )
{
    // Set Pivot-Point.
    mPivotPoint.x = pivotPoint.x;
    mPivotPoint.y = pivotPoint.y;

    // Calculate Global Clip Boundary.
    mGlobalClipBoundary[0].Set( -mPivotPoint.x, -mPivotPoint.y );
    mGlobalClipBoundary[1].Set( 1.0f-mPivotPoint.x, -mPivotPoint.y );
    mGlobalClipBoundary[2].Set( 1.0f-mPivotPoint.x, 1.0f-mPivotPoint.y );
    mGlobalClipBoundary[3].Set( -mPivotPoint.x, 1.0f-mPivotPoint.y );
}

//------------------------------------------------------------------------------

void ParticleEmitter::setUseEffectEmission( bool useEffectEmission )
{
    // Set Use Effect Emission.
    mUseEffectEmission = useEffectEmission;
}

//------------------------------------------------------------------------------

void ParticleEmitter::setLinkEmissionRotation( bool linkEmissionRotation )
{
    // Set Link Emission Rotation.
    mLinkEmissionRotation = linkEmissionRotation;
}

//------------------------------------------------------------------------------

void ParticleEmitter::setIntenseParticles( bool intenseParticles )
{
    // Set Intense Particles.
    mIntenseParticles = intenseParticles;
}

//------------------------------------------------------------------------------

void ParticleEmitter::setSingleParticle( bool singleParticle )
{
    // Set Single Particle.
    mSingleParticle = singleParticle;

    // Free All Particles.
    freeAllParticles();
}

//------------------------------------------------------------------------------

void ParticleEmitter::setAttachPositionToEmitter( bool attachPositionToEmitter )
{
    // Do we need to attach to the emitter?
    if ( attachPositionToEmitter && !mAttachPositionToEmitter )
    {
        // Yes, so we need to translate the position into Local Effect-Space...

        // Get Parent Effect Position.
        const Vector2 effectPos = pParentEffectObject->getPosition();

        // Fetch First Particle.
        tParticleNode* pParticleNode = mParticleNodeHead.mNextNode;

        // Process All particle nodes.
        while ( pParticleNode != &mParticleNodeHead )
        {
            // Move into Local Effect-Space
            pParticleNode->mPosition -= effectPos;

            // Move to next Particle.
            pParticleNode = pParticleNode->mNextNode;
        };

    }
    else if ( !attachPositionToEmitter && mAttachPositionToEmitter )
    {
        // Yes, so we need to translate the positions permanently into World-Space...

        // Get Parent Effect Position.
        const Vector2 effectPos = pParentEffectObject->getPosition();

        // Fetch First Particle.
        tParticleNode* pParticleNode = mParticleNodeHead.mNextNode;

        // Process All particle nodes.
        while ( pParticleNode != &mParticleNodeHead )
        {
            // Move into World-Space.
            pParticleNode->mPosition += effectPos;

            // Move to next Particle.
            pParticleNode = pParticleNode->mNextNode;
        };
    }

    // Set Attach Position To Emitter.
    mAttachPositionToEmitter = attachPositionToEmitter;
}

//------------------------------------------------------------------------------

void ParticleEmitter::setAttachRotationToEmitter( bool attachRotationToEmitter )
{
    // Set Attach Rotation To Emitter.
    mAttachRotationToEmitter = attachRotationToEmitter;
}

//------------------------------------------------------------------------------

void ParticleEmitter::setFirstInFrontOrder( bool firstInFrontOrder )
{
    // Set First In Front Order.
    mFirstInFrontOrder = firstInFrontOrder;
}

//------------------------------------------------------------------------------

void ParticleEmitter::setBlending( bool status, S32 srcBlendFactor, S32 dstBlendFactor )
{
    // Set Blending Flag.
    mBlendMode = status;

    // Set Blending Factors.
    mSrcBlendFactor = srcBlendFactor;
    mDstBlendFactor = dstBlendFactor;
}

//------------------------------------------------------------------------------

bool ParticleEmitter::getEmitterVisible( void ) const
{
    // Get Emitter Visibility.
    return mEmitterVisible;
}

//------------------------------------------------------------------------------

const char* ParticleEmitter::getEmitterName(void) const
{
    // Return Emitter Name.
    return mEmitterName;
}

//------------------------------------------------------------------------------

bool ParticleEmitter::getFixedAspect( void ) const
{
    // Get Fixed Aspect.
    return mFixedAspect;
}

//------------------------------------------------------------------------------

F32 ParticleEmitter::getFixedForceAngle( void ) const
{
    // Return Fixed-Angle Direction.
    return mFixedForceAngle;
}

//------------------------------------------------------------------------------

const char* ParticleEmitter::getParticleOrientation( void ) const
{
    // Search for Mnemonic.
    for(U32 i = 0; i < (sizeof(particleOrientationLookup) / sizeof(particleOrientationLookup[0])); i++)
    {
        if( (ParticleEmitter::tEmitterOrientationMode)particleOrientationLookup[i].index == mParticleOrientationMode )
        {
            // Return Particle Orientation Description.
            return particleOrientationLookup[i].label;
        }
    }

    // Bah!
    return NULL;
}

//------------------------------------------------------------------------------

F32 ParticleEmitter::getAlignAngleOffset( void ) const
{
    // Get Align Angle Offset.
    return mAlign_AngleOffset;
}

//------------------------------------------------------------------------------

bool ParticleEmitter::getAlignKeepAligned( void ) const
{
    // Get Align Keep Aligned.
    return mAlign_KeepAligned;
}

//------------------------------------------------------------------------------

F32 ParticleEmitter::getRandomAngleOffset( void ) const
{
    // Get Random Angle Offset.
    return mRandom_AngleOffset;
}

//------------------------------------------------------------------------------

F32 ParticleEmitter::getRandomArc( void ) const
{
    // Get Random Arc.
    return mRandom_Arc;
}

//------------------------------------------------------------------------------

F32 ParticleEmitter::getFixedAngleOffset( void ) const
{
    // Get Fixed Angle Offset.
    return mFixed_AngleOffset;
}

//------------------------------------------------------------------------------

const char* ParticleEmitter::getEmitterType( void ) const
{
    // Search for Mnemonic.
    for(U32 i = 0; i < (sizeof(emitterTypeLookup) / sizeof(emitterTypeLookup[0])); i++)
    {
        if( (ParticleEmitter::tEmitterType)emitterTypeLookup[i].index == mEmitterType )
        {
            // Return Emitter Type Description.
            return emitterTypeLookup[i].label;
        }
    }

    // Bah!
    return NULL;
}

//------------------------------------------------------------------------------

const char* ParticleEmitter::getImageMapNameFrame( void ) const
{
    // Nothing to return if an animation is selected!
    if ( !mStaticMode )
        return NULL;

    // Get Console Buffer.
    char* pConBuffer = Con::getReturnBuffer(256);

    // Write ImageMap Name/Frame String.
    dSprintf( pConBuffer, 256, "%s %d", mImageAsset.getAssetId(), mImageMapFrame );

    // Return Buffer.
    return pConBuffer;
}

//------------------------------------------------------------------------------

const char* ParticleEmitter::getAnimation( void ) const
{
    // Nothing to return if an animation is NOT selected!
    if ( mStaticMode )
        return NULL;

    // Return Animation Name.
    return mAnimationAsset.getAssetId();
}

//------------------------------------------------------------------------------

const char* ParticleEmitter::getPivotPoint( void ) const
{
    // Get Console Buffer.
    char* pConBuffer = Con::getReturnBuffer(32);

    // Write ImageMap Name/Frame String.
    dSprintf( pConBuffer, 32, "%f %f", mPivotPoint.x, mPivotPoint.y );

    // Return Buffer.
    return pConBuffer;
}

//------------------------------------------------------------------------------

bool ParticleEmitter::getUseEffectEmission( void ) const
{
    // Get Use Effect Emission.
    return mUseEffectEmission;
}

//------------------------------------------------------------------------------

bool ParticleEmitter::getLinkEmissionRotation( void ) const
{
    // Get Link Emission Rotation.
    return mLinkEmissionRotation;
}

//------------------------------------------------------------------------------

bool ParticleEmitter::getIntenseParticles( void ) const
{
    // Get Intense Particles.
    return mIntenseParticles;
}

//------------------------------------------------------------------------------

bool ParticleEmitter::getSingleParticle( void ) const
{
    // Get Single Particle.
    return mSingleParticle;
}

//------------------------------------------------------------------------------

bool ParticleEmitter::getAttachPositionToEmitter( void ) const
{
    // Get Attach Position To Emitter.
    return mAttachPositionToEmitter;
}

//------------------------------------------------------------------------------

bool ParticleEmitter::getAttachRotationToEmitter( void ) const
{
    // Get Attach Rotation To Emitter.
    return mAttachRotationToEmitter;
}

//------------------------------------------------------------------------------

bool ParticleEmitter::getFirstInFrontOrder( void ) const
{
    // Get First In Front Order.
    return mFirstInFrontOrder;
}

//------------------------------------------------------------------------------

bool ParticleEmitter::getUsingAnimation( void ) const
{
    // Get 'Using Animation' Flag.
    return !mStaticMode;
}

//------------------------------------------------------------------------------

void ParticleEmitter::playEmitter( bool resetParticles )
{
    // Reset Time Since Last Generation.
    mTimeSinceLastGeneration = 0.0f;

    // Kill all particles if selected.
    if ( resetParticles )
        // Free All Particles.
        freeAllParticles();

    // Reset Pause Emitter.
    mPauseEmitter = false;
}

//------------------------------------------------------------------------------

void ParticleEmitter::stopEmitter( void )
{
    // Destroy particle pool.
    destroyParticlePool();
}

//------------------------------------------------------------------------------

void ParticleEmitter::pauseEmitter( void )
{
    // Pause Emitter.
    mPauseEmitter = true;

    // Are we in single particle mode?
    if ( mSingleParticle )
    {
        // Yes, so remove all particles now!
        freeAllParticles();
    }
}

//------------------------------------------------------------------------------

ParticleEmitter::tParticleNode* ParticleEmitter::createParticle( void )
{
    // Have we got any free particle nodes?
    if ( mpFreeParticleNodes == NULL )
        // No, so create a new block.
        createParticlePoolBlock();

    // Fetch Free Node.
    tParticleNode* pFreeParticleNode = mpFreeParticleNodes;

    // Reposition Free Node Reference.
    mpFreeParticleNodes = mpFreeParticleNodes->mNextNode;

    // Insert Particle into Head Chain.
    pFreeParticleNode->mNextNode        = mParticleNodeHead.mNextNode;
    pFreeParticleNode->mPreviousNode    = &mParticleNodeHead;
    mParticleNodeHead.mNextNode         = pFreeParticleNode;
    pFreeParticleNode->mNextNode->mPreviousNode = pFreeParticleNode;

    // Increase Active Particle Count.
    mActiveParticles++;

    // Configure Particle.
    configureParticle( pFreeParticleNode );

    // Return New Particle.
    return pFreeParticleNode;
}

//------------------------------------------------------------------------------

void ParticleEmitter::freeParticle( tParticleNode* pParticleNode )
{
    // Remove Particle from Head Chain.
    pParticleNode->mPreviousNode->mNextNode = pParticleNode->mNextNode;
    pParticleNode->mNextNode->mPreviousNode = pParticleNode->mPreviousNode;

    // Reset Extraneous Data.
    pParticleNode->mPreviousNode = NULL;
    
    // Insert into Free Pool.
    pParticleNode->mNextNode = mpFreeParticleNodes;
    mpFreeParticleNodes = pParticleNode;

    // Decrease Active Particle Count.
    mActiveParticles--;
}

//------------------------------------------------------------------------------

void ParticleEmitter::freeAllParticles( void )
{
    // Free All Allocated Particles.
    while( mParticleNodeHead.mNextNode != &mParticleNodeHead )
        freeParticle( mParticleNodeHead.mNextNode );
}

//------------------------------------------------------------------------------

void ParticleEmitter::createParticlePoolBlock( void )
{
    // Generate Free Pool Block.
    tParticleNode* pFreePoolBlock = new tParticleNode[mParticlePoolBlockSize];

    // Generate/Add New Pool Block.
    mParticlePool.push_back( pFreePoolBlock );

    // Initialise Free Pool Block.
    for ( U32 n = 0; n < (mParticlePoolBlockSize-1); n++ )
    {
        pFreePoolBlock[n].mPreviousNode = NULL;
        pFreePoolBlock[n].mNextNode     = pFreePoolBlock+n+1;
    }

    // Insert Last Node Preceding any existing free nodes.
    pFreePoolBlock[mParticlePoolBlockSize-1].mPreviousNode = NULL;
    pFreePoolBlock[mParticlePoolBlockSize-1].mNextNode = mpFreeParticleNodes;

    // Set Free References.
    mpFreeParticleNodes = pFreePoolBlock;
}

//------------------------------------------------------------------------------

void ParticleEmitter::destroyParticlePool( void )
{
    // Free All Particles.
    freeAllParticles();

    // Destroy All Blocks.
    for ( U32 n = 0; n < (U32)mParticlePool.size(); n++ )
        delete [] mParticlePool[n];

    // Clear particle pool.
    mParticlePool.clear();

    // Reset Free Particles.
    mpFreeParticleNodes = NULL;

    // Reset particle node head.
    mParticleNodeHead.mNextNode = mParticleNodeHead.mPreviousNode = &mParticleNodeHead;
}

//------------------------------------------------------------------------------

void ParticleEmitter::configureParticle( tParticleNode* pParticleNode )
{
    // Fetch Effect Age.
    const F32 effectAge = pParentEffectObject->mEffectAge;

    // Fetch Effect Position.
    const Vector2 effectPos = pParentEffectObject->getPosition();


    // Default to not suppressing movement.
    pParticleNode->mSuppressMovement = false;


    // **********************************************************************************************************************
    // Calculate Particle Position.
    // **********************************************************************************************************************

    // Are we using Single Particle?
    if ( mSingleParticle )
    {
        // Yes, so if use Effect Position ( or origin if attached to emitter).
        if ( mAttachPositionToEmitter )
            pParticleNode->mPosition.Set(0.0f, 0.0f);
        else
            pParticleNode->mPosition = effectPos;
    }
    else
    {
        // No, so select Emitter-Type.
        switch( mEmitterType )
        {
            // Use Pivot-Point.
            case POINT:
            {
                // Yes, so if use Effect Position ( or origin if attached to emitter).
                if ( mAttachPositionToEmitter )
                    pParticleNode->mPosition.Set(0.0f, 0.0f);
                else
                    pParticleNode->mPosition = effectPos;

            } break;

            // Use X-Size and Pivot-Y.
            case LINEX:
            {
                // Fetch Local Clip Boundary.
                const b2Vec2* pLocalSizeVertice = pParentEffectObject->getLocalSizeVertices();

                // Choose Random Position along line within Boundary with @ Pivot-Y.
                F32 minX = pLocalSizeVertice[0].x;
                F32 maxX = pLocalSizeVertice[1].x;
                F32 midY = (pLocalSizeVertice[0].y + pLocalSizeVertice[3].y) * 0.5f;

                Vector2 tempPos;

                // Normalise.
                if ( minX > maxX )
                    mSwap( minX, maxX );

                // Potential Vertical Line.
                if ( minX == maxX )
                    tempPos.Set( minX, midY );
                else
                    // Normalised.
                    tempPos.Set( CoreMath::mGetRandomF( minX, maxX ), midY );

                // Rotate into Emitter-Space.
                //
                // NOTE:-   If we're attached to the emitter, then we keep the particle coordinates
                //          in local-space and use the hardware to render them in emitter-space.
                b2Transform xform;
                xform.Set( mAttachPositionToEmitter ? b2Vec2_zero : effectPos, (mAttachPositionToEmitter && mAttachRotationToEmitter) ? 0.0f : pParentEffectObject->getAngle() );
                pParticleNode->mPosition = b2Mul( xform, tempPos );

            } break;

            // Use Y-Size and Pivot-X.
            case LINEY:
            {
                // Fetch Local Clip Boundary.
                const b2Vec2* pLocalSizeVertice = pParentEffectObject->getLocalSizeVertices();

                // Choose Random Position along line within Boundary with @ Pivot-Y.
                F32 midX = (pLocalSizeVertice[0].x + pLocalSizeVertice[1].x) * 0.5f;
                F32 minY = pLocalSizeVertice[0].y;
                F32 maxY = pLocalSizeVertice[3].y;

                Vector2 tempPos;

                // Normalise.
                if ( minY > maxY )
                    mSwap( minY, maxY );

                // Potential Horizontal Line.
                if ( minY == maxY )
                    tempPos.Set( midX, minY );
                else
                    // Normalised.
                    tempPos.Set( midX, CoreMath::mGetRandomF( minY, maxY ) );

                // Rotate into Emitter-Space.
                //
                // NOTE:-   If we're attached to the emitter, then we keep the particle coordinates
                //          in local-space and use the hardware to render them in emitter-space.
                b2Transform xform;
                xform.Set( mAttachPositionToEmitter ? b2Vec2_zero : effectPos, (mAttachPositionToEmitter && mAttachRotationToEmitter) ? 0.0f : pParentEffectObject->getAngle() );
                pParticleNode->mPosition = b2Mul( xform, tempPos );

            } break;

            // Use X/Y Sizes.
            case AREA:
            {
                // Fetch Local Clip Boundary.
                const b2Vec2* pLocalSizeVertice = pParentEffectObject->getLocalSizeVertices();

                // Choose a random position in the area.
                F32 minX = pLocalSizeVertice[0].x;
                F32 maxX = pLocalSizeVertice[1].x;
                F32 minY = pLocalSizeVertice[0].y;
                F32 maxY = pLocalSizeVertice[3].y;

                Vector2 tempPos;

                // Normalise.
                if ( minX > maxX )
                    mSwap( minX, maxX );
                // Normalise.
                if ( minY > maxY )
                    mSwap( minY, maxY );

                // Normalised.
                tempPos.Set( (minX == maxX) ? minX : CoreMath::mGetRandomF( minX, maxX ), (minY == maxY) ? minY : CoreMath::mGetRandomF( minY, maxY ) );

                // Rotate into Emitter-Space.
                //
                // NOTE:-   If we're attached to the emitter, then we keep the particle coordinates
                //          in local-space and use the hardware to render them in emitter-space.
                b2Transform xform;
                xform.Set( mAttachPositionToEmitter ? b2Vec2_zero : effectPos, (mAttachPositionToEmitter && mAttachRotationToEmitter) ? 0.0f : pParentEffectObject->getAngle() );
                pParticleNode->mPosition = b2Mul( xform, tempPos );

            } break;
        }
    }


    // **********************************************************************************************************************
    // Calculate Particle Lifetime.
    // **********************************************************************************************************************
    pParticleNode->mParticleAge = 0.0f;
    pParticleNode->mParticleLifetime = GraphField::calcGraphBVE( mParticleLife.GraphField_Base,
                                                                        mParticleLife.GraphField_Variation,
                                                                        pParentEffectObject->mParticleLife.GraphField_Base,
                                                                        effectAge );


    // **********************************************************************************************************************
    // Calculate Particle Size-X.
    // **********************************************************************************************************************
    pParticleNode->mSize.x = GraphField::calcGraphBVE(  mSizeX.GraphField_Base,
                                                            mSizeX.GraphField_Variation,
                                                            pParentEffectObject->mSizeX.GraphField_Base,
                                                            effectAge );

    // Is the Particle Aspect-Locked?
    if ( mFixedAspect )
    {
        // Yes, so simply copy Size-X.
        pParticleNode->mSize.y = pParticleNode->mSize.x;
    }
    else
    {
        // No, so calculate Particle Size-Y.
        pParticleNode->mSize.y = GraphField::calcGraphBVE(  mSizeY.GraphField_Base,
                                                                mSizeY.GraphField_Variation,
                                                                pParentEffectObject->mSizeY.GraphField_Base,
                                                                effectAge );
    }

    // Reset Render Size.
    pParticleNode->mRenderSize.Set(-1,-1);


    // **********************************************************************************************************************
    // Calculate Speed.
    // **********************************************************************************************************************

    // Ignore if we're using Single-Particle.
    if ( !mSingleParticle )
    {
        pParticleNode->mSpeed = GraphField::calcGraphBVE(    mSpeed.GraphField_Base,
                                                                mSpeed.GraphField_Variation,
                                                                pParentEffectObject->mSpeed.GraphField_Base,
                                                                effectAge );
    }


    // **********************************************************************************************************************
    // Calculate Spin.
    // **********************************************************************************************************************
    pParticleNode->mSpin = GraphField::calcGraphBVE( mSpin.GraphField_Base,
                                                            mSpin.GraphField_Variation,
                                                            pParentEffectObject->mSpin.GraphField_Base,
                                                            effectAge );


    // **********************************************************************************************************************
    // Calculate Fixed-Force.
    // **********************************************************************************************************************
    pParticleNode->mFixedForce = GraphField::calcGraphBVE(   mFixedForce.GraphField_Base,
                                                                mFixedForce.GraphField_Variation,
                                                                pParentEffectObject->mFixedForce.GraphField_Base,
                                                                effectAge );


    // **********************************************************************************************************************
    // Calculate Random Motion..
    // **********************************************************************************************************************

    // Ignore if we're using Single-Particle.
    if ( !mSingleParticle )
    {
        pParticleNode->mRandomMotion = GraphField::calcGraphBVE( mRandomMotion.GraphField_Base,
                                                                        mRandomMotion.GraphField_Variation,
                                                                        pParentEffectObject->mRandomMotion.GraphField_Base,
                                                                        effectAge );
    }


    // **********************************************************************************************************************
    // Calculate Emission Angle.
    // **********************************************************************************************************************

    // Note:- We reset the emission angle/arc in-case we're using Single-Particle mode as this is the default.
    F32 emissionForce = 0;
    F32 emissionAngle = 0;
    F32 emissionArc = 0;

    // Ignore if we're using Single-Particle.
    if ( !mSingleParticle )
    {
        // Are we using the Effects Emission?
        if ( mUseEffectEmission )
        {
            // Yes, so calculate emission from effect...

            // Calculate Emission Force.
            emissionForce = GraphField::calcGraphBV( pParentEffectObject->mEmissionForce.GraphField_Base,
                                                            pParentEffectObject->mEmissionForce.GraphField_Variation,
                                                            effectAge);

            // Calculate Emission Angle.
            emissionAngle = GraphField::calcGraphBV( pParentEffectObject->mEmissionAngle.GraphField_Base,
                                                            pParentEffectObject->mEmissionAngle.GraphField_Variation,
                                                            effectAge);

            // Calculate Emission Arc.
            // NOTE:-   We're actually interested in half the emission arc!
            emissionArc = GraphField::calcGraphBV(   pParentEffectObject->mEmissionArc.GraphField_Base,
                                                        pParentEffectObject->mEmissionArc.GraphField_Variation,
                                                        effectAge ) * 0.5f;
        }
        else
        {
            // No, so calculate emission from emitter...

            // Calculate Emission Force.
            emissionForce = GraphField::calcGraphBV( mEmissionForce.GraphField_Base,
                                                            mEmissionForce.GraphField_Variation,
                                                            effectAge);

            // Calculate Emission Angle.
            emissionAngle = GraphField::calcGraphBV( mEmissionAngle.GraphField_Base,
                                                        mEmissionAngle.GraphField_Variation,
                                                        effectAge );

            // Calculate Emission Arc.
            // NOTE:-   We're actually interested in half the emission arc!
            emissionArc = GraphField::calcGraphBV(   mEmissionArc.GraphField_Base,
                                                        mEmissionArc.GraphField_Variation,
                                                        effectAge ) * 0.5f;
        }

        // Is the Emission Rotation linked?
        if ( mLinkEmissionRotation )
            // Yes, so add Effect Object-Rotation.
            emissionAngle += pParentEffectObject->getAngle();

        // Calculate Final Emission Angle by choosing random Arc.
        emissionAngle = mFmod( CoreMath::mGetRandomF( emissionAngle-emissionArc, emissionAngle+emissionArc ), 360.0f ) ;

        // Calculate Normalised Velocity.
        pParticleNode->mVelocity.Set( emissionForce * mSin( mDegToRad(emissionAngle) ), emissionForce * mCos( mDegToRad(emissionAngle) ) );
    }

    // **********************************************************************************************************************
    // Calculate Orientation Angle.
    // **********************************************************************************************************************

    // Handle Particle Orientation Mode.
    switch( mParticleOrientationMode )
    {
        // Aligned to Initial Emission.
        case ALIGNED:
        {
            // Just use the emission angle plus offset.
            pParticleNode->mOrientationAngle = mFmod( emissionAngle - mAlign_AngleOffset, 360.0f );

        } break;

        // Fixed Orientation.
        case FIXED:
        {
            // Use Fixed Angle.
            pParticleNode->mOrientationAngle = mFmod( mFixed_AngleOffset, 360.0f );

        } break;

        // Random with Constraints.
        case RANDOM:
        {
            // Used Random Angle/Arc.
            F32 randomArc = mRandom_Arc * 0.5f;
            pParticleNode->mOrientationAngle = mFmod( CoreMath::mGetRandomF( mRandom_AngleOffset - randomArc, mRandom_AngleOffset + randomArc ), 360.0f );

        } break;

    }

    // **********************************************************************************************************************
    // Calculate RGBA Components.
    // **********************************************************************************************************************

    pParticleNode->mColour.set( mClampF( mColourRed.GraphField_OverLife.getGraphValue( 0.0f ), mColourRed.GraphField_OverLife.getMinValue(), mColourRed.GraphField_OverLife.getMaxValue() ),
                                mClampF( mColourGreen.GraphField_OverLife.getGraphValue( 0.0f ), mColourGreen.GraphField_OverLife.getMinValue(), mColourGreen.GraphField_OverLife.getMaxValue() ),
                                mClampF( mColourBlue.GraphField_OverLife.getGraphValue( 0.0f ), mColourBlue.GraphField_OverLife.getMinValue(), mColourBlue.GraphField_OverLife.getMaxValue() ),
                                mClampF( mVisibility.GraphField_OverLife.getGraphValue( 0.0f ) * pParentEffectObject->mVisibility.GraphField_Base.getGraphValue( 0.0f ), mVisibility.GraphField_OverLife.getMinValue(), mVisibility.GraphField_OverLife.getMaxValue() ) );


    // **********************************************************************************************************************
    // Animation Controller.
    // **********************************************************************************************************************

    // If an animation is selected, start it playing.
    if ( !mStaticMode && mAnimationAsset.notNull() )
        pParticleNode->mAnimationController.playAnimation( mAnimationAsset.getAssetId(), false );


    // Reset Last Render Size.
    pParticleNode->mLastRenderSize.Set(-1, -1);


    // **********************************************************************************************************************
    // Reset Tick Position.
    // **********************************************************************************************************************
    pParticleNode->mPreTickPosition = pParticleNode->mPostTickPosition = pParticleNode->mRenderTickPosition = pParticleNode->mPosition;


    // **********************************************************************************************************************
    // Do a Single Particle Integration to get things going.
    // **********************************************************************************************************************
    integrateParticle( pParticleNode, 0.0f, 0.0f );
}

//------------------------------------------------------------------------------

bool ParticleEmitter::loadEmitter( const char* emitterFile )
{
    // Check we're part of a parent-effect.
    // NOTE:-   There's no need to suppose we're not as this should not happen but
    //          if it does then let's fail gracefully.
    if ( !pParentEffectObject )
    {
        // Warn.
        Con::warnf("ParticleEmitter::loadEmitter() - The emitter is not part of a Scene!");
        // Return Error.
        return false;
    }

    // Expand relative paths.
    char buffer[1024];
    if ( emitterFile )
        if ( Con::expandPath( buffer, sizeof( buffer ), emitterFile ) )
            emitterFile = buffer;

    // Open Emitter File.
    Stream* pStream = ResourceManager->openStream( emitterFile );
    // Check Stream.
    if ( !pStream )
    {
        // Warn.
        Con::warnf("ParticleEmitter::loadEmitter() - Could not Open File '%s' for Emitter Load.", emitterFile);
        // Return Error.
        return false;
    }

    // Scene Objects.
    Vector<SceneObject*> ObjReferenceList;

    // Set Vector Associations.
    VECTOR_SET_ASSOCIATION( ObjReferenceList );

    // Load Stream.
    if ( !loadStream( *pStream, pParentEffectObject->getScene(), ObjReferenceList, true  ) )
    {
        // Warn.
        Con::warnf("ParticleEmitter::loadEmitter() - Error Loading Emitter!");
        // Return Error.
        return false;
    }

    // Close Stream.
    ResourceManager->closeStream( pStream );

    // Return Okay.
    return true;
}

//------------------------------------------------------------------------------

bool ParticleEmitter::saveEmitter( const char* emitterFile )
{
    // Check we're part of a parent-effect.
    // NOTE:-   There's no need to suppose we're not as this should not happen but
    //          if it does then let's fail gracefully.
    if ( !pParentEffectObject )
    {
        // Warn.
        Con::warnf("ParticleEmitter::saveEmitter() - The emitter is not part of a Scene!");
        // Return Error.
        return false;
    }

    // Expand relative paths.
    char buffer[1024];
    if ( emitterFile )
        if ( Con::expandPath( buffer, sizeof( buffer ), emitterFile ) )
            emitterFile = buffer;

    // Open Emitter File.
    FileStream fileStream;
    if ( !ResourceManager->openFileForWrite( fileStream, emitterFile, FileStream::Write ) )
    {
        // Warn.
        Con::warnf("ParticleEmitter::saveEmitter() - Could not open File '%s' for Emitter Save.", emitterFile);
        // Return Error.
        return false;
    }

    // Stop Parent Effect.
    // NOTE:-   We do this so that we don't save our active particles.
    pParentEffectObject->stopEffect(false, false);

    // Save Stream.
    if ( !saveStream( fileStream, pParentEffectObject->getScene()->getNextSerialiseID(), 1 ) )
    {
        // Warn.
        Con::warnf("ParticleEmitter::saveEmitter() - Error Saving Emitter!");
        // Return Error.
        return false;
    }

    // Return Okay.
    return true;
}

//------------------------------------------------------------------------------

void ParticleEmitter::setEmitterCollisionStatus( const bool status )
{
    // Set Emitter Collision Status.
    mUseEmitterCollisions = status;
}

//------------------------------------------------------------------------------

bool ParticleEmitter::getEmitterCollisionStatus( void )
{
    // Get Emitter Collision Status.
    return mUseEmitterCollisions;
}

//------------------------------------------------------------------------------

#if 0
bool ParticleEmitter::checkParticleCollisions( const ParticleEffect* pParentEffect, const F32 elapsedTime, t2dPhysics::cCollisionStatus& sendCollisionStatus, DebugStats* pDebugStats )
{
    // Reset Initial Collision Status.
    bool collisionStatus = false;

    // Fetch scene.
    Scene* pScene = pParentEffect->getScene();

    // Get Parent Collision Masks.
    const U32 groupMask = pParentEffect->getCollisionGroupMask();
    const U32 layerMask = pParentEffect->getCollisionLayerMask();

    // Get Parent Collision Response.
    const t2dPhysics::eCollisionResponse collisionResponse = pParentEffect->getCollisionResponseMode();

    // Get Parent Restitution.
    const F32 restitution = pParentEffect->getRestitution();

    // Fetch First Particle Node.
    tParticleNode* pParticleNode = mParticleNodeHead.mNextNode;

    // Next Particle Node.
    tParticleNode* pNextParticleNode;

    // Process All particle nodes.
    while ( pParticleNode != &mParticleNodeHead )
    {
        Vector2 newPosition;

        // Default to not suppressing movement.
        pParticleNode->mSuppressMovement = false;

        // Fetch Start Position.
        Vector2& startPosition = pParticleNode->mPosition;
        // Calculate Projected Position.
        const Vector2 endPosition = pParticleNode->mPosition + (pParticleNode->mVelocity * pParticleNode->mRenderSpeed * elapsedTime);

        // Pick Objects along our velocity path.
        const U32 pickedObjects = pScene->pickLine( startPosition, endPosition, groupMask, layerMask, false, pParentEffect );

        // Reference next Particle Node.
        // NOTE:-   We do this here because we may destroy the particle if the collision-response is in "KILL" mode.
        pNextParticleNode = pParticleNode->mNextNode;

        // Did we collide?
        if ( pickedObjects > 0 )
        {
            // Flag Collision Occurred.
            collisionStatus = true;

            // Fetch Colliding Object.
            typeSceneObjectVectorConstRef pickVector = pScene->getPickList();
            // Fetch Scene Object.
            SceneObject* pSceneObject = pickVector[0];
            // Fetch Collisoin Time.
            const F32& collisionTime = pSceneObject->mSortKeyCollisionTime;

            // Lerp to collision position.
            startPosition.lerp( endPosition, collisionTime, startPosition );

            // Reference Velocity.
            Vector2& linearVelocity = pParticleNode->mVelocity;
            // Reference Collision Normal.
            Vector2& collisionNormal = pSceneObject->mSortKeyCollisionNormal;

            switch( collisionResponse )
            {
                // Bounce.
                case t2dPhysics::T2D_RESPONSE_BOUNCE:
                {
                    // Calculate Dot Velocity Normal.
                    const F32 dotVelocityNormal = collisionNormal * linearVelocity;
                    // Any velocity to clamp?
                    if ( mNotZero(dotVelocityNormal) )
                     {
                        // Set new Linear Velocity.
                        linearVelocity -= (collisionNormal*(2.0f*dotVelocityNormal) * restitution);
                    }

                } break;

                // Clamp.
                case t2dPhysics::T2D_RESPONSE_CLAMP:
                {
                    // Calculate Dot Velocity Normal.
                    const F32 dotVelocityNormal = collisionNormal * linearVelocity;
                    // Any velocity to clamp?
                    if ( mNotZero(dotVelocityNormal) )
                    {
                        // Set new Linear Velocity.
                        linearVelocity -= collisionNormal*dotVelocityNormal;
                    }

                } break;

                // Bounce.
                case t2dPhysics::T2D_RESPONSE_STICKY:
                {
                    // Set at rest.
                    linearVelocity.Set(0.0f,0.0f);

                } break;

                // Kill.
                case t2dPhysics::T2D_RESPONSE_KILL:
                {
                    // Free Particle.
                    freeParticle( pParticleNode );

                } break;
            };

            // Suppress Movment.
            pParticleNode->mSuppressMovement = true;
        }

        // Move to next Particle Node.
        pParticleNode = pNextParticleNode;
    };   

    // Return Collision Status.
    return collisionStatus;
}
#endif

//-----------------------------------------------------------------------------
// Serialisation.
//-----------------------------------------------------------------------------

// Register Handlers.
REGISTER_SERIALISE_START( ParticleEmitter )
    REGISTER_SERIALISE_VERSION( ParticleEmitter, 3, true )
    REGISTER_SERIALISE_VERSION( ParticleEmitter, 4, false )
REGISTER_SERIALISE_END()

// Implement Leaf Serialisation.
IMPLEMENT_SERIALISE_LEAF( ParticleEmitter, 4 )


//-----------------------------------------------------------------------------
// Load v3
//-----------------------------------------------------------------------------
IMPLEMENT_2D_LOAD_METHOD( ParticleEmitter, 3 )
{
    // Free All Particles.
    object->freeAllParticles();

    // *********************************************************
    // Read Object Information.
    // *********************************************************

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


    // Load Non-Graph Entries.
    object->mEmitterName = stream.readSTString();

    bool    emitterCollisionStatus;
    bool    blending = false;;
    U32     srcBlendFactor = 0;
    U32     dstBlendFactor = 0;

    if (    !stream.read( &(object->mGlobalClipBoundary[0].x) ) ||
            !stream.read( &(object->mGlobalClipBoundary[0].y) ) ||
            !stream.read( &(object->mGlobalClipBoundary[1].x) ) ||
            !stream.read( &(object->mGlobalClipBoundary[1].y) ) ||
            !stream.read( &(object->mGlobalClipBoundary[2].x) ) ||
            !stream.read( &(object->mGlobalClipBoundary[2].y) ) ||
            !stream.read( &(object->mGlobalClipBoundary[3].x) ) ||
            !stream.read( &(object->mGlobalClipBoundary[3].y) ) ||
            !stream.read( &object->mTimeSinceLastGeneration ) ||
            !stream.read( &object->mPauseEmitter ) ||
            !stream.read( &object->mFixedAspect ) ||
            !stream.read( &object->mFixedForceDirection.x ) ||
            !stream.read( &object->mFixedForceDirection.y ) ||
            !stream.read( &object->mFixedForceAngle ) ||
            !stream.read( (S32*)&object->mParticleOrientationMode ) ||
            !stream.read( &object->mAlign_AngleOffset ) ||
            !stream.read( &object->mAlign_KeepAligned ) ||
            !stream.read( &object->mRandom_AngleOffset ) ||
            !stream.read( &object->mRandom_Arc ) ||
            !stream.read( &object->mFixed_AngleOffset ) ||
            !stream.read( (S32*)&object->mEmitterType ) ||
            !stream.read( &object->mPivotPoint.x ) ||
            !stream.read( &object->mPivotPoint.y ) ||
            !stream.read( &object->mUseEffectEmission ) ||
            !stream.read( &object->mLinkEmissionRotation ) ||
            !stream.read( &object->mIntenseParticles ) ||
            !stream.read( &object->mSingleParticle ) ||
            !stream.read( &object->mAttachPositionToEmitter ) ||
            !stream.read( &object->mAttachRotationToEmitter ) ||
            !stream.read( &object->mFirstInFrontOrder ) ||
            !stream.read( &object->mPauseEmitter ) ||
            !stream.read( &blending ) ||
            !stream.read( &srcBlendFactor ) ||
            !stream.read( &dstBlendFactor ) ||
            !stream.read( &emitterCollisionStatus ) )
        return false;

    // Rescale global clip boundary.
    for ( U32 n = 0; n < 4; ++n )
    {
        object->mGlobalClipBoundary[n] *= 0.5f;
    }

    // Flip Y appropriately.
    object->mFixedForceDirection.y = -object->mFixedForceDirection.y;
    object->mPivotPoint.y = -object->mPivotPoint.y;
    object->mFixedForceAngle = mGetFlippedYAngle( mDegToRad( object->mFixedForceAngle ) );
    object->mAlign_AngleOffset = mDegToRad( object->mAlign_AngleOffset );
    object->mRandom_AngleOffset = mDegToRad( object->mRandom_AngleOffset );
    object->mRandom_Arc = mDegToRad( object->mRandom_Arc );
    object->mFixed_AngleOffset = mDegToRad( object->mFixed_AngleOffset );

    //Emitter Collision Status.
    object->setEmitterCollisionStatus( emitterCollisionStatus );

    // Set Blending.
    object->setBlending( blending, srcBlendFactor, dstBlendFactor );

    // Load Animation Flag.
    bool animationActive;
    if ( !stream.read( &animationActive ) )
        return false;
    object->mStaticMode = !animationActive;

    // Do we have an animation selected?
    if ( !object->mStaticMode )
    {
        // Yes, so load Animation Controller Proxy.
        if ( !object->mAnimationControllerProxy.loadStream( SERIALISE_LOAD_ARGS_PASS ) )
            return false;

        // LoadAnimation Name.
        object->mAnimationAsset = stream.readSTString();
    }
    else
    {
        // No, so Load ImageMap Name..
        object->mImageAsset = stream.readSTString();
        // Load ImageMap Frame.
        if ( !stream.read( &object->mImageMapFrame ) )
            return false;

        // Set ImageMap Name/Frame.
        object->setImageMap( object->mImageAsset.getAssetId(), object->mImageMapFrame );
    }

    // Load Particle Count.
    U32 particleCount;
    if ( !stream.read( &particleCount ) )
        return false;

    F32 dummyValue;

    // Load All particle nodes.
    // NOTE:-   We load all the particles which were saved in reverse order
    //          which means that they'll now be put into forward order.
    for ( U32 n = 0; n < particleCount; n++ )
    {
        // Create Particle.
        tParticleNode* pParticleNode = object->createParticle();

        // Load Particle Node.
        if (    !stream.read( &(pParticleNode->mParticleLifetime) ) ||
                !stream.read( &(pParticleNode->mParticleAge) ) ||
                !stream.read( &(pParticleNode->mPosition.x) ) ||
                !stream.read( &(pParticleNode->mPosition.y) ) ||
                !stream.read( &(pParticleNode->mVelocity.x) ) ||
                !stream.read( &(pParticleNode->mVelocity.y) ) ||
                !stream.read( &(pParticleNode->mOrientationAngle) ) ||
                !stream.read( &(pParticleNode->mOOBB[0].x) ) ||
                !stream.read( &(pParticleNode->mOOBB[0].y) ) ||
                !stream.read( &(pParticleNode->mOOBB[1].x) ) ||
                !stream.read( &(pParticleNode->mOOBB[1].y) ) ||
                !stream.read( &(pParticleNode->mOOBB[2].x) ) ||
                !stream.read( &(pParticleNode->mOOBB[2].y) ) ||
                !stream.read( &(pParticleNode->mOOBB[3].x) ) ||
                !stream.read( &(pParticleNode->mOOBB[3].y) ) ||
                !stream.read( &dummyValue ) ||
                !stream.read( &dummyValue ) ||
                !stream.read( &dummyValue ) ||
                !stream.read( &dummyValue ) ||
                !stream.read( &(pParticleNode->mLastRenderSize.x) ) ||
                !stream.read( &(pParticleNode->mLastRenderSize.y) ) ||
                !stream.read( &(pParticleNode->mRenderSize.x) ) ||
                !stream.read( &(pParticleNode->mRenderSize.y) ) ||
                !stream.read( &(pParticleNode->mRenderSpeed) ) ||
                !stream.read( &(pParticleNode->mRenderSpin) ) ||
                !stream.read( &(pParticleNode->mRenderFixedForce) ) ||
                !stream.read( &(pParticleNode->mRenderRandomMotion) ) ||
                !stream.read( &(pParticleNode->mSize.x) ) ||
                !stream.read( &(pParticleNode->mSize.y) ) ||
                !stream.read( &(pParticleNode->mSpeed) ) ||
                !stream.read( &(pParticleNode->mSpin) ) ||
                !stream.read( &(pParticleNode->mFixedForce) ) ||
                !stream.read( &(pParticleNode->mRandomMotion) ) ||
                !stream.read( &(pParticleNode->mColour) ) )
            return false;

        // Rescale local clip boundary.
        for ( U32 n = 0; n < 4; ++n )
        {
            pParticleNode->mOOBB[n] *= 0.5f;
        }

        // Flip Y appropriately.
        pParticleNode->mPosition.y = -pParticleNode->mPosition.y;
        pParticleNode->mVelocity.y = -pParticleNode->mVelocity.y;
        pParticleNode->mOrientationAngle = mGetFlippedYAngle( mDegToRad( pParticleNode->mOrientationAngle ) );
        pParticleNode->mRenderSpin = mGetFlippedYAngle( mDegToRad( pParticleNode->mRenderSpin ) );
        pParticleNode->mSpin = mGetFlippedYAngle( mDegToRad( pParticleNode->mSpin ) );

        // Load Animation Controller (if animation is selected).
        if ( !object->mStaticMode )
            pParticleNode->mAnimationController.loadStream( SERIALISE_LOAD_ARGS_PASS );
    };

    // Free All Particles.
    object->freeAllParticles();

    // Return Okay.
    return true;
}

//-----------------------------------------------------------------------------
// Save v3
//-----------------------------------------------------------------------------
IMPLEMENT_2D_SAVE_METHOD( ParticleEmitter, 3 )
{
    AssertFatal( false, "Saving to v3 is not supported" );
    return false;

}


//-----------------------------------------------------------------------------
// Load v4
//-----------------------------------------------------------------------------
IMPLEMENT_2D_LOAD_METHOD( ParticleEmitter, 4 )
{
    // Free All Particles.
    object->freeAllParticles();

    // *********************************************************
    // Read Object Information.
    // *********************************************************

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


    // Load Non-Graph Entries.
    object->mEmitterName = stream.readSTString();

    bool    emitterCollisionStatus;
    bool    blending;
    U32     srcBlendFactor;
    U32     dstBlendFactor;

    if (    !stream.read( &(object->mGlobalClipBoundary[0].x) ) ||
            !stream.read( &(object->mGlobalClipBoundary[0].y) ) ||
            !stream.read( &(object->mGlobalClipBoundary[1].x) ) ||
            !stream.read( &(object->mGlobalClipBoundary[1].y) ) ||
            !stream.read( &(object->mGlobalClipBoundary[2].x) ) ||
            !stream.read( &(object->mGlobalClipBoundary[2].y) ) ||
            !stream.read( &(object->mGlobalClipBoundary[3].x) ) ||
            !stream.read( &(object->mGlobalClipBoundary[3].y) ) ||
            !stream.read( &object->mTimeSinceLastGeneration ) ||
            !stream.read( &object->mPauseEmitter ) ||
            !stream.read( &object->mFixedAspect ) ||
            !stream.read( &object->mFixedForceDirection.x ) ||
            !stream.read( &object->mFixedForceDirection.y ) ||
            !stream.read( &object->mFixedForceAngle ) ||
            !stream.read( (S32*)&object->mParticleOrientationMode ) ||
            !stream.read( &object->mAlign_AngleOffset ) ||
            !stream.read( &object->mAlign_KeepAligned ) ||
            !stream.read( &object->mRandom_AngleOffset ) ||
            !stream.read( &object->mRandom_Arc ) ||
            !stream.read( &object->mFixed_AngleOffset ) ||
            !stream.read( (S32*)&object->mEmitterType ) ||
            !stream.read( &object->mPivotPoint.x ) ||
            !stream.read( &object->mPivotPoint.y ) ||
            !stream.read( &object->mUseEffectEmission ) ||
            !stream.read( &object->mLinkEmissionRotation ) ||
            !stream.read( &object->mIntenseParticles ) ||
            !stream.read( &object->mSingleParticle ) ||
            !stream.read( &object->mAttachPositionToEmitter ) ||
            !stream.read( &object->mAttachRotationToEmitter ) ||
            !stream.read( &object->mOrderedParticles ) ||
            !stream.read( &object->mFirstInFrontOrder ) ||
            !stream.read( &object->mPauseEmitter ) ||
            !stream.read( &blending ) ||
            !stream.read( &srcBlendFactor ) ||
            !stream.read( &dstBlendFactor ) ||
            !stream.read( &emitterCollisionStatus ) )
        return false;

    //Emitter Collision Status.
    object->setEmitterCollisionStatus( emitterCollisionStatus );

    // Set Blending.
    object->setBlending( blending, srcBlendFactor, dstBlendFactor );

    // Load Animation Flag.
    bool animationActive;
    if ( !stream.read( &animationActive ) )
        return false;
    object->mStaticMode = !animationActive;

    // Do we have an animation selected?
    if ( !object->mStaticMode )
    {
        // Yes, so load Animation Controller Proxy.
        if ( !object->mAnimationControllerProxy.loadStream( SERIALISE_LOAD_ARGS_PASS ) )
            return false;

        // LoadAnimation Name.
        object->mAnimationAsset = stream.readSTString();
    }
    else
    {
        // No, so Load ImageMap Name..
        object->mImageAsset = stream.readSTString();
        // Load ImageMap Frame.
        if ( !stream.read( &object->mImageMapFrame ) )
            return false;

        // Set ImageMap Name/Frame.
        object->setImageMap( object->mImageAsset.getAssetId(), object->mImageMapFrame );
    }

    // Return Okay.
    return true;
}

//-----------------------------------------------------------------------------
// Save v4
//-----------------------------------------------------------------------------
IMPLEMENT_2D_SAVE_METHOD( ParticleEmitter, 4 )
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

    // Save Non-Graph Entries.
    stream.writeString( object->mEmitterName );

    if (    !stream.write( object->mGlobalClipBoundary[0].x ) ||
            !stream.write( object->mGlobalClipBoundary[0].y ) ||
            !stream.write( object->mGlobalClipBoundary[1].x ) ||
            !stream.write( object->mGlobalClipBoundary[1].y ) ||
            !stream.write( object->mGlobalClipBoundary[2].x ) ||
            !stream.write( object->mGlobalClipBoundary[2].y ) ||
            !stream.write( object->mGlobalClipBoundary[3].x ) ||
            !stream.write( object->mGlobalClipBoundary[3].y ) ||
            !stream.write( object->mTimeSinceLastGeneration ) ||
            !stream.write( object->mPauseEmitter ) ||
            !stream.write( object->mFixedAspect ) ||
            !stream.write( object->mFixedForceDirection.x ) ||
            !stream.write( object->mFixedForceDirection.y ) ||
            !stream.write( object->mFixedForceAngle ) ||
            !stream.write( (S32)object->mParticleOrientationMode ) ||
            !stream.write( object->mAlign_AngleOffset ) ||
            !stream.write( object->mAlign_KeepAligned ) ||
            !stream.write( object->mRandom_AngleOffset ) ||
            !stream.write( object->mRandom_Arc ) ||
            !stream.write( object->mFixed_AngleOffset ) ||
            !stream.write( (S32)object->mEmitterType ) ||
            !stream.write( object->mPivotPoint.x ) ||
            !stream.write( object->mPivotPoint.y ) ||
            !stream.write( object->mUseEffectEmission ) ||
            !stream.write( object->mLinkEmissionRotation ) ||
            !stream.write( object->mIntenseParticles ) ||
            !stream.write( object->mSingleParticle ) ||
            !stream.write( object->mAttachPositionToEmitter ) ||
            !stream.write( object->mAttachRotationToEmitter ) ||
            !stream.write( object->mOrderedParticles ) ||
            !stream.write( object->mFirstInFrontOrder ) ||
            !stream.write( object->mPauseEmitter ) ||
            !stream.write( object->mBlendMode ) ||
            !stream.write( object->mSrcBlendFactor ) ||
            !stream.write( object->mDstBlendFactor ) ||
            !stream.write( object->getEmitterCollisionStatus() ) )
        return false;

    // Save Animation Flag.
    bool animationActive = !object->mStaticMode;
    if ( !stream.write( animationActive ) )
        return false;

    // Do we have an animation selected?
    if ( !object->mStaticMode )
    {
        // Yes, so save Animation Controller Proxy.
        if ( !object->mAnimationControllerProxy.saveStream( SERIALISE_SAVE_ARGS_PASS ) )
            return false;

        // Save Animation Name.
        stream.writeString( object->mAnimationAsset.getAssetId() );
    }
    else
    {
        // No, so Save ImageMap Name..
        stream.writeString( object->mImageAsset.getAssetId() );
        // Save ImageMap Frame.
        if ( !stream.write( object->mImageMapFrame ) )
            return false;
    }

    // Return Okay.
    return true;
}
