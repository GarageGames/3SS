//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

new AudioDescription(NonLoopingAudioEffectDescription)
{
   volume = 1.0;
   isLooping = false;
   is3D = false;
   type = $effectsAudioType;
};

new AudioDescription(LoopingAudioEffectDescription)
{
   volume = 1.0;
   isLooping = true;
   is3D = false;
   type = $effectsAudioType;
};

new AudioDescription(NonLoopingMusicDescription)
{
   volume = 1.0;
   isLooping = false;
   is3D = false;
   type = $musicAudioType;
};

new AudioDescription(LoopingMusicDescription)
{
   volume = 1.0;
   isLooping = true;
   is3D = false;
   type = $musicAudioType;
};