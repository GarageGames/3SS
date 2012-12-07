//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

//--------------------------------------
// audioStreamSource.cc
// implementation of streaming audio source
//
// Kurtis Seebaldt
//--------------------------------------

#include "audio/audioStreamSourceFactory.h"

#include "audio/wavStreamSource.h"

AudioStreamSource* AudioStreamSourceFactory::getNewInstance(const char *filename)
{
	S32 len = dStrlen(filename);
	if(len > 3 && !dStricmp(filename + len - 4, ".wav"))
		return new WavStreamSource(filename);
	
	return NULL;
}
