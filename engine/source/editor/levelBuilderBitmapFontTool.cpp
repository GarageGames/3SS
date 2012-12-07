//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef TORQUE_PLAYER


#include "console/console.h"
#include "editor/LevelBuilderBitmapFontTool.h"

// Implement Our Console Object
IMPLEMENT_CONOBJECT(LevelBuilderBitmapFontTool);

LevelBuilderBitmapFontTool::LevelBuilderBitmapFontTool() : LevelBuilderCreateTool(),
    mImageMapName(NULL),
    mText("")
{
    // Set our tool name
    mToolName      = StringTable->insert("Bitmap Font Tool");
}

LevelBuilderBitmapFontTool::~LevelBuilderBitmapFontTool()
{
}

SceneObject* LevelBuilderBitmapFontTool::createObject()
{
    BitmapFontObject* bitmapFontObject = dynamic_cast<BitmapFontObject*>(ConsoleObject::create("BitmapFontObject"));

    if (bitmapFontObject)
    {
        bitmapFontObject->setImageMap(mImageMapName);
    }

    return bitmapFontObject;
}

Point2I LevelBuilderBitmapFontTool::getPixelSize()
{
    BitmapFontObject* bitmapFontObject = dynamic_cast<BitmapFontObject*>(mCreatedObject);
    if (bitmapFontObject)
    {
        ImageAsset* imageMap = dynamic_cast<ImageAsset*>(Sim::findObject(bitmapFontObject->getImageMap()));
        if (imageMap)
        {
            const ImageAsset::FrameArea::PixelArea& pixelArea = imageMap->getImageFrameArea(0).mPixelArea;
            return Point2I(pixelArea.mPixelWidth, pixelArea.mPixelHeight);
        }
    }

    return Parent::getPixelSize();
}

ConsoleMethod(LevelBuilderBitmapFontTool, setImageMap, void, 3, 3, "Sets the image map for the created bitmap font object.")
{
    ImageAsset* imageMap = dynamic_cast<ImageAsset*>(Sim::findObject(argv[2]));
    if (imageMap)
    {
        object->setImageMapName(argv[2]);
    }
    else
        Con::warnf("LevelBuilderBitmapFontTool::setImageMap - Invalid image map: %s", argv[2]);
}

void LevelBuilderBitmapFontTool::setImageMapName( const char* name )
{
    mImageMapName = StringTable->insert(name); 
};

ConsoleMethod(LevelBuilderBitmapFontTool, setBitmapText, void, 3, 3, "Sets the text for a bitmap font object.")
{
    object->setText(argv[2]);
}

void LevelBuilderBitmapFontTool::setText( const char* text )
{
    mText.set(text);
};



#endif // TORQUE_TOOLS
