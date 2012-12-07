//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef  _SCENE_OBJECT_SET_H_
#define  _SCENE_OBJECT_SET_H_

#include "sim/simBase.h"
#include "collection/vector.h"
#include "2d/core/Vector2.h"
#include "graphics/color.h"

//------------------------------------------------------------------------------

class SceneObject;

//------------------------------------------------------------------------------

class SceneObjectSet : public SimSet
{
private:
   typedef SimSet Parent;

   RectF mObjectRect;
   F32 mAngle;

   bool mFlipX;
   bool mFlipY;
   U32 mSceneGroup;
   U32 mSceneLayer;

public:
   DECLARE_CONOBJECT(SceneObjectSet);

   SceneObjectSet();
   ~SceneObjectSet();

   void setPosition(Vector2 position);
   void setSize(Vector2 size);
   void setAngle(F32 radians);
   void rotate(F32 radians);
   void flipX();
   void flipY();
   void setSceneGroup(U32 group);
   void setSceneLayer(U32 layer);

   void addObject(SimObject* object);
   void removeObject(SimObject* object);
   
   void getSceneObjects(Vector<SceneObject*>& objects) const;

   void calculateObjectRect();
   RectF getBoundingRect() const { return mObjectRect; };
   Vector2 getPosition() const { Point2F temp = mObjectRect.point + (mObjectRect.extent * 0.5f); return Vector2(temp.x, temp.y); };
   Vector2 getSize() const { return mObjectRect.extent; };
   F32 getAngle() const;
   bool getFlipX() const { return mFlipX; };
   bool getFlipY() const { return mFlipY; };
   S32 getSceneGroup();
   S32 getSceneLayer();
};

#endif // _SCENE_OBJECT_SET_H_
