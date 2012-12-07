//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _ASSET_FUNCTIONS_H_
#include "assetFunctions.h"
#endif

#ifndef _ASSET_MANAGER_H
#include "assets/assetManager.h"
#endif

#ifndef _IMAGE_ASSET_H_
#include "2d/assets/ImageAsset.h"
#endif

#ifndef _ANIMATION_ASSET_H_
#include "2d/assets/AnimationAsset.h"
#endif

#ifndef _AUDIO_ASSET_H_
#include "audio/audioAsset.h"
#endif

//-----------------------------------------------------------------------------

ConsoleFunction( getImageCellCount, S32, 2, 2, "(imageAssetId) - Returns the number of cells in an ImageAsset.")
{
	const char* type = AssetDatabase.getAssetType(argv[1]);

	if (dStrcmp(type, "ImageAsset"))
	{
		Con::errorf("getImageCellCount() - Asset passed in was not an ImageAsset");
		return 0;
	}

	ImageAsset* pImageAsset = AssetDatabase.acquireAsset<ImageAsset>( argv[1] );

	S32 cellCount = pImageAsset->getFrameCount();
	
	AssetDatabase.releaseAsset(argv[1]);

	return cellCount;
}

//-----------------------------------------------------------------------------

ConsoleFunction( getAssetSourceFile, const char*, 2, 2, "(assetId) - Returns the source file for a provided asset")
{
	const char* type = AssetDatabase.getAssetType(argv[1]);
	char* pBuffer;

	if (!dStrcmp(type, "ImageAsset"))
	{
		ImageAsset* pImageAsset = AssetDatabase.acquireAsset<ImageAsset>( argv[1] );
		pBuffer = Con::getReturnBuffer(pImageAsset->getImageFile());
		AssetDatabase.releaseAsset(argv[1]);
		return pBuffer;
	}
	else if (!dStrcmp(type, "AudioAsset"))
	{
		AudioAsset* pAudioAsset = AssetDatabase.acquireAsset<AudioAsset>( argv[1] );
		pBuffer = Con::getReturnBuffer(pAudioAsset->getAudioFile());
		AssetDatabase.releaseAsset(argv[1]);
		return pBuffer;
	}
	else
	{
		return "";
	}	
}

//-----------------------------------------------------------------------------

ConsoleFunction( getAnimationAssetFrameCount, S32, 2, 2, "(animationAssetId) - Returns the number of frames in an AnimationAsset")
{
	const char* type = AssetDatabase.getAssetType(argv[1]);

	if (dStrcmp(type, "AnimationAsset"))
	{
		Con::errorf("getAnimationAssetFrameCount() - Asset passed in was not an AnimationAsset");
		return 0;
	}

	AnimationAsset* pAnimationAsset = AssetDatabase.acquireAsset<AnimationAsset>( argv[1] );

	S32 frameCount = pAnimationAsset->getValidatedAnimationFrames().size();
	
	AssetDatabase.releaseAsset(argv[1]);

	return frameCount;
}

//-----------------------------------------------------------------------------