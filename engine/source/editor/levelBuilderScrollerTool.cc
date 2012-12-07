//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef TORQUE_PLAYER


#include "console/console.h"
#include "editor/levelBuilderScrollerTool.h"

// Implement Our Console Object
IMPLEMENT_CONOBJECT(LevelBuilderScrollerTool);

LevelBuilderScrollerTool::LevelBuilderScrollerTool() : LevelBuilderCreateTool(),
                                                       mImageMapName(NULL)
{
   // Set our tool name
   mToolName = StringTable->insert("Scroller Tool");
}

LevelBuilderScrollerTool::~LevelBuilderScrollerTool()
{
}

SceneObject* LevelBuilderScrollerTool::createObject()
{
   Scroller* scroller = dynamic_cast<Scroller*>(ConsoleObject::create("Scroller"));

   if (scroller)
      static_cast<SpriteProxyBase*>(scroller)->setImage(mImageMapName);

   return scroller;
}

Point2I LevelBuilderScrollerTool::getPixelSize()
{
   Scroller* scroller = dynamic_cast<Scroller*>(mCreatedObject);
   if (scroller)
   {
      // No way to get size from the actual object. We'll do it this way for now.
      ImageAsset* imageMap = dynamic_cast<ImageAsset*>(Sim::findObject(mImageMapName));
      if (imageMap)
      {
          const ImageAsset::FrameArea::PixelArea& pixelArea = imageMap->getImageFrameArea(0).mPixelArea;
          return Point2I(pixelArea.mPixelWidth, pixelArea.mPixelHeight);
      }
   }

   return Parent::getPixelSize();
}

ConsoleMethod(LevelBuilderScrollerTool, setImageMap, void, 3, 3, "Sets the image map for the created scrollers.")
{
   ImageAsset* imageMap = dynamic_cast<ImageAsset*>(Sim::findObject(argv[2]));
   if (imageMap)
      object->setImageMapName(argv[2]);
   else
      Con::warnf("LevelBuilderScrollerTool::setImageMap - Invalid image map: %s", argv[2]);;
}


#endif // TORQUE_TOOLS
