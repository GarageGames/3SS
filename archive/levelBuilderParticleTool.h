//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef TORQUE_PLAYER

#ifndef _LEVELBUILDERPARTICLETOOL_H_
#define _LEVELBUILDERPARTICLETOOL_H_

#ifndef _SCENE_WINDOW_H_
#include "2d/camera/SceneWindow.h"
#endif

#ifndef _PARTICLE_EFFECT_H_
#include "2d/sceneobject/ParticleEffect.h"
#endif

#ifndef _LEVELBUILDERCREATETOOL_H_
#include "editor/levelBuilderCreateTool.h"
#endif

#ifndef _CONSOLETYPES_H_
#include "console/consoleTypes.h"
#endif

//-----------------------------------------------------------------------------
// LevelBuilderParticleTool
//-----------------------------------------------------------------------------
class LevelBuilderParticleTool : public LevelBuilderCreateTool
{
   typedef LevelBuilderCreateTool Parent;

private:
   StringTableEntry mEffectName;

protected:
   virtual SceneObject* createObject();
   virtual void showObject();
  
public:
   LevelBuilderParticleTool();
   ~LevelBuilderParticleTool();

   void setEffect(const char* name) { mEffectName = StringTable->insert(name); };

   // Declare our Console Object
   DECLARE_CONOBJECT(LevelBuilderParticleTool);
};

#endif


#endif // TORQUE_TOOLS
