//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef  _SCENE_OBJECT_GROUP_H_
#define  _SCENE_OBJECT_GROUP_H_

#include "SceneObjectSet.h"
#include "2d/core/Vector2.h"
#include "collection/vector.h"
#include "graphics/color.h"

//------------------------------------------------------------------------------

class Scene;

//------------------------------------------------------------------------------

class SceneObjectGroup: public SceneObjectSet
{
private:
   typedef SceneObjectSet Parent;

   Scene* mScene;
   SceneObjectGroup* mSceneObjectGroup;

   static void setSceneObjectGroup(SimObject* object, SceneObjectGroup* group);

public:
   DECLARE_CONOBJECT(SceneObjectGroup);

   friend class Scene;

   SceneObjectGroup();
   ~SceneObjectGroup();

   virtual bool onAdd();
   virtual void onRemove();

   static SceneObjectGroup* getSceneObjectGroup(SimObject* object);
   static Scene* getScene(SimObject* object);

   void addObject(SimObject* object);
   void removeObject(SimObject* object);
   bool findChildObject(SimObject* object) const;
   
   Scene* getScene() const { return mScene; };
   SceneObjectGroup* getSceneObjectGroup() const { return mSceneObjectGroup; };
};

#endif // _SCENE_OBJECT_GROUP_H_
