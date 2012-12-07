//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _CONSOLETYPES_H_
#include "console/consoleTypes.h"
#endif

#ifndef _SCENE_OBJECT_H_
#include "2d/sceneobject/SceneObject.h"
#endif

#ifndef  _SCENE_OBJECT_SET_H_
#include "SceneObjectSet.h"
#endif

// Script bindings.
#include "SceneObjectSet_ScriptBinding.h"

//------------------------------------------------------------------------------

IMPLEMENT_CONOBJECT(SceneObjectSet);

//------------------------------------------------------------------------------

extern EnumTable collisionDetectionTable;
extern EnumTable collisionResponseTable;
extern EnumTable srcBlendFactorTable;
extern EnumTable dstBlendFactorTable;

//------------------------------------------------------------------------------

SceneObjectSet::SceneObjectSet() : mObjectRect(0.0f, 0.0f, 0.0f, 0.0f),
                                         mSceneGroup(0),
                                         mSceneLayer(0),
                                         mFlipX(false),
                                         mFlipY(false),
                                         mAngle(0.0f)
{
}

//------------------------------------------------------------------------------

SceneObjectSet::~SceneObjectSet()
{
}

//------------------------------------------------------------------------------

void SceneObjectSet::addObject(SimObject* object)
{
   Parent::addObject(object);

   mAngle = 0.0f;
   calculateObjectRect();
}

//------------------------------------------------------------------------------

void SceneObjectSet::removeObject(SimObject* object)
{
   Parent::removeObject(object);

   mAngle = 0.0f;
   calculateObjectRect();
}

//------------------------------------------------------------------------------

void SceneObjectSet::calculateObjectRect()
{
   Vector<SceneObject*> objects;
   getSceneObjects(objects);

   if (objects.size())
       mObjectRect = objects[0]->getAABBRectangle();
   else
      mObjectRect = RectF(0.0f, 0.0f, 0.0f, 0.0f);

    F32 minX = mObjectRect.point.x;
    F32 minY = mObjectRect.point.y;
    F32 maxX = mObjectRect.point.x + mObjectRect.extent.x;
    F32 maxY = mObjectRect.point.y + mObjectRect.extent.y;

    Vector2 lowerBound = mObjectRect.point;
    Vector2 upperBound = lowerBound + mObjectRect.extent;

   for (S32 i = 1; i < objects.size(); i++)
   {
       const RectF aabb = objects[i]->getAABBRectangle();
       
      Vector2 lowerBound = aabb.point;
      Vector2 upperBound = lowerBound + aabb.extent;

      minX = getMin(lowerBound.x, minX);
      maxX = getMax(upperBound.x, maxX);
      minY = getMin(lowerBound.y, minY);
      maxY = getMax(upperBound.y, maxY);
   }

   mObjectRect = RectF(minX, minY, maxX - minX, maxY - minY);
}

//------------------------------------------------------------------------------

void SceneObjectSet::getSceneObjects(Vector<SceneObject*>& objects) const
{
   for(S32 i = 0; i < size(); i++)
   {
      SimObject* object = at(i);

      SceneObject* sceneObject = dynamic_cast<SceneObject*>(object);
      if(sceneObject)
         objects.push_back(sceneObject);

      else
      {
         SceneObjectGroup* sceneObjectGroup = dynamic_cast<SceneObjectGroup*>(object);
         if(sceneObjectGroup)
            sceneObjectGroup->getSceneObjects(objects);

         else
         {
            SceneObjectSet* sceneObjectSet = dynamic_cast<SceneObjectSet*>(object);
            if(sceneObjectSet)
               sceneObjectSet->getSceneObjects(objects);
         }
      }
   }
}

//------------------------------------------------------------------------------

void SceneObjectSet::setPosition(Vector2 position)
{
   Vector<SceneObject*> objects;
   getSceneObjects(objects);

   Vector2 midPoint = mObjectRect.point + (mObjectRect.extent * 0.5);

   for (S32 i = 0; i < objects.size(); i++)
   {
      SceneObject* object = objects[i];
      object->setPosition((object->getPosition() - midPoint) + position);
   }

   calculateObjectRect();
}

//------------------------------------------------------------------------------

void SceneObjectSet::setSize(Vector2 size)
{
   Vector<SceneObject*> objects;
   getSceneObjects(objects);

   Vector<Vector2> positions;

   const F32 halfPi = b2_pi * 0.5f;

   for (S32 i = 0; i < objects.size(); i++)
   {
      SceneObject* object = objects[i];

      // Objects rotated 90 or 270 need the axes switched.
      Vector2 newSize = object->getSize().div(mObjectRect.extent).mult(size);

      if (mIsEqual(object->getAngle(), halfPi) || mIsEqual(object->getAngle(), -halfPi))
      {
         Vector2 rotatedObjectSize;
         rotatedObjectSize.x = object->getSize().y;
         rotatedObjectSize.y = object->getSize().x;
         newSize = rotatedObjectSize.div(mObjectRect.extent).mult(size);
         F32 temp = newSize.x;
         newSize.x = newSize.y;
         newSize.y = temp;
      }

      positions.push_back((object->getPosition() - getPosition()).div(mObjectRect.extent));
      object->setPosition(getPosition());

      // Resize object if not auto-sizing.
      if ( !object->getAutoSizing() )
        object->setSize(newSize);
   }

   calculateObjectRect();
   
   for (S32 i = 0; i < objects.size(); i++)
   {
      SceneObject* object = objects[i];

      object->setPosition(positions[i].mult(size) + getPosition());
   }
   
   calculateObjectRect();
}

//------------------------------------------------------------------------------

void SceneObjectSet::setAngle(F32 radians)
{
   rotate(radians - mAngle);
}

//------------------------------------------------------------------------------

void SceneObjectSet::rotate(F32 radians)
{
   Vector<SceneObject*> objects;
   getSceneObjects(objects);

   for (S32 i = 0; i < objects.size(); i++)
   {
      SceneObject* object = objects[i];

      Vector2 offset = object->getPosition() - getPosition();
      F32 distance = offset.Length();
      F32 rotationOffset = mAtan(offset.x, offset.y);
      Vector2 position = Vector2(distance * mSin(rotationOffset - radians), distance * mCos(rotationOffset - radians));
      object->setPosition(getPosition() + position);

      object->setAngle(object->getAngle() + radians);
   }
   
   mAngle += radians;
   calculateObjectRect();
}

//------------------------------------------------------------------------------

void SceneObjectSet::flipX()
{
   Vector<SceneObject*> objects;
   getSceneObjects(objects);

   for (S32 i = 0; i < objects.size(); i++)
   {
      SceneObject* object = objects[i];

      object->setFlip(!object->getFlipX(), object->getFlipY());
      Vector2 offset = object->getPosition() - getPosition();
      offset.x = -offset.x;
      object->setPosition(getPosition() + offset);
   }

   mFlipX = !mFlipX;
}

//------------------------------------------------------------------------------

void SceneObjectSet::flipY()
{
   Vector<SceneObject*> objects;
   getSceneObjects(objects);

   for (S32 i = 0; i < objects.size(); i++)
   {
      SceneObject* object = objects[i];

      object->setFlip(object->getFlipX(), !object->getFlipY());
      Vector2 offset = object->getPosition() - getPosition();
      offset.y = -offset.y;
      object->setPosition(getPosition() + offset);
   }

   mFlipY = !mFlipY;
}

//------------------------------------------------------------------------------

void SceneObjectSet::setSceneGroup(U32 group)
{
   // Don't want to update unless a change actually occurs. Otherwise, object specific
   // changes could be reset.
   if (mSceneGroup == group)
      return;

   mSceneGroup = group;

   Vector<SceneObject*> objects;
   getSceneObjects(objects);

   for (S32 i = 0; i < objects.size(); i++)
   {
      SceneObject* object = objects[i];
      object->setSceneGroup(group);
   }
}

//------------------------------------------------------------------------------

void SceneObjectSet::setSceneLayer(U32 layer)
{
   if (mSceneLayer == layer)
      return;

   mSceneLayer = layer;

   Vector<SceneObject*> objects;
   getSceneObjects(objects);

   for (S32 i = 0; i < objects.size(); i++)
   {
      SceneObject* object = objects[i];
      object->setSceneLayer(layer);
   }
}

//------------------------------------------------------------------------------

S32 SceneObjectSet::getSceneGroup()
{
   Vector<SceneObject*> objects;
   getSceneObjects(objects);

   for (S32 i = 0; i < objects.size(); i++)
   {
      SceneObject* object = objects[i];

      if (i == 0) mSceneGroup = object->getSceneGroup();
      else
      {
         if (object->getSceneGroup() != mSceneGroup)
            mSceneGroup = 0;
      }
   }

   return mSceneGroup;
}

//------------------------------------------------------------------------------

S32 SceneObjectSet::getSceneLayer()
{
   Vector<SceneObject*> objects;
   getSceneObjects(objects);

   for (S32 i = 0; i < objects.size(); i++)
   {
      SceneObject* object = objects[i];

      if (i == 0) mSceneLayer = object->getSceneLayer();
      else
      {
         if (object->getSceneLayer() != mSceneLayer)
            mSceneLayer = 0;
      }
   }

   return mSceneLayer;
}

//------------------------------------------------------------------------------

F32 SceneObjectSet::getAngle() const
{
   if (size() == 1)
   {
      SceneObject* object = dynamic_cast<SceneObject*>(at(0));
      if (object)
         return object->getAngle();
   }
   return mAngle;
}
