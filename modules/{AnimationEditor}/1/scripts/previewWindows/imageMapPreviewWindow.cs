//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

/// @class ImageMapPreviewWindow
/// This class is a SceneWindow that displays a single image map. It will
/// automatically manage displaying each of the frames in a different sprite
/// at the correct aspect ratio. After creating a SceneWindow and assigning
/// it this class, call ImageMapPreviewWindow::display, passing it the image
/// map you would like to display. To update the display in the case of a
/// property change on the image map, call ImageMapPreviewWindow::update.
/// 
/// Sprites in the window have mouse events enabled, as does the window itself.
/// The sprites can be assigned a class by setting the 'spriteClass' field on
/// the window. The sprites also have a reference to the window that created
/// them so positional data can be properly extracted. This is on the 'window'
/// field. This should allow any sort of custom funniness that needs to go
/// on within the window.
/// 
/// @field scene The scene that renders the preview.
/// @field staticSpriteGroup A SimGroup that stores the t2dStaticSprites that
/// display the frames of the image.
/// @field spacing The amount of space between each frame of the image map.
/// @field baseDimension The length of the shorter side of the camera.
/// @field imageMap The image map being displayed display.

/// void(ImageMapPreviewWindow this)
/// Initializes various properties necessary for the window.
/// @param this The ImageMapPreviewWindow.
function ImageMapPreviewWindow::onAdd(%this)
{
   Assert(%this.getClassName() $= "SceneWindow",
             "ImageMapPreviewWindow::onAdd - This is not a SceneWindow!");
   
   %this.scene = new Scene();
   %this.staticSpriteGroup = new SimGroup();
   %this.spacing = 2;
   %this.baseDimension = 100;
   %this.imageMap = "";
   %this.UseObjectInputEvents = true;
   %this.UseWindowInputEvents = true;
   %this.spriteClass = "";
   
   %this.setScene(%this.scene);
}

/// void(ImageMapPreviewWindow this)
/// Cleans up data allocated for this window.
/// @param this The ImageMapPreviewWindow.
function ImageMapPreviewWindow::onRemove(%this)
{
    if (isObject(%this.imageMap))
        AssetDatabase.releaseAsset(%this.imageMap);
   
   if (isObject(%this.scene))
      %this.scene.delete();
      
   if (isObject(%this.staticSpriteGroup))
      %this.staticSpriteGroup.delete();
}

/// void(ImageMapPreviewWindow this)
/// Called when the window is first shown to make sure things are properly
/// sized.
/// @param this The ImageMapPreviewWindow.
function ImageMapPreviewWindow::onWake(%this)
{
   // rdbnote: this doesn't actually do anything because we don't have an image
   %this.updateSize();
}

/// void(ImageMapPreviewWindow this, RectF newDimensions)
/// Resizes the scenes camera to maintain the correct aspect ratio.
/// @param this The ImageMapPreviewWindow.
/// @param newDimensions The new position and size of the window.
function ImageMapPreviewWindow::onExtentChange(%this, %newDimensions)
{
   %this.updateSize();
}

/// void(ImageMapPreviewWindow this, ImageAsset imageMap)
/// Displays the specified image map in this window.
/// @param this The ImageMapPreviewWindow.
/// @param imageMap The image map to display.
function ImageMapPreviewWindow::display(%this, %imageMap)
{
   Assert(AssetDatabase.isDeclaredAsset(%imageMap), "ImageMapPreviewWindow::display - Object is not a valid image map asset!");
   
   %this.assetId = %imageMap;
   %this.imageMap = AssetDatabase.acquireAsset(%imageMap);
   %this.update();
}

/// void(ImageMapPreviewWindow this)
/// Clears the display.
/// @param this The ImageMapPreviewWindow.
function ImageMapPreviewWindow::clear(%this)
{
   %this.imageMap = "";
   %this.update();
}

/// void(ImageMapPreviewWindow this)
/// Updates the displayed image.
/// @param this The ImageMapPreviewWindow.
/// @todo This currently does not handle "LINK" or "KEY" mode image maps.
/// @todo This doesn't take advantage of width or height when displaying
/// non-square image maps.
function ImageMapPreviewWindow::update(%this)
{
   // Clear out the old stuff.
   %this.staticSpriteGroup.deleteContents();
   
   %imageMap = %this.imageMap;
   %scene = %this.scene;
   
   // Nothing doing if we don't have an image.
   if (!isObject(%imageMap))
      return;
      
   // because we need to make sure the camera size is right
   %this.updateSize();
   
  %scene.setDebugOn(2, 3);
   
   %cameraSize = %this.getCurrentCameraSize();
   %maxWidth = getWord(%cameraSize, 0) - 1;
   %maxHeight = getWord(%cameraSize, 1) - 1;
   
  %baseX = 0;
  %baseY = 0;
  %frameCount = %imageMap.getFrameCount();
  
  %sqrt = mSqrt(%frameCount);
  %div = mCeil(%sqrt);
  
  %rowSpace = (%maxWidth / %div) * 0.05;
  %colSpace = (%maxHeight / %div) * 0.05;
  if (%rowSpace < 1.5)
     %rowSpace = 1.5;
  if (%colSpace < 1.5)
     %colSpace = 1.5;
     
  %objWidth = (%maxWidth / %div) - (%rowSpace + %this.spacing);
  %objHeight = (%maxHeight / %div) - (%colSpace + %this.spacing);
  
  %baseX += %this.spacing;
  %baseY += %this.spacing;
  
  %posX = %baseX - (%maxWidth / 2) + (%objWidth / 2);
  %posY = %baseY - (%maxHeight / 2) + (%objHeight / 2);
  
  %rowCount = 0;
  %colCount = 0;
  
  for (%i = 0; %i < %frameCount; %i++)
  {
     %sprite = new sprite()
     {
        class = %this.spriteClass;
        superClass = ImageMapPreviewSprite;
        scene = %scene;
        UseInputEvents = true;
        window = %this;
        tooltipprofile="GuiToolTipProfile";
        ToolTip="Each frame is displayed in order from left to right starting in the top left corner. You can drag and drop individual frames to the Animation Timeline.";
     };
     
     %sprite.setImageMap(%this.assetId, %i);
     %sprite.setPosition(%posX, %posY);
     %sprite.setSize(%objWidth, %objHeight);
     
     %this.staticSpriteGroup.add(%sprite);
     
     if (%colCount >= %div - 1)
     {
        %rowCount++;
        %colCount = 0;
        %posX = %baseX - (%maxWidth / 2) + (%objWidth / 2);
        %posY = %baseY - (%maxHeight / 2) + (%objHeight / 2) + ((%objHeight + %rowSpace + %this.spacing) * %rowCount);
     }
     else
     {
        %colCount++;
        %posX = %baseX - (%maxWidth / 2) + (%objWidth / 2) + ((%objWidth + %colSpace + %this.spacing) * %colCount);
     }
  }
}

/// void()
/// Updates the camera size to display images at the correct aspect ratio.
function ImageMapPreviewWindow::updateSize(%this)
{
   if(!isObject(%this.imageMap))
      return;
   
   // Grab some size info.
   %windowSize = %this.getExtent();
   %imageSize = %this.imageMap.getImageSize();
      
   %windowAR = getWord(%windowSize, 0) /  getWord(%windowSize, 1);
   %imageAR = getWord(%imageSize, 0) / getWord(%imageSize, 1);
      
   // Basically, instead of sizing the images correctly, we're sizing the
   // camera at the inverse of the image's aspect ratio, thereby achieving the
   // same affect.
   %newSize = resolveAspectRatio(%windowAR / %imageAR, %this.baseDimension);
   %this.setCurrentCameraPosition("0 0", %newSize);
}
