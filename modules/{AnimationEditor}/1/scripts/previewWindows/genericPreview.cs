//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

/// @class GenericPreviewWindow
/// This class is a SceneWindow that displays a single animation or static sprite.
/// After creating a SceneWindow and assigning it this class, call
/// GenericPreviewWindow::display, passing it the animation or sprite you 
/// would like to display.
/// 
/// @field scene The scene that renders the preview.
/// @field sprite The sprite that renders the animation/image map.
/// @field baseDimension The length of the shorter side of the camera.
/// @field resource The animation or image map datablock being previewed.

/// void(GenericPreviewWindow this)
/// Initializes various properties necessary for the window.
/// @param this The GenericPreviewWindow.
function GenericPreviewWindow::onAdd(%this)
{
   Assert(%this.getClassName() $= "SceneWindow",
             "GenericPreviewWindow::onAdd - This is not a SceneWindow!");
   
   %this.scene = new Scene();
   %this.baseDimension = 100;
   %this.resource = "";
   %this.frame = 0;
   
   %this.setScene(%this.scene);
}

/// void(GenericPreviewWindow this)
/// Cleans up data allocated for this window.
/// @param this The GenericPreviewWindow.
function GenericPreviewWindow::onRemove(%this)
{
   if (isObject(%this.scene))
      %this.scene.delete();
      
   if (isObject(%this.sprite))
      %this.sprite.delete();
}

/// void(GenericPreviewWindow this)
/// Called when the window is first shown to make sure things are properly
/// sized.
/// @param this The GenericPreviewWindow.
function GenericPreviewWindow::onWake(%this)
{
   %this.onExtentChange(%this.position SPC %this.extent);
}

/// void(GenericPreviewWindow this, RectF newDimensions)
/// Resizes the scenes camera to maintain the correct aspect ratio.
/// @param this The GenericPreviewWindow.
/// @param newDimensions The new position and size of the window.
function GenericPreviewWindow::onExtentChange(%this, %newDimensions)
{
   %width = getWord(%newDimensions, 2);
   %height = getWord(%newDimensions, 3);
   
   %dimensions = resolveAspectRatio(%width / %height, %this.baseDimension);
   %cameraWidth = getWord(%dimensions, 0);
   %cameraHeight = getWord(%dimensions, 1);
   
   %this.setCurrentCameraPosition(0, 0, %cameraWidth, %cameraHeight);
}

/// void(GenericPreviewWindow this, ImageAsset imageMap)
/// Displays the specified image map in this window.
/// @param this The GenericPreviewWindow.
/// @param animation The animation to display.
function GenericPreviewWindow::display(%this, %resource, %type)
{  
   if (isObject(%this.sprite))
      %this.sprite.delete();
      
   %this.resource = %resource;
   %this.objectType = %type;
   %this.update();
}

/// void(GenericPreviewWindow this)
/// Clears the display.
/// @param this The GenericPreviewWindow.
function GenericPreviewWindow::clear(%this)
{
   %this.resource = "";
   %this.update();
}

/// void(GenericPreviewWindow this)
/// Updates the displayed animation.
/// @param this The GenericPreviewWindow.
/// @todo This doesn't take advantage of width or height when displaying
/// non-square animations.
function GenericPreviewWindow::update(%this)
{
   if (!isObject(%this.resource))
   {
      if (isObject(%this.sprite))
         %this.sprite.setVisible(false);
      
      return;
   }
   
   if(isObject(%this.Sprite))
      %this.sprite.setVisible(true);
   
   switch$(%this.objectType)
   {
      case "t2dStaticSprite":
         %this.sprite = new t2dStaticSprite ();
         %this.sprite.scene = %this.scene;
         %size = %this.resource.getFrameSize(0);
         %this.sprite.setImageMap(%this.resource);
         %this.sprite.setFrame(%this.frame);
         
      case "t2dAnimatedSprite":
         %this.sprite = new t2dAnimatedSprite();
         %this.sprite.scene = %this.scene;
         %size = %this.resource.imageMap.getFrameSize(0);
         %this.sprite.playAnimation(%this.resource);
         
      case "BitmapFontObject":
         %this.sprite = new BitmapFontObject();
         %this.sprite.scene = %this.scene;
         %this.sprite.setImageMap(%this.resource);
         %size = %this.resource.getFrameSize(0);
   }
   
   %width = getWord(%size, 0);
   %height = getWord(%size, 1);
   
   // Camera size.
   %cameraSize = %this.getCurrentCameraSize();
   %cameraWidth = getWord(%cameraSize, 0);
   %cameraHeight = getWord(%cameraSize, 1);
   
   // Determine the amount to scale the sprite so it fills the camera.
   %widthScale = %cameraWidth / %width;
   %heightScale = %cameraHeight / %height;
   %scale = %widthScale < %heightScale ? %widthScale : %heightScale;
   
   %width *= %scale;
   %height *= %scale;
   
   if(%this.objectType !$= "BitmapFontObject")
      %this.sprite.size = %width SPC %height;
}

/// void(GenericPreviewWindow this)
/// Pauses the animation that is playing.
/// @param this The AnimationPreviewWindow.
function GenericPreviewWindow::pause(%this)
{
   if (isObject(%this.sprite.getAnimation()))
   {
      %this.sprite.pauseAnimation(true);
      %this.sprite.paused = true;
   }
}

/// void(GenericPreviewWindow this)
/// Plays the animation.
/// @param this The AnimationPreviewWindow.
function GenericPreviewWindow::play(%this)
{
    if (isObject(%this.sprite.getAnimation()))
    {
        if (%this.sprite.paused)
        {
            %this.sprite.pauseAnimation(false);
            %this.sprite.paused = false;
            %this.sprite.playAnimation(%this.animation, true);
        }
        else
        {
            %this.sprite.pauseAnimation(false);
            %this.sprite.paused = false;
            %this.sprite.playAnimation(%this.animation, true);
        }
    }
}
