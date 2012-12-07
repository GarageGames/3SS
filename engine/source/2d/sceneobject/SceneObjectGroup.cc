//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "2d/sceneobject/SceneObjectGroup.h"
#include "2d/sceneobject/SceneObject.h"
#include "console/consoleTypes.h"

IMPLEMENT_CONOBJECT(SceneObjectGroup);

//---------------------------------------------------------------------------------------------
// Constructor
//---------------------------------------------------------------------------------------------
SceneObjectGroup::SceneObjectGroup() : mSceneObjectGroup(NULL),
                                             mScene(NULL)
{
   mNSLinkMask = LinkSuperClassName | LinkClassName;
}

//---------------------------------------------------------------------------------------------
// Destructor
//---------------------------------------------------------------------------------------------
SceneObjectGroup::~SceneObjectGroup()
{
}

bool SceneObjectGroup::onAdd()
{
    // Call Parent.
    if(!Parent::onAdd())
        return false;

    // Synchronize Namespace's
    linkNamespaces();

    // tell the scripts
    Con::executef(this, 1, "onAdd");

    return true;
}

void SceneObjectGroup::onRemove()
{
    // tell the scripts
    Con::executef(this, 1, "onRemove");

    // Restore NameSpace's
    unlinkNamespaces();

    // Call Parent.
    Parent::onRemove();
}

// Get a scene object or scene group's parent group.
SceneObjectGroup* SceneObjectGroup::getSceneObjectGroup(SimObject* object)
{
   SceneObject* sceneObject = dynamic_cast<SceneObject*>(object);
   if (sceneObject)
      return sceneObject->getSceneObjectGroup();

   else
   {
      SceneObjectGroup* group = dynamic_cast<SceneObjectGroup*>(object);
      if (group)
         return group->getSceneObjectGroup();
   }

   return NULL;
}

// Set a scene object or scene group's parent group.
void SceneObjectGroup::setSceneObjectGroup(SimObject* object, SceneObjectGroup* group)
{
   SceneObject* sceneObject = dynamic_cast<SceneObject*>(object);
   if (sceneObject)
      sceneObject->mpSceneObjectGroup = group;

   else
   {
      SceneObjectGroup* sceneGroup = dynamic_cast<SceneObjectGroup*>(object);
      if (sceneGroup)
         sceneGroup->mSceneObjectGroup = group;
   }
}

// Get a scene object or scene group's scene.
Scene* SceneObjectGroup::getScene(SimObject* object)
{
   SceneObject* sceneObject = dynamic_cast<SceneObject*>(object);
   if (sceneObject)
      return sceneObject->getScene();

   else
   {
      SceneObjectGroup* group = dynamic_cast<SceneObjectGroup*>(object);
      if (group)
         return group->getScene();
   }

   return NULL;
}

void SceneObjectGroup::addObject(SimObject* object)
{
   if (object == this)
   {
      Con::errorf("SceneObjectGroup::addObject - (%d) can't add self!", getId());
      return;
   }

   SceneObjectGroup* testGroup = dynamic_cast<SceneObjectGroup*>(object);
   if (testGroup)
   {
      if (testGroup->findChildObject(this))
      {
         Con::errorf("SceneObjectGroup::addObject - (%d) can't add parent!", object->getId());
         return;
      }
   }

   SceneObjectGroup* parentGroup = SceneObjectGroup::getSceneObjectGroup(object);
   Scene* parentScene = SceneObjectGroup::getScene(object);

   if (parentGroup != this)
   {
      if (parentGroup)
      {
         parentGroup->removeObject(object);
      }
      else if (parentScene)
      {
         parentScene->removeObject(object);
      }

      SceneObjectGroup::setSceneObjectGroup(object, this);
      Parent::addObject(object);

      if (mScene && (parentScene != mScene))
         mScene->addToScene(object);
   }
}

void SceneObjectGroup::removeObject(SimObject* object)
{
   if (SceneObjectGroup::getSceneObjectGroup(object) == this)
   {
      SceneObjectGroup::setSceneObjectGroup(object, NULL);
      Parent::removeObject(object);
   }
}

bool SceneObjectGroup::findChildObject(SimObject* searchObject) const
{
   for (S32 i = 0; i < size(); i++)
   {
      SimObject* object = at(i);

      if (object == searchObject)
         return true;

      SceneObjectGroup* group = dynamic_cast<SceneObjectGroup*>(object);
      if (group)
      {
         if (group->findChildObject(searchObject))
            return true;
      }
   }

   // If we make it here, no.
   return false;
}
