//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef TORQUE_PLAYER


#include "console/console.h"
#include "editor/levelBuilderBaseEditTool.h"

// Implement Our Console Object
IMPLEMENT_CONOBJECT(LevelBuilderBaseEditTool);

//-----------------------------------------------------------------------------
// Constructor
//-----------------------------------------------------------------------------
LevelBuilderBaseEditTool::LevelBuilderBaseEditTool() : LevelBuilderBaseTool(),
                                                       mSizingState(0)
{
   // Set our tool name
   mToolName = StringTable->insert("Edit Base Tool");
}

//-----------------------------------------------------------------------------
// Destructor
//-----------------------------------------------------------------------------
LevelBuilderBaseEditTool::~LevelBuilderBaseEditTool()
{
}

//-----------------------------------------------------------------------------
// rotate
//-----------------------------------------------------------------------------
void LevelBuilderBaseEditTool::rotate(LevelBuilderSceneEdit* sceneEdit, F32 rotation, Vector2 rotationVector, Vector2 newVector, F32& newRotation)
{
   PROFILE_SCOPE(LevelBuilderBaseEditTool_rotate);

   AssertFatal(sceneEdit, "LevelBuilderBaseEditTool - Editing objects without a valid scene edit context.");

   F32 oldRotationOffset = -mAtan(rotationVector.x, rotationVector.y) - rotation;
   newRotation = -mAtan(newVector.x, newVector.y) - oldRotationOffset;

   if (!sceneEdit->getRotationSnap())
      return;

   // Grab the snap values.
   F32 snapThreshold = mDegToRad(sceneEdit->getRotationSnapThreshold());
   F32 snapAngle = mDegToRad(sceneEdit->getRotationSnapAngle());

   // Snap the new rotation.
   F32 closestSnap = mFloor((newRotation / snapAngle) + 0.5f) * snapAngle;
   if (mFabs(newRotation - closestSnap) < snapThreshold)
      newRotation = closestSnap;

   // Always snap to 90 degree increments.
   F32 piOver2 = (F32)M_PI * 0.5f;
   if (mNotZero(piOver2 / snapAngle))
   {
      F32 closest90 = mFloor((newRotation / piOver2) + 0.5f) * piOver2;
      if (mFabs(newRotation - closest90) < snapThreshold)
         newRotation = closest90;
   }
}

//-----------------------------------------------------------------------------
// move
//-----------------------------------------------------------------------------
void LevelBuilderBaseEditTool::move(LevelBuilderSceneEdit* sceneEdit, Vector2 size, Vector2 position, Vector2& finalPosition)
{
   PROFILE_SCOPE(LevelBuilderBaseEditTool_move);

   AssertFatal(sceneEdit, "LevelBuilderBaseEditTool - Editing objects without a valid scene edit context.");

   Vector2 halfSize = size * 0.5;
   Vector2 upperLeft = position - halfSize;
   Vector2 lowerRight = position + halfSize;

   // The final position is just the position, unless one of the edges of the
   // rectangle are within snapThreshold of a grid line.
   finalPosition = position;
   F32 snapThreshold = sceneEdit->getSnapThreshold();
   if (sceneEdit->isGridSnapX() && sceneEdit->getSnapToGridX())
   {
      F32 xSnap = sceneEdit->getGridSnapX();

      // Snap line closest to the left.
      F32 closestX = mFloor((upperLeft.x / xSnap) + 0.5f) * xSnap;
      F32 distance = mFabs(upperLeft.x - closestX);
      
      // Snap line closest to the right.
      F32 closestX2 = mFloor((lowerRight.x / xSnap) + 0.5f) * xSnap;
      F32 distance2 = mFabs(lowerRight.x - closestX2);

      // Guide closest to the left.
      F32 closestGuide = sceneEdit->getClosestXGuide( upperLeft.x );
      F32 distance3 = mFabs( upperLeft.x - closestGuide );

      // Guide closest to the right.
      F32 closestGuide2 = sceneEdit->getClosestXGuide( lowerRight.x );
      F32 distance4 = mFabs( lowerRight.x - closestGuide2 );

      // Grab the smallest distance.
      F32 smallest = distance;
      F32 offset = closestX;
      S32 side = 1;
      if( distance2 < smallest )
      {
         smallest = distance2;
         offset = closestX2;
         side = -1;
      }
      if( distance3 < smallest )
      {
         smallest = distance3;
         offset = closestGuide;
         side = 1;
      }
      if( distance4 < smallest )
      {
         smallest = distance4;
         offset = closestGuide2;
         side = -1;
      }

      if (smallest < snapThreshold)
         finalPosition.x = offset + ( halfSize.x * side );
   }
   
   if (sceneEdit->isGridSnapY() && sceneEdit->getSnapToGridY())
   {
      F32 ySnap = sceneEdit->getGridSnapY();
      F32 closestY = mFloor((upperLeft.y / ySnap) + 0.5f) * ySnap;
      F32 distance = mFabs(upperLeft.y - closestY);
      
      F32 closestY2 = mFloor((lowerRight.y / ySnap) + 0.5f) * ySnap;
      F32 distance2 = mFabs(lowerRight.y - closestY2);

      // Guide closest to the top.
      F32 closestGuide = sceneEdit->getClosestYGuide( upperLeft.y );
      F32 distance3 = mFabs( upperLeft.y - closestGuide );

      // Guide closest to the bottom.
      F32 closestGuide2 = sceneEdit->getClosestYGuide( lowerRight.y );
      F32 distance4 = mFabs( lowerRight.y - closestGuide2 );

      // Grab the smallest distance.
      F32 smallest = distance;
      F32 offset = closestY;
      S32 side = 1;
      if( distance2 < smallest )
      {
         smallest = distance2;
         offset = closestY2;
         side = -1;
      }
      if( distance3 < smallest )
      {
         smallest = distance3;
         offset = closestGuide;
         side = 1;
      }
      if( distance4 < smallest )
      {
         smallest = distance4;
         offset = closestGuide2;
         side = -1;
      }

      if (smallest < snapThreshold)
         finalPosition.y = offset + ( halfSize.y * side );
   }
}

//-----------------------------------------------------------------------------
// scale
//-----------------------------------------------------------------------------
void LevelBuilderBaseEditTool::scale(LevelBuilderSceneEdit* sceneEdit, Vector2 size, Vector2 pos, Vector2 mousePoint2D, bool uniform, bool maintainAR, F32 ar, Vector2& newSize, Vector2& newPosition, bool& flipX, bool& flipY)
{
   PROFILE_SCOPE(LevelBuilderBaseEditTool_scale);

   AssertFatal(sceneEdit, "LevelBuilderBaseEditTool - Editing objects without a valid scene edit context.");

   //F32 ar = size.x / size.y;

   // Snap To Grid
   Vector2 mousePoint = mousePoint2D;
   F32 snapThreshold = sceneEdit->getSnapThreshold();
   if (sceneEdit->getSnapToGridX())
   {
      F32 xSnap = sceneEdit->getGridSnapX();
      F32 closestX = mFloor((mousePoint.x / xSnap) + 0.5f) * xSnap;
      if (mFabs(mousePoint.x - closestX) < snapThreshold)
         mousePoint.x = closestX;
   }
   
   if (sceneEdit->getSnapToGridY())
   {
      F32 ySnap = sceneEdit->getGridSnapY();
      F32 closestY = mFloor((mousePoint.y / ySnap) + 0.5f) * ySnap;
      if (mFabs(mousePoint.y - closestY) < snapThreshold)
         mousePoint.y = closestY;
   }

   // Find The Scale Change
   Vector2 scaleDelta = Vector2(0.0f, 0.0f);
   if( mSizingState & SizingLeft )
      scaleDelta.x = mousePoint.x - (pos.x - (size.x * 0.5f));
   else if( mSizingState & SizingRight )
      scaleDelta.x = mousePoint.x - (pos.x + (size.x * 0.5f));

   if( mSizingState & SizingTop )
      scaleDelta.y = mousePoint.y - (pos.y + (size.y * 0.5f));
   else if( mSizingState & SizingBottom )
      scaleDelta.y = mousePoint.y - (pos.y - (size.y * 0.5f));

   if (uniform)
      scaleDelta *= 2.0f;

   // Apply The Scale Change
   newSize = size;
   newPosition = pos;
   if( mSizingState & SizingLeft )
   {
      newSize.x -= scaleDelta.x;
      newPosition.x += scaleDelta.x * 0.5f;
   }
   else if( mSizingState & SizingRight )
   {
      newSize.x += scaleDelta.x;
      newPosition.x += scaleDelta.x * 0.5f;
   }

   if( mSizingState & SizingTop )
   {
      newSize.y += scaleDelta.y;
      newPosition.y += scaleDelta.y * 0.5f;
   }
   else if( mSizingState & SizingBottom )
   {
      newSize.y -= scaleDelta.y;
      newPosition.y += scaleDelta.y * 0.5f;
   }

   // If the new size is small, don't change anything. This needs some fixing up still.
   if (((mFabs(newSize.x) < 0.001f) && (sceneEdit->getSnapToGridX())) ||
       ((mFabs(newSize.y) < 0.001f) && (sceneEdit->getSnapToGridY())))
   {
      newSize = size;
      newPosition = pos;
      flipX = flipY = false;
      return;
   }

   // FlipX
   flipX = flipY = false;
   if (newSize.x < 0.0f)
   {
      if (mSizingState & SizingLeft)
      {
         mSizingState &= ~SizingLeft;
         mSizingState |= SizingRight;
      }
      else if (mSizingState & SizingRight)
      {
         mSizingState &= ~SizingRight;
         mSizingState |= SizingLeft;
      }
      flipX = true;
      newSize.x = -newSize.x;
   }

   // FlipY
   if (newSize.y < 0.0f)
   {
      if (mSizingState & SizingTop)
      {
         mSizingState &= ~SizingTop;
         mSizingState |= SizingBottom;
      }
      else if (mSizingState & SizingBottom)
      {
         mSizingState &= ~SizingBottom;
         mSizingState |= SizingTop;
      }
      flipY = true;
      newSize.y = -newSize.y;
   }

   // Maintain Aspect Ratio
   if (maintainAR)
   {
      F32 newAR = newSize.x / newSize.y;
      Vector2 oldNewSize = newSize;

      if (newAR < ar)
      {
         if ((newSize.x < size.x) && !(mSizingState & SizingBottom) && !(mSizingState & SizingTop))
            newSize.y *= newAR / ar;
         else
            newSize.x *= ar / newAR;
      }

      else
      {
         if ((newSize.y < size.y) && !(mSizingState & SizingLeft) && !(mSizingState & SizingRight))
            newSize.x *= ar / newAR;
         else
            newSize.y *= newAR / ar;
      }
      
      if( mSizingState & SizingLeft )
      {
         newPosition.x -= (newSize.x - oldNewSize.x) * 0.5f;
      }
      else if( mSizingState & SizingRight )
      {
         newPosition.x += (newSize.x - oldNewSize.x) * 0.5f;
      }

      if( mSizingState & SizingTop )
      {
         newPosition.y -= (newSize.y - oldNewSize.y) * 0.5f;
      }
      else if( mSizingState & SizingBottom )
      {
         newPosition.y += (newSize.y - oldNewSize.y) * 0.5f;
      }
   }

   // Uniform
   if (uniform)
      newPosition = pos;
}

//-----------------------------------------------------------------------------
// nudge
//-----------------------------------------------------------------------------
void LevelBuilderBaseEditTool::nudge(Vector2 pos, S32 directionX, S32 directionY, bool fast, Vector2& newPos)
{
   PROFILE_SCOPE(LevelBuilderBaseEditTool_nudge);
   newPos = pos;
   Vector2 kDirection((F32)directionX, (F32)directionY);

   if (fast)
      kDirection *= 10.0f;
   
   newPos += kDirection;
}

//-----------------------------------------------------------------------------
// getSizingState
//-----------------------------------------------------------------------------
S32 LevelBuilderBaseEditTool::getSizingState(LevelBuilderSceneWindow* sceneWindow, const Point2I &pt, const RectF &rect)
{
   PROFILE_SCOPE(LevelBuilderBaseEditTool_getSizingState);
   if( !rect.isValidRect() )
      return SizingNone;

   Vector2 upperLeft = Vector2( rect.point.x, rect.point.y + rect.extent.y );
   Vector2 lowerRight = Vector2( rect.point.x + rect.extent.x, rect.point.y );

   // Convert to window coords.
   Vector2 windowUpperLeft, windowLowerRight;
   sceneWindow->sceneToWindowPoint(upperLeft, windowUpperLeft);
   sceneWindow->sceneToWindowPoint(lowerRight, windowLowerRight);

   RectI box = RectI(S32(windowUpperLeft.x), S32(windowUpperLeft.y),
                     S32(windowLowerRight.x - windowUpperLeft.x),
                     S32(windowLowerRight.y - windowUpperLeft.y));

   S32 lx = box.point.x, rx = box.point.x + box.extent.x - 1;
   S32 cx = (lx + rx) >> 1;
   S32 ty = box.point.y, by = box.point.y + box.extent.y - 1;
   S32 cy = (ty + by) >> 1;

   if (inNut(pt, lx, ty))
      return SizingLeft | SizingTop;
   if (inNut(pt, cx, ty))
      return SizingTop;
   if (inNut(pt, rx, ty))
      return SizingRight | SizingTop;
   if (inNut(pt, lx, by))
      return SizingLeft | SizingBottom;
   if (inNut(pt, cx, by))
      return SizingBottom;
   if (inNut(pt, rx, by))
      return SizingRight | SizingBottom;
   if (inNut(pt, lx, cy))
      return SizingLeft;
   if (inNut(pt, rx, cy))
      return SizingRight;

   return SizingNone;
}

//-----------------------------------------------------------------------------
// drawSizingNuts
//-----------------------------------------------------------------------------
void LevelBuilderBaseEditTool::drawSizingNuts(LevelBuilderSceneWindow* sceneWindow, const RectF& rect)
{
   PROFILE_SCOPE(LevelBuilderBaseEditTool_drawSizingNuts);

   Vector2 upperLeft = Vector2( rect.point.x, rect.point.y + rect.extent.y );
   Vector2 lowerRight = Vector2( rect.point.x + rect.extent.x, rect.point.y );

   // Convert to window coords.
   Vector2 windowUpperLeft, windowLowerRight;
   sceneWindow->sceneToWindowPoint(upperLeft, windowUpperLeft);
   sceneWindow->sceneToWindowPoint(lowerRight, windowLowerRight);
   windowUpperLeft = sceneWindow->localToGlobalCoord(Point2I(S32(windowUpperLeft.x), S32(windowUpperLeft.y)));
   windowLowerRight = sceneWindow->localToGlobalCoord(Point2I(S32(windowLowerRight.x), S32(windowLowerRight.y)));

   RectI selectionRect = RectI(S32(windowUpperLeft.x), S32(windowUpperLeft.y),
                               S32(windowLowerRight.x - windowUpperLeft.x),
                               S32(windowLowerRight.y - windowUpperLeft.y));

   // Top Left Sizing Knob
   drawNut( selectionRect.point);
   // Middle Left Sizing Knob
   drawNut( Point2I( selectionRect.point.x, selectionRect.point.y + ( selectionRect.extent.y / 2 ) ));
   // Bottom Left Sizing Knob
   drawNut( Point2I( selectionRect.point.x, selectionRect.point.y + selectionRect.extent.y ));
   // Bottom Right Sizing Knob
   drawNut( selectionRect.point + selectionRect.extent);
   // Middle Right Sizing Knob
   drawNut( Point2I( selectionRect.point.x + selectionRect.extent.x , selectionRect.point.y + ( selectionRect.extent.y / 2 ) ));
   // Top Right Sizing Knob
   drawNut( Point2I( selectionRect.point.x + selectionRect.extent.x , selectionRect.point.y ));
   // Top Middle Sizing Knob
   drawNut( Point2I( selectionRect.point.x + ( selectionRect.extent.x / 2) , selectionRect.point.y ));
   // Bottom Middle Sizing Knob
   drawNut( Point2I( selectionRect.point.x + ( selectionRect.extent.x / 2) , selectionRect.point.y + selectionRect.extent.y ));
}

#endif // TORQUE_TOOLS
