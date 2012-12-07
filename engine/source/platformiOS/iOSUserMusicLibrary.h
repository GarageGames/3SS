//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

/*Functions that we expose to the scripts are the following 
	- Play
	- Pause
	- Stop
	- Seek
	- Skip to next item
	- Skip to previous item
	- Skip to beginning
*/

#ifdef TORQUE_ALLOW_MUSICPLAYER

void createMusicPlayer();
void destroyMusicPlayer();
void updateVolume();

#endif //TORQUE_ALLOW_MUSICPLAYER