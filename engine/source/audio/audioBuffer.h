//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _AUDIOBUFFER_H_
#define _AUDIOBUFFER_H_

#ifndef _PLATFORM_H_
#include "platform/platform.h"
#endif
#ifndef _PLATFORMAL_H_
#include "platform/platformAL.h"
#endif
#ifndef _RESMANAGER_H_
#include "io/resource/resourceManager.h"
#endif

//--------------------------------------------------------------------------

class AudioBuffer: public ResourceInstance
{
   friend class AudioThread;

private:
   StringTableEntry  mFilename;
   bool              mLoading;
   ALuint            malBuffer;

   bool readRIFFchunk(Stream &s, const char *seekLabel, U32 *size);
   bool readWAV(ResourceObject *obj);

public:
   AudioBuffer(StringTableEntry filename);
   ~AudioBuffer();
   ALuint getALBuffer();
   bool isLoading() {return(mLoading);}

   static Resource<AudioBuffer> find(const char *filename);
   static ResourceInstance* construct(Stream& stream);

};


#endif  // _H_AUDIOBUFFER_
