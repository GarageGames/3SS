//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef TORQUE_PLAYER


#include "console/console.h"
#include "editor/levelBuilderParticleTool.h"

// Implement Our Console Object
IMPLEMENT_CONOBJECT(LevelBuilderParticleTool);

LevelBuilderParticleTool::LevelBuilderParticleTool() : LevelBuilderCreateTool(),
                                                       mEffectName(NULL)
{
   // Set our tool name
   mToolName = StringTable->insert("Particle Effect Tool");
}

LevelBuilderParticleTool::~LevelBuilderParticleTool()
{
}

SceneObject* LevelBuilderParticleTool::createObject()
{
   ParticleEffect* effect = dynamic_cast<ParticleEffect*>(ConsoleObject::create("ParticleEffect"));

   return effect;
}

void LevelBuilderParticleTool::showObject()
{
   mCreatedObject->setVisible(true);
   ParticleEffect* effect = dynamic_cast<ParticleEffect*>(mCreatedObject);
   if (effect)
   {
      // Loading and playing an effect messes with size and position so we need to save and reset.
      Vector2 size = effect->getSize();
      Vector2 position = effect->getPosition();

      effect->loadEffect(mEffectName);
      effect->playEffect(true);

      effect->setSize(size);
      effect->setPosition(position);
   }
}

ConsoleMethod(LevelBuilderParticleTool, setEffect, void, 3, 3, "Sets the effect file for the created particle effects.")
{
   if (Platform::isFile(argv[2]))
      object->setEffect(argv[2]);
   else
      Con::warnf("LevelBuilderParticleTool::setEffect - Invalid effect file: %s", argv[2]);
}


#endif // TORQUE_TOOLS
