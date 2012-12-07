//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _IOSSTREAMSOURCE_H_
#define _IOSSTREAMSOURCE_H_

#include "sim/simBase.h"

class iOSStreamSource: public SimObject
{
	public:
	char * mFilename;
	iOSStreamSource(const char *filename);
	~iOSStreamSource();
	bool isPlaying();
	bool start( bool loop = false );
	bool stop();
};

#endif // _AUDIOSTREAMSOURCE_H_
