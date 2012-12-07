//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _AUDIO_ASSET_H_
#include "audioAsset.h"
#endif

#ifndef _ASSET_FIELD_TYPES_H
#include "assets/assetFieldTypes.h"
#endif

#ifndef _CONSOLETYPES_H_
#include "console/consoleTypes.h"
#endif

//--------------------------------------------------------------------------

IMPLEMENT_CONOBJECT(AudioAsset);

//--------------------------------------------------------------------------

AudioAsset::AudioAsset()
{
   mAudioFile                        = StringTable->EmptyString;
   mDescription.mVolume              = 1.0f;
   mDescription.mVolumeChannel       = 0;
   mDescription.mIsLooping           = false;
   mDescription.mIsStreaming		 = false;

   mDescription.mIs3D                = false;
   mDescription.mReferenceDistance   = 1.0f;
   mDescription.mMaxDistance         = 100.0f;
   mDescription.mEnvironmentLevel    = 0.0f;
   mDescription.mConeInsideAngle     = 360;
   mDescription.mConeOutsideAngle    = 360;
   mDescription.mConeOutsideVolume   = 1.0f;
   mDescription.mConeVector.set(0, 0, 1);

}

//--------------------------------------------------------------------------

void AudioAsset::initPersistFields()
{
   Parent::initPersistFields();

   addProtectedField("AudioFile", TypeAssetLooseFilePath, Offset(mAudioFile, AudioAsset), &setAudioFile, &getAudioFile, &defaultProtectedWriteFn, "" );
   addProtectedField("Volume", TypeF32, Offset(mDescription.mVolume, AudioAsset), &setVolume, &defaultProtectedGetFn, &writeVolume, "");
   addProtectedField("VolumeChannel", TypeS32, Offset(mDescription.mVolumeChannel, AudioAsset), &setVolumeChannel, &defaultProtectedGetFn, &writeVolumeChannel, "");
   addProtectedField("Looping", TypeBool, Offset(mDescription.mIsLooping, AudioAsset), &setLooping, &defaultProtectedGetFn, &writeLooping, "");
   addProtectedField("Streaming", TypeBool, Offset(mDescription.mIsStreaming, AudioAsset), &setStreaming, &defaultProtectedGetFn, &writeStreaming, "");

   //addField("is3D",              TypeBool,    Offset(mDescription.mIs3D, AudioAsset));
   //addField("referenceDistance", TypeF32,     Offset(mDescription.mReferenceDistance, AudioAsset));
   //addField("maxDistance",       TypeF32,     Offset(mDescription.mMaxDistance, AudioAsset));
   //addField("coneInsideAngle",   TypeS32,     Offset(mDescription.mConeInsideAngle, AudioAsset));
   //addField("coneOutsideAngle",  TypeS32,     Offset(mDescription.mConeOutsideAngle, AudioAsset));
   //addField("coneOutsideVolume", TypeF32,     Offset(mDescription.mConeOutsideVolume, AudioAsset));
   //addField("coneVector",        TypePoint3F, Offset(mDescription.mConeVector, AudioAsset));
   //addField("environmentLevel",  TypeF32,     Offset(mDescription.mEnvironmentLevel, AudioAsset));
}

//--------------------------------------------------------------------------

void AudioAsset::initializeAsset( void )
{
    // Call parent.
    Parent::initializeAsset();

    // Ensure the audio-file is expanded.
    mAudioFile = expandAssetFilePath( mAudioFile );

    // Asset should never auto-unload.
    setAssetAutoUnload( false );

    // Clamp these for now.
    if (mDescription.mIs3D)
    {
        mDescription.mReferenceDistance   = mClampF(mDescription.mReferenceDistance, 0.0f, mDescription.mReferenceDistance);
        mDescription.mMaxDistance         = mDescription.mMaxDistance > mDescription.mReferenceDistance ? mDescription.mMaxDistance : (mDescription.mReferenceDistance+0.01f);
        mDescription.mEnvironmentLevel    = mClampF(mDescription.mEnvironmentLevel, 0.0f, 1.0f);
        mDescription.mConeInsideAngle     = mClamp(mDescription.mConeInsideAngle, 0, 360);
        mDescription.mConeOutsideAngle    = mClamp(mDescription.mConeOutsideAngle, mDescription.mConeInsideAngle, 360);
        mDescription.mConeOutsideVolume   = mClampF(mDescription.mConeOutsideVolume, 0.0f, 1.0f);
        mDescription.mConeVector.normalize();
    }
}

//--------------------------------------------------------------------------

void AudioAsset::setAudioFile( const char* pAudioFile )
{
    // Sanity!
    AssertFatal( pAudioFile != NULL, "Cannot use a NULL audio filename." );

    // Fetch audio filename.
    pAudioFile = StringTable->insert( pAudioFile );

    // Ignore no change,
    if ( pAudioFile == mAudioFile )
        return;

    // Update.
    mAudioFile = getOwned() ? expandAssetFilePath( pAudioFile ) : pAudioFile;

    // Refresh the asset.
    refreshAsset();
}

//--------------------------------------------------------------------------

void AudioAsset::setVolume( const F32 volume )
{
    // Ignore no change.
    if ( mIsEqual( volume, mDescription.mVolume ) )
        return;

    // Update.
    mDescription.mVolume = mClampF(volume, 0.0f, 1.0f);;

    // Refresh the asset.
    refreshAsset();
}

//--------------------------------------------------------------------------

void AudioAsset::setVolumeChannel( const S32 volumeChannel )
{
    // Ignore no change.
    if ( volumeChannel == mDescription.mVolumeChannel )
        return;

    // Update.
    mDescription.mVolumeChannel = mClamp( volumeChannel, 0, Audio::AudioVolumeChannels-1 );

    // Refresh the asset.
    refreshAsset();
}

//--------------------------------------------------------------------------

void AudioAsset::setLooping( const bool looping )
{
    // Ignore no change.
    if ( looping == mDescription.mIsLooping )
        return;

    // Update.
    mDescription.mIsLooping = looping;

    // Refresh the asset.
    refreshAsset();
}


//--------------------------------------------------------------------------

void AudioAsset::setStreaming( const bool streaming )
{
    // Ignore no change.
    if ( streaming == mDescription.mIsStreaming )
        return;

    // UPdate.
    mDescription.mIsStreaming = streaming;

    // Refresh the asset.
    refreshAsset();
}

//--------------------------------------------------------------------------

void AudioAsset::setDescription( const Audio::Description& audioDescription )
{
    // Update.
    mDescription = audioDescription;

    // Refresh the asset.
    refreshAsset();
}

//-----------------------------------------------------------------------------

void AudioAsset::onTamlPreWrite( void )
{
    // Call parent.
    Parent::onTamlPreWrite();

    // Ensure the audio-file is collapsed.
    mAudioFile = collapseAssetFilePath( mAudioFile );
}

//-----------------------------------------------------------------------------

void AudioAsset::onTamlPostWrite( void )
{
    // Call parent.
    Parent::onTamlPostWrite();

    // Ensure the audio-file is expanded.
    mAudioFile = expandAssetFilePath( mAudioFile );
}


