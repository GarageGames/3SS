//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

/// @class ABImageMapPreviewWindow
/// This class must be derived from ImageMapPreviewWindow. It provides drag and
/// drop functionality for the animation builder.

/// void(ABImageMapPreviewWindow this)
/// Assigns a class to the sprites created by the preview so they can be
/// dragged onto the animation builder's storyboard.
/// @param this The SceneWindow.
function ABImageMapPreviewWindow::onAdd(%this)
{
   Assert(%this.superclass $= "ImageMapPreviewWindow",
             "ABImageMapPreviewWindow::onAdd - This class is designed to subclass ImageMapPreviewWindow!");
   
   Parent::onAdd(%this);
   %this.spriteClass = "ABImageMapPreviewSprite";
}

/// void(ABImageMapPreviewSprite this, ModifierKey modifier, Point2F position, int clicks)
/// When double clicking, appends the frame represented by this sprite to the
/// animation builder storyboard.
/// @param modifier A number representing the modifier keys that are being pressed.
/// @param position The position in the scene of the mouse.
/// @param clicks The number of times the mouse has been clicked in recent succession.
function ABImageMapPreviewSprite::onTouchDown(%this, %modifier, %position, %clicks)
{
   if (%clicks == 2)
      AnimationBuilder.appendFrame(%this.frame);
}

/// <summary>
/// void(ABImageMapPreviewSprite this, ModifierKey modifier, Point2F position, int clicks)
/// Creates a drag and drop control through the animation builder.
/// </summary>
/// <param name="modifier"> A number representing the modifier keys that are being pressed.</param>
/// <param name="position"> The position in the scene of the mouse.</param>
/// <param name="clicks"> The number of times the mouse has been clicked in recent succession.</param>
function ABImageMapPreviewSprite::onTouchDragged(%this, %modifier, %position, %clicks)
{
    %upperLeft = getWords(%this.getArea(), 0, 1);
    %windowPos = %this.window.getWindowPoint(%upperLeft);
    %spritePosition = %this.window.getCanvasPoint(%windowPos);
    %windowPos = %this.window.getWindowPoint(%position);
    %mousePosition = %this.window.getCanvasPoint(%windowPos);

    %origin = %this.window.getWindowPoint("0 0");
    %size = %this.window.getWindowPoint(%this.size);
    %size = Vector2Sub(%size, %origin);

    // Round these off.
    %spritePosition = mFloatLength(getWord(%spritePosition, 0), 0) SPC mFloatLength(getWord(%spritePosition, 1), 0);
    %mousePosition = mFloatLength(getWord(%mousePosition, 0), 0) SPC mFloatLength(getWord(%mousePosition, 1), 0);
    %size = mFloatLength(getWord(%size, 0), 0) SPC mFloatLength(getWord(%size, 1), 0);

    AnimationBuilder.createDraggingControl(%this, %spritePosition, %mousePosition, %size);
}
