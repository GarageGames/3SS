//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "iOSStreamSource.h"
#include "SoundEngine.h"

#define BUFFERSIZE 32768

iOSStreamSource::iOSStreamSource(const char *filename)  {
	this->registerObject();
	int len = dStrlen( filename );
	mFilename = new char[len + 1];
	dStrcpy( mFilename, filename );
	//SoundEngine::SoundEngine_LoadBackgroundMusicTrack( mFilename, true, false );
}

iOSStreamSource::~iOSStreamSource() {
	stop();
	delete [] mFilename;
	//SoundEngine::SoundEngine_UnloadBackgroundMusicTrack();
}

bool iOSStreamSource::isPlaying() {
	return true;
}

bool iOSStreamSource::start( bool loop ) {
	SoundEngine::SoundEngine_LoadBackgroundMusicTrack( mFilename, true, false );
	SoundEngine::SoundEngine_StartBackgroundMusic();
	if( !loop ) {
		//stop at end
		SoundEngine::SoundEngine_StopBackgroundMusic( true );
		Con::executef(1,"oniOSStreamEnd");
	}
	return true;
}

bool iOSStreamSource::stop() {
	//false == stop now
	SoundEngine::SoundEngine_StopBackgroundMusic( false );
	SoundEngine::SoundEngine_UnloadBackgroundMusicTrack();
	return true;
}
