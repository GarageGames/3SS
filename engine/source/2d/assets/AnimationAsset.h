//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _ANIMATION_ASSET_H_
#define _ANIMATION_ASSET_H_

#ifndef _ASSET_PTR_H_
#include "assets/assetPtr.h"
#endif

//-----------------------------------------------------------------------------

class ImageAsset;

//-----------------------------------------------------------------------------

class AnimationAsset : public AssetBase
{
private:
    typedef AssetBase  Parent;

    AssetPtr<ImageAsset>    mImageAsset;
    Vector<S32>             mAnimationFrames;
    Vector<S32>             mValidatedFrames;
    F32                     mAnimationTime;
    bool                    mAnimationCycle;
    bool                    mRandomStart;

    F32                     mAnimationIntegration;

public:
    AnimationAsset();
    virtual ~AnimationAsset();

    static void initPersistFields();
    virtual bool onAdd();
    virtual void onRemove();

    void            setImageMap( const char* pAssetId );
    const AssetPtr<ImageAsset>& getImageMap( void ) const     { return mImageAsset; }

    void            setAnimationFrames( const char* pAnimationFrames );
    inline const Vector<S32>& getSpecifiedAnimationFrames( void ) const { return mAnimationFrames; }
    inline const Vector<S32>& getValidatedAnimationFrames( void ) const { return mValidatedFrames; }

    void            setAnimationTime( const F32 animationTime );
    inline F32      getAnimationTime( void ) const                      { return mAnimationTime; }
    void            setAnimationCycle( const bool animationCycle );
    inline bool     getAnimationCycle( void ) const                     { return mAnimationCycle; }
    void            setRandomStart( const bool randomStart );
    inline bool     getRandomStart( void ) const                        { return mRandomStart; }

    // Frame validation.
    void            validateFrames( void );

    /// Declare Console Object.
    DECLARE_CONOBJECT(AnimationAsset);

protected:
    virtual void initializeAsset( void );
    virtual void onAssetRefresh( void );

protected:
    static bool setImageMap( void* obj, const char* data )                      { static_cast<AnimationAsset*>(obj)->setImageMap( data ); return false; }
    static bool writeImageMap( void* obj, StringTableEntry pFieldName )         { return static_cast<AnimationAsset*>(obj)->mImageAsset.notNull(); }
    static bool setAnimationFrames( void* obj, const char* data )               { static_cast<AnimationAsset*>(obj)->setAnimationFrames( data ); return false; }    
    static bool writeAnimationFrames( void* obj, StringTableEntry pFieldName )  { return static_cast<AnimationAsset*>(obj)->mAnimationFrames.size() > 0; }
    static bool setAnimationTime( void* obj, const char* data )                 { static_cast<AnimationAsset*>(obj)->setAnimationTime( dAtof(data) ); return false; }
    static bool setAnimationCycle( void* obj, const char* data )                { static_cast<AnimationAsset*>(obj)->setAnimationCycle( dAtob(data) ); return false; }
    static bool writeAnimationCycle( void* obj, StringTableEntry pFieldName )   { return static_cast<AnimationAsset*>(obj)->getAnimationCycle() == false; }
    static bool setRandomStart( void* obj, const char* data )                   { static_cast<AnimationAsset*>(obj)->setRandomStart( dAtob(data) ); return false; }
    static bool writeRandomStart( void* obj, StringTableEntry pFieldName )      { return static_cast<AnimationAsset*>(obj)->getRandomStart() == true; }
};

#endif // _ANIMATION_ASSET_H_