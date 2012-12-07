//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _AUDIO_H_
#define _AUDIO_H_

#ifndef _PLATFORMAUDIO_H_
#include "platform/platformAudio.h"
#endif

#ifndef _AUDIODATABLOCK_H_
#include "audio/audioDataBlock.h"
#endif

#ifndef _AUDIO_ASSET_H_
#include "audio/audioAsset.h"
#endif

//-Mat default sample rate, change as needed
#define DEFAULT_SOUND_OUTPUT_RATE		44100
bool alxCheckError(const char*, const char* );

#endif  // _H_AUDIO_
